#region "SoapBox.Snap License"
/// <header module="SoapBox.Snap"> 
/// Copyright (C) 2009-2015 SoapBox Automation, All Rights Reserved.
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
using System.ComponentModel.Composition;
using System.ComponentModel;
using SoapBox.Utilities;
using SoapBox.Protocol.Base;
using System.Collections.ObjectModel;

namespace SoapBox.Snap.LD
{
    [Export(Snap.ExtensionPoints.Workbench.Documents.PageEditor.InstructionGroupItems, 
        typeof(IInstructionGroupItem))]
    [InstructionGroupItem(
        Language = Extensions.Workbench.Documents.PageEditor_.InstructionGroupItems.LD,
        LanguageHeaderType = typeof(Resources.Strings), LanguageHeaderKey = "InstructionGroup_Header")]
    public class InstructionGroupLD : AbstractInstructionGroupItem
    {
        /// <summary>
        /// Just here for MEF to call, to do the imports
        /// </summary>
        internal InstructionGroupLD()
            : base(null)
        {
        }

        internal InstructionGroupLD(IEditorItem parent, NodeInstructionGroup instructionGroup)
            : base(parent)
        {
            if (instructionGroup == null)
            {
                // a new group
                InstructionGroup = emptyNode();
            }
            else
            {
                if (instructionGroup.Language != Extensions.Workbench.Documents.PageEditor_.InstructionGroupItems.LD)
                {
                    throw new InvalidOperationException("instructionGroup");
                }
                InstructionGroup = instructionGroup;
            }
        }

        #region " Create "
        /// <summary>
        /// Factory function for creating LD Editors
        /// </summary>
        /// <param name="parent">Usually the PageEditor.PageEditorItem, but could be null</param>
        /// <param name="instructionGroup">The wrapped model class.  Null = create a new one</param>
        /// <returns></returns>
        public override IInstructionGroupItem Create(IEditorItem parent, NodeInstructionGroup instructionGroup)
        {
            return new InstructionGroupLD(parent, instructionGroup);
        }
        #endregion

        public override NodeInstructionGroup CreateEmptyNode()
        {
            return emptyNode();
        }

        private static NodeInstructionGroup emptyNode()
        {
            var newInstructionGroup = NodeInstructionGroup.BuildWith(
                new FieldIdentifier(Extensions.Workbench.Documents.PageEditor_.InstructionGroupItems.LD));
            newInstructionGroup = newInstructionGroup.NodeInstructionChildren.Append(
                InstructionLDSeries.EmptyRung()); // empty rungs are just a Series instruction
            return newInstructionGroup;
        }

        public INodeWrapper Rung
        {
            get
            {
                return Items[0];
            }
        }

        protected override void setItems()
        {
            base.setItems();
            if(InstructionGroup.NodeInstructionChildren.Items.Count != 1)
            {
                throw new InvalidOperationException("An " +
                    Extensions.Workbench.Documents.PageEditor_.InstructionGroupItems.LD +
                    " instruction group can only have one instruction child.");
            }
            NodeInstruction nInstruction = InstructionGroup.NodeInstructionChildren.Items[0];
            if (nInstruction.InstructionType != InstructionLDSeries.StaticInstructionType)
            {
                throw new InvalidOperationException("The first and only instruction in an " +
                    Extensions.Workbench.Documents.PageEditor_.InstructionGroupItems.LD +
                    " instruction group must be a " +
                    InstructionLDSeries.StaticInstructionType.ToString() + 
                    " instruction.");
            }
            var newCollection = new ObservableCollection<INodeWrapper>();
            IInstructionItem editorItem = FindItemByNodeId(nInstruction.ID) as InstructionLDSeries;
            if (editorItem == null)
            {
                editorItem = InstructionLDSeries.CreateForLD(this, nInstruction);
                HookupHandlers(editorItem);
            }
            newCollection.Add(editorItem);
            Items = newCollection;
        }

        void HookupHandlers(IInstructionItem editorItem)
        {
            editorItem.Parent = this;
            editorItem.Edited += new EditedHandler(Instruction_Edited);
        }

        void Instruction_Edited(INodeWrapper sender, NodeBase oldNode, NodeBase newNode)
        {
            var oldInstruction = oldNode as NodeInstruction;
            var newInstruction = newNode as NodeInstruction;
            if (oldInstruction != null && newInstruction != null)
            {
                if (newInstruction.InstructionType != InstructionLDSeries.StaticInstructionType)
                {
                    throw new InvalidOperationException("The first and only instruction in an " +
                        Extensions.Workbench.Documents.PageEditor_.InstructionGroupItems.LD +
                        " instruction group must be a " +
                        InstructionLDSeries.StaticInstructionType.ToString() +
                        " instruction.");
                }
                InstructionGroup = InstructionGroup.NodeInstructionChildren.Replace(oldInstruction, newInstruction);
            }
        }

    }
}
