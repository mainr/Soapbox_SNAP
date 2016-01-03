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
using SoapBox.Protocol.Automation;
using SoapBox.Protocol.Base;

namespace SoapBox.Snap.Phidgets.Extensions
{
    public static class Runtime
    {
        public static class Snap
        {
            public static class Drivers
            {
                public const string Phidgets = "Phidgets";
            }
        }
    }
    public static class Driver
    {
        public static class Types
        {
            public const string Phidgets = "Phidgets";
        }
    }
    public static class Device
    {
        public static class Types
        {
            public static class Phidgets
            {
                public const string Unknown_Phidget = "Unknown_Phidget";
                public const string Phidget_InterfaceKit_888 = "Phidget_InterfaceKit_888";
                public const string Phidget_InterfaceKit_004 = "Phidget_InterfaceKit_004";
                public const string Phidget_InterfaceKit_008 = "Phidget_InterfaceKit_008";
                public const string Phidget_InterfaceKit_088 = "Phidget_InterfaceKit_088";
                public const string Phidget_InterfaceKit_488 = "Phidget_InterfaceKit_488";
                public const string Phidget_InterfaceKit_0_16_16 = "Phidget_InterfaceKit_0_16_16";
                public const string Phidget_TextLCD_2x20 = "Phidget_TextLCD_2x20";
                public const string Phidget_ServoMotor_1 = "Phidget_ServoMotor_1";
                public const string Phidget_ServoMotor_4 = "Phidget_ServoMotor_4";
                public const string Phidget_ServoMotor_8 = "Phidget_ServoMotor_8";
                public const string Phidget_AdvancedServo = "Phidget_AdvancedServo";
            }
        }
    }
}
