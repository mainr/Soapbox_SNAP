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
    /// Represents a Analog input on a device
    /// </summary>
    public sealed class NodeAnalogInput : NodeBase
    {

        #region "Value"
        /// <summary>
        /// Input scanning can set this value, and it will
        /// update the signal Value depending on force status.
        /// Threadsafe.
        /// </summary>
        public Decimal Value
        {
            get
            {
                return (Decimal)Signal.Value;
            }
            set
            {
                if (Forced.BoolValue)
                {
                    Signal.Value = ForcedValue.Value;
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
        public NodeAnalogInput SetCode(FieldIdentifier Code)
        {
            if (Code == null)
            {
                throw new ArgumentNullException(m_CodeName);
            }
            return new NodeAnalogInput(this.SetField(new FieldIdentifier(m_CodeName), Code), ChildCollection);
        }
        static readonly string m_CodeName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeAnalogInput>(o => o.Code);
        #endregion
        
        #region " Address (FieldString) "
        public FieldString Address
        {
            get
            {
                return (FieldString)Fields[new FieldIdentifier(m_AddressName)];
            }
        }
        public NodeAnalogInput SetAddress(FieldString Address)
        {
            if (Address == null)
            {
                throw new ArgumentNullException(m_AddressName);
            }
            return new NodeAnalogInput(this.SetField(new FieldIdentifier(m_AddressName), Address), ChildCollection);
        }
        static readonly string m_AddressName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeAnalogInput>(o => o.Address);
        #endregion
        
        #region " Forced (FieldBool) "
        public FieldBool Forced
        {
            get
            {
                return (FieldBool)Fields[new FieldIdentifier(m_ForcedName)];
            }
        }
        public NodeAnalogInput SetForced(FieldBool Forced)
        {
            if (Forced == null)
            {
                throw new ArgumentNullException(m_ForcedName);
            }
            return new NodeAnalogInput(this.SetField(new FieldIdentifier(m_ForcedName), Forced), ChildCollection);
        }
        static readonly string m_ForcedName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeAnalogInput>(o => o.Forced);
        #endregion
        
        #region " ForcedValue (FieldConstant) "
        public FieldConstant ForcedValue
        {
            get
            {
                return (FieldConstant)Fields[new FieldIdentifier(m_ForcedValueName)];
            }
        }
        public NodeAnalogInput SetForcedValue(FieldConstant ForcedValue)
        {
            if (ForcedValue == null)
            {
                throw new ArgumentNullException(m_ForcedValueName);
            }
            return new NodeAnalogInput(this.SetField(new FieldIdentifier(m_ForcedValueName), ForcedValue), ChildCollection);
        }
        static readonly string m_ForcedValueName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeAnalogInput>(o => o.ForcedValue);
        #endregion

        #region " Signal (NodeSignal) "
        private readonly SingleChild<NodeSignal, NodeAnalogInput> m_Signal = null;
        public NodeSignal Signal
        {
            get
            {
                return m_Signal.Item;
            }
        }
        public NodeAnalogInput SetSignal(NodeSignal NewSignal)
        {
            if (NewSignal == null)
            {
                throw new ArgumentNullException(m_SignalName);
            }
            return m_Signal.Set(NewSignal);
        }
        static readonly string m_SignalName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeAnalogInput>(o => o.Signal);
        #endregion
        
        #region " InputName (FieldSignalName) "
        public FieldSignalName InputName
        {
            get
            {
                return (FieldSignalName)Fields[new FieldIdentifier(m_InputNameName)];
            }
        }
        public NodeAnalogInput SetInputName(FieldSignalName InputName)
        {
            if (InputName == null)
            {
                throw new ArgumentNullException(m_InputNameName);
            }
            return new NodeAnalogInput(this.SetField(new FieldIdentifier(m_InputNameName), InputName), ChildCollection);
        }
        static readonly string m_InputNameName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeAnalogInput>(o => o.InputName);
        #endregion

        #endregion

        #region " CONSTRUCTORS "
        private NodeAnalogInput(ReadOnlyDictionary<FieldIdentifier, FieldBase> Fields,
            ReadOnlyCollection<NodeBase> Children)
            : base(Fields, Children)
        {
            m_Signal = new SingleChild<NodeSignal, NodeAnalogInput>(this);

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
            return new NodeAnalogInput(Fields, NewChildren);
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
                    new FieldConstant(FieldDataType.DataTypeEnum.NUMBER, FieldDataType.DefaultValue(FieldDataType.DataTypeEnum.NUMBER)));
            //Add Fields here: mutableFields.Add(new FieldIdentifier(m_CodeName),
            //        new FieldSolutionName("A123"));

            ReadOnlyDictionary<FieldIdentifier, FieldBase> defaultFields =
                new ReadOnlyDictionary<FieldIdentifier, FieldBase>(mutableFields);
            return new NodeAnalogInput(
                SetFieldDefaults(Fields, defaultFields), Children);
        }

        public static NodeAnalogInput BuildWith(FieldIdentifier Code, FieldString Address, FieldSignalName InputName)
        {
            //build fields
            Dictionary<FieldIdentifier, FieldBase> mutableFields =
                new Dictionary<FieldIdentifier, FieldBase>();
            mutableFields.Add(new FieldIdentifier(m_CodeName), Code);
            mutableFields.Add(new FieldIdentifier(m_AddressName), Address);
            mutableFields.Add(new FieldIdentifier(m_ForcedName), new FieldBool(false));
            mutableFields.Add(new FieldIdentifier(m_ForcedValueName), new FieldConstant(FieldDataType.DataTypeEnum.NUMBER, FieldDataType.DefaultValue(FieldDataType.DataTypeEnum.NUMBER)));
            mutableFields.Add(new FieldIdentifier(m_InputNameName), InputName);
            //Add Fields here: mutableFields.Add(new FieldIdentifier(m_CodeName), Code);

            //build children
            KeyedNodeCollection<NodeBase> mutableChildren =
                new KeyedNodeCollection<NodeBase>();
            mutableChildren.Add(NodeSignal.BuildWith(
                InputName, new FieldDataType(FieldDataType.DataTypeEnum.NUMBER),
                new FieldBool(false), new FieldConstant(FieldDataType.DataTypeEnum.NUMBER, FieldDataType.DefaultValue(FieldDataType.DataTypeEnum.NUMBER))));
            //Add Children here: mutableChildren.Add(SomeChild);

            //build node
            NodeAnalogInput Builder = new NodeAnalogInput(
                new ReadOnlyDictionary<FieldIdentifier, FieldBase>(mutableFields),
                new ReadOnlyCollection<NodeBase>(mutableChildren));

            return Builder;
        }
        #endregion
    }
}
