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
    [Export(SoapBox.Core.ExtensionPoints.Workbench.MainMenu.FileMenu, typeof(IMenuItem))]
    class FileMenuNew : AbstractMenuItem, IPartImportsSatisfiedNotification
    {
        public FileMenuNew()
        {
            ID = Extensions.Workbench.MainMenu.FileMenu.New;
            Header = Resources.Strings.Workbench_MainMenu_File_New;
            ToolTip = Resources.Strings.Solution_Command_Tooltip_New;
            InsertRelativeToID = Extensions.Workbench.MainMenu.FileMenu.ExitSeparator;
            BeforeOrAfter = RelativeDirection.Before;
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

    [Export(SoapBox.Core.ExtensionPoints.Workbench.MainMenu.FileMenu, typeof(IMenuItem))]
    class FileMenuOpen : AbstractMenuItem, IPartImportsSatisfiedNotification
    {
        public FileMenuOpen()
        {
            ID = Extensions.Workbench.MainMenu.FileMenu.Open;
            Header = Resources.Strings.Workbench_MainMenu_File_Open;
            ToolTip = Resources.Strings.Solution_Command_Tooltip_Open;
            InsertRelativeToID = Extensions.Workbench.MainMenu.FileMenu.New;
            BeforeOrAfter = RelativeDirection.After;
            SetIconFromBitmap(Resources.Images.Solution_Command_Open);
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

    [Export(SoapBox.Core.ExtensionPoints.Workbench.MainMenu.FileMenu, typeof(IMenuItem))]
    class FileMenuClose : AbstractMenuItem, IPartImportsSatisfiedNotification
    {
        public FileMenuClose()
        {
            ID = Extensions.Workbench.MainMenu.FileMenu.Close;
            Header = Resources.Strings.Workbench_MainMenu_File_Close;
            ToolTip = Resources.Strings.Solution_Command_Tooltip_Close;
            InsertRelativeToID = Extensions.Workbench.MainMenu.FileMenu.Open;
            BeforeOrAfter = RelativeDirection.After;
        }

        public void OnImportsSatisfied()
        {
            EnableCondition = solutionService.CloseEnabled;
        }

        [Import(Services.Solution.SolutionService, typeof(ISolutionService))]
        private ISolutionService solutionService { get; set; }

        protected override void Run()
        {
            solutionService.CloseExecute();
        }
    }

    [Export(SoapBox.Core.ExtensionPoints.Workbench.MainMenu.FileMenu, typeof(IMenuItem))]
    class FileMenuCloseSeparator : AbstractMenuItem
    {
        public FileMenuCloseSeparator()
        {
            ID = Extensions.Workbench.MainMenu.FileMenu.CloseSeparator;
            InsertRelativeToID = Extensions.Workbench.MainMenu.FileMenu.Close;
            BeforeOrAfter = RelativeDirection.After;
            IsSeparator = true;
        }
    }

    [Export(SoapBox.Core.ExtensionPoints.Workbench.MainMenu.FileMenu, typeof(IMenuItem))]
    class FileMenuSave : AbstractMenuItem, IPartImportsSatisfiedNotification
    {
        public FileMenuSave()
        {
            ID = Extensions.Workbench.MainMenu.FileMenu.Save;
            Header = Resources.Strings.Workbench_MainMenu_File_Save;
            ToolTip = Resources.Strings.Solution_Command_Tooltip_Save;
            InsertRelativeToID = Extensions.Workbench.MainMenu.FileMenu.CloseSeparator;
            BeforeOrAfter = RelativeDirection.After;
            SetIconFromBitmap(Resources.Images.Solution_Command_Save);
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

    [Export(SoapBox.Core.ExtensionPoints.Workbench.MainMenu.FileMenu, typeof(IMenuItem))]
    class FileMenuSaveAs : AbstractMenuItem, IPartImportsSatisfiedNotification
    {
        public FileMenuSaveAs()
        {
            ID = Extensions.Workbench.MainMenu.FileMenu.SaveAs;
            Header = Resources.Strings.Workbench_MainMenu_File_SaveAs;
            ToolTip = Resources.Strings.Solution_Command_Tooltip_SaveAs;
            InsertRelativeToID = Extensions.Workbench.MainMenu.FileMenu.Save;
            BeforeOrAfter = RelativeDirection.After;
        }

        public void OnImportsSatisfied()
        {
            EnableCondition = solutionService.SaveAsEnabled;
        }

        [Import(Services.Solution.SolutionService, typeof(ISolutionService))]
        private ISolutionService solutionService { get; set; }

        protected override void Run()
        {
            solutionService.SaveAsExecute();
        }
    }

    [Export(SoapBox.Core.ExtensionPoints.Workbench.MainMenu.FileMenu, typeof(IMenuItem))]
    class FileMenuExitSeparator : AbstractMenuItem
    {
        public FileMenuExitSeparator()
        {
            ID = Extensions.Workbench.MainMenu.FileMenu.ExitSeparator;
            InsertRelativeToID = SoapBox.Core.Extensions.Workbench.MainMenu.FileMenu.Exit;
            BeforeOrAfter = RelativeDirection.Before;
            IsSeparator = true;
        }
    }

}
