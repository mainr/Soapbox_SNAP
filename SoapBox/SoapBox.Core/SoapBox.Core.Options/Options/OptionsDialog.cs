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
using System.Windows;
using System.ComponentModel.Composition;

namespace SoapBox.Core.Options
{
    /// <summary>
    /// ViewModel for the Options Dialog
    /// Inheriting from AbstractOptionsItem is just a hack
    /// because it makes it easy to set the Items property.
    /// </summary>
    [Export(CompositionPoints.Options.DefaultOptionsDialog, typeof(IOptionsDialog))]
    class OptionsDialog : AbstractOptionsItem, IOptionsDialog, IPartImportsSatisfiedNotification
    {
        public OptionsDialog()
        {
            OptionChanged += new EventHandler(OptionsDialog_OptionChanged);
        }

        void OptionsDialog_OptionChanged(object sender, EventArgs e)
        {
            m_dirtyCondition.SetCondition(true);
        }

        [Import(Services.Host.ExtensionService)] 
        private IExtensionService extensionService { get; set; }

        [ImportMany(ExtensionPoints.Options.OptionsDialog.OptionsItems, typeof(IOptionsItem), AllowRecomposition=true)] 
        private IEnumerable<IOptionsItem> items { get; set; }

        public void OnImportsSatisfied()
        {
            Items = extensionService.Sort(items);
        }

        [Import(CompositionPoints.Host.MainWindow)]
        private Lazy<Window> mainWindowExport { get; set; }

        /// <summary>
        /// Displays the Options Dialog as modal
        /// </summary>
        public void ShowDialog(Window optionsDialogView)
        {
            Window mainWindow = mainWindowExport.Value;
            optionsDialogView.Owner = mainWindow;
            optionsDialogView.DataContext = this;
            logger.Info("Showing options dialog...");
            m_dirtyCondition.SetCondition(false);
            optionsDialogView.ShowDialog();
            ReloadSavedValues();
            logger.Info("Options dialog closed.");
        }

        private ICondition dirtyCondition
        {
            get
            {
                return m_dirtyCondition;
            }
        }
        private ConcreteCondition m_dirtyCondition = new ConcreteCondition(false);

        #region " OK Button "
        public IControl OKButton
        {
            get
            {
                if (m_OKButton == null)
                {
                    m_OKButton = new CommitChangesButton(this);
                }
                return m_OKButton;
            }
        }
        private IControl m_OKButton = null;

        private class CommitChangesButton : AbstractButton
        {
            public CommitChangesButton(OptionsDialog dlg)
            {
                m_OptionsDialog = dlg;
                EnableCondition = dlg.dirtyCondition;
            }

            private OptionsDialog m_OptionsDialog = null;

            protected override void Run()
            {
                foreach (IOptionsItem item in m_OptionsDialog.Items)
                {
                    item.CommitChanges();
                }
            }
        }
        #endregion

        #region " Cancel Button "
        public IControl CancelButton
        {
            get
            {
                if (m_CancelButton == null)
                {
                    m_CancelButton = new CancelChangesButton(this);
                }
                return m_CancelButton;
            }
        }
        private IControl m_CancelButton = null;

        private class CancelChangesButton : AbstractButton
        {
            public CancelChangesButton(OptionsDialog dlg)
            {
                m_OptionsDialog = dlg;
            }

            private OptionsDialog m_OptionsDialog = null;

            protected override void Run()
            {
                foreach (IOptionsItem item in m_OptionsDialog.Items)
                {
                    item.CancelChanges();
                }
            }
        }
        #endregion

        private void ReloadSavedValues()
        {
            foreach (IOptionsItem item in Items)
            {
                item.CancelChanges();
            }
        }

    }
}
