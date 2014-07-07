#region "SoapBox.Snap License"
/// <header module="SoapBox.Snap"> 
/// Copyright (C) 2009-2014 SoapBox Automation, All Rights Reserved.
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
using SoapBox.Snap.Runtime;
using SoapBox.Protocol.Automation;
using SoapBox.Protocol.Base;
using SoapBox.Snap.GameControllers.Driver;
using System.Collections.ObjectModel;
using SlimDX;
using SlimDX.DirectInput;
using SoapBox.Snap.GameControllers.Device;

namespace SoapBox.Snap.GameControllers.Runtime
{
    [Export(SoapBox.Snap.Runtime.ExtensionPoints.Runtime.Snap.Drivers, typeof(IRuntimeDriver))]
    public class RuntimeDriver : AbstractExtension, IRuntimeDriver
    {
        private readonly DirectInput directInput;
        private readonly Dictionary<Guid, Joystick> acquiredJoysticks = new Dictionary<Guid, Joystick>();

        public RuntimeDriver()
        {
            try
            {
                this.directInput = new DirectInput();
            }
            catch { }
        }

        #region IRuntimeDriver Members

        public void Start()
        {
            Running = true;
        }

        public void Stop()
        {
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
                new FieldGuid(SnapDriver.GAMECONTROLLERS_DRIVER_ID),  // TypeId
                new FieldString(string.Empty),      // Address
                new FieldBase64(string.Empty),      // Configuration
                new FieldString(Resources.Strings.DriverName));// DriverName
            driver = driver.SetRunning(new FieldBool(Running)); // status only
            if (Running && this.directInput != null) // Running property is threadsafe
            {
                var devicesMutable = new Collection<NodeDevice>();
                var joystickDevices = getJoystickCompatibleDevices();
                foreach (var joystickDevice in joystickDevices)
                {
                    var dev = JoystickDevice.StaticBuild(this.directInput, joystickDevice);
                    devicesMutable.Add(dev);
                }
                driver = driver.NodeDeviceChildren.Append(
                    new ReadOnlyCollection<NodeDevice>(devicesMutable));
            }
            return driver;
        }

        private IEnumerable<DeviceInstance> getJoystickCompatibleDevices()
        {
            var result =
                (from d in this.directInput.GetDevices()
                 where (DeviceType)(Convert.ToInt32(d.Type) & 0xFF) == DeviceType.Joystick
                    || (DeviceType)(Convert.ToInt32(d.Type) & 0xFF) == DeviceType.Driving
                    || (DeviceType)(Convert.ToInt32(d.Type) & 0xFF) == DeviceType.Gamepad
                    || (DeviceType)(Convert.ToInt32(d.Type) & 0xFF) == DeviceType.Flight
                    || (DeviceType)(Convert.ToInt32(d.Type) & 0xFF) == DeviceType.FirstPerson
                 select d);
            return result;
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
        private readonly Guid m_TypeId = new Guid(SnapDriver.GAMECONTROLLERS_DRIVER_ID);

        public void ScanInputs(NodeDriver driver)
        {
            if (this.directInput == null)
            {
                return;
            }

            foreach (var device in driver.NodeDeviceChildren)
            {
                switch (device.TypeId.ToString())
                {
                    case JoystickDevice.TYPE_ID:
                        scanJoystickInputs(device);
                        break;
                }
            }
        }

        public void ScanOutputs(NodeDriver driver, NodeRuntimeApplication runtimeApplication)
        {
        }

        private bool deviceAttached(Guid instanceId)
        {
            bool result = false;
            try
            {
                result = this.directInput.IsDeviceAttached(instanceId);
            }
            catch (DirectInputException)
            {
                result = false;
            }
            return result;
        }

        private void scanJoystickInputs(NodeDevice device)
        {
            var address = device.Address.ToString();
            Guid instanceId = Guid.Parse(address);
            if (deviceAttached(instanceId))
            {
                Joystick joystick;
                if (this.acquiredJoysticks.ContainsKey(instanceId))
                {
                    joystick = this.acquiredJoysticks[instanceId];
                }
                else
                {
                    joystick = new Joystick(this.directInput, instanceId);
                    joystick.Acquire();
                    this.acquiredJoysticks.Add(instanceId, joystick);
                }

                scanJoystickInputs(device, joystick);
            }
            else
            {
                clearJoystickInputs(device);
                if (this.acquiredJoysticks.ContainsKey(instanceId))
                {
                    var joystick = this.acquiredJoysticks[instanceId];
                    joystick.Unacquire();
                    joystick.Dispose();
                    this.acquiredJoysticks.Remove(instanceId);
                }
            }
        }

        private void scanJoystickInputs(NodeDevice device, Joystick joystick)
        {
            var state = joystick.GetCurrentState();
            foreach (var discreteInput in device.NodeDiscreteInputChildren)
            {
                int buttonIndex = Convert.ToInt32(discreteInput.Address.ToString());
                var buttons = state.GetButtons();
                discreteInput.Value = buttons[buttonIndex];
            }
            foreach (var analogInput in device.NodeAnalogInputChildren)
            {
                int rawValue = 0;
                if (analogInput.Code.ToString().StartsWith(Resources.Strings.PoVHat))
                {
                    var povIndex = Convert.ToInt32(analogInput.Address.ToString());
                    rawValue = state.GetPointOfViewControllers()[povIndex];
                }
                else
                {
                    switch (analogInput.Address.ToString())
                    {
                        case "X":
                            rawValue = state.X;
                            break;
                        case "Y":
                            rawValue = state.Y;
                            break;
                        case "Z":
                            rawValue = state.Z;
                            break;
                        case "RotationX":
                            rawValue = state.RotationX;
                            break;
                        case "RotationY":
                            rawValue = state.RotationY;
                            break;
                        case "RotationZ":
                            rawValue = state.RotationZ;
                            break;
                    }
                }
                analogInput.Value = rawValue;
            }
        }

        private void clearJoystickInputs(NodeDevice device)
        {
            foreach (var discreteInput in device.NodeDiscreteInputChildren)
            {
                discreteInput.Value = false;
            }
            foreach (var analogInput in device.NodeAnalogInputChildren)
            {
                analogInput.Value = 0M;
            }
        }

        #endregion


    }
}
