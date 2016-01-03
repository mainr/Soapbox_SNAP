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
using SoapBox.Protocol.Base;
using SoapBox.Protocol.Automation;
using System.Collections.ObjectModel;
using System.IO.Ports;
using SoapBox.Snap.ArduinoRuntime.Driver;
using SoapBox.Snap.ArduinoRuntime.Protocol;
using SoapBox.Snap.ArduinoRuntime.Protocol.Compiler;
using System.Xml.Linq;
using System.ComponentModel.Composition;
using SoapBox.Core;
using System.IO;

namespace SoapBox.Snap.ArduinoRuntime
{
    public class ArduinoRuntimeProxy : IRuntime, IDisposable
    {
        const string DISCRETE_INPUTS_DEVICE_ID = "f1ca0605-62b6-417a-bbec-d8ee79f788f1";
        const string DISCRETE_OUTPUTS_DEVICE_ID = "f95cf75a-3c0e-4893-a2a1-8ebf754cb023";
        const string ANALOG_INPUTS_DEVICE_ID = "de51bdd2-243b-4745-8544-985e46e04a76";
        const string ANALOG_OUTPUTS_DEVICE_ID = "8a05305d-1984-44db-8812-ef5abf700925";

        private readonly ArduinoRuntimeProtocol m_arduinoRuntimeProtocol;
        private readonly InformationResponse m_information;
        private readonly SignalTable m_signalTable;
        private readonly ArduinoLadderCompiler m_compiler;
        private readonly Lazy<IRuntimeService> m_runtimeService = null;

        public ArduinoRuntimeProxy(
            IRuntimeType runtimeType,
            string comPort,
            Lazy<IRuntimeService> runtimeService,
            IMessagingService messagingService)
        {
            if (runtimeType == null) throw new ArgumentNullException("runtimeType");
            if (runtimeService == null) throw new ArgumentNullException("runtimeService");
            if (messagingService == null) throw new ArgumentNullException("messagingService");
            this.Type = runtimeType;
            this.m_runtimeService = runtimeService;
            this.m_arduinoRuntimeProtocol = new ArduinoRuntimeProtocol(comPort);
            this.messagingService = messagingService;

            this.m_runtimeService.Value.SignalChanged += (s) =>
                {
                    Console.WriteLine("Signal {0} changed. Forced: {1}, Forced Value: {2}", 
                        s.SignalName.ToString(),
                        s.Forced.BoolValue,
                        s.ForcedValue.Value.ToString());
                };

            this.m_arduinoRuntimeProtocol.Connect();
            this.m_information = this.m_arduinoRuntimeProtocol.GetInformation();
            this.m_arduinoRuntimeProtocol.Disconnect();
            this.m_signalTable = new SignalTable(this.m_information);
            this.m_compiler = new ArduinoLadderCompiler(this.m_information);
        }

        private IMessagingService messagingService { get; set; }

        public string ComPort { get { return this.m_arduinoRuntimeProtocol.ComPort; } }
        public IRuntimeType Type { get; private set; }

        public bool Start()
        {
            this.m_arduinoRuntimeProtocol.Connect();
            return this.m_arduinoRuntimeProtocol.Start().Success;
        }

        public bool Stop()
        {
            this.m_arduinoRuntimeProtocol.Connect();
            return this.m_arduinoRuntimeProtocol.Stop().Success;
        }

        public bool Running
        {
            get
            {
                this.m_arduinoRuntimeProtocol.Connect();
                var runtimeStatus = this.m_arduinoRuntimeProtocol.GetRuntimeStatus();
                return runtimeStatus.Running; 
            }
        }

        private readonly object m_runtimeApplication_Lock = new object();
        private NodeRuntimeApplication m_runtimeApplication = null;

        /// <summary>
        /// Threadsafe local storage of runtimeApplication
        /// </summary>
        private NodeRuntimeApplication nodeRuntimeApplication
        {
            get
            {
                lock (this.m_runtimeApplication_Lock)
                {
                    return this.m_runtimeApplication;
                }
            }
            set
            {
                lock (this.m_runtimeApplication_Lock)
                {
                    this.m_runtimeApplication = value;
                    this.m_signalTable.BuildSignalTable(value);
                }
            }
        }

        public void RuntimeApplicationGoOnline(NodeRuntimeApplication runtimeApplication)
        {
            if (runtimeApplication == null)
            {
                throw new ArgumentNullException("runtimeApplication");
            }
            this.nodeRuntimeApplication = runtimeApplication;
        }

        public bool RuntimeApplicationDownload(NodeRuntimeApplication runtimeApplication, bool onlineChange)
        {
            this.m_arduinoRuntimeProtocol.Connect();
            this.nodeRuntimeApplication = runtimeApplication;
            if (runtimeApplication != null)
            {
                if (Properties.Settings.Default.ExportSignalTable)
                {
                    var csv = this.m_signalTable.GenerateCSV();
                    try
                    {
                        var directory = Properties.Settings.Default.ExportSignalTableTo;
                        var filename = string.Format(@"{0}_{1}.csv",
                            runtimeApplication.Code,
                            "SignalTable");
                        var fullyQualifiedFilename = Path.Combine(directory, filename);
                        File.WriteAllText(fullyQualifiedFilename, csv);
                    }
                    catch (Exception)
                    {
                    }
                }
                var compiledApplication = this.m_compiler.Compile(runtimeApplication, this.m_signalTable);
                var bytes = compiledApplication.ToByteArray();

                if (bytes.Length > this.m_information.MaxProgramSize)
                {
                    this.messagingService.ShowMessage(
                        string.Format("The compiled program size ({0} bytes) is larger than the maximum allowed program size of {1} bytes.",
                            bytes.Length,
                            this.m_information.MaxProgramSize),
                        "Compiled program is too large.");
                    return false;
                }
                else if (!onlineChange && bytes.Length > this.m_information.MaxProgramSize * 0.9)
                {
                    this.messagingService.ShowMessage(
                        string.Format("The compiled program size ({0} bytes) is more than 90% of the maximum allowed program size of {1} bytes.",
                            bytes.Length,
                            this.m_information.MaxProgramSize),
                        "Nearly reached compiled program size limit.");
                }

                if (!onlineChange)
                {
                    CommandResponse response;
                    try
                    {
                        response = this.m_arduinoRuntimeProtocol.ResetConfiguration();
                    }
                    catch (ProtocolException ex)
                    {
                        this.messagingService.ShowMessage(
                            ex.Message,
                            "Error resetting device configuration");
                        return false;
                    }
                    if (!response.Success)
                    {
                        throw new Exception("Configuration reset failed.");
                    }

                    var configurationLines = runtimeApplication.Configuration.Decode()
                        .Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var configurationLine in configurationLines)
                    {
                        if (configurationLine.StartsWith("config-output ")
                            || configurationLine.StartsWith("config-pwm "))
                        {
                            var configurationResponse = this.m_arduinoRuntimeProtocol.Configure(configurationLine);
                            if (!configurationResponse.Success)
                            {
                                throw new Exception("Configuration failed: " + configurationLine);
                            }
                        }
                    }
                }

                var downloadResponse = onlineChange
                    ? this.m_arduinoRuntimeProtocol.Patch(bytes)
                    : this.m_arduinoRuntimeProtocol.Download(bytes);
                if (!downloadResponse.Success)
                {
                    return false;
                }
            }
            return true;
        }

        public NodeRuntimeApplication RuntimeApplicationUpload()
        {
            this.m_arduinoRuntimeProtocol.Connect();
            // This would be difficult, based on the limited
            // resources of the Arduino board.  Even if we could do
            // an upload, original coil names would be lost
            // along with the comments (obviously).
            return this.nodeRuntimeApplication;
        }

        public void Disconnect()
        {
            this.m_arduinoRuntimeProtocol.Disconnect();
        }

        /// <summary>
        /// Supposed to return a unique ID for the runtime (physical Arduino in this case)
        /// Should return null if no program in runtime
        /// </summary>
        /// <returns></returns>
        public FieldGuid RuntimeId()
        {
            RuntimeIdResponse response;
            try
            {
                this.m_arduinoRuntimeProtocol.Connect();
                response = this.m_arduinoRuntimeProtocol.RuntimeId();
            }
            catch (ProtocolException ex)
            {
                this.messagingService.ShowMessage(
                    ex.Message,
                    "Error reading Runtime ID");
                return null;
            }
            if (response.RuntimeId == Guid.Empty)
            {
                return null;
            }
            return new FieldGuid(response.RuntimeId);
        }

        /// <summary>
        /// This returns the NodeApplication.RuntimeID of the latest download.
        /// This is to check if the version of the program in the runtime (Arduino)
        /// is the same as the version in the ladder editor.  If not, when we
        /// have to do a download (or upload if possible) before going online.
        /// </summary>
        public FieldGuid RuntimeVersionId()
        {
            this.m_arduinoRuntimeProtocol.Connect();
            var response = this.m_arduinoRuntimeProtocol.VersionId();
            if (response.RuntimeVersionId == Guid.Empty)
            {
                return null;
            }
            return new FieldGuid(response.RuntimeVersionId);
        }

        public NodeDeviceConfiguration ReadConfiguration()
        {
            this.m_arduinoRuntimeProtocol.Connect();
            var deviceConfigurationResponse = this.m_arduinoRuntimeProtocol.GetDeviceConfiguration();

            NodeDeviceConfiguration existingDeviceConfiguration = null;
            if (this.nodeRuntimeApplication != null)
            {
                existingDeviceConfiguration = this.nodeRuntimeApplication.DeviceConfiguration;
            }

            return MergeDeviceConfiguration(deviceConfigurationResponse, existingDeviceConfiguration);
        }

        internal static NodeDeviceConfiguration MergeDeviceConfiguration(
            DeviceConfigurationResponse deviceConfigurationResponse, 
            NodeDeviceConfiguration existingDeviceConfiguration = null)
        {
            var driversMutable = new Collection<NodeDriver>();
            var nodeDriver = NodeDriver.BuildWith(
                new FieldGuid(SnapDriver.ARDUINO_LOCAL_IO_DRIVER_TYPE_ID), // TypeId
                new FieldString(string.Empty), // Address
                FieldBase64.Encode(string.Empty), // Configuration
                new FieldString(deviceConfigurationResponse.DeviceName));

            var devicesMutable = new Collection<NodeDevice>();

            // Just creating 4 "virtual" devices under the Arduino Local I/O driver
            // so that it looks organized in the Solution Explorer
            var discreteInputsDevice = NodeDevice.BuildWith(
                new FieldIdentifier("DiscreteInputs"), // Code
                new FieldGuid(DISCRETE_INPUTS_DEVICE_ID), // TypeId
                new FieldString(), // Address
                new FieldBase64(string.Empty), // Configuration
                new FieldDeviceName("Discrete Inputs"));
            var discreteOutputsDevice = NodeDevice.BuildWith(
                new FieldIdentifier("DiscreteOutputs"), // Code
                new FieldGuid(DISCRETE_OUTPUTS_DEVICE_ID), // TypeId
                new FieldString(), // Address
                new FieldBase64(string.Empty), // Configuration
                new FieldDeviceName("Discrete Outputs"));
            var analogInputsDevice = NodeDevice.BuildWith(
                new FieldIdentifier("AnalogInputs"), // Code
                new FieldGuid(ANALOG_INPUTS_DEVICE_ID), // TypeId
                new FieldString(), // Address
                new FieldBase64(string.Empty), // Configuration
                new FieldDeviceName("Analog Inputs"));
            var analogOutputsDevice = NodeDevice.BuildWith(
                new FieldIdentifier("AnalogOutputs"), // Code
                new FieldGuid(ANALOG_OUTPUTS_DEVICE_ID), // TypeId
                new FieldString(), // Address
                new FieldBase64(string.Empty), // Configuration
                new FieldDeviceName("Analog Outputs"));

            var discreteInputsMutable = new Collection<NodeDiscreteInput>();
            var discreteOutputsMutable = new Collection<NodeDiscreteOutput>();
            var analogInputsMutable = new Collection<NodeAnalogInput>();
            var analogOutputsMutable = new Collection<NodeAnalogOutput>();

            foreach (var ioSignal in deviceConfigurationResponse.IOSignals)
            {
                switch (ioSignal.Type)
                {
                    case DeviceConfigurationResponse.IOSignalType.DiscreteInput:
                        var newDiscreteInput = NodeDiscreteInput.BuildWith(
                            new FieldIdentifier(ioSignal.Name),
                            new FieldString(ioSignal.Address),
                            new FieldSignalName(ioSignal.Name));
                        if (existingDeviceConfiguration != null)
                        {
                            var existingDiscreteInput = existingDeviceConfiguration.GetChildrenRecursive()
                                .Select(x => x.Value as NodeDiscreteInput)
                                .Where(x => x != null)
                                .Where(x => x.Code.ToString() == ioSignal.Name)
                                .SingleOrDefault(x => x.Address.ToString() == ioSignal.Address);
                            if (existingDiscreteInput != null)
                            {
                                newDiscreteInput = newDiscreteInput
                                    .SetSignal(existingDiscreteInput.Signal);
                            }
                        }
                        discreteInputsMutable.Add(newDiscreteInput);
                        break;
                    case DeviceConfigurationResponse.IOSignalType.DiscreteOutput:
                        var newDiscreteOutput = NodeDiscreteOutput.BuildWith(
                            new FieldIdentifier(ioSignal.Name),
                            new FieldString(ioSignal.Address),
                            new FieldSignalName(ioSignal.Name));
                        if (existingDeviceConfiguration != null)
                        {
                            var existingDiscreteOutput = existingDeviceConfiguration.GetChildrenRecursive()
                                .Select(x => x.Value as NodeDiscreteOutput)
                                .Where(x => x != null)
                                .Where(x => x.Code.ToString() == ioSignal.Name)
                                .SingleOrDefault(x => x.Address.ToString() == ioSignal.Address);
                            if (existingDiscreteOutput != null)
                            {
                                newDiscreteOutput = newDiscreteOutput
                                    .SetSignalIn(existingDiscreteOutput.SignalIn);
                            }
                        }
                        discreteOutputsMutable.Add(newDiscreteOutput);
                        break;
                    case DeviceConfigurationResponse.IOSignalType.AnalogInput:
                        var newAnalogInput = NodeAnalogInput.BuildWith(
                            new FieldIdentifier(ioSignal.Name),
                            new FieldString(ioSignal.Address),
                            new FieldSignalName(ioSignal.Name));
                        if (existingDeviceConfiguration != null)
                        {
                            var existingAnalogInput = existingDeviceConfiguration.GetChildrenRecursive()
                                .Select(x => x.Value as NodeAnalogInput)
                                .Where(x => x != null)
                                .Where(x => x.Code.ToString() == ioSignal.Name)
                                .SingleOrDefault(x => x.Address.ToString() == ioSignal.Address);
                            if (existingAnalogInput != null)
                            {
                                newAnalogInput = newAnalogInput
                                    .SetSignal(existingAnalogInput.Signal);
                            }
                        }
                        analogInputsMutable.Add(newAnalogInput);
                        break;
                    case DeviceConfigurationResponse.IOSignalType.AnalogOutput:
                        var newAnalogOutput = NodeAnalogOutput.BuildWith(
                            new FieldIdentifier(ioSignal.Name),
                            new FieldString(ioSignal.Address),
                            new FieldSignalName(ioSignal.Name));
                        if (existingDeviceConfiguration != null)
                        {
                            var existingAnalogOutput = existingDeviceConfiguration.GetChildrenRecursive()
                                .Select(x => x.Value as NodeAnalogOutput)
                                .Where(x => x != null)
                                .Where(x => x.Code.ToString() == ioSignal.Name)
                                .SingleOrDefault(x => x.Address.ToString() == ioSignal.Address);
                            if (existingAnalogOutput != null)
                            {
                                newAnalogOutput = newAnalogOutput
                                    .SetSignalIn(existingAnalogOutput.SignalIn);
                            }
                        }
                        analogOutputsMutable.Add(newAnalogOutput);
                        break;
                }
            }

            discreteInputsDevice = discreteInputsDevice.NodeDiscreteInputChildren.Append(
                new ReadOnlyCollection<NodeDiscreteInput>(discreteInputsMutable));
            discreteOutputsDevice = discreteOutputsDevice.NodeDiscreteOutputChildren.Append(
                new ReadOnlyCollection<NodeDiscreteOutput>(discreteOutputsMutable));
            analogInputsDevice = analogInputsDevice.NodeAnalogInputChildren.Append(
                new ReadOnlyCollection<NodeAnalogInput>(analogInputsMutable));
            analogOutputsDevice = analogOutputsDevice.NodeAnalogOutputChildren.Append(
                new ReadOnlyCollection<NodeAnalogOutput>(analogOutputsMutable));

            devicesMutable.Add(discreteInputsDevice);
            devicesMutable.Add(discreteOutputsDevice);
            devicesMutable.Add(analogInputsDevice);
            devicesMutable.Add(analogOutputsDevice);
            nodeDriver = nodeDriver.NodeDeviceChildren.Append(
                new ReadOnlyCollection<NodeDevice>(devicesMutable));

            driversMutable.Add(nodeDriver);
            var nodeDeviceConfiguration = NodeDeviceConfiguration.BuildWith(
                new ReadOnlyCollection<NodeDriver>(driversMutable));
            return nodeDeviceConfiguration;
        }

        public void MessageReceivedFromPeer(NodeBase message)
        {
            // not using SoapBox.Protocol to talk to the runtime (Arduino)
            throw new NotImplementedException();
        }

        public NodeBase DeltaReceivedFromPeer(FieldGuid basedOnMessageID)
        {
            // not using SoapBox.Protocol to talk to the runtime (Arduino)
            throw new NotImplementedException();
        }

        // Ladder editor calls this when it needs to display current values
        // of variables (coils, registers etc.) in the runtime (Arduino).
        public void ReadSignalValues(IEnumerable<NodeSignal> signals)
        {
            if (!this.m_arduinoRuntimeProtocol.Connected)
            {
                return;
            }
            if (this.nodeRuntimeApplication == null)
            {
                throw new Exception("nodeRuntimeApplication is null.");
            }
            foreach (var signal in signals)
            {
                switch (signal.DataType.DataType)
                {
                    case FieldDataType.DataTypeEnum.BOOL:
                        Int16 booleanAddress = -1;
                        var gotBooleanAddress = false;
                        lock (this.m_runtimeApplication_Lock)
                        {
                            gotBooleanAddress = this.m_signalTable.TryGetBooleanAddressBySignal(signal, out booleanAddress);
                        }
                        if (gotBooleanAddress && booleanAddress > -1)
                        {
                            try
                            {
                                this.readBooleanSignal(signal, booleanAddress);
                            }
                            catch (ProtocolException)
                            {
                            }
                        }
                        break;
                    case FieldDataType.DataTypeEnum.NUMBER:
                        byte? numericAddress = null;
                        var gotNumericAddress = false;
                        lock (this.m_runtimeApplication_Lock)
                        {
                            gotNumericAddress = this.m_signalTable.TryGetNumericAddressBySignal(signal, out numericAddress);
                        }
                        if (gotNumericAddress && numericAddress.HasValue)
                        {
                            try
                            {
                                this.readNumericSignal(signal, numericAddress.Value);
                            }
                            catch (ProtocolException)
                            {
                            }
                        }
                        break;
                }
            }
        }

        private void readNumericSignal(NodeSignal signal, byte numericAddress)
        {
            var readNumericResponse = this.m_arduinoRuntimeProtocol.ReadNumeric(numericAddress);
            signal.Value = readNumericResponse.Value;
            this.m_runtimeService.Value.NotifyValueChanged(signal, readNumericResponse.Value);
        }

        private void readBooleanSignal(NodeSignal signal, Int16 booleanAddress)
        {
            var readBooleanResponse = this.m_arduinoRuntimeProtocol.ReadBoolean(booleanAddress);
            signal.Value = readBooleanResponse.Value;
            this.m_runtimeService.Value.NotifyValueChanged(signal, readBooleanResponse.Value);
        }

        public void Dispose()
        {
            if (this.m_arduinoRuntimeProtocol != null)
            {
                this.m_arduinoRuntimeProtocol.Dispose();
            }
        }
    }
}
