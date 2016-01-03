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

namespace SoapBox.Snap.Application.Workbench.Pads.SolutionPad.PageCollectionItem.ContextMenu
{
    [Export(ExtensionPoints.Workbench.Pads.SolutionPad.PageCollectionItem.ContextMenu, typeof(IMenuItem))]
    class DeleteSeparator : AbstractMenuItem
    {
        public DeleteSeparator()
        {
            ID = Extensions.Workbench.Pads.SolutionPad_.PageCollectionItem.ContextMenu.DeleteSeparator;
            IsSeparator = true;

            InsertRelativeToID = Extensions.Workbench.Pads.SolutionPad_.PageCollectionItem.ContextMenu.Add;
            BeforeOrAfter = RelativeDirection.After;
        }
    }
        
    [Export(ExtensionPoints.Workbench.Pads.SolutionPad.PageCollectionItem.ContextMenu, typeof(IMenuItem))]
    class Delete : AbstractMenuItem
    {
        public Delete()
        {
            ID = Extensions.Workbench.Pads.SolutionPad_.PageCollectionItem.ContextMenu.Delete;
            Header = Resources.Strings.Solution_Pad_PageCollectionItem_Delete;
            ToolTip = Resources.Strings.Solution_Pad_PageCollectionItem_Delete_ToolTip;
            SetIconFromBitmap(Resources.Images.PageCollection_Delete);
            InsertRelativeToID = Extensions.Workbench.Pads.SolutionPad_.PageCollectionItem.ContextMenu.DeleteSeparator;
            BeforeOrAfter = RelativeDirection.After;
        }

        protected override void Run()
        {
            base.Run();
            var pc = Context as SoapBox.Snap.Application.PageCollectionItem;
            if (pc != null)
            {
                pc.Delete();
            }
        }
    }
}
