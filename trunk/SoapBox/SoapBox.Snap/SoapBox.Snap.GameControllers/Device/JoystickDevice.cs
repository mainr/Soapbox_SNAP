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
using System.ComponentModel.Composition;
using SoapBox.Protocol.Automation;
using SlimDX.DirectInput;

namespace SoapBox.Snap.GameControllers.Device
{
    [Export(SoapBox.Snap.ExtensionPoints.Device.Types, typeof(ISnapDevice))]
    class JoystickDevice : AbstractGameController
    {
        public const string TYPE_ID = "264d5cdf-fa50-48ab-a31c-ab21351d2e93";
        public const string CODE = "GameController_Joystick";

        public JoystickDevice()
            : base(TYPE_ID, Resources.Strings.Joystick)
        {
            ID = Extensions.Device.Types.GameControllers.GameController_Joystick;
        }

        public override NodeDevice Build()
        {
            throw new NotImplementedException();
        }

        public static NodeDevice StaticBuild(DirectInput directInput, DeviceInstance deviceInstance)
        {
            NodeDevice result;
            using (var joystick = new Joystick(directInput, deviceInstance.InstanceGuid))
            {
                var capabilities = joystick.Capabilities;
                result = StaticBuildHelper(deviceInstance.InstanceName, TYPE_ID,
                    deviceInstance.InstanceGuid, CODE,
                    capabilities.ButtonCount, capabilities.AxesCount, capabilities.PovCount);
            }
            return result;
        }
    }
}
