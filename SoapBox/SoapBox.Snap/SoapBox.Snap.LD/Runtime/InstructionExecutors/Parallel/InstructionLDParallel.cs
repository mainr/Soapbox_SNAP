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
using SoapBox.Snap.LD;

namespace SoapBox.Snap.Runtime.LD.Runtime.InstructionExecutors
{
    [Export(SoapBox.Snap.LD.ExtensionPoints.Runtime.Snap.GroupExecutors_.LD_.InstructionExecutors, 
        typeof(IInstructionExecutor<InstructionGroupExecutorContextLD>))]
    class InstructionLDParallel: AbstractExtension, IInstructionExecutor<InstructionGroupExecutorContextLD>
    {
        public FieldInstructionType InstructionType
        {
            get 
            { 
                return SoapBox.Snap.LD.InstructionLDParallel.m_InstructionType; 
            }
        }

        [Import(SoapBox.Snap.LD.CompositionPoints.Runtime.Snap.LD.GroupExecutor, typeof(GroupExecutor))]
        private GroupExecutor groupExecutorLD { get; set; }

        public InstructionGroupExecutorContextLD ScanInstruction(
            NodeRuntimeApplication rta, NodeInstruction instruction,
            InstructionGroupExecutorContextLD context)
        {
            bool mutableRungCondition = false; // OR's together the results
            foreach (var childInstruction in instruction.NodeInstructionChildren)
            {
                var contextResult = groupExecutorLD.ScanInstruction(rta, childInstruction, context);
                mutableRungCondition = contextResult.RungCondition || mutableRungCondition;
            }
            // Removed this because in some cases the rung-in condition doesn't have to be true for the output
            // of a branch to be true.  Seems odd, but the best example of this is a FallingEdge instruction,
            // which is true on the scan that the rung-in condition goes false.
            //mutableRungCondition = context.RungCondition && mutableRungCondition;
            return new InstructionGroupExecutorContextLD(mutableRungCondition);
        }

    }
}
