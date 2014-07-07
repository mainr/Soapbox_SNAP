#region "SoapBox.Snap License"
/// <header module="SoapBox.Snap"> 
/// Copyright (C) 2009-2014 SoapBox Automation, All Rights Reserved.
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
using System.Windows;
using System.ComponentModel.Composition;
using SoapBox.Core;
using SoapBox.Protocol.Base;
using System.ComponentModel;
using SoapBox.Utilities;
using SoapBox.Protocol.Automation;
using System.Collections.ObjectModel;

namespace SoapBox.Snap.Application
{
    /// <summary>
    /// ViewModel for the UploadDownload Dialog
    /// </summary>
    [Export(CompositionPoints.Workbench.Dialogs.UploadDownloadDialog, typeof(UploadDownloadDialog))]
    public class UploadDownloadDialog : AbstractViewModel
    {
        public UploadDownloadDialog()
        {
        }

        [Import(SoapBox.Snap.Services.Solution.RuntimeService, typeof(IRuntimeService))]
        private IRuntimeService runtimeService { get; set; }

        [Import(SoapBox.Core.CompositionPoints.Host.MainWindow)]
        private Lazy<Window> mainWindowExport { get; set; }

        [Import(SoapBox.Core.Services.Layout.LayoutManager)]
        private ILayoutManager layoutManager { get; set; }

        /// <summary>
        /// Displays the Dialog as modal
        /// </summary>
        /// <returns>true if we are in sync</returns>
        public bool ShowDialog(RuntimeApplicationItem runtimeApplicationItem)
        {
            if (runtimeApplicationItem == null || runtimeApplicationItem.Runtime == null || runtimeApplicationItem.RuntimeApplication == null)
            {
                return false;
            }

            m_inSync = false;
            if (runtimeApplicationItem.Runtime.RuntimeId() != runtimeApplicationItem.RuntimeApplication.RuntimeId)
            {
                // not even the same application
                Message = Resources.Strings.UploadDownloadDialog_Message_ApplicationMismatch;
            }
            else if (runtimeApplicationItem.Runtime.RuntimeVersionId() != runtimeApplicationItem.RuntimeApplication.ID)
            {
                // same application but versions don't match
                Message = Resources.Strings.UploadDownloadDialog_Message_VersionMismatch;
            }
            else
            {
                throw new InvalidOperationException();
            }
            Window dlg = new UploadDownloadDialogView();
            dlg.Owner = mainWindowExport.Value;
            this.m_Upload = true; // default to upload
            dlg.DataContext = this;
            dlg.ShowDialog();
            if (m_inSync)
            {
                // user clicked ok
                if (Upload)
                {
                    var rta = runtimeApplicationItem.Runtime.RuntimeApplicationUpload();
                    if (rta != null)
                    {
                        runtimeApplicationItem.RuntimeApplication = rta;
                        // after an upload we have to reset the whole tree
                        layoutManager.CloseAllDocuments();
                        runtimeApplicationItem.SetItems();
                    }
                    else
                    {
                        m_inSync = false;
                    }
                }
                else // download
                {
                    m_inSync = runtimeApplicationItem.Runtime
                        .RuntimeApplicationDownload(runtimeApplicationItem.RuntimeApplication, onlineChange: false);
                }
            }
            return m_inSync;
        }

        private bool m_inSync = false;

        #region " Message "
        public string Message
        {
            get
            {
                return m_Message;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(m_MessageName);
                }
                if (m_Message != value)
                {
                    m_Message = value;
                    NotifyPropertyChanged(m_MessageArgs);
                }
            }
        }
        private string m_Message = string.Empty;
        private static readonly PropertyChangedEventArgs m_MessageArgs =
            NotifyPropertyChangedHelper.CreateArgs<UploadDownloadDialog>(o => o.Message);
        private static string m_MessageName =
            NotifyPropertyChangedHelper.GetPropertyName<UploadDownloadDialog>(o => o.Message);
        #endregion

        #region " Upload "
        public bool Upload
        {
            get
            {
                return m_Upload;
            }
            set
            {
                if (m_Upload != value)
                {
                    m_Upload = value;
                    NotifyPropertyChanged(m_UploadArgs);
                }
            }
        }
        private bool m_Upload = true;
        private static readonly PropertyChangedEventArgs m_UploadArgs =
            NotifyPropertyChangedHelper.CreateArgs<UploadDownloadDialog>(o => o.Upload);
        private static string m_UploadName =
            NotifyPropertyChangedHelper.GetPropertyName<UploadDownloadDialog>(o => o.Upload);
        #endregion

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
            public CommitChangesButton(UploadDownloadDialog dlg)
            {
                m_UploadDownloadDialog = dlg;
            }

            private UploadDownloadDialog m_UploadDownloadDialog = null;

            protected override void Run()
            {
                m_UploadDownloadDialog.m_inSync = true;
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
            public CancelChangesButton(UploadDownloadDialog dlg)
            {
                m_UploadDownloadDialog = dlg;
            }

            private UploadDownloadDialog m_UploadDownloadDialog = null;

            protected override void Run()
            {
                m_UploadDownloadDialog.m_inSync = false;
            }
        }
        #endregion

    }
}
