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
using SoapBox.Protocol.Automation;
using SoapBox.Protocol.Base;
using SoapBox.Core;
using System.ComponentModel;
using SoapBox.Utilities;
using System.Windows;

namespace SoapBox.Snap.LD
{
    [Export(Snap.ExtensionPoints.Workbench.Documents.PageEditor.InstructionItems,
           typeof(IInstructionItem))]
    [InstructionItem(
        Language = Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD,
        Library = Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.Snap,
        Code = Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.Snap_.CntUD,
        SortOrder = Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.Snap_.CntUD_SortOrder,
        SpriteType = typeof(Resources.Images), SpriteKey = "SprCntUD",
        ToolTipType = typeof(Resources.Strings), ToolTipKey = "ToolTipCntUD")]
    public class InstructionLDCntUD : AbstractLDInstructionItem
    {
        public const double RUNG_IN_OFFSET = 10;
        public const double MAX_WIDTH = 150;
        public const double MAX_DESCRIPTION_HEIGHT = 70;
        public const string SEPARATOR = ".";

        internal static readonly FieldInstructionType m_InstructionType = new FieldInstructionType(
                    new FieldIdentifier(Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD),
                    new FieldIdentifier(Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.Snap),
                    new FieldIdentifier(Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.Snap_.CntUD));

        /// <summary>
        /// Just here for MEF to call, to do the imports
        /// </summary>
        internal InstructionLDCntUD()
            : base(null, m_InstructionType)
        {
        }

        private InstructionLDCntUD(IEditorItem parent, NodeInstruction instruction)
            : base(parent, m_InstructionType)
        {
            if (instruction == null)
            {
                var newInstruction = NodeInstruction.BuildWith(InstructionType);
                // Output signal: Named boolean - the counter equals-zero signal
                newInstruction = newInstruction.NodeSignalChildren.Append(
                    NodeSignal.BuildWith(
                        new FieldSignalName(Resources.Strings.LD_Snap_Ctr_DefaultName),
                        new FieldDataType(FieldDataType.DataTypeEnum.BOOL),
                        new FieldBool(false), // not forced
                        FieldConstant.Constants.BOOL.LOW)
                    );
                newInstruction = newInstruction.NodeSignalChildren.Append( // memory of counter value
                    NodeSignal.BuildWith(
                        new FieldSignalName(Resources.Strings.LD_Snap_Ctr_DefaultName + SEPARATOR + Resources.Strings.LD_Snap_Ctr_CountName),
                        new FieldDataType(FieldDataType.DataTypeEnum.NUMBER),
                        new FieldBool(false), // not forced
                        FieldConstant.Constants.NUMBER.ZERO)
                    );
                newInstruction = newInstruction.NodeSignalChildren.Append( // saves previous rung-in (count-up) state
                    NodeSignal.BuildWith(
                        new FieldSignalName(Resources.Strings.LD_Snap_Ctr_DefaultName + SEPARATOR + Resources.Strings.LD_Snap_Ctr_OneshotStateName),
                        new FieldDataType(FieldDataType.DataTypeEnum.BOOL),
                        new FieldBool(false), // not forced
                        FieldConstant.Constants.BOOL.LOW)
                    );
                newInstruction = newInstruction.NodeSignalChildren.Append( // saves previous count-down state
                    NodeSignal.BuildWith(
                        new FieldSignalName(Resources.Strings.LD_Snap_Ctr_DefaultName + SEPARATOR + Resources.Strings.LD_Snap_Ctr_CountDownStateName),
                        new FieldDataType(FieldDataType.DataTypeEnum.BOOL),
                        new FieldBool(false), // not forced
                        FieldConstant.Constants.BOOL.LOW)
                    );
                // Input signal: count-down
                newInstruction = newInstruction.NodeSignalInChildren.Append(
                    NodeSignalIn.BuildWith(
                        new FieldDataType(FieldDataType.DataTypeEnum.BOOL),
                        new FieldConstant(FieldDataType.DataTypeEnum.BOOL, false)));
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
                    throw new InvalidOperationException("Tried to instantiate a Counter but passed a different instruction type.");
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

        public string InstructionName
        {
            get 
            {
                return Resources.Strings.LD_Snap_CntUD_InstructionName;
            }
        }

        private static IMenuItem m_staticMenuItemSeparator = new ConcreteMenuItemSeparator();

        #region "extensionServiceSingleton"
        [Import(SoapBox.Core.Services.Host.ExtensionService)]
        protected IExtensionService extensionService
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
        protected IEnumerable<IMenuItem> ldInstructionContextMenu
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
        [ImportMany(ExtensionPoints.Instructions.CntUD.ContextMenu, typeof(IMenuItem))]
        protected IEnumerable<IMenuItem> contextMenu
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
            return new InstructionLDCntUD(parent, instruction);
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
            NotifyPropertyChangedHelper.CreateArgs<InstructionLDAbstractCounter>(o => o.CoilSignal);

        public string CtrName
        {
            get
            {
                return CoilSignal.SignalName.ToString();
            }
        }
        protected static readonly PropertyChangedEventArgs m_CtrNameArgs =
            NotifyPropertyChangedHelper.CreateArgs<InstructionLDCntUD>(o => o.CtrName);

        public string CtrDescription
        {
            get
            {
                return CoilSignal.Comment.ToString();
            }
        }
        protected static readonly PropertyChangedEventArgs m_CtrDescriptionArgs =
            NotifyPropertyChangedHelper.CreateArgs<InstructionLDCntUD>(o => o.CtrDescription);

        protected override void setItems()
        {
            // this means the Instruction changed
            CtrNameEditor.Text = CtrName;
            CtrDescriptionEditor.Text = CtrDescription;
            calculateVerticalRungOffset();
            NotifyPropertyChanged(m_CtrNameArgs);
            NotifyPropertyChanged(m_CtrDescriptionArgs);
        }

        #region " CountDownSignalIn "
        public NodeSignalIn CountDownSignalIn
        {
            get
            {
                return Instruction.NodeSignalInChildren[0];
            }
        }
        private static readonly PropertyChangedEventArgs m_CountDownSignalInArgs =
            NotifyPropertyChangedHelper.CreateArgs<InstructionLDCntUD>(o => o.CountDownSignalIn);
        private static string m_CountDownSignalInName =
            NotifyPropertyChangedHelper.GetPropertyName<InstructionLDCntUD>(o => o.CountDownSignalIn);
        #endregion

        #region " ResetSignalIn "
        public NodeSignalIn ResetSignalIn
        {
            get
            {
                return Instruction.NodeSignalInChildren[1];
            }
        }
        private static readonly PropertyChangedEventArgs m_ResetSignalInArgs =
            NotifyPropertyChangedHelper.CreateArgs<InstructionLDCntUD>(o => o.ResetSignalIn);
        private static string m_ResetSignalInName =
            NotifyPropertyChangedHelper.GetPropertyName<InstructionLDCntUD>(o => o.ResetSignalIn);
        #endregion

        #region " CtrNameEditor "
        public SignalNameEditor CtrNameEditor
        {
            get
            {
                if (m_CtrNameEditor == null)
                {
                    m_CtrNameEditor = new SignalNameEditor(CtrName, MAX_WIDTH, TextAlignment.Center);
                    m_CtrNameEditor.PropertyChanged += new PropertyChangedEventHandler(m_CtrNameEditor_PropertyChanged);
                }
                return m_CtrNameEditor;
            }
        }
        private SignalNameEditor m_CtrNameEditor = null;

        void m_CtrNameEditor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == m_SignalNameEditor_ActualHeightName)
            {
                calculateVerticalRungOffset();
            }
            else if (e.PropertyName == m_SignalNameEditor_TextName
                && CtrName != CtrNameEditor.Text)
            {
                var oldSignal = Instruction.NodeSignalChildren.Items[0];
                var newSignal = oldSignal.SetSignalName(new FieldSignalName(CtrNameEditor.Text));
                var newInstruction = Instruction.NodeSignalChildren.Replace(oldSignal, newSignal);

                // update the count name (it's a function of the Counter name)
                var oldCountSignal = newInstruction.NodeSignalChildren.Items[1];
                var newCountSignal = oldCountSignal.SetSignalName(new FieldSignalName(CtrNameEditor.Text + SEPARATOR + Resources.Strings.LD_Snap_Ctr_CountName));
                newInstruction = newInstruction.NodeSignalChildren.Replace(oldCountSignal, newCountSignal);

                // update the oneshot state name (it's a function of the Counter name)
                var oldOneShotStateSignal = newInstruction.NodeSignalChildren.Items[2];
                var newOneShotStateSignal = oldOneShotStateSignal.SetSignalName(new FieldSignalName(CtrNameEditor.Text + SEPARATOR + Resources.Strings.LD_Snap_Ctr_OneshotStateName));
                newInstruction = newInstruction.NodeSignalChildren.Replace(oldOneShotStateSignal, newOneShotStateSignal);

                SimpleUndoableInstructionEdit(newInstruction, Resources.Strings.Undo_Action_EditCtrName);
            }
        }
        static readonly string m_SignalNameEditor_ActualHeightName =
            NotifyPropertyChangedHelper.GetPropertyName<SignalNameEditor>(o => o.ActualHeight);
        static readonly string m_SignalNameEditor_TextName =
            NotifyPropertyChangedHelper.GetPropertyName<SignalNameEditor>(o => o.Text);
        #endregion

        #region " CtrDescriptionEditor "
        public SignalDescriptionEditor CtrDescriptionEditor
        {
            get
            {
                if (m_CtrDescriptionEditor == null)
                {
                    m_CtrDescriptionEditor = new SignalDescriptionEditor(CtrDescription, MAX_WIDTH, MAX_DESCRIPTION_HEIGHT, TextAlignment.Center, true);
                    m_CtrDescriptionEditor.PropertyChanged += new PropertyChangedEventHandler(m_CtrDescriptionEditor_PropertyChanged);
                }
                return m_CtrDescriptionEditor;
            }
        }
        private SignalDescriptionEditor m_CtrDescriptionEditor = null;

        void m_CtrDescriptionEditor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == m_SignalDescriptionEditor_ActualHeightName)
            {
                calculateVerticalRungOffset();
            }
            else if (e.PropertyName == m_SignalDescriptionEditor_TextName
                && CtrDescription != CtrDescriptionEditor.Text)
            {
                var oldSignal = Instruction.NodeSignalChildren.Items[0];
                var newSignal = oldSignal.SetComment(new FieldString(CtrDescriptionEditor.Text));
                var newInstruction = Instruction.NodeSignalChildren.Replace(oldSignal, newSignal);
                SimpleUndoableInstructionEdit(newInstruction, Resources.Strings.Undo_Action_EditCtrDescription);
            }
        }
        static readonly string m_SignalDescriptionEditor_ActualHeightName =
            NotifyPropertyChangedHelper.GetPropertyName<SignalDescriptionEditor>(o => o.ActualHeight);
        static readonly string m_SignalDescriptionEditor_TextName =
            NotifyPropertyChangedHelper.GetPropertyName<SignalDescriptionEditor>(o => o.Text);
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
            NotifyPropertyChangedHelper.CreateArgs<InstructionLDCntUD>(o => o.VerticalRungOffset);

        private void calculateVerticalRungOffset()
        {
            // It's the size of the description, plus the Ctr name, plus half the Ctr size
            m_VerticalRungOffset = RUNG_IN_OFFSET;
            m_VerticalRungOffset += CtrDescriptionEditor.ActualHeight;
            m_VerticalRungOffset += CtrNameEditor.ActualHeight;
            NotifyPropertyChanged(m_VerticalRungOffsetArgs);
        }

        #region " CountDownSignalChooser "
        public SignalChooser CountDownSignalChooser
        {
            get
            {
                if (m_CountDownSignalChooser == null)
                {
                    m_CountDownSignalChooser = new SignalChooser(this, CountDownSignalIn, MAX_WIDTH, TextAlignment.Center);
                    m_CountDownSignalChooser.PropertyChanged += new PropertyChangedEventHandler(m_CountDownSignalChooser_PropertyChanged);
                }
                return m_CountDownSignalChooser;
            }
        }
        private SignalChooser m_CountDownSignalChooser = null;

        void m_CountDownSignalChooser_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == m_CountDownSignalChooser_SignalIdName)
            {
                var oldSignalIn = Instruction.NodeSignalInChildren.Items[0];
                var newSignalIn = CountDownSignalChooser.SignalIn;
                var newInstruction = Instruction = Instruction.NodeSignalInChildren.Replace(oldSignalIn, newSignalIn);
                SimpleUndoableInstructionEdit(newInstruction, Resources.Strings.Undo_Action_EditCountDownSignal);
            }
        }
        static readonly string m_CountDownSignalChooser_SignalIdName =
            NotifyPropertyChangedHelper.GetPropertyName<SignalChooser>(o => o.SignalIn);
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
                var oldSignalIn = Instruction.NodeSignalInChildren.Items[1];
                var newSignalIn = ResetSignalChooser.SignalIn;
                var newInstruction = Instruction = Instruction.NodeSignalInChildren.Replace(oldSignalIn, newSignalIn);
                SimpleUndoableInstructionEdit(newInstruction, Resources.Strings.Undo_Action_EditResetSignal);
            }
        }
        static readonly string m_ResetSignalChooser_SignalIdName =
            NotifyPropertyChangedHelper.GetPropertyName<SignalChooser>(o => o.SignalIn);
        #endregion

        #region " CountSignalValue "
        public SignalValue CountSignalValue
        {
            get
            {
                if (m_CountSignalValue == null)
                {
                    m_CountSignalValue = new SignalValue(this, Instruction.NodeSignalChildren[1], MAX_WIDTH, TextAlignment.Center, "{0:0.##############################}");
                }
                return m_CountSignalValue;
            }
        }
        private SignalValue m_CountSignalValue = null;

        #endregion
    }
}
