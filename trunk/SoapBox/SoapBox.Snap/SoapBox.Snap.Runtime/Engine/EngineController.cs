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
using System.ComponentModel.Composition;
using System.ComponentModel;
using SoapBox.Utilities;
using System.Windows;
using SoapBox.Protocol.Base;
using System.Diagnostics;
using System.ServiceProcess;
using SoapBox.Core;

namespace SoapBox.Snap.Runtime
{
    [Export(SoapBox.Snap.Runtime.CompositionPoints.Runtime.EngineController, typeof(EngineController))]
    public class EngineController
    {
        public const string SERVICE_NAME = "SoapBox.Snap.Runtime.Service";
        public const string SERVICE_EXE = SERVICE_NAME + ".exe";

        public EngineController()
        {
        }

        [Import(SoapBox.Snap.Runtime.CompositionPoints.Runtime.Engine, typeof(IEngine))]
        private Lazy<Engine> engine { get; set; }

        [Import(CompositionPoints.Options.OptionsPad, typeof(RuntimeOptionsPad))]
        private Lazy<RuntimeOptionsPad> runtimeOptionsPad { get; set; }

        [Import(Snap.Runtime.CompositionPoints.Runtime.RuntimeType, typeof(RuntimeType))]
        private Lazy<RuntimeType> runtimeType { get; set; }

        [Import(SoapBox.Core.CompositionPoints.Host.MainWindow, typeof(Window))]
        private Lazy<Window> mainWindow { get; set; }

        [Import(SoapBox.Core.Services.Messaging.MessagingService, typeof(IMessagingService))]
        private Lazy<IMessagingService> messagingService { get; set; }

        static readonly string m_RunAsServiceName =
            NotifyPropertyChangedHelper.GetPropertyName<RuntimeOptionsPad>(o => o.RunAsService);

        static readonly string m_PortNumberName =
            NotifyPropertyChangedHelper.GetPropertyName<RuntimeOptionsPad>(o => o.PortNumber);

        // Called when the app starts
        public void Startup()
        {
            startStopEngine();

            // We want notification when the options change
            runtimeOptionsPad.Value.PropertyChanged += new PropertyChangedEventHandler((s, e) =>
            {
                if (e.PropertyName == m_RunAsServiceName)
                {
                    startStopEngine();
                }
                else if (e.PropertyName == m_PortNumberName)
                {
                    stopServiceEngine();
                    startStopEngine();
                }
            });

            // Watch for local engines
            var list = CommunicationManager.RegisterLocalPeerListChangedCallback(
                new CommunicationManager.LocalPeerListChangedHandler(
                    delegate(NodePeerList newList, NodePeerList oldList)
                    {
                        runtimeType.Value.SetLocalRuntimePeer(newList);
                    }
            ));
            runtimeType.Value.SetLocalRuntimePeer(list);


        }

        // Called when the app stops
        public void Shutdown()
        {
            stopLocalEngine();
        }

        public bool IsServiceRunning()
        {
            using (ServiceController controller = new ServiceController(SERVICE_NAME))
            {
                if (!IsServiceInstalled()) return false;
                return (controller.Status == ServiceControllerStatus.Running);
            }
        }

        public bool IsServiceInstalled()
        {
            using (ServiceController controller = new ServiceController(SERVICE_NAME))
            {
                try
                {
                    ServiceControllerStatus status = controller.Status;
                }
                catch
                {
                    return false;
                }
                return true;
            }
        }

        private void startStopEngine()
        {
            if (Properties.Settings.Default.RunAsService)
            {
                stopLocalEngine();
                bool closing = false;
                if (!IsServiceRunning())
                {
                    messagingService.Value.ShowMessage(
                        Resources.Strings.RuntimeController_InstallMessage,
                        Resources.Strings.RuntimeController_InstallTitle);

                    if (true)
                    {
                        string exeArgs = "-install " + Properties.Settings.Default.PortNumber.ToString();
                        Process.Start(SERVICE_EXE, exeArgs).WaitForExit();
                    }
                    if (engine.IsValueCreated)
                    {
                        // we have to restart the app so that it unloads the local engine (it has a lock on the drivers)
                        closing = true;
                        messagingService.Value.ShowMessage(
                            Resources.Strings.RuntimeController_ShutdownMessage, 
                            Resources.Strings.RuntimeController_ShutdownTitle);
                        mainWindow.Value.Close();
                    }
                }
                if (!closing)
                {
                    newOutgoingConnection();
                }
           }
            else
            {
                stopServiceEngine();// shuts down existing connection if necessary
                engine.Value.Load();
            }
        }

        private int m_port = 0;
        private NodeConnection m_outgoingConnection = null;
        private void newOutgoingConnection()
        {
            int port = Properties.Settings.Default.PortNumber;
            if (!Properties.Settings.Default.RunAsService)
            {
                port = 0;
            }
            if (port != m_port)
            {
                if (port > 0)
                {
                    m_port = port;
                    m_outgoingConnection = CommunicationManager.StartOutgoingConnection(
                        new FieldIdentifier("test"), "localhost", port);
                    var remoteList = m_outgoingConnection.RegisterRemotePeerListChangedCallback(
                        new NodeConnection.RemotePeerListChangedHandler(
                            delegate(NodePeerList remotePeerList)
                            {
                                runtimeType.Value.SetLocalRuntimePeer(remotePeerList);
                            }
                    ));
                    runtimeType.Value.SetLocalRuntimePeer(remoteList);
                }
            }
        }

        private void stopOutgoingConnection()
        {
            int port = Properties.Settings.Default.PortNumber;
            if (!Properties.Settings.Default.RunAsService)
            {
                port = 0;
            }
            if (port != m_port)
            {
                if (m_port > 0 && m_outgoingConnection != null)
                {
                    // stop existing connection
                    m_outgoingConnection.Close();
                    m_outgoingConnection = null;
                }
            }
        }

        private void stopLocalEngine()
        {
            if (engine.IsValueCreated)
            {
                engine.Value.Stop();
            }
        }

        private void stopServiceEngine()
        {
            stopOutgoingConnection();
            if (IsServiceInstalled())
            {
                messagingService.Value.ShowMessage(
                    Resources.Strings.RuntimeController_UninstallMessage,
                    Resources.Strings.RuntimeController_UninstallTitle);
                string exeArgs = "-uninstall";
                Process.Start(SERVICE_EXE, exeArgs).WaitForExit();
            }
        }
    }
}
