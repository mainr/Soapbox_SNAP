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
using SoapBox.Snap.ArduinoRuntime.Protocol.Helpers;
using System.IO.Ports;
using System.Threading;
using System.Collections.Concurrent;
using System.IO;

namespace SoapBox.Snap.ArduinoRuntime.Protocol
{
    class ArduinoRuntimeProtocol : IDisposable
    {
        const string TERMINATION_LINE = "EOM";

        private readonly object m_logItemsLock = new object();
        private readonly List<LogItem> m_logItems = new List<LogItem>();

        public ArduinoRuntimeProtocol(string comPort)
        {
            if (comPort == null) throw new ArgumentNullException("comPort");
            this.ComPort = comPort.ToUpper();
            var portName = SerialPort.GetPortNames()
                    .SingleOrDefault(x => x.ToUpper() == this.ComPort);
            if (portName == null)
            {
                throw new ProtocolException("COM port " + this.ComPort + " doesn't exist.");
            }
            lock (this.m_sendAndReceive_Lock)
            {
                this.m_serialPort = new SerialPort(portName, 115200);
                this.m_serialPort.DtrEnable = false; // disable or it resets Arduino
                this.m_serialPort.Encoding = Encoding.GetEncoding(28591); // allows use of 8-bit ascii
                this.m_serialPort.NewLine = "\r\n"; // matches Arduino default
            }
        }

        public string ComPort { get; private set; }

        public void Connect()
        {
            lock (this.m_sendAndReceive_Lock)
            {
                if (!this.m_serialPort.IsOpen)
                {
                    this.m_serialPort.Open();
                }
            }
        }

        public void Disconnect()
        {
            lock (this.m_sendAndReceive_Lock)
            {
                if (this.m_serialPort.IsOpen)
                {
                    this.m_serialPort.Close();
                }
            }
        }

        public bool Connected
        {
            get 
            {
                lock (this.m_sendAndReceive_Lock)
                {
                    return this.m_serialPort.IsOpen;
                }
            }
        }

        public InformationResponse GetInformation()
        {
            var response = sendAndReceive("information");
            return new InformationResponse(response);
        }

        public ReadNumericResponse ReadNumeric(Byte address)
        {
            var response = sendAndReceive("read n" + address);
            return new ReadNumericResponse(response);
        }

        public ReadBooleanResponse ReadBoolean(Int16 address)
        {
            var response = sendAndReceive("read b" + address);
            return new ReadBooleanResponse(response);
        }

        public CommandResponse Start()
        {
            var response = sendAndReceive("start");
            return new CommandResponse(response);
        }

        public CommandResponse Stop()
        {
            var response = sendAndReceive("stop");
            return new CommandResponse(response);
        }

        public RuntimeStatusResponse GetRuntimeStatus()
        {
            var response = sendAndReceive("status");
            return new RuntimeStatusResponse(response);
        }

        public CommandResponse ResetConfiguration()
        {
            var response = sendAndReceive("config-reset");
            return new CommandResponse(response);
        }

        public CommandResponse Configure(string command)
        {
            var response = sendAndReceive(command);
            return new CommandResponse(response);
        }

        public DeviceConfigurationResponse GetDeviceConfiguration()
        {
            var response = sendAndReceive("device-config");
            return new DeviceConfigurationResponse(response);
        }

        public DownloadOrPatchResponse Download(byte[] bytes)
        {
            return downloadOrPatch(bytes, "download");
        }

        public DownloadOrPatchResponse Patch(byte[] bytes)
        {
            return downloadOrPatch(bytes, "patch");
        }

        private DownloadOrPatchResponse downloadOrPatch(byte[] bytes, string command)
        {
            if (command == null) throw new ArgumentNullException("command");
            lock (this.m_sendAndReceive_Lock) // don't allow other communication to interrupt
            {
                var response1 = sendAndReceive(command + " " + bytes.Length);
                var commandResponse1 = new DownloadOrPatchResponse(response1);
                if (commandResponse1.Success && commandResponse1.Bytes == bytes.Length)
                {
                    // it's now ready to accept a binary program (expecting the length we gave it above)
                    var response2 = sendAndReceive(bytes);
                    var commandResponse2 = new DownloadOrPatchResponse(response2);
                    if (commandResponse2.Success && commandResponse2.Bytes == bytes.Length)
                    {
                        return commandResponse2;
                    }
                    else if (!commandResponse2.Success)
                    {
                        throw new ProtocolException("Download/patch failed.");
                    }
                    else
                    {
                        throw new ProtocolException("Tried to download/patch " + bytes.Length
                            + " bytes but runtime responded that " + commandResponse2.Bytes
                            + " bytes were downloaded/patched.");
                    }
                }
                else if (!commandResponse1.Success)
                {
                    throw new ProtocolException("Download/patch failed.");
                }
                else
                {
                    throw new ProtocolException("Tried to download/patch " + bytes.Length
                        + " bytes but runtime responded that " + commandResponse1.Bytes
                        + " were expected to be downloaded/patched.");
                }
            }
        }

        public RuntimeVersionIdResponse VersionId()
        {
            var response = sendAndReceive("version-id");
            return new RuntimeVersionIdResponse(response);
        }

        public RuntimeIdResponse RuntimeId()
        {
            var response = sendAndReceive("runtime-id");
            return new RuntimeIdResponse(response);
        }

        // have to protect communications over the serial port, so there's only one going on at a time
        private readonly object m_sendAndReceive_Lock = new object();
        private SerialPort m_serialPort;

        private SendReceiveResult sendAndReceive(string request)
        {
            SendReceiveResult result;
            try
            {
                result = this.sendAndReceive(serialPort => serialPort.WriteLine(request));
                Console.WriteLine("sent: " + request);
                if (result.Success)
                {
                    foreach (var line in result.Lines)
                    {
                        Console.WriteLine(line);
                    }
                }
                else
                {
                    Console.WriteLine(result.Error);
                }
            }
            catch (IOException ex)
            {
                result = new SendReceiveResult(
                    success: false,
                    error: ex.Message,
                    lines: new List<string>());
            }
            catch (TimeoutException ex)
            {
                result = new SendReceiveResult(
                    success: false,
                    error: ex.Message,
                    lines: new List<string>());
            }
            catch (UnauthorizedAccessException ex) // can happen when opening port
            {
                result = new SendReceiveResult(
                    success: false,
                    error: ex.Message,
                    lines: new List<string>());
            }
            catch (InvalidOperationException ex) // can happen when serial port is closed
            {
                result = new SendReceiveResult(
                    success: false,
                    error: ex.Message,
                    lines: new List<string>());
            }
            // for debugging:
            //var logItem = new LogItem(request, result);
            //lock (this.m_logItemsLock)
            //{
            //    this.m_logItems.Add(logItem);
            //}
            return result;
        }

        private SendReceiveResult sendAndReceive(byte[] request)
        {
            const int MAX_BYTES = 64;
            var result = this.sendAndReceive(serialPort =>
                {
                    var sent = 0;
                    while (sent < request.Length)
                    {
                        if (sent > 0)
                        {
                            Thread.Sleep(100);
                        }
                        var bytesToSend = request.Length - sent;
                        if (bytesToSend > MAX_BYTES)
                        {
                            bytesToSend = MAX_BYTES;
                        }
                        serialPort.Write(request, sent, bytesToSend);
                        sent += bytesToSend;
                    }
                });
            var logItem = new LogItem(request, result);
            lock (this.m_logItemsLock)
            {
                this.m_logItems.Add(logItem);
            }
            return result;
        }

        private SendReceiveResult sendAndReceive(Action<SerialPort> sendAction)
        {
            lock (this.m_sendAndReceive_Lock)
            {
                if (!this.Connected)
                {
                    throw new ProtocolException("Not connected to runtime.");
                }
                var result = new List<string>();
                sendAction(this.m_serialPort);
                while (true)
                {
                    this.m_serialPort.ReadTimeout = 1000;
                    var line = this.m_serialPort.ReadLine();
                    if (line == TERMINATION_LINE)
                    {
                        return new SendReceiveResult(true, string.Empty, result);
                    }
                    result.Add(line);
                }
            }
        }

        public void Dispose()
        {
            lock (this.m_sendAndReceive_Lock)
            {
                if (this.m_serialPort != null)
                {
                    this.m_serialPort.Dispose();
                }
            }
        }

        class LogItem
        {
            private readonly string send = null;
            private readonly byte[] bytes = null;
            private readonly SendReceiveResult result;

            public LogItem(
                string send,
                SendReceiveResult result)
            {
                this.send = send;
                this.result = result;
                Console.WriteLine("Sent: {0}, received success: {1}, lines: {2}",
                    send, result.Success, result.Lines.Count);
            }

            public LogItem(
                byte[] bytes,
                SendReceiveResult result)
            {
                this.bytes = bytes;
                this.result = result;
                Console.WriteLine("Sent: {0} bytes, received success: {1}, lines: {2}",
                    bytes.Length, result.Success, result.Lines.Count);
            }
        }
    }
}
