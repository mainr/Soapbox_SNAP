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

namespace SoapBox.Snap.LD
{
    public abstract class AbstractLDInstructionItem : AbstractInstructionItem, ILDInstructionItem
    {
        public AbstractLDInstructionItem(IEditorItem parent, FieldInstructionType instructionType)
            : base(parent, instructionType)
        {
        }

        public abstract bool IsRight { get; }
        public abstract double VerticalRungOffset { get; set; }

        public void DeleteRequest()
        {
            // delegate deletes to the parent (can only call DeleteRequest on a 
            // non-series instruction, and then only a series instruction can delete children)
            var myParent = Parent as ILDInstructionItem;
            if (myParent != null)
            {
                myParent.DeleteSelectedChildren();
            }
        }

        public virtual void DeleteSelectedChildren() { }
    }
}
