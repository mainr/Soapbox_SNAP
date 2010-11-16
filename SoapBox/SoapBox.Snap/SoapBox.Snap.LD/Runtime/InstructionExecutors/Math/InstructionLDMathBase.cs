#region "SoapBox.Snap License"
/// <header module="SoapBox.Snap"> 
/// Copyright (C) 2009 SoapBox Automation Inc., All Rights Reserved.
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
using System.ComponentModel.Composition;
using SoapBox.Core;
using SoapBox.Protocol.Automation;
using SoapBox.Snap.Runtime;

namespace SoapBox.Snap.LD.Runtime.InstructionExecutors
{
    class InstructionLDMathBase: AbstractExtension, IInstructionExecutor<InstructionGroupExecutorContextLD>
    {
        public InstructionLDMathBase(FieldInstructionType instructionType)
        {
            this.InstructionType = instructionType;
        }

        public FieldInstructionType InstructionType { get; private set; }
        
        public InstructionGroupExecutorContextLD  ScanInstruction(
            NodeRuntimeApplication rta, NodeInstruction instruction, 
            InstructionGroupExecutorContextLD context)
        {
            // inputs: 0 = input1, 1 = input2
            // outputs: 0 = result

            Decimal input1 = 0; 
            var signalIn = instruction.NodeSignalInChildren[0]; // input1
            var value = signalIn.GetValue(rta);
            if (value != null)
            {
                input1 = Convert.ToDecimal(value.Value);
            }

            Decimal input2 = 0;
            signalIn = instruction.NodeSignalInChildren[1]; // input2
            value = signalIn.GetValue(rta);
            if(value != null)
            {
                input2 = Convert.ToDecimal(value.Value);
            }

            Decimal oldResult = Convert.ToDecimal(instruction.NodeSignalChildren[0].Value);
            Decimal result = ComputeResult(context.RungCondition, input1, input2, oldResult);

            instruction.NodeSignalChildren[0].Value = result;
            return context;
        }

        protected virtual Decimal ComputeResult(
            bool rungIn, Decimal input1, Decimal input2, Decimal oldResult) 
        { return oldResult; }

    }
}
