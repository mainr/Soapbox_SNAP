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
using System.ComponentModel.Composition;
using SoapBox.Protocol.Automation;
using SoapBox.Protocol.Base;
using System.Collections.ObjectModel;
using System.ComponentModel;
using SoapBox.Utilities;
using System.Windows.Controls;
using System.Windows;
using SoapBox.Core;

namespace SoapBox.Snap.LD
{
    [Export(Snap.ExtensionPoints.Workbench.Documents.PageEditor.InstructionItems,
        typeof(IInstructionItem))]
    [InstructionItem(
        Language = Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD,
        Library = Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.Snap,
        Code = Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.Snap_.CntDN,
        SortOrder = Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.Snap_.CntDN_SortOrder,
        SpriteType = typeof(Resources.Images), SpriteKey = "SprCntDN",
        ToolTipType = typeof(Resources.Strings), ToolTipKey = "ToolTipCntDN")]
    public class InstructionLDCntDN : InstructionLDAbstractCounter
    {

        internal static readonly FieldInstructionType m_InstructionType = new FieldInstructionType(
                    new FieldIdentifier(Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD),
                    new FieldIdentifier(Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.Snap),
                    new FieldIdentifier(Extensions.Workbench.Documents.PageEditor_.InstructionItems.LD_.Snap_.CntDN));

        /// <summary>
        /// Just here for MEF to call, to do the imports
        /// </summary>
        internal InstructionLDCntDN()
            : base(m_InstructionType)
        {
        }

        private InstructionLDCntDN(IEditorItem parent, NodeInstruction instruction)
            : base(parent, instruction, m_InstructionType)
        {
        }

        public override string InstructionName
        {
            get 
            {
                return Resources.Strings.LD_Snap_CntDN_InstructionName;
            }
        }

        #region "extensionServiceSingleton"
        [Import(SoapBox.Core.Services.Host.ExtensionService)]
        protected override IExtensionService extensionService
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
        protected override IEnumerable<IMenuItem> ldInstructionContextMenu
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

        #region "contextMenuSingleton"
        [ImportMany(ExtensionPoints.Instructions.CntDN.ContextMenu, typeof(IMenuItem))]
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
            return new InstructionLDCntDN(parent, instruction);
        }

    }
}
