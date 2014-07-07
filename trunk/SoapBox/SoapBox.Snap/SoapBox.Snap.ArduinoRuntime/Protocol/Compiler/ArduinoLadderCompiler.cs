#region "SoapBox.Snap License"
/// <header module="SoapBox.Snap"> 
/// Copyright (C) 2009-2014 SoapBox Automation, All Rights Reserved.
/// Contact: SoapBox Automation Licencing (license@soapboxautomation.com)
/// 
/// This file is part of SoapBox Snap.
/// 
/// SoapBox Snap is free software: you can redistribute it and/or modify it
/// under the terms of the GNU General Public License as published by the 
/// Free Software Foundation, either version 3 of the License, or 
/// (at your option) any later version.
/// 
/// SoapBox Snap is distributed in the hope that it will be useful, but 
/// WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
/// GNU General Public License for more details.
/// 
/// You should have received a copy of the GNU General Public License along
/// with SoapBox Snap. If not, see <http://www.gnu.org/licenses/>.
/// </header>
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SoapBox.Protocol.Automation;
using SoapBox.Protocol.Base;
using SoapBox.Snap.ArduinoRuntime.Protocol.Compiler.Helpers;

namespace SoapBox.Snap.ArduinoRuntime.Protocol.Compiler
{
    class ArduinoLadderCompiler
    {
        private readonly InformationResponse m_information;

        public ArduinoLadderCompiler(InformationResponse information)
        {
            if (information == null) throw new ArgumentNullException("information");
            this.m_information = information;
        }

        internal const byte PROGRAM_END = 0xFF;

        public CompiledProgram Compile(NodeRuntimeApplication rta, SignalTable signalTable)
        {
            if (rta == null) throw new ArgumentNullException("rta");
            if (signalTable == null) throw new ArgumentNullException("signalTable");
            var runtimeId = Guid.Parse(rta.RuntimeId.ToString());
            var versionId = Guid.Parse(rta.ID.ToString());

            var logic = rta.Logic;
            var compiledInstructions = new List<CompiledInstruction>();

            var discreteInputs = rta.DeviceConfiguration.GetChildrenRecursive()
                .Select(x => x.Value)
                .OfType<NodeDiscreteInput>();
            foreach (var discreteInput in discreteInputs)
            {
                if (discreteInput.Forced.BoolValue)
                {
                    var address = Int16.Parse(discreteInput.Address.ToString());
                    this.driveSignalWithCoilToConstantValue(
                        address, discreteInput.ForcedValue.BoolValue,
                        signalTable, compiledInstructions);
                }
            }

            var analogInputs = rta.DeviceConfiguration.GetChildrenRecursive()
                .Select(x => x.Value)
                .OfType<NodeAnalogInput>();
            foreach (var analogInput in analogInputs)
            {
                if (analogInput.Forced.BoolValue)
                {
                    if (analogInput.ForcedValue.DataType != FieldDataType.DataTypeEnum.NUMBER)
                    {
                        throw new Exception("Data type of forced value must be NUMBER.");
                    }
                    var address = byte.Parse(analogInput.Address.ToString());
                    this.setNumericAddressToValueWithAddInstruction(
                        address, (decimal)(analogInput.ForcedValue.Value),
                        signalTable, compiledInstructions);
                }
            }

            compiledInstructions.AddRange(compilePageCollection(logic, signalTable));

            // discrete output literals
            var discreteOutputs = rta.DeviceConfiguration.GetChildrenRecursive()
                .Select(x => x.Value)
                .OfType<NodeDiscreteOutput>();
            foreach (var discreteOutput in discreteOutputs)
            {
                var literal = discreteOutput.SignalIn.Literal;
                var address = Int16.Parse(discreteOutput.Address.ToString());
                if (literal != null)
                {
                    // insert a new rung to set the literal value
                    if (literal.DataType != FieldDataType.DataTypeEnum.BOOL)
                    {
                        throw new Exception("Data type must be BOOL.");
                    }
                    var literalValue = (bool)(literal.Value);

                    this.driveSignalWithCoilToConstantValue(
                        address, literalValue, 
                        signalTable, compiledInstructions);
                }
            }

            // boolean jumpers (from internal signals to outputs)
            foreach (var booleanJumper in signalTable.BooleanJumpers)
            {
                var compiledContactSignal = new CompiledBooleanSignal(true, booleanJumper.Item1, signalTable.BooleanAddressBits);
                var contact = new CompiledInstruction(
                    0x01, // NO contact
                    3,
                    compiledContactSignal);
                compiledInstructions.Add(contact);

                var compiledDiscreteOutputSignal = new CompiledBooleanSignal(false, booleanJumper.Item2, signalTable.BooleanAddressBits);
                var coil = new CompiledInstruction(
                    0x00, // coil
                    3,
                    compiledDiscreteOutputSignal);
                compiledInstructions.Add(coil);
                compiledInstructions.Add(new CompiledInstruction(0x03, 3)); // series instruction end (end of rung)
            }

            // boolean forces
            foreach (var discreteOutput in discreteOutputs)
            {
                var address = Int16.Parse(discreteOutput.Address.ToString());
                if (discreteOutput.Forced.BoolValue)
                {
                    this.driveSignalWithCoilToConstantValue(
                        address, discreteOutput.ForcedValue.BoolValue,
                        signalTable, compiledInstructions);
                }
            }

            // analog output literals
            var analogOutputs = rta.DeviceConfiguration.GetChildrenRecursive()
                .Select(x => x.Value)
                .OfType<NodeAnalogOutput>();
            foreach (var analogOutput in analogOutputs)
            {
                var literal = analogOutput.SignalIn.Literal;
                var address = Byte.Parse(analogOutput.Address.ToString());
                if (literal != null)
                {
                    // insert a new rung to set the literal value
                    if (literal.DataType != FieldDataType.DataTypeEnum.NUMBER)
                    {
                        throw new Exception("Data type of literal must be NUMBER.");
                    }
                    this.setNumericAddressToValueWithAddInstruction(
                        address, (decimal)(literal.Value),
                        signalTable, compiledInstructions);
                }
            }

            // analog jumpers (from internal signals to outputs)
            foreach (var numericJumper in signalTable.NumericJumpers)
            {
                throw new NotImplementedException("Numeric output jumpers (2 analog outputs mapped to same signal) not implemented");
            }

            // analog forces
            foreach (var analogOutput in analogOutputs)
            {
                var address = Byte.Parse(analogOutput.Address.ToString());
                if (analogOutput.Forced.BoolValue)
                {
                    if (analogOutput.ForcedValue.DataType != FieldDataType.DataTypeEnum.NUMBER)
                    {
                        throw new Exception("Data type of forced value must be NUMBER.");
                    }
                    this.setNumericAddressToValueWithAddInstruction(
                        address, (decimal)(analogOutput.ForcedValue.Value),
                        signalTable, compiledInstructions);
                }
            }

            // end of program
            compiledInstructions.Add(new CompiledInstruction(PROGRAM_END,8));

            var packedInstructions = bitPackInstructions(compiledInstructions);

            return new CompiledProgram(
                runtimeId,
                versionId,
                signalTable.BooleanAddressBits,
                signalTable.NumericAddressBits,
                packedInstructions);
        }

        private void setNumericAddressToValueWithAddInstruction(
            byte address, decimal value,
            SignalTable signalTable, List<CompiledInstruction> compiledInstructions)
        {
            var compiledOperand1Signal = new CompiledNumericSignal(value);
            var operand2SignalIn = NodeSignalIn.BuildWith(
                new FieldDataType(FieldDataType.DataTypeEnum.NUMBER),
                new FieldConstant(FieldDataType.DataTypeEnum.NUMBER, 0M));
            var compiledOperand2Signal = numericSignal(operand2SignalIn, signalTable);

            var compiledResultSignal = new CompiledNumericSignal(false, address, signalTable.NumericAddressBits);
            var add = new CompiledInstruction(
                0x78, // add
                7,
                compiledOperand1Signal, compiledOperand2Signal, compiledResultSignal);
            compiledInstructions.Add(add);
            compiledInstructions.Add(new CompiledInstruction(0x03, 3)); // series instruction end (end of rung)
        }

        private void driveSignalWithCoilToConstantValue(
            Int16 address, bool value,
            SignalTable signalTable, List<CompiledInstruction> compiledInstructions)
        {
            if (!value) // if value is TRUE, only need the coil, otherwise need a NO contact
            {
                var compiledContactSignal = new CompiledBooleanSignal(false);
                var contact = new CompiledInstruction(
                    0x01, // NO contact
                    3,
                    compiledContactSignal);
                compiledInstructions.Add(contact);
            }

            var compiledDiscreteInputSignal = new CompiledBooleanSignal(false, address, signalTable.BooleanAddressBits);
            var coil = new CompiledInstruction(
                0x00, // coil
                3,
                compiledDiscreteInputSignal);
            compiledInstructions.Add(coil);
            compiledInstructions.Add(new CompiledInstruction(0x03, 3)); // series instruction end (end of rung)
        }

        private byte[] bitPackInstructions(IEnumerable<CompiledInstruction> compiledInstructions)
        {
            var result = new List<byte>();
            var bits = 0;
            var shifter = 0;
            foreach (var compiledInstruction in compiledInstructions)
            {
                foreach (var bit in compiledInstruction.ToBits())
                {
                    shifter = shifter << 1;
                    bits++;
                    if (bit == true)
                    {
                        shifter = shifter | 1;
                    }
                    if (bits == 8)
                    {
                        result.Add((byte)shifter);
                        shifter = 0;
                        bits = 0;
                    }
                }
            }
            if(bits > 0) 
            {
                while (bits < 8)
                {
                    shifter = shifter << 1;
                    bits++;
                }
                result.Add((byte)shifter);
            }
            return result.ToArray();
        }

        private IEnumerable<CompiledInstruction> compilePageCollection(
            NodePageCollection pageCollection, 
            SignalTable signalTable)
        {
            if (pageCollection == null) throw new ArgumentNullException("pageCollection");
            if (signalTable == null) throw new ArgumentNullException("signalTable");
            var result = new List<CompiledInstruction>();

            foreach (var subFolder in pageCollection.NodePageCollectionChildren)
            {
                result.AddRange(compilePageCollection(subFolder, signalTable));
            }

            foreach (var page in pageCollection.NodePageChildren)
            {
                result.AddRange(compilePage(page, signalTable));
            }

            return result;
        }

        private IEnumerable<CompiledInstruction> compilePage(
            NodePage page,
            SignalTable signalTable)
        {
            if (page == null) throw new ArgumentNullException("page");
            if (signalTable == null) throw new ArgumentNullException("signalTable");
            var result = new List<CompiledInstruction>();

            foreach (var instructionGroup in page.NodeInstructionGroupChildren) // instructionGroup == rung
            {
                result.AddRange(compileInstructionGroup(instructionGroup, signalTable));
            }

            return result;
        }

        private IEnumerable<CompiledInstruction> compileInstructionGroup(
            NodeInstructionGroup instructionGroup,
            SignalTable signalTable)
        {
            if (instructionGroup == null) throw new ArgumentNullException("instructionGroup");
            if (signalTable == null) throw new ArgumentNullException("signalTable");
            var result = new List<CompiledInstruction>();
            if (instructionGroup.Language.ToString() != 
                SoapBox.Snap.LD.Extensions.Workbench.Documents.PageEditor_.InstructionGroupItems.LD)
            {
                throw new ArgumentOutOfRangeException("Can only compile ladder logic");
            }

            foreach (var instruction in instructionGroup.NodeInstructionChildren) 
            {
                result.AddRange(compileInstruction(instruction, signalTable));
            }

            return result;
        }

        private IEnumerable<CompiledInstruction> compileInstruction(
            NodeInstruction instruction,
            SignalTable signalTable)
        {
            if (instruction == null) throw new ArgumentNullException("instruction");
            if (signalTable == null) throw new ArgumentNullException("signalTable");

            if(instruction.InstructionType.Language.ToString() != 
                SoapBox.Snap.LD.Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD)
            {
                throw new ArgumentOutOfRangeException("Can only compile ladder logic");
            }

            var result = new List<CompiledInstruction>();
            switch (instruction.InstructionType.Code.ToString())
            {
                case SoapBox.Snap.LD.Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.Snap_.Coil:
                    result.Add(compileCoil(instruction, signalTable));
                    break;
                case SoapBox.Snap.LD.Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.Snap_.ContactNO:
                    result.Add(compileContactNO(instruction, signalTable));
                    break;
                case SoapBox.Snap.LD.Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.Snap_.ContactNC:
                    result.Add(compileContactNC(instruction, signalTable));
                    break;
                case SoapBox.Snap.LD.Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.Snap_.Series:
                    result.AddRange(compileSeries(instruction, signalTable));
                    break;
                case SoapBox.Snap.LD.Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.Snap_.RisingEdge:
                    result.Add(compileRisingEdge(instruction, signalTable));
                    break;
                case SoapBox.Snap.LD.Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.Snap_.FallingEdge:
                    result.Add(compileFallingEdge(instruction, signalTable));
                    break;
                case SoapBox.Snap.LD.Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.Snap_.SetReset:
                    result.Add(compileSetReset(instruction, signalTable));
                    break;
                case SoapBox.Snap.LD.Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.Snap_.TmrON:
                    result.Add(compileTmrON(instruction, signalTable));
                    break;
                case SoapBox.Snap.LD.Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.Snap_.TmrOFF:
                    result.Add(compileTmrOFF(instruction, signalTable));
                    break;
                case SoapBox.Snap.LD.Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.Snap_.Parallel:
                    result.AddRange(compileParallel(instruction, signalTable));
                    break;
                case SoapBox.Snap.LD.Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.Snap_.CntUP:
                    result.Add(compileCntUP(instruction, signalTable));
                    break;
                case SoapBox.Snap.LD.Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.Snap_.CntDN:
                    result.Add(compileCntDN(instruction, signalTable));
                    break;
                case SoapBox.Snap.LD.Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.SnapComparison_.Equal:
                    result.Add(compileEqual(instruction, signalTable));
                    break;
                case SoapBox.Snap.LD.Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.SnapComparison_.GreaterThan:
                    result.Add(compileGreaterThan(instruction, signalTable));
                    break;
                case SoapBox.Snap.LD.Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.SnapComparison_.GreaterThanOrEqual:
                    result.Add(compileGreaterThanOrEqual(instruction, signalTable));
                    break;
                case SoapBox.Snap.LD.Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.SnapComparison_.LessThan:
                    result.Add(compileLessThan(instruction, signalTable));
                    break;
                case SoapBox.Snap.LD.Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.SnapComparison_.LessThanOrEqual:
                    result.Add(compileLessThanOrEqual(instruction, signalTable));
                    break;
                case SoapBox.Snap.LD.Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.SnapComparison_.NotEqual:
                    result.Add(compileNotEqual(instruction, signalTable));
                    break;
                case SoapBox.Snap.LD.Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.SnapMath_.Add:
                    result.Add(compileAdd(instruction, signalTable));
                    break;
                case SoapBox.Snap.LD.Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.SnapMath_.Subtract:
                    result.Add(compileSubtract(instruction, signalTable));
                    break;
                case SoapBox.Snap.LD.Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.SnapMath_.Multiply:
                    result.Add(compileMultiply(instruction, signalTable));
                    break;
                case SoapBox.Snap.LD.Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.SnapMath_.Divide:
                    result.Add(compileDivide(instruction, signalTable));
                    break;
                case SoapBox.Snap.LD.Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.SnapMath_.ChooseNumber:
                    result.Add(compileChooseNumber(instruction, signalTable));
                    break;
                default:
                    throw new NotSupportedException("Instruction not supported: " + instruction.InstructionType.Code.ToString());
            }

            return result;
        }

        private CompiledInstruction compileCoil(NodeInstruction instruction, SignalTable signalTable)
        {
            var stateSignal = instruction.NodeSignalChildren[0];
            var compiledStateSignal = booleanSignal(stateSignal, signalTable);
            return new CompiledInstruction(
                0x00,
                3,
                compiledStateSignal);
        }

        private CompiledInstruction compileContactNO(NodeInstruction instruction, SignalTable signalTable)
        {
            return compileContact(instruction, signalTable, 0x01);
        }

        private CompiledInstruction compileContactNC(NodeInstruction instruction, SignalTable signalTable)
        {
            return compileContact(instruction, signalTable, 0x02);
        }

        private CompiledInstruction compileContact(NodeInstruction instruction, SignalTable signalTable, Byte opCode)
        {
            var contactSignalIn = instruction.NodeSignalInChildren[0];
            var compiledContactSignal = booleanSignal(contactSignalIn, signalTable);
            return new CompiledInstruction(
                opCode,
                3,
                compiledContactSignal);
        }

        private IEnumerable<CompiledInstruction> compileSeries(NodeInstruction seriesInstruction, SignalTable signalTable)
        {
            var result = new List<CompiledInstruction>();
            foreach (var instruction in seriesInstruction.NodeInstructionChildren)
            {
                result.AddRange(compileInstruction(instruction, signalTable));
            }
            result.Add(new CompiledInstruction(0x03, 3));
            return result;
        }

        private CompiledInstruction compileRisingEdge(NodeInstruction instruction, SignalTable signalTable)
        {
            return compileEdge(instruction, signalTable, 0x10);
        }

        private CompiledInstruction compileFallingEdge(NodeInstruction instruction, SignalTable signalTable)
        {
            return compileEdge(instruction, signalTable, 0x11);
        }

        private CompiledInstruction compileEdge(NodeInstruction instruction, SignalTable signalTable, Byte opCode)
        {
            var stateSignal = instruction.NodeSignalChildren[0];
            var compiledStateSignal = booleanSignal(stateSignal, signalTable);
            return new CompiledInstruction(
                opCode,
                5,
                compiledStateSignal);
        }

        private CompiledInstruction compileSetReset(NodeInstruction instruction, SignalTable signalTable)
        {
            var resetSignalIn = instruction.NodeSignalInChildren[0];
            var compiledResetSignal = booleanSignal(resetSignalIn, signalTable);
            var stateSignal = instruction.NodeSignalChildren[0];
            var compiledStateSignal = booleanSignal(stateSignal, signalTable);
            return new CompiledInstruction(
                0x12,
                5,
                compiledResetSignal, compiledStateSignal);
        }

        private CompiledInstruction compileTmrON(NodeInstruction instruction, SignalTable signalTable)
        {
            return compileTimer(instruction, signalTable, 0x13);
        }

        private CompiledInstruction compileTmrOFF(NodeInstruction instruction, SignalTable signalTable)
        {
            return compileTimer(instruction, signalTable, 0x14);
        }

        private CompiledInstruction compileTimer(NodeInstruction instruction, SignalTable signalTable, Byte opCode)
        {
            var setpointSignalIn = instruction.NodeSignalInChildren[0];
            var compiledSetpointSignal = numericSignal(setpointSignalIn, signalTable);
            var doneSignal = instruction.NodeSignalChildren[0];
            var compiledDoneSignal = booleanSignal(doneSignal, signalTable);
            var elapsedSignal = instruction.NodeSignalChildren[1];
            var compiledElapsedSignal = numericSignal(elapsedSignal, signalTable);
            return new CompiledInstruction(
                opCode,
                5,
                compiledSetpointSignal, compiledDoneSignal, compiledElapsedSignal);
        }

        private IEnumerable<CompiledInstruction> compileParallel(NodeInstruction parallelInstruction, SignalTable signalTable)
        {
            var result = new List<CompiledInstruction>();
            result.Add(new CompiledInstruction(0x15, 5)); // Parallel Start
            foreach (var instruction in parallelInstruction.NodeInstructionChildren)
            {
                result.AddRange(compileInstruction(instruction, signalTable));
            }
            result.Add(new CompiledInstruction(0x16, 5)); // Parallel End
            return result;
        }

        private CompiledInstruction compileCntUP(NodeInstruction instruction, SignalTable signalTable)
        {
            return compileCounter(instruction, signalTable, 0x2E);
        }

        private CompiledInstruction compileCntDN(NodeInstruction instruction, SignalTable signalTable)
        {
            return compileCounter(instruction, signalTable, 0x2F);
        }

        private CompiledInstruction compileCounter(NodeInstruction instruction, SignalTable signalTable, Byte opCode)
        {
            var setpointSignalIn = instruction.NodeSignalInChildren[0];
            var compiledSetpointSignal = numericSignal(setpointSignalIn, signalTable);
            var resetSignalIn = instruction.NodeSignalInChildren[1];
            var compiledResetSignal = booleanSignal(resetSignalIn, signalTable);

            var doneSignal = instruction.NodeSignalChildren[0];
            var compiledDoneSignal = booleanSignal(doneSignal, signalTable);
            var countSignal = instruction.NodeSignalChildren[1];
            var compiledCountSignal = numericSignal(countSignal, signalTable);
            var stateSignal = instruction.NodeSignalChildren[2];
            var compiledStateSignal = booleanSignal(stateSignal, signalTable);
            return new CompiledInstruction(
                opCode,
                6,
                compiledSetpointSignal, compiledResetSignal,
                compiledDoneSignal, compiledCountSignal, compiledStateSignal);
        }

        private CompiledInstruction compileEqual(NodeInstruction instruction, SignalTable signalTable)
        {
            return compileComparison(instruction, signalTable, 0x18);
        }

        private CompiledInstruction compileGreaterThan(NodeInstruction instruction, SignalTable signalTable)
        {
            return compileComparison(instruction, signalTable, 0x19);
        }

        private CompiledInstruction compileGreaterThanOrEqual(NodeInstruction instruction, SignalTable signalTable)
        {
            return compileComparison(instruction, signalTable, 0x1A);
        }

        private CompiledInstruction compileLessThan(NodeInstruction instruction, SignalTable signalTable)
        {
            return compileComparison(instruction, signalTable, 0x1B);
        }

        private CompiledInstruction compileLessThanOrEqual(NodeInstruction instruction, SignalTable signalTable)
        {
            return compileComparison(instruction, signalTable, 0x1C);
        }

        private CompiledInstruction compileNotEqual(NodeInstruction instruction, SignalTable signalTable)
        {
            return compileComparison(instruction, signalTable, 0x1D);
        }

        private CompiledInstruction compileComparison(NodeInstruction instruction, SignalTable signalTable, Byte opCode)
        {
            var operand1SignalIn = instruction.NodeSignalInChildren[0];
            var compiledOperand1Signal = numericSignal(operand1SignalIn, signalTable);
            var operand2SignalIn = instruction.NodeSignalInChildren[1];
            var compiledOperand2Signal = numericSignal(operand2SignalIn, signalTable);
            return new CompiledInstruction(
                opCode,
                5,
                compiledOperand1Signal, compiledOperand2Signal);
        }

        private CompiledInstruction compileAdd(NodeInstruction instruction, SignalTable signalTable)
        {
            return compileMath(instruction, signalTable, 0x78);
        }

        private CompiledInstruction compileSubtract(NodeInstruction instruction, SignalTable signalTable)
        {
            return compileMath(instruction, signalTable, 0x79);
        }

        private CompiledInstruction compileMultiply(NodeInstruction instruction, SignalTable signalTable)
        {
            return compileMath(instruction, signalTable, 0x7A);
        }

        private CompiledInstruction compileDivide(NodeInstruction instruction, SignalTable signalTable)
        {
            return compileMath(instruction, signalTable, 0x7B);
        }

        private CompiledInstruction compileChooseNumber(NodeInstruction instruction, SignalTable signalTable)
        {
            return compileMath(instruction, signalTable, 0x7C);
        }

        private CompiledInstruction compileMath(NodeInstruction instruction, SignalTable signalTable, Byte opCode)
        {
            var operand1SignalIn = instruction.NodeSignalInChildren[0];
            var compiledOperand1Signal = numericSignal(operand1SignalIn, signalTable);
            var operand2SignalIn = instruction.NodeSignalInChildren[1];
            var compiledOperand2Signal = numericSignal(operand2SignalIn, signalTable);

            var resultSignal = instruction.NodeSignalChildren[0];
            var compiledResultSignal = numericSignal(resultSignal, signalTable);
            return new CompiledInstruction(
                opCode,
                7,
                compiledOperand1Signal, compiledOperand2Signal, compiledResultSignal);
        }

        private CompiledBooleanSignal booleanSignal(NodeSignal signal, SignalTable signalTable)
        {
            var signalAddress = signalTable.GetBooleanSignalAddress(signal);
            var compiledSignal = new CompiledBooleanSignal(false, signalAddress, signalTable.BooleanAddressBits);
            return compiledSignal;
        }

        private CompiledBooleanSignal booleanSignal(NodeSignalIn signalIn, SignalTable signalTable)
        {
            if (signalIn.SignalId != null)
            {
                var signalAddress = signalTable.GetBooleanSignalAddress(signalIn);
                var compiledSignal = new CompiledBooleanSignal(true, signalAddress, signalTable.BooleanAddressBits);
                return compiledSignal;
            }
            else if (signalIn.Literal != null)
            {
                var literal = signalIn.Literal;
                if (literal.DataType != FieldDataType.DataTypeEnum.BOOL)
                {
                    throw new Exception("Signal should be a BOOL.");
                }
                var literalValue = (bool)(literal.Value);
                return new CompiledBooleanSignal(literalValue);
            }
            else
            {
                throw new Exception("Expecting one of SignalIn.SignalId or SignalIn.Literal to be non-null");
            }
        }

        private CompiledNumericSignal numericSignal(NodeSignal signal, SignalTable signalTable)
        {
            var signalAddress = signalTable.GetNumericSignalAddress(signal);
            var compiledSignal = new CompiledNumericSignal(false, signalAddress, signalTable.NumericAddressBits);
            return compiledSignal;
        }

        private CompiledNumericSignal numericSignal(NodeSignalIn signalIn, SignalTable signalTable)
        {
            if (signalIn.SignalId != null)
            {
                var signalAddress = signalTable.GetNumericSignalAddress(signalIn);
                var compiledSignal = new CompiledNumericSignal(true, signalAddress, signalTable.NumericAddressBits);
                return compiledSignal;
            }
            else if (signalIn.Literal != null)
            {
                var literal = signalIn.Literal;
                if (literal.DataType != FieldDataType.DataTypeEnum.NUMBER)
                {
                    throw new Exception("Signal should be a NUMBER.");
                }
                var literalValue = Convert.ToDecimal(literal.Value);
                return new CompiledNumericSignal(literalValue);
            }
            else
            {
                throw new Exception("Expecting one of SignalIn.SignalId or SignalIn.Literal to be non-null");
            }
        }
    }
}
