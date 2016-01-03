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
using System.ComponentModel.Composition;
using SoapBox.Protocol.Automation;
using SoapBox.Snap.Runtime;

namespace SoapBox.Snap.LD
{
    /// <summary>
    /// Executes the ladder logic (called by the runtime engine)
    /// </summary>
    [Export(SoapBox.Snap.Runtime.ExtensionPoints.Runtime.Snap.GroupExecutors, typeof(IGroupExecutor))]
    [Export(CompositionPoints.Runtime.Snap.LD.GroupExecutor, typeof(GroupExecutor))]
    [GroupExecutor(
        Language = Extensions.Runtime.Snap.GroupExecutors.LD)]
    class GroupExecutor : AbstractExtension, IGroupExecutorLD, IPartImportsSatisfiedNotification
    {
        private GroupExecutor()
        {
            ID = Extensions.Runtime.Snap.GroupExecutors.LD;
        }

        [ImportMany(ExtensionPoints.Runtime.Snap.GroupExecutors_.LD_.InstructionExecutors,
            typeof(IInstructionExecutor<InstructionGroupExecutorContextLD>))]
        private IEnumerable<IInstructionExecutor<InstructionGroupExecutorContextLD>> instructionExecutorsImported { get; set; }

        private Dictionary<FieldInstructionType, IInstructionExecutor<InstructionGroupExecutorContextLD>> m_instructionExecutors;

        public void OnImportsSatisfied()
        {
            // Put them in a dictionary for more efficient lookup at runtime
            m_instructionExecutors = new Dictionary<FieldInstructionType, IInstructionExecutor<InstructionGroupExecutorContextLD>>();
            foreach (var instructionExecutor in instructionExecutorsImported)
            {
                m_instructionExecutors.Add(instructionExecutor.InstructionType, instructionExecutor);
            }
        }

        #region Language
        public string Language
        {
            get
            {
                return Extensions.Runtime.Snap.GroupExecutors.LD;
            }
        }
        #endregion

        public void ScanInstructionGroup(NodeRuntimeApplication rta, NodeInstructionGroup instructionGroup)
        {
            var contextIterator = new InstructionGroupExecutorContextLD(true); // rung condition at the beginning of a rung is always true
            // A rung only ever has one series instruction as a child, so the foreach is kind of overkill, but should work
            foreach (var instruction in instructionGroup.NodeInstructionChildren)
            {
                contextIterator = ScanInstruction(rta, instruction, contextIterator);
            }
        }

        public InstructionGroupExecutorContextLD ScanInstruction(NodeRuntimeApplication rta, NodeInstruction instruction, InstructionGroupExecutorContextLD context)
        {
            var retVal = new InstructionGroupExecutorContextLD(false);
            if (m_instructionExecutors.ContainsKey(instruction.InstructionType))
            {
                var executor = m_instructionExecutors[instruction.InstructionType];
                retVal = executor.ScanInstruction(rta, instruction, context);
            }
            else
            {
                // FIXME - throw some kind of exception?
            }
            return retVal;
        }


    }
}
