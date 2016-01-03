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

namespace SoapBox.Snap.Application
{
    [Export(SoapBox.Core.ExtensionPoints.Workbench.MainMenu.ViewMenu, typeof(IMenuItem))]
    class ViewMenuNew : AbstractMenuItem, IPartImportsSatisfiedNotification
    {
        public ViewMenuNew()
        {
            ID = Extensions.Workbench.MainMenu.ViewMenu.SolutionExplorer;
            Header = Resources.Strings.Workbench_MainMenu_View_SolutionExplorer;
            ToolTip = Resources.Strings.Workbench_MainMenu_View_SolutionExplorer_ToolTip;
            InsertRelativeToID = SoapBox.Core.Extensions.Workbench.MainMenu.ViewMenu.ToolBars;
            BeforeOrAfter = RelativeDirection.Before;
        }

        public void OnImportsSatisfied()
        {
            // A bit of a hack - enable this when we have a solution opened
            EnableCondition = solutionService.CloseEnabled;
        }

        [Import(Services.Solution.SolutionService, typeof(ISolutionService))]
        private ISolutionService solutionService { get; set; }

        [Import(SoapBox.Core.Services.Layout.LayoutManager, typeof(ILayoutManager))]
        private Lazy<ILayoutManager> layoutManager { get; set; }

        [Import(CompositionPoints.Workbench.Pads.SolutionPad, typeof(SolutionPad))]
        private Lazy<SolutionPad> solutionPad { get; set; }

        protected override void Run()
        {
            layoutManager.Value.ShowPad(solutionPad.Value);
        }
    }

    [Export(SoapBox.Core.ExtensionPoints.Workbench.MainMenu.ViewMenu, typeof(IMenuItem))]
    class ViewMenuInstructionPad : AbstractMenuItem, IPartImportsSatisfiedNotification
    {
        public ViewMenuInstructionPad()
        {
            ID = Extensions.Workbench.MainMenu.ViewMenu.Instructions;
            Header = Resources.Strings.Workbench_MainMenu_View_Instructions;
            ToolTip = Resources.Strings.Workbench_MainMenu_View_Instructions_ToolTip;
            InsertRelativeToID = Extensions.Workbench.MainMenu.ViewMenu.SolutionExplorer;
            BeforeOrAfter = RelativeDirection.After;
        }

        public void OnImportsSatisfied()
        {
            // A bit of a hack - enable this when we have a solution opened
            EnableCondition = solutionService.CloseEnabled;
        }

        [Import(Services.Solution.SolutionService, typeof(ISolutionService))]
        private ISolutionService solutionService { get; set; }

        [Import(SoapBox.Core.Services.Layout.LayoutManager, typeof(ILayoutManager))]
        private Lazy<ILayoutManager> layoutManager { get; set; }

        [Import(CompositionPoints.Workbench.Pads.InstructionPad, typeof(InstructionPad))]
        private Lazy<InstructionPad> instructionPad { get; set; }

        protected override void Run()
        {
            layoutManager.Value.ShowPad(instructionPad.Value);
        }
    }

    [Export(SoapBox.Core.ExtensionPoints.Workbench.MainMenu.ViewMenu, typeof(IMenuItem))]
    class ViewMenuStartPage : AbstractMenuItem
    {
        public ViewMenuStartPage()
        {
            ID = Extensions.Workbench.MainMenu.ViewMenu.StartPage;
            Header = Resources.Strings.Workbench_MainMenu_View_StartPage;
            ToolTip = Resources.Strings.Workbench_MainMenu_View_StartPage_ToolTip;
            InsertRelativeToID = Extensions.Workbench.MainMenu.ViewMenu.Instructions;
            BeforeOrAfter = RelativeDirection.After;
        }

        [Import(SoapBox.Core.Services.Layout.LayoutManager, typeof(ILayoutManager))]
        private Lazy<ILayoutManager> layoutManager { get; set; }

        [Import(CompositionPoints.Workbench.Documents.StartPage, typeof(StartPage))]
        private Lazy<StartPage> startPage { get; set; }

        protected override void Run()
        {
            layoutManager.Value.ShowDocument(startPage.Value, startPage.Value.Memento);
        }
    }
}
