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
using SoapBox.Core;
using SoapBox.Protocol.Automation;
using SoapBox.Protocol.Base;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;

namespace SoapBox.Snap.Phidgets
{
    public abstract class AbstractPhidget : AbstractExtension, ISnapDevice
    {
        public AbstractPhidget(string typeId, string name)
        {
            m_TypeId = new Guid(typeId);
            m_Name = name;
        }

        public Guid TypeId
        {
            get
            {
                return m_TypeId;
            }
        }
        private readonly Guid m_TypeId = Guid.Empty;

        public string Name
        {
            get
            {
                return m_Name;
            }
        }
        private readonly string m_Name = string.Empty;

        public abstract NodeDevice Build();

        [ImportMany(ExtensionPoints.Device.ContextMenu, typeof(IMenuItem))]
        public IEnumerable<IMenuItem> ContextMenu { get; set; }

        public static NodeDevice StaticBuildHelper(string deviceName, int serialNumber, string code, string typeId,
            int discreteInputs, int discreteOutputs, int analogInputs, int analogOutputs, int stringInputs, int stringOutputs)
        {
            return StaticBuildHelper(deviceName, serialNumber, code, typeId, discreteInputs, discreteOutputs, analogInputs, analogOutputs,
                stringInputs, stringOutputs, Resources.Strings.AnalogOutput, Resources.Strings.Output);
        }

        public static NodeDevice StaticBuildHelper(string deviceName, int serialNumber, string code, string typeId,
            int discreteInputs, int discreteOutputs, int analogInputs, int analogOutputs, int stringInputs, int stringOutputs,
            string analogOutputNameOverride)
        {
            return StaticBuildHelper(deviceName, serialNumber, code, typeId, discreteInputs, discreteOutputs, analogInputs, analogOutputs,
                stringInputs, stringOutputs, analogOutputNameOverride, Resources.Strings.Output);
        }

        public static NodeDevice StaticBuildHelper(string deviceName, int serialNumber, string code, string typeId, 
            int discreteInputs, int discreteOutputs, int analogInputs, int analogOutputs, int stringInputs, int stringOutputs, 
            string analogOutputNameOverride, string discreteOutputNameOverride)
        {
            FieldIdentifier c;
            FieldGuid typ;
            FieldString address;
            FieldBase64 configuration;
            FieldDeviceName dName;

            c = new FieldIdentifier(code);
            typ = new FieldGuid(typeId);
            address = new FieldString(serialNumber.ToString());
            configuration = new FieldBase64(string.Empty);
            dName = new FieldDeviceName(deviceName);

            NodeDevice device = NodeDevice.BuildWith(c, typ, address, configuration, dName);

            // Add the inputs
            var inputsMutable = new Collection<NodeDiscreteInput>();
            for (int i = 0; i < discreteInputs; i++)
            {
                inputsMutable.Add(NodeDiscreteInput.BuildWith(
                    new FieldIdentifier(Resources.Strings.Input + i),
                    new FieldString(i.ToString()),
                    new FieldSignalName(Resources.Strings.Input + " " + i)));
            }
            var inputs = new ReadOnlyCollection<NodeDiscreteInput>(inputsMutable);
            device = device.NodeDiscreteInputChildren.Append(inputs);

            var analogInputsMutable = new Collection<NodeAnalogInput>();
            for (int i = 0; i < analogInputs; i++)
            {
                analogInputsMutable.Add(NodeAnalogInput.BuildWith(
                    new FieldIdentifier(Resources.Strings.AnalogInput + i),
                    new FieldString(i.ToString()),
                    new FieldSignalName(Resources.Strings.AnalogInput + " " + i)));
            }
            device = device.NodeAnalogInputChildren.Append(new ReadOnlyCollection<NodeAnalogInput>(analogInputsMutable));

            var stringInputsMutable = new Collection<NodeStringInput>();
            for (int i = 0; i < stringInputs; i++)
            {
                stringInputsMutable.Add(NodeStringInput.BuildWith(
                    new FieldIdentifier(Resources.Strings.StringInput + i),
                    new FieldString(i.ToString()),
                    new FieldSignalName(Resources.Strings.StringInput + " " + i)));
            }
            device = device.NodeStringInputChildren.Append(new ReadOnlyCollection<NodeStringInput>(stringInputsMutable));

            // Add the outputs
            var outputsMutable = new Collection<NodeDiscreteOutput>();
            for (int i = 0; i < discreteOutputs; i++)
            {
                outputsMutable.Add(NodeDiscreteOutput.BuildWith(
                    new FieldIdentifier(Resources.Strings.Output + i),
                    new FieldString(i.ToString()),
                    new FieldSignalName(discreteOutputNameOverride + " " + i)));
            }
            var outputs = new ReadOnlyCollection<NodeDiscreteOutput>(outputsMutable);
            device = device.NodeDiscreteOutputChildren.Append(outputs);

            var analogOutputsMutable = new Collection<NodeAnalogOutput>();
            for (int i = 0; i < analogOutputs; i++)
            {
                analogOutputsMutable.Add(NodeAnalogOutput.BuildWith(
                    new FieldIdentifier(Resources.Strings.AnalogOutput + i),
                    new FieldString(i.ToString()),
                    new FieldSignalName(analogOutputNameOverride + " " + i)));
            }
            device = device.NodeAnalogOutputChildren.Append(new ReadOnlyCollection<NodeAnalogOutput>(analogOutputsMutable));

            var stringOutputsMutable = new Collection<NodeStringOutput>();
            for (int i = 0; i < stringOutputs; i++)
            {
                stringOutputsMutable.Add(NodeStringOutput.BuildWith(
                    new FieldIdentifier(Resources.Strings.StringOutput + i),
                    new FieldString(i.ToString()),
                    new FieldSignalName(Resources.Strings.StringOutput + " " + i)));
            }
            device = device.NodeStringOutputChildren.Append(new ReadOnlyCollection<NodeStringOutput>(stringOutputsMutable));

            return device;
        }
    }
}
