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

namespace SoapBox.Snap.Application.Workbench.Pads.SolutionPad.RootSolutionItem.ContextMenu
{
    [Export(ExtensionPoints.Workbench.Pads.SolutionPad.RootSolutionItem.ContextMenu, typeof(IMenuItem))]
    class Add : AbstractMenuItem, IPartImportsSatisfiedNotification
    {
        public Add()
        {
            ID = Extensions.Workbench.Pads.SolutionPad_.RootSolutionItem.ContextMenu.Add;
            Header = Resources.Strings.Solution_Pad_RootSolutionItem_Add;
        }

        [Import(SoapBox.Core.Services.Host.ExtensionService)]
        private IExtensionService extensionService { get; set; }

        [ImportMany(ExtensionPoints.Workbench.Pads.SolutionPad.RootSolutionItem.ContextMenu_.Add, 
            typeof(IMenuItem), AllowRecomposition = true)]
        private IEnumerable<IMenuItem> menu { get; set; }

        public void OnImportsSatisfied()
        {
            Items = extensionService.Sort(menu);
        }
    }

    [Export(ExtensionPoints.Workbench.Pads.SolutionPad.RootSolutionItem.ContextMenu_.Add, typeof(IMenuItem))]
    class AddRuntimeApplication : AbstractMenuItem
    {
        public AddRuntimeApplication()
        {
            ID = Extensions.Workbench.Pads.SolutionPad_.RootSolutionItem.ContextMenu.Add_.RuntimeApplication;
            Header = Resources.Strings.Solution_Pad_RootSolutionItem_Add_RuntimeApplication;
            ToolTip = Resources.Strings.Solution_Pad_RootSolutionItem_Add_RuntimeApplication_ToolTip;
        }

        [Import(SoapBox.Core.Services.Layout.LayoutManager, typeof(ILayoutManager))]
        private Lazy<ILayoutManager> layoutManager { get; set; }

        [Import(CompositionPoints.Workbench.Documents.RuntimeApplicationProperties, typeof(RuntimeApplicationProperties))]
        private Lazy<RuntimeApplicationProperties> runtimeApplicationProperties { get; set; }

        protected override void Run()
        {
            base.Run();
            layoutManager.Value.ShowDocument(
                runtimeApplicationProperties.Value, 
                null);
        }
    }
}
