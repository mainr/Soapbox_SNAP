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
    /// Represents a discrete input on a device
    /// </summary>
    public sealed class NodeDiscreteInput : NodeBase
    {

        #region "Value"
        /// <summary>
        /// Input scanning can set this value, and it will
        /// update the signal Value depending on force status.
        /// Threadsafe.
        /// </summary>
        public bool Value
        {
            get
            {
                return (bool)Signal.Value;
            }
            set
            {
                if (Forced.BoolValue)
                {
                    Signal.Value = ForcedValue.BoolValue;
                }
                else
                {
                    Signal.Value = value;
                }
            }
        }
        #endregion

        #region " FIELDS "

        #region " Code (FieldIdentifier) "
        public FieldIdentifier Code
        {
            get
            {
                return (FieldIdentifier)Fields[new FieldIdentifier(m_CodeName)];
            }
        }
        public NodeDiscreteInput SetCode(FieldIdentifier Code)
        {
            if (Code == null)
            {
                throw new ArgumentNullException(m_CodeName);
            }
            return new NodeDiscreteInput(this.SetField(new FieldIdentifier(m_CodeName), Code), ChildCollection);
        }
        static readonly string m_CodeName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeDiscreteInput>(o => o.Code);
        #endregion
        
        #region " Address (FieldString) "
        public FieldString Address
        {
            get
            {
                return (FieldString)Fields[new FieldIdentifier(m_AddressName)];
            }
        }
        public NodeDiscreteInput SetAddress(FieldString Address)
        {
            if (Address == null)
            {
                throw new ArgumentNullException(m_AddressName);
            }
            return new NodeDiscreteInput(this.SetField(new FieldIdentifier(m_AddressName), Address), ChildCollection);
        }
        static readonly string m_AddressName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeDiscreteInput>(o => o.Address);
        #endregion
        
        #region " Forced (FieldBool) "
        public FieldBool Forced
        {
            get
            {
                return (FieldBool)Fields[new FieldIdentifier(m_ForcedName)];
            }
        }
        public NodeDiscreteInput SetForced(FieldBool Forced)
        {
            if (Forced == null)
            {
                throw new ArgumentNullException(m_ForcedName);
            }
            return new NodeDiscreteInput(this.SetField(new FieldIdentifier(m_ForcedName), Forced), ChildCollection);
        }
        static readonly string m_ForcedName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeDiscreteInput>(o => o.Forced);
        #endregion
        
        #region " ForcedValue (FieldBool) "
        public FieldBool ForcedValue
        {
            get
            {
                return (FieldBool)Fields[new FieldIdentifier(m_ForcedValueName)];
            }
        }
        public NodeDiscreteInput SetForcedValue(FieldBool ForcedValue)
        {
            if (ForcedValue == null)
            {
                throw new ArgumentNullException(m_ForcedValueName);
            }
            return new NodeDiscreteInput(this.SetField(new FieldIdentifier(m_ForcedValueName), ForcedValue), ChildCollection);
        }
        static readonly string m_ForcedValueName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeDiscreteInput>(o => o.ForcedValue);
        #endregion

        #region " Signal (NodeSignal) "
        private readonly SingleChild<NodeSignal, NodeDiscreteInput> m_Signal = null;
        public NodeSignal Signal
        {
            get
            {
                return m_Signal.Item;
            }
        }
        public NodeDiscreteInput SetSignal(NodeSignal NewSignal)
        {
            if (NewSignal == null)
            {
                throw new ArgumentNullException(m_SignalName);
            }
            return m_Signal.Set(NewSignal);
        }
        static readonly string m_SignalName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeDiscreteInput>(o => o.Signal);
        #endregion
        
        #region " InputName (FieldSignalName) "
        public FieldSignalName InputName
        {
            get
            {
                return (FieldSignalName)Fields[new FieldIdentifier(m_InputNameName)];
            }
        }
        public NodeDiscreteInput SetInputName(FieldSignalName InputName)
        {
            if (InputName == null)
            {
                throw new ArgumentNullException(m_InputNameName);
            }
            return new NodeDiscreteInput(this.SetField(new FieldIdentifier(m_InputNameName), InputName), ChildCollection);
        }
        static readonly string m_InputNameName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeDiscreteInput>(o => o.InputName);
        #endregion

        #endregion

        #region " CONSTRUCTORS "
        private NodeDiscreteInput(ReadOnlyDictionary<FieldIdentifier, FieldBase> Fields,
            ReadOnlyCollection<NodeBase> Children)
            : base(Fields, Children)
        {
            m_Signal = new SingleChild<NodeSignal, NodeDiscreteInput>(this);

            // Validation
            if (Code == null)
            {
                throw new ArgumentNullException(m_CodeName);
            }
            if (Address == null)
            {
                throw new ArgumentNullException(m_AddressName);
            }
            if (Forced == null)
            {
                throw new ArgumentNullException(m_ForcedName);
            }
            if (ForcedValue == null)
            {
                throw new ArgumentNullException(m_ForcedValueName);
            }
            if (Signal == null)
            {
                throw new ArgumentNullException(m_SignalName);
            } 
            if (InputName == null)
            {
                throw new ArgumentNullException(m_InputNameName);
            }
        }

        protected override NodeBase CopyWithNewChildren(ReadOnlyCollection<NodeBase> NewChildren)
        {
            return new NodeDiscreteInput(Fields, NewChildren);
        }
        #endregion

        #region " BUILDER(S) "

        private static NodeBase Resurrect(
            ReadOnlyDictionary<FieldIdentifier, FieldBase> Fields,
            ReadOnlyCollection<NodeBase> Children)
        {
            Dictionary<FieldIdentifier, FieldBase> mutableFields =
                new Dictionary<FieldIdentifier, FieldBase>();
            // Code and Address are mandatory
            mutableFields.Add(new FieldIdentifier(m_ForcedName),
                    new FieldBool(false));
            mutableFields.Add(new FieldIdentifier(m_ForcedValueName),
                    new FieldBool(false));
            //Add Fields here: mutableFields.Add(new FieldIdentifier(m_CodeName),
            //        new FieldSolutionName("A123"));

            ReadOnlyDictionary<FieldIdentifier, FieldBase> defaultFields =
                new ReadOnlyDictionary<FieldIdentifier, FieldBase>(mutableFields);
            return new NodeDiscreteInput(
                SetFieldDefaults(Fields, defaultFields), Children);
        }

        public static NodeDiscreteInput BuildWith(FieldIdentifier Code, FieldString Address, FieldSignalName InputName)
        {
            //build fields
            Dictionary<FieldIdentifier, FieldBase> mutableFields =
                new Dictionary<FieldIdentifier, FieldBase>();
            mutableFields.Add(new FieldIdentifier(m_CodeName), Code);
            mutableFields.Add(new FieldIdentifier(m_AddressName), Address);
            mutableFields.Add(new FieldIdentifier(m_ForcedName), new FieldBool(false));
            mutableFields.Add(new FieldIdentifier(m_ForcedValueName), new FieldBool(false));
            mutableFields.Add(new FieldIdentifier(m_InputNameName), InputName);
            //Add Fields here: mutableFields.Add(new FieldIdentifier(m_CodeName), Code);

            //build children
            KeyedNodeCollection<NodeBase> mutableChildren =
                new KeyedNodeCollection<NodeBase>();
            mutableChildren.Add(NodeSignal.BuildWith(
                InputName, new FieldDataType(FieldDataType.DataTypeEnum.BOOL),
                new FieldBool(false), new FieldConstant(FieldDataType.DataTypeEnum.BOOL,false)));
            //Add Children here: mutableChildren.Add(SomeChild);

            //build node
            NodeDiscreteInput Builder = new NodeDiscreteInput(
                new ReadOnlyDictionary<FieldIdentifier, FieldBase>(mutableFields),
                new ReadOnlyCollection<NodeBase>(mutableChildren));

            return Builder;
        }
        #endregion
    }
}
