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
using SoapBox.Protocol.Automation;
using SoapBox.Core;
using System.ComponentModel;
using System.ComponentModel.Composition;
using SoapBox.Protocol.Base;
using SoapBox.Utilities;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Threading;

namespace SoapBox.Snap.Application
{
    [Export(ExtensionPoints.Workbench.Pads.SolutionPad.SolutionItems, typeof(ISolutionItem))]
    public class RuntimeApplicationItem : AbstractSolutionItem
    {
        /// <summary>
        /// Called by MEF (just so we can do imports)
        /// </summary>
        private RuntimeApplicationItem()
            : base(null, string.Empty)
        {
        }

        public RuntimeApplicationItem(ISolutionItem parent, NodeRuntimeApplication runtimeApplication)
            : base(parent, runtimeApplication.Code.ToString())
        {
            ContextMenu = extensionService.Sort(contextMenu);
            RuntimeApplication = runtimeApplication;
            SetItems();

            SetIconFromBitmap(Resources.Images.Disconnected);
        }

        #region "runtimeApplicationPropertiesSingleton"
        [Import(CompositionPoints.Workbench.Documents.RuntimeApplicationProperties, typeof(RuntimeApplicationProperties))]
        private Lazy<RuntimeApplicationProperties> runtimeApplicationProperties
        {
            get
            {
                return runtimeApplicationPropertiesSingleton;
            }
            set
            {
                runtimeApplicationPropertiesSingleton = value;
            }
        }
        private static Lazy<RuntimeApplicationProperties> runtimeApplicationPropertiesSingleton = null;
        #endregion

        #region "uploadDownloadDialogSingleton"
        [Import(CompositionPoints.Workbench.Dialogs.UploadDownloadDialog, typeof(UploadDownloadDialog))]
        private Lazy<UploadDownloadDialog> uploadDownloadDialog
        {
            get
            {
                return uploadDownloadDialogSingleton;
            }
            set
            {
                uploadDownloadDialogSingleton = value;
            }
        }
        private static Lazy<UploadDownloadDialog> uploadDownloadDialogSingleton = null;
        #endregion

        #region "contextMenuSingleton"
        [ImportMany(ExtensionPoints.Workbench.Pads.SolutionPad.RuntimeApplicationItem.ContextMenu, typeof(IMenuItem))]
        private IEnumerable<IMenuItem> contextMenu
        {
            get
            {
                return contextMenuSingleton;
            }
            set
            {
                contextMenuSingleton = value;
            }
        }
        private static IEnumerable<IMenuItem> contextMenuSingleton = null;
        #endregion

        #region "solutionServiceSingleton"
        [Import(Services.Solution.SolutionService, typeof(ISolutionService))]
        private Lazy<ISolutionService> solutionService
        {
            get
            {
                return solutionServiceSingleton;
            }
            set
            {
                solutionServiceSingleton = value;
            }
        }
        private static Lazy<ISolutionService> solutionServiceSingleton = null;
        #endregion

        #region "RuntimeApplication"

        public NodeRuntimeApplication RuntimeApplication
        {
            get
            {
                return Node as NodeRuntimeApplication;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(m_RuntimeApplicationName);
                }
                Node = value;
                Header = value.Code.ToString();
                if (Connected)
                {
                    Runtime.RuntimeApplicationDownload(value, onlineChange: true);
                }
                NotifyPropertyChanged(m_RuntimeApplicationArgs);
            }
        }
        private static PropertyChangedEventArgs m_RuntimeApplicationArgs
            = NotifyPropertyChangedHelper.CreateArgs<RuntimeApplicationItem>(o => o.RuntimeApplication);
        private static string m_RuntimeApplicationName
            = NotifyPropertyChangedHelper.GetPropertyName<RuntimeApplicationItem>(o => o.RuntimeApplication);

        #endregion

        public override void Open()
        {
            base.Open();
            layoutManager.Value.ShowDocument(
                runtimeApplicationProperties.Value,
                RuntimeApplication.RuntimeId.ToString());
        }

        #region Connected

        public bool Connected
        {
            get
            {
                bool retVal;
                lock (m_Connected_Lock)
                {
                    retVal = m_Connected;
                }
                return retVal;
            }
            private set
            {
                lock (m_Connected_Lock)
                {
                    if (m_Connected != value)
                    {
                        m_Connected = value;
                        if (m_Connected)
                        {
                            SetIconFromBitmap(Resources.Images.Connected);
                            if (Runtime.Running)
                            {
                                SetIcon2FromBitmap(Resources.Images.RuntimeRunning);
                            }
                            else
                            {
                                SetIcon2FromBitmap(Resources.Images.RuntimeStopped);
                            }
                        }
                        else
                        {
                            Runtime.Disconnect();
                            SetIconFromBitmap(Resources.Images.Disconnected);
                            Icon2 = null;
                        }
                        NotifyPropertyChanged(m_ConnectedArgs);
                    }
                }
            }
        }
        private bool m_Connected = false;
        private readonly object m_Connected_Lock = new object();
        private static PropertyChangedEventArgs m_ConnectedArgs =
            NotifyPropertyChangedHelper.CreateArgs<RuntimeApplicationItem>(o => o.Connected);

        #endregion

        #region Runtime

        public IRuntime Runtime
        {
            get
            {
                IRuntime rt = null;
                lock (m_Runtime_Lock)
                {
                    rt = m_Runtime;
                }
                return rt;
            }
            private set
            {
                lock (m_Runtime_Lock)
                {
                    if (m_Runtime != value)
                    {
                        m_Runtime = value;
                        NotifyPropertyChanged(m_RuntimeArgs);
                    }
                }
            }
        }
        private IRuntime m_Runtime = null;
        private readonly object m_Runtime_Lock = new object();
        private static PropertyChangedEventArgs m_RuntimeArgs =
            NotifyPropertyChangedHelper.CreateArgs<RuntimeApplicationItem>(o => o.Runtime);

        #endregion

        private void hookupDisconnectedEvent(IRuntimeType rt)
        {
            if (rt != null && !m_hookedUpDisconnectedEvent)
            {
                var d = Dispatcher.CurrentDispatcher; // capture the dispatcher outside the callback
                rt.Disconnected += new EventHandler((s, e) =>
                {
                    d.BeginInvoke((Action)(() => Disconnect()));
                });
                m_hookedUpDisconnectedEvent = true;
            }
        }
        private bool m_hookedUpDisconnectedEvent = false;

        public void Connect()
        {
            Connect(false);
        }

        public void Connect(bool downloadIfNoApplication)
        {
            foreach (var rt in runtimeApplicationProperties.Value.RuntimeTypes)
            {
                if (new FieldGuid(rt.TypeId) == RuntimeApplication.TypeId)
                {
                    Runtime = rt.OpenRuntime(RuntimeApplication);
                    hookupDisconnectedEvent(rt);
                    break;
                }
            }
            if (Runtime != null)
            {
                // Can't connect unless we're saved (user might have unsaved changes in a page window that aren't saved up the tree yet)
                if (solutionService.Value.SaveEnabled.Condition)
                {
                    if (messagingService.Value.ShowDialog(Resources.Strings.Solution_Pad_RuntimeApplicationItem_Save,
                        Resources.Strings.Solution_Pad_RuntimeApplicationItem_SaveTitle,
                        System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                    {
                        solutionService.Value.SaveExecute();
                        if (solutionService.Value.SaveEnabled.Condition)
                        {
                            // still didn't save, so assume it's a cancel
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }

                // When connecting, we have to get "in sync" with the version on the runtime.
                // If we can't get in sync, we disconnect.
                bool inSync = false;
                if (Runtime.RuntimeId() == null)
                {
                    // Runtime has no app, so ask the user if they want to download
                    if (downloadIfNoApplication ||
                        messagingService.Value.ShowDialog(Resources.Strings.Solution_Pad_RuntimeApplicationItem_NoApplication,
                        Resources.Strings.Solution_Pad_RuntimeApplicationItem_DownloadTitle, 
                        System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                    {
                        inSync = Runtime.RuntimeApplicationDownload(RuntimeApplication, onlineChange: false);
                    }
                }
                else if (Runtime.RuntimeId() != RuntimeApplication.RuntimeId || Runtime.RuntimeVersionId() != RuntimeApplication.ID)
                {
                    // application mismatch - let user decide
                    inSync = uploadDownloadDialog.Value.ShowDialog(this);
                }
                else
                {
                    inSync = true;
                    messagingService.Value.ShowMessage(Resources.Strings.Solution_Pad_RuntimeApplicationItem_ConnectedMessage, 
                        Resources.Strings.Solution_Pad_RuntimeApplicationItem_ConnectedTitle);
                    this.Runtime.RuntimeApplicationGoOnline(RuntimeApplication);
                }

                if (!inSync)
                {
                    Disconnect();
                    messagingService.Value.ShowMessage(Resources.Strings.Solution_Pad_RuntimeApplicationItem_NotConnectedMessage,
                        Resources.Strings.Solution_Pad_RuntimeApplicationItem_NotConnectedTitle);
                }
                else
                {
                    Connected = true;
                }

            }
        }

        public void Disconnect()
        {
            Connected = false;
            Runtime = null;
        }

        public void Start()
        {
            if (Runtime != null && Connected && Runtime.Start())
            {
                SetIcon2FromBitmap(Resources.Images.RuntimeRunning);
            }
        }

        public void Stop()
        {
            if (Runtime != null && Connected && Runtime.Stop())
            {
                SetIcon2FromBitmap(Resources.Images.RuntimeStopped);
            }
        }

        public void SetItems()
        {
            var newCollection = new ObservableCollection<INodeWrapper>();

            // Logic
            var logic = new PageCollectionItem(this, RuntimeApplication.Logic);
            logic.Edited += new EditedHandler(Logic_Edited);
            newCollection.Add(logic);

            // DeviceConfiguration
            var dc = new DeviceConfigurationItem(this, RuntimeApplication.DeviceConfiguration);
            dc.Edited += new EditedHandler(DeviceConfiguration_Edited);
            newCollection.Add(dc);

            Items = newCollection;
        }

        void Logic_Edited(INodeWrapper sender, NodeBase oldNode, NodeBase newNode)
        {
            var newLogic = newNode as NodePageCollection;
            if (newLogic != null)
            {
                RuntimeApplication = RuntimeApplication.SetLogic(newLogic);
            }
        }

        private void DeviceConfiguration_Edited(INodeWrapper sender, NodeBase oldNode, NodeBase newNode)
        {
            var newDC = newNode as NodeDeviceConfiguration;
            if (newDC != null)
            {
                RuntimeApplication = RuntimeApplication.SetDeviceConfiguration(newDC);
            }
        }

        public void Delete()
        {
            FireDeletedEvent(this, Node);
        }

        public override void KeyDown(object sender, KeyEventArgs e)
        {
            base.KeyDown(sender, e);
            switch (e.Key)
            {
                case Key.Delete:
                    Delete();
                    e.Handled = true;
                    break;
            }
        }
    }
}
