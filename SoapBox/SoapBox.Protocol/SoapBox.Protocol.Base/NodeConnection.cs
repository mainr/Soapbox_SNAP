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
using System.Net.Security;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using SoapBox.Utilities;
using System.Diagnostics;

namespace SoapBox.Protocol.Base
{
    /// <summary>
    /// Implements a connection object. (SSL) 
    /// </summary>
    public sealed class NodeConnection 
        : NodeBase
    {
        #region " FIELDS "

        #region " Code (FieldIdentifier) "
        public FieldIdentifier Code
        {
            get
            {
                return (FieldIdentifier)Fields[new FieldIdentifier(m_CodeName)];
            }
        }
        public NodeConnection SetCode(FieldIdentifier Code)
        {
            if (Code == null)
            {
                throw new ArgumentNullException(m_CodeName);
            }
            return new NodeConnection(this.SetField(new FieldIdentifier(m_CodeName), Code), ChildCollection);
        }
        static readonly string m_CodeName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeConnection>(o => o.Code);
        #endregion
       
        #endregion

        #region " CONSTRUCTORS "
        private NodeConnection(ReadOnlyDictionary<FieldIdentifier, FieldBase> Fields,
            ReadOnlyCollection<NodeBase> Children)
            : base(Fields, Children)
        {
            //validation
            if (Code == null)
            {
                throw new ArgumentNullException(m_CodeName);
            }
        }

        protected override NodeBase CopyWithNewChildren(ReadOnlyCollection<NodeBase> NewChildren)
        {
            return new NodeConnection(Fields, NewChildren);
        }
        #endregion

        #region " BUILDER(S) "

        private static NodeBase Resurrect(
            ReadOnlyDictionary<FieldIdentifier, FieldBase> Fields,
            ReadOnlyCollection<NodeBase> Children)
        {
            return new NodeConnection(Fields, Children);
        }

        public static NodeConnection BuildWith(FieldIdentifier Code)
        {
            //build fields
            Dictionary<FieldIdentifier, FieldBase> mutableFields =
                new Dictionary<FieldIdentifier, FieldBase>();
            mutableFields.Add(new FieldIdentifier(m_CodeName), Code);

            //build children
            KeyedNodeCollection<NodeBase> mutableChildren =
                new KeyedNodeCollection<NodeBase>();

            //build node
            NodeConnection Builder = new NodeConnection(
                new ReadOnlyDictionary<FieldIdentifier, FieldBase>(mutableFields),
                new ReadOnlyCollection<NodeBase>(mutableChildren));

            return Builder;
        }

        #endregion

        #region " RemotePeerList "

        //not inherently thread safe, need to lock
        private readonly object m_remotePeerList_Lock = new object();
        private NodePeerList m_remotePeerList = NodePeerList.BuildWith();

        /// <summary>
        /// Returns the current list of remote Peers
        /// </summary>
        public NodePeerList RemotePeerList
        {
            get
            {
                lock (m_remotePeerList_Lock)
                {
                    return m_remotePeerList;
                }
            }
            private set
            {
                lock (m_remotePeerList_Lock)
                {
                    m_remotePeerList = value;
                    foreach (NodePeer p in m_remotePeerList.Peers)
                    {
                        p.Connection = this;
                    }
                    OnRemotePeerListChanged(m_remotePeerList);
                }
            }
        }

        public delegate void RemotePeerListChangedHandler(
            NodePeerList newRemotePeerList);
        //adding/removing delegates to events is thread safe
        public event RemotePeerListChangedHandler OnRemotePeerListChanged = delegate { };

        /// <summary>
        /// Registers a RemotePeerListChangedHandler and returns the existing
        /// remote peer list in one operation, so you are guaranteed not
        /// to miss any changes to the list.  Be careful - the event could
        /// actually fire before you do something with the returned value.
        /// </summary>
        /// <param name="newHandler"></param>
        /// <returns></returns>
        public NodePeerList RegisterRemotePeerListChangedCallback(RemotePeerListChangedHandler newHandler)
        {
            lock (m_remotePeerList_Lock)
            {
                OnRemotePeerListChanged += newHandler;
                return RemotePeerList;
            }
        }

        #endregion

        #region " SslStream "

        internal void Send(NodePeer ToPeer, NodePeer FromPeer, NodeBase Message)
        {
            Send(ToPeer, FromPeer, Message, null);
        }

        internal void Send(NodePeer ToPeer, NodePeer FromPeer, NodeBase Message, NodeBase BasedOnMessage)
        {
            if (ToPeer == null)
            {
                throw new ArgumentNullException("ToPeer");
            }
            if (FromPeer == null)
            {
                throw new ArgumentNullException("FromPeer");
            }
            if (Message == null)
            {
                throw new ArgumentNullException("Message");
            }

            NodePeer to = RemotePeerList.FindPeerByID(ToPeer.ID);
            NodePeer from = CommunicationManager.LocalPeerList.FindPeerByID(FromPeer.ID);
            if (to != null && from != null)
            {
                string s = Message.ToXml(BasedOnMessage, to, from);
                Send(s);
            }
            else
            {
                //TODO - log undeliverable message (peer lists changed?)
            }
        }

        private void Send(string message)
        {

            //TextWriter tw = new StreamWriter("send_" + this.ID.ToString() + ".txt", true);
            //tw.WriteLine(message);
            //tw.Close();

            lock (m_OutgoingQueue_Lock)
            {
                m_OutgoingQueue.Enqueue(message);
            }
        }

        /// <summary>
        /// When the local peer list changes, we have to send updates to
        /// the other end of this connection.
        /// </summary>
        /// <param name="newPeerList"></param>
        /// <param name="oldPeerList"></param>
        private void LocalPeerListChangedCallback(NodePeerList newPeerList,
            NodePeerList oldPeerList)
        {
            string s = newPeerList.ToXml(oldPeerList);
            Send(s);
        }

        private void Receive(string message)
        {
            //TextWriter tw = new StreamWriter("receive_" + this.ID.ToString() + ".txt", true);
            //tw.WriteLine(message);
            //tw.Close();

            if (ValidateXmlToSchema(message))
            {
                FieldGuid toPeerID = NodeBase.ToPeerFromXML(message);
                FieldGuid fromPeerID = NodeBase.FromPeerFromXML(message);
                if (toPeerID != null && fromPeerID != null)
                {
                    //it's a peer-to-peer message

                    //find the ToPeer in the local peer list
                    NodePeer ToPeer = CommunicationManager.LocalPeerList.FindPeerByID(toPeerID);

                    //find the FromPeer in the remote peer list
                    NodePeer FromPeer = RemotePeerList.FindPeerByID(fromPeerID);

                    if (ToPeer != null && FromPeer != null)
                    {
                        //see if this message is based on another node
                        FieldGuid BasedOnNodeID = NodeBase.BasedOnNodeIDFromXML(message);
                        NodeBase basedOnNode = null;
                        if (BasedOnNodeID != null)
                        {
                            basedOnNode = ToPeer.ReceiveDeltaFromPeer(FromPeer, BasedOnNodeID);
                            NodeBase msg = NodeBase.NodeFromXML(message, basedOnNode.GetChildrenRecursive());
                            CommunicationManager.SendDeltaToPeer(ToPeer, FromPeer, msg, basedOnNode);
                        }
                        else
                        {
                            NodeBase msg = NodeBase.NodeFromXML(message, null);
                            CommunicationManager.SendToPeer(ToPeer, FromPeer, msg);
                        }
                    }
                    else
                    {
                        //TODO - log this - undeliverable message (peer lists not up to date?)
                    }
                }
                else
                {
                    //it's a system message (not peer-to-peer)
                    FieldNodeType nodeType = NodeBase.NodeTypeFromXML(message);
                    if (nodeType.ToString() == typeof(NodePeerList).FullName)
                    {
                        lock (m_remotePeerList_Lock) //lock so we read/write in one operation
                        {
                            //When we receive a remote peer list, it is generally just a diff
                            //from the last peer list.
                            RemotePeerList = (NodePeerList)NodeBase.NodeFromXML(
                                message, RemotePeerList.GetChildrenRecursive());
                        }
                    }
                    else
                    {
                        //TODO - log this?  Unknown root message type?
                    }
                }
            }
            else
            {
                //TODO - log this?  Unknown garbage message?
            }
        }

        //not thread safe, so we need to lock
        private readonly object m_OutgoingQueue_Lock = new object();
        private readonly Queue<String> m_OutgoingQueue = new Queue<String>();
        private readonly object m_IncomingQueue_Lock = new object();
        private readonly Queue<String> m_IncomingQueue = new Queue<string>();

        internal void ThreadStart(Object o)
        {
            SslStream sslStream = o as SslStream;

            if (sslStream != null)
            {

                //Have to send the local peer list to the other end of
                //the connection, and we also need to be notified of any
                //changes to the local peer list.  This has to be done
                //in one operation so a new node can't sneak in between
                //us reading the nodelist, and us registering the event.
                lock (m_OutgoingQueue_Lock) //don't let the callback enqueue a message first
                {
                    NodePeerList localPeerList =
                        CommunicationManager.RegisterLocalPeerListChangedCallback(
                        new CommunicationManager.LocalPeerListChangedHandler(
                            LocalPeerListChangedCallback));
                    Send(localPeerList.ToXml());
                }

                //kick off a read callback chain of threads
                lock (m_ReadCallback_Lock)
                {
                    sslStream.BeginRead(m_newData, 0, m_newData.Length, 
                        new AsyncCallback(ReadCallback), sslStream);
                }

                bool stop = false;
                while (!stop)
                {
                    //send any queued messages
                    int outgoingCount = 0;
                    lock (m_OutgoingQueue_Lock)
                    {
                        outgoingCount = m_OutgoingQueue.Count;
                    }
                    while (outgoingCount > 0)
                    {
                        string data = null;
                        lock (m_OutgoingQueue_Lock)
                        {
                            data = m_OutgoingQueue.Dequeue();
                        }

                        //encode it to base 64 so we don't have to worry about control codes
                        byte[] encbuf = System.Text.Encoding.Unicode.GetBytes(data);
                        string encodedData = Convert.ToBase64String(encbuf) + "\n"; //terminate in newline to denote the end of the message
                        
                        //convert to a byte array
                        byte[] writebuf = System.Text.Encoding.ASCII.GetBytes(encodedData);

                        sslStream.Write(writebuf);
                        sslStream.Flush();

                        outgoingCount--;
                    }

                    //process any received messages
                    int incomingCount = 0;
                    lock (m_IncomingQueue_Lock)
                    {
                        incomingCount = m_IncomingQueue.Count;
                    }
                    while (incomingCount > 0)
                    {
                        string data = null;
                        lock (m_IncomingQueue_Lock)
                        {
                            data = m_IncomingQueue.Dequeue();
                        }

                        //message is in base 64, so convert back to a string
                        byte[] decbuff = Convert.FromBase64String(data);
                        string message = System.Text.Encoding.Unicode.GetString(decbuff);

                        Receive(message);
                        incomingCount--;
                    }

                    //wait
                    System.Threading.Thread.Sleep(100);

                    lock (m_CloseRequest_Lock)
                    {
                        if(stop == false)
                        {
                            stop = m_CloseRequest;
                        }
                    }
                }

                //See if we closed due to an exception in the reader thread
                Exception readException = null;
                lock (m_readException_Lock)
                {
                    readException = m_readException;
                }
                if (readException != null)
                {
                    //TODO propagate this exception to higher code
                }

                //clean up the stream
                sslStream.Close();
            }
        }

        // Careful with threading here.  Basically:
        //    1. m_processor is only accessed by one thread at a time in the ReadCallback
        //    2. m_lineBuffer is also only accessed by one thread at a time in the ReadCallback
        //    3. m_newData is handed from thread to thread both writing and reading
        const int READ_BUFFER_SIZE = 8192;
        //lock access to all of these just to be sure
        private readonly object m_ReadCallback_Lock = new object();
        private readonly ByteArrayProcessor m_processor = new ByteArrayProcessor();
        private readonly StringBuilder m_lineBuffer = new StringBuilder();
        private byte[] m_newData = new byte[READ_BUFFER_SIZE];

        //not thread safe, so we need to lock when accessing
        private Exception m_readException = null;
        private readonly object m_readException_Lock = new object();

        private void ReadCallback(IAsyncResult ar)
        {
            SslStream sslStream = (SslStream)ar.AsyncState;
            int byteCount = -1;
            try
            {
                byteCount = sslStream.EndRead(ar);

                //See if we're supposed to stop reading
                bool stop = false;
                lock (m_CloseRequest_Lock)
                {
                    stop = m_CloseRequest;
                }

                //May not be necessary but technically m_newData is
                //shared state, so I'm locking it
                lock (m_ReadCallback_Lock)
                {
                    if (byteCount > 0)
                    {
                        foreach (string s in m_processor.Process(m_newData, byteCount))
                        {
                            lock (m_IncomingQueue_Lock)
                            {
                                m_IncomingQueue.Enqueue(s);
                            }
                        }
                    }
                    if (!stop)
                    {
                        m_newData = new byte[READ_BUFFER_SIZE];
                        sslStream.BeginRead(m_newData, 0, m_newData.Length,
                            new AsyncCallback(ReadCallback),
                            sslStream);
                    }
                }
            }
            catch (Exception readException)
            {
                //carve the dash on our tombstone before we leave
                lock (m_readException_Lock)
                {
                    m_readException = readException;
                }
                Close(); //signals the connection to exit
            }
        }

        //not thread safe, so we need to lock when accessing
        private bool m_CloseRequest = false;
        private readonly object m_CloseRequest_Lock = new object();
        public void Close()
        {
            lock (m_CloseRequest_Lock)
            {
                m_CloseRequest = true;
            }
        }

        #endregion

    }
}
