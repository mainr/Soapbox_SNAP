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

#region "SoapBox.Snap License"
/// <header module="SoapBox.Snap"> 
/// </header>
#endregion
        
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using SoapBox.Protocol.Base;
using SoapBox.Utilities;

namespace SoapBox.Protocol.Automation
{
    /// <summary>
    /// A container for everything that needs to be sent to a runtime including
    /// tags, programs, etc.
    /// </summary>
    public sealed class NodeRuntimeApplication : NodeBase
    {

        #region " FIELDS "
        
        #region " Code (FieldIdentifier) "
        /// <summary>
        /// Has to be unique within the NodeSolution
        /// </summary>
        public FieldIdentifier Code
        {
            get
            {
                return (FieldIdentifier)Fields[new FieldIdentifier(m_CodeName)];
            }
        }
        public NodeRuntimeApplication SetCode(FieldIdentifier Code)
        {
            if (Code == null)
            {
                throw new ArgumentNullException(m_CodeName);
            }
            return new NodeRuntimeApplication(this.SetField(new FieldIdentifier(m_CodeName), Code), ChildCollection);
        }
        static readonly string m_CodeName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeRuntimeApplication>(o => o.Code);
        #endregion
        
        #region " TypeId (FieldGuid) "
        /// <summary>
        /// This is a unique ID that the developer of the runtime 
        /// engine creates.  Runtime applications will have different
        /// features based on the actual implementation of the runtime,
        /// so we'll use this to change the behavior of the editor(s).
        /// </summary>
        public FieldGuid TypeId
        {
            get
            {
                return (FieldGuid)Fields[new FieldIdentifier(m_TypeIdName)];
            }
        }
        public NodeRuntimeApplication SetTypeId(FieldGuid TypeId)
        {
            if (TypeId == null)
            {
                throw new ArgumentNullException(m_TypeIdName);
            }
            return new NodeRuntimeApplication(this.SetField(new FieldIdentifier(m_TypeIdName), TypeId), ChildCollection);
        }
        static readonly string m_TypeIdName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeRuntimeApplication>(o => o.TypeId);
        #endregion

        #region " RuntimeId (FieldGuid) "
        /// <summary>
        /// This is a unique Identifier for this particular runtime.
        /// Once created, this always stays the same.
        /// </summary>
        public FieldGuid RuntimeId
        {
            get
            {
                return (FieldGuid)Fields[new FieldIdentifier(m_RuntimeIdName)];
            }
        }
        public NodeRuntimeApplication SetRuntimeId(FieldGuid RuntimeId)
        {
            if (RuntimeId == null)
            {
                throw new ArgumentNullException(m_RuntimeIdName);
            }
            return new NodeRuntimeApplication(this.SetField(new FieldIdentifier(m_RuntimeIdName), RuntimeId), ChildCollection);
        }
        static readonly string m_RuntimeIdName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeRuntimeApplication>(o => o.RuntimeId);
        #endregion

        #region " Address (FieldString) "
        public FieldString Address
        {
            get
            {
                return (FieldString)Fields[new FieldIdentifier(m_AddressName)];
            }
        }
        public NodeRuntimeApplication SetAddress(FieldString Address)
        {
            if (Address == null)
            {
                throw new ArgumentNullException(m_AddressName);
            }
            return new NodeRuntimeApplication(this.SetField(new FieldIdentifier(m_AddressName), Address), ChildCollection);
        }
        static readonly string m_AddressName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeRuntimeApplication>(o => o.Address);
        #endregion

        #region " Configuration (FieldBase64) "
        public FieldBase64 Configuration
        {
            get
            {
                return (FieldBase64)Fields[new FieldIdentifier(m_ConfigurationName)];
            }
        }
        public NodeRuntimeApplication SetConfiguration(FieldBase64 Configuration)
        {
            if (Configuration == null)
            {
                throw new ArgumentNullException(m_ConfigurationName);
            }
            return new NodeRuntimeApplication(this.SetField(new FieldIdentifier(m_ConfigurationName), Configuration), ChildCollection);
        }
        static readonly string m_ConfigurationName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeRuntimeApplication>(o => o.Configuration);
        #endregion

        #region " ExecuteOnStartup (FieldBool) "
        /// <summary>
        /// If set to true, the runtime engine should start
        /// execution of this application as soon as it loads.
        /// </summary>
        public FieldBool ExecuteOnStartup
        {
            get
            {
                return (FieldBool)Fields[new FieldIdentifier(m_ExecuteOnStartupName)];
            }
        }
        public NodeRuntimeApplication SetExecuteOnStartup(FieldBool ExecuteOnStartup)
        {
            if (ExecuteOnStartup == null)
            {
                throw new ArgumentNullException(m_ExecuteOnStartupName);
            }
            return new NodeRuntimeApplication(this.SetField(new FieldIdentifier(m_ExecuteOnStartupName), ExecuteOnStartup), ChildCollection);
        }
        static readonly string m_ExecuteOnStartupName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeRuntimeApplication>(o => o.ExecuteOnStartup);
        #endregion

        #region " Logic (NodePageCollection) "
        private readonly SingleChild<NodePageCollection, NodeRuntimeApplication> m_Logic = null;
        public NodePageCollection Logic
        {
            get
            {
                return m_Logic.Item;
            }
        }
        public NodeRuntimeApplication SetLogic(NodePageCollection NewLogic)
        {
            if (NewLogic == null)
            {
                throw new ArgumentNullException(m_LogicName);
            }
            return m_Logic.Set(NewLogic);
        }
        static readonly string m_LogicName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeRuntimeApplication>(o => o.Logic);
        #endregion
        
        #region " DeviceConfiguration (NodeDeviceConfiguration) "
        private readonly SingleChild<NodeDeviceConfiguration, NodeRuntimeApplication> m_DeviceConfiguration = null;
        public NodeDeviceConfiguration DeviceConfiguration
        {
            get
            {
                return m_DeviceConfiguration.Item;
            }
        }
        public NodeRuntimeApplication SetDeviceConfiguration(NodeDeviceConfiguration NewDeviceConfiguration)
        {
            if (NewDeviceConfiguration == null)
            {
                throw new ArgumentNullException(m_DeviceConfigurationName);
            }
            return m_DeviceConfiguration.Set(NewDeviceConfiguration);
        }
        static readonly string m_DeviceConfigurationName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeRuntimeApplication>(o => o.DeviceConfiguration);
        #endregion
        
        #region " TryMode (FieldBool) "
        public FieldBool TryMode
        {
            get
            {
                return (FieldBool)Fields[new FieldIdentifier(m_TryModeName)];
            }
        }
        public NodeRuntimeApplication SetTryMode(FieldBool TryMode)
        {
            if (TryMode == null)
            {
                throw new ArgumentNullException(m_TryModeName);
            }
            return new NodeRuntimeApplication(this.SetField(new FieldIdentifier(m_TryModeName), TryMode), ChildCollection);
        }
        static readonly string m_TryModeName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeRuntimeApplication>(o => o.TryMode);
        #endregion

        #endregion

        #region " CONSTRUCTORS "
        private NodeRuntimeApplication(ReadOnlyDictionary<FieldIdentifier, FieldBase> Fields,
            ReadOnlyCollection<NodeBase> Children)
            : base(Fields, Children)
        {
            m_Logic = new SingleChild<NodePageCollection, NodeRuntimeApplication>(this);
            m_DeviceConfiguration = new SingleChild<NodeDeviceConfiguration, NodeRuntimeApplication>(this);

            //validation
            if (Code == null)
            {
                throw new ArgumentNullException(m_CodeName);
            }
            if (TypeId == null)
            {
                throw new ArgumentNullException(m_TypeIdName);
            }
            if (RuntimeId == null)
            {
                throw new ArgumentNullException(m_RuntimeIdName);
            }
            if (Address == null)
            {
                throw new ArgumentNullException(m_AddressName);
            }
            if (Configuration == null)
            {
                throw new ArgumentNullException(m_ConfigurationName);
            }
            if (ExecuteOnStartup == null)
            {
                throw new ArgumentNullException(m_ExecuteOnStartupName);
            }
            if (Logic == null)
            {
                throw new ArgumentNullException(m_LogicName);
            }
            if (DeviceConfiguration == null)
            {
                throw new ArgumentNullException(m_DeviceConfigurationName);
            }
            if (TryMode == null)
            {
                throw new ArgumentNullException(m_TryModeName);
            }

            // Build the signal lookup table
            // (lazy evaluating it in a threadsafe way gets messy)
            var allChildren = GetChildrenRecursive();
            var dict = new Dictionary<string, NodeSignal>();
            foreach (var child in allChildren.Values)
            {
                var signal = child as NodeSignal;
                if (signal != null)
                {
                    dict.Add(signal.SignalId.ToString(), signal);
                }
            }
            m_signalLookup = new ReadOnlyDictionary<string, NodeSignal>(dict);
        }

        protected override NodeBase CopyWithNewChildren(ReadOnlyCollection<NodeBase> NewChildren)
        {
            return new NodeRuntimeApplication(Fields, NewChildren);
        }
        #endregion

        #region " BUILDER(S) "

        private static NodeBase Resurrect(
            ReadOnlyDictionary<FieldIdentifier, FieldBase> Fields,
            ReadOnlyCollection<NodeBase> Children)
        {
            Dictionary<FieldIdentifier, FieldBase> mutableFields =
                new Dictionary<FieldIdentifier, FieldBase>();
            mutableFields.Add(new FieldIdentifier(m_TryModeName),
                    new FieldBool(false));
            //Add Fields here: mutableFields.Add(new FieldIdentifier("Code"),
            //        new FieldSolutionName("A123"));

            ReadOnlyDictionary<FieldIdentifier, FieldBase> defaultFields =
                new ReadOnlyDictionary<FieldIdentifier, FieldBase>(mutableFields);
            return new NodeRuntimeApplication(
                SetFieldDefaults(Fields, defaultFields), Children);
        }

        public static NodeRuntimeApplication BuildWith(
            FieldIdentifier Code, FieldGuid TypeId, FieldGuid RuntimeId, 
            FieldString Address, FieldBase64 Configuration, FieldBool ExecuteOnStartup)
        {
            //build fields
            Dictionary<FieldIdentifier, FieldBase> mutableFields =
                new Dictionary<FieldIdentifier, FieldBase>();
            mutableFields.Add(new FieldIdentifier(m_CodeName), Code);
            mutableFields.Add(new FieldIdentifier(m_TypeIdName), TypeId);
            mutableFields.Add(new FieldIdentifier(m_RuntimeIdName), RuntimeId);
            mutableFields.Add(new FieldIdentifier(m_AddressName), Address);
            mutableFields.Add(new FieldIdentifier(m_ConfigurationName), Configuration);
            mutableFields.Add(new FieldIdentifier(m_ExecuteOnStartupName), ExecuteOnStartup);
            mutableFields.Add(new FieldIdentifier(m_TryModeName), new FieldBool(false));
            //Add Fields here: mutableFields.Add(new FieldIdentifier("Code"), Code);

            //build children
            KeyedNodeCollection<NodeBase> mutableChildren =
                new KeyedNodeCollection<NodeBase>();
            var pc = NodePageCollection.BuildWith(
                    new FieldPageCollectionName(m_LogicName)
                );
            pc = pc.SetLogicRoot(new FieldBool(true));
            mutableChildren.Add(pc);
            mutableChildren.Add(
                NodeDeviceConfiguration.BuildWith(
                    new ReadOnlyCollection<NodeDriver>(new Collection<NodeDriver>())
                ));
            //Add Children here: mutableChildren.Add(SomeChild);

            //build node
            NodeRuntimeApplication Builder = new NodeRuntimeApplication(
                new ReadOnlyDictionary<FieldIdentifier, FieldBase>(mutableFields),
                new ReadOnlyCollection<NodeBase>(mutableChildren));

            return Builder;
        }

        public static NodeRuntimeApplication BuildWith(
            FieldIdentifier Code, FieldGuid TypeId, FieldGuid RuntimeId,
            FieldString Address, FieldBase64 Configuration, FieldBool ExecuteOnStartup, 
            NodePageCollection Logic, NodeDeviceConfiguration DeviceConfiguration)
        {
            var rta = NodeRuntimeApplication.BuildWith(
                    Code, TypeId, RuntimeId, Address, Configuration, ExecuteOnStartup);
            rta = rta.SetLogic(Logic);
            return rta.SetDeviceConfiguration(DeviceConfiguration);
        }
        #endregion

        /// <summary>
        /// Searches the tree for a signal matching this signal ID.
        /// </summary>
        public NodeSignal FindSignal(FieldGuid signalId)
        {
            if (m_signalLookup.ContainsKey(signalId.ToString()))
            {
                return m_signalLookup[signalId.ToString()];
            }
            else
            {
                return null;
            }
        }
        private readonly ReadOnlyDictionary<string, NodeSignal> m_signalLookup = null;

        public IEnumerable<NodeSignal> Signals
        {
            get
            {
                return m_signalLookup.Values;
            }
        }
    }
}
