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
    /// This kind of represents a memory location.  Each signal needs one of these.
    /// </summary>
    public sealed class NodeValue : NodeBase
    {

        #region " Value "
        /// <summary>
        /// NOT a permanent field.  Just a threadsafe
        /// spot to store the current signal value during
        /// the logic solve or display.
        /// 
        /// Note: uses boxed values, and could be null 
        /// </summary>
        public object Value
        {
            get
            {
                object retVal = null;
                lock (m_ValueLock)
                {
                    retVal = m_Value;
                }
                if (retVal == null)
                {
                    return FieldDataType.DefaultValue(DataType.DataType);
                }
                else if (retVal.GetType() == typeof(DateTime))
                {
                    // make a new object to hide the mutable one
                    DateTime dt = (DateTime)retVal;
                    return new DateTime(dt.Ticks);
                }
                else
                {
                    return retVal;
                }
            }
            set
            {
                if (!FieldDataType.CheckType(value, DataType.DataType))
                {
                    throw new ArgumentOutOfRangeException(m_ValueName);
                }
                // if it's bool, int, etc., then we will box it, which makes a copy
                // if it's a string, it's immutable, so we're threadsafe
                // if it's a date time, it's mutable, so we should make a copy
                object o = value;
                if (value.GetType() == typeof(DateTime))
                {
                    DateTime dt = (DateTime)value;
                    o = new DateTime(dt.Ticks);
                }
                lock (m_ValueLock)
                {
                    m_Value = o;
                }
            }
        }
        private object m_Value = null;
        private readonly object m_ValueLock = new object();
        static readonly string m_ValueName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeSignal>(o => o.Value);

        #endregion

        #region " FIELDS "
        
        #region " DataType (FieldDataType) "
        public FieldDataType DataType
        {
            get
            {
                return (FieldDataType)Fields[new FieldIdentifier(m_DataTypeName)];
            }
        }
        //public NodeValue SetDataType(FieldDataType DataType)
        //{
        //    if (DataType == null)
        //    {
        //        throw new ArgumentNullException(m_DataTypeName);
        //    }
        //    return new NodeValue(this.SetField(new FieldIdentifier(m_DataTypeName), DataType), ChildCollection);
        //}
        static readonly string m_DataTypeName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeValue>(o => o.DataType);
        #endregion

        #endregion

        #region " CONSTRUCTORS "
        private NodeValue(ReadOnlyDictionary<FieldIdentifier, FieldBase> Fields,
            ReadOnlyCollection<NodeBase> Children)
            : base(Fields, Children)
        {

            //validation
            if (DataType == null)
            {
                throw new ArgumentNullException(m_DataTypeName);
            }

            Value = FieldDataType.DefaultValue(DataType.DataType);
        }

        protected override NodeBase CopyWithNewChildren(ReadOnlyCollection<NodeBase> NewChildren)
        {
            return new NodeValue(Fields, NewChildren);
        }
        #endregion

        #region " BUILDER(S) "

        private static NodeBase Resurrect(
            ReadOnlyDictionary<FieldIdentifier, FieldBase> Fields,
            ReadOnlyCollection<NodeBase> Children)
        {
            Dictionary<FieldIdentifier, FieldBase> mutableFields =
                new Dictionary<FieldIdentifier, FieldBase>();
            //Add Fields here: mutableFields.Add(new FieldIdentifier(m_CodeName),
            //        new FieldSolutionName("A123"));

            ReadOnlyDictionary<FieldIdentifier, FieldBase> defaultFields =
                new ReadOnlyDictionary<FieldIdentifier, FieldBase>(mutableFields);
            return new NodeValue(
                SetFieldDefaults(Fields, defaultFields), Children);
        }

        public static NodeValue BuildWith(FieldDataType DataType)
        {
            //build fields
            Dictionary<FieldIdentifier, FieldBase> mutableFields =
                new Dictionary<FieldIdentifier, FieldBase>();
            mutableFields.Add(new FieldIdentifier(m_DataTypeName), DataType);
            //Add Fields here: mutableFields.Add(new FieldIdentifier(m_CodeName), Code);

            //build children
            KeyedNodeCollection<NodeBase> mutableChildren =
                new KeyedNodeCollection<NodeBase>();
            //Add Children here: mutableChildren.Add(SomeChild);

            //build node
            NodeValue Builder = new NodeValue(
                new ReadOnlyDictionary<FieldIdentifier, FieldBase>(mutableFields),
                new ReadOnlyCollection<NodeBase>(mutableChildren));

            return Builder;
        }
        #endregion
    }
}
