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
    class Phidget_ServoMotor_8 : AbstractPhidget
    {
        public const string TYPE_ID = "56c2eb72-495e-4813-b1f9-0054a339e8cb";
        public const string CODE = "Phidget_ServoMotor_8";

        public Phidget_ServoMotor_8()
            : base(TYPE_ID, Resources.Strings.Phidget_ServoMotor_8_Name)
        {
            ID = Extensions.Device.Types.Phidgets.Phidget_ServoMotor_8;
        }

        public override NodeDevice Build()
        {
            return StaticBuild(0);
        }

        public static NodeDevice StaticBuild(int serialNumber)
        {
            return StaticBuildHelper(Resources.Strings.Phidget_ServoMotor_8_Name, 
                serialNumber, CODE, TYPE_ID, 
                discreteInputs: 0, 
                discreteOutputs: 0, 
                analogInputs: 0,
                analogOutputs: 8,
                stringInputs: 0,
                stringOutputs: 0,
                analogOutputNameOverride: Resources.Strings.ServoOutputName);
        }
    }
}
