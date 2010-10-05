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
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition;
using SoapBox.Protocol.Base;

namespace SoapBox.Snap.Runtime.Service
{
    public partial class RuntimeService : ServiceBase
    {
        public const string SERVICE_NAME = "SoapBox.Snap.Runtime.Service";

        public RuntimeService()
        {
            InitializeComponent();
            this.ServiceName = SERVICE_NAME;
            this.CanStop = true;
            this.CanPauseAndContinue = false;
            this.AutoLog = true;
        }

        protected override void OnStart(string[] args)
        {
            if (args.Length == 1)
            {
                int port;
                if (int.TryParse(args[0], out port))
                {
                    Properties.Settings.Default.PortNumber = port;
                }
            }
            MainStart();
        }

        public void MainStart()
        {
            if (Compose())
            {
                // we just want to call the constructor
                var e = engine.Value;

                if (Properties.Settings.Default.PortNumber > 0)
                {
                    CommunicationManager.StartAcceptingIncomingConnections(Properties.Settings.Default.PortNumber);
                }

            }
        }

        protected override void OnStop()
        {
            if (_container != null)
            {
                CommunicationManager.StopAcceptingIncomingConnections(Properties.Settings.Default.PortNumber);
                engine.Value.Stop();
                _container.Dispose();
            }
            _container = null;
        }

        [Import(SoapBox.Snap.Runtime.CompositionPoints.Runtime.Engine, typeof(IEngine))]
        private Lazy<IEngine> engine { get; set; }

        private CompositionContainer _container;

        private bool Compose()
        {
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new DirectoryCatalog("."));

            _container = new CompositionContainer(catalog);

            try
            {
                _container.ComposeParts(this);
            }
            catch (CompositionException)
            {

                return false;
            }
            return true;
        }

    }
}
