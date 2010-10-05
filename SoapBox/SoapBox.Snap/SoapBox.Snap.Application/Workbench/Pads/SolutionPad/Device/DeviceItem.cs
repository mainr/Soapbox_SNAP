#region "SoapBox.Snap License"
/// <header module="SoapBox.Snap"> 
/// Copyright (C) 2009 SoapBox Automation Inc., All Rights Reserved.
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
    public class DeviceItem : AbstractSolutionItem, IDeviceSolutionItem
    {
        /// <summary>
        /// Called by MEF (just so we can do imports)
        /// </summary>
        private DeviceItem()
            : base(null, string.Empty)
        {
        }

        public DeviceItem(ISolutionItem parent, NodeDevice device)
            : base(parent, device.DeviceName.ToString())
        {
            ContextMenu = extensionService.Sort(contextMenu);
            Device = device;
            HeaderIsEditable = true;
        }

        #region "contextMenuSingleton"
        [ImportMany(ExtensionPoints.Workbench.Pads.SolutionPad.DeviceItem.ContextMenu, typeof(IMenuItem))]
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

        #region "devicesSingleton"
        [ImportMany(ExtensionPoints.Device.Types, typeof(ISnapDevice))]
        private IEnumerable<ISnapDevice> devices
        {
            get
            {
                return devicesSingleton;
            }
            set
            {
                devicesSingleton = value;
            }
        }
        private static IEnumerable<ISnapDevice> devicesSingleton = null;
        #endregion

        #region "Device"

        public NodeDevice Device
        {
            get
            {
                return Node as NodeDevice;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(m_DeviceName);
                }
                Node = value;
                setHeaderAndContextMenu();
                setItems();
                NotifyPropertyChanged(m_DeviceArgs);
            }
        }
        private static PropertyChangedEventArgs m_DeviceArgs
            = NotifyPropertyChangedHelper.CreateArgs<DeviceItem>(o => o.Device);
        private static string m_DeviceName
            = NotifyPropertyChangedHelper.GetPropertyName<DeviceItem>(o => o.Device);

        #endregion

        #region "Header"

        private void setHeaderAndContextMenu()
        {
            bool found = false;
            foreach (var device in devices)
            {
                if (Device.TypeId == new FieldGuid(device.TypeId))
                {
                    found = true;
                    ContextMenu = extensionService.SortAndJoin(device.ContextMenu, new ConcreteMenuItemSeparator(), contextMenu);
                    break;
                }
            }
            if (!found)
            {
                ContextMenu = null;
            }
            Header = Device.DeviceName.ToString();
        }

        public override void HeaderEditAccept()
        {
            base.HeaderEditAccept();
            bool accepted = FieldDeviceName.CheckSyntax(HeaderEdit);
            if (accepted)
            {
                Device = Device.SetDeviceName(new FieldDeviceName(HeaderEdit));
            }
            else
            {
                HeaderEdit = Device.DeviceName.ToString();
            }
        }

        public override void HeaderEditCancel()
        {
            base.HeaderEditCancel();
            HeaderEdit = Device.DeviceName.ToString();
        }

        #endregion

        private void setItems()
        {
            var newCollection = new ObservableCollection<INodeWrapper>();
            foreach (var nDevice in Device.NodeDeviceChildren.Items)
            {
                var device = FindItemByNodeId(nDevice.ID) as DeviceItem;
                if (device == null)
                {
                    device = new DeviceItem(this, nDevice);
                    HookupHandlers(device);
                }
                newCollection.Add(device);
            }
            foreach (var nDiscreteInput in Device.NodeDiscreteInputChildren.Items)
            {
                var discreteInput = FindItemByNodeId(nDiscreteInput.ID) as DiscreteInputItem;
                if (discreteInput == null)
                {
                    discreteInput = new DiscreteInputItem(this, nDiscreteInput);
                    HookupHandlers(discreteInput);
                }
                newCollection.Add(discreteInput);
            }
            foreach (var nDiscreteOutput in Device.NodeDiscreteOutputChildren.Items)
            {
                var discreteOutput = FindItemByNodeId(nDiscreteOutput.ID) as DiscreteOutputItem;
                if (discreteOutput == null)
                {
                    discreteOutput = new DiscreteOutputItem(this, nDiscreteOutput);
                    HookupHandlers(discreteOutput);
                }
                newCollection.Add(discreteOutput);
            }
            foreach (var nAnalogInput in Device.NodeAnalogInputChildren.Items)
            {
                var analogInput = FindItemByNodeId(nAnalogInput.ID) as AnalogInputItem;
                if (analogInput == null)
                {
                    analogInput = new AnalogInputItem(this, nAnalogInput);
                    HookupHandlers(analogInput);
                }
                newCollection.Add(analogInput);
            }
            foreach (var nAnalogOutput in Device.NodeAnalogOutputChildren.Items)
            {
                var analogOutput = FindItemByNodeId(nAnalogOutput.ID) as AnalogOutputItem;
                if (analogOutput == null)
                {
                    analogOutput = new AnalogOutputItem(this, nAnalogOutput);
                    HookupHandlers(analogOutput);
                }
                newCollection.Add(analogOutput);
            }
            foreach (var nStringInput in Device.NodeStringInputChildren.Items)
            {
                var stringInput = FindItemByNodeId(nStringInput.ID) as StringInputItem;
                if (stringInput == null)
                {
                    stringInput = new StringInputItem(this, nStringInput);
                    HookupHandlers(stringInput);
                }
                newCollection.Add(stringInput);
            }
            foreach (var nStringOutput in Device.NodeStringOutputChildren.Items)
            {
                var stringOutput = FindItemByNodeId(nStringOutput.ID) as StringOutputItem;
                if (stringOutput == null)
                {
                    stringOutput = new StringOutputItem(this, nStringOutput);
                    HookupHandlers(stringOutput);
                }
                newCollection.Add(stringOutput);
            }
            Items = newCollection;
        }

        #region Device Children

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
                Device = Device.NodeDeviceChildren.Replace(oldDevice, newDevice);
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
                Device = Device.NodeDeviceChildren.Remove(deletedDevice);
                solutionItem.Edited -= new EditedHandler(Device_Edited);
                solutionItem.Deleted -= new DeletedHandler(Device_Deleted);
                Items.Remove(solutionItem);
            }
        }

        #endregion

        #region DiscreteInput Children

        void HookupHandlers(DiscreteInputItem discreteInput)
        {
            discreteInput.Parent = this;
            discreteInput.Edited += new EditedHandler(DiscreteInput_Edited);
            discreteInput.Deleted += new DeletedHandler(DiscreteInput_Deleted);
        }

        void DiscreteInput_Edited(INodeWrapper sender, NodeBase oldNode, NodeBase newNode)
        {
            var oldDiscreteInput = oldNode as NodeDiscreteInput;
            var newDiscreteInput = newNode as NodeDiscreteInput;
            if (oldDiscreteInput != null && newDiscreteInput != null)
            {
                Device = Device.NodeDiscreteInputChildren.Replace(oldDiscreteInput, newDiscreteInput);
            }
        }
        void DiscreteInput_Deleted(INodeWrapper sender, NodeBase deletedNode)
        {
            var deletedDiscreteInput = deletedNode as NodeDiscreteInput;
            var solutionItem = sender as DriverItem;
            if (deletedDiscreteInput != null && solutionItem != null)
            {
                if (solutionItem.Parent == this)
                {
                    solutionItem.Parent = null;
                }
                Device = Device.NodeDiscreteInputChildren.Remove(deletedDiscreteInput);
                solutionItem.Edited -= new EditedHandler(DiscreteInput_Edited);
                solutionItem.Deleted -= new DeletedHandler(DiscreteInput_Deleted);
                Items.Remove(solutionItem);
            }
        }

        #endregion

        #region DiscreteOutput Children

        void HookupHandlers(DiscreteOutputItem discreteOutput)
        {
            discreteOutput.Parent = this;
            discreteOutput.Edited += new EditedHandler(DiscreteOutput_Edited);
            discreteOutput.Deleted += new DeletedHandler(DiscreteOutput_Deleted);
        }

        void DiscreteOutput_Edited(INodeWrapper sender, NodeBase oldNode, NodeBase newNode)
        {
            var oldDiscreteOutput = oldNode as NodeDiscreteOutput;
            var newDiscreteOutput = newNode as NodeDiscreteOutput;
            if (oldDiscreteOutput != null && newDiscreteOutput != null)
            {
                Device = Device.NodeDiscreteOutputChildren.Replace(oldDiscreteOutput, newDiscreteOutput);
            }
        }
        void DiscreteOutput_Deleted(INodeWrapper sender, NodeBase deletedNode)
        {
            var deletedDiscreteOutput = deletedNode as NodeDiscreteOutput;
            var solutionItem = sender as DriverItem;
            if (deletedDiscreteOutput != null && solutionItem != null)
            {
                if (solutionItem.Parent == this)
                {
                    solutionItem.Parent = null;
                }
                Device = Device.NodeDiscreteOutputChildren.Remove(deletedDiscreteOutput);
                solutionItem.Edited -= new EditedHandler(DiscreteOutput_Edited);
                solutionItem.Deleted -= new DeletedHandler(DiscreteOutput_Deleted);
                Items.Remove(solutionItem);
            }
        }

        #endregion

        #region AnalogInput Children

        void HookupHandlers(AnalogInputItem analogInput)
        {
            analogInput.Parent = this;
            analogInput.Edited += new EditedHandler(AnalogInput_Edited);
            analogInput.Deleted += new DeletedHandler(AnalogInput_Deleted);
        }

        void AnalogInput_Edited(INodeWrapper sender, NodeBase oldNode, NodeBase newNode)
        {
            var oldAnalogInput = oldNode as NodeAnalogInput;
            var newAnalogInput = newNode as NodeAnalogInput;
            if (oldAnalogInput != null && newAnalogInput != null)
            {
                Device = Device.NodeAnalogInputChildren.Replace(oldAnalogInput, newAnalogInput);
            }
        }
        void AnalogInput_Deleted(INodeWrapper sender, NodeBase deletedNode)
        {
            var deletedAnalogInput = deletedNode as NodeAnalogInput;
            var solutionItem = sender as DriverItem;
            if (deletedAnalogInput != null && solutionItem != null)
            {
                if (solutionItem.Parent == this)
                {
                    solutionItem.Parent = null;
                }
                Device = Device.NodeAnalogInputChildren.Remove(deletedAnalogInput);
                solutionItem.Edited -= new EditedHandler(AnalogInput_Edited);
                solutionItem.Deleted -= new DeletedHandler(AnalogInput_Deleted);
                Items.Remove(solutionItem);
            }
        }

        #endregion

        #region AnalogOutput Children

        void HookupHandlers(AnalogOutputItem analogOutput)
        {
            analogOutput.Parent = this;
            analogOutput.Edited += new EditedHandler(AnalogOutput_Edited);
            analogOutput.Deleted += new DeletedHandler(AnalogOutput_Deleted);
        }

        void AnalogOutput_Edited(INodeWrapper sender, NodeBase oldNode, NodeBase newNode)
        {
            var oldAnalogOutput = oldNode as NodeAnalogOutput;
            var newAnalogOutput = newNode as NodeAnalogOutput;
            if (oldAnalogOutput != null && newAnalogOutput != null)
            {
                Device = Device.NodeAnalogOutputChildren.Replace(oldAnalogOutput, newAnalogOutput);
            }
        }
        void AnalogOutput_Deleted(INodeWrapper sender, NodeBase deletedNode)
        {
            var deletedAnalogOutput = deletedNode as NodeAnalogOutput;
            var solutionItem = sender as DriverItem;
            if (deletedAnalogOutput != null && solutionItem != null)
            {
                if (solutionItem.Parent == this)
                {
                    solutionItem.Parent = null;
                }
                Device = Device.NodeAnalogOutputChildren.Remove(deletedAnalogOutput);
                solutionItem.Edited -= new EditedHandler(AnalogOutput_Edited);
                solutionItem.Deleted -= new DeletedHandler(AnalogOutput_Deleted);
                Items.Remove(solutionItem);
            }
        }

        #endregion

        #region StringInput Children

        void HookupHandlers(StringInputItem stringInput)
        {
            stringInput.Parent = this;
            stringInput.Edited += new EditedHandler(StringInput_Edited);
            stringInput.Deleted += new DeletedHandler(StringInput_Deleted);
        }

        void StringInput_Edited(INodeWrapper sender, NodeBase oldNode, NodeBase newNode)
        {
            var oldStringInput = oldNode as NodeStringInput;
            var newStringInput = newNode as NodeStringInput;
            if (oldStringInput != null && newStringInput != null)
            {
                Device = Device.NodeStringInputChildren.Replace(oldStringInput, newStringInput);
            }
        }
        void StringInput_Deleted(INodeWrapper sender, NodeBase deletedNode)
        {
            var deletedStringInput = deletedNode as NodeStringInput;
            var solutionItem = sender as DriverItem;
            if (deletedStringInput != null && solutionItem != null)
            {
                if (solutionItem.Parent == this)
                {
                    solutionItem.Parent = null;
                }
                Device = Device.NodeStringInputChildren.Remove(deletedStringInput);
                solutionItem.Edited -= new EditedHandler(StringInput_Edited);
                solutionItem.Deleted -= new DeletedHandler(StringInput_Deleted);
                Items.Remove(solutionItem);
            }
        }

        #endregion

        #region StringOutput Children

        void HookupHandlers(StringOutputItem stringOutput)
        {
            stringOutput.Parent = this;
            stringOutput.Edited += new EditedHandler(StringOutput_Edited);
            stringOutput.Deleted += new DeletedHandler(StringOutput_Deleted);
        }

        void StringOutput_Edited(INodeWrapper sender, NodeBase oldNode, NodeBase newNode)
        {
            var oldStringOutput = oldNode as NodeStringOutput;
            var newStringOutput = newNode as NodeStringOutput;
            if (oldStringOutput != null && newStringOutput != null)
            {
                Device = Device.NodeStringOutputChildren.Replace(oldStringOutput, newStringOutput);
            }
        }
        void StringOutput_Deleted(INodeWrapper sender, NodeBase deletedNode)
        {
            var deletedStringOutput = deletedNode as NodeStringOutput;
            var solutionItem = sender as DriverItem;
            if (deletedStringOutput != null && solutionItem != null)
            {
                if (solutionItem.Parent == this)
                {
                    solutionItem.Parent = null;
                }
                Device = Device.NodeStringOutputChildren.Remove(deletedStringOutput);
                solutionItem.Edited -= new EditedHandler(StringOutput_Edited);
                solutionItem.Deleted -= new DeletedHandler(StringOutput_Deleted);
                Items.Remove(solutionItem);
            }
        }

        #endregion

    }
}
