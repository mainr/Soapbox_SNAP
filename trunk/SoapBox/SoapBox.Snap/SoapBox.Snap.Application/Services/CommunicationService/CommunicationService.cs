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
using System.ComponentModel.Composition;
using SoapBox.Protocol.Base;

namespace SoapBox.Snap.Application
{
    [Export(Services.CommunicationManager.CommunicationService, typeof(ICommunicationService))]
    public class CommunicationService : ICommunicationService
    {
        public CommunicationService()
        {
            m_peer = SoapBox.Protocol.Base.CommunicationManager.RegisterLocalPeer(
                new FieldIdentifier("P_" +
                Guid.NewGuid().ToString().Replace("-",string.Empty)));
            peer.OnReceive += new NodePeer.ReceiveHandler(peer_OnReceive);
            peer.OnReceiveDelta += new NodePeer.ReceiveDeltaHandler(peer_OnReceiveDelta);
        }

        void peer_OnReceive(NodePeer FromPeer, NodeBase Message)
        {
            IRuntime runtime = getRelationship(FromPeer);
            if (runtime != null)
            {
                runtime.MessageReceivedFromPeer(Message);
            }
        }

        NodeBase peer_OnReceiveDelta(NodePeer FromPeer, FieldGuid BasedOnMessageID)
        {
            NodeBase originalMessage = null;
            IRuntime runtime = getRelationship(FromPeer);
            if (runtime != null)
            {
                originalMessage = runtime.DeltaReceivedFromPeer(BasedOnMessageID);
            }
            return originalMessage;
        }

        private NodePeer peer
        {
            get
            {
                return m_peer;
            }
        }
        private readonly NodePeer m_peer = null;

        #region Threadsafe NodePeer -> IRuntime Dictionary

        private void setRelationship(NodePeer peer, IRuntime runtime)
        {
            lock (m_runtimeLookup_Lock)
            {
                if(!m_runtimeLookup.ContainsKey(peer))
                {
                    m_runtimeLookup.Add(peer, runtime);
                }
            }
        }
        private IRuntime getRelationship(NodePeer peer)
        {
            IRuntime runtime = null;
            lock (m_runtimeLookup_Lock)
            {
                if (m_runtimeLookup.ContainsKey(peer))
                {
                    runtime = m_runtimeLookup[peer];
                }
            }
            return runtime;
        }

        private readonly Dictionary<NodePeer, IRuntime> m_runtimeLookup = new Dictionary<NodePeer, IRuntime>();
        private readonly object m_runtimeLookup_Lock = new object();

        #endregion

        #region ICommunicationService Members

        public void SendToPeer(IRuntime fromRuntime, NodePeer toPeer, NodeBase message)
        {
            setRelationship(toPeer, fromRuntime);
            peer.SendToPeer(toPeer, message);
        }

        public void SendDeltaToPeer(IRuntime fromRuntime, NodePeer toPeer, NodeBase message, NodeBase basedOnMessage)
        {
            setRelationship(toPeer, fromRuntime);
            peer.SendDeltaToPeer(toPeer, message, basedOnMessage);
        }
        #endregion
    }
}
