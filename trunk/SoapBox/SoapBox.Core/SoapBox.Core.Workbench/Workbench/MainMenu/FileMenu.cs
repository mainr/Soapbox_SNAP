#region "SoapBox.Core License"
/// <header module="SoapBox.Core"> 
/// Copyright (C) 2009 SoapBox Automation Inc., All Rights Reserved.
/// Contact: SoapBox Automation Licencing (license@soapboxautomation.com)
/// 
/// This file is part of SoapBox Core.
/// 
/// Commercial Usage
/// Licensees holding valid SoapBox Automation Commercial licenses may use  
/// this file in accordance with the SoapBox Automation Commercial License
/// Agreement provided with the Software or, alternatively, in accordance 
/// with the terms contained in a written agreement between you and
/// SoapBox Automation Inc.
/// 
/// GNU Lesser General Public License Usage
/// SoapBox Core is free software: you can redistribute it and/or modify 
/// it under the terms of the GNU Lesser General Public License
/// as published by the Free Software Foundation, either version 3 of the
/// License, or (at your option) any later version.
/// 
/// SoapBox Core is distributed in the hope that it will be useful, 
/// but WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
/// GNU Lesser General Public License for more details.
/// 
/// You should have received a copy of the GNU Lesser General Public License 
/// along with SoapBox Core. If not, see <http://www.gnu.org/licenses/>.
/// </header>
#endregion
        
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using SoapBox.Core;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Media;

namespace SoapBox.Core.Workbench
{
    [Export(ExtensionPoints.Workbench.MainMenu.Self, typeof(IMenuItem))]
    class FileMenu : AbstractMenuItem, IPartImportsSatisfiedNotification
    {
        public FileMenu()
        {
            ID = Extensions.Workbench.MainMenu.File;
            Header = Resources.Strings.Workbench_MainMenu_File;
        }

        [Import(Services.Host.ExtensionService)] 
        private IExtensionService extensionService { get; set; }

        [ImportMany(ExtensionPoints.Workbench.MainMenu.FileMenu, typeof(IMenuItem), AllowRecomposition=true)] 
        private IEnumerable<IMenuItem> menu { get; set; }

        public void OnImportsSatisfied()
        {
            Items = extensionService.Sort(menu);
        }
    }

    [Export(ExtensionPoints.Workbench.MainMenu.FileMenu, typeof(IMenuItem))]
    class FileMenuExit : AbstractMenuItem
    {
        public FileMenuExit()
        {
            ID = Extensions.Workbench.MainMenu.FileMenu.Exit;
            Header = Resources.Strings.Workbench_MainMenu_File_Exit;
            ToolTip = Resources.Strings.Workbench_Command_Tooltip_Exit;
            SetIconFromBitmap(Resources.Images.Workbench_Command_Exit);
        }

        // This has to be a Lazy import because we're instantiated 
        // when the Workbench itself is instantiated, so the Workbench
        // isn't finished being constructed yet.
        [Import(CompositionPoints.Host.MainWindow, typeof(Window))]
        private Lazy<Window> mainWindowExport { get; set; }

        protected override void Run()
        {
            Window mainWindow = mainWindowExport.Value;
            mainWindow.Close();
        }
    }
}

