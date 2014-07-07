#region "SoapBox.Protocol License"
/// <header module="SoapBox.Protocol"> 
/// Copyright (C) 2010 SoapBox Automation, All Rights Reserved.
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
    /// Represents a driver in a soft runtime (to talk to I/O)
    /// </summary>
    public sealed class NodeDriver : NodeBase
    {
        //readonly members
        public readonly TypedChildCollection<NodeDevice, NodeDriver> NodeDeviceChildren = null;

        #region " FIELDS "
        
        #region " TypeId (FieldGuid) "
        public FieldGuid TypeId
        {
            get
            {
                return (FieldGuid)Fields[new FieldIdentifier(m_TypeIdName)];
            }
        }
        public NodeDriver SetTypeId(FieldGuid TypeId)
        {
            if (TypeId == null)
            {
                throw new ArgumentNullException(m_TypeIdName);
            }
            return new NodeDriver(this.SetField(new FieldIdentifier(m_TypeIdName), TypeId), ChildCollection);
        }
        static readonly string m_TypeIdName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeDriver>(o => o.TypeId);
        #endregion
        
        #region " Address (FieldString) "
        public FieldString Address
        {
            get
            {
                return (FieldString)Fields[new FieldIdentifier(m_AddressName)];
            }
        }
        public NodeDriver SetAddress(FieldString Address)
        {
            if (Address == null)
            {
                throw new ArgumentNullException(m_AddressName);
            }
            return new NodeDriver(this.SetField(new FieldIdentifier(m_AddressName), Address), ChildCollection);
        }
        static readonly string m_AddressName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeDriver>(o => o.Address);
        #endregion

        #region " Running (FieldBool) "
        public FieldBool Running
        {
            get
            {
                return (FieldBool)Fields[new FieldIdentifier(m_RunningName)];
            }
        }
        public NodeDriver SetRunning(FieldBool Running)
        {
            if (Running == null)
            {
                throw new ArgumentNullException(m_RunningName);
            }
            return new NodeDriver(this.SetField(new FieldIdentifier(m_RunningName), Running), ChildCollection);
        }
        static readonly string m_RunningName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeDriver>(o => o.Running);
        #endregion
        
        #region " Configuration (FieldBase64) "
        public FieldBase64 Configuration
        {
            get
            {
                return (FieldBase64)Fields[new FieldIdentifier(m_ConfigurationName)];
            }
        }
        public NodeDriver SetConfiguration(FieldBase64 Configuration)
        {
            if (Configuration == null)
            {
                throw new ArgumentNullException(m_ConfigurationName);
            }
            return new NodeDriver(this.SetField(new FieldIdentifier(m_ConfigurationName), Configuration), ChildCollection);
        }
        static readonly string m_ConfigurationName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeDriver>(o => o.Configuration);
        #endregion
        
        #region " DriverName (FieldString) "
        public FieldString DriverName
        {
            get
            {
                return (FieldString)Fields[new FieldIdentifier(m_DriverNameName)];
            }
        }
        public NodeDriver SetDriverName(FieldString DriverName)
        {
            if (DriverName == null)
            {
                throw new ArgumentNullException(m_DriverNameName);
            }
            return new NodeDriver(this.SetField(new FieldIdentifier(m_DriverNameName), DriverName), ChildCollection);
        }
        static readonly string m_DriverNameName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeDriver>(o => o.DriverName);
        #endregion

        #endregion

        #region " CONSTRUCTORS "
        private NodeDriver(ReadOnlyDictionary<FieldIdentifier, FieldBase> Fields,
            ReadOnlyCollection<NodeBase> Children)
            : base(Fields, Children)
        {
            NodeDeviceChildren = new TypedChildCollection<NodeDevice, NodeDriver>(this);

            // Validation
            if (TypeId == null)
            {
                throw new ArgumentNullException(m_TypeIdName);
            }
            if (Address == null)
            {
                throw new ArgumentNullException(m_AddressName);
            }
            if (Running == null)
            {
                throw new ArgumentNullException(m_RunningName);
            }
            if (Configuration == null)
            {
                throw new ArgumentNullException(m_ConfigurationName);
            }
        }

        protected override NodeBase CopyWithNewChildren(ReadOnlyCollection<NodeBase> NewChildren)
        {
            return new NodeDriver(Fields, NewChildren);
        }
        #endregion

        #region " BUILDER(S) "

        private static NodeBase Resurrect(
            ReadOnlyDictionary<FieldIdentifier, FieldBase> Fields,
            ReadOnlyCollection<NodeBase> Children)
        {
            Dictionary<FieldIdentifier, FieldBase> mutableFields =
                new Dictionary<FieldIdentifier, FieldBase>();
            // TypeId, Address and Configuration are mandatory - no default
            mutableFields.Add(new FieldIdentifier(m_RunningName),
                    new FieldBool(false));
            //Add Fields here: mutableFields.Add(new FieldIdentifier(m_CodeName),
            //        new FieldSolutionName("A123"));

            ReadOnlyDictionary<FieldIdentifier, FieldBase> defaultFields =
                new ReadOnlyDictionary<FieldIdentifier, FieldBase>(mutableFields);
            return new NodeDriver(
                SetFieldDefaults(Fields, defaultFields), Children);
        }

        public static NodeDriver BuildWith(
            FieldGuid TypeId, FieldString Address, FieldBase64 Configuration, FieldString DriverName)
        {
            //build fields
            Dictionary<FieldIdentifier, FieldBase> mutableFields =
                new Dictionary<FieldIdentifier, FieldBase>();
            mutableFields.Add(new FieldIdentifier(m_TypeIdName), TypeId);
            mutableFields.Add(new FieldIdentifier(m_AddressName), Address);
            mutableFields.Add(new FieldIdentifier(m_RunningName), new FieldBool(false));
            mutableFields.Add(new FieldIdentifier(m_ConfigurationName), Configuration);
            mutableFields.Add(new FieldIdentifier(m_DriverNameName), DriverName);
            //Add Fields here: mutableFields.Add(new FieldIdentifier(m_CodeName), Code);

            //build children
            KeyedNodeCollection<NodeBase> mutableChildren =
                new KeyedNodeCollection<NodeBase>();
            //Add Children here: mutableChildren.Add(SomeChild);

            //build node
            NodeDriver Builder = new NodeDriver(
                new ReadOnlyDictionary<FieldIdentifier, FieldBase>(mutableFields),
                new ReadOnlyCollection<NodeBase>(mutableChildren));

            return Builder;
        }
        #endregion
    }
}
