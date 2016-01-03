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

namespace SoapBox.Snap.Application
{
    [Export(SoapBox.Core.ExtensionPoints.Workbench.MainMenu.HelpMenu, typeof(IMenuItem))]
    class HelpMenuAbout : AbstractMenuItem
    {
        public HelpMenuAbout()
        {
            ID = Extensions.Workbench.MainMenu.HelpMenu.About;
            Header = Resources.Strings.Workbench_MainMenu_Help_About;
            ToolTip = Resources.Strings.Workbench_Command_Tooltip_About;
            SetIconFromBitmap(Resources.Images.Workbench_MainMenu_Help_About);
        }

        [Import(SoapBox.Core.CompositionPoints.Host.MainWindow, typeof(Window))]
        private Lazy<Window> mainWindowExport { get; set; }

        protected override void Run()
        {
            Window mainWindow = mainWindowExport.Value;
            Window aboutWindow = new AboutBoxView();
            aboutWindow.Owner = mainWindow;
            aboutWindow.ShowDialog();
        }
    }

    [Export(SoapBox.Core.ExtensionPoints.Workbench.MainMenu.HelpMenu, typeof(IMenuItem))]
    class HelpMenuSeparator : AbstractMenuItem
    {
        public HelpMenuSeparator()
        {
            ID = Extensions.Workbench.MainMenu.HelpMenu.AboutSeparator;
            IsSeparator = true;
            InsertRelativeToID = Extensions.Workbench.MainMenu.HelpMenu.About;
            BeforeOrAfter = RelativeDirection.Before;
        }
    }

}
