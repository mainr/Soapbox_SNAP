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

namespace SoapBox.Snap.Application
{
    [Export(SoapBox.Core.ExtensionPoints.Workbench.ToolBars, typeof(IToolBar))]
    public class SolutionToolBar : AbstractToolBar, IPartImportsSatisfiedNotification
    {
        public SolutionToolBar()
        {
            Name = Resources.Strings.Solution_ToolBar_Name;
            VisibleCondition = new ConcreteCondition(true);
        }

        [Import(SoapBox.Core.Services.Host.ExtensionService, typeof(IExtensionService))]
        private IExtensionService extensionService { get; set; }

        [ImportMany(ExtensionPoints.Workbench.ToolBars.SolutionToolBar, typeof(IToolBarItem), AllowRecomposition=true)]
        private IEnumerable<IToolBarItem> items { get; set; }

        public void OnImportsSatisfied()
        {
            Items = extensionService.Sort(items);
        }
    }

    [Export(ExtensionPoints.Workbench.ToolBars.SolutionToolBar, typeof(IToolBarItem))]
    public class SolutionToolBarNew : AbstractToolBarButton, IPartImportsSatisfiedNotification
    {
        public SolutionToolBarNew()
        {
            ID = Extensions.Workbench.ToolBars.SolutionToolBar.New;
            ToolTip = Resources.Strings.Solution_Command_Tooltip_New;
            SetIconFromBitmap(Resources.Images.Solution_Command_New);
        }

        public void OnImportsSatisfied()
        {
            EnableCondition = solutionService.NewEnabled;
        }

        [Import(Services.Solution.SolutionService, typeof(ISolutionService))]
        private ISolutionService solutionService { get; set; }

        protected override void Run()
        {
            solutionService.NewExecute();
        }
    }

    [Export(ExtensionPoints.Workbench.ToolBars.SolutionToolBar, typeof(IToolBarItem))]
    public class SolutionToolBarOpen : AbstractToolBarButton, IPartImportsSatisfiedNotification
    {
        public SolutionToolBarOpen()
        {
            ID = Extensions.Workbench.ToolBars.SolutionToolBar.Open;
            ToolTip = Resources.Strings.Solution_Command_Tooltip_Open;
            SetIconFromBitmap(Resources.Images.Solution_Command_Open);
            InsertRelativeToID = Extensions.Workbench.ToolBars.SolutionToolBar.New;
            BeforeOrAfter = RelativeDirection.After;
        }

        public void OnImportsSatisfied()
        {
            EnableCondition = solutionService.OpenEnabled;
        }

        [Import(Services.Solution.SolutionService, typeof(ISolutionService))]
        private ISolutionService solutionService { get; set; }

        protected override void Run()
        {
            solutionService.OpenExecute();
        }
    }

    [Export(ExtensionPoints.Workbench.ToolBars.SolutionToolBar, typeof(IToolBarItem))]
    public class SolutionToolBarSave : AbstractToolBarButton, IPartImportsSatisfiedNotification
    {
        public SolutionToolBarSave()
        {
            ID = Extensions.Workbench.ToolBars.SolutionToolBar.Save;
            ToolTip = Resources.Strings.Solution_Command_Tooltip_Save;
            SetIconFromBitmap(Resources.Images.Solution_Command_Save);
            InsertRelativeToID = Extensions.Workbench.ToolBars.SolutionToolBar.Open;
            BeforeOrAfter = RelativeDirection.After;
        }

        public void OnImportsSatisfied()
        {
            EnableCondition = solutionService.SaveEnabled;
        }

        [Import(Services.Solution.SolutionService, typeof(ISolutionService))]
        private ISolutionService solutionService { get; set; }

        protected override void Run()
        {
            solutionService.SaveExecute();
        }
    }

}
