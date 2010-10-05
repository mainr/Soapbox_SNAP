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
    [Export(ExtensionPoints.Runtime.Snap.GroupExecutors_.LD_.InstructionExecutors, 
        typeof(IInstructionExecutor<InstructionGroupExecutorContextLD>))]
    class InstructionLDTmrOFF: AbstractExtension, IInstructionExecutor<InstructionGroupExecutorContextLD>
    {

        [Import(SoapBox.Snap.Runtime.CompositionPoints.Runtime.Engine, typeof(IEngine))]
        private IEngine engine { get; set; }

        public FieldInstructionType InstructionType
        {
            get 
            { 
                return SoapBox.Snap.LD.InstructionLDTmrOFF.m_InstructionType; 
            }
        }

        
        public InstructionGroupExecutorContextLD  ScanInstruction(
            NodeRuntimeApplication rta, NodeInstruction instruction, 
            InstructionGroupExecutorContextLD context)
        {
            bool newRungOutCondition;
            if (context.RungCondition)
            {
                newRungOutCondition = true;
                instruction.NodeSignalChildren[1].Value = (Decimal)0;
            }
            else
            {
                bool oldRungOut = Convert.ToBoolean(instruction.NodeSignalChildren[0].Value);
                Decimal oldElapsed = Convert.ToDecimal(instruction.NodeSignalChildren[1].Value);

                Decimal setPoint = Decimal.MaxValue; // default to not turning on (for a long time - 50 days?)
                var signalIn = instruction.NodeSignalInChildren[0]; // setpoint
                var value = signalIn.GetValue(rta);
                if (value != null)
                {
                    setPoint = Convert.ToDecimal(value.Value);
                }

                Decimal newElapsed = 0;
                if (setPoint < Decimal.MaxValue)
                {
                    // don't want to run the timer if we didn't find a valid setpoint
                    newElapsed = Convert.ToDecimal(Math.Min(oldElapsed + Convert.ToDecimal(engine.LastScanTime), setPoint));
                }

                if (newElapsed < setPoint)
                {
                    newRungOutCondition = oldRungOut;
                }
                else
                {
                    newRungOutCondition = false;
                }
                instruction.NodeSignalChildren[1].Value = newElapsed;
            }
            instruction.NodeSignalChildren[0].Value = newRungOutCondition;
            return new InstructionGroupExecutorContextLD(newRungOutCondition);
        }

    }
}
