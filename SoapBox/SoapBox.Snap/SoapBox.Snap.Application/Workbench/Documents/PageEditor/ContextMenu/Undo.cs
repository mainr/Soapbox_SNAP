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
using SoapBox.Core;
using System.ComponentModel.Composition;

namespace SoapBox.Snap.Application.Workbench.Documents.PageEditor.ContextMenu
{
    [Export(ExtensionPoints.Workbench.Documents.PageEditor.ContextMenu, typeof(IMenuItem))]
    class Undo : AbstractMenuItem
    {
        public Undo()
        {
            ID = Extensions.Workbench.Documents.PageEditor_.ContextMenu.Undo;
            Header = Resources.Strings.PageEditor_GroupCommand_Undo;
            ToolTip = Resources.Strings.PageEditor_GroupCommand_Undo_ToolTip;
            SetIconFromBitmap(Resources.Images.PageEditor_Undo);
            BeforeOrAfter = RelativeDirection.After;
            InsertRelativeToID = Extensions.Workbench.Documents.PageEditor_.ContextMenu.MoveSeparator;
        }

        protected override void Run()
        {
            base.Run();
            var pageEditorItem = Context as SoapBox.Snap.Application.PageEditor.PageEditorItem;
            if (pageEditorItem != null)
            {
                pageEditorItem.PageEditorParent.Undo();
            }
        }
    }
}
