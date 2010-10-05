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
using Phidgets;
using Phidgets.Events;
using SoapBox.Protocol.Base;
using SoapBox.Protocol.Automation;
using System.Collections.ObjectModel;

namespace SoapBox.Snap.Phidgets
{
    class PhidgetsDevice
    {
        public PhidgetsDevice(Phidget phidget)
        {
            m_Phidget = phidget;
            m_Device = buildDevice();
        }

        #region "Phidget"

        public Phidget Phidget
        {
            get
            {
                return m_Phidget;
            }
        }
        private readonly Phidget m_Phidget = null;

        #endregion

        #region "Device"

        public NodeDevice Device
        {
            get
            {
                return m_Device;
            }
        }
        private readonly NodeDevice m_Device = null;

        private NodeDevice buildDevice()
        {
            NodeDevice device = null;
            switch (Phidget.ID)
            {
                case Phidget.PhidgetID.INTERFACEKIT_0_16_16:
                    device = Phidget_InterfaceKit_0_16_16.StaticBuild(Phidget.SerialNumber);
                    break;
                case Phidget.PhidgetID.INTERFACEKIT_0_0_4:
                    device = Phidget_InterfaceKit_004.StaticBuild(Phidget.SerialNumber);
                    break;
                case Phidget.PhidgetID.INTERFACEKIT_0_0_8:
                    device = Phidget_InterfaceKit_008.StaticBuild(Phidget.SerialNumber);
                    break;
                case Phidget.PhidgetID.INTERFACEKIT_4_8_8:
                    device = Phidget_InterfaceKit_488.StaticBuild(Phidget.SerialNumber);
                    break;
                case Phidget.PhidgetID.INTERFACEKIT_0_8_8_w_LCD:
                    device = Phidget_InterfaceKit_088.StaticBuild(Phidget.SerialNumber);
                    break;
                case Phidget.PhidgetID.INTERFACEKIT_8_8_8:
                case Phidget.PhidgetID.INTERFACEKIT_8_8_8_w_LCD:
                    device = Phidget_InterfaceKit_888.StaticBuild(Phidget.SerialNumber);
                    break;
                case Phidget.PhidgetID.TEXTLCD_2x20:
                case Phidget.PhidgetID.TEXTLCD_2x20_w_0_8_8:
                case Phidget.PhidgetID.TEXTLCD_2x20_w_8_8_8:
                    device = Phidget_TextLCD_2x20.StaticBuild(Phidget.SerialNumber);
                    break;
                case Phidget.PhidgetID.SERVO_1MOTOR:
                case Phidget.PhidgetID.SERVO_1MOTOR_OLD:
                    device = Phidget_ServoMotor_1.StaticBuild(Phidget.SerialNumber);
                    break;
                case Phidget.PhidgetID.SERVO_4MOTOR:
                case Phidget.PhidgetID.SERVO_4MOTOR_OLD:
                    device = Phidget_ServoMotor_4.StaticBuild(Phidget.SerialNumber);
                    break;
                case Phidget.PhidgetID.ADVANCEDSERVO_8MOTOR:
                    device = Phidget_ServoMotor_8.StaticBuild(Phidget.SerialNumber);
                    break;
                default:
                    device = Unknown_Phidget.StaticBuild();
                    break;
            }

            return device;
        }

        #endregion

    }
}
