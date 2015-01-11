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
using SoapBox.Core;
using SoapBox.Protocol.Automation;
using SoapBox.Protocol.Base;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;

namespace SoapBox.Snap.GameControllers
{
    public abstract class AbstractGameController : AbstractExtension, ISnapDevice
    {
        public AbstractGameController(string typeId, string name)
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

        public static NodeDevice StaticBuildHelper(string deviceName, string typeId, Guid instanceId, string code,
            int buttons, int axes, int povhats)
        {
            FieldIdentifier c;
            FieldGuid typ;
            FieldString address;
            FieldBase64 configuration;
            FieldDeviceName dName;

            c = new FieldIdentifier(code);
            typ = new FieldGuid(typeId);
            address = new FieldString(instanceId.ToString());
            configuration = new FieldBase64(string.Empty);
            dName = new FieldDeviceName(deviceName);

            NodeDevice device = NodeDevice.BuildWith(c, typ, address, configuration, dName);

            // Add the inputs
            var inputsMutable = new Collection<NodeDiscreteInput>();
            for (int i = 0; i < buttons; i++)
            {
                int buttonNumber = i + 1;
                inputsMutable.Add(NodeDiscreteInput.BuildWith(
                    new FieldIdentifier(Resources.Strings.Button + buttonNumber),
                    new FieldString(i.ToString()),
                    new FieldSignalName(Resources.Strings.Button + " " + buttonNumber)));
            }
            var inputs = new ReadOnlyCollection<NodeDiscreteInput>(inputsMutable);
            device = device.NodeDiscreteInputChildren.Append(inputs);

            var analogInputsMutable = new Collection<NodeAnalogInput>();
            for (int i = 0; i < axes; i++)
            {
                if (i == 3) break; // only supports up to 3 axes

                int axisNumber = i + 1;
                string axisName =
                    axisNumber == 1 ? "X" :
                    axisNumber == 2 ? "Y" :
                    axisNumber == 3 ? "Z" :
                    null;

                analogInputsMutable.Add(NodeAnalogInput.BuildWith(
                    new FieldIdentifier(axisName),
                    new FieldString(axisName),
                    new FieldSignalName(axisName)));

                string rotationName = "Rotation" + axisName;
                analogInputsMutable.Add(NodeAnalogInput.BuildWith(
                    new FieldIdentifier(rotationName),
                    new FieldString(rotationName),
                    new FieldSignalName(rotationName)));

            }
            for (int i = 0; i < povhats; i++)
            {
                int povNumber = i + 1;
                analogInputsMutable.Add(NodeAnalogInput.BuildWith(
                    new FieldIdentifier(Resources.Strings.PoVHat + povNumber.ToString()),
                    new FieldString(i.ToString()),
                    new FieldSignalName(Resources.Strings.PoVHat + " " + povNumber.ToString())));
            }
            device = device.NodeAnalogInputChildren.Append(new ReadOnlyCollection<NodeAnalogInput>(analogInputsMutable));

            return device;
        }
    }
}
