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
using System.Diagnostics;
using System.ComponentModel.Composition;
using SoapBox.Core;
using System.Timers;
using SoapBox.Protocol.Base;
using SoapBox.Protocol.Automation;
using System.Collections.ObjectModel;
using SoapBox.Snap.Runtime;

namespace SoapBox.Snap.Phidgets
{
    [Export(SoapBox.Snap.Runtime.ExtensionPoints.Runtime.Snap.Drivers, typeof(IRuntimeDriver))]
    public class RuntimeDriver : AbstractExtension, IRuntimeDriver
    {

        private readonly Manager manager = new Manager();

        public RuntimeDriver()
        {
            ID = SoapBox.Snap.Phidgets.Extensions.Runtime.Snap.Drivers.Phidgets;

            //link the event handlers
            manager.Attach += new AttachEventHandler(man_Attach);
            manager.Detach += new DetachEventHandler(man_Detach);
            manager.Error += new ErrorEventHandler(man_Error);
        }

        void man_Attach(object sender, AttachEventArgs e)
        {
            logger.InfoWithFormat("Device {0} serial no. {1} attached!", e.Device.Name,
                                    e.Device.SerialNumber.ToString());
            RaiseConfigurationChanged();
        }

        void man_Detach(object sender, DetachEventArgs e)
        {
            logger.InfoWithFormat("Device {0} serial no. {1} detached!", e.Device.Name,
                                    e.Device.SerialNumber.ToString());
            RaiseConfigurationChanged();
        }

        void man_Error(object sender, ErrorEventArgs e)
        {
            logger.Error(e.Description);
        }

        #region IRuntimeDriver Members

        public void Start()
        {
            Running = true;
            manager.open();
        }

        public void Stop()
        {
            manager.close();
            Running = false;
        }

        public bool Running
        {
            get 
            {
                bool retVal;
                lock (m_RunningLock)
                {
                    retVal = m_Running;
                }
                return retVal; 
            }
            private set
            {
                lock (m_RunningLock)
                {
                    m_Running = value;
                }
            }
        }
        private bool m_Running = false;
        private readonly object m_RunningLock = new object();

        // BE CAREFUL!  RUNS ON A SEPARATE THREAD!
        public NodeDriver ReadConfiguration()
        {
            var driver = NodeDriver.BuildWith(
                new FieldGuid(SnapDriver.PHIDGETS_DRIVER_ID),  // TypeId
                new FieldString(string.Empty),      // Address
                new FieldBase64(string.Empty),      // Configuration
                new FieldString(Resources.Strings.DriverName));// DriverName
            driver = driver.SetRunning(new FieldBool(Running)); // status only
            if (Running) // Running property is threadsafe
            {
                var devicesMutable = new Collection<NodeDevice>();
                // Phidgets claims to be a threadsafe library,
                // and manager is readonly
                foreach (Phidget phidget in manager.Devices)
                {
                    var dev = new PhidgetsDevice(phidget);
                    devicesMutable.Add(dev.Device);
                }
                driver = driver.NodeDeviceChildren.Append(
                    new ReadOnlyCollection<NodeDevice>(devicesMutable));
            }
            return driver;
        }

        public event EventHandler ConfigurationChanged;

        private void RaiseConfigurationChanged()
        {
            var evt = ConfigurationChanged;
            if (evt != null)
            {
                evt(this, new EventArgs());
            }
        }

        public Guid TypeId
        {
            get
            {
                return m_TypeId;
            }
        }
        private readonly Guid m_TypeId = new Guid(SnapDriver.PHIDGETS_DRIVER_ID);

        public void ScanInputs(NodeDriver driver)
        {
            foreach (var device in driver.NodeDeviceChildren.Items)
            {
                if (device.Address.ToString() != string.Empty)
                {
                    switch(device.Code.ToString())
                    {
                        case Phidget_InterfaceKit_888.CODE:
                            var ifKit = OpenInterfaceKit(device);
                            if (ifKit.Attached)
                            {
                                for (int i = 0; i < ifKit.inputs.Count; i++)
                                {
                                    foreach (var input in device.NodeDiscreteInputChildren.Items)
                                    {
                                        if (Int32.Parse(input.Address.ToString()) == i)
                                        {
                                            input.Value = ifKit.inputs[i];
                                        }
                                    }
                                }
                                for (int i = 0; i < ifKit.sensors.Count; i++)
                                {
                                    foreach (var input in device.NodeAnalogInputChildren.Items)
                                    {
                                        if (Int32.Parse(input.Address.ToString()) == i)
                                        {
                                            input.Value = ifKit.sensors[i].Value;
                                        }
                                    }
                                }
                            }
                            break;
                    }
                }
            }
        }

        public void ScanOutputs(NodeDriver driver, NodeRuntimeApplication runtimeApplication)
        {
            foreach (var device in driver.NodeDeviceChildren.Items)
            {
                if (device.Address.ToString() != string.Empty)
                {
                    switch(device.Code.ToString())
                    {
                        case Phidget_InterfaceKit_888.CODE:
                            var ifKit = OpenInterfaceKit(device);
                            if (ifKit.Attached)
                            {
                                for (int i = 0; i < ifKit.outputs.Count; i++)
                                {
                                    foreach (var output in device.NodeDiscreteOutputChildren.Items)
                                    {
                                        if (Int32.Parse(output.Address.ToString()) == i)
                                        {
                                            try
                                            {
                                                ifKit.outputs[i] = output.GetValue(runtimeApplication);
                                            }
                                            catch
                                            {
                                                // FIXME - tie in to error reporting mechanism
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                        case Phidget_ServoMotor_1.CODE:
                            var mtrControl = OpenServo(device);
                            if (mtrControl.Attached)
                            {
                                for (int i = 0; i < mtrControl.servos.Count; i++)
                                {
                                    foreach (var output in device.NodeAnalogOutputChildren.Items)
                                    {
                                        if (Int32.Parse(output.Address.ToString()) == i)
                                        {
                                            try
                                            {
                                                mtrControl.servos[i].Position = Convert.ToDouble(output.GetValue(runtimeApplication));
                                            }
                                            catch
                                            {
                                                // FIXME - tie in to error reporting mechanism
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                        case Phidget_TextLCD_2x20.CODE:
                            var textLCD = OpenTextLCD(device);
                            if (textLCD.Attached)
                            {
                                for (int i = 0; i < textLCD.rows.Count; i++)
                                {
                                    foreach (var output in device.NodeStringOutputChildren.Items)
                                    {
                                        if (Int32.Parse(output.Address.ToString()) == i)
                                        {
                                            var text = output.GetValue(runtimeApplication);
                                            if (text.Length > textLCD.rows[i].MaximumLength)
                                            {
                                                text = text.Substring(0, textLCD.rows[i].MaximumLength);
                                            }
                                            try
                                            {
                                                textLCD.rows[i].DisplayString = text;
                                            }
                                            catch
                                            {
                                                // FIXME - tie in to error reporting mechanism
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                    }
                }
            }
        }

        private InterfaceKit OpenInterfaceKit(NodeDevice device)
        {
            string address = device.Address.ToString();
            if (address == string.Empty)
            {
                throw new ArgumentOutOfRangeException();
            }
            if (!ifKits.ContainsKey(address))
            {
                ifKits.Add(address, new InterfaceKit());
                ifKits[address].open(Int32.Parse(address));
            }
            return ifKits[address];
        }
        private readonly Dictionary<string, InterfaceKit> ifKits = 
            new Dictionary<string, InterfaceKit>();

        private Servo OpenServo(NodeDevice device)
        {
            string address = device.Address.ToString();
            if (address == string.Empty)
            {
                throw new ArgumentOutOfRangeException();
            }
            if (!servos.ContainsKey(address))
            {
                servos.Add(address, new Servo());
                servos[address].open(Int32.Parse(address));
            }
            return servos[address];
        }
        private readonly Dictionary<string, Servo> servos =
            new Dictionary<string, Servo>();

        private TextLCD OpenTextLCD(NodeDevice device)
        {
            string address = device.Address.ToString();
            if (address == string.Empty)
            {
                throw new ArgumentOutOfRangeException();
            }
            if (!textLCDs.ContainsKey(address))
            {
                textLCDs.Add(address, new TextLCD());
                textLCDs[address].open(Int32.Parse(address));
            }
            return textLCDs[address];
        }
        private readonly Dictionary<string, TextLCD> textLCDs =
            new Dictionary<string, TextLCD>();

        #endregion

    }
}
