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
using System.Windows.Controls;
using System.Windows;
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
        Code = Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.Snap_.Coil,
        SortOrder = Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.Snap_.Coil_SortOrder,
        SpriteType = typeof(Resources.Images), SpriteKey = "SprCoil",
        ToolTipType = typeof(Resources.Strings), ToolTipKey = "ToolTipCoil")]
    public class InstructionLDCoil : AbstractLDInstructionItem
    {
        public const double COIL_DIAMETER = 20;
        public const double MAX_WIDTH = 100;
        public const double MAX_DESCRIPTION_HEIGHT = 70;

        internal static readonly FieldInstructionType m_InstructionType = new FieldInstructionType(
                    new FieldIdentifier(Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD),
                    new FieldIdentifier(Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.Snap),
                    new FieldIdentifier(Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.Snap_.Coil));

        /// <summary>
        /// Just here for MEF to call, to do the imports
        /// </summary>
        internal InstructionLDCoil()
            : base(null, m_InstructionType)
        {
        }

        private InstructionLDCoil(IEditorItem parent, NodeInstruction instruction)
            : base(parent, m_InstructionType)
        {
            if (instruction == null)
            {
                var newInstruction = NodeInstruction.BuildWith(InstructionType);
                // Output signal: Named boolean - the coil signal
                newInstruction = newInstruction.NodeSignalChildren.Append(
                    NodeSignal.BuildWith(
                        new FieldSignalName(Resources.Strings.LD_Snap_Coil_DefaultCoilName),
                        new FieldDataType(FieldDataType.DataTypeEnum.BOOL),
                        new FieldBool(false),
                        FieldConstant.Constants.BOOL.LOW) // not forced
                    );
                Instruction = newInstruction;
            }
            else
            {
                if (instruction.InstructionType != InstructionType)
                {
                    throw new InvalidOperationException("Tried to instantiate InstructionLDCoil but passed a different instruction type.");
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
        [ImportMany(ExtensionPoints.Instructions.Coil.ContextMenu, typeof(IMenuItem))]
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
                return true;
            }
        }
        #endregion

        public override IInstructionItem Create(IEditorItem parent, NodeInstruction instruction)
        {
            return new InstructionLDCoil(parent, instruction);
        }

        public NodeSignal CoilSignal
        {
            get
            {
                return Instruction.NodeSignalChildren.Items[0];
            }
        }
        private static readonly PropertyChangedEventArgs m_CoilSignalArgs =
            NotifyPropertyChangedHelper.CreateArgs<InstructionLDCoil>(o => o.CoilSignal);

        public string CoilName
        {
            get
            {
                return CoilSignal.SignalName.ToString();
            }
        }
        private static readonly PropertyChangedEventArgs m_CoilNameArgs =
            NotifyPropertyChangedHelper.CreateArgs<InstructionLDCoil>(o => o.CoilName);

        public string CoilDescription
        {
            get
            {
                return CoilSignal.Comment.ToString();
            }
        }
        private static readonly PropertyChangedEventArgs m_CoilDescriptionArgs =
            NotifyPropertyChangedHelper.CreateArgs<InstructionLDCoil>(o => o.CoilDescription);

        protected override void setItems()
        {
            // this means the Instruction changed
            CoilNameEditor.Text = CoilName;
            CoilDescriptionEditor.Text = CoilDescription;
            calculateVerticalRungOffset();
            NotifyPropertyChanged(m_CoilNameArgs);
            NotifyPropertyChanged(m_CoilDescriptionArgs);
        }

        double m_VerticalRungOffset = -1;
        public override double VerticalRungOffset
        {
            get
            {
                if (m_VerticalRungOffset < 0)
                {
                    calculateVerticalRungOffset();
                }
                return m_VerticalRungOffset;
            }
            set
            {
            }
        }
        private static readonly PropertyChangedEventArgs m_VerticalRungOffsetArgs =
            NotifyPropertyChangedHelper.CreateArgs<InstructionLDCoil>(o => o.VerticalRungOffset);

        private void calculateVerticalRungOffset()
        {
            // It's the size of the description, plus the coil name, plus half the coil size
            m_VerticalRungOffset = COIL_DIAMETER / 2;
            m_VerticalRungOffset += CoilDescriptionEditor.ActualHeight;
            m_VerticalRungOffset += CoilNameEditor.ActualHeight;
            NotifyPropertyChanged(m_VerticalRungOffsetArgs);
        }

        #region " CoilNameEditor "
        public SignalNameEditor CoilNameEditor
        {
            get
            {
                if (m_CoilNameEditor == null)
                {
                    m_CoilNameEditor = new SignalNameEditor(CoilName, MAX_WIDTH, TextAlignment.Center);
                    m_CoilNameEditor.PropertyChanged += new PropertyChangedEventHandler(m_CoilNameEditor_PropertyChanged);
                }
                return m_CoilNameEditor;
            }
        }
        private SignalNameEditor m_CoilNameEditor = null;

        void m_CoilNameEditor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == m_SignalNameEditor_ActualHeightName)
            {
                calculateVerticalRungOffset();
            }
            else if (e.PropertyName == m_SignalNameEditor_TextName
                && CoilName != CoilNameEditor.Text)
            {
                var oldSignal = Instruction.NodeSignalChildren.Items[0];
                var newSignal = oldSignal.SetSignalName(new FieldSignalName(CoilNameEditor.Text));
                var newInstruction = Instruction.NodeSignalChildren.Replace(oldSignal, newSignal);
                SimpleUndoableInstructionEdit(newInstruction, Resources.Strings.Undo_Action_EditCoilName);
            }
        }
        static readonly string m_SignalNameEditor_ActualHeightName =
            NotifyPropertyChangedHelper.GetPropertyName<SignalNameEditor>(o => o.ActualHeight);
        static readonly string m_SignalNameEditor_TextName =
            NotifyPropertyChangedHelper.GetPropertyName<SignalNameEditor>(o => o.Text);
        #endregion

        #region " CoilDescriptionEditor "
        public SignalDescriptionEditor CoilDescriptionEditor
        {
            get
            {
                if (m_CoilDescriptionEditor == null)
                {
                    m_CoilDescriptionEditor = new SignalDescriptionEditor(CoilDescription, MAX_WIDTH, MAX_DESCRIPTION_HEIGHT, TextAlignment.Center, true);
                    m_CoilDescriptionEditor.PropertyChanged += new PropertyChangedEventHandler(m_CoilDescriptionEditor_PropertyChanged);
                }
                return m_CoilDescriptionEditor;
            }
        }
        private SignalDescriptionEditor m_CoilDescriptionEditor = null;

        void m_CoilDescriptionEditor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == m_SignalDescriptionEditor_ActualHeightName)
            {
                calculateVerticalRungOffset();
            }
            else if (e.PropertyName == m_SignalDescriptionEditor_TextName
                && CoilDescription != CoilDescriptionEditor.Text)
            {
                var oldSignal = Instruction.NodeSignalChildren.Items[0];
                var newSignal = oldSignal.SetComment(new FieldString(CoilDescriptionEditor.Text));
                var newInstruction = Instruction.NodeSignalChildren.Replace(oldSignal, newSignal);
                SimpleUndoableInstructionEdit(newInstruction, Resources.Strings.Undo_Action_EditCoilDescription);
            }
        }
        static readonly string m_SignalDescriptionEditor_ActualHeightName =
            NotifyPropertyChangedHelper.GetPropertyName<SignalDescriptionEditor>(o => o.ActualHeight);
        static readonly string m_SignalDescriptionEditor_TextName =
            NotifyPropertyChangedHelper.GetPropertyName<SignalDescriptionEditor>(o => o.Text);
        #endregion

    }
}
