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
    /// 
    /// </summary>
    public sealed class NodeInstruction : NodeBase
    {
        //readonly members
        public readonly TypedChildCollection<NodeInstruction, NodeInstruction> NodeInstructionChildren = null;
        public readonly TypedChildCollection<NodeSignalIn, NodeInstruction> NodeSignalInChildren = null;
        public readonly TypedChildCollection<NodeSignal, NodeInstruction> NodeSignalChildren = null;

        #region " FIELDS "
        
        #region " InstructionType (FieldInstructionType) "
        public FieldInstructionType InstructionType
        {
            get
            {
                return (FieldInstructionType)Fields[new FieldIdentifier(m_InstructionTypeName)];
            }
        }
        public NodeInstruction SetInstructionType(FieldInstructionType InstructionType)
        {
            if (InstructionType == null)
            {
                throw new ArgumentNullException(m_InstructionTypeName);
            }
            return new NodeInstruction(this.SetField(new FieldIdentifier(m_InstructionTypeName), InstructionType), ChildCollection);
        }
        static readonly string m_InstructionTypeName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeInstruction>(o => o.InstructionType);
        #endregion

        #region " Comment (FieldString) "
        public FieldString Comment
        {
            get
            {
                return (FieldString)Fields[new FieldIdentifier(m_CommentName)];
            }
        }
        public NodeInstruction SetComment(FieldString Comment)
        {
            if (Comment == null)
            {
                throw new ArgumentNullException(m_CommentName);
            }
            return new NodeInstruction(this.SetField(new FieldIdentifier(m_CommentName), Comment), ChildCollection);
        }
        static readonly string m_CommentName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeInstruction>(o => o.Comment);
        #endregion

        #endregion

        #region " CONSTRUCTORS "
        private NodeInstruction(ReadOnlyDictionary<FieldIdentifier, FieldBase> Fields,
            ReadOnlyCollection<NodeBase> Children)
            : base(Fields, Children)
        {
            NodeInstructionChildren = new TypedChildCollection<NodeInstruction, NodeInstruction>(this);
            NodeSignalInChildren = new TypedChildCollection<NodeSignalIn, NodeInstruction>(this);
            NodeSignalChildren = new TypedChildCollection<NodeSignal, NodeInstruction>(this);

            // validation
            if (InstructionType == null)
            {
                throw new ArgumentNullException(m_InstructionTypeName);
            }
            if (Comment == null)
            {
                throw new ArgumentNullException(m_CommentName);
            }
        }

        protected override NodeBase CopyWithNewChildren(ReadOnlyCollection<NodeBase> NewChildren)
        {
            return new NodeInstruction(Fields, NewChildren);
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
            return new NodeInstruction(
                SetFieldDefaults(Fields, defaultFields), Children);
        }

        public static NodeInstruction BuildWith(FieldInstructionType InstructionType)
        {
            //build fields
            Dictionary<FieldIdentifier, FieldBase> mutableFields =
                new Dictionary<FieldIdentifier, FieldBase>();
            mutableFields.Add(new FieldIdentifier(m_InstructionTypeName), InstructionType);
            mutableFields.Add(new FieldIdentifier(m_CommentName), new FieldString());
            //Add Fields here: mutableFields.Add(new FieldIdentifier(m_CodeName), Code);

            //build children
            KeyedNodeCollection<NodeBase> mutableChildren =
                new KeyedNodeCollection<NodeBase>();
            //Add Children here: mutableChildren.Add(SomeChild);

            //build node
            NodeInstruction Builder = new NodeInstruction(
                new ReadOnlyDictionary<FieldIdentifier, FieldBase>(mutableFields),
                new ReadOnlyCollection<NodeBase>(mutableChildren));

            return Builder;
        }
        #endregion
    }
}
