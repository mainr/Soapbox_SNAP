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
using System.ComponentModel.Composition;
using SoapBox.Protocol.Automation;
using SoapBox.Protocol.Base;
using System.Collections.ObjectModel;
using System.ComponentModel;
using SoapBox.Utilities;
using SoapBox.Core;

namespace SoapBox.Snap.LD
{
    /// <summary>
    /// Implements a parallel branch in the LD editor (OR condition)
    /// </summary>
    [Export(Snap.ExtensionPoints.Workbench.Documents.PageEditor.InstructionItems,
        typeof(IInstructionItem))]
    [InstructionItem(
        Language = Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD,
        Library = Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.Snap,
        Code = Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.Snap_.Parallel,
        SortOrder = Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.Snap_.Parallel_SortOrder,
         SpriteType = typeof(Resources.Images), SpriteKey = "SprParallel",
        ToolTipType = typeof(Resources.Strings), ToolTipKey = "ToolTipParallel")]
    public class InstructionLDParallel : AbstractLDInstructionItem 
    {
        internal static readonly FieldInstructionType m_InstructionType = new FieldInstructionType(
                    new FieldIdentifier(Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD),
                    new FieldIdentifier(Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.Snap),
                    new FieldIdentifier(Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.Snap_.Parallel));

        /// <summary>
        /// Just here for MEF to call, to do the imports
        /// </summary>
        internal InstructionLDParallel()
            : base(null, m_InstructionType)
        {
        }

        protected InstructionLDParallel(IEditorItem parent, NodeInstruction instruction)
            : base(parent, m_InstructionType)
        {
            if (instruction == null)
            {
                var newInstruction = NodeInstruction.BuildWith(InstructionType);
                newInstruction = newInstruction.NodeInstructionChildren.Append(
                    InstructionLDSeries.EmptyRung());
                newInstruction = newInstruction.NodeInstructionChildren.Append(
                    InstructionLDSeries.EmptyRung());
                Instruction = newInstruction;
            }
            else
            {
                if (instruction.InstructionType != InstructionType)
                {
                    throw new InvalidOperationException("Tried to instantiate InstructionLDParallel but passed a different instruction type.");
                }
                Instruction = instruction;
            }

            // Build the context menu
            if (extensionService != null)
            {
                ContextMenu = extensionService.SortAndJoin(ldInstructionContextMenu, m_staticMenuItemSeparator, contextMenu);
                ContextMenuEnabled = true;
            }
        }

        private static IMenuItem m_staticMenuItemSeparator = new ConcreteMenuItemSeparator();

        #region "extensionServiceSingleton"
        [Import(SoapBox.Core.Services.Host.ExtensionService)]
        private IExtensionService extensionService
        {
            get
            {
                return extensionServiceSingleton;
            }
            set
            {
                extensionServiceSingleton = value;
            }
        }
        private static IExtensionService extensionServiceSingleton = null;
        #endregion

        #region "ldInstructionContextMenuSingleton"
        [ImportMany(ExtensionPoints.Instructions.ContextMenu, typeof(IMenuItem))]
        private IEnumerable<IMenuItem> ldInstructionContextMenu
        {
            get
            {
                return ldInstructionContextMenuSingleton;
            }
            set
            {
                ldInstructionContextMenuSingleton = value;
            }
        }
        private static IEnumerable<IMenuItem> ldInstructionContextMenuSingleton = null;
        #endregion

        #region "contextMenuSingleton"
        [ImportMany(ExtensionPoints.Instructions.Parallel.ContextMenu, typeof(IMenuItem))]
        private IEnumerable<IMenuItem> contextMenu
        {
            get
            {
                return contextMenuSingleton;
            }
            set
            {
                contextMenuSingleton = value;
            }
        }
        private static IEnumerable<IMenuItem> contextMenuSingleton = null;
        #endregion

        #region " IsRight "
        public override bool IsRight
        {
            get
            {
                bool retVal = false;
                foreach (var instructionItem in Items)
                {
                    var series = instructionItem as InstructionLDSeries;
                    if (series.IsRight)
                    {
                        retVal = true;
                        break;
                    }
                }
                return retVal;
            }
        }
        private static readonly PropertyChangedEventArgs m_IsRightArgs =
            NotifyPropertyChangedHelper.CreateArgs<InstructionLDParallel>(o => o.IsRight);
        #endregion

        #region " Create "
        public override IInstructionItem Create(IEditorItem parent, NodeInstruction instruction)
        {
            return new InstructionLDParallel(parent, instruction);
        }
        #endregion

        #region " setItems "
        protected override void setItems()
        {
            if (Instruction.NodeInstructionChildren.Items.Count < 2)
            {
                throw new InvalidOperationException("An LD Parallel instruction must have at least 2 series children.");
            }
            foreach (var nodeWrapper in Items)
            {
                var instruc = nodeWrapper as ILDInstructionItem;
                if (instruc != null)
                {
                    instruc.PropertyChanged -= new PropertyChangedEventHandler(editorItem_PropertyChanged);
                }
            }
            var newCollection = new ObservableCollection<INodeWrapper>();
            foreach (var nInstruction in Instruction.NodeInstructionChildren.Items)
            {
                if (nInstruction.InstructionType != InstructionLDSeries.StaticInstructionType)
                {
                    throw new InvalidOperationException("All children of the LD Parallel instruction must be " +
                        InstructionLDSeries.StaticInstructionType.ToString() +
                        " instructions.");
                }
                IInstructionItem editorItem = FindItemByNodeId(nInstruction.ID) as IInstructionItem;
                if (editorItem == null)
                {
                    editorItem = InstructionLDSeries.CreateForLD(this, nInstruction);
                    logger.Info("Created new series editor item as child of parallel instruction.");
                }
                else
                {
                    editorItem.Parent = this;
                }
                editorItem.PropertyChanged += new PropertyChangedEventHandler(editorItem_PropertyChanged);
                newCollection.Add(editorItem);
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

        // watch child series elements for properties changing
        void editorItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == m_InstructionLDSeries_VerticalRungOffsetName)
            {
                NotifyPropertyChanged(m_VerticalRungOffsetArgs);
            }
        }
        #endregion

        #region " VerticalRungOffset "
        public override double VerticalRungOffset
        {
            get
            {
                double retVal = 0;
                foreach (var nodeWrapper in Items)
                {
                    var instruc = nodeWrapper as ILDInstructionItem;
                    if (instruc != null)
                    {
                        retVal = Math.Max(retVal, instruc.VerticalRungOffset);
                        break;
                    }
                }
                return retVal;
            }
            set
            {
            }
        }
        static PropertyChangedEventArgs m_VerticalRungOffsetArgs =
            NotifyPropertyChangedHelper.CreateArgs<InstructionLDParallel>(o => o.VerticalRungOffset);
        static string m_InstructionLDSeries_VerticalRungOffsetName =
            NotifyPropertyChangedHelper.GetPropertyName<InstructionLDSeries>(o => o.VerticalRungOffset);
        #endregion

        public void AppendBranch()
        {
            if (runtimeService.DisconnectDialog(this))
            {
                var doActions = new List<Action>();
                var undoActions = new List<Action>();
                var origInstruction = Instruction;
                var newSeriesItem = InstructionLDSeries.CreateForLD(this, null);
                doActions.Add(() => Items.Add(newSeriesItem));
                doActions.Add(() => Instruction = Instruction.NodeInstructionChildren.Append(newSeriesItem.Instruction));
                undoActions.Add(() => Instruction = origInstruction);
                Do(new UndoMemento(this, ActionType.EDIT,
                    Resources.Strings.Undo_Action_Instruction_Parallel_AppendBranch, undoActions, doActions));
            }
        }

        public void RemoveEmptyBranches()
        {
            if (runtimeService.DisconnectDialog(this))
            {
                var doActions = new List<Action>();
                var undoActions = new List<Action>();
                var origInstruction = Instruction;
                var newInstruction = Instruction;
                foreach (var branch in Instruction.NodeInstructionChildren.Items)
                {
                    if (branch.NodeInstructionChildren.Count == 0)
                    {
                        if (newInstruction.NodeInstructionChildren.Count > 2) // can't remove all of them
                        {
                            var ldInstructionItem = FindItemByNodeId(branch.ID) as ILDInstructionItem;
                            if (ldInstructionItem != null)
                            {
                                undoActions.Add(() => Items.Add(ldInstructionItem)); // re-use the viewmodel
                            }
                            newInstruction = newInstruction.NodeInstructionChildren.Remove(branch);
                        }
                    }
                }
                if (newInstruction != origInstruction)
                {
                    doActions.Add(() => Instruction = newInstruction);
                    undoActions.Add(() => Instruction = origInstruction);
                    Do(new UndoMemento(this, ActionType.EDIT,
                        Resources.Strings.Undo_Action_Instruction_Parallel_RemoveEmptyBranches, undoActions, doActions));
                }
            }
        }
    }
}
