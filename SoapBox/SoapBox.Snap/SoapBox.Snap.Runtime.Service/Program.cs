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
using System.ServiceProcess;
using System.Text;
using System.Collections;
using System.Configuration.Install;

namespace SoapBox.Snap.Runtime.Service
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                // Run your service normally.
                //var service = new RuntimeService();
                //service.MainStart();
                //System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
                ServiceBase[] ServicesToRun = new ServiceBase[] { new RuntimeService() };
                ServiceBase.Run(ServicesToRun);
            }
            else if (args.Length >= 1)
            {
                var serviceManager = new ServiceManager(RuntimeService.SERVICE_NAME, typeof(RuntimeServiceInstaller));
                switch (args[0])
                {
                    case "-manual":
                        Properties.Settings.Default.PortNumber = int.Parse(args[1]);
                        var service = new RuntimeService();
                        service.MainStart();
                        System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
                        break;
                    case "-install":
                        Properties.Settings.Default.PortNumber = int.Parse(args[1]);
                        serviceManager.InstallService();
                        serviceManager.StartService(new string[] { Properties.Settings.Default.PortNumber.ToString() });
                        break;
                    case "-uninstall":
                        serviceManager.StopService();
                        serviceManager.UninstallService();
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }


    }
}
