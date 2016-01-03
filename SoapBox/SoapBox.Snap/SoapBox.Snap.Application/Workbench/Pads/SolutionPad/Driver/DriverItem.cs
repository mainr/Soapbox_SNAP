#region "SoapBox.Snap License"
/// <header module="SoapBox.Snap"> 
/// Copyright (C) 2009-2016 SoapBox Automation, All Rights Reserved.
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
    public class DriverItem : AbstractSolutionItem, IDriverSolutionItem
    {
        /// <summary>
        /// Called by MEF (just so we can do imports)
        /// </summary>
        private DriverItem()
            : base(null, string.Empty)
        {
        }

        public DriverItem(ISolutionItem parent, NodeDriver driver)
            : base(parent, driver.TypeId.ToString())
        {
            Driver = driver;
        }

        #region "contextMenuSingleton"
        [ImportMany(ExtensionPoints.Workbench.Pads.SolutionPad.DriverItem.ContextMenu, typeof(IMenuItem))]
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

        #region "driversSingleton"
        [ImportMany(ExtensionPoints.Driver.Types, typeof(ISnapDriver))]
        private IEnumerable<ISnapDriver> drivers
        {
            get
            {
                return driversSingleton;
            }
            set
            {
                driversSingleton = value;
            }
        }
        private static IEnumerable<ISnapDriver> driversSingleton = null;
        #endregion

        #region "Driver"

        public NodeDriver Driver
        {
            get
            {
                return Node as NodeDriver;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(m_DriverName);
                }
                Node = value;
                setHeaderAndContextMenu();
                setItems();
                NotifyPropertyChanged(m_DriverArgs);
            }
        }
        private static PropertyChangedEventArgs m_DriverArgs
            = NotifyPropertyChangedHelper.CreateArgs<DriverItem>(o => o.Driver);
        private static string m_DriverName
            = NotifyPropertyChangedHelper.GetPropertyName<DriverItem>(o => o.Driver);

        #endregion

        private void setHeaderAndContextMenu()
        {
            bool found = false;
            foreach (var driver in drivers)
            {
                if (Driver.TypeId == new FieldGuid(driver.TypeId))
                {
                    found = true;
                    Header = driver.Name;
                    ContextMenu = extensionService.SortAndJoin(driver.ContextMenu,new ConcreteMenuItemSeparator(),contextMenu);
                    break;
                }
            }
            if (!found)
            {
                Header = Driver.TypeId.ToString();
                ContextMenu = null;
            }
        }

        private void setItems()
        {
            var newCollection = new ObservableCollection<INodeWrapper>();
            foreach (var nDevice in Driver.NodeDeviceChildren.Items)
            {
                var device = FindItemByNodeId(nDevice.ID) as DeviceItem;
                if (device == null)
                {
                    device = new DeviceItem(this, nDevice);
                    HookupHandlers(device);
                }
                newCollection.Add(device);
            }
            Items = newCollection;
        }

        void HookupHandlers(DeviceItem device)
        {
            device.Parent = this;
            device.Edited += new EditedHandler(Device_Edited);
            device.Deleted += new DeletedHandler(Device_Deleted);
        }

        void Device_Edited(INodeWrapper sender, NodeBase oldNode, NodeBase newNode)
        {
            var oldDevice = oldNode as NodeDevice;
            var newDevice = newNode as NodeDevice;
            if (oldDevice != null && newDevice != null)
            {
                Driver = Driver.NodeDeviceChildren.Replace(oldDevice, newDevice);
            }
        }
        void Device_Deleted(INodeWrapper sender, NodeBase deletedNode)
        {
            var deletedDevice = deletedNode as NodeDevice;
            var solutionItem = sender as DriverItem;
            if (deletedDevice != null && solutionItem != null)
            {
                if (solutionItem.Parent == this)
                {
                    solutionItem.Parent = null;
                }
                Driver = Driver.NodeDeviceChildren.Remove(deletedDevice);
                solutionItem.Edited -= new EditedHandler(Device_Edited);
                solutionItem.Deleted -= new DeletedHandler(Device_Deleted);
                Items.Remove(solutionItem);
            }
        }
    }
}
