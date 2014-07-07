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
using SoapBox.Core;
using SoapBox.Utilities;
using System.ComponentModel;
using SoapBox.Protocol.Automation;
using SoapBox.Protocol.Base;
using System.Windows;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;

namespace SoapBox.Snap
{
    public class AbstractInstructionItem : AbstractEditorItem, IInstructionItem
    {
        public AbstractInstructionItem(IEditorItem parent, FieldInstructionType instructionType)
            : base(parent)
        {
            if (instructionType == null)
            {
                throw new ArgumentNullException("instructionType");
            }
            m_InstructionType = instructionType;

            PropertyChanged += new PropertyChangedEventHandler(AbstractEditorItem_PropertyChanged);
            parentChanged();
        }

        private void parentChanged()
        {
            if (oldParent != Parent)
            {
                if (oldParent != null)
                {
                    this.Edited -= new EditedHandler(oldParent.ChildInstruction_Edited);
                    this.Deleted -= new DeletedHandler(oldParent.ChildInstruction_Deleted);
                }
                oldParent = Parent as IInstructionItem;
                if (oldParent != null)
                {
                    this.Edited += new EditedHandler(oldParent.ChildInstruction_Edited);
                    this.Deleted += new DeletedHandler(oldParent.ChildInstruction_Deleted);
                }
            }
        }

        private IInstructionItem oldParent = null;

        // Makes all undoable action events go up the tree
        void AbstractEditorItem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == m_ParentName)
            {
                parentChanged();
            }
        }
        static readonly string m_ParentName =
            NotifyPropertyChangedHelper.GetPropertyName<AbstractInstructionItem>(o => o.Parent);

        #region " ContextMenu "
        /// <summary>
        /// Derived classes can use this to store a context menu
        /// and the series instruction hooks a context menu here.
        /// </summary>
        public IEnumerable<IMenuItem> ContextMenu
        {
            get
            {
                return m_ContextMenu;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(m_ContextMenuName);
                }
                if (m_ContextMenu != value)
                {
                    m_ContextMenu = value;
                    NotifyPropertyChanged(m_ContextMenuArgs);
                }
            }
        }
        private IEnumerable<IMenuItem> m_ContextMenu = new Collection<IMenuItem>();
        private static readonly PropertyChangedEventArgs m_ContextMenuArgs =
            NotifyPropertyChangedHelper.CreateArgs<AbstractInstructionItem>(o => o.ContextMenu);
        private static string m_ContextMenuName =
            NotifyPropertyChangedHelper.GetPropertyName<AbstractInstructionItem>(o => o.ContextMenu);
        #endregion

        #region " ContextMenuEnabled "
        public bool ContextMenuEnabled
        {
            get
            {
                return m_ContextMenuEnabled;
            }
            set
            {
                if (m_ContextMenuEnabled != value)
                {
                    m_ContextMenuEnabled = value;
                    NotifyPropertyChanged(m_ContextMenuEnabledArgs);
                }
            }
        }
        private bool m_ContextMenuEnabled = false;
        private static readonly PropertyChangedEventArgs m_ContextMenuEnabledArgs =
            NotifyPropertyChangedHelper.CreateArgs<AbstractInstructionItem>(o => o.ContextMenuEnabled);
        private static string m_ContextMenuEnabledName =
            NotifyPropertyChangedHelper.GetPropertyName<AbstractInstructionItem>(o => o.ContextMenuEnabled);
        #endregion

        #region " Instruction "
        public NodeInstruction Instruction
        {
            get
            {
                return Node as NodeInstruction;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(m_InstructionName);
                }
                if (Node != value)
                {
                    Node = value;
                    setItems();
                    NotifyPropertyChanged(m_InstructionArgs);
                }
            }
        }
        private static readonly PropertyChangedEventArgs m_InstructionArgs =
            NotifyPropertyChangedHelper.CreateArgs<AbstractInstructionItem>(o => o.Instruction);
        private static string m_InstructionName =
            NotifyPropertyChangedHelper.GetPropertyName<AbstractInstructionItem>(o => o.Instruction);
        #endregion

        public virtual IInstructionItem Create(IEditorItem parent, NodeInstruction instruction)
        {
            throw new NotImplementedException();
        }

        protected virtual void setItems()
        {
        }

        public void ChildInstruction_Edited(INodeWrapper sender, NodeBase oldNode, NodeBase newNode)
        {
            var oldInstruction = oldNode as NodeInstruction;
            var newInstruction = newNode as NodeInstruction;
            if (oldInstruction != null && newInstruction != null)
            {
                Instruction = Instruction.NodeInstructionChildren.Replace(oldInstruction, newInstruction);
            }
        }

        public void ChildInstruction_Deleted(INodeWrapper sender, NodeBase deletedNode)
        {
            var deletedInstruction = deletedNode as NodeInstruction;
            var instructionItem = sender as IInstructionItem;
            if (deletedInstruction != null && instructionItem != null)
            {
                if (instructionItem.Parent == this)
                {
                    instructionItem.Parent = null;
                }
                Instruction = Instruction.NodeInstructionChildren.Remove(deletedInstruction);
                instructionItem.Edited -= new EditedHandler(ChildInstruction_Edited);
                instructionItem.Deleted -= new DeletedHandler(ChildInstruction_Deleted);
                Items.Remove(instructionItem);
            }
        }

        #region " InstructionType "
        public FieldInstructionType InstructionType
        {
            get
            {
                return m_InstructionType;
            }
        }
        private readonly FieldInstructionType m_InstructionType = null;
        #endregion

        /// <summary>
        /// You can use this to change the Instruction property including doing all
        /// the legwork for the Undo/Redo framework, but only if the change *doesn't*
        /// involve editing the NodeInstructionChildren (child instructions)
        /// </summary>
        /// <param name="newInstruction"></param>
        protected void SimpleUndoableInstructionEdit(NodeInstruction newInstruction, string undoDescription)
        {
            var undoActions = new Collection<Action>();
            var doActions = new Collection<Action>();
            var saveNewInstruction = newInstruction;
            var origInstruction = Instruction;
            doActions.Add(() => Instruction = saveNewInstruction);
            undoActions.Add(() => Instruction = origInstruction);
            Do(new UndoMemento(this, ActionType.EDIT, undoDescription, undoActions, doActions));
        }

    }
}
