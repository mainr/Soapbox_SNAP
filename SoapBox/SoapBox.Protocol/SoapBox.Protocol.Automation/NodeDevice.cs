#region "SoapBox.Protocol License"
/// <header module="SoapBox.Protocol"> 
/// Copyright (C) 2010 SoapBox Automation Inc., All Rights Reserved.
/// Contact: SoapBox Automation Licencing (license@soapboxautomation.com)
/// 
/// This file is part of SoapBox Protocol.
///
/// SoapBox Protocol is available under your choice of these licenses:
///  - GPLv3
///  - CDDLv1.0
///
/// GNU General Public License Usage
/// SoapBox Protocol is free software: you can redistribute it and/or modify it
/// under the terms of the GNU General Public License as published by the 
/// Free Software Foundation, either version 3 of the License, or 
/// (at your option) any later version.
/// 
/// SoapBox Protocol is distributed in the hope that it will be useful, but 
/// WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
/// GNU General Public License for more details.
/// 
/// You should have received a copy of the GNU General Public License along
/// with SoapBox Protocol. If not, see <http://www.gnu.org/licenses/>.
/// 
/// Common Development and Distribution License Usage
/// SoapBox Protocol is subject to the CDDL Version 1.0. 
/// You should have received a copy of the CDDL Version 1.0 along
/// with SoapBox Protocol.  If not, see <http://www.sun.com/cddl/cddl.html>.
/// The CDDL is a royalty free, open source, file based license.
/// </header>
#endregion


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using SoapBox.Utilities;
using SoapBox.Protocol.Base;

namespace SoapBox.Protocol.Automation
{
    /// <summary>
    /// Abstraction for a piece of hardware in the I/O config tree
    /// </summary>
    public sealed class NodeDevice : NodeBase
    {
        //readonly members
        public readonly TypedChildCollection<NodeDevice, NodeDevice> NodeDeviceChildren = null;
        public readonly TypedChildCollection<NodeDiscreteInput, NodeDevice> NodeDiscreteInputChildren = null;
        public readonly TypedChildCollection<NodeDiscreteOutput, NodeDevice> NodeDiscreteOutputChildren = null;
        public readonly TypedChildCollection<NodeAnalogInput, NodeDevice> NodeAnalogInputChildren = null;
        public readonly TypedChildCollection<NodeAnalogOutput, NodeDevice> NodeAnalogOutputChildren = null;
        public readonly TypedChildCollection<NodeStringInput, NodeDevice> NodeStringInputChildren = null;
        public readonly TypedChildCollection<NodeStringOutput, NodeDevice> NodeStringOutputChildren = null;

        #region " FIELDS "
        
        #region " Code (FieldIdentifier) "
        public FieldIdentifier Code
        {
            get
            {
                return (FieldIdentifier)Fields[new FieldIdentifier(m_CodeName)];
            }
        }
        public NodeDevice SetCode(FieldIdentifier Code)
        {
            if (Code == null)
            {
                throw new ArgumentNullException(m_CodeName);
            }
            return new NodeDevice(this.SetField(new FieldIdentifier(m_CodeName), Code), ChildCollection);
        }
        static readonly string m_CodeName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeDevice>(o => o.Code);
        #endregion

        #region " TypeId (FieldGuid) "
        public FieldGuid TypeId
        {
            get
            {
                return (FieldGuid)Fields[new FieldIdentifier(m_TypeIdName)];
            }
        }
        public NodeDevice SetTypeId(FieldGuid TypeId)
        {
            if (TypeId == null)
            {
                throw new ArgumentNullException(m_TypeIdName);
            }
            return new NodeDevice(this.SetField(new FieldIdentifier(m_TypeIdName), TypeId), ChildCollection);
        }
        static readonly string m_TypeIdName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeDevice>(o => o.TypeId);
        #endregion
        
        #region " Address (FieldString) "
        public FieldString Address
        {
            get
            {
                return (FieldString)Fields[new FieldIdentifier(m_AddressName)];
            }
        }
        public NodeDevice SetAddress(FieldString Address)
        {
            if (Address == null)
            {
                throw new ArgumentNullException(m_AddressName);
            }
            return new NodeDevice(this.SetField(new FieldIdentifier(m_AddressName), Address), ChildCollection);
        }
        static readonly string m_AddressName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeDevice>(o => o.Address);
        #endregion

        #region " Configuration (FieldBase64) "
        public FieldBase64 Configuration
        {
            get
            {
                return (FieldBase64)Fields[new FieldIdentifier(m_ConfigurationName)];
            }
        }
        public NodeDevice SetConfiguration(FieldBase64 Configuration)
        {
            if (Configuration == null)
            {
                throw new ArgumentNullException(m_ConfigurationName);
            }
            return new NodeDevice(this.SetField(new FieldIdentifier(m_ConfigurationName), Configuration), ChildCollection);
        }
        static readonly string m_ConfigurationName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeDevice>(o => o.Configuration);
        #endregion

        #region " DeviceName (FieldDeviceName) "
        public FieldDeviceName DeviceName
        {
            get
            {
                return (FieldDeviceName)Fields[new FieldIdentifier(m_DeviceNameName)];
            }
        }
        public NodeDevice SetDeviceName(FieldDeviceName DeviceName)
        {
            if (DeviceName == null)
            {
                throw new ArgumentNullException(m_DeviceNameName);
            }
            return new NodeDevice(this.SetField(new FieldIdentifier(m_DeviceNameName), DeviceName), ChildCollection);
        }
        static readonly string m_DeviceNameName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeDevice>(o => o.DeviceName);
        #endregion

        #endregion

        #region " CONSTRUCTORS "
        private NodeDevice(ReadOnlyDictionary<FieldIdentifier, FieldBase> Fields,
            ReadOnlyCollection<NodeBase> Children)
            : base(Fields, Children)
        {
            NodeDeviceChildren = new TypedChildCollection<NodeDevice, NodeDevice>(this);
            NodeDiscreteInputChildren = new TypedChildCollection<NodeDiscreteInput, NodeDevice>(this);
            NodeDiscreteOutputChildren = new TypedChildCollection<NodeDiscreteOutput, NodeDevice>(this);
            NodeAnalogInputChildren = new TypedChildCollection<NodeAnalogInput, NodeDevice>(this);
            NodeAnalogOutputChildren = new TypedChildCollection<NodeAnalogOutput, NodeDevice>(this);
            NodeStringInputChildren = new TypedChildCollection<NodeStringInput, NodeDevice>(this);
            NodeStringOutputChildren = new TypedChildCollection<NodeStringOutput, NodeDevice>(this);

            // Validation
            if (Code == null)
            {
                throw new ArgumentNullException(m_CodeName);
            }
            if (TypeId == null)
            {
                throw new ArgumentNullException(m_TypeIdName);
            }
            if (Address == null)
            {
                throw new ArgumentNullException(m_AddressName);
            }
            if (Configuration == null)
            {
                throw new ArgumentNullException(m_ConfigurationName);
            } 
            if (DeviceName == null)
            {
                throw new ArgumentNullException(m_DeviceNameName);
            }
        }

        protected override NodeBase CopyWithNewChildren(ReadOnlyCollection<NodeBase> NewChildren)
        {
            return new NodeDevice(Fields, NewChildren);
        }
        #endregion

        #region " BUILDER(S) "

        private static NodeBase Resurrect(
            ReadOnlyDictionary<FieldIdentifier, FieldBase> Fields,
            ReadOnlyCollection<NodeBase> Children)
        {
            Dictionary<FieldIdentifier, FieldBase> mutableFields =
                new Dictionary<FieldIdentifier, FieldBase>();
            // Code, TypeId, Address, Configuration, DeviceName are required - no default

            //Add Fields here: mutableFields.Add(new FieldIdentifier(m_CodeName),
            //        new FieldSolutionName("A123"));

            ReadOnlyDictionary<FieldIdentifier, FieldBase> defaultFields =
                new ReadOnlyDictionary<FieldIdentifier, FieldBase>(mutableFields);
            return new NodeDevice(
                SetFieldDefaults(Fields, defaultFields), Children);
        }

        public static NodeDevice BuildWith(
            FieldIdentifier Code, FieldGuid TypeId, 
            FieldString Address, FieldBase64 Configuration, 
            FieldDeviceName DeviceName)
        {
            //build fields
            Dictionary<FieldIdentifier, FieldBase> mutableFields =
                new Dictionary<FieldIdentifier, FieldBase>();
            mutableFields.Add(new FieldIdentifier(m_CodeName), Code);
            mutableFields.Add(new FieldIdentifier(m_TypeIdName), TypeId);
            mutableFields.Add(new FieldIdentifier(m_AddressName), Address);
            mutableFields.Add(new FieldIdentifier(m_ConfigurationName), Configuration);
            mutableFields.Add(new FieldIdentifier(m_DeviceNameName), DeviceName);
            //Add Fields here: mutableFields.Add(new FieldIdentifier(m_CodeName), Code);

            //build children
            KeyedNodeCollection<NodeBase> mutableChildren =
                new KeyedNodeCollection<NodeBase>();
            //Add Children here: mutableChildren.Add(SomeChild);

            //build node
            NodeDevice Builder = new NodeDevice(
                new ReadOnlyDictionary<FieldIdentifier, FieldBase>(mutableFields),
                new ReadOnlyCollection<NodeBase>(mutableChildren));

            return Builder;
        }
        #endregion
    }
}
        
