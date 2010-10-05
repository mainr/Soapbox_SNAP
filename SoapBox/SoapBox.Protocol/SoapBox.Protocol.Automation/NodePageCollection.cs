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
    /// A collection of pages, or other page collections.  This forms a folder structure,
    /// and really just exists for organization purposes.
    /// </summary>
    public sealed class NodePageCollection : NodeBase
    {
        //readonly members
        public readonly TypedChildCollection<NodePageCollection, NodePageCollection> NodePageCollectionChildren = null;
        public readonly TypedChildCollection<NodePage, NodePageCollection> NodePageChildren = null;

        #region " FIELDS "
        
        #region " PageCollectionName (FieldPageCollectionName) "
        public FieldPageCollectionName PageCollectionName
        {
            get
            {
                return (FieldPageCollectionName)Fields[new FieldIdentifier(m_PageCollectionNameName)];
            }
        }
        public NodePageCollection SetPageCollectionName(FieldPageCollectionName PageCollectionName)
        {
            if (PageCollectionName == null)
            {
                throw new ArgumentNullException(m_PageCollectionNameName);
            }
            return new NodePageCollection(this.SetField(new FieldIdentifier(m_PageCollectionNameName), PageCollectionName), ChildCollection);
        }
        static readonly string m_PageCollectionNameName =
            NotifyPropertyChangedHelper.GetPropertyName<NodePageCollection>(o => o.PageCollectionName);
        #endregion
        
        #region " LogicRoot (FieldBool) "
        public FieldBool LogicRoot
        {
            get
            {
                return (FieldBool)Fields[new FieldIdentifier(m_LogicRootName)];
            }
        }
        public NodePageCollection SetLogicRoot(FieldBool LogicRoot)
        {
            if (LogicRoot == null)
            {
                throw new ArgumentNullException(m_LogicRootName);
            }
            return new NodePageCollection(this.SetField(new FieldIdentifier(m_LogicRootName), LogicRoot), ChildCollection);
        }
        static readonly string m_LogicRootName =
            NotifyPropertyChangedHelper.GetPropertyName<NodePageCollection>(o => o.LogicRoot);
        #endregion

        #endregion

        #region " CONSTRUCTORS "
        private NodePageCollection(ReadOnlyDictionary<FieldIdentifier, FieldBase> Fields,
            ReadOnlyCollection<NodeBase> Children)
            : base(Fields, Children)
        {
            NodePageCollectionChildren = new TypedChildCollection<NodePageCollection, NodePageCollection>(this);
            NodePageChildren = new TypedChildCollection<NodePage, NodePageCollection>(this);

            //validation
            if (PageCollectionName == null)
            {
                throw new ArgumentNullException(m_PageCollectionNameName);
            }
            if (LogicRoot == null)
            {
                throw new ArgumentNullException(m_LogicRootName);
            }
        }

        protected override NodeBase CopyWithNewChildren(ReadOnlyCollection<NodeBase> NewChildren)
        {
            return new NodePageCollection(Fields, NewChildren);
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
            return new NodePageCollection(
                SetFieldDefaults(Fields, defaultFields), Children);
        }

        public static NodePageCollection BuildWith(FieldPageCollectionName PageCollectionName)
        {
            //build fields
            Dictionary<FieldIdentifier, FieldBase> mutableFields =
                new Dictionary<FieldIdentifier, FieldBase>();
            mutableFields.Add(new FieldIdentifier(m_PageCollectionNameName), PageCollectionName);
            mutableFields.Add(new FieldIdentifier(m_LogicRootName), new FieldBool(false));
            //Add Fields here: mutableFields.Add(new FieldIdentifier(m_CodeName), Code);

            //build children
            KeyedNodeCollection<NodeBase> mutableChildren =
                new KeyedNodeCollection<NodeBase>();
            //Add Children here: mutableChildren.Add(SomeChild);

            //build node
            NodePageCollection Builder = new NodePageCollection(
                new ReadOnlyDictionary<FieldIdentifier, FieldBase>(mutableFields),
                new ReadOnlyCollection<NodeBase>(mutableChildren));

            return Builder;
        }
        #endregion
    }
}
