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
    /// Represents a "signal reader" like an input to an instruction.
    /// </summary>
    public sealed class NodeSignalIn : NodeBase
    {
        public NodeValue GetValue(NodeRuntimeApplication runtimeApplication)
        {
            if (runtimeApplication == null)
            {
                throw new ArgumentNullException();
            }
            if (SignalId != null)
            {
                var signal = runtimeApplication.FindSignal(SignalId);
                if (signal != null)
                {
                    return signal.Storage;
                }
                else
                {
                    return null;
                }
            }
            else if (Literal != null)
            {
                NodeValue retVal = null;
                lock (m_LiteralNodeValue_Lock)
                {
                    if (m_LiteralNodeValue == null)
                    {
                        m_LiteralNodeValue = NodeValue.BuildWith(new FieldDataType(Literal.DataType));
                        m_LiteralNodeValue.Value = Literal.Value;
                    }
                    retVal = m_LiteralNodeValue;
                }
                return retVal;
            }
            else
            {
                return null;
            }
        }

        private NodeValue m_LiteralNodeValue = null;
        private readonly object m_LiteralNodeValue_Lock = new object();

        #region " FIELDS "
        
        #region " SignalId (FieldGuid) "
        /// <summary>
        /// Can be null
        /// </summary>
        public FieldGuid SignalId
        {
            get
            {
                var fi = new FieldIdentifier(m_SignalIdName);
                if (Fields.ContainsKey(fi))
                {
                    return (FieldGuid)Fields[fi];
                }
                else
                {
                    return null;
                }
            }
        }
        public NodeSignalIn SetSignalId(FieldGuid SignalId)
        {
            return new NodeSignalIn(this.SetField(new FieldIdentifier(m_SignalIdName), SignalId), ChildCollection);
        }
        static readonly string m_SignalIdName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeSignalIn>(o => o.SignalId);
        #endregion
        
        #region " Literal (FieldConstant) "
        /// <summary>
        /// Can be null
        /// </summary>
        public FieldConstant Literal
        {
            get
            {
                var fi = new FieldIdentifier(m_LiteralName);
                if (Fields.ContainsKey(fi))
                {
                    return (FieldConstant)Fields[fi];
                }
                else
                {
                    return null;
                }
            }
        }
        public NodeSignalIn SetLiteral(FieldConstant Literal)
        {

            return new NodeSignalIn(this.SetField(new FieldIdentifier(m_LiteralName), Literal), ChildCollection);
        }
        static readonly string m_LiteralName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeSignalIn>(o => o.Literal);
        #endregion
        
        #region " DataType (FieldDataType) "
        public FieldDataType DataType
        {
            get
            {
                return (FieldDataType)Fields[new FieldIdentifier(m_DataTypeName)];
            }
        }
        public NodeSignalIn SetDataType(FieldDataType DataType)
        {
            if (DataType == null)
            {
                throw new ArgumentNullException(m_DataTypeName);
            }
            return new NodeSignalIn(this.SetField(new FieldIdentifier(m_DataTypeName), DataType), ChildCollection);
        }
        static readonly string m_DataTypeName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeSignalIn>(o => o.DataType);
        #endregion
        
        #region " CompatibleTypes (FieldDataType) "
        public FieldDataType CompatibleTypes
        {
            get
            {
                return (FieldDataType)Fields[new FieldIdentifier(m_CompatibleTypesName)];
            }
        }
        public NodeSignalIn SetCompatibleTypes(FieldDataType CompatibleTypes)
        {
            if (CompatibleTypes == null)
            {
                throw new ArgumentNullException(m_CompatibleTypesName);
            }
            return new NodeSignalIn(this.SetField(new FieldIdentifier(m_CompatibleTypesName), CompatibleTypes), ChildCollection);
        }
        static readonly string m_CompatibleTypesName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeSignalIn>(o => o.CompatibleTypes);
        #endregion

        #endregion

        #region " CONSTRUCTORS "
        private NodeSignalIn(ReadOnlyDictionary<FieldIdentifier, FieldBase> Fields,
            ReadOnlyCollection<NodeBase> Children)
            : base(Fields, Children)
        {
            // validation
            if (SignalId == null && Literal == null)
            {
                throw new InvalidOperationException();
            }
            if (SignalId != null && Literal != null)
            {
                throw new InvalidOperationException();
            }
            if (DataType == null)
            {
                throw new ArgumentNullException(m_DataTypeName);
            }
            if (CompatibleTypes == null)
            {
                throw new ArgumentNullException(m_CompatibleTypesName);
            }
        }

        protected override NodeBase CopyWithNewChildren(ReadOnlyCollection<NodeBase> NewChildren)
        {
            return new NodeSignalIn(Fields, NewChildren);
        }
        #endregion

        #region " BUILDER(S) "

        private static NodeBase Resurrect(
            ReadOnlyDictionary<FieldIdentifier, FieldBase> Fields,
            ReadOnlyCollection<NodeBase> Children)
        {
            Dictionary<FieldIdentifier, FieldBase> mutableFields =
                new Dictionary<FieldIdentifier, FieldBase>();
            mutableFields.Add(new FieldIdentifier(m_DataTypeName),
                    new FieldDataType(FieldDataType.DataTypeEnum.BOOL));
            //Add Fields here: mutableFields.Add(new FieldIdentifier(m_CodeName),
            //        new FieldSolutionName("A123"));

            ReadOnlyDictionary<FieldIdentifier, FieldBase> defaultFields =
                new ReadOnlyDictionary<FieldIdentifier, FieldBase>(mutableFields);
            return new NodeSignalIn(
                SetFieldDefaults(Fields, defaultFields), Children);
        }

        public static NodeSignalIn BuildWith(FieldDataType DataType, FieldGuid SignalId)
        {
            return BuildWith(DataType, DataType, SignalId);
        }

        public static NodeSignalIn BuildWith(FieldDataType DataType, FieldDataType CompatibleTypes, FieldGuid SignalId)
        {
            //build fields
            Dictionary<FieldIdentifier, FieldBase> mutableFields =
                new Dictionary<FieldIdentifier, FieldBase>();
            mutableFields.Add(new FieldIdentifier(m_DataTypeName), DataType);
            mutableFields.Add(new FieldIdentifier(m_CompatibleTypesName), CompatibleTypes);
            mutableFields.Add(new FieldIdentifier(m_SignalIdName), SignalId);
            //Add Fields here: mutableFields.Add(new FieldIdentifier(m_CodeName), Code);

            //build children
            KeyedNodeCollection<NodeBase> mutableChildren =
                new KeyedNodeCollection<NodeBase>();
            //Add Children here: mutableChildren.Add(SomeChild);

            //build node
            NodeSignalIn Builder = new NodeSignalIn(
                new ReadOnlyDictionary<FieldIdentifier, FieldBase>(mutableFields),
                new ReadOnlyCollection<NodeBase>(mutableChildren));

            return Builder;
        }

        public static NodeSignalIn BuildWith(FieldDataType DataType, FieldConstant Literal)
        {
            return BuildWith(DataType, DataType, Literal);
        }

        public static NodeSignalIn BuildWith(FieldDataType DataType, FieldDataType CompatibleTypes, FieldConstant Literal)
        {
            //build fields
            Dictionary<FieldIdentifier, FieldBase> mutableFields =
                new Dictionary<FieldIdentifier, FieldBase>();
            mutableFields.Add(new FieldIdentifier(m_DataTypeName), DataType);
            mutableFields.Add(new FieldIdentifier(m_CompatibleTypesName), CompatibleTypes);
            mutableFields.Add(new FieldIdentifier(m_LiteralName), Literal);
            //Add Fields here: mutableFields.Add(new FieldIdentifier(m_CodeName), Code);

            //build children
            KeyedNodeCollection<NodeBase> mutableChildren =
                new KeyedNodeCollection<NodeBase>();
            //Add Children here: mutableChildren.Add(SomeChild);

            //build node
            NodeSignalIn Builder = new NodeSignalIn(
                new ReadOnlyDictionary<FieldIdentifier, FieldBase>(mutableFields),
                new ReadOnlyCollection<NodeBase>(mutableChildren));

            return Builder;
        }
        #endregion
    }
}
