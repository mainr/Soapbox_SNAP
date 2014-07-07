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
using SoapBox.Protocol.Automation;
using SoapBox.Protocol.Base;
using System.Threading;

namespace SoapBox.Snap.Runtime
{
    public class Runtime : IRuntime
    {
        private const int COMM_TIMEOUT = 1000;

        // Pre-build the telegram messages for communicating with the Engine
        private readonly NodeTelegram m_TelegramReadConfiguration = NodeTelegram.BuildWith(
            new FieldString(TelegramType.READCONFIGURATION.ToString()));
        private readonly NodeTelegram m_TelegramStart = NodeTelegram.BuildWith(
            new FieldString(TelegramType.START.ToString()));
        private readonly NodeTelegram m_TelegramStop = NodeTelegram.BuildWith(
            new FieldString(TelegramType.STOP.ToString()));
        private readonly NodeTelegram m_TelegramRunning = NodeTelegram.BuildWith(
            new FieldString(TelegramType.RUNNING.ToString()));
        private readonly NodeTelegram m_TelegramUpload = NodeTelegram.BuildWith(
            new FieldString(TelegramType.UPLOAD.ToString()));
        private readonly NodeTelegram m_TelegramRuntimeId = NodeTelegram.BuildWith(
            new FieldString(TelegramType.RUNTIMEID.ToString()));
        private readonly NodeTelegram m_TelegramRuntimeVersionId = NodeTelegram.BuildWith(
            new FieldString(TelegramType.RUNTIMEVERSIONID.ToString()));
        private readonly NodeTelegram m_TelegramReadSignals = NodeTelegram.BuildWith(
            new FieldString(TelegramType.READSIGNALS.ToString()));

        public Runtime(Lazy<ICommunicationService> communicationService, 
            Lazy<IRuntimeService> runtimeService,
            RuntimeType runtimeType, NodePeer enginePeer)
        {
            if (communicationService == null)
            {
                throw new ArgumentNullException();
            }
            if (runtimeService == null)
            {
                throw new ArgumentNullException();
            }
            if (runtimeType == null)
            {
                throw new ArgumentNullException();
            }
            if (enginePeer == null)
            {
                throw new ArgumentNullException();
            }
            m_Type = runtimeType;
            m_Peer = enginePeer;
            m_communicationService = communicationService;
            m_runtimeService = runtimeService;
        }

        private readonly Lazy<ICommunicationService> m_communicationService = null;
        private readonly Lazy<IRuntimeService> m_runtimeService = null;

        #region IRuntime Members

        public IRuntimeType Type
        {
            get
            {
                return m_Type;
            }
        }
        private readonly RuntimeType m_Type = null;

        public bool Start()
        {
            var response = sendReceiveTelegram<NodeTelegram>(m_TelegramStart);
            if (response != null && response.Payload.Decode() == true.ToString())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Stop()
        {
            var response = sendReceiveTelegram<NodeTelegram>(m_TelegramStop);
            if (response != null && response.Payload.Decode() == true.ToString())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Running
        {
            get
            {
                var response = sendReceiveTelegram<NodeTelegram>(m_TelegramRunning);
                if (response != null && response.Payload.Decode() == true.ToString())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public void RuntimeApplicationGoOnline(NodeRuntimeApplication runtimeApplication)
        {
            if (runtimeApplication == null)
            {
                throw new ArgumentNullException("runtimeApplication");
            }
        }

        public bool RuntimeApplicationDownload(NodeRuntimeApplication runtimeApplication, bool onlineChange)
        {
            if (runtimeApplication == null)
            {
                throw new ArgumentNullException("runtimeApplication");
            }
            m_communicationService.Value.SendDeltaToPeer(this, Peer, runtimeApplication, null);
            return true;
        }

        public NodeRuntimeApplication RuntimeApplicationUpload()
        {
            return sendReceiveTelegram<NodeRuntimeApplication>(m_TelegramUpload);
        }

        public NodeDeviceConfiguration ReadConfiguration()
        {
            return sendReceiveTelegram<NodeDeviceConfiguration>(m_TelegramReadConfiguration);
        }

        public FieldGuid RuntimeId()
        {
            return sendTelegramWithGuidPayloadResponse(m_TelegramRuntimeId);
        }

        public FieldGuid RuntimeVersionId()
        {
            return sendTelegramWithGuidPayloadResponse(m_TelegramRuntimeVersionId);
        }

        private FieldGuid sendTelegramWithGuidPayloadResponse(NodeTelegram telegram)
        {
            var response = sendReceiveTelegram<NodeTelegram>(telegram);
            if (response != null)
            {
                string payload = response.Payload.Decode();
                if (FieldGuid.CheckSyntax(payload))
                {
                    return new FieldGuid(payload);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        #region " sendReceiveTelegram "
        /// <summary>
        /// Generic way to send a telegram to the engine, and get a response of a given type
        /// If the receive type if NodeTelegram, it checks the MessageId to make sure the 
        /// response matches the outgoing MessageId.
        /// </summary>
        /// <returns>null if no response</returns>
        public T sendReceiveTelegram<T>(NodeTelegram outgoingTelegram) where T : NodeBase
        {
            responseNode = null;
            m_sendReceiveTelegram_ARE.Reset();
            var msgId = new FieldGuid(); // generates a unique message ID
            m_communicationService.Value.SendToPeer(this, Peer, outgoingTelegram.SetMessageId(msgId));
            if (m_sendReceiveTelegram_ARE.WaitOne(COMM_TIMEOUT, false))
            {
                var telegramRead = responseNode as NodeTelegram;
                if (telegramRead != null && telegramRead.MessageId != msgId)
                {
                    return null;
                }
                return responseNode as T;
            }
            else
            {
                // timeout
                return null;
            }
        }
        private readonly AutoResetEvent m_sendReceiveTelegram_ARE = new AutoResetEvent(false);
        private NodeBase responseNode // threadsafe place to store result
        {
            get
            {
                NodeBase n = null;
                lock (m_telegramRead_Lock)
                {
                    n = m_telegramRead;
                }
                return n;
            }
            set
            {
                lock (m_telegramRead_Lock)
                {
                    m_telegramRead = value;
                }
            }
        }
        private NodeBase m_telegramRead = null;
        private readonly object m_telegramRead_Lock = new object();
        #endregion

        public void MessageReceivedFromPeer(NodeBase message)
        {
            responseNode = message;
            m_sendReceiveTelegram_ARE.Set();
        }

        public NodeBase DeltaReceivedFromPeer(FieldGuid basedOnMessageID)
        {
            // shouldn't receive any deltas at this time
            throw new NotImplementedException();
        }

        // this is the Runtime Engine Peer
        public NodePeer Peer
        {
            get
            {
                return m_Peer;
            }
        }
        private readonly NodePeer m_Peer = null;

        public void ReadSignalValues(IEnumerable<NodeSignal> signals)
        {
            if (Properties.Settings.Default.RunAsService)
            {
                var outPayload = new StringBuilder();
                foreach (var signal in signals)
                {
                    outPayload.Append(signal.SignalId.ToString());
                }
                var response = sendReceiveTelegram<NodeTelegram>(m_TelegramReadSignals.SetPayload(FieldBase64.Encode(outPayload.ToString())));
                if (response != null)
                {
                    foreach (var tpl in EncodedSignalValue.ParseEncodedSignals(response.Payload.Decode(), signals))
                    {
                        var signal = tpl.Item1;
                        var value = tpl.Item2;
                        m_runtimeService.Value.NotifyValueChanged(signal, value);
                    }
                }
            }
            else
            {
                // uploading is instant
                var nRuntimeApplication = RuntimeApplicationUpload();
                if (nRuntimeApplication != null)
                {
                    foreach (var sig in signals)
                    {
                        var foundSig = nRuntimeApplication.FindSignal(sig.SignalId);
                        if (foundSig != null)
                        {
                            m_runtimeService.Value.NotifyValueChanged(foundSig, foundSig.Value);
                        }
                    }
                }
            }
        }

        public void Disconnect()
        {
        }
        #endregion

    }
}
