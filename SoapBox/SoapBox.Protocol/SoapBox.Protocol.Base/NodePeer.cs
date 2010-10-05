#region "SoapBox.Protocol License"
/// <header module="SoapBox.Protocol"> 
/// Copyright (C) 2010 SoapBox Automation Inc., All Rights Reserved.
/// Contact: SoapBox Automation Licencing (license@soapboxautomation.com)
/// 
/// This file is part of SoapBox Protocol.
///
/// SoapBox Protocol is available under your choice of these licenses:
///  - GPLv3
///  - CDDLv1.0
///
/// GNU General Public License Usage
/// SoapBox Protocol is free software: you can redistribute it and/or modify it
/// under the terms of the GNU General Public License as published by the 
/// Free Software Foundation, either version 3 of the License, or 
/// (at your option) any later version.
/// 
/// SoapBox Protocol is distributed in the hope that it will be useful, but 
/// WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
/// GNU General Public License for more details.
/// 
/// You should have received a copy of the GNU General Public License along
/// with SoapBox Protocol. If not, see <http://www.gnu.org/licenses/>.
/// 
/// Common Development and Distribution License Usage
/// SoapBox Protocol is subject to the CDDL Version 1.0. 
/// You should have received a copy of the CDDL Version 1.0 along
/// with SoapBox Protocol.  If not, see <http://www.sun.com/cddl/cddl.html>.
/// The CDDL is a royalty free, open source, file based license.
/// </header>
#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using SoapBox.Protocol.Base;
using SoapBox.Utilities;

namespace SoapBox.Protocol.Base
{
    /// <summary>
    /// Implements a Peer object.  
    /// </summary>
    public sealed class NodePeer : NodeBase
    {

        #region " IsLocal (bool) "
        private readonly bool m_IsLocal = false;
        public bool IsLocal
        {
            get
            {
                return m_IsLocal;
            }
        }
        #endregion

        #region " Connection (NodeConnection) "

        private readonly object m_Connection_Lock = new object();
        private NodeConnection m_Connection = null;
        /// <summary>
        /// This is mutable and not part of the fields that are
        /// passed from one side of a connection to the other. It
        /// is strictly for keeping a reference to which connection
        /// object we need to use to reach this peer.
        /// </summary>
        internal NodeConnection Connection
        {
            get
            {
                lock (m_Connection_Lock)
                {
                    return m_Connection;
                }
            }
            set
            {
                lock (m_Connection_Lock)
                {
                    m_Connection = value;
                }
            }
        }

        #endregion

        #region " FIELDS "

        #region " Code (FieldIdentifier) "
        public FieldIdentifier Code
        {
            get
            {
                return (FieldIdentifier)Fields[new FieldIdentifier(m_CodeName)];
            }
        }
        static readonly string m_CodeName =
            NotifyPropertyChangedHelper.GetPropertyName<NodePeer>(o => o.Code);
        #endregion

        #endregion

        #region " CONSTRUCTORS "
        private NodePeer(ReadOnlyDictionary<FieldIdentifier, FieldBase> Fields,
            ReadOnlyCollection<NodeBase> Children, bool local)
            : base(Fields, Children)
        {
            m_IsLocal = local;

            //validation
            if (Code == null)
            {
                throw new ArgumentNullException(m_CodeName);
            }
        }

        protected override NodeBase CopyWithNewChildren(ReadOnlyCollection<NodeBase> NewChildren)
        {
            return new NodePeer(Fields, NewChildren, IsLocal);
        }
        #endregion

        #region " BUILDER(S) "

        private static NodeBase Resurrect(
            ReadOnlyDictionary<FieldIdentifier, FieldBase> Fields,
            ReadOnlyCollection<NodeBase> Children)
        {
            return new NodePeer(Fields, Children, 
                false /* peers from remote locations are resurrected */);
        }

        internal static NodePeer BuildWith(FieldIdentifier Code)
        {
            //build fields
            Dictionary<FieldIdentifier, FieldBase> mutableFields =
                new Dictionary<FieldIdentifier, FieldBase>();
            mutableFields.Add(new FieldIdentifier(m_CodeName), Code);

            //build children
            KeyedNodeCollection<NodeBase> mutableChildren =
                new KeyedNodeCollection<NodeBase>();

            //build node
            NodePeer Builder = new NodePeer(
                new ReadOnlyDictionary<FieldIdentifier, FieldBase>(mutableFields),
                new ReadOnlyCollection<NodeBase>(mutableChildren), 
                true /* peers built by the Communication manager are local */);

            return Builder;
        }

        #endregion

        #region " COMMUNICATION LOGIC "
        public delegate void ReceiveHandler(NodePeer FromPeer, NodeBase Message);
        public delegate NodeBase ReceiveDeltaHandler(NodePeer FromPeer, FieldGuid BasedOnMessageID);
        //adding/removing delegates to events is thread safe
        public event ReceiveHandler OnReceive = delegate { };
        public event ReceiveDeltaHandler OnReceiveDelta = delegate { return null; };

        internal void ReceiveFromPeer(NodePeer FromPeer, NodeBase Message)
        {
            if (CommunicationManager.LocalPeerList.Peers.Contains(this))
            {
                OnReceive(FromPeer, Message);
            }
        }

        internal NodeBase ReceiveDeltaFromPeer(NodePeer FromPeer, FieldGuid BasedOnMessageID)
        {
            if (CommunicationManager.LocalPeerList.Peers.Contains(this))
            {
                return OnReceiveDelta(FromPeer, BasedOnMessageID);
            }
            else
            {
                return null;
            }
        }

        public void SendToPeer(NodePeer ToPeer, NodeBase Message)
        {
            CommunicationManager.SendToPeer(ToPeer, this, Message);
        }

        public void SendDeltaToPeer(NodePeer ToPeer, NodeBase Message, NodeBase BasedOnMessage)
        {
            CommunicationManager.SendDeltaToPeer(ToPeer, this, Message, BasedOnMessage);
        }

        //Called when the peer is being unregistered.
        //Clears event handlers.
        internal void Unregister()
        {
            OnReceive = delegate { };
            OnReceiveDelta = delegate { return null; };
        }
        #endregion
    }
}
