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

namespace SoapBox.Snap.ArduinoRuntime.Driver
{
    [Export(SoapBox.Snap.ExtensionPoints.Driver.Types, typeof(ISnapDriver))]
    public class SnapDriver : AbstractExtension, ISnapDriver
    {
        public const string ARDUINO_LOCAL_IO_DRIVER_TYPE_ID = "4003fd70-4862-4110-b53a-2c45780b6b45";

        public SnapDriver()
        {
            ID = Extensions.Driver.Types.ArduinoLocalIO;
        }

        public Guid TypeId
        {
            get
            {
                return m_TypeId;
            }
        }
        private readonly Guid m_TypeId = new Guid(SnapDriver.ARDUINO_LOCAL_IO_DRIVER_TYPE_ID);

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
