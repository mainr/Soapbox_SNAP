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

namespace SoapBox.Snap.Twitter
{
    [Export(SoapBox.Snap.ExtensionPoints.Driver.Types, typeof(ISnapDriver))]
    public class SnapDriver : AbstractExtension, ISnapDriver
    {
        public const string TWITTER_DRIVER_ID = "ec70cda5-fb8b-4a93-b295-e64ee91d745b";

        public SnapDriver()
        {
            ID = Extensions.Driver.Types.Twitter;
        }

        public Guid TypeId
        {
            get
            {
                return m_TypeId;
            }
        }
        private readonly Guid m_TypeId = new Guid(SnapDriver.TWITTER_DRIVER_ID);

        public string Name
        {
            get
            {
                return Resources.Strings.DriverName;
            }
        }

        [ImportMany(ExtensionPoints.Driver.ContextMenu, typeof(IMenuItem))]
        public IEnumerable<IMenuItem> ContextMenu { get; set; }
    }
}
