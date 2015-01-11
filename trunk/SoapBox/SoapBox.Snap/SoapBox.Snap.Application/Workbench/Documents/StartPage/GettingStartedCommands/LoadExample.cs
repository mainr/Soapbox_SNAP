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
using System.ComponentModel.Composition;
using SoapBox.Core;
using System.Reflection;
using System.IO;

namespace SoapBox.Snap.Application
{
    [Export(ExtensionPoints.Workbench.Documents.StartPage.GettingStartedItems, typeof(IButton))]
    class LoadExample : AbstractButton
    {
        private const string EXAMPLE_FILE_NAME = "Example.snp";

        public LoadExample()
        {
            ID = Extensions.Workbench.Documents.StartPage_.GettingStartedCommands.LoadExample;
            InsertRelativeToID = Extensions.Workbench.Documents.StartPage_.GettingStartedCommands.LoadNewSolution;
            BeforeOrAfter = RelativeDirection.After;
            Text = Resources.Strings.Workbench_Documents_StartPage_LoadExample;
        }

        [Import(Services.Solution.SolutionService, typeof(ISolutionService))]
        private Lazy<ISolutionService> solutionService { get; set; }

        [Import(CompositionPoints.Workbench.Pads.SolutionPad_.RootSolutionItem, typeof(RootSolutionItem))]
        private Lazy<RootSolutionItem> rootSolutionItem { get; set; }

        [Import(SoapBox.Core.Services.Layout.LayoutManager, typeof(ILayoutManager))]
        private ILayoutManager layoutManager { get; set; }

        [Import(CompositionPoints.Workbench.Documents.StartPage, typeof(StartPage))]
        private Lazy<StartPage> startPage { get; set; }

        protected override void Run()
        {
            try
            {
                var exampleFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),EXAMPLE_FILE_NAME);
                solutionService.Value.OpenExecute(exampleFile, true);
                layoutManager.CloseDocument(startPage.Value);
                foreach (var childOfSolution in rootSolutionItem.Value.Items)
                {
                    var runtimeApplicationItem = childOfSolution as RuntimeApplicationItem;
                    if (runtimeApplicationItem != null)
                    {
                        runtimeApplicationItem.Connect(true);
                        runtimeApplicationItem.Start();
                        break;
                    }
                }
            }
            catch(Exception ex)
            {
                logger.Error("Error executing LoadExample", ex);
            }
        }
    }
}
