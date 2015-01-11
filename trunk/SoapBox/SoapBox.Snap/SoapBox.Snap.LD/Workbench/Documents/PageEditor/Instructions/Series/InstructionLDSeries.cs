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
using SoapBox.Protocol.Base;
using System.Collections.ObjectModel;
using System.ComponentModel;
using SoapBox.Utilities;
using SoapBox.Core;

namespace SoapBox.Snap.LD
{
    /// <summary>
    /// Implements a series branch in the LD editor (AND condition)
    /// </summary>
    [Export(SoapBox.Core.ExtensionPoints.Host.Void, typeof(Object))] // forces us to be non-lazy loaded
    [Export(Snap.ExtensionPoints.Workbench.Documents.PageEditor.InstructionItems,
        typeof(IInstructionItem))]
    [InstructionItem(
        Language = Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD,
        Library = Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.Snap,
        Code = Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.Snap_.Series,
        Hidden = true)]
    public class InstructionLDSeries : AbstractLDInstructionItem
    {
        internal static readonly FieldInstructionType m_InstructionType = new FieldInstructionType(
                    new FieldIdentifier(Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD),
                    new FieldIdentifier(Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.Snap),
                    new FieldIdentifier(Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.Snap_.Series));

        public static FieldInstructionType StaticInstructionType
        {
            get
            {
                return m_InstructionType;
            }
        }

        /// <summary>
        /// Just here for MEF to call, to do the imports
        /// </summary>
        internal InstructionLDSeries()
            : base(null, m_InstructionType)
        {
        }

        protected InstructionLDSeries(IEditorItem parent, NodeInstruction instruction)
            : base(parent, m_InstructionType)
        {
            if (instruction == null)
            {
                Instruction = EmptyRung();
            }
            else
            {
                if (instruction.InstructionType != InstructionType)
                {
                    throw new InvalidOperationException("Tried to instantiate InstructionLDSeries but passed a different instruction type.");
                }
                Instruction = instruction;
            }
        }

        #region " instructionItemsSingleton "
        [ImportMany(Snap.ExtensionPoints.Workbench.Documents.PageEditor.InstructionItems,
            typeof(IInstructionItem), AllowRecomposition = true)]
        private IEnumerable<Lazy<IInstructionItem, IInstructionItemMeta>> instructionItems
        {
            get
            {
                lock (m_instructionItems_Lock)
                {
                    return m_instructionItems;
                }
            }
            set
            {
                lock (m_instructionItems_Lock)
                {
                    m_instructionItems = value;
                }
            }
        }
        private static IEnumerable<Lazy<IInstructionItem, IInstructionItemMeta>> m_instructionItems = null;
        private static readonly object m_instructionItems_Lock = new object();

        #endregion

        #region " Create "
        public override IInstructionItem Create(IEditorItem parent, NodeInstruction instruction)
        {
            return CreateForLD(parent, instruction);
        }

        /// <summary>
        /// Just a static method that the rest of this assembly can call
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="instruction"></param>
        /// <returns></returns>
        public static IInstructionItem CreateForLD(IEditorItem parent, NodeInstruction instruction)
        {
            return new InstructionLDSeries(parent, instruction);
        }

        public static NodeInstruction EmptyRung()
        {
            return NodeInstruction.BuildWith(m_InstructionType);
        }
        #endregion

        #region " IsRight "
        public override bool IsRight
        {
            get
            {
                return m_IsRight;
            }
        }
        private bool m_IsRight = false;
        private static readonly PropertyChangedEventArgs m_IsRightArgs =
            NotifyPropertyChangedHelper.CreateArgs<InstructionLDSeries>(o => o.IsRight);
        #endregion

        protected override void setItems()
        {
            m_IsRight = false;
            var newCollection = new ObservableCollection<INodeWrapper>();
            foreach (var nInstruction in Instruction.NodeInstructionChildren.Items)
            {
                var editorItem = FindItemByNodeId(nInstruction.ID) as ILDInstructionItem;
                if (editorItem == null)
                {
                    foreach (var lazyInstructionItem in instructionItems)
                    {
                        if (lazyInstructionItem.Metadata.Language == nInstruction.InstructionType.Language.ToString() &&
                            lazyInstructionItem.Metadata.Library == nInstruction.InstructionType.Library.ToString() &&
                            lazyInstructionItem.Metadata.Code == nInstruction.InstructionType.Code.ToString())
                        {
                            editorItem = lazyInstructionItem.Value.Create(this, nInstruction) as ILDInstructionItem;
                            if (editorItem != null)
                            {
                                break;
                            }
                        }
                    }
                }
                else
                {
                    editorItem.Parent = this;
                }
                newCollection.Add(editorItem);
                if (editorItem.IsRight)
                {
                    m_IsRight = true;
                }
            }
            foreach (var item in Items)
            {
                if (!newCollection.Contains(item))
                {
                    item.Parent = null;
                }
            }
            Items = newCollection;
            NotifyPropertyChanged(m_IsRightArgs);
            NotifyPropertyChanged(m_VerticalRungOffsetArgs);
        }

        #region " Drag & Drop "
        // Index is the item to drop it *before*.  If index is
        // one greater than the last index, we drop it at the end.
        public bool CanDrop(IEnumerable<ILDInstructionItem> source, int index)
        {
            if (index > Items.Count || index < 0)
            {
                return false;
            }
            else
            {
                int rightCount = 0;
                foreach (var ldInstructionItem in source)
                {
                    if (ldInstructionItem.InstructionType.Language !=
                        Extensions.Workbench.Documents.PageEditor_.InstructionGroupItems.LD
                        || ldInstructionItem.Instruction == null)
                    {
                        return false;
                    }
                    if (ldInstructionItem.IsRight)
                    {
                        rightCount++;
                    }
                }
                if (rightCount > 1)
                {
                    return false;
                }
                else if (rightCount == 1)
                {
                    // can only insert in the rightmost position,
                    // and only if we're not a right instruction
                    if (this.IsRight || index < Items.Count)
                    {
                        return false;
                    }

                    // the right instruction better be the last one
                    if (!source.Last().IsRight)
                    {
                        return false;
                    }
                }
                else
                {
                    // we can't insert to the right if we're a right instruction
                    if (this.IsRight && index >= Items.Count)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public void Drop(IEnumerable<ILDInstructionItem> source, int index)
        {
            if (CanDrop(source, index))
            {
                insert(source, index);
            }
        }
        #endregion

        /// <summary>
        /// Inserts the given item *before* the given index.
        /// If the given index is one greater than the last index,
        /// it appends it to the items.  Also takes care of removing the 
        /// items from wherever they came from (i.e. for drag and drop)
        /// and builds the Do/Undo/Redo actions.
        /// </summary>
        internal void insert(IEnumerable<ILDInstructionItem> source, int index)
        {
            if (runtimeService.DisconnectDialog(this))
            {
                NodeInstruction nInstruction = null;
                if (index < Items.Count)
                {
                    nInstruction = Items[index].Node as NodeInstruction;
                }
                var doActions = new List<Action>();
                var undoActions = new List<Action>();
                var origInstruction = Instruction;
                var newInstruction = Instruction;

                undoActions.Add(() => Instruction = origInstruction);

                // Collect a list of the parents that we're taking these from, and state of each (there can only be one parent)
                InstructionLDSeries previousParent = null;
                NodeInstruction previousParentNodeOrig = null;
                NodeInstruction previousParentNodeNew = null;
                foreach (var ldInstructionItem in source)
                {
                    if (ldInstructionItem.Parent != null)
                    {
                        var instructionParent = ldInstructionItem.Parent as InstructionLDSeries;
                        if (instructionParent != null)
                        {
                            if (previousParent != null && previousParent != instructionParent)
                            {
                                throw new InvalidOperationException("Tried to drop items that were sourced from more than one series instruction.");
                            }
                            if (previousParent == null)
                            {
                                previousParent = instructionParent;
                                previousParentNodeOrig = previousParent.Instruction;
                                previousParentNodeNew = previousParent.Instruction;
                            }

                            previousParentNodeNew = previousParentNodeNew.NodeInstructionChildren.Remove(ldInstructionItem.Instruction);
                        }
                    }
                }

                bool destinationIsAncestor = false;
                bool sourceIsAncestor = false;

                // This block deals with where the items came from (if anywhere)
                if (previousParent != null)
                {
                    if (previousParent == this)
                    {
                        // simple case - do nothing (we're just moving things around in the same series instruction
                    }
                    else
                    {
                        if (previousParent.FindAncestorByNodeId(this.Node.ID) != null)
                        {
                            destinationIsAncestor = true;
                        }
                        else if (FindAncestorByNodeId(previousParent.Node.ID) != null)
                        {
                            sourceIsAncestor = true;
                        }

                        if (!sourceIsAncestor && !destinationIsAncestor)
                        {
                            // They are from different branches, and one is not the ancestor of another

                            // -------------- UNDO ---------------------
                            foreach (var ldInstructionItem in source)
                            {
                                undoActions.Add(() => ldInstructionItem.Parent = null);
                                undoActions.Add(() => previousParent.Items.Add(ldInstructionItem));
                            }
                            // we also need to restore the parents' parent node, etc.
                            INodeWrapper parentIterator = previousParent;
                            while (parentIterator != null)
                            {
                                saveNodeState(parentIterator, undoActions);
                                parentIterator = parentIterator.Parent;
                            }
                            undoActions.Add(() => previousParent.setItems());

                            // --------------- DO ----------------------
                            doActions.Add(() => previousParent.Instruction = previousParentNodeNew);
                        }
                        else
                        {
                            // Break it down into two steps
                            previousParent.DeleteSelectedChildren(true);
                            this.insert(source, index);
                        }
                    }
                }

                // This block deals with where the items are going (here)
                if (!sourceIsAncestor && !destinationIsAncestor)
                {
                    foreach (var ldInstructionItem in source)
                    {
                        var instruc = ldInstructionItem.Instruction;

                        doActions.Add(() => ldInstructionItem.Parent = null);
                        doActions.Add(() =>
                            {
                                if (!Items.Contains(ldInstructionItem))
                                {
                                    Items.Add(ldInstructionItem);
                                }
                            });
                        if (index >= Items.Count)
                        {
                            newInstruction = newInstruction.NodeInstructionChildren.Append(instruc);
                        }
                        else
                        {
                            if (nInstruction == null)
                            {
                                throw new ArgumentOutOfRangeException();
                            }
                            if (newInstruction.NodeInstructionChildren.Contains(instruc)) // in case we're dragging and dropping from the same rung
                            {
                                newInstruction = newInstruction.NodeInstructionChildren.Remove(instruc);
                            }
                            newInstruction = newInstruction.NodeInstructionChildren.InsertBefore(nInstruction, instruc);
                        }
                    }
                    doActions.Add(() => Instruction = newInstruction);

                    string undoMessage;
                    if (previousParent == null)
                    {
                        undoMessage = Resources.Strings.Undo_Action_Instruction_Series_Insert;
                    }
                    else
                    {
                        undoMessage = Resources.Strings.Undo_Action_Instruction_Series_Move;
                    }

                    Do(new UndoMemento(this, ActionType.EDIT,
                        undoMessage, undoActions, doActions), previousParent);
                }
            }
        }

        private void saveNodeState(INodeWrapper nodeWrapper, List<Action> actions)
        {
            if (nodeWrapper != null && actions != null)
            {
                var saveNodeWrapper = nodeWrapper;
                var saveNode = saveNodeWrapper.Node;
                actions.Add(() => saveNodeWrapper.StealthSetNode(saveNode));
            }
        }

        #region " VerticalRungOffset "
        public override double VerticalRungOffset
        {
            get
            {
                return m_VerticalRungOffset + 4;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(m_VerticalRungOffsetName);
                }
                if (m_VerticalRungOffset != value)
                {
                    m_VerticalRungOffset = value;
                    NotifyPropertyChanged(m_VerticalRungOffsetArgs);
                }
            }
        }
        private double m_VerticalRungOffset = 0.0;
        private static readonly PropertyChangedEventArgs m_VerticalRungOffsetArgs =
            NotifyPropertyChangedHelper.CreateArgs<InstructionLDSeries>(o => o.VerticalRungOffset);
        private static string m_VerticalRungOffsetName =
            NotifyPropertyChangedHelper.GetPropertyName<InstructionLDSeries>(o => o.VerticalRungOffset);

        #endregion

        #region " VerticalRungOutOffset "
        public double VerticalRungOutOffset
        {
            get
            {
                return Math.Max(m_VerticalRungOutOffset,4);
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(m_VerticalRungOutOffsetName);
                }
                if (m_VerticalRungOutOffset != value)
                {
                    m_VerticalRungOutOffset = value;
                    NotifyPropertyChanged(m_VerticalRungOutOffsetArgs);
                }
            }
        }
        private double m_VerticalRungOutOffset = 0;
        private static readonly PropertyChangedEventArgs m_VerticalRungOutOffsetArgs =
            NotifyPropertyChangedHelper.CreateArgs<InstructionLDSeries>(o => o.VerticalRungOutOffset);
        private static string m_VerticalRungOutOffsetName =
            NotifyPropertyChangedHelper.GetPropertyName<InstructionLDSeries>(o => o.VerticalRungOutOffset);
        #endregion

        public override void DeleteSelectedChildren()
        {
            DeleteSelectedChildren(false);
        }

        private void DeleteSelectedChildren(bool bindWithNext)
        {
            if (runtimeService.DisconnectDialog(this))
            {
                var doActions = new List<Action>();
                var undoActions = new List<Action>();
                var origInstruction = Instruction;
                var newInstruction = Instruction;

                foreach (var child in Items)
                {
                    if (child.IsSelected)
                    {
                        undoActions.Add(() => Items.Add(child)); // re-use the viewmodel
                        newInstruction = newInstruction.NodeInstructionChildren.Remove(child.Node as NodeInstruction);
                    }
                }

                if (newInstruction != origInstruction)
                {
                    doActions.Add(() => Instruction = newInstruction);
                    undoActions.Add(() => Instruction = origInstruction);
                    Do(new UndoMemento(this, ActionType.DELETE,
                        Resources.Strings.Undo_Action_Instruction_Series_DeleteSelected, undoActions, doActions, bindWithNext));
                }
            }
        }

     }
}
