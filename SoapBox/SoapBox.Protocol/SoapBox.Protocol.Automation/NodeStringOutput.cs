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
    /// Represents a String output on a device
    /// </summary>
    public sealed class NodeStringOutput : NodeBase
    {

        /// <summary>
        /// The value that should be output to the actual hardware
        /// and takes the forced status into account.  Threadsafe.
        /// </summary>
        public String GetValue(NodeRuntimeApplication runtimeApplication)
        {
            if (runtimeApplication == null)
            {
                throw new ArgumentNullException();
            }
            if (Forced.BoolValue)
            {
                return (String)ForcedValue.Value;
            }
            else
            {
                var nValue = SignalIn.GetValue(runtimeApplication);
                if (nValue != null && nValue.DataType.DataType == FieldDataType.DataTypeEnum.STRING)
                {
                    return (String)nValue.Value;
                }
                else
                {
                    return (String)FieldDataType.DefaultValue(FieldDataType.DataTypeEnum.STRING);
                }
            }
        }

        #region " FIELDS "

        #region " Code (FieldIdentifier) "
        public FieldIdentifier Code
        {
            get
            {
                return (FieldIdentifier)Fields[new FieldIdentifier(m_CodeName)];
            }
        }
        public NodeStringOutput SetCode(FieldIdentifier Code)
        {
            if (Code == null)
            {
                throw new ArgumentNullException(m_CodeName);
            }
            return new NodeStringOutput(this.SetField(new FieldIdentifier(m_CodeName), Code), ChildCollection);
        }
        static readonly string m_CodeName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeStringOutput>(o => o.Code);
        #endregion
        
        #region " Address (FieldString) "
        public FieldString Address
        {
            get
            {
                return (FieldString)Fields[new FieldIdentifier(m_AddressName)];
            }
        }
        public NodeStringOutput SetAddress(FieldString Address)
        {
            if (Address == null)
            {
                throw new ArgumentNullException(m_AddressName);
            }
            return new NodeStringOutput(this.SetField(new FieldIdentifier(m_AddressName), Address), ChildCollection);
        }
        static readonly string m_AddressName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeStringOutput>(o => o.Address);
        #endregion
        
        #region " Forced (FieldBool) "
        public FieldBool Forced
        {
            get
            {
                return (FieldBool)Fields[new FieldIdentifier(m_ForcedName)];
            }
        }
        public NodeStringOutput SetForced(FieldBool Forced)
        {
            if (Forced == null)
            {
                throw new ArgumentNullException(m_ForcedName);
            }
            return new NodeStringOutput(this.SetField(new FieldIdentifier(m_ForcedName), Forced), ChildCollection);
        }
        static readonly string m_ForcedName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeStringOutput>(o => o.Forced);
        #endregion
        
        #region " ForcedValue (FieldConstant) "
        public FieldConstant ForcedValue
        {
            get
            {
                return (FieldConstant)Fields[new FieldIdentifier(m_ForcedValueName)];
            }
        }
        public NodeStringOutput SetForcedValue(FieldConstant ForcedValue)
        {
            if (ForcedValue == null)
            {
                throw new ArgumentNullException(m_ForcedValueName);
            }
            return new NodeStringOutput(this.SetField(new FieldIdentifier(m_ForcedValueName), ForcedValue), ChildCollection);
        }
        static readonly string m_ForcedValueName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeStringOutput>(o => o.ForcedValue);
        #endregion
        
        #region " SignalIn (NodeSignalIn) "
        private readonly SingleChild<NodeSignalIn, NodeStringOutput> m_SignalIn = null;
        public NodeSignalIn SignalIn
        {
            get
            {
                return m_SignalIn.Item;
            }
        }
        public NodeStringOutput SetSignalIn(NodeSignalIn NewSignalIn)
        {
            if (NewSignalIn == null)
            {
                throw new ArgumentNullException(m_SignalInName);
            }
            return m_SignalIn.Set(NewSignalIn);
        }
        static readonly string m_SignalInName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeStringOutput>(o => o.SignalIn);
        #endregion
        
        #region " OutputName (FieldSignalName) "
        public FieldSignalName OutputName
        {
            get
            {
                return (FieldSignalName)Fields[new FieldIdentifier(m_OutputNameName)];
            }
        }
        public NodeStringOutput SetOutputName(FieldSignalName OutputName)
        {
            if (OutputName == null)
            {
                throw new ArgumentNullException(m_OutputNameName);
            }
            return new NodeStringOutput(this.SetField(new FieldIdentifier(m_OutputNameName), OutputName), ChildCollection);
        }
        static readonly string m_OutputNameName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeStringOutput>(o => o.OutputName);
        #endregion

        #endregion

        #region " CONSTRUCTORS "
        private NodeStringOutput(ReadOnlyDictionary<FieldIdentifier, FieldBase> Fields,
            ReadOnlyCollection<NodeBase> Children)
            : base(Fields, Children)
        {
            m_SignalIn = new SingleChild<NodeSignalIn, NodeStringOutput>(this);

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
            if (SignalIn == null)
            {
                throw new ArgumentNullException(m_SignalInName);
            }
            if (OutputName == null)
            {
                throw new ArgumentNullException(m_OutputNameName);
            }
        }

        protected override NodeBase CopyWithNewChildren(ReadOnlyCollection<NodeBase> NewChildren)
        {
            return new NodeStringOutput(Fields, NewChildren);
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
                    new FieldConstant(FieldDataType.DataTypeEnum.STRING, FieldDataType.DefaultValue(FieldDataType.DataTypeEnum.STRING)));
            //Add Fields here: mutableFields.Add(new FieldIdentifier(m_CodeName),
            //        new FieldSolutionName("A123"));

            ReadOnlyDictionary<FieldIdentifier, FieldBase> defaultFields =
                new ReadOnlyDictionary<FieldIdentifier, FieldBase>(mutableFields);
            return new NodeStringOutput(
                SetFieldDefaults(Fields, defaultFields), Children);
        }

        public static NodeStringOutput BuildWith(FieldIdentifier Code, FieldString Address, FieldSignalName OutputName)
        {
            //build fields
            Dictionary<FieldIdentifier, FieldBase> mutableFields =
                new Dictionary<FieldIdentifier, FieldBase>();
            mutableFields.Add(new FieldIdentifier(m_CodeName), Code);
            mutableFields.Add(new FieldIdentifier(m_AddressName), Address);
            mutableFields.Add(new FieldIdentifier(m_ForcedName), new FieldBool(false));
            mutableFields.Add(new FieldIdentifier(m_ForcedValueName), new FieldConstant(FieldDataType.DataTypeEnum.STRING, FieldDataType.DefaultValue(FieldDataType.DataTypeEnum.STRING)));
            mutableFields.Add(new FieldIdentifier(m_OutputNameName), OutputName);
            //Add Fields here: mutableFields.Add(new FieldIdentifier(m_CodeName), Code);

            //build children
            KeyedNodeCollection<NodeBase> mutableChildren =
                new KeyedNodeCollection<NodeBase>();
            mutableChildren.Add(NodeSignalIn.BuildWith(
                new FieldDataType(FieldDataType.DataTypeEnum.STRING),
                new FieldConstant(FieldDataType.DataTypeEnum.STRING, FieldDataType.DefaultValue(FieldDataType.DataTypeEnum.STRING))));
            //Add Children here: mutableChildren.Add(SomeChild);

            //build node
            NodeStringOutput Builder = new NodeStringOutput(
                new ReadOnlyDictionary<FieldIdentifier, FieldBase>(mutableFields),
                new ReadOnlyCollection<NodeBase>(mutableChildren));

            return Builder;
        }
        #endregion
    }
}
