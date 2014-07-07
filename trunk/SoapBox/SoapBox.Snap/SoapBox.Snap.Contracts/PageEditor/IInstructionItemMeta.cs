#region "SoapBox.Snap License"
/// <header module="SoapBox.Snap"> 
/// Copyright (C) 2009-2014 SoapBox Automation, All Rights Reserved.
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
using System.Reflection;

namespace SoapBox.Snap
{
    public interface IInstructionItemMeta
    {
        string Language { get; }
        string Library { get; }
        string Code { get; }
        double SortOrder { get; } // Sorts in order of Language, Library, SortOrder
        bool Hidden { get; } // if true, the user can't drop this instruction on a page directly
        Type SpriteType { get; } // usually "typeof(Resources.Images)"
        string SpriteKey { get; } // resource key within the resource file
        Type ToolTipType { get; } // usually "typeof(Resources.Strings)"
        string ToolTipKey { get; } // resource key within resources file for tool tip
    }

    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class InstructionItemAttribute : ExportAttribute
    {
        public InstructionItemAttribute() : base(typeof(IInstructionItem)) { }
        public InstructionItemAttribute(Type contractType) : base(contractType) { }
        public string Language { get; set; }
        public string Library { get; set; }
        public string Code { get; set; }
        public double SortOrder { get; set; }
        public bool Hidden { get; set; }
        public Type SpriteType { get; set; }
        public string SpriteKey { get; set; }
        public Type ToolTipType { get; set; }
        public string ToolTipKey { get; set; }
    }
}
