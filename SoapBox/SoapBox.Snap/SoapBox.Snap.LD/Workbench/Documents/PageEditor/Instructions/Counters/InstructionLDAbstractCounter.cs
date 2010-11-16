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

namespace SoapBox.Snap.LD
{
    public abstract class InstructionLDAbstractCounter : AbstractLDInstructionItem
    {
        public const double RUNG_IN_OFFSET = 10;
        public const double MAX_WIDTH = 150;
        public const double MAX_DESCRIPTION_HEIGHT = 70;
        public const string SEPARATOR = ".";

        protected InstructionLDAbstractCounter(FieldInstructionType instructionType)
            : base(null, instructionType)
        {
        }

        protected InstructionLDAbstractCounter(IEditorItem parent, NodeInstruction instruction, FieldInstructionType instructionType)
            : base(parent, instructionType)
        {
            if (instruction == null)
            {
                var newInstruction = NodeInstruction.BuildWith(InstructionType);
                // Output signal: Named boolean - the counter done signal
                newInstruction = newInstruction.NodeSignalChildren.Append(
                    NodeSignal.BuildWith(
                        new FieldSignalName(Resources.Strings.LD_Snap_Ctr_DefaultName),
                        new FieldDataType(FieldDataType.DataTypeEnum.BOOL),
                        new FieldBool(false), // not forced
                        FieldConstant.Constants.BOOL.LOW)
                    );
                newInstruction = newInstruction.NodeSignalChildren.Append(
                    NodeSignal.BuildWith(
                        new FieldSignalName(Resources.Strings.LD_Snap_Ctr_DefaultName + SEPARATOR + Resources.Strings.LD_Snap_Ctr_CountName),
                        new FieldDataType(FieldDataType.DataTypeEnum.NUMBER),
                        new FieldBool(false), // not forced
                        FieldConstant.Constants.NUMBER.ZERO)
                    );
                newInstruction = newInstruction.NodeSignalChildren.Append(
                    NodeSignal.BuildWith(
                        new FieldSignalName(Resources.Strings.LD_Snap_Ctr_DefaultName + SEPARATOR + Resources.Strings.LD_Snap_Ctr_OneshotStateName),
                        new FieldDataType(FieldDataType.DataTypeEnum.BOOL),
                        new FieldBool(false), // not forced
                        FieldConstant.Constants.BOOL.LOW)
                    );
                // Input signal: setpoint
                newInstruction = newInstruction.NodeSignalInChildren.Append(
                    NodeSignalIn.BuildWith(
                        new FieldDataType(FieldDataType.DataTypeEnum.NUMBER),
                        new FieldConstant(FieldDataType.DataTypeEnum.NUMBER, 0)));
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

        private static IMenuItem m_staticMenuItemSeparator = new ConcreteMenuItemSeparator();

        protected abstract IExtensionService extensionService { get; set; }
        protected abstract IEnumerable<IMenuItem> ldInstructionContextMenu { get; set; }
        protected abstract IEnumerable<IMenuItem> contextMenu { get; set; }
        public abstract string InstructionName { get; }

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
            NotifyPropertyChangedHelper.CreateArgs<InstructionLDAbstractCounter>(o => o.CtrName);

        public string CtrDescription
        {
            get
            {
                return CoilSignal.Comment.ToString();
            }
        }
        protected static readonly PropertyChangedEventArgs m_CtrDescriptionArgs =
            NotifyPropertyChangedHelper.CreateArgs<InstructionLDAbstractCounter>(o => o.CtrDescription);

        protected override void setItems()
        {
            // this means the Instruction changed
            CtrNameEditor.Text = CtrName;
            CtrDescriptionEditor.Text = CtrDescription;
            calculateVerticalRungOffset();
            NotifyPropertyChanged(m_CtrNameArgs);
            NotifyPropertyChanged(m_CtrDescriptionArgs);
        }

        #region " SetpointSignalIn "
        public NodeSignalIn SetpointSignalIn
        {
            get
            {
                return Instruction.NodeSignalInChildren[0];
            }
        }
        private static readonly PropertyChangedEventArgs m_SetpointSignalInArgs =
            NotifyPropertyChangedHelper.CreateArgs<InstructionLDAbstractCounter>(o => o.SetpointSignalIn);
        private static string m_SetpointSignalInName =
            NotifyPropertyChangedHelper.GetPropertyName<InstructionLDAbstractCounter>(o => o.SetpointSignalIn);
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
            NotifyPropertyChangedHelper.CreateArgs<InstructionLDAbstractCounter>(o => o.ResetSignalIn);
        private static string m_ResetSignalInName =
            NotifyPropertyChangedHelper.GetPropertyName<InstructionLDAbstractCounter>(o => o.ResetSignalIn);
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
            NotifyPropertyChangedHelper.CreateArgs<InstructionLDAbstractCounter>(o => o.VerticalRungOffset);

        private void calculateVerticalRungOffset()
        {
            // It's the size of the description, plus the Ctr name, plus half the Ctr size
            m_VerticalRungOffset = RUNG_IN_OFFSET;
            m_VerticalRungOffset += CtrDescriptionEditor.ActualHeight;
            m_VerticalRungOffset += CtrNameEditor.ActualHeight;
            NotifyPropertyChanged(m_VerticalRungOffsetArgs);
        }

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

        #region " SetpointSignalChooser "
        public SignalChooser SetpointSignalChooser
        {
            get
            {
                if (m_SetpointSignalChooser == null)
                {
                    m_SetpointSignalChooser = new SignalChooser(this, SetpointSignalIn, MAX_WIDTH, TextAlignment.Center);
                    m_SetpointSignalChooser.PropertyChanged += new PropertyChangedEventHandler(m_SetpointSignalChooser_PropertyChanged);
                }
                return m_SetpointSignalChooser;
            }
        }
        private SignalChooser m_SetpointSignalChooser = null;

        void m_SetpointSignalChooser_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == m_SetpointSignalChooser_SignalIdName)
            {
                var oldSignalIn = Instruction.NodeSignalInChildren.Items[0];
                var newSignalIn = SetpointSignalChooser.SignalIn;
                var newInstruction = Instruction = Instruction.NodeSignalInChildren.Replace(oldSignalIn, newSignalIn);
                SimpleUndoableInstructionEdit(newInstruction, Resources.Strings.Undo_Action_EditCtrSetpointSignal);
            }
        }
        static readonly string m_SetpointSignalChooser_SignalIdName =
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
                    m_CountSignalValue = new SignalValue(this, Instruction.NodeSignalChildren[1], MAX_WIDTH, TextAlignment.Center, "{0:0.}");
                }
                return m_CountSignalValue;
            }
        }
        private SignalValue m_CountSignalValue = null;

        #endregion

    }
}
