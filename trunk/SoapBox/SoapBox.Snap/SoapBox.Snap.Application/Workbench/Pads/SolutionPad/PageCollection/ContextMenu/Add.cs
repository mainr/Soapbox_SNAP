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
using SoapBox.Core;
using System.ComponentModel.Composition;
using SoapBox.Protocol.Automation;
using SoapBox.Protocol.Base;

namespace SoapBox.Snap.Application.Workbench.Pads.SolutionPad.PageCollection.ContextMenu
{
    [Export(ExtensionPoints.Workbench.Pads.SolutionPad.PageCollectionItem.ContextMenu, typeof(IMenuItem))]
    class AddSeparator : AbstractMenuItem
    {
        public AddSeparator()
        {
            ID = Extensions.Workbench.Pads.SolutionPad_.PageCollectionItem.ContextMenu.AddSeparator;
            IsSeparator = true;

            InsertRelativeToID = Extensions.Workbench.Pads.SolutionPad_.PageCollectionItem.ContextMenu.MoveDown;
            BeforeOrAfter = RelativeDirection.After;
        }
    }
    
    [Export(ExtensionPoints.Workbench.Pads.SolutionPad.PageCollectionItem.ContextMenu, typeof(IMenuItem))]
    class Add : AbstractMenuItem, IPartImportsSatisfiedNotification
    {
        public Add()
        {
            ID = Extensions.Workbench.Pads.SolutionPad_.PageCollectionItem.ContextMenu.Add;
            InsertRelativeToID = Extensions.Workbench.Pads.SolutionPad_.PageCollectionItem.ContextMenu.AddSeparator;
            BeforeOrAfter = RelativeDirection.After;
            Header = Resources.Strings.Solution_Pad_PageCollectionItem_Add;
        }

        [Import(SoapBox.Core.Services.Host.ExtensionService)]
        private IExtensionService extensionService { get; set; }

        [ImportMany(ExtensionPoints.Workbench.Pads.SolutionPad.PageCollectionItem.ContextMenu_.Add, 
            typeof(IMenuItem), AllowRecomposition = true)]
        private IEnumerable<IMenuItem> menu { get; set; }

        public void OnImportsSatisfied()
        {
            Items = extensionService.Sort(menu);
        }
    }

    [Export(ExtensionPoints.Workbench.Pads.SolutionPad.PageCollectionItem.ContextMenu_.Add, typeof(IMenuItem))]
    class AddPageCollection : AbstractMenuItem
    {
        public AddPageCollection()
        {
            ID = Extensions.Workbench.Pads.SolutionPad_.PageCollectionItem.ContextMenu.Add_.PageCollection;
            Header = Resources.Strings.Solution_Pad_PageCollectionItem_Add_PageCollection;
            ToolTip = Resources.Strings.Solution_Pad_PageCollectionItem_Add_PageCollection_ToolTip;
            SetIconFromBitmap(Resources.Images.PageCollection_Add);
        }

        [Import(SoapBox.Core.Services.Layout.LayoutManager, typeof(ILayoutManager))]
        private Lazy<ILayoutManager> layoutManager { get; set; }

        protected override void Run()
        {
            base.Run();
            var pc = Context as SoapBox.Snap.Application.PageCollectionItem;
            if (pc != null)
            {
                pc.AddPageCollection();
            }
        }
    }

    [Export(ExtensionPoints.Workbench.Pads.SolutionPad.PageCollectionItem.ContextMenu_.Add, typeof(IMenuItem))]
    class AddPage : AbstractMenuItem
    {
        public AddPage()
        {
            ID = Extensions.Workbench.Pads.SolutionPad_.PageCollectionItem.ContextMenu.Add_.Page;
            Header = Resources.Strings.Solution_Pad_PageCollectionItem_Add_Page;
            ToolTip = Resources.Strings.Solution_Pad_PageCollectionItem_Add_Page_ToolTip;
            SetIconFromBitmap(Resources.Images.Page_Add);
        }

        [Import(SoapBox.Core.Services.Layout.LayoutManager, typeof(ILayoutManager))]
        private Lazy<ILayoutManager> layoutManager { get; set; }

        protected override void Run()
        {
            base.Run();
            var pc = Context as SoapBox.Snap.Application.PageCollectionItem;
            if (pc != null)
            {
                pc.AddPage();
            }
        }
    }
}
