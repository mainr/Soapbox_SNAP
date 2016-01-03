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
using SoapBox.Core;
using System.ComponentModel;
using SoapBox.Utilities;
using System.Windows;
using SoapBox.Protocol.Base;
using System.ComponentModel.Composition;

namespace SoapBox.Snap.LD
{
    public abstract class InstructionLDAbstractEdge : AbstractLDInstructionItem
    {
        public const double RUNG_IN_OFFSET = 10;
        public const double MAX_WIDTH = 80;
        public const double MAX_DESCRIPTION_HEIGHT = 70;
        public const string SEPARATOR = ".";

        protected InstructionLDAbstractEdge(FieldInstructionType instructionType)
            : base(null, instructionType)
        {
        }

        protected InstructionLDAbstractEdge(IEditorItem parent, NodeInstruction instruction, FieldInstructionType instructionType)
            : base(parent, instructionType)
        {
            if (instruction == null)
            {
                var newInstruction = NodeInstruction.BuildWith(InstructionType);
                // Output signal: Named boolean - memory of last rung in
                newInstruction = newInstruction.NodeSignalChildren.Append(
                    NodeSignal.BuildWith(
                        new FieldSignalName(InstructionName),
                        new FieldDataType(FieldDataType.DataTypeEnum.BOOL),
                        new FieldBool(false), // not forced
                        FieldConstant.Constants.BOOL.LOW)
                    );
                Instruction = newInstruction;
            }
            else
            {
                if (instruction.InstructionType != InstructionType)
                {
                    throw new InvalidOperationException("Tried to instantiate an edge but passed a different instruction type.");
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
                return false;
            }
        }
        #endregion

        public string AbstractEdgeName
        {
            get
            {
                return Instruction.NodeSignalChildren.Items[0].SignalName.ToString();
            }
        }
        protected static readonly PropertyChangedEventArgs m_AbstractEdgeNameArgs =
            NotifyPropertyChangedHelper.CreateArgs<InstructionLDAbstractEdge>(o => o.AbstractEdgeName);

        public string AbstractEdgeDescription
        {
            get
            {
                return Instruction.NodeSignalChildren.Items[0].Comment.ToString();
            }
        }
        protected static readonly PropertyChangedEventArgs m_AbstractEdgeDescriptionArgs =
            NotifyPropertyChangedHelper.CreateArgs<InstructionLDAbstractEdge>(o => o.AbstractEdgeDescription);

        protected override void setItems()
        {
            // this means the Instruction changed
            AbstractEdgeNameEditor.Text = AbstractEdgeName;
            AbstractEdgeDescriptionEditor.Text = AbstractEdgeDescription;
            calculateVerticalRungOffset();
            NotifyPropertyChanged(m_AbstractEdgeNameArgs);
            NotifyPropertyChanged(m_AbstractEdgeDescriptionArgs);
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
            NotifyPropertyChangedHelper.CreateArgs<InstructionLDAbstractEdge>(o => o.VerticalRungOffset);

        private void calculateVerticalRungOffset()
        {
            // It's the size of the description, plus the AbstractEdge name, plus half the AbstractEdge size
            m_VerticalRungOffset = RUNG_IN_OFFSET;
            m_VerticalRungOffset += AbstractEdgeDescriptionEditor.ActualHeight;
            m_VerticalRungOffset += AbstractEdgeNameEditor.ActualHeight;
            NotifyPropertyChanged(m_VerticalRungOffsetArgs);
        }

        #region " AbstractEdgeNameEditor "
        public SignalNameEditor AbstractEdgeNameEditor
        {
            get
            {
                if (m_AbstractEdgeNameEditor == null)
                {
                    m_AbstractEdgeNameEditor = new SignalNameEditor(AbstractEdgeName, MAX_WIDTH, TextAlignment.Center);
                    m_AbstractEdgeNameEditor.PropertyChanged += new PropertyChangedEventHandler(m_AbstractEdgeNameEditor_PropertyChanged);
                }
                return m_AbstractEdgeNameEditor;
            }
        }
        private SignalNameEditor m_AbstractEdgeNameEditor = null;

        void m_AbstractEdgeNameEditor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == m_SignalNameEditor_ActualHeightName)
            {
                calculateVerticalRungOffset();
            }
            else if (e.PropertyName == m_SignalNameEditor_TextName
                && AbstractEdgeName != AbstractEdgeNameEditor.Text)
            {
                var oldSignal = Instruction.NodeSignalChildren.Items[0];
                var newSignal = oldSignal.SetSignalName(new FieldSignalName(AbstractEdgeNameEditor.Text));
                var newInstruction = Instruction.NodeSignalChildren.Replace(oldSignal, newSignal);

                SimpleUndoableInstructionEdit(newInstruction, Resources.Strings.Undo_Action_EditEdgeName);
            }
        }
        static readonly string m_SignalNameEditor_ActualHeightName =
            NotifyPropertyChangedHelper.GetPropertyName<SignalNameEditor>(o => o.ActualHeight);
        static readonly string m_SignalNameEditor_TextName =
            NotifyPropertyChangedHelper.GetPropertyName<SignalNameEditor>(o => o.Text);
        #endregion

        #region " AbstractEdgeDescriptionEditor "
        public SignalDescriptionEditor AbstractEdgeDescriptionEditor
        {
            get
            {
                if (m_AbstractEdgeDescriptionEditor == null)
                {
                    m_AbstractEdgeDescriptionEditor = new SignalDescriptionEditor(AbstractEdgeDescription, MAX_WIDTH, MAX_DESCRIPTION_HEIGHT, TextAlignment.Center, true);
                    m_AbstractEdgeDescriptionEditor.PropertyChanged += new PropertyChangedEventHandler(m_AbstractEdgeDescriptionEditor_PropertyChanged);
                }
                return m_AbstractEdgeDescriptionEditor;
            }
        }
        private SignalDescriptionEditor m_AbstractEdgeDescriptionEditor = null;

        void m_AbstractEdgeDescriptionEditor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == m_SignalDescriptionEditor_ActualHeightName)
            {
                calculateVerticalRungOffset();
            }
            else if (e.PropertyName == m_SignalDescriptionEditor_TextName
                && AbstractEdgeDescription != AbstractEdgeDescriptionEditor.Text)
            {
                var oldSignal = Instruction.NodeSignalChildren.Items[0];
                var newSignal = oldSignal.SetComment(new FieldString(AbstractEdgeDescriptionEditor.Text));
                var newInstruction = Instruction.NodeSignalChildren.Replace(oldSignal, newSignal);
                SimpleUndoableInstructionEdit(newInstruction, Resources.Strings.Undo_Action_EditEdgeDescription);
            }
        }
        static readonly string m_SignalDescriptionEditor_ActualHeightName =
            NotifyPropertyChangedHelper.GetPropertyName<SignalDescriptionEditor>(o => o.ActualHeight);
        static readonly string m_SignalDescriptionEditor_TextName =
            NotifyPropertyChangedHelper.GetPropertyName<SignalDescriptionEditor>(o => o.Text);
        #endregion

    }
}
