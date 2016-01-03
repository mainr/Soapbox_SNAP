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
using SoapBox.Protocol.Automation;
using System.ComponentModel;
using SoapBox.Utilities;
using SoapBox.Protocol.Base;
using SoapBox.Core;
using System.ComponentModel.Composition;
using System.Windows;

namespace SoapBox.Snap.LD
{
    [Export(SoapBox.Core.ExtensionPoints.Host.Void, typeof(object))]
    public abstract class InstructionLDMathBase : AbstractLDInstructionItem
    {
        public const double RUNG_IN_OFFSET = 10;
        public const double MAX_WIDTH = 150;
        public const double MAX_DESCRIPTION_HEIGHT = 70;

        private InstructionLDMathBase() // just here for MEF to call
            : base(null, null)
        {
        }

        protected InstructionLDMathBase(IEditorItem parent, FieldInstructionType instructionType, NodeInstruction instruction,
            string defaultName, bool factory, string instructionName, string firstSignalName, string secondSignalName)
            : base(parent, instructionType)
        {
            if (factory)
            {
                return;
            }

            this.InstructionName = instructionName;
            this.FirstSignalName = firstSignalName;
            this.SecondSignalName = secondSignalName;

            if (instruction == null)
            {
                var newInstruction = NodeInstruction.BuildWith(InstructionType);
                // Output signal: Named number - result
                newInstruction = newInstruction.NodeSignalChildren.Append(
                    NodeSignal.BuildWith(
                        new FieldSignalName(defaultName),
                        new FieldDataType(FieldDataType.DataTypeEnum.NUMBER),
                        new FieldBool(false), // not forced
                        FieldConstant.Constants.NUMBER.ZERO)
                    );
                // Input signal: First
                newInstruction = newInstruction.NodeSignalInChildren.Append(
                    NodeSignalIn.BuildWith(
                        new FieldDataType(FieldDataType.DataTypeEnum.NUMBER),
                        new FieldConstant(FieldDataType.DataTypeEnum.NUMBER, 0)));
                // Input signal: Second
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
                    throw new InvalidOperationException("Tried to instantiate a different instruction type.");
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

        protected abstract IEnumerable<IMenuItem> contextMenu { get; set; }

        #region " IsRight "
        public override bool IsRight
        {
            get
            {
                return true;
            }
        }
        #endregion

        public string InstructionName { get; private set; }

        public NodeSignal Result
        {
            get
            {
                return Instruction.NodeSignalChildren[0];
            }
        }

        public string OutputSignalName
        {
            get
            {
                return Result.SignalName.ToString();
            }
        }
        protected static readonly PropertyChangedEventArgs m_InstructionNameArgs =
            NotifyPropertyChangedHelper.CreateArgs<InstructionLDMathBase>(o => o.OutputSignalName);

        public string OutputSignalDescription
        {
            get
            {
                return Result.Comment.ToString();
            }
        }
        protected static readonly PropertyChangedEventArgs m_InstructionDescriptionArgs =
            NotifyPropertyChangedHelper.CreateArgs<InstructionLDMathBase>(o => o.OutputSignalDescription);

        protected override void setItems()
        {
            // this means the Instruction changed
            OutputSignalNameEditor.Text = OutputSignalName;
            OutputSignalDescriptionEditor.Text = OutputSignalDescription;
            calculateVerticalRungOffset();
            NotifyPropertyChanged(m_InstructionNameArgs);
            NotifyPropertyChanged(m_InstructionDescriptionArgs);
        }

        public string FirstSignalName { get; private set; }

        #region " FirstSignalIn "
        public NodeSignalIn FirstSignalIn
        {
            get
            {
                return Instruction.NodeSignalInChildren[0];
            }
        }
        private static readonly PropertyChangedEventArgs m_FirstSignalInArgs =
            NotifyPropertyChangedHelper.CreateArgs<InstructionLDMathBase>(o => o.FirstSignalIn);
        private static string m_FirstSignalInName =
            NotifyPropertyChangedHelper.GetPropertyName<InstructionLDMathBase>(o => o.FirstSignalIn);
        #endregion

        public string SecondSignalName { get; private set; }

        #region " SecondSignalIn "
        public NodeSignalIn SecondSignalIn
        {
            get
            {
                return Instruction.NodeSignalInChildren[1];
            }
        }
        private static readonly PropertyChangedEventArgs m_SecondSignalInArgs =
            NotifyPropertyChangedHelper.CreateArgs<InstructionLDMathBase>(o => o.SecondSignalIn);
        private static string m_SecondSignalInName =
            NotifyPropertyChangedHelper.GetPropertyName<InstructionLDMathBase>(o => o.SecondSignalIn);
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
            NotifyPropertyChangedHelper.CreateArgs<InstructionLDMathBase>(o => o.VerticalRungOffset);

        private void calculateVerticalRungOffset()
        {
            m_VerticalRungOffset = RUNG_IN_OFFSET;
            m_VerticalRungOffset += OutputSignalDescriptionEditor.ActualHeight;
            m_VerticalRungOffset += OutputSignalNameEditor.ActualHeight;
            NotifyPropertyChanged(m_VerticalRungOffsetArgs);
        }

        #region " OutputSignalNameEditor "
        public SignalNameEditor OutputSignalNameEditor
        {
            get
            {
                if (m_InstructionNameEditor == null)
                {
                    m_InstructionNameEditor = new SignalNameEditor(OutputSignalName, MAX_WIDTH, TextAlignment.Center);
                    m_InstructionNameEditor.PropertyChanged += new PropertyChangedEventHandler(m_InstructionNameEditor_PropertyChanged);
                }
                return m_InstructionNameEditor;
            }
        }
        private SignalNameEditor m_InstructionNameEditor = null;

        void m_InstructionNameEditor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == m_SignalNameEditor_ActualHeightName)
            {
                calculateVerticalRungOffset();
            }
            else if (e.PropertyName == m_SignalNameEditor_TextName
                && OutputSignalName != OutputSignalNameEditor.Text)
            {
                var oldSignal = Result;
                var newSignal = oldSignal.SetSignalName(new FieldSignalName(OutputSignalNameEditor.Text));
                var newInstruction = Instruction.NodeSignalChildren.Replace(oldSignal, newSignal);

                SimpleUndoableInstructionEdit(newInstruction, Resources.Strings.Undo_Action_EditInstructionName);
            }
        }
        static readonly string m_SignalNameEditor_ActualHeightName =
            NotifyPropertyChangedHelper.GetPropertyName<SignalNameEditor>(o => o.ActualHeight);
        static readonly string m_SignalNameEditor_TextName =
            NotifyPropertyChangedHelper.GetPropertyName<SignalNameEditor>(o => o.Text);
        #endregion

        #region " OutputSignalDescriptionEditor "
        public SignalDescriptionEditor OutputSignalDescriptionEditor
        {
            get
            {
                if (m_InstructionDescriptionEditor == null)
                {
                    m_InstructionDescriptionEditor = new SignalDescriptionEditor(OutputSignalDescription, MAX_WIDTH, MAX_DESCRIPTION_HEIGHT, TextAlignment.Center, true);
                    m_InstructionDescriptionEditor.PropertyChanged += new PropertyChangedEventHandler(m_InstructionDescriptionEditor_PropertyChanged);
                }
                return m_InstructionDescriptionEditor;
            }
        }
        private SignalDescriptionEditor m_InstructionDescriptionEditor = null;

        void m_InstructionDescriptionEditor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == m_SignalDescriptionEditor_ActualHeightName)
            {
                calculateVerticalRungOffset();
            }
            else if (e.PropertyName == m_SignalDescriptionEditor_TextName
                && OutputSignalDescription != OutputSignalDescriptionEditor.Text)
            {
                var oldSignal = Result;
                var newSignal = oldSignal.SetComment(new FieldString(OutputSignalDescriptionEditor.Text));
                var newInstruction = Instruction.NodeSignalChildren.Replace(oldSignal, newSignal);
                SimpleUndoableInstructionEdit(newInstruction, Resources.Strings.Undo_Action_EditInstructionDescription);
            }
        }
        static readonly string m_SignalDescriptionEditor_ActualHeightName =
            NotifyPropertyChangedHelper.GetPropertyName<SignalDescriptionEditor>(o => o.ActualHeight);
        static readonly string m_SignalDescriptionEditor_TextName =
            NotifyPropertyChangedHelper.GetPropertyName<SignalDescriptionEditor>(o => o.Text);
        #endregion

        #region " FirstSignalChooser "
        public SignalChooser FirstSignalChooser
        {
            get
            {
                if (m_FirstSignalChooser == null)
                {
                    m_FirstSignalChooser = new SignalChooser(this, FirstSignalIn, MAX_WIDTH, TextAlignment.Center);
                    m_FirstSignalChooser.PropertyChanged += new PropertyChangedEventHandler(m_FirstSignalChooser_PropertyChanged);
                }
                return m_FirstSignalChooser;
            }
        }
        private SignalChooser m_FirstSignalChooser = null;

        void m_FirstSignalChooser_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == m_FirstSignalChooser_SignalIdName)
            {
                var oldSignalIn = Instruction.NodeSignalInChildren.Items[0];
                var newSignalIn = FirstSignalChooser.SignalIn;
                var newInstruction = Instruction = Instruction.NodeSignalInChildren.Replace(oldSignalIn, newSignalIn);
                SimpleUndoableInstructionEdit(newInstruction, Resources.Strings.Undo_Action_EditMathFirstSignal);
            }
        }
        static readonly string m_FirstSignalChooser_SignalIdName =
            NotifyPropertyChangedHelper.GetPropertyName<SignalChooser>(o => o.SignalIn);
        #endregion

        #region " SecondSignalChooser "
        public SignalChooser SecondSignalChooser
        {
            get
            {
                if (m_SecondSignalChooser == null)
                {
                    m_SecondSignalChooser = new SignalChooser(this, SecondSignalIn, MAX_WIDTH, TextAlignment.Center);
                    m_SecondSignalChooser.PropertyChanged += new PropertyChangedEventHandler(m_SecondSignalChooser_PropertyChanged);
                }
                return m_SecondSignalChooser;
            }
        }
        private SignalChooser m_SecondSignalChooser = null;

        void m_SecondSignalChooser_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == m_SecondSignalChooser_SignalIdName)
            {
                var oldSignalIn = Instruction.NodeSignalInChildren.Items[1];
                var newSignalIn = SecondSignalChooser.SignalIn;
                var newInstruction = Instruction = Instruction.NodeSignalInChildren.Replace(oldSignalIn, newSignalIn);
                SimpleUndoableInstructionEdit(newInstruction, Resources.Strings.Undo_Action_EditMathSecondSignal);
            }
        }
        static readonly string m_SecondSignalChooser_SignalIdName =
            NotifyPropertyChangedHelper.GetPropertyName<SignalChooser>(o => o.SignalIn);
        #endregion

        #region " ResultSignalValue "
        public SignalValue ResultSignalValue
        {
            get
            {
                if (m_ResultSignalValue == null)
                {
                    m_ResultSignalValue = new SignalValue(this, Result, MAX_WIDTH, TextAlignment.Center, "{0:0.##############################}");
                }
                return m_ResultSignalValue;
            }
        }
        private SignalValue m_ResultSignalValue = null;

        #endregion
    }
}
