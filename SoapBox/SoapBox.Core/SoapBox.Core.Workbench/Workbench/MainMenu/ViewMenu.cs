#region "SoapBox.Core License"
/// <header module="SoapBox.Core"> 
/// Copyright (C) 2009 SoapBox Automation, All Rights Reserved.
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

namespace SoapBox.Core.Workbench
{
    [Export(ExtensionPoints.Workbench.MainMenu.Self, typeof(IMenuItem))]
    class ViewMenu : AbstractMenuItem, IPartImportsSatisfiedNotification
    {
        public ViewMenu()
        {
            ID = Extensions.Workbench.MainMenu.View;
            Header = Resources.Strings.Workbench_MainMenu_View;
            InsertRelativeToID = Extensions.Workbench.MainMenu.Edit;
            BeforeOrAfter = RelativeDirection.After;
        }

        [Import(Services.Host.ExtensionService)]
        private IExtensionService extensionService { get; set; }

        [ImportMany(ExtensionPoints.Workbench.MainMenu.ViewMenu, typeof(IMenuItem), AllowRecomposition=true)]
        private IEnumerable<IMenuItem> menu { get; set; }

        public void OnImportsSatisfied()
        {
            Items = extensionService.Sort(menu);
        }
    }

    /// <summary>
    /// This is a wrapper for a toolbar so it can be controlled
    /// by a menu item in View->Toolbars
    /// </summary>
    class ToolBarMenuItem : AbstractMenuItem
    {
        public ToolBarMenuItem(IToolBar toolBar)
        {
            // store the toolbar and create a condition to 
            // control its visibility
            m_toolBar = toolBar;
            // preserve the existing visible state, in case it 
            // was set to true in the toolbar constructor
            m_toolBarVisibleCondition = new ConcreteCondition(m_toolBar.Visible);
            m_toolBar.VisibleCondition = m_toolBarVisibleCondition;

            ID = toolBar.ID;
            Header = toolBar.Name;
            InsertRelativeToID = toolBar.InsertRelativeToID;
            BeforeOrAfter = toolBar.BeforeOrAfter;
            IsCheckable = true;
            IsChecked = toolBar.Visible;
        }

        private readonly IToolBar m_toolBar = null;
        private readonly ConcreteCondition m_toolBarVisibleCondition;

        protected override void OnIsCheckedChanged()
        {
            m_toolBarVisibleCondition.SetCondition(IsChecked);
        }
    }

    [Export(ExtensionPoints.Workbench.MainMenu.ViewMenu, typeof(IMenuItem))]
    class ViewMenuToolBars : AbstractMenuItem, IPartImportsSatisfiedNotification
    {
        public ViewMenuToolBars()
        {
            ID = Extensions.Workbench.MainMenu.ViewMenu.ToolBars;
            Header = Resources.Strings.Workbench_MainMenu_View_ToolBars;
        }

        [Import(Services.Host.ExtensionService)] 
        private IExtensionService extensionService { get; set; }

        [ImportMany(ExtensionPoints.Workbench.ToolBars, typeof(IToolBar), AllowRecomposition=true)] 
        private IEnumerable<IToolBar> toolBars { get; set; }

        public void OnImportsSatisfied()
        {
            // have to convert the ToolBars into MenuItems
            IList<IToolBar> sortedToolBars = extensionService.Sort(toolBars);
            List<IMenuItem> toolBarMenuItems = new List<IMenuItem>();
            foreach (IToolBar tb in sortedToolBars)
            {
                toolBarMenuItems.Add(new ToolBarMenuItem(tb));
            }
            Items = extensionService.Sort(toolBarMenuItems);
        }
    }

}
