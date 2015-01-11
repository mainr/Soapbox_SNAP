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
using SoapBox.Core;
using System.ComponentModel;
using SoapBox.Utilities;
using System.Windows;
using SoapBox.Protocol.Base;
using System.ComponentModel.Composition;

namespace SoapBox.Snap.LD
{
    [Export(Snap.ExtensionPoints.Workbench.Documents.PageEditor.InstructionItems,
        typeof(IInstructionItem))]
    [InstructionItem(
        Language = Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD,
        Library = Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.Snap,
        Code = Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.Snap_.SetReset,
        SortOrder = Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.Snap_.SetReset_SortOrder,
        SpriteType = typeof(Resources.Images), SpriteKey = "SprSetReset",
        ToolTipType = typeof(Resources.Strings), ToolTipKey = "ToolTipSetReset")]
    public class InstructionLDSetReset : AbstractLDInstructionItem
    {
        public const double RUNG_IN_OFFSET = 10;
        public const double MAX_WIDTH = 150;
        public const double MAX_DESCRIPTION_HEIGHT = 70;

        internal static readonly FieldInstructionType m_InstructionType = new FieldInstructionType(
                   new FieldIdentifier(Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD),
                   new FieldIdentifier(Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.Snap),
                   new FieldIdentifier(Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.Snap_.SetReset));

        internal InstructionLDSetReset()
            : base(null, m_InstructionType)
        {
        }

        protected InstructionLDSetReset(IEditorItem parent, NodeInstruction instruction)
            : base(parent, m_InstructionType)
        {
            if (instruction == null)
            {
                var newInstruction = NodeInstruction.BuildWith(InstructionType);
                // Output signal: Named boolean - the latched state
                newInstruction = newInstruction.NodeSignalChildren.Append(
                    NodeSignal.BuildWith(
                        new FieldSignalName(Resources.Strings.LD_Snap_SetReset_DefaultName),
                        new FieldDataType(FieldDataType.DataTypeEnum.BOOL),
                        new FieldBool(false), // not forced
                        FieldConstant.Constants.BOOL.LOW)
                    );
                // Input signal: reset
                newInstruction = newInstruction.NodeSignalInChildren.Append(
                    NodeSignalIn.BuildWith(
                        new FieldDataType(FieldDataType.DataTypeEnum.BOOL),
                        new FieldConstant(FieldDataType.DataTypeEnum.BOOL, false)));
                Instruction = newInstruction;
            }
            else
            {
                if (instruction.InstructionType != InstructionType)
                {
                    throw new InvalidOperationException("Tried to instantiate a SetReset but passed a different instruction type.");
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
        [ImportMany(ExtensionPoints.Instructions.SetReset.ContextMenu, typeof(IMenuItem))]
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

        public override IInstructionItem Create(IEditorItem parent, NodeInstruction instruction)
        {
            return new InstructionLDSetReset(parent, instruction);
        }

        #region " IsRight "
        public override bool IsRight
        {
            get
            {
                return true;
            }
        }
        #endregion

        public NodeSignal CoilSignal
        {
            get
            {
                return Instruction.NodeSignalChildren.Items[0];
            }
        }
        private static readonly PropertyChangedEventArgs m_CoilSignalArgs =
            NotifyPropertyChangedHelper.CreateArgs<InstructionLDSetReset>(o => o.CoilSignal);

        public string SetResetName
        {
            get
            {
                return CoilSignal.SignalName.ToString();
            }
        }
        protected static readonly PropertyChangedEventArgs m_SetResetNameArgs =
            NotifyPropertyChangedHelper.CreateArgs<InstructionLDSetReset>(o => o.SetResetName);

        public string SetResetDescription
        {
            get
            {
                return CoilSignal.Comment.ToString();
            }
        }
        protected static readonly PropertyChangedEventArgs m_SetResetDescriptionArgs =
            NotifyPropertyChangedHelper.CreateArgs<InstructionLDSetReset>(o => o.SetResetDescription);

        protected override void setItems()
        {
            // this means the Instruction changed
            SetResetNameEditor.Text = SetResetName;
            SetResetDescriptionEditor.Text = SetResetDescription;
            calculateVerticalRungOffset();
            NotifyPropertyChanged(m_SetResetNameArgs);
            NotifyPropertyChanged(m_SetResetDescriptionArgs);
        }

        #region " ResetSignalIn "
        public NodeSignalIn ResetSignalIn
        {
            get
            {
                return Instruction.NodeSignalInChildren[0];
            }
        }
        private static readonly PropertyChangedEventArgs m_ResetSignalInArgs =
            NotifyPropertyChangedHelper.CreateArgs<InstructionLDSetReset>(o => o.ResetSignalIn);
        private static string m_ResetSignalInName =
            NotifyPropertyChangedHelper.GetPropertyName<InstructionLDSetReset>(o => o.ResetSignalIn);
        #endregion

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
            NotifyPropertyChangedHelper.CreateArgs<InstructionLDSetReset>(o => o.VerticalRungOffset);

        private void calculateVerticalRungOffset()
        {
            m_VerticalRungOffset = RUNG_IN_OFFSET;
            m_VerticalRungOffset += SetResetDescriptionEditor.ActualHeight;
            m_VerticalRungOffset += SetResetNameEditor.ActualHeight;
            NotifyPropertyChanged(m_VerticalRungOffsetArgs);
        }

        #region " SetResetNameEditor "
        public SignalNameEditor SetResetNameEditor
        {
            get
            {
                if (m_SetResetNameEditor == null)
                {
                    m_SetResetNameEditor = new SignalNameEditor(SetResetName, MAX_WIDTH, TextAlignment.Center);
                    m_SetResetNameEditor.PropertyChanged += new PropertyChangedEventHandler(m_SetResetNameEditor_PropertyChanged);
                }
                return m_SetResetNameEditor;
            }
        }
        private SignalNameEditor m_SetResetNameEditor = null;

        void m_SetResetNameEditor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == m_SignalNameEditor_ActualHeightName)
            {
                calculateVerticalRungOffset();
            }
            else if (e.PropertyName == m_SignalNameEditor_TextName
                && SetResetName != SetResetNameEditor.Text)
            {
                var oldSignal = Instruction.NodeSignalChildren.Items[0];
                var newSignal = oldSignal.SetSignalName(new FieldSignalName(SetResetNameEditor.Text));
                var newInstruction = Instruction.NodeSignalChildren.Replace(oldSignal, newSignal);

                SimpleUndoableInstructionEdit(newInstruction, Resources.Strings.Undo_Action_EditSetResetName);
            }
        }
        static readonly string m_SignalNameEditor_ActualHeightName =
            NotifyPropertyChangedHelper.GetPropertyName<SignalNameEditor>(o => o.ActualHeight);
        static readonly string m_SignalNameEditor_TextName =
            NotifyPropertyChangedHelper.GetPropertyName<SignalNameEditor>(o => o.Text);
        #endregion

        #region " SetResetDescriptionEditor "
        public SignalDescriptionEditor SetResetDescriptionEditor
        {
            get
            {
                if (m_SetResetDescriptionEditor == null)
                {
                    m_SetResetDescriptionEditor = new SignalDescriptionEditor(SetResetDescription, MAX_WIDTH, MAX_DESCRIPTION_HEIGHT, TextAlignment.Center, true);
                    m_SetResetDescriptionEditor.PropertyChanged += new PropertyChangedEventHandler(m_SetResetDescriptionEditor_PropertyChanged);
                }
                return m_SetResetDescriptionEditor;
            }
        }
        private SignalDescriptionEditor m_SetResetDescriptionEditor = null;

        void m_SetResetDescriptionEditor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == m_SignalDescriptionEditor_ActualHeightName)
            {
                calculateVerticalRungOffset();
            }
            else if (e.PropertyName == m_SignalDescriptionEditor_TextName
                && SetResetDescription != SetResetDescriptionEditor.Text)
            {
                var oldSignal = Instruction.NodeSignalChildren.Items[0];
                var newSignal = oldSignal.SetComment(new FieldString(SetResetDescriptionEditor.Text));
                var newInstruction = Instruction.NodeSignalChildren.Replace(oldSignal, newSignal);
                SimpleUndoableInstructionEdit(newInstruction, Resources.Strings.Undo_Action_EditSetResetDescription);
            }
        }
        static readonly string m_SignalDescriptionEditor_ActualHeightName =
            NotifyPropertyChangedHelper.GetPropertyName<SignalDescriptionEditor>(o => o.ActualHeight);
        static readonly string m_SignalDescriptionEditor_TextName =
            NotifyPropertyChangedHelper.GetPropertyName<SignalDescriptionEditor>(o => o.Text);
        #endregion

        #region " ResetSignalChooser "
        public SignalChooser ResetSignalChooser
        {
            get
            {
                if (m_ResetSignalChooser == null)
                {
                    m_ResetSignalChooser = new SignalChooser(this, ResetSignalIn, MAX_WIDTH, TextAlignment.Center);
                    m_ResetSignalChooser.PropertyChanged += new PropertyChangedEventHandler(m_ResetSignalChooser_PropertyChanged);
                }
                return m_ResetSignalChooser;
            }
        }
        private SignalChooser m_ResetSignalChooser = null;

        void m_ResetSignalChooser_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == m_ResetSignalChooser_SignalIdName)
            {
                var oldSignalIn = Instruction.NodeSignalInChildren.Items[0];
                var newSignalIn = ResetSignalChooser.SignalIn;
                var newInstruction = Instruction = Instruction.NodeSignalInChildren.Replace(oldSignalIn, newSignalIn);
                SimpleUndoableInstructionEdit(newInstruction, Resources.Strings.Undo_Action_EditSetResetSignal);
            }
        }
        static readonly string m_ResetSignalChooser_SignalIdName =
            NotifyPropertyChangedHelper.GetPropertyName<SignalChooser>(o => o.SignalIn);
        #endregion

    }
}
