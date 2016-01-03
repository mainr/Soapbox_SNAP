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
using SoapBox.Protocol.Automation;
using SoapBox.Protocol.Base;
using System.Collections.ObjectModel;

namespace SoapBox.Snap.Phidgets
{
    [Export(SoapBox.Snap.ExtensionPoints.Device.Types, typeof(ISnapDevice))]
    class Phidget_InterfaceKit_088 : AbstractPhidget
    {
        public const string TYPE_ID = "326643cb-f65e-4e49-a7eb-c2041ffde4d7";
        public const string CODE = "Phidget_InterfaceKit_088";

        public Phidget_InterfaceKit_088()
            : base(TYPE_ID, Resources.Strings.Phidget_InterfaceKit_088_Name)
        {
            ID = Extensions.Device.Types.Phidgets.Phidget_InterfaceKit_088;
        }

        public override NodeDevice Build()
        {
            return StaticBuild(0);
        }

        public static NodeDevice StaticBuild(int serialNumber)
        {
            return StaticBuildHelper(Resources.Strings.Phidget_InterfaceKit_088_Name, 
                serialNumber, CODE, TYPE_ID, 
                discreteInputs: 8, 
                discreteOutputs: 8, 
                analogInputs: 0,
                analogOutputs: 0,
                stringInputs: 0,
                stringOutputs: 0);
        }
    }
}
