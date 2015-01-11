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
using System.IO;

namespace SoapBox.Snap.Application
{
    [Export(SoapBox.Core.ExtensionPoints.Host.StartupCommands, typeof(IExecutableCommand))]
    class OpenSolutionFromArgumentOrStartPage : AbstractExtension, IExecutableCommand
    {
        public OpenSolutionFromArgumentOrStartPage()
        {
            ID = Extensions.Host.StartupCommands.OpenSolutionFromArgumentOrStartPage;
        }

        [Import(CompositionPoints.Workbench.Documents.StartPage)]
        private Lazy<StartPage> startPage { get; set; }

        [Import(SoapBox.Core.Services.Layout.LayoutManager)]
        private ILayoutManager layoutManager { get; set; }

        [Import(SoapBox.Core.Services.Host.ArgumentsService)]
        private IArgumentsService argumentsService { get; set; }

        [Import(Services.Solution.SolutionService)]
        private Lazy<ISolutionService> solutionService { get; set; }

        public void Run(params object[] args)
        {
            var possibleFilename = solutionFileToOpenFromArguments();
            if(!string.IsNullOrEmpty(possibleFilename))
            {
                layoutManager.Loaded += (s, e) =>
                    {
                        solutionService.Value.OpenExecute(possibleFilename, false);
                    };
            }
            else if (Properties.Settings.Default.AutoOpenStartPage)
            {
                layoutManager.ShowDocument(startPage.Value, startPage.Value.Memento);
            }
        }

        private string solutionFileToOpenFromArguments()
        {
            string retVal = string.Empty;
            if (argumentsService.Args.Length >= 1)
            {
                var firstArgument = argumentsService.Args[0];
                if(File.Exists(firstArgument))
                {
                    retVal = firstArgument;
                }
            }
            return retVal;
        }
    }
}
