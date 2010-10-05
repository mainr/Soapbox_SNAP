#region "SoapBox.Snap License"
/// <header module="SoapBox.Snap"> 
/// Copyright (C) 2009 SoapBox Automation Inc., All Rights Reserved.
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
using SoapBox.Protocol.Base;
using SoapBox.Protocol.Automation;

namespace SoapBox.Snap.Twitter
{
    [Export(SoapBox.Snap.ExtensionPoints.Device.Types, typeof(ISnapDevice))]
    class UserStatusMonitor : AbstractTwitterDevice
    {
        public const string TYPE_ID = "847d8296-2499-44be-8d7c-3f4431eda3d4";
        public const string CODE = "Twitter_UserStatusMonitor";
        public const string STRING_INPUT_0_CODE = "LatestStatus";
        public const string DISCRETE_INPUT_0_CODE = "StatusChanged";

        public UserStatusMonitor()
            : base(TYPE_ID, Resources.Strings.Twitter_UserStatusMonitor)
        {
            ID = Extensions.Device.Types.Twitter.UserStatusMonitor;
        }

        public override NodeDevice Build()
        {
            var device = NodeDevice.BuildWith(
                new FieldIdentifier(CODE),
                new FieldGuid(TYPE_ID),
                new FieldString(string.Empty),
                new FieldBase64(string.Empty),
                new FieldDeviceName(this.Name));
            device = device.NodeStringInputChildren.Append(
                NodeStringInput.BuildWith(
                    new FieldIdentifier(STRING_INPUT_0_CODE),
                    new FieldString(string.Empty),
                    new FieldSignalName(Resources.Strings.UserStatusMonitor_StringInput0)));
            device = device.NodeDiscreteInputChildren.Append(
                NodeDiscreteInput.BuildWith(
                    new FieldIdentifier(DISCRETE_INPUT_0_CODE),
                    new FieldString(string.Empty),
                    new FieldSignalName(Resources.Strings.UserStatusMonitor_DiscreteInput0)));
            return device;
        }
    }
}
