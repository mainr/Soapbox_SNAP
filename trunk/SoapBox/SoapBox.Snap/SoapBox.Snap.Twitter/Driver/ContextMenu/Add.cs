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
using SoapBox.Core;

namespace SoapBox.Snap.Twitter.Driver.ContextMenu
{
    [Export(ExtensionPoints.Driver.ContextMenu, typeof(IMenuItem))]
    class Add : AbstractMenuItem, IPartImportsSatisfiedNotification
    {
        public Add()
        {
            ID = Extensions.Driver.ContextMenu.Add;
            Header = Resources.Strings.Driver_ContextMenu_Add;
        }

        [ImportMany(ExtensionPoints.Driver.ContextMenu_.Add, typeof(IMenuItem))]
        private IEnumerable<IMenuItem> contextMenu { get; set; }

        [Import(SoapBox.Core.Services.Host.ExtensionService)]
        private IExtensionService extensionService { get; set; }

        public void OnImportsSatisfied()
        {
            Items = extensionService.Sort(contextMenu);
        }
    }

    [Export(ExtensionPoints.Driver.ContextMenu_.Add, typeof(IMenuItem))]
    class AddUserStatusMonitor : AbstractMenuItem
    {
        public AddUserStatusMonitor()
        {
            ID = Extensions.Driver.ContextMenu.Add_.UserStatusMonitor;
            Header = Resources.Strings.Driver_ContextMenu_Add_UserStatusMonitor;
            ToolTip = Resources.Strings.Driver_ContextMenu_Add_UserStatusMonitor_ToolTip;
        }

        protected override void Run()
        {
            var driverItem = Context as IDriverSolutionItem;
            if (driverItem != null)
            {
                var factory = new UserStatusMonitor();
                driverItem.Driver = driverItem.Driver.NodeDeviceChildren.Append(factory.Build());
            }
        }
    }
}
