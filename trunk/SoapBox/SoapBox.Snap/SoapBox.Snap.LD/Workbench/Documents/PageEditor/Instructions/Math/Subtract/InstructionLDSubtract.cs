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
    [Export(Snap.ExtensionPoints.Workbench.Documents.PageEditor.InstructionItems,
        typeof(IInstructionItem))]
    [InstructionItem(
        Language = Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD,
        Library = Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.SnapMath,
        Code = Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.SnapMath_.Subtract,
        SortOrder = Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.SnapMath_.Subtract_SortOrder,
        SpriteType = typeof(Resources.Images), SpriteKey = "SprSubtract",
        ToolTipType = typeof(Resources.Strings), ToolTipKey = "ToolTipSubtract")]
    public class InstructionLDSubtract : InstructionLDMathBase
    {
        internal static readonly FieldInstructionType m_InstructionType = new FieldInstructionType(
                   new FieldIdentifier(Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD),
                   new FieldIdentifier(Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.SnapMath),
                   new FieldIdentifier(Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.SnapMath_.Subtract));

        internal InstructionLDSubtract()
            : base(null, m_InstructionType, null, Resources.Strings.LD_Snap_Subtract_DefaultName, true,
            Resources.Strings.LD_Snap_Subtract_InstructionName,
            Resources.Strings.LD_Snap_Subtract_FirstSignalName, Resources.Strings.LD_Snap_Subtract_SecondSignalName)
        {
        }

        protected InstructionLDSubtract(IEditorItem parent, NodeInstruction instruction)
            : base(parent, m_InstructionType, instruction, Resources.Strings.LD_Snap_Subtract_DefaultName, false,
            Resources.Strings.LD_Snap_Subtract_InstructionName,
            Resources.Strings.LD_Snap_Subtract_FirstSignalName, Resources.Strings.LD_Snap_Subtract_SecondSignalName)
        {
        }

        #region "contextMenuSingleton"
        [ImportMany(ExtensionPoints.Instructions.Subtract.ContextMenu, typeof(IMenuItem))]
        protected override IEnumerable<IMenuItem> contextMenu
        {
            get
            {
                return contextMenuSingleton;
            }
            set
            {
                contextMenuSingleton = value;
            }
        }
        private static IEnumerable<IMenuItem> contextMenuSingleton = null;
        #endregion

        public override IInstructionItem Create(IEditorItem parent, NodeInstruction instruction)
        {
            return new InstructionLDSubtract(parent, instruction);
        }

    }
}
