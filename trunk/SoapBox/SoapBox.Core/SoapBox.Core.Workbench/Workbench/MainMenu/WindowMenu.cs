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
    class WindowMenu : AbstractMenuItem, IPartImportsSatisfiedNotification
    {
        public WindowMenu()
        {
            ID = Extensions.Workbench.MainMenu.Window;
            Header = Resources.Strings.Workbench_MainMenu_Window;
            InsertRelativeToID = Extensions.Workbench.MainMenu.Tools;
            BeforeOrAfter = RelativeDirection.After;
        }

        [Import(Services.Host.ExtensionService)]
        private IExtensionService extensionService { get; set; }

        [Import(Services.Layout.LayoutManager)]
        private ILayoutManager layoutManager { get; set; }

        [ImportMany(ExtensionPoints.Workbench.MainMenu.WindowMenu, typeof(IMenuItem), AllowRecomposition = true)] 
        private IEnumerable<IMenuItem> menu { get; set; }

        public void OnImportsSatisfied()
        {
            Items = extensionService.Sort(menu);
        }

        protected override void OnIsSubmenuOpenChanged()
        {
            if (IsSubmenuOpen)
            {
                var newItems = new List<IMenuItem>();
                string previousId = Guid.NewGuid().ToString();
                foreach (IDocument doc in layoutManager.Documents)
                {
                    var docMenuItem = new DocumentMenuItem(doc, previousId, layoutManager);
                    previousId = docMenuItem.ID;
                    newItems.Add(docMenuItem);
                }
                Items = extensionService.SortAndJoin(menu, new ConcreteMenuItemSeparator(), newItems);
            }
        }
    }

    [Export(ExtensionPoints.Workbench.MainMenu.WindowMenu, typeof(IMenuItem))]
    class CloseAllDocuments : AbstractMenuItem
    {
        public CloseAllDocuments()
        {
            Header = Resources.Strings.Workbench_MainMenu_Window_CloseAllDocuments;
            ToolTip = Resources.Strings.Workbench_MainMenu_Window_CloseAllDocuments_ToolTip;
            ID = Extensions.Workbench.MainMenu.WindowMenu.CloseAllDocuments;
        }

        [Import(Services.Layout.LayoutManager)]
        private ILayoutManager layoutManager { get; set; }

        protected override void Run()
        {
            layoutManager.CloseAllDocuments();
        }
    }

    /// <summary>
    /// This is a wrapper class for a document so that it
    /// shows up in the Window menu, and so that clicking
    /// the menu item brings the document to the front.
    /// </summary>
    class DocumentMenuItem : AbstractMenuItem
    {
        public DocumentMenuItem(IDocument doc, string previousId, ILayoutManager layoutManager)
        {
            m_doc = doc;
            Header = doc.Title;
            ID = Guid.NewGuid().ToString();
            InsertRelativeToID = previousId;
            BeforeOrAfter = RelativeDirection.After;
            if (layoutManager.IsActive(doc))
            {
                IsChecked = true;
            }
            m_layoutManager = layoutManager;
        }

        private readonly IDocument m_doc = null;
        private readonly ILayoutManager m_layoutManager = null;

        protected override void Run()
        {
            m_layoutManager.ShowDocument(m_doc, m_doc.Memento, true);
        }
    }
}
