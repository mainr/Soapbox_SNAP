#region "SoapBox.Snap License"
/// <header module="SoapBox.Snap"> 
/// Copyright (C) 2009-2015 SoapBox Automation, All Rights Reserved.
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
using System.ComponentModel.Composition;
using SoapBox.Core;
using System.Collections.ObjectModel;
using System.ComponentModel;
using SoapBox.Utilities;
using SoapBox.Protocol.Base;

namespace SoapBox.Snap.Application
{
    [Export(ExtensionPoints.Workbench.Pads.SolutionPad.SolutionItems, typeof(ISolutionItem))]
    public class DeviceConfigurationItem : AbstractSolutionItem
    {
        /// <summary>
        /// Called by MEF (just so we can do imports)
        /// </summary>
        private DeviceConfigurationItem()
            : base(null, string.Empty)
        {
        }

        public DeviceConfigurationItem(RuntimeApplicationItem runtimeApplicationItem, 
            NodeDeviceConfiguration deviceConfiguration)
            : base(runtimeApplicationItem, Resources.Strings.Solution_Pad_DeviceConfigurationItem_Header)
        {
            if (runtimeApplicationItem == null)
            {
                throw new ArgumentNullException();
            }
            m_runtimeApplicationItem = runtimeApplicationItem;

            ContextMenu = extensionService.Sort(contextMenu);
            DeviceConfiguration = deviceConfiguration;

            SetIconFromBitmap(Resources.Images.DeviceConfiguration);
        }

        private readonly RuntimeApplicationItem m_runtimeApplicationItem = null;

        #region "contextMenuSingleton"
        [ImportMany(ExtensionPoints.Workbench.Pads.SolutionPad.DeviceConfigurationItem.ContextMenu, typeof(IMenuItem))]
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

        #region "DeviceConfiguration"

        public NodeDeviceConfiguration DeviceConfiguration
        {
            get
            {
                return Node as NodeDeviceConfiguration;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(m_DeviceConfigurationName);
                }
                Node = value;
                setItems();
                NotifyPropertyChanged(m_DeviceConfigurationArgs);
            }
        }
        private static PropertyChangedEventArgs m_DeviceConfigurationArgs
            = NotifyPropertyChangedHelper.CreateArgs<DeviceConfigurationItem>(o => o.DeviceConfiguration);
        private static string m_DeviceConfigurationName
            = NotifyPropertyChangedHelper.GetPropertyName<DeviceConfigurationItem>(o => o.DeviceConfiguration);

        #endregion

        private void setItems()
        {
            var newCollection = new ObservableCollection<INodeWrapper>();
            foreach (var nDriver in DeviceConfiguration.NodeDriverChildren.Items)
            {
                var driver = FindItemByNodeId(nDriver.ID) as DriverItem;
                if (driver == null)
                {
                    driver = new DriverItem(this, nDriver);
                    HookupHandlers(driver);
                }
                newCollection.Add(driver);
            }
            Items = newCollection;
        }

        void HookupHandlers(DriverItem driver)
        {
            driver.Parent = this;
            driver.Edited += new EditedHandler(Driver_Edited);
            driver.Deleted += new DeletedHandler(Driver_Deleted);
        }

        void Driver_Edited(INodeWrapper sender, NodeBase oldNode, NodeBase newNode)
        {
            var oldDriver = oldNode as NodeDriver;
            var newDriver = newNode as NodeDriver;
            if (oldDriver != null && newDriver != null)
            {
                DeviceConfiguration = DeviceConfiguration.NodeDriverChildren.Replace(oldDriver, newDriver);
            }
        }
        void Driver_Deleted(INodeWrapper sender, NodeBase deletedNode)
        {
            var deletedDriver = deletedNode as NodeDriver;
            var solutionItem = sender as DriverItem;
            if (deletedDriver != null && solutionItem != null)
            {
                if (solutionItem.Parent == this)
                {
                    solutionItem.Parent = null;
                }
                DeviceConfiguration = DeviceConfiguration.NodeDriverChildren.Remove(deletedDriver);
                solutionItem.Edited -= new EditedHandler(Driver_Edited);
                solutionItem.Deleted -= new DeletedHandler(Driver_Deleted);
                Items.Remove(solutionItem);
            }
        }

        public void ReadDeviceConfiguration()
        {
            if (m_runtimeApplicationItem.Connected)
            {
                var cfg = m_runtimeApplicationItem.Runtime.ReadConfiguration();
                if(runtimeService.DisconnectDialog(this))
                {
                    DeviceConfiguration = cfg;
                }
            }
            else
            {
                messagingService.Value.ShowMessage(
                    Resources.Strings.Solution_Pad_DeviceConfigurationItem_NotConnected_Message,
                    Resources.Strings.Solution_Pad_DeviceConfigurationItem_NotConnected_Title);
            }
        }
    }
}
