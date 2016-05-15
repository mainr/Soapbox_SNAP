#region "SoapBox.Snap License"
/// <header module="SoapBox.Snap"> 
/// Copyright (C) 2009-2016 SoapBox Automation, All Rights Reserved.
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
using SoapBox.Core;
using SoapBox.Snap.Runtime;
using System.ComponentModel.Composition;
using SoapBox.Protocol.Automation;

namespace SoapBox.Snap.LD.Runtime.InstructionExecutors
{
    [Export(ExtensionPoints.Runtime.Snap.GroupExecutors_.LD_.InstructionExecutors,
        typeof(IInstructionExecutor<InstructionGroupExecutorContextLD>))]
    class InstructionLDCntUD : AbstractExtension, IInstructionExecutor<InstructionGroupExecutorContextLD>
    {

        [Import(SoapBox.Snap.Runtime.CompositionPoints.Runtime.Engine, typeof(IEngine))]
        private IEngine engine { get; set; }

        public FieldInstructionType InstructionType
        {
            get
            {
                return SoapBox.Snap.LD.InstructionLDCntUD.m_InstructionType;
            }
        }
        public InstructionGroupExecutorContextLD ScanInstruction(
            NodeRuntimeApplication rta, NodeInstruction instruction,
            InstructionGroupExecutorContextLD context)
        {
            // inputs: 0 = count-down, 1 = reset
            // outputs: 0 = counter value is zero, 1 = count, 2 = previous count-up state, 3 = previous count-down state

            bool newRungOutCondition;
            Decimal newCount;

            bool countDown = false;
            var signalIn = instruction.NodeSignalInChildren[0]; // count-down
            var value = signalIn.GetValue(rta);
            if (value != null)
            {
                countDown = Convert.ToBoolean(value.Value);
            }

            bool reset = false;
            signalIn = instruction.NodeSignalInChildren[1]; // reset
            value = signalIn.GetValue(rta);
            if (value != null)
            {
                reset = Convert.ToBoolean(value.Value);
            }

            bool oldRungOutCondition = Convert.ToBoolean(instruction.NodeSignalChildren[0].Value);
            Decimal oldCount = Convert.ToDecimal(instruction.NodeSignalChildren[1].Value);
            bool oldCountUpState = Convert.ToBoolean(instruction.NodeSignalChildren[2].Value);
            bool oldCountDownState = Convert.ToBoolean(instruction.NodeSignalChildren[3].Value);

            if (reset)
            {
                newCount = 0M;
            }
            else
            {
                newCount = oldCount;
                if (context.RungCondition && !oldCountUpState) // rising edge count up
                {
                    newCount = newCount + 1;
                }
                if (countDown && !oldCountDownState) // rising edge count up
                {
                    newCount = newCount - 1;
                }
            }
            newRungOutCondition = newCount == 0M;

            instruction.NodeSignalChildren[0].Value = newRungOutCondition;
            instruction.NodeSignalChildren[1].Value = newCount;
            instruction.NodeSignalChildren[2].Value = context.RungCondition; // save state for next time
            instruction.NodeSignalChildren[3].Value = countDown; // save state for next time
            return new InstructionGroupExecutorContextLD(newRungOutCondition);
        }
    }
}
