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
using System.Windows;
using SoapBox.Protocol.Base;
using SoapBox.Core;

namespace SoapBox.Snap
{
    public interface IInstructionItem : IEditorItem, IContextMenu
    {
        IInstructionItem Create(IEditorItem parent, NodeInstruction instruction);
        NodeInstruction Instruction { get; set; }
        FieldInstructionType InstructionType { get; }
        void ChildInstruction_Edited(INodeWrapper sender, NodeBase oldNode, NodeBase newNode);
        void ChildInstruction_Deleted(INodeWrapper sender, NodeBase deletedNode);
    }
}
