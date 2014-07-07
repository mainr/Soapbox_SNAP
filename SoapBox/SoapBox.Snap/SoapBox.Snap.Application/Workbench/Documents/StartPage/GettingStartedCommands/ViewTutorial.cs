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
using System.ComponentModel.Composition;
using SoapBox.Core;

namespace SoapBox.Snap.Application
{
    [Export(ExtensionPoints.Workbench.Documents.StartPage.GettingStartedItems, typeof(IButton))]
    class ViewTutorial : AbstractButton
    {
        public const string TUTORIAL_URL = StartPage.SOAPBOXAUTOMATION_URL + "/support-2/soapbox-snap-tutorial";

        public ViewTutorial()
        {
            ID = Extensions.Workbench.Documents.StartPage_.GettingStartedCommands.ViewTutorial;
            InsertRelativeToID = Extensions.Workbench.Documents.StartPage_.GettingStartedCommands.LoadExample;
            BeforeOrAfter = RelativeDirection.After;
            Text = Resources.Strings.Workbench_Documents_StartPage_ViewTutorial;
        }

        [Import(CompositionPoints.Workbench.Documents.StartPage, typeof(StartPage))]
        private Lazy<StartPage> startPage { get; set; }

        protected override void Run()
        {
            startPage.Value.FrameSource = new Uri(TUTORIAL_URL);
        }
    }
}
