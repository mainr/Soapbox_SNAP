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
using SoapBox.Core;
using System.Windows;

namespace SoapBox.Snap.Runtime
{
    [Export(SoapBox.Core.ExtensionPoints.Host.ShutdownCommands,typeof(IExecutableCommand))]
    class StopRuntimeEngine : AbstractExtension, IExecutableCommand
    {
        public StopRuntimeEngine()
        {
            ID = Snap.Extensions.Host.ShutdownCommands.StopRuntimeEngine;
        }

        [Import(SoapBox.Snap.Runtime.CompositionPoints.Runtime.EngineController, typeof(EngineController))]
        private EngineController engineController { get; set; }

        public void Run(params object[] args)
        {
            engineController.Shutdown();
        }
    }
}
