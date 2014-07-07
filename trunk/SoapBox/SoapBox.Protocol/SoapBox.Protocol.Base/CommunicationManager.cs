#region "SoapBox.Protocol License"
/// <header module="SoapBox.Protocol"> 
/// Copyright (C) 2010 SoapBox Automation, All Rights Reserved.
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
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Security.Authentication;

namespace SoapBox.Protocol.Base
{
    public sealed class CommunicationManager
    {

        public static string COMMON_APPLICATION_DATA_FOLDER = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\SoapBox Automation\\SoapBox Protocol";
        public static string PUBLIC_KEY_RING_FILE_NAME = Path.Combine(COMMON_APPLICATION_DATA_FOLDER, @"PublicKeyRing.xml");
        public static string SERVER_CERT_FILENAME = Path.Combine(COMMON_APPLICATION_DATA_FOLDER, @"ServerCertificate.pfx");

        #region " SINGLETON DESIGN PATTERN "
        //this design pattern pulled from here: http://www.yoda.arachsys.com/csharp/singleton.html
        CommunicationManager()
        {
        }

        public static CommunicationManager Instance
        {
            get
            {
                return Nested.instance;
            }
        }
        
        class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }

            internal static readonly CommunicationManager instance = new CommunicationManager();
        }
        #endregion

        #region " PUBLIC KEY MANAGEMENT "

        //not inherently thread safe, need to lock
        private readonly object m_keyRing_Lock = new object();
        private NodePublicKeyRing m_keyRing = null;

        private static void KeyRingInit()
        {
            lock (Instance.m_keyRing_Lock)
            {
                if (Instance.m_keyRing == null)
                {
                    if (File.Exists(PUBLIC_KEY_RING_FILE_NAME))
                    {
                        //if we have an error, then let it throw the exception
                        TextReader tr = new StreamReader(PUBLIC_KEY_RING_FILE_NAME);
                        string xml = tr.ReadToEnd();
                        tr.Close();
                        NodePublicKeyRing newKeyRing = (NodePublicKeyRing)NodeBase.NodeFromXML(xml, null);
                        Instance.m_keyRing = newKeyRing;
                    }
                    else
                    {
                        Instance.m_keyRing = NodePublicKeyRing.BuildWith();
                    }
                }
            }
        }

        private static void KeyRingSave()
        {
            lock (Instance.m_keyRing_Lock)
            {
                KeyRingInit();
                // Make sure the directory path exists
                Directory.CreateDirectory(COMMON_APPLICATION_DATA_FOLDER);
                TextWriter tw = new StreamWriter(PUBLIC_KEY_RING_FILE_NAME);
                string xml = Instance.m_keyRing.ToXml();
                tw.Write(xml);
                tw.Close();
            }
        }

        /// <summary>
        /// Adds a new public key to the KeyRing (if it's not already on there)
        /// and then saves the new ring to the PublicKeyRing file.
        /// </summary>
        /// <param name="key">New public key to add</param>
        public static void PublicKeyRingAdd(NodePublicKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            lock (Instance.m_keyRing_Lock)
            {
                KeyRingInit(); //make sure we've loaded the keyring
                //only add if it doesn't already exist
                if (!PublicKeyRingHasKey(key))
                {
                    Instance.m_keyRing = Instance.m_keyRing.Keys.Append(key);
                    KeyRingSave();
                }
            }
        }

        /// <summary>
        /// Removes an existing public key from the KeyRing (if it's there)
        /// and then saves the new ring to the PublicKeyRing file.
        /// </summary>
        /// <param name="key">New public key to remove</param>
        public static void PublicKeyRingRemove(NodePublicKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            lock (Instance.m_keyRing_Lock)
            {
                KeyRingInit(); //make sure we've loaded the keyring
                NodePublicKey findKey = null;
                foreach (NodePublicKey item in Instance.m_keyRing.Keys)
                {
                    if (item.Key == key.Key)
                    {
                        findKey = item;
                        break;
                    }
                }
                if (findKey != null)
                {
                    Instance.m_keyRing = Instance.m_keyRing.Keys.Remove(findKey);
                    KeyRingSave();
                }
            }
        }

        /// <summary>
        /// Removes all public keys from the KeyRing
        /// and then saves the new empty ring to the PublicKeyRing file.
        /// </summary>
        public static void PublicKeyRingClearAll()
        {
            lock (Instance.m_keyRing_Lock)
            {
                Instance.m_keyRing = NodePublicKeyRing.BuildWith();
                KeyRingSave();
            }
        }

        /// <summary>
        /// Checks to see if the given public key is on the Key Ring.
        /// Note that if the Key Ring is empty, then this function will
        /// add this key to the Key Ring and return true!  This is the 
        /// intended behavior (to bond with the first remote host that
        /// connects with it).  If you don't want that behavior, then 
        /// provide your own key file with a "safe" public key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool PublicKeyRingHasKey(NodePublicKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            lock (Instance.m_keyRing_Lock)
            {
                KeyRingInit(); //make sure we've loaded the keyring
                if (Instance.m_keyRing.Keys.Count == 0)
                {
                    //No keys, so we bond with the first one, 
                    //like a newly hatched chick
                    Instance.m_keyRing = Instance.m_keyRing.Keys.Append(key);
                    KeyRingSave();
                    return true;
                }
                else
                {
                    //check against each key individually
                    bool found = false;
                    foreach (NodePublicKey pubKey in Instance.m_keyRing.Keys)
                    {
                        if (pubKey.Key == key.Key)
                        {
                            found = true;
                            break;
                        }
                    }
                    return found;
                }
            }
        }

        #endregion

        #region " PEER MANAGEMENT "

        //not inherently thread safe, need to lock
        private readonly object m_localPeerList_Lock = new object();
        private NodePeerList m_localPeerList = NodePeerList.BuildWith();

        public delegate void LocalPeerListChangedHandler(
            NodePeerList newLocalPeerList, NodePeerList oldLocalPeerList);
        //adding/removing delegates to events is thread safe
        public static event LocalPeerListChangedHandler OnLocalPeerListChanged = delegate { };

        /// <summary>
        /// Returns the current list of local Peers
        /// </summary>
        public static NodePeerList LocalPeerList
        {
            get
            {
                lock (Instance.m_localPeerList_Lock)
                {
                    return Instance.m_localPeerList;
                }
            }
            private set
            {
                lock (Instance.m_localPeerList_Lock)
                {
                    if (value != Instance.m_localPeerList)
                    {
                        OnLocalPeerListChanged(value, Instance.m_localPeerList);
                        Instance.m_localPeerList = value;
                    }
                }
            }
        }

        /// <summary>
        /// Registers a LocalPeerListChangedHandler and returns the existing
        /// local peer list in one operation, so you are guaranteed not
        /// to miss any changes to the list.  Be careful - the event could
        /// actually fire before you do something with the returned value.
        /// </summary>
        /// <param name="newHandler"></param>
        /// <returns></returns>
        public static NodePeerList RegisterLocalPeerListChangedCallback(LocalPeerListChangedHandler newHandler)
        {
            lock (Instance.m_localPeerList_Lock)
            {
                OnLocalPeerListChanged += newHandler;
                return LocalPeerList;
            }
        }

        /// <summary>
        /// Registers a new local peer.  The calling function has to pass
        /// in a Code that is a unique name for the peer.  No other peer
        /// by this name can exist locally.  Returns null if it already 
        /// exists, or a new NodePeer object if successful.
        /// </summary>
        /// <param name="Code">Unique name for the new peer</param>
        /// <returns></returns>
        public static NodePeer RegisterLocalPeer(FieldIdentifier Code)
        {
            if (Code == null)
            {
                throw new ArgumentNullException("Code");
            }

            lock (Instance.m_localPeerList_Lock)
            {
                //make sure "Local.{thisCode}" doesn't exist yet
                //(no duplicate peer names)
                bool found = false;
                foreach (NodePeer p in Instance.m_localPeerList.Peers)
                {
                    if (p.Code == Code)
                    {
                        found = true;
                        break;
                    }
                }
                if (found)
                {
                    return null;
                }
                else
                {
                    NodePeer newPeer = NodePeer.BuildWith(Code);
                    LocalPeerList = LocalPeerList.Peers.Append(newPeer);
                    return newPeer;
                }
            }
        }

        /// <summary>
        /// Unregisters a peer that was created by calling the
        /// RegisterLocalPeer function.  Throws an exception if
        /// the peer is not a local peer.
        /// </summary>
        /// <param name="Peer"></param>
        /// <returns>True if successful, False otherwise</returns>
        public static bool UnregisterLocalPeer(NodePeer Peer)
        {
            if (Peer == null)
            {
                throw new ArgumentNullException("Peer");
            }

            if (!Peer.IsLocal)
            {
                throw new ArgumentOutOfRangeException("Peer");
            }

            lock (Instance.m_localPeerList_Lock)
            {
                if (Instance.m_localPeerList.Peers.Contains(Peer))
                {
                    Peer.Unregister(); //clears all event handlers
                    LocalPeerList = LocalPeerList.Peers.Remove(Peer);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        #endregion

        #region " SEND/RECEIVE PEER "
        static internal void SendToPeer(NodePeer ToPeer, NodePeer FromPeer, NodeBase Message)
        {
            if (ToPeer == null)
            {
                throw new ArgumentNullException("ToPeer");
            }
            if (FromPeer == null)
            {
                throw new ArgumentNullException("FromPeer");
            }
            if (Message != null)
            {
                if (ToPeer.IsLocal)
                {
                    ToPeer.ReceiveFromPeer(FromPeer, Message);
                }
                else
                {
                    ToPeer.Connection.Send(ToPeer, FromPeer, Message);
                }
            }
        }

        static internal void SendDeltaToPeer(NodePeer ToPeer, NodePeer FromPeer, NodeBase Message, NodeBase BasedOnMessage)
        {
            if (BasedOnMessage == null)
            {
                SendToPeer(ToPeer, FromPeer, Message);
            }
            else
            {
                if (ToPeer == null)
                {
                    throw new ArgumentNullException("ToPeer");
                }
                if (FromPeer == null)
                {
                    throw new ArgumentNullException("FromPeer");
                }
                if (Message != null)
                {
                    if (ToPeer.IsLocal)
                    {
                        ReceiveDeltaFromLocalPeerThreadStart(ToPeer, FromPeer, Message, BasedOnMessage);
                    }
                    else
                    {
                        ToPeer.Connection.Send(ToPeer, FromPeer, Message,BasedOnMessage);
                    }
                }
            }
        }

        static private void ReceiveDeltaFromLocalPeerThreadStart(NodePeer ToPeer, NodePeer FromPeer, NodeBase Message, NodeBase BasedOnMessage)
        {
            NodeBase BasedOnMessage_rcv = ToPeer.ReceiveDeltaFromPeer(FromPeer, BasedOnMessage.ID);
            if (BasedOnMessage_rcv != null)
            {
                ToPeer.ReceiveFromPeer(FromPeer, Message);
            }
        }
        #endregion

        #region " CONNECTION MANAGEMENT "

        //When used as a server this is meant to operate without human
        //interaction, so a password is kind of useless here.
        //However, my Certificate generation could needs one.
        //Ultimately the security of this certificate depends on the 
        //security of the host system.
        const string SERVER_CERT_PASSWORD = "SoapBox"; 

        //not inherently thread safe, need to lock
        private readonly object m_listener_Lock = new object();
        private readonly Dictionary<int, TcpListener> m_listenerDictionary =
            new Dictionary<int, TcpListener>();

        private static TcpListener ListenerByPort(int port)
        {
            lock(Instance.m_listener_Lock)
            {
                if (Instance.m_listenerDictionary.ContainsKey(port))
                {
                    return Instance.m_listenerDictionary[port];
                }
                else
                {
                    return null;
                }
            }
        }

        public static NodeConnection StartOutgoingConnection(FieldIdentifier Code, string hostName, int port)
        {
            if (hostName == null)
            {
                throw new ArgumentNullException("hostName");
            }
            if (port <= 0)
            {
                throw new ArgumentOutOfRangeException("port");
            }

            TcpClient client = new TcpClient();
            client.Connect(hostName, port);

            SslStream sslStream = new SslStream(client.GetStream(),
                false,
                new RemoteCertificateValidationCallback(CertificateValidationCallback),
                new LocalCertificateSelectionCallback(CertificateSelectionCallback));

            bool authenticationPassed = true;
            try
            {
                string serverName = System.Environment.MachineName;

                X509Certificate cert = GetServerCert(SERVER_CERT_FILENAME, SERVER_CERT_PASSWORD);
                X509CertificateCollection certs = new X509CertificateCollection();
                certs.Add(cert);

                sslStream.AuthenticateAsClient(
                    serverName,
                    certs,
                    SslProtocols.Default,
                    false); // check cert revokation
            }
            catch (AuthenticationException)
            {
                authenticationPassed = false;
            }
            if (authenticationPassed)
            {
                NodeConnection newConnection = NodeConnection.BuildWith(Code);
                ThreadPool.QueueUserWorkItem(new WaitCallback(newConnection.ThreadStart), sslStream);

                return newConnection;
            }
            else
            {
                return null;
            }
        }

        private static X509Certificate CertificateSelectionCallback(object sender,
            string targetHost,
            X509CertificateCollection localCertificates,
            X509Certificate remoteCertificate,
            string[] acceptableIssuers)
        {
            return localCertificates[0];
        }

        public static bool StopAcceptingIncomingConnections(int port)
        {
            if (port <= 0)
            {
                throw new ArgumentOutOfRangeException("port");
            }

            lock (Instance.m_listener_Lock)
            {
                if (!Instance.m_listenerDictionary.ContainsKey(port))
                {
                    return false;
                }
                else
                {
                    Instance.m_listenerDictionary.Remove(port);
                    return true;
                }
            }
        }

        public static bool StartAcceptingIncomingConnections(int port)
        {
            if (port <= 0)
            {
                throw new ArgumentOutOfRangeException("port");
            }

            lock (Instance.m_listener_Lock)
            {
                if (Instance.m_listenerDictionary.ContainsKey(port))
                {
                    return false;
                }
                else
                {
                    TcpListener newListener = new TcpListener(
                    IPAddress.Parse("127.0.0.1"), port);
                    newListener.Start();
                    Instance.m_listenerDictionary.Add(port, newListener);
                    ThreadPool.QueueUserWorkItem(new WaitCallback(ListenerThreadStart), port);

                    return true;
                }
            }
        }

        private static void ListenerThreadStart(object o)
        {
            int port = (int)o; //unboxing
            TcpListener listener = ListenerByPort(port);

            //Listen for new connections until the listener
            //gets removed from the dictionary
            while(listener != null)
            {
                while (listener.Pending())
                {
                    TcpClient client = listener.AcceptTcpClient(); //blocks
                    ThreadPool.QueueUserWorkItem(new WaitCallback(ServerThreadStart), client);
                }
                Thread.Sleep(100);
                TcpListener newListener = ListenerByPort(port); // checks if port has been requested to be closed
                if (newListener == null)
                {
                    listener.Stop();
                    return;
                }
                listener = newListener;
            }
        }

        private static void ServerThreadStart(object o)
        {
            TcpClient client = o as TcpClient;

            SslStream sslStream = new SslStream(client.GetStream(),
                false, //leave inner stream open
                new RemoteCertificateValidationCallback(CertificateValidationCallback)
                );

            bool authenticationPassed = true;
            try
            {
                X509Certificate cert = GetServerCert(SERVER_CERT_FILENAME, SERVER_CERT_PASSWORD);
                sslStream.AuthenticateAsServer(
                    cert,
                    true, //client cert required 
                    SslProtocols.Default,
                    false); //check for revoked cert
            }
            catch (AuthenticationException)
            {
                authenticationPassed = false;
            }
            if (authenticationPassed)
            {
                FieldIdentifier Code = new FieldIdentifier("Incoming_" + new FieldGuid().ToString().Replace("-","_"));
                NodeConnection newConnection = NodeConnection.BuildWith(Code);
                OnNewIncomingConnection(newConnection);
                newConnection.ThreadStart(sslStream); //essentially blocks until the connection dies
            }
        }

        public delegate void NewIncomingConnectionHandler(NodeConnection NewIncomingConnection);
        //adding/removing delegates to events is thread safe
        public static event NewIncomingConnectionHandler OnNewIncomingConnection = delegate { };

        static bool CertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors != SslPolicyErrors.None)
            {
                if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors)
                {
                    //Make sure the only error is an untrusted root
                    //(because we're assuming it's a self-signed certificate and 
                    // we're going to check it against an internal list of public keys)
                    bool failed = false;
                    foreach (X509ChainStatus status in chain.ChainStatus)
                    {
                        if (status.Status != X509ChainStatusFlags.UntrustedRoot)
                        {
                            failed = true;
                            break;
                        }
                    }
                    //Pull the public key out of the certificate
                    NodePublicKey key = NodePublicKey.BuildWith(new FieldPublicKey(certificate.GetPublicKeyString()));
                    if (!failed && PublicKeyRingHasKey(key))
                    {
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("SSL Certificate Validation Error!");
                        Console.WriteLine(sslPolicyErrors.ToString());
                        return false;
                    }
                }
                else
                {
                    Console.WriteLine("SSL Certificate Validation Error!");
                    Console.WriteLine(sslPolicyErrors.ToString());
                    return false;
                }

            }
            else
                return true;
        }

        /// <summary>
        /// Returns the public key of this server from the certificate file.
        /// </summary>
        /// <returns></returns>
        public static NodePublicKey GetServerPublicKey()
        {
            FieldPublicKey key = new FieldPublicKey(
                GetServerPublicKeyString());

            return NodePublicKey.BuildWith(key);
        }

        /// <summary>
        /// Returns the public key of this server from the certificate file.
        /// </summary>
        /// <returns></returns>
        public static string GetServerPublicKeyString()
        {
            return GetServerCert(SERVER_CERT_FILENAME, SERVER_CERT_PASSWORD)
                .GetPublicKeyString();
        }

        //Only want one thread reading the certificate at any time
        //so we don't get any race conditions when building the certificate
        private readonly object m_cert_Lock = new object();

        /// <summary>
        /// Loads the server certificate from a file.  If the file
        /// doesn't exist it will try to create a new one.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private static X509Certificate GetServerCert(
            string filename,
            string password
            )
        {
            lock (Instance.m_cert_Lock)
            {
                X509Certificate cert = null;

                try
                {
                    cert = new X509Certificate2(
                                            filename,
                                            password);
                }
                catch (System.Security.Cryptography.CryptographicException)
                {
                    cert = null;
                    GenerateNewCert(SERVER_CERT_FILENAME,
                        System.Environment.MachineName,
                        DateTime.Today,
                        DateTime.MaxValue,
                        SERVER_CERT_PASSWORD);
                }

                if (cert == null)
                {
                    cert = new X509Certificate2(
                                            filename,
                                            password);
                }

                return cert;
            }
        }

        /// <summary>
        /// Creates a new server certificate.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="hostname"></param>
        /// <param name="notValidBefore"></param>
        /// <param name="notValidAfter"></param>
        /// <param name="password"></param>
        private static void GenerateNewCert(
            string filename,
            string hostname,
            DateTime notValidBefore,
            DateTime notValidAfter,
            string password
            )
        {
            Directory.CreateDirectory(COMMON_APPLICATION_DATA_FOLDER);
            byte[] c = Certificate.CreateSelfSignCertificatePfx(
                "CN=" + hostname, //host name
                notValidBefore, //not valid before
                notValidAfter, //not valid after
                password); //password to encrypt key file

            using (BinaryWriter binWriter = new BinaryWriter(
                File.Open(filename, FileMode.Create)))
            {
                binWriter.Write(c);
            }
        }

        #endregion
    }

}




