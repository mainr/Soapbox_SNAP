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
using SoapBox.Core;
using System.ComponentModel.Composition;
using TweetSharp.Twitter.Fluent;
using TweetSharp.Twitter.Extensions;
using SoapBox.Protocol.Base;

namespace SoapBox.Snap.Twitter.Device.ContextMenu
{
    [Export(ExtensionPoints.Device.ContextMenu, typeof(IMenuItem))]
    class ClearCredentials : AbstractMenuItem
    {
        public ClearCredentials()
        {
            ID = Extensions.Device.ContextMenu.ClearCredentials;
            Header = Resources.Strings.ContextMenu_ClearCredentials;
            ToolTip = Resources.Strings.ContextMenu_ClearCredentials_ToolTip;
            SetIconFromBitmap(Resources.Images.ClearCredentials);
        }

        protected override void Run()
        {
            base.Run();
            var deviceItem = Context as IDeviceSolutionItem;
            if (deviceItem != null)
            {
                deviceItem.Device = deviceItem.Device.SetAddress(new FieldString(string.Empty));
            }
        }

    }
}
