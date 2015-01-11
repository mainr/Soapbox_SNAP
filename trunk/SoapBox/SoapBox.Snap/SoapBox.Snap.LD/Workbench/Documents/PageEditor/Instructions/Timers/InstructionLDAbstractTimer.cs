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

namespace SoapBox.Snap.LD
{
    public abstract class InstructionLDAbstractTimer : AbstractLDInstructionItem
    {
        public const double RUNG_IN_OFFSET = 10;
        public const double MAX_WIDTH = 150;
        public const double MAX_DESCRIPTION_HEIGHT = 70;
        public const string SEPARATOR = ".";

        protected InstructionLDAbstractTimer(FieldInstructionType instructionType)
            : base(null, instructionType)
        {
        }

        protected InstructionLDAbstractTimer(IEditorItem parent, NodeInstruction instruction, FieldInstructionType instructionType)
            : base(parent, instructionType)
        {
            if (instruction == null)
            {
                var newInstruction = NodeInstruction.BuildWith(InstructionType);
                // Output signal: Named boolean - the timer done signal
                newInstruction = newInstruction.NodeSignalChildren.Append(
                    NodeSignal.BuildWith(
                        new FieldSignalName(Resources.Strings.LD_Snap_Tmr_DefaultName),
                        new FieldDataType(FieldDataType.DataTypeEnum.BOOL),
                        new FieldBool(false), // not forced
                        FieldConstant.Constants.BOOL.LOW)
                    );
                newInstruction = newInstruction.NodeSignalChildren.Append(
                    NodeSignal.BuildWith(
                        new FieldSignalName(Resources.Strings.LD_Snap_Tmr_DefaultName + SEPARATOR + Resources.Strings.LD_Snap_Tmr_ElapsedName),
                        new FieldDataType(FieldDataType.DataTypeEnum.NUMBER),
                        new FieldBool(false), // not forced
                        FieldConstant.Constants.NUMBER.ZERO)
                    );
                // Input signal: setpoint
                newInstruction = newInstruction.NodeSignalInChildren.Append(
                    NodeSignalIn.BuildWith(
                        new FieldDataType(FieldDataType.DataTypeEnum.NUMBER),
                        new FieldConstant(FieldDataType.DataTypeEnum.NUMBER, 0)));
                Instruction = newInstruction;
            }
            else
            {
                if (instruction.InstructionType != InstructionType)
                {
                    throw new InvalidOperationException("Tried to instantiate a Timer but passed a different instruction type.");
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
            NotifyPropertyChangedHelper.CreateArgs<InstructionLDAbstractTimer>(o => o.CoilSignal);

        public string TmrName
        {
            get
            {
                return CoilSignal.SignalName.ToString();
            }
        }
        protected static readonly PropertyChangedEventArgs m_TmrNameArgs =
            NotifyPropertyChangedHelper.CreateArgs<InstructionLDAbstractTimer>(o => o.TmrName);

        public string TmrDescription
        {
            get
            {
                return CoilSignal.Comment.ToString();
            }
        }
        protected static readonly PropertyChangedEventArgs m_TmrDescriptionArgs =
            NotifyPropertyChangedHelper.CreateArgs<InstructionLDAbstractTimer>(o => o.TmrDescription);

        protected override void setItems()
        {
            // this means the Instruction changed
            TmrNameEditor.Text = TmrName;
            TmrDescriptionEditor.Text = TmrDescription;
            calculateVerticalRungOffset();
            NotifyPropertyChanged(m_TmrNameArgs);
            NotifyPropertyChanged(m_TmrDescriptionArgs);
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
            NotifyPropertyChangedHelper.CreateArgs<InstructionLDAbstractTimer>(o => o.SetpointSignalIn);
        private static string m_SetpointSignalInName =
            NotifyPropertyChangedHelper.GetPropertyName<InstructionLDAbstractTimer>(o => o.SetpointSignalIn);
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
            NotifyPropertyChangedHelper.CreateArgs<InstructionLDAbstractTimer>(o => o.VerticalRungOffset);

        private void calculateVerticalRungOffset()
        {
            // It's the size of the description, plus the Tmr name, plus half the Tmr size
            m_VerticalRungOffset = RUNG_IN_OFFSET;
            m_VerticalRungOffset += TmrDescriptionEditor.ActualHeight;
            m_VerticalRungOffset += TmrNameEditor.ActualHeight;
            NotifyPropertyChanged(m_VerticalRungOffsetArgs);
        }

        #region " TmrNameEditor "
        public SignalNameEditor TmrNameEditor
        {
            get
            {
                if (m_TmrNameEditor == null)
                {
                    m_TmrNameEditor = new SignalNameEditor(TmrName, MAX_WIDTH, TextAlignment.Center);
                    m_TmrNameEditor.PropertyChanged += new PropertyChangedEventHandler(m_TmrNameEditor_PropertyChanged);
                }
                return m_TmrNameEditor;
            }
        }
        private SignalNameEditor m_TmrNameEditor = null;

        void m_TmrNameEditor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == m_SignalNameEditor_ActualHeightName)
            {
                calculateVerticalRungOffset();
            }
            else if (e.PropertyName == m_SignalNameEditor_TextName
                && TmrName != TmrNameEditor.Text)
            {
                var oldSignal = Instruction.NodeSignalChildren.Items[0];
                var newSignal = oldSignal.SetSignalName(new FieldSignalName(TmrNameEditor.Text));
                var newInstruction = Instruction.NodeSignalChildren.Replace(oldSignal, newSignal);

                // update the elapsed time name (it's a function of the timer name)
                var oldElapsedSignal = newInstruction.NodeSignalChildren.Items[1];
                var newElapsedSignal = oldElapsedSignal.SetSignalName(new FieldSignalName(TmrNameEditor.Text + SEPARATOR + Resources.Strings.LD_Snap_Tmr_ElapsedName));
                newInstruction = newInstruction.NodeSignalChildren.Replace(oldElapsedSignal, newElapsedSignal);

                SimpleUndoableInstructionEdit(newInstruction, Resources.Strings.Undo_Action_EditTmrName);
            }
        }
        static readonly string m_SignalNameEditor_ActualHeightName =
            NotifyPropertyChangedHelper.GetPropertyName<SignalNameEditor>(o => o.ActualHeight);
        static readonly string m_SignalNameEditor_TextName =
            NotifyPropertyChangedHelper.GetPropertyName<SignalNameEditor>(o => o.Text);
        #endregion

        #region " TmrDescriptionEditor "
        public SignalDescriptionEditor TmrDescriptionEditor
        {
            get
            {
                if (m_TmrDescriptionEditor == null)
                {
                    m_TmrDescriptionEditor = new SignalDescriptionEditor(TmrDescription, MAX_WIDTH, MAX_DESCRIPTION_HEIGHT, TextAlignment.Center, true);
                    m_TmrDescriptionEditor.PropertyChanged += new PropertyChangedEventHandler(m_TmrDescriptionEditor_PropertyChanged);
                }
                return m_TmrDescriptionEditor;
            }
        }
        private SignalDescriptionEditor m_TmrDescriptionEditor = null;

        void m_TmrDescriptionEditor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == m_SignalDescriptionEditor_ActualHeightName)
            {
                calculateVerticalRungOffset();
            }
            else if (e.PropertyName == m_SignalDescriptionEditor_TextName
                && TmrDescription != TmrDescriptionEditor.Text)
            {
                var oldSignal = Instruction.NodeSignalChildren.Items[0];
                var newSignal = oldSignal.SetComment(new FieldString(TmrDescriptionEditor.Text));
                var newInstruction = Instruction.NodeSignalChildren.Replace(oldSignal, newSignal);
                SimpleUndoableInstructionEdit(newInstruction, Resources.Strings.Undo_Action_EditTmrDescription);
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
                SimpleUndoableInstructionEdit(newInstruction, Resources.Strings.Undo_Action_EditTmrSetpointSignal);
            }
        }
        static readonly string m_SetpointSignalChooser_SignalIdName =
            NotifyPropertyChangedHelper.GetPropertyName<SignalChooser>(o => o.SignalIn);
        #endregion

        #region " ElapsedSignalValue "
        public SignalValue ElapsedSignalValue
        {
            get
            {
                if (m_ElapsedSignalValue == null)
                {
                    m_ElapsedSignalValue = new SignalValue(this, Instruction.NodeSignalChildren[1], MAX_WIDTH, TextAlignment.Center, "{0:0.##############################}");
                }
                return m_ElapsedSignalValue;
            }
        }
        private SignalValue m_ElapsedSignalValue = null;

        #endregion
    }
}
