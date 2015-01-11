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
using SoapBox.Core;
using System.ComponentModel.Composition;
using SoapBox.Protocol.Base;
using SoapBox.Protocol.Automation;
using System.Collections.ObjectModel;

namespace SoapBox.Snap.Runtime
{
    [Export(Snap.ExtensionPoints.Runtime.Types, typeof(IRuntimeType))]
    [Export(Snap.Runtime.CompositionPoints.Runtime.RuntimeType, typeof(RuntimeType))]
    public class RuntimeType : AbstractExtension, IRuntimeType
    {
        private const string RUNTIME_TYPE_ID = "91b1f5a8-2cf4-402d-8d22-0a63a85b7aeb";

        [ImportingConstructor]
        public RuntimeType(
            [Import(Services.CommunicationManager.CommunicationService)]
                Lazy<ICommunicationService> communicationService,
            [Import(Services.Solution.RuntimeService)]
                Lazy<IRuntimeService> runtimeService)
        {
            ID = Extensions.Runtime.Snap_;
            m_communicationService = communicationService;
            m_runtimeService = runtimeService;
        }

        public event EventHandler Disconnected;

        private void fireDisconnectedEvent()
        {
            var evt = Disconnected;
            if (evt != null)
            {
                evt(this, EventArgs.Empty);
            }
        }

        private readonly Lazy<ICommunicationService> m_communicationService = null;
        private Lazy<IRuntimeService> m_runtimeService = null;

        #region "localRuntimePeer"

        private NodePeer localRuntimePeer
        {
            get
            {
                return m_localRuntimePeer;
            }
            set
            {
                if (m_localRuntimePeer != value)
                {
                    if (m_localRuntimePeer != null)
                    {
                        fireDisconnectedEvent();
                    }
                    m_localRuntimePeer = value;
                    m_Runtimes.Clear();
                    m_Runtimes.Add(new Runtime(m_communicationService, m_runtimeService, this, value));
                }
            }
        }
        private NodePeer m_localRuntimePeer = null;

        internal void SetLocalRuntimePeer(NodePeerList list)
        {
            foreach (var peer in list.Peers)
            {
                if (peer.Code.ToString() == Engine.PEER_CODE)
                {
                    localRuntimePeer = peer;
                    break;
                }
            }
        }

        #endregion

        #region "IRuntimeType Members"

        public Guid TypeId
        {
            get
            {
                return m_TypeId;
            }
        }
        private readonly Guid m_TypeId = new Guid(RUNTIME_TYPE_ID);

        public string Name
        {
            get
            {
                return Resources.Strings.Runtime_Name;
            }
        }

        public IRuntime OpenRuntime(NodeRuntimeApplication rta)
        {
            IRuntime retVal = null;
            foreach (var rt in Runtimes)
            {
                retVal = rt;
                break;
            }
            return retVal;
        }

        public IEnumerable<IRuntime> Runtimes
        {
            get
            {
                return m_Runtimes;
            }
        }
        private readonly Collection<IRuntime> m_Runtimes = new Collection<IRuntime>();

        #endregion
    }
}
