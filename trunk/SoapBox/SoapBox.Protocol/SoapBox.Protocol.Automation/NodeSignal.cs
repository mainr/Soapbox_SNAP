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
    /// A signal is like a variable: a named place to store a given type of data
    /// </summary>
    public sealed class NodeSignal : NodeBase
    {
        #region "Value"
        /// <summary>
        /// Just a shortcut to the underlying stored value
        /// </summary>
        public object Value
        {
            get
            {
                if (Forced.BoolValue)
                {
                    return ForcedValue.Value;
                }
                else
                {
                    return Storage.Value;
                }
            }
            set
            {
                Storage.Value = value;
            }
        }
        #endregion

        #region "DataType"
        /// <summary>
        /// Just a shortcut to the underlying stored DataType
        /// </summary>
        public FieldDataType DataType
        {
            get
            {
                return Storage.DataType;
            }
        }
        #endregion

        #region " FIELDS "

        #region " SignalId (FieldGuid) "
        public FieldGuid SignalId
        {
            get
            {
                return (FieldGuid)Fields[new FieldIdentifier(m_SignalIdName)];
            }
        }
        public NodeSignal SetSignalId(FieldGuid SignalId)
        {
            if (SignalId == null)
            {
                throw new ArgumentNullException(m_SignalIdName);
            }
            return new NodeSignal(this.SetField(new FieldIdentifier(m_SignalIdName), SignalId), ChildCollection);
        }
        static readonly string m_SignalIdName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeSignal>(o => o.SignalId);
        #endregion

        #region " SignalName (FieldSignalname) "
        public FieldSignalName SignalName
        {
            get
            {
                return (FieldSignalName)Fields[new FieldIdentifier(m_SignalNameName)];
            }
        }
        public NodeSignal SetSignalName(FieldSignalName SignalName)
        {
            if (SignalName == null)
            {
                throw new ArgumentNullException(m_SignalNameName);
            }
            return new NodeSignal(this.SetField(new FieldIdentifier(m_SignalNameName), SignalName), ChildCollection);
        }
        static readonly string m_SignalNameName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeSignal>(o => o.SignalName);
        #endregion
        
        #region " Storage (NodeValue) "
        private readonly SingleChild<NodeValue, NodeSignal> m_Storage = null;
        public NodeValue Storage
        {
            get
            {
                return m_Storage.Item;
            }
        }
        //public NodeSignal SetStorage(NodeValue NewStorage)
        //{
        //    if (NewStorage == null)
        //    {
        //        throw new ArgumentNullException(m_StorageName);
        //    }
        //    return m_Storage.Set(NewStorage);
        //}
        static readonly string m_StorageName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeSignal>(o => o.Storage);
        #endregion
        
        #region " Forced (FieldBool) "
        public FieldBool Forced
        {
            get
            {
                return (FieldBool)Fields[new FieldIdentifier(m_ForcedName)];
            }
        }
        public NodeSignal SetForced(FieldBool Forced)
        {
            if (Forced == null)
            {
                throw new ArgumentNullException(m_ForcedName);
            }
            return new NodeSignal(this.SetField(new FieldIdentifier(m_ForcedName), Forced), ChildCollection);
        }
        static readonly string m_ForcedName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeSignal>(o => o.Forced);
        #endregion
        
        #region " ForcedValue (FieldConstant) "
        public FieldConstant ForcedValue
        {
            get
            {
                return (FieldConstant)Fields[new FieldIdentifier(m_ForcedValueName)];
            }
        }
        public NodeSignal SetForcedValue(FieldConstant ForcedValue)
        {
            if (ForcedValue == null)
            {
                throw new ArgumentNullException(m_ForcedValueName);
            }
            return new NodeSignal(this.SetField(new FieldIdentifier(m_ForcedValueName), ForcedValue), ChildCollection);
        }
        static readonly string m_ForcedValueName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeSignal>(o => o.ForcedValue);
        #endregion
        
        #region " Comment (FieldString) "
        public FieldString Comment
        {
            get
            {
                return (FieldString)Fields[new FieldIdentifier(m_CommentName)];
            }
        }
        public NodeSignal SetComment(FieldString Comment)
        {
            if (Comment == null)
            {
                throw new ArgumentNullException(m_CommentName);
            }
            return new NodeSignal(this.SetField(new FieldIdentifier(m_CommentName), Comment), ChildCollection);
        }
        static readonly string m_CommentName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeSignal>(o => o.Comment);
        #endregion

        #endregion

        #region " CONSTRUCTORS "
        private NodeSignal(ReadOnlyDictionary<FieldIdentifier, FieldBase> Fields,
            ReadOnlyCollection<NodeBase> Children)
            : base(Fields, Children)
        {
            m_Storage = new SingleChild<NodeValue, NodeSignal>(this);

            //validation
            if (SignalId == null)
            {
                throw new ArgumentNullException(m_SignalIdName);
            }
            if (SignalName == null)
            {
                throw new ArgumentNullException(m_SignalNameName);
            }
            if (Storage == null)
            {
                throw new ArgumentNullException(m_StorageName);
            }
            if (Forced == null)
            {
                throw new ArgumentNullException(m_ForcedName);
            }
            if (ForcedValue == null)
            {
                throw new ArgumentNullException(m_ForcedValueName);
            }
            if (Comment == null)
            {
                throw new ArgumentNullException(m_CommentName);
            }

            if (Storage.DataType.DataType != ForcedValue.DataType)
            {
                throw new ArgumentOutOfRangeException(m_ForcedValueName);
            }
        }

        protected override NodeBase CopyWithNewChildren(ReadOnlyCollection<NodeBase> NewChildren)
        {
            return new NodeSignal(Fields, NewChildren);
        }
        #endregion

        #region " BUILDER(S) "

        private static NodeBase Resurrect(
            ReadOnlyDictionary<FieldIdentifier, FieldBase> Fields,
            ReadOnlyCollection<NodeBase> Children)
        {
            Dictionary<FieldIdentifier, FieldBase> mutableFields =
                new Dictionary<FieldIdentifier, FieldBase>();
            mutableFields.Add(new FieldIdentifier(m_CommentName),
                    new FieldString());
            //Add Fields here: mutableFields.Add(new FieldIdentifier(m_CodeName),
            //        new FieldSolutionName("A123"));

            ReadOnlyDictionary<FieldIdentifier, FieldBase> defaultFields =
                new ReadOnlyDictionary<FieldIdentifier, FieldBase>(mutableFields);
            return new NodeSignal(
                SetFieldDefaults(Fields, defaultFields), Children);
        }

        public static NodeSignal BuildWith(FieldSignalName SignalName,
            FieldDataType DataType, FieldBool Forced,
            FieldConstant ForcedValue)
        {
            //build fields
            Dictionary<FieldIdentifier, FieldBase> mutableFields =
                new Dictionary<FieldIdentifier, FieldBase>();
            mutableFields.Add(new FieldIdentifier(m_SignalIdName), new FieldGuid());
            mutableFields.Add(new FieldIdentifier(m_SignalNameName), SignalName);
            mutableFields.Add(new FieldIdentifier(m_ForcedName), Forced);
            mutableFields.Add(new FieldIdentifier(m_ForcedValueName), ForcedValue);
            mutableFields.Add(new FieldIdentifier(m_CommentName), new FieldString());
            //Add Fields here: mutableFields.Add(new FieldIdentifier(m_CodeName), Code);

            //build children
            KeyedNodeCollection<NodeBase> mutableChildren =
                new KeyedNodeCollection<NodeBase>();
            mutableChildren.Add(NodeValue.BuildWith(DataType));
            //Add Children here: mutableChildren.Add(SomeChild);

            //build node
            NodeSignal Builder = new NodeSignal(
                new ReadOnlyDictionary<FieldIdentifier, FieldBase>(mutableFields),
                new ReadOnlyCollection<NodeBase>(mutableChildren));

            return Builder;
        }
        #endregion

        public static NodeSignal BuildBooleanSignal(string signalName)
        {
            return buildHelper(signalName, FieldDataType.DataTypeEnum.BOOL, false);
        }

        public static NodeSignal BuildNumberSignal(string signalName)
        {
            return buildHelper(signalName, FieldDataType.DataTypeEnum.NUMBER, 0);
        }

        public static NodeSignal BuildStringSignal(string signalName)
        {
            return buildHelper(signalName, FieldDataType.DataTypeEnum.STRING, string.Empty);
        }

        private static NodeSignal buildHelper(string signalName, FieldDataType.DataTypeEnum dataType, object defaultValue)
        {
            var signal = BuildWith(new FieldSignalName(signalName),
                new FieldDataType(dataType),
                new FieldBool(false), new FieldConstant(dataType, defaultValue));
            return signal;
        }
    }
}
