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

namespace SoapBox.Protocol.Base
{
    /// <summary>
    /// Can be used for simple string-based message passing.
    /// MessageId is used to match responses to the send telegram
    /// MessageType is a string, and Payload is a Base64 encoded string
    /// </summary>
    public sealed class NodeTelegram : NodeBase
    {

        #region " FIELDS "
        
        #region " MessageId (FieldGuid) "
        public FieldGuid MessageId
        {
            get
            {
                return (FieldGuid)Fields[new FieldIdentifier(m_MessageIdName)];
            }
        }
        public NodeTelegram SetMessageId(FieldGuid MessageId)
        {
            if (MessageId == null)
            {
                throw new ArgumentNullException(m_MessageIdName);
            }
            return new NodeTelegram(this.SetField(new FieldIdentifier(m_MessageIdName), MessageId), ChildCollection);
        }
        static readonly string m_MessageIdName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeTelegram>(o => o.MessageId);
        #endregion

        #region " MessageType (FieldString) "
        public FieldString MessageType
        {
            get
            {
                return (FieldString)Fields[new FieldIdentifier(m_MessageTypeName)];
            }
        }
        public NodeTelegram SetMessageType(FieldString MessageType)
        {
            if (MessageType == null)
            {
                throw new ArgumentNullException(m_MessageTypeName);
            }
            return new NodeTelegram(this.SetField(new FieldIdentifier(m_MessageTypeName), MessageType), ChildCollection);
        }
        static readonly string m_MessageTypeName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeTelegram>(o => o.MessageType);
        #endregion
    
        #region " Payload (FieldBase64) "
        public FieldBase64 Payload
        {
            get
            {
                return (FieldBase64)Fields[new FieldIdentifier(m_PayloadName)];
            }
        }
        public NodeTelegram SetPayload(FieldBase64 Payload)
        {
            if (Payload == null)
            {
                throw new ArgumentNullException(m_PayloadName);
            }
            return new NodeTelegram(this.SetField(new FieldIdentifier(m_PayloadName), Payload), ChildCollection);
        }
        static readonly string m_PayloadName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeTelegram>(o => o.Payload);
        #endregion

        #endregion

        #region " CONSTRUCTORS "
        private NodeTelegram(ReadOnlyDictionary<FieldIdentifier, FieldBase> Fields,
            ReadOnlyCollection<NodeBase> Children)
            : base(Fields, Children)
        {
            // Validation
            if (MessageId == null)
            {
                throw new ArgumentNullException(m_MessageIdName);
            }
            if (MessageType == null)
            {
                throw new ArgumentNullException(m_MessageTypeName);
            }
            if (Payload == null)
            {
                throw new ArgumentNullException(m_PayloadName);
            }
        }

        protected override NodeBase CopyWithNewChildren(ReadOnlyCollection<NodeBase> NewChildren)
        {
            return new NodeTelegram(Fields, NewChildren);
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
            return new NodeTelegram(
                SetFieldDefaults(Fields, defaultFields), Children);
        }

        public static NodeTelegram BuildWith(FieldString MessageType)
        {
            return BuildWith(new FieldGuid(), MessageType, new FieldBase64());
        }

        public static NodeTelegram BuildWith(FieldGuid MessageId, FieldString MessageType, FieldBase64 Payload)
        {
            //build fields
            Dictionary<FieldIdentifier, FieldBase> mutableFields =
                new Dictionary<FieldIdentifier, FieldBase>();
            mutableFields.Add(new FieldIdentifier(m_MessageIdName), MessageId);
            mutableFields.Add(new FieldIdentifier(m_MessageTypeName), MessageType);
            mutableFields.Add(new FieldIdentifier(m_PayloadName), Payload);
            //Add Fields here: mutableFields.Add(new FieldIdentifier(m_CodeName), Code);

            //build children
            KeyedNodeCollection<NodeBase> mutableChildren =
                new KeyedNodeCollection<NodeBase>();
            //Add Children here: mutableChildren.Add(SomeChild);

            //build node
            NodeTelegram Builder = new NodeTelegram(
                new ReadOnlyDictionary<FieldIdentifier, FieldBase>(mutableFields),
                new ReadOnlyCollection<NodeBase>(mutableChildren));

            return Builder;
        }
        #endregion
    }
}
