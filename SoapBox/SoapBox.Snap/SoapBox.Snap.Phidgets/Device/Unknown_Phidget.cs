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
using SoapBox.Protocol.Automation;
using SoapBox.Protocol.Base;

namespace SoapBox.Snap.Phidgets
{
    [Export(SoapBox.Snap.ExtensionPoints.Device.Types, typeof(ISnapDevice))]
    class Unknown_Phidget : AbstractPhidget
    {
        public const string TYPE_ID = "97b0109f-ee65-4674-8413-05f5fc51f9b1";
        public const string CODE = "Unknown_Phidget";

        public Unknown_Phidget()
            : base(TYPE_ID, Resources.Strings.Unknown_Phidget_Name)
        {
            ID = Extensions.Device.Types.Phidgets.Unknown_Phidget;
        }

        public override NodeDevice Build()
        {
            return StaticBuild();
        }

        public static NodeDevice StaticBuild()
        {
            FieldIdentifier code;
            FieldGuid typeId;
            FieldString address;
            FieldBase64 configuration;
            FieldDeviceName deviceName;

            code = new FieldIdentifier(CODE);
            typeId = new FieldGuid(TYPE_ID);
            address = new FieldString(string.Empty);
            configuration = new FieldBase64(string.Empty);
            deviceName = new FieldDeviceName(Resources.Strings.Unknown_Phidget_Name);

            NodeDevice device = NodeDevice.BuildWith(code, typeId, address, configuration, deviceName);

            return device;
        }
    }
}
