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
using System.ComponentModel.Composition;

namespace SoapBox.Snap.LD
{
    [Export(Snap.ExtensionPoints.Workbench.Documents.PageEditor.InstructionItems,
        typeof(IInstructionItem))]
    [InstructionItem(
        Language = Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD,
        Library = Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.SnapComparison,
        Code = Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.SnapComparison_.GreaterThanOrEqual,
        SortOrder = Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.SnapComparison_.GreaterThanOrEqual_SortOrder,
        SpriteType = typeof(Resources.Images), SpriteKey = "SprGreaterThanOrEqual",
        ToolTipType = typeof(Resources.Strings), ToolTipKey = "ToolTipGreaterThanOrEqual")]
    public class InstructionLDGreaterThanOrEqual : InstructionLDComparisonBase
    {
        internal static readonly FieldInstructionType m_InstructionType = new FieldInstructionType(
                   new FieldIdentifier(Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD),
                   new FieldIdentifier(Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.SnapComparison),
                   new FieldIdentifier(Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.SnapComparison_.GreaterThanOrEqual));

        internal InstructionLDGreaterThanOrEqual()
            : base(null, m_InstructionType, null, true,
            Resources.Strings.LD_Snap_GreaterThanOrEqual_InstructionName,
            Resources.Strings.LD_Snap_GreaterThanOrEqual_FirstSignalName, Resources.Strings.LD_Snap_GreaterThanOrEqual_SecondSignalName)
        {
        }

        protected InstructionLDGreaterThanOrEqual(IEditorItem parent, NodeInstruction instruction)
            : base(parent, m_InstructionType, instruction, false,
            Resources.Strings.LD_Snap_GreaterThanOrEqual_InstructionName,
            Resources.Strings.LD_Snap_GreaterThanOrEqual_FirstSignalName, Resources.Strings.LD_Snap_GreaterThanOrEqual_SecondSignalName)
        {
        }

        #region "contextMenuSingleton"
        [ImportMany(ExtensionPoints.Instructions.GreaterThanOrEqual.ContextMenu, typeof(IMenuItem))]
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
            return new InstructionLDGreaterThanOrEqual(parent, instruction);
        }

    }
}
