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
        Code = Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.Snap_.StringContains,
        SortOrder = Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.Snap_.StringContains_SortOrder,
        SpriteType = typeof(Resources.Images), SpriteKey = "SprStringContains",
        ToolTipType = typeof(Resources.Strings), ToolTipKey = "ToolTipStringContains")]
    public class InstructionLDStringContains : AbstractLDInstructionItem
    {
        public const double RUNG_IN_OFFSET = 10;
        public const double MAX_WIDTH = 150;
        public const double MAX_DESCRIPTION_HEIGHT = 70;
        public const string SEPARATOR = ".";

        internal static readonly FieldInstructionType m_InstructionType = new FieldInstructionType(
                   new FieldIdentifier(Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD),
                   new FieldIdentifier(Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.Snap),
                   new FieldIdentifier(Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.Snap_.StringContains));

        protected InstructionLDStringContains()
            : base(null, m_InstructionType)
        {
        }

        protected InstructionLDStringContains(IEditorItem parent, NodeInstruction instruction)
            : base(parent, m_InstructionType)
        {
            if (instruction == null)
            {
                var newInstruction = NodeInstruction.BuildWith(InstructionType);
                // Output signal: Named boolean - result of the comparison
                newInstruction = newInstruction.NodeSignalChildren.Append(
                    NodeSignal.BuildWith(
                        new FieldSignalName(Resources.Strings.LD_Snap_StringContains_DefaultName),
                        new FieldDataType(FieldDataType.DataTypeEnum.BOOL),
                        new FieldBool(false), // not forced
                        FieldConstant.Constants.BOOL.LOW)
                    );
                // Input signal: string to search
                newInstruction = newInstruction.NodeSignalInChildren.Append(
                    NodeSignalIn.BuildWith(
                        new FieldDataType(FieldDataType.DataTypeEnum.STRING),
                        new FieldConstant(FieldDataType.DataTypeEnum.STRING, string.Empty)));
                // Input signal: string to find
                newInstruction = newInstruction.NodeSignalInChildren.Append(
                    NodeSignalIn.BuildWith(
                        new FieldDataType(FieldDataType.DataTypeEnum.STRING),
                        new FieldConstant(FieldDataType.DataTypeEnum.STRING, string.Empty)));
                // Input signal: case sensitive
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
                    throw new InvalidOperationException("Tried to instantiate a StringContains but passed a different instruction type.");
                }
                Instruction = instruction;
            }

            // Build the context menu
            ContextMenu = extensionService.SortAndJoin(ldInstructionContextMenu, m_staticMenuItemSeparator, contextMenu);
            ContextMenuEnabled = true;
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
        [ImportMany(ExtensionPoints.Instructions.StringContains.ContextMenu, typeof(IMenuItem))]
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
            return new InstructionLDStringContains(parent, instruction);
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
            NotifyPropertyChangedHelper.CreateArgs<InstructionLDStringContains>(o => o.CoilSignal);

        public string StringContainsName
        {
            get
            {
                return CoilSignal.SignalName.ToString();
            }
        }
        protected static readonly PropertyChangedEventArgs m_StringContainsNameArgs =
            NotifyPropertyChangedHelper.CreateArgs<InstructionLDStringContains>(o => o.StringContainsName);

        public string StringContainsDescription
        {
            get
            {
                return CoilSignal.Comment.ToString();
            }
        }
        protected static readonly PropertyChangedEventArgs m_StringContainsDescriptionArgs =
            NotifyPropertyChangedHelper.CreateArgs<InstructionLDStringContains>(o => o.StringContainsDescription);

        protected override void setItems()
        {
            // this means the Instruction changed
            StringContainsNameEditor.Text = StringContainsName;
            StringContainsDescriptionEditor.Text = StringContainsDescription;
            calculateVerticalRungOffset();
            NotifyPropertyChanged(m_StringContainsNameArgs);
            NotifyPropertyChanged(m_StringContainsDescriptionArgs);
        }

        #region " StringToSearchSignalIn "
        public NodeSignalIn StringToSearchSignalIn
        {
            get
            {
                return Instruction.NodeSignalInChildren[0];
            }
        }
        private static readonly PropertyChangedEventArgs m_StringToSearchSignalInArgs =
            NotifyPropertyChangedHelper.CreateArgs<InstructionLDStringContains>(o => o.StringToSearchSignalIn);
        private static string m_StringToSearchSignalInName =
            NotifyPropertyChangedHelper.GetPropertyName<InstructionLDStringContains>(o => o.StringToSearchSignalIn);
        #endregion

        #region " StringToFindSignalIn "
        public NodeSignalIn StringToFindSignalIn
        {
            get
            {
                return Instruction.NodeSignalInChildren[1];
            }
        }
        private static readonly PropertyChangedEventArgs m_StringToFindSignalInArgs =
            NotifyPropertyChangedHelper.CreateArgs<InstructionLDStringContains>(o => o.StringToFindSignalIn);
        private static string m_StringToFindSignalInName =
            NotifyPropertyChangedHelper.GetPropertyName<InstructionLDStringContains>(o => o.StringToFindSignalIn);
        #endregion

        #region " CaseSensitiveSignalIn "
        public NodeSignalIn CaseSensitiveSignalIn
        {
            get
            {
                return Instruction.NodeSignalInChildren[2];
            }
        }
        private static readonly PropertyChangedEventArgs m_CaseSensitiveSignalInArgs =
            NotifyPropertyChangedHelper.CreateArgs<InstructionLDStringContains>(o => o.CaseSensitiveSignalIn);
        private static string m_CaseSensitiveSignalInName =
            NotifyPropertyChangedHelper.GetPropertyName<InstructionLDStringContains>(o => o.CaseSensitiveSignalIn);
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
            NotifyPropertyChangedHelper.CreateArgs<InstructionLDStringContains>(o => o.VerticalRungOffset);

        private void calculateVerticalRungOffset()
        {
            // It's the size of the description, plus the StringContains name, plus half the StringContains size
            m_VerticalRungOffset = RUNG_IN_OFFSET;
            m_VerticalRungOffset += StringContainsDescriptionEditor.ActualHeight;
            m_VerticalRungOffset += StringContainsNameEditor.ActualHeight;
            NotifyPropertyChanged(m_VerticalRungOffsetArgs);
        }

        #region " StringContainsNameEditor "
        public SignalNameEditor StringContainsNameEditor
        {
            get
            {
                if (m_StringContainsNameEditor == null)
                {
                    m_StringContainsNameEditor = new SignalNameEditor(StringContainsName, MAX_WIDTH, TextAlignment.Center);
                    m_StringContainsNameEditor.PropertyChanged += new PropertyChangedEventHandler(m_StringContainsNameEditor_PropertyChanged);
                }
                return m_StringContainsNameEditor;
            }
        }
        private SignalNameEditor m_StringContainsNameEditor = null;

        void m_StringContainsNameEditor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == m_SignalNameEditor_ActualHeightName)
            {
                calculateVerticalRungOffset();
            }
            else if (e.PropertyName == m_SignalNameEditor_TextName
                && StringContainsName != StringContainsNameEditor.Text)
            {
                var oldSignal = Instruction.NodeSignalChildren.Items[0];
                var newSignal = oldSignal.SetSignalName(new FieldSignalName(StringContainsNameEditor.Text));
                var newInstruction = Instruction.NodeSignalChildren.Replace(oldSignal, newSignal);

                SimpleUndoableInstructionEdit(newInstruction, Resources.Strings.Undo_Action_EditStringContainsName);
            }
        }
        static readonly string m_SignalNameEditor_ActualHeightName =
            NotifyPropertyChangedHelper.GetPropertyName<SignalNameEditor>(o => o.ActualHeight);
        static readonly string m_SignalNameEditor_TextName =
            NotifyPropertyChangedHelper.GetPropertyName<SignalNameEditor>(o => o.Text);
        #endregion

        #region " StringContainsDescriptionEditor "
        public SignalDescriptionEditor StringContainsDescriptionEditor
        {
            get
            {
                if (m_StringContainsDescriptionEditor == null)
                {
                    m_StringContainsDescriptionEditor = new SignalDescriptionEditor(StringContainsDescription, MAX_WIDTH, MAX_DESCRIPTION_HEIGHT, TextAlignment.Center, true);
                    m_StringContainsDescriptionEditor.PropertyChanged += new PropertyChangedEventHandler(m_StringContainsDescriptionEditor_PropertyChanged);
                }
                return m_StringContainsDescriptionEditor;
            }
        }
        private SignalDescriptionEditor m_StringContainsDescriptionEditor = null;

        void m_StringContainsDescriptionEditor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == m_SignalDescriptionEditor_ActualHeightName)
            {
                calculateVerticalRungOffset();
            }
            else if (e.PropertyName == m_SignalDescriptionEditor_TextName
                && StringContainsDescription != StringContainsDescriptionEditor.Text)
            {
                var oldSignal = Instruction.NodeSignalChildren.Items[0];
                var newSignal = oldSignal.SetComment(new FieldString(StringContainsDescriptionEditor.Text));
                var newInstruction = Instruction.NodeSignalChildren.Replace(oldSignal, newSignal);
                SimpleUndoableInstructionEdit(newInstruction, Resources.Strings.Undo_Action_EditStringContainsDescription);
            }
        }
        static readonly string m_SignalDescriptionEditor_ActualHeightName =
            NotifyPropertyChangedHelper.GetPropertyName<SignalDescriptionEditor>(o => o.ActualHeight);
        static readonly string m_SignalDescriptionEditor_TextName =
            NotifyPropertyChangedHelper.GetPropertyName<SignalDescriptionEditor>(o => o.Text);
        #endregion

        #region " StringToSearchSignalChooser "
        public SignalChooser StringToSearchSignalChooser
        {
            get
            {
                if (m_StringToSearchSignalChooser == null)
                {
                    m_StringToSearchSignalChooser = new SignalChooser(this, StringToSearchSignalIn, MAX_WIDTH, TextAlignment.Center);
                    m_StringToSearchSignalChooser.PropertyChanged += new PropertyChangedEventHandler(m_StringToSearchSignalChooser_PropertyChanged);
                }
                return m_StringToSearchSignalChooser;
            }
        }
        private SignalChooser m_StringToSearchSignalChooser = null;

        void m_StringToSearchSignalChooser_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == m_StringToSearchSignalChooser_SignalIdName)
            {
                var oldSignalIn = Instruction.NodeSignalInChildren.Items[0];
                var newSignalIn = StringToSearchSignalChooser.SignalIn;
                var newInstruction = Instruction = Instruction.NodeSignalInChildren.Replace(oldSignalIn, newSignalIn);
                SimpleUndoableInstructionEdit(newInstruction, Resources.Strings.Undo_Action_EditStringContainsSignal);
            }
        }
        static readonly string m_StringToSearchSignalChooser_SignalIdName =
            NotifyPropertyChangedHelper.GetPropertyName<SignalChooser>(o => o.SignalIn);
        #endregion

        #region " StringToFindSignalChooser "
        public SignalChooser StringToFindSignalChooser
        {
            get
            {
                if (m_StringToFindSignalChooser == null)
                {
                    m_StringToFindSignalChooser = new SignalChooser(this, StringToFindSignalIn, MAX_WIDTH, TextAlignment.Center);
                    m_StringToFindSignalChooser.PropertyChanged += new PropertyChangedEventHandler(m_StringToFindSignalChooser_PropertyChanged);
                }
                return m_StringToFindSignalChooser;
            }
        }
        private SignalChooser m_StringToFindSignalChooser = null;

        void m_StringToFindSignalChooser_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == m_StringToFindSignalChooser_SignalIdName)
            {
                var oldSignalIn = Instruction.NodeSignalInChildren.Items[1];
                var newSignalIn = StringToFindSignalChooser.SignalIn;
                var newInstruction = Instruction = Instruction.NodeSignalInChildren.Replace(oldSignalIn, newSignalIn);
                SimpleUndoableInstructionEdit(newInstruction, Resources.Strings.Undo_Action_EditStringContainsSignal);
            }
        }
        static readonly string m_StringToFindSignalChooser_SignalIdName =
            NotifyPropertyChangedHelper.GetPropertyName<SignalChooser>(o => o.SignalIn);
        #endregion

        #region " CaseSensitiveSignalChooser "
        public SignalChooser CaseSensitiveSignalChooser
        {
            get
            {
                if (m_CaseSensitiveSignalChooser == null)
                {
                    m_CaseSensitiveSignalChooser = new SignalChooser(this, CaseSensitiveSignalIn, MAX_WIDTH, TextAlignment.Center);
                    m_CaseSensitiveSignalChooser.PropertyChanged += new PropertyChangedEventHandler(m_CaseSensitiveSignalChooser_PropertyChanged);
                }
                return m_CaseSensitiveSignalChooser;
            }
        }
        private SignalChooser m_CaseSensitiveSignalChooser = null;

        void m_CaseSensitiveSignalChooser_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == m_CaseSensitiveSignalChooser_SignalIdName)
            {
                var oldSignalIn = Instruction.NodeSignalInChildren.Items[2];
                var newSignalIn = CaseSensitiveSignalChooser.SignalIn;
                var newInstruction = Instruction = Instruction.NodeSignalInChildren.Replace(oldSignalIn, newSignalIn);
                SimpleUndoableInstructionEdit(newInstruction, Resources.Strings.Undo_Action_EditStringContainsSignal);
            }
        }
        static readonly string m_CaseSensitiveSignalChooser_SignalIdName =
            NotifyPropertyChangedHelper.GetPropertyName<SignalChooser>(o => o.SignalIn);
        #endregion

    }
}
