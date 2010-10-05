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
using SoapBox.Utilities;

namespace SoapBox.Core.Options
{
    /// <summary>
    /// Adds the Options Dialog to the tools menu
    /// </summary>
    [Export(ExtensionPoints.Workbench.MainMenu.ToolsMenu, typeof(IMenuItem))]
    class ToolsMenuOptions : AbstractMenuItem 
    {
        public ToolsMenuOptions()
        {
            ID = Extensions.Workbench.MainMenu.ToolsMenu.Options;
            Header = Resources.Strings.Workbench_MainMenu_Tools_Options;
            SetIconFromBitmap(Resources.Images.Workbench_MainMenu_ToolsMenu_Options);
        }

        [Import(CompositionPoints.Options.DefaultOptionsDialog, typeof(IOptionsDialog))]
        private IOptionsDialog defaultOptionsDialog { get; set; }

        [Import(ExtensionPoints.Options.OptionsDialog_, typeof(IOptionsDialog),
            AllowDefault = true)]
        private IOptionsDialog optionsDialog { get; set; }

        [Import(ExtensionPoints.Options.OptionsDialog.View, typeof(IFactory<Window>), 
            AllowRecomposition=true, AllowDefault=true)]
        private IFactory<Window> optionsDialogView { get; set; }

        protected override void Run()
        {
            var optionsDialogToUse = optionsDialog ?? defaultOptionsDialog;
            optionsDialogToUse.ShowDialog(optionsDialogView != null ? optionsDialogView.Manufacture() : new OptionsDialogView());
        }
    }
}
