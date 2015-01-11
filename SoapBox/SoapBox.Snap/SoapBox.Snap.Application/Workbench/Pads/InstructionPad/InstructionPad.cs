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
using SoapBox.Core;
using System.Collections.ObjectModel;

namespace SoapBox.Snap.Application
{
    [Export(SoapBox.Core.ExtensionPoints.Workbench.Pads, typeof(IPad))]
    [Export(CompositionPoints.Workbench.Pads.InstructionPad, typeof(InstructionPad))]
    [Pad(Name = Extensions.Workbench.Pads.InstructionPad)]
    public class InstructionPad : AbstractPad
    {
        private InstructionPad()
        {
            ID = Extensions.Workbench.Pads.InstructionPad;
            Name = Extensions.Workbench.Pads.InstructionPad;
            Title = Resources.Strings.Instruction_Pad_Title;
        }

        [ImportMany(ExtensionPoints.Workbench.Documents.PageEditor.InstructionItems,
            typeof(IInstructionItem), AllowRecomposition = true)]
        private IEnumerable<Lazy<IInstructionItem, IInstructionItemMeta>> instructionItems { get; set; }

        public IEnumerable<Lazy<IInstructionItem, IInstructionItemMeta>> Instructions
        {
            get
            {
                return from i in instructionItems 
                       where !i.Metadata.Hidden 
                       orderby i.Metadata.Language, i.Metadata.Library, i.Metadata.SortOrder 
                       select i;
            }
        }
    }
}
