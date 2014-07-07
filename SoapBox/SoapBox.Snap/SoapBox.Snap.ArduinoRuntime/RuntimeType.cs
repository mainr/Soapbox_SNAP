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
using SoapBox.Core;
using System.ComponentModel.Composition;
using SoapBox.Protocol.Automation;
using System.Collections.ObjectModel;
using SoapBox.Snap.ArduinoRuntime.Protocol;

namespace SoapBox.Snap.ArduinoRuntime
{
    [Export(Snap.ExtensionPoints.Runtime.Types, typeof(IRuntimeType))]
    [Export(Snap.Runtime.CompositionPoints.Runtime.RuntimeType, typeof(RuntimeType))]
    public class RuntimeType : AbstractExtension, IRuntimeType
    {
        private const string RUNTIME_TYPE_ID = "893ecf1f-dba8-4dfe-86de-7ec83fc869e0";

        private readonly Lazy<IRuntimeService> m_runtimeService;

        [ImportingConstructor]
        public RuntimeType(
            [Import(Services.Solution.RuntimeService)]
                Lazy<IRuntimeService> runtimeService)
        {
            if (runtimeService == null) throw new ArgumentNullException("runtimeService");
            ID = Extensions.Runtime.Arduino;
            this.m_runtimeService = runtimeService;
        }

        [Import(SoapBox.Core.Services.Messaging.MessagingService, typeof(IMessagingService))]
        private IMessagingService messagingService { get; set; }



        public event EventHandler Disconnected;

        private void fireDisconnectedEvent()
        {
            var evt = Disconnected;
            if (evt != null)
            {
                evt(this, EventArgs.Empty);
            }
        }

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
            var comPort = rta.Address.ToString().ToUpper();
            foreach (var rt in Runtimes)
            {
                var arduinoRuntimeProxy = rt as ArduinoRuntimeProxy;
                if (arduinoRuntimeProxy != null
                    && arduinoRuntimeProxy.ComPort.ToUpper() == comPort.ToUpper())
                {
                    return arduinoRuntimeProxy;
                }
            }
            ArduinoRuntimeProxy retVal = null;
            try
            {
                retVal = new ArduinoRuntimeProxy(
                    this,
                    comPort,
                    this.m_runtimeService,
                    this.messagingService);
                this.m_Runtimes.Add(retVal);
            }
            catch (ProtocolException ex)
            {
                this.messagingService.ShowMessage(
                    ex.Message,
                    "Error connecting to Runtime");
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
