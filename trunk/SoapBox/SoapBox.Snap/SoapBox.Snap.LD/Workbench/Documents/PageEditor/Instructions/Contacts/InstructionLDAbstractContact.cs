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
using System.ComponentModel.Composition;
using SoapBox.Protocol.Automation;
using SoapBox.Protocol.Base;
using System.Collections.ObjectModel;
using System.ComponentModel;
using SoapBox.Utilities;
using System.Windows.Controls;
using System.Windows;

namespace SoapBox.Snap.LD
{
    /// <summary>
    /// This is a base class for normally open, normally closed, and other "concrete" contact types
    /// </summary>
    public abstract class InstructionLDAbstractContact : AbstractLDInstructionItem 
    {
        public const double CONTACT_HEIGHT = InstructionLDCoil.COIL_DIAMETER;
        public const double CONTACT_WIDTH = InstructionLDCoil.COIL_DIAMETER * 0.7;
        public const double MAX_WIDTH = 220;
        public const double MAX_DESCRIPTION_WIDTH = InstructionLDCoil.MAX_WIDTH;
        public const double MAX_DESCRIPTION_HEIGHT = InstructionLDCoil.MAX_DESCRIPTION_HEIGHT;

        protected InstructionLDAbstractContact(IEditorItem parent, FieldInstructionType instructionType)
            : base(parent, instructionType)
        {
        }

        protected InstructionLDAbstractContact(IEditorItem parent, FieldInstructionType instructionType, NodeInstruction instruction)
            : base(parent, instructionType)
        {
            if (instruction == null)
            {
                var newInstruction = NodeInstruction.BuildWith(InstructionType);
                // Input signal: (coil that we're a contact off) default to always false
                newInstruction = newInstruction.NodeSignalInChildren.Append(
                    NodeSignalIn.BuildWith(
                        new FieldDataType(FieldDataType.DataTypeEnum.BOOL),
                        new FieldConstant(FieldDataType.DataTypeEnum.BOOL, false)));
                Instruction = newInstruction;
            }
            else
            {
                if (instructionType != instruction.InstructionType)
                {
                    throw new ArgumentOutOfRangeException("Tried to instantiate a contact of type " + 
                        instructionType.ToString() + " with an instruction of a different type.");
                }
                Instruction = instruction;
            }
        }

        #region " IsRight "
        public override bool IsRight
        {
            get
            {
                return false;
            }
        }
        #endregion

        #region " ContactSignalIn "
        public NodeSignalIn ContactSignalIn
        {
            get
            {
                return Instruction.NodeSignalInChildren[0];
            }
        }
        private static readonly PropertyChangedEventArgs m_ContactSignalInArgs =
            NotifyPropertyChangedHelper.CreateArgs<InstructionLDContactNO>(o => o.ContactSignalIn);
        private static string m_ContactSignalInName =
            NotifyPropertyChangedHelper.GetPropertyName<InstructionLDContactNO>(o => o.ContactSignalIn);
        #endregion

        protected override void setItems()
        {
            // this means the Instruction changed
            ContactSignalChooser.SignalIn = ContactSignalIn;
            ContactDescriptionDisplay.SignalIn = ContactSignalIn;
            calculateVerticalRungOffset();
            NotifyPropertyChanged(m_ContactSignalInArgs);
        }

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
        double m_VerticalRungOffset = -1;

        private void calculateVerticalRungOffset()
        {
            // It's the size of the description, plus the coil name, plus half the coil size
            m_VerticalRungOffset = CONTACT_HEIGHT / 2;
            m_VerticalRungOffset += ContactDescriptionDisplay.ActualHeight;
            m_VerticalRungOffset += ContactSignalChooser.ActualHeight;
        }

        #region " ContactSignalChooser "
        public SignalChooser ContactSignalChooser
        {
            get
            {
                if (m_ContactSignalChooser == null)
                {
                    m_ContactSignalChooser = new SignalChooser(this, ContactSignalIn, MAX_WIDTH, TextAlignment.Center);
                    m_ContactSignalChooser.PropertyChanged += new PropertyChangedEventHandler(m_ContactSignalChooser_PropertyChanged);
                }
                return m_ContactSignalChooser;
            }
        }
        private SignalChooser m_ContactSignalChooser = null;

        void m_ContactSignalChooser_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == m_ContactSignalChooser_ActualHeightName)
            {
                calculateVerticalRungOffset();
            }
            else if (e.PropertyName == m_ContactSignalChooser_SignalIdName)
            {
                var oldSignalIn = Instruction.NodeSignalInChildren.Items[0];
                var newSignalIn = ContactSignalChooser.SignalIn;
                var newInstruction = Instruction = Instruction.NodeSignalInChildren.Replace(oldSignalIn, newSignalIn);
                SimpleUndoableInstructionEdit(newInstruction, Resources.Strings.Undo_Action_EditContactSignal);
            }
        }
        static readonly string m_ContactSignalChooser_ActualHeightName =
            NotifyPropertyChangedHelper.GetPropertyName<SignalChooser>(o => o.ActualHeight);
        static readonly string m_ContactSignalChooser_SignalIdName =
            NotifyPropertyChangedHelper.GetPropertyName<SignalChooser>(o => o.SignalIn);
        #endregion

        #region " ContactDescriptionDisplay "
        public SignalDescriptionDisplay ContactDescriptionDisplay
        {
            get
            {
                if (m_ContactDescriptionDisplay == null)
                {
                    m_ContactDescriptionDisplay = new SignalDescriptionDisplay(this, ContactSignalIn, MAX_DESCRIPTION_WIDTH, MAX_DESCRIPTION_HEIGHT, TextAlignment.Center);
                    m_ContactDescriptionDisplay.PropertyChanged += new PropertyChangedEventHandler(m_ContactDescriptionDisplay_PropertyChanged);
                }
                return m_ContactDescriptionDisplay;
            }
        }
        private SignalDescriptionDisplay m_ContactDescriptionDisplay = null;

        void m_ContactDescriptionDisplay_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == m_SignalDescriptionDisplay_ActualHeightName)
            {
                calculateVerticalRungOffset();
            }
        }
        static readonly string m_SignalDescriptionDisplay_ActualHeightName =
            NotifyPropertyChangedHelper.GetPropertyName<SignalDescriptionDisplay>(o => o.ActualHeight);
        #endregion

    }
}
