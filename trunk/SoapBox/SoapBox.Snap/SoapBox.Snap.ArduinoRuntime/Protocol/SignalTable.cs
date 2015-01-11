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
using SoapBox.Protocol.Automation;
using SoapBox.Protocol.Base;
using System.Collections.ObjectModel;

namespace SoapBox.Snap.ArduinoRuntime.Protocol
{
    class SignalTable
    {
        private readonly InformationResponse m_information;
        private NodeSignal[] m_booleanSignalsByAddress;
        private NodeSignal[] m_numericSignalsByAddress;
        private readonly Dictionary<Guid, Int16> m_booleanAddressBySignal = new Dictionary<Guid, Int16>();
        private readonly Dictionary<Guid, byte> m_numericAddressBySignal = new Dictionary<Guid, byte>();
        private readonly List<Tuple<Int16, Int16>> m_booleanJumpers = new List<Tuple<Int16, Int16>>();
        private readonly List<Tuple<byte, byte>> m_numericJumpers = new List<Tuple<byte, byte>>();

        public SignalTable(InformationResponse information)
        {
            if (information == null) throw new ArgumentNullException("information");
            this.m_information = information;
            this.m_booleanSignalsByAddress = new NodeSignal[information.Booleans];
            this.m_numericSignalsByAddress = new NodeSignal[information.Numerics];
            this.BooleanAddressBits = bitsPerAddress(information.Booleans - 1);
            this.NumericAddressBits = bitsPerAddress(information.Numerics - 1);
        }

        public byte BooleanAddressBits { get; private set; }
        public byte NumericAddressBits { get; private set; }
        public ReadOnlyCollection<Tuple<Int16, Int16>> BooleanJumpers
        {
            get
            {
                return this.m_booleanJumpers.AsReadOnly();
            }
        }
        public ReadOnlyCollection<Tuple<byte, byte>> NumericJumpers
        {
            get
            {
                return this.m_numericJumpers.AsReadOnly();
            }
        }

        private byte bitsPerAddress(int maxAddress)
        {
            var x = 1;
            for (byte n = 1; n <= 255; n++)
            {
                x *= 2;
                var maxAddressable = x - 1;
                if (maxAddressable >= maxAddress)
                {
                    return n;
                }
            }
            throw new ArgumentOutOfRangeException("maxAddress is too big");
        }

        public NodeSignal GetBooleanSignalByAddress(Int16 address)
        {
            NodeSignal result;
            if (!this.TryGetBooleanSignalByAddress(address, out result))
            {
                throw new Exception("Invalid boolean address: " + address);
            }
            return result;
        }

        public bool TryGetBooleanSignalByAddress(Int16 address, out NodeSignal signal)
        {
            if (address < 0 || address >= this.m_booleanSignalsByAddress.Length)
            {
                signal = null;
                return false;
            }
            signal = this.m_booleanSignalsByAddress[address];
            return signal != null;
        }

        public NodeSignal GetNumericSignalByAddress(byte address)
        {
            NodeSignal result;
            if (!this.TryGetNumericSignalByAddress(address, out result))
            {
                throw new Exception("Invalid numeric address.");
            }
            return result;
        }

        public bool TryGetNumericSignalByAddress(Byte address, out NodeSignal signal)
        {
            if (address < 0 || address >= this.m_numericSignalsByAddress.Length)
            {
                signal = null;
                return false;
            }
            signal = this.m_numericSignalsByAddress[address];
            return signal != null;
        }

        public Int16 GetBooleanSignalAddress(NodeSignal signal)
        {
            if (signal == null) throw new ArgumentNullException("signal");
            Int16 result;
            if (!this.TryGetBooleanAddressBySignal(signal, out result))
            {
                throw new Exception("Signal not found.");
            }
            return result;
        }

        public Int16 GetBooleanSignalAddress(NodeSignalIn signalIn)
        {
            if (signalIn == null) throw new ArgumentNullException("signalIn");
            Int16 result;
            if (!this.TryGetBooleanAddressBySignal(signalIn, out result))
            {
                throw new Exception("Signal not found.");
            }
            return result;
        }

        public bool TryGetBooleanAddressBySignal(NodeSignal signal, out Int16 booleanAddress)
        {
            if (signal == null) throw new ArgumentNullException("signal");
            var signalId = Guid.Parse(signal.SignalId.ToString());
            if (this.m_booleanAddressBySignal.ContainsKey(signalId))
            {
                booleanAddress = this.m_booleanAddressBySignal[signalId];
                return true;
            }
            booleanAddress = -1;
            return false;
        }

        public bool TryGetBooleanAddressBySignal(NodeSignalIn signalIn, out Int16 booleanAddress)
        {
            if (signalIn == null) throw new ArgumentNullException("signalIn");
            if (signalIn.SignalId == null) throw new ArgumentNullException("signalIn.SignalId");
            var signalId = Guid.Parse(signalIn.SignalId.ToString());
            if (this.m_booleanAddressBySignal.ContainsKey(signalId))
            {
                booleanAddress = this.m_booleanAddressBySignal[signalId];
                return true;
            }
            booleanAddress = -1;
            return false;
        }

        public byte GetNumericSignalAddress(NodeSignal signal)
        {
            if (signal == null) throw new ArgumentNullException("signal");
            Byte? result;
            if (!this.TryGetNumericAddressBySignal(signal, out result))
            {
                throw new Exception("Signal not found.");
            }
            return result.Value;
        }

        public byte GetNumericSignalAddress(NodeSignalIn signalIn)
        {
            if (signalIn == null) throw new ArgumentNullException("signalIn");
            Byte? result;
            if (!this.TryGetNumericAddressBySignal(signalIn, out result))
            {
                throw new Exception("Signal not found.");
            }
            return result.Value;
        }

        public bool TryGetNumericAddressBySignal(NodeSignal signal, out Byte? numericAddress)
        {
            var signalId = Guid.Parse(signal.SignalId.ToString());
            if (this.m_numericAddressBySignal.ContainsKey(signalId))
            {
                numericAddress = this.m_numericAddressBySignal[signalId];
                return true;
            }
            numericAddress = null;
            return false;
        }

        public bool TryGetNumericAddressBySignal(NodeSignalIn signalIn, out Byte? numericAddress)
        {
            var signalId = Guid.Parse(signalIn.SignalId.ToString());
            if (this.m_numericAddressBySignal.ContainsKey(signalId))
            {
                numericAddress = this.m_numericAddressBySignal[signalId];
                return true;
            }
            numericAddress = null;
            return false;
        }

        public void BuildSignalTable(NodeRuntimeApplication runtimeApplication)
        {
            // initialize signal table to empty
            for (int i = 0; i < this.m_information.Booleans; i++)
            {
                this.m_booleanSignalsByAddress[i] = null;
            }
            for (int i = 0; i < this.m_information.Numerics; i++)
            {
                this.m_numericSignalsByAddress[i] = null;
            }
            this.m_booleanAddressBySignal.Clear();
            this.m_numericAddressBySignal.Clear();
            this.m_booleanJumpers.Clear();
            this.m_numericJumpers.Clear();

            // build the table
            Int16 maxBooleanAddress = 0;
            Byte maxNumericAddress = 0;
            foreach (var keyValuePair in runtimeApplication.DeviceConfiguration.GetChildrenRecursive())
            {
                if (keyValuePair.Value is NodeDiscreteInput)
                {
                    var nodeDiscreteInput = keyValuePair.Value as NodeDiscreteInput;
                    var address = Int16.Parse(nodeDiscreteInput.Address.ToString());
                    this.m_booleanSignalsByAddress[address] = nodeDiscreteInput.Signal;
                    var signalId = Guid.Parse(nodeDiscreteInput.Signal.SignalId.ToString());
                    this.m_booleanAddressBySignal.Add(signalId, address);
                    if (address > maxBooleanAddress) maxBooleanAddress = address;
                }
                else if (keyValuePair.Value is NodeDiscreteOutput)
                {
                    var nodeDiscreteOutput = keyValuePair.Value as NodeDiscreteOutput;
                    var address = Int16.Parse(nodeDiscreteOutput.Address.ToString());
                    if (address > maxBooleanAddress) maxBooleanAddress = address;
                }
                else if (keyValuePair.Value is NodeAnalogInput)
                {
                    var nodeAnalogInput = keyValuePair.Value as NodeAnalogInput;
                    var address = Byte.Parse(nodeAnalogInput.Address.ToString());
                    this.m_numericSignalsByAddress[address] = nodeAnalogInput.Signal;
                    var signalIdGuid = Guid.Parse(nodeAnalogInput.Signal.SignalId.ToString());
                    this.m_numericAddressBySignal.Add(signalIdGuid, address);
                    if (address > maxNumericAddress) maxNumericAddress = address;
                }
                else if (keyValuePair.Value is NodeAnalogOutput)
                {
                    var nodeAnalogOutput = keyValuePair.Value as NodeAnalogOutput;
                    var address = Byte.Parse(nodeAnalogOutput.Address.ToString());
                    if (address > maxNumericAddress) maxNumericAddress = address;
                }
            }

            // now add the other signals
            foreach (var signal in runtimeApplication.Signals)
            {
                var signalId = Guid.Parse(signal.SignalId.ToString());
                if (!this.m_booleanAddressBySignal.ContainsKey(signalId)
                    && !this.m_numericAddressBySignal.ContainsKey(signalId))
                {
                    switch (signal.DataType.DataType)
                    {
                        case FieldDataType.DataTypeEnum.BOOL:
                            Int16 booleanAddress = (Int16)(maxBooleanAddress + 1);
                            maxBooleanAddress = booleanAddress;
                            this.m_booleanAddressBySignal.Add(signalId, booleanAddress);
                            this.m_booleanSignalsByAddress[booleanAddress] = signal;
                            break;
                        case FieldDataType.DataTypeEnum.NUMBER:
                            Byte numericAddress = (Byte)(maxNumericAddress + 1);
                            maxNumericAddress = numericAddress;
                            this.m_numericAddressBySignal.Add(signalId, numericAddress);
                            this.m_numericSignalsByAddress[numericAddress] = signal;
                            break;
                        default:
                            throw new Exception("Arduino runtime only supports BOOL and NUMBER datatypes.");
                    }
                }
            }

            // now put in the jumpers between internal signals and outputs
            foreach (var keyValuePair in runtimeApplication.DeviceConfiguration.GetChildrenRecursive())
            {
                if (keyValuePair.Value is NodeDiscreteOutput)
                {
                    var nodeDiscreteOutput = keyValuePair.Value as NodeDiscreteOutput;
                    var address = Int16.Parse(nodeDiscreteOutput.Address.ToString());
                    var signalId = nodeDiscreteOutput.SignalIn.SignalId;
                    if (signalId != null)
                    {
                        var signal = runtimeApplication.FindSignal(signalId);
                        this.m_booleanSignalsByAddress[address] = signal;
                        var signalIdGuid = Guid.Parse(signal.SignalId.ToString());
                        this.m_booleanJumpers.Add(
                            new Tuple<Int16, Int16>(
                                this.m_booleanAddressBySignal[signalIdGuid], address));
                    }
                }
                else if (keyValuePair.Value is NodeAnalogOutput)
                {
                    var nodeAnalogOutput = keyValuePair.Value as NodeAnalogOutput;
                    var address = Byte.Parse(nodeAnalogOutput.Address.ToString());
                    var signalId = nodeAnalogOutput.SignalIn.SignalId;
                    if (signalId != null)
                    {
                        var signal = runtimeApplication.FindSignal(signalId);
                        this.m_numericSignalsByAddress[address] = signal;
                        var signalIdGuid = Guid.Parse(signal.SignalId.ToString());
                        this.m_numericJumpers.Add(
                            new Tuple<byte, byte>(
                                this.m_numericAddressBySignal[signalIdGuid], address));
                    }
                }
            }
        }
    }
}
