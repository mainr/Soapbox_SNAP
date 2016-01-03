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
using System.ComponentModel.Composition;
using SoapBox.Core;
using SoapBox.Protocol.Automation;
using SoapBox.Snap.Runtime;

namespace SoapBox.Snap.LD.Runtime.InstructionExecutors
{
    [Export(ExtensionPoints.Runtime.Snap.GroupExecutors_.LD_.InstructionExecutors, 
        typeof(IInstructionExecutor<InstructionGroupExecutorContextLD>))]
    class InstructionLDStringContains: AbstractExtension, IInstructionExecutor<InstructionGroupExecutorContextLD>
    {
        public FieldInstructionType InstructionType
        {
            get 
            { 
                return SoapBox.Snap.LD.InstructionLDStringContains.m_InstructionType; 
            }
        }

        public InstructionGroupExecutorContextLD  ScanInstruction(
            NodeRuntimeApplication rta, NodeInstruction instruction, 
            InstructionGroupExecutorContextLD context)
        {

            bool enable = context.RungCondition;

            string stringToSearch = string.Empty;
            var stringToSearchSignalIn = instruction.NodeSignalInChildren[0]; // stringToSearch
            var stringToSearchValue = stringToSearchSignalIn.GetValue(rta);
            if (stringToSearchValue != null)
            {
                stringToSearch = Convert.ToString(stringToSearchValue.Value);
            }

            string stringToFind = string.Empty;
            var stringToFindSignalIn = instruction.NodeSignalInChildren[1]; // stringToFind
            var stringToFindValue = stringToFindSignalIn.GetValue(rta);
            if (stringToFindValue != null)
            {
                stringToFind = Convert.ToString(stringToFindValue.Value);
            }

            bool caseSensitive = false;
            var caseSensitiveSignalIn = instruction.NodeSignalInChildren[2]; // caseSensitive
            var caseSensitiveValue = caseSensitiveSignalIn.GetValue(rta);
            if (caseSensitiveValue != null)
            {
                caseSensitive = Convert.ToBoolean(caseSensitiveValue.Value);
            }

            bool oldRungOutCondition = Convert.ToBoolean(instruction.NodeSignalChildren[0].Value);
            bool newRungOutCondition;

            if (!enable)
            {
                newRungOutCondition = false;
            }
            else
            {
                if (caseSensitive)
                {
                    newRungOutCondition = stringToSearch.Contains(stringToFind);
                }
                else
                {
                    newRungOutCondition = stringToSearch.ToLower().Contains(stringToFind.ToLower());
                }
            }

            instruction.NodeSignalChildren[0].Value = newRungOutCondition;
            return new InstructionGroupExecutorContextLD(newRungOutCondition);
        }

    }
}
