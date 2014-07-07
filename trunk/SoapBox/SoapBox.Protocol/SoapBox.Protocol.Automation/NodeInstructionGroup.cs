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
    /// A group of instructions that can be edited as a block
    /// </summary>
    public sealed class NodeInstructionGroup : NodeBase
    {
        //readonly members
        public readonly TypedChildCollection<NodeInstruction, NodeInstructionGroup> NodeInstructionChildren = null;

        #region " FIELDS "
        
        #region " Language (FieldIdentifier) "
        public FieldIdentifier Language
        {
            get
            {
                return (FieldIdentifier)Fields[new FieldIdentifier(m_LanguageName)];
            }
        }
        public NodeInstructionGroup SetLanguage(FieldIdentifier Language)
        {
            if (Language == null)
            {
                throw new ArgumentNullException(m_LanguageName);
            }
            return new NodeInstructionGroup(this.SetField(new FieldIdentifier(m_LanguageName), Language), ChildCollection);
        }
        static readonly string m_LanguageName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeInstructionGroup>(o => o.Language);
        #endregion

        #region " InstructionGroupTry (NodeInstructionGroupTry) "
        private readonly SingleChild<NodeInstructionGroupTry, NodeInstructionGroup> m_InstructionGroupTry = null;
        public NodeInstructionGroupTry InstructionGroupTry
        {
            get
            {
                return m_InstructionGroupTry.Item;
            }
        }
        public NodeInstructionGroup SetInstructionGroupTry(NodeInstructionGroupTry NewInstructionGroupTry)
        {
            if (NewInstructionGroupTry == null)
            {
                throw new ArgumentNullException(m_InstructionGroupTryName);
            }
            return m_InstructionGroupTry.Set(NewInstructionGroupTry);
        }
        static readonly string m_InstructionGroupTryName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeInstructionGroup>(o => o.InstructionGroupTry);
        #endregion
        
        #region " Comment (FieldString) "
        public FieldString Comment
        {
            get
            {
                return (FieldString)Fields[new FieldIdentifier(m_CommentName)];
            }
        }
        public NodeInstructionGroup SetComment(FieldString Comment)
        {
            if (Comment == null)
            {
                throw new ArgumentNullException(m_CommentName);
            }
            return new NodeInstructionGroup(this.SetField(new FieldIdentifier(m_CommentName), Comment), ChildCollection);
        }
        static readonly string m_CommentName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeInstructionGroup>(o => o.Comment);
        #endregion

        #endregion

        #region " CONSTRUCTORS "
        private NodeInstructionGroup(ReadOnlyDictionary<FieldIdentifier, FieldBase> Fields,
            ReadOnlyCollection<NodeBase> Children)
            : base(Fields, Children)
        {
            NodeInstructionChildren = new TypedChildCollection<NodeInstruction, NodeInstructionGroup>(this);
            m_InstructionGroupTry = new SingleChild<NodeInstructionGroupTry, NodeInstructionGroup>(this);

            // validation
            if (Language == null)
            {
                throw new ArgumentNullException(m_LanguageName);
            }
            if (InstructionGroupTry == null)
            {
                throw new ArgumentNullException(m_InstructionGroupTryName);
            }
            if (Comment == null)
            {
                throw new ArgumentNullException(m_CommentName);
            }
        }

        protected override NodeBase CopyWithNewChildren(ReadOnlyCollection<NodeBase> NewChildren)
        {
            return new NodeInstructionGroup(Fields, NewChildren);
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
            return new NodeInstructionGroup(
                SetFieldDefaults(Fields, defaultFields), Children);
        }

        public static NodeInstructionGroup BuildWith(FieldIdentifier Language)
        {
            //build fields
            Dictionary<FieldIdentifier, FieldBase> mutableFields =
                new Dictionary<FieldIdentifier, FieldBase>();
            mutableFields.Add(new FieldIdentifier(m_LanguageName), Language);
            mutableFields.Add(new FieldIdentifier(m_CommentName), new FieldString());
            //Add Fields here: mutableFields.Add(new FieldIdentifier(m_CodeName), Code);

            //build children
            KeyedNodeCollection<NodeBase> mutableChildren =
                new KeyedNodeCollection<NodeBase>();
            mutableChildren.Add(NodeInstructionGroupTry.BuildWith());
            //Add Children here: mutableChildren.Add(SomeChild);

            //build node
            NodeInstructionGroup Builder = new NodeInstructionGroup(
                new ReadOnlyDictionary<FieldIdentifier, FieldBase>(mutableFields),
                new ReadOnlyCollection<NodeBase>(mutableChildren));

            return Builder;
        }
        #endregion
    }
}
