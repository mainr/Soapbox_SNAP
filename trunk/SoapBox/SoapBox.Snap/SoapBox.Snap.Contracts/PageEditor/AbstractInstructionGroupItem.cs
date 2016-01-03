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
using SoapBox.Utilities;
using System.ComponentModel;

namespace SoapBox.Snap
{
    public abstract class AbstractInstructionGroupItem : AbstractEditorItem, IInstructionGroupItem
    {
        public AbstractInstructionGroupItem(IEditorItem parent)
            : base(parent)
        {
        }

        #region " InstructionGroup "
        public NodeInstructionGroup InstructionGroup
        {
            get
            {
                return Node as NodeInstructionGroup;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(m_InstructionGroupName);
                }
                if (Node != value)
                {
                    Node = value;
                    setItems();
                    NotifyPropertyChanged(m_InstructionGroupArgs);
                }
            }
        }
        private static readonly PropertyChangedEventArgs m_InstructionGroupArgs =
            NotifyPropertyChangedHelper.CreateArgs<AbstractInstructionGroupItem>(o => o.InstructionGroup);
        private static string m_InstructionGroupName =
            NotifyPropertyChangedHelper.GetPropertyName<AbstractInstructionGroupItem>(o => o.InstructionGroup);
        #endregion

        public virtual IInstructionGroupItem Create(IEditorItem parent, NodeInstructionGroup instructionGroup) { return null; }
        protected virtual void setItems() { }
        public virtual NodeInstructionGroup CreateEmptyNode() { return null; }
    }
}
