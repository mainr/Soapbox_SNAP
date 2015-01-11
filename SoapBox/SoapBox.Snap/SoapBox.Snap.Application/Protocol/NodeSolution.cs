#region "SoapBox.Snap License"
/// <header module="SoapBox.Snap"> 
/// Copyright (C) 2009-2015 SoapBox Automation, All Rights Reserved.
/// Contact: SoapBox Automation Licencing (license@soapboxautomation.com)
/// 
/// This file is part of SoapBox Snap.
/// 
/// SoapBox Snap is free software: you can redistribute it and/or modify it
/// under the terms of the GNU General Public License as published by the 
/// Free Software Foundation, either version 3 of the License, or 
/// (at your option) any later version.
/// 
/// SoapBox Snap is distributed in the hope that it will be useful, but 
/// WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
/// GNU General Public License for more details.
/// 
/// You should have received a copy of the GNU General Public License along
/// with SoapBox Snap. If not, see <http://www.gnu.org/licenses/>.
/// </header>
#endregion
        
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using SoapBox.Protocol.Base;
using SoapBox.Protocol.Automation;
using SoapBox.Utilities;

namespace SoapBox.Snap.Application
{
    /// <summary>
    /// Represents an "automation solution"
    /// </summary>
    public sealed class NodeSolution : NodeBase
    {
        //readonly members
        public readonly TypedChildCollection<NodeRuntimeApplication, NodeSolution> NodeRuntimeApplicationChildren = null;

        #region " FIELDS "
        
        #region " SolutionName (FieldSolutionName) "
        public FieldSolutionName SolutionName
        {
            get
            {
                return (FieldSolutionName)Fields[new FieldIdentifier(m_SolutionNameName)];
            }
        }
        public NodeSolution SetSolutionName(FieldSolutionName SolutionName)
        {
            if (SolutionName == null)
            {
                throw new ArgumentNullException(m_SolutionNameName);
            }
            return new NodeSolution(this.SetField(new FieldIdentifier(m_SolutionNameName), SolutionName), ChildCollection);
        }
        static readonly string m_SolutionNameName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeSolution>(o => o.SolutionName);
        #endregion
        
        #region " Layout (FieldLayout) "
        public FieldLayout Layout
        {
            get
            {
                return (FieldLayout)Fields[new FieldIdentifier(m_LayoutName)];
            }
        }
        public NodeSolution SetLayout(FieldLayout Layout)
        {
            if (Layout == null)
            {
                throw new ArgumentNullException(m_LayoutName);
            }
            return new NodeSolution(this.SetField(new FieldIdentifier(m_LayoutName), Layout), ChildCollection);
        }
        static readonly string m_LayoutName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeSolution>(o => o.Layout);
        #endregion

        #endregion

        #region " CONSTRUCTORS "
        private NodeSolution(ReadOnlyDictionary<FieldIdentifier, FieldBase> Fields,
            ReadOnlyCollection<NodeBase> Children)
            : base(Fields, Children)
        {
            NodeRuntimeApplicationChildren = new TypedChildCollection<NodeRuntimeApplication, NodeSolution>(this);

            //validation
            if (SolutionName == null)
            {
                throw new ArgumentNullException(m_SolutionNameName);
            }
            if (Layout == null)
            {
                throw new ArgumentNullException(m_LayoutName);
            }
        }

        protected override NodeBase CopyWithNewChildren(ReadOnlyCollection<NodeBase> NewChildren)
        {
            return new NodeSolution(Fields, NewChildren);
        }
        #endregion

        #region " BUILDER(S) "

        private static NodeBase Resurrect(
            ReadOnlyDictionary<FieldIdentifier, FieldBase> Fields,
            ReadOnlyCollection<NodeBase> Children)
        {
            Dictionary<FieldIdentifier, FieldBase> mutableFields =
                new Dictionary<FieldIdentifier, FieldBase>();
            mutableFields.Add(new FieldIdentifier(m_SolutionNameName),
                    new FieldSolutionName(string.Empty));
            mutableFields.Add(new FieldIdentifier(m_LayoutName),
                    new FieldLayout(string.Empty));

            ReadOnlyDictionary<FieldIdentifier, FieldBase> defaultFields =
                new ReadOnlyDictionary<FieldIdentifier, FieldBase>(mutableFields);
            return new NodeSolution(
                SetFieldDefaults(Fields, defaultFields), Children);
        }

        public static NodeSolution BuildWith(FieldSolutionName SolutionName, FieldLayout Layout)
        {
            //build fields
            Dictionary<FieldIdentifier, FieldBase> mutableFields =
                new Dictionary<FieldIdentifier, FieldBase>();
            mutableFields.Add(new FieldIdentifier(m_SolutionNameName), SolutionName);
            mutableFields.Add(new FieldIdentifier(m_LayoutName), Layout);

            //build children
            KeyedNodeCollection<NodeBase> mutableChildren =
                new KeyedNodeCollection<NodeBase>();
            //Add Children here: mutableChildren.Add(SomeChild);

            //build node
            NodeSolution Builder = new NodeSolution(
                new ReadOnlyDictionary<FieldIdentifier, FieldBase>(mutableFields),
                new ReadOnlyCollection<NodeBase>(mutableChildren));

            return Builder;
        }
        #endregion
    }
}
        
