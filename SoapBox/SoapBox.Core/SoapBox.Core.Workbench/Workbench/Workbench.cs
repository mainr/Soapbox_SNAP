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
using System.Windows.Media.Imaging;
using System.IO;
using System.ComponentModel;
using SoapBox.Utilities;

namespace SoapBox.Core.Workbench
{
    [Export(CompositionPoints.Workbench.ViewModel)]
    public class Workbench : AbstractViewModel, IPartImportsSatisfiedNotification
    {
        [Import(Services.Host.ExtensionService)]
        private IExtensionService extensionService { get; set; }

        [Import(Services.Layout.LayoutManager, typeof(ILayoutManager))]
        public ILayoutManager LayoutManager { get; set; }

        [ImportMany(ExtensionPoints.Workbench.MainMenu.Self, typeof(IMenuItem), AllowRecomposition = true)]
        private IEnumerable<IMenuItem> menu { get; set; }

        [ImportMany(ExtensionPoints.Workbench.ToolBars, typeof(IToolBar), AllowRecomposition = true)]
        private IEnumerable<IToolBar> toolBars { get; set; }

        [ImportMany(ExtensionPoints.Workbench.StatusBar, typeof(IStatusBarItem), AllowRecomposition=true)]
        private IEnumerable<IStatusBarItem> statusBar { get; set; }

        [ImportMany(ExtensionPoints.Workbench.Pads, typeof(IPad), AllowRecomposition = true)]
        private IEnumerable<Lazy<IPad,IPadMeta>> pads { get; set; }

        [ImportMany(ExtensionPoints.Workbench.Documents, typeof(IDocument), AllowRecomposition = true)]
        private IEnumerable<Lazy<IDocument,IDocumentMeta>> documents { get; set; }

        public void OnImportsSatisfied()
        {
            // when this is called, all imports that could be satisfied have been satisfied.
            MainMenu = extensionService.Sort(menu);
            StatusBar = extensionService.Sort(statusBar);
            ToolBars = extensionService.Sort(toolBars);
            LayoutManager.SetAllPadsDocuments(pads, documents);
        }

        #region "MainMenu"

        public IEnumerable<IMenuItem> MainMenu
        {
            get
            {
                return m_MainMenu;
            }
            private set
            {
                if (m_MainMenu != value)
                {
                    m_MainMenu = value;
                    NotifyPropertyChanged(m_MainMenuArgs);
                }
            }
        }
        private IEnumerable<IMenuItem> m_MainMenu = null;
        static readonly PropertyChangedEventArgs m_MainMenuArgs =
            NotifyPropertyChangedHelper.CreateArgs<Workbench>(o => o.MainMenu);

        #endregion

        #region "StatusBar"

        public IEnumerable<IStatusBarItem> StatusBar
        {
            get
            {
                return m_StatusBar;
            }
            private set
            {
                if (m_StatusBar != value)
                {
                    m_StatusBar = value;
                    NotifyPropertyChanged(m_StatusBarArgs);
                }
            }
        }
        private IEnumerable<IStatusBarItem> m_StatusBar = null;
        static readonly PropertyChangedEventArgs m_StatusBarArgs =
            NotifyPropertyChangedHelper.CreateArgs<Workbench>(o => o.StatusBar);

        #endregion

        #region "ToolBars"

        public IEnumerable<IToolBar> ToolBars
        {
            get
            {
                return m_ToolBars;
            }
            set
            {
                if (m_ToolBars != value)
                {
                    m_ToolBars = value;
                    NotifyPropertyChanged(m_ToolBarsArgs);
                }
            }
        }
        private IEnumerable<IToolBar> m_ToolBars = null;
        static readonly PropertyChangedEventArgs m_ToolBarsArgs =
            NotifyPropertyChangedHelper.CreateArgs<Workbench>(o => o.ToolBars);

        #endregion

        public void OnClosing(object sender, EventArgs e)
        {
            logger.Info("Workbench closing.");
            LayoutManager.UnloadingWorkbench();
        }

        public void OnClosed(object sender, EventArgs e)
        {
            logger.Info("Workbench closed.");
        }

    }
}
