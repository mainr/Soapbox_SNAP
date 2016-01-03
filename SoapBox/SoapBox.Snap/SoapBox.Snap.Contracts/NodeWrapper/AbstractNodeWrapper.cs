#region "SoapBox.Snap License"
/// <header module="SoapBox.Snap"> 
/// Copyright (C) 2009-2016 SoapBox Automation, All Rights Reserved.
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
using System.Linq;
using System.Text;
using SoapBox.Core;
using SoapBox.Protocol.Base;
using System.ComponentModel;
using System.Collections.ObjectModel;
using SoapBox.Utilities;

namespace SoapBox.Snap
{
    public class AbstractNodeWrapper : AbstractControl, INodeWrapper
    {

        public AbstractNodeWrapper(INodeWrapper parent)
        {
            Parent = parent;
        }

        #region " Node "
        /// <summary>
        /// This is the SoapBox Protocol Node.
        /// Can be null.
        /// </summary>
        public NodeBase Node
        {
            get
            {
                return m_Node;
            }
            protected set
            {
                if (m_Node != value)
                {
                    NodeBase oldNode = m_Node;
                    m_Node = value;
                    NotifyPropertyChanged(m_NodeArgs);
                    if (oldNode == null)
                    {
                        logger.Info(this.GetType().ToString() + " setting node from null to " + value.ID.ToString());
                    }
                    if (oldNode != null && value != null)
                    {
                        logger.Info(this.GetType().ToString() + " setting node from " + oldNode.ID.ToString() + " to " + value.ID.ToString());
                        FireEditedEvent(this, oldNode, value);
                    }
                    else if (oldNode != null && value == null)
                    {
                        FireDeletedEvent(this, oldNode);
                    }
                }
            }
        }
        private NodeBase m_Node;
        static readonly PropertyChangedEventArgs m_NodeArgs =
            NotifyPropertyChangedHelper.CreateArgs<AbstractNodeWrapper>(o => o.Node);
        static readonly string m_NodeName =
            NotifyPropertyChangedHelper.GetPropertyName<AbstractNodeWrapper>(o => o.Node);

        // Used for undo
        public void StealthSetNode(NodeBase value)
        {
            if (m_Node != value)
            {
                m_Node = value;
                NotifyPropertyChanged(m_NodeArgs);
            }
        }
        #endregion

        public event EditedHandler Edited = delegate { };
        public event DeletedHandler Deleted = delegate { };
        protected bool disableEditedDeletedEvents = false;

        protected void FireEditedEvent(INodeWrapper sender, NodeBase oldNode, NodeBase newNode)
        {
            if (!disableEditedDeletedEvents)
            {
                logger.Info("Firing Edited Event in item " + this.GetType().ToString());
                Edited(this, oldNode, newNode);
            }
        }

        protected void FireDeletedEvent(INodeWrapper sender, NodeBase deletedNode)
        {
            if (!disableEditedDeletedEvents)
            {
                logger.Info("Firing Deleted Event in item " + this.GetType().ToString());
                Deleted(this, deletedNode);
            }
        }

        #region " Items "
        public ObservableCollection<INodeWrapper> Items
        {
            get
            {
                return m_Items;
            }
            protected set
            {
                if (m_Items != value)
                {
                    m_Items = value;
                    NotifyPropertyChanged(m_ItemsArgs);
                }
            }
        }
        protected ObservableCollection<INodeWrapper> m_Items = new ObservableCollection<INodeWrapper>();
        static readonly PropertyChangedEventArgs m_ItemsArgs =
            NotifyPropertyChangedHelper.CreateArgs<INodeWrapper>(o => o.Items);

        /// <summary>
        /// Finds the item (if it exists) in the Items collection 
        /// matching the given nodeId.  It is *not* recursive.
        /// </summary>
        public INodeWrapper FindItemByNodeId(FieldGuid nodeId)
        {
            INodeWrapper retVal = null;
            foreach (var item in Items)
            {
                if (item.Node != null && item.Node.ID == nodeId)
                {
                    retVal = item;
                    break;
                }
            }
            return retVal;
        }
        #endregion

        /// <summary>
        /// This implements a depth first search looking for children of type T
        /// </summary>
        public IEnumerable<T> FindChildrenOfType<T>() where T : INodeWrapper
        {
            var retVal = new List<T>();
            var stk = new Stack<INodeWrapper>();
            stk.Push(this);
            while (stk.Count > 0)
            {
                var next = stk.Pop();
                foreach (var item in next.Items)
                {
                    stk.Push(item);
                }
                if (next is T && next != this)
                {
                    retVal.Add((T)next);
                }
            }
            return retVal;
        }

        #region " Parent "
        public INodeWrapper Parent
        {
            get
            {
                return m_Parent;
            }
            set
            {
                if (m_Parent != value)
                {
                    m_Parent = value;
                    NotifyPropertyChanged(m_ParentArgs);
                }
            }
        }
        private INodeWrapper m_Parent = null;
        private static PropertyChangedEventArgs m_ParentArgs =
            NotifyPropertyChangedHelper.CreateArgs<AbstractNodeWrapper>(o => o.Parent);
        #endregion

        public INodeWrapper FindAncestorOfType(Type findType)
        {
            INodeWrapper parent = Parent;
            while (parent != null)
            {
                if (parent.GetType() == findType)
                {
                    break;
                }
                parent = parent.Parent;
            }
            return parent;
        }

        public INodeWrapper FindAncestorByNodeId(FieldGuid nodeId)
        {
            INodeWrapper parent = Parent;
            while (parent != null)
            {
                if (parent.Node.ID == nodeId)
                {
                    break;
                }
                parent = parent.Parent;
            }
            return parent;
        }

        #region " IsSelected "
        public bool IsSelected
        {
            get
            {
                return m_IsSelected;
            }
            set
            {
                if (m_IsSelected != value)
                {
                    m_IsSelected = value;
                    IsSelectedChanged();
                    NotifyPropertyChanged(m_IsSelectedArgs);
                }
            }
        }
        private bool m_IsSelected = false;
        private static readonly PropertyChangedEventArgs m_IsSelectedArgs =
            NotifyPropertyChangedHelper.CreateArgs<AbstractNodeWrapper>(o => o.IsSelected);
        private static string m_IsSelectedName =
            NotifyPropertyChangedHelper.GetPropertyName<AbstractNodeWrapper>(o => o.IsSelected);

        protected virtual void IsSelectedChanged() { }
        #endregion

    }
}
