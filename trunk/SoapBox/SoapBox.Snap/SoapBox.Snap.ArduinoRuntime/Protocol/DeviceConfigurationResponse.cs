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
using SoapBox.Snap.ArduinoRuntime.Protocol.Helpers;
using System.Collections.ObjectModel;

namespace SoapBox.Snap.ArduinoRuntime.Protocol
{
    class DeviceConfigurationResponse
    {
        public DeviceConfigurationResponse(SendReceiveResult response)
        {
            if (response == null) throw new ArgumentNullException("response");
            if (!response.Success)
            {
                throw new ProtocolException(response.Error);
            }
            if (response.Lines.Count < 1)
            {
                throw new ProtocolException("Response to \"device-config\" command wasn't at least 1 line long.");
            }

            var deviceNameLine = new KeyAndValue(response.Lines[0]);
            if (deviceNameLine.Key != "Device Name") throw new ProtocolException("Key of Device Name line isn't right.");
            this.DeviceName = deviceNameLine.Value;

            // Remaining lines are comma (and space) separated with the format:
            // Name, Address, Type
            // where Type is one of: input, output, analogInput, analogOutput

            var ioSignals = new List<IOSignal>();
            for (var i = 1; i < response.Lines.Count; i++)
            {
                ioSignals.Add(new IOSignal(response.Lines[i]));
            }
            this.IOSignals = ioSignals.AsReadOnly();
        }

        public string DeviceName { get; private set; }
        public ReadOnlyCollection<IOSignal> IOSignals { get; private set; }

        public class IOSignal
        {
            public IOSignal(string csvLine)
            {
                var split = csvLine.Split(new string[] { ", " }, StringSplitOptions.None);
                this.Name = split[0];
                this.Address = split[1];
                switch (split[2])
                {
                    case "input":
                        this.Type = IOSignalType.DiscreteInput;
                        break;
                    case "output":
                        this.Type = IOSignalType.DiscreteOutput;
                        break;
                    case "analogInput":
                        this.Type = IOSignalType.AnalogInput;
                        break;
                    case "analogOutput":
                        this.Type = IOSignalType.AnalogOutput;
                        break;
                    default:
                        this.Type = IOSignalType.Unknown;
                        break;
                }
            }

            public string Name { get; private set; }
            public string Address { get; private set; }
            public IOSignalType Type { get; private set; }
        }

        public enum IOSignalType
        {
            Unknown = 0,
            DiscreteInput = 1,
            DiscreteOutput = 2,
            AnalogInput = 3,
            AnalogOutput = 4,
        }
    }
}
