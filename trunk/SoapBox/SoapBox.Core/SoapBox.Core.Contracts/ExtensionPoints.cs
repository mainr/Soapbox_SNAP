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

namespace SoapBox.Core.ExtensionPoints
{
    public static class Host
    {
        public const string Styles = "ExtensionPoints.Host.Styles";
        public const string Views = "ExtensionPoints.Host.Views";
        public const string StartupCommands = "ExtensionPoints.Host.StartupCommands";
        public const string ShutdownCommands = "ExtensionPoints.Host.ShutdownCommands";
        public const string Void = "ExtensionPoints.Host.Void";
    }
    public static class Workbench
    {
        public const string ToolBars = "ExtensionPoints.Workbench.ToolBars";
        public const string StatusBar = "ExtensionPoints.Workbench.StatusBar";
        public const string Pads = "ExtensionPoints.Workbench.Pads";
        public const string Documents = "ExtensionPoints.Workbench.Documents";

        public static class MainMenu
        {
            public const string Self = "ExtensionPoints.Workbench.MainMenu";
            public const string FileMenu = "ExtensionPoints.Workbench.MainMenu.FileMenu";
            public const string EditMenu = "ExtensionPoints.Workbench.MainMenu.EditMenu";
            public const string ViewMenu = "ExtensionPoints.Workbench.MainMenu.ViewMenu";
            public const string ToolsMenu = "ExtensionPoints.Workbench.MainMenu.ToolsMenu";
            public const string WindowMenu = "ExtensionPoints.Workbench.MainMenu.WindowMenu";
            public const string HelpMenu = "ExtensionPoints.Workbench.MainMenu.HelpMenu";
        }
    }

    public static class Options
    {
        public const string OptionsDialog_ = "ExtensionPoints.Options.OptionsDialog_";
        public static class OptionsDialog
        {
            public const string OptionsItems = "ExtensionPoints.Options.OptionsDialog.OptionsItems";
            public const string View = "ExtensionPoints.Options.OptionsDialog.View";
        }
    }
}
