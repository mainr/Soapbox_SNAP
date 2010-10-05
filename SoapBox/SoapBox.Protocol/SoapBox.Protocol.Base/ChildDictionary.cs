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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SoapBox.Protocol.Base
{
    /// <summary>
    /// Implements a strongly typed collection of NodeBase derived
    /// objects, to represent a child collection of another NodeBase
    /// derived object.
    /// </summary>
    /// <typeparam name="TChild"></typeparam>
    /// <typeparam name="TParent"></typeparam>
    public sealed class ChildDictionary<TChild, TParent> : IEnumerable<TChild> 
        where TChild : NodeBase 
        where TParent : NodeBase
    {

        #region " INSTANTIATION AND MEMBERS "
        //readonly members
        private readonly TParent m_parent;

        public ChildDictionary(TParent Parent)
        {
            m_parent = Parent;
        }
        #endregion

        #region " INNER COLLECTION "
        //Lazy initialization
        //Represents the collection of child objects of the parent that
        //match the TChild type.
        private ReadOnlyDictionary<FieldGuid, TChild> m_Items = null;
        public ReadOnlyDictionary<FieldGuid, TChild> Items
        {
            get
            {
                if (m_Items == null)
                {
                    ReadOnlyDictionary<FieldGuid, NodeBase> ChildTags =
                        m_parent.GetChildrenByType(typeof(TChild));
                    Dictionary<FieldGuid, TChild> MutableChildTags =
                        new Dictionary<FieldGuid, TChild>();
                    foreach (KeyValuePair<FieldGuid, NodeBase> item in ChildTags)
                    {
                        MutableChildTags.Add(item.Key, (TChild)item.Value);
                    }
                    System.Threading.Interlocked.CompareExchange(ref m_Items,
                        new ReadOnlyDictionary<FieldGuid, TChild>(MutableChildTags), null);
                }
                return m_Items;
            }
        }
        #endregion

        #region " ADD, REMOVE, REPLACE "
        /// <summary>
        /// Adds a new child object to the collection.
        /// </summary>
        /// <param name="NewItem"></param>
        /// <returns></returns>
        public TParent Add(TChild NewItem)
        {
            if (NewItem == null)
            {
                return m_parent;
            }
            else
            {
                return (TParent)m_parent.CopyWithNewChildren(
                    m_parent.ReplaceChild(null, NewItem)
                    );
            }
        }

        /// <summary>
        /// Removes a child object from the collection.
        /// </summary>
        /// <param name="OldItem"></param>
        /// <returns></returns>
        public TParent Remove(TChild OldItem)
        {
            if (OldItem == null)
            {
                return m_parent;
            }
            else
            {
                return (TParent)m_parent.CopyWithNewChildren(
                    m_parent.ReplaceChild(OldItem, null)
                    );
            }
        }

        /// <summary>
        /// Removes one child object from the collection, and
        /// adds a new one on the end.
        /// </summary>
        /// <param name="OldItem"></param>
        /// <param name="NewItem"></param>
        /// <returns></returns>
        public TParent Replace(TChild OldItem, TChild NewItem)
        {
            if (OldItem == null && NewItem == null)
            {
                return m_parent;
            }
            else
            {
                return (TParent)m_parent.CopyWithNewChildren(
                    m_parent.ReplaceChild(OldItem, NewItem)
                    );
            }
        }

        /// <summary>
        /// Adds a list of child objects to the collection.
        /// </summary>
        /// <param name="NewItems"></param>
        /// <returns></returns>
        public TParent Add(ReadOnlyDictionary<FieldGuid, TChild> NewItems)
        {
            if (NewItems == null)
            {
                return m_parent;
            }
            else if (NewItems.Count == 0)
            {
                return m_parent;
            }
            else
            {
                ReadOnlyDictionary<FieldGuid, NodeBase> tempNewItems =
                    CastToBase(NewItems);
                return (TParent)m_parent.CopyWithNewChildren(
                    m_parent.ReplaceChildren(null, tempNewItems)
                    );
            }
        }

        /// <summary>
        /// Removes a list of child objects from the collection.
        /// </summary>
        /// <param name="OldItems"></param>
        /// <returns></returns>
        public TParent Remove(ReadOnlyDictionary<FieldGuid, TChild> OldItems)
        {
            if (OldItems == null)
            {
                return m_parent;
            }
            else if (OldItems.Count == 0)
            {
                return m_parent;
            }
            else
            {
                ReadOnlyDictionary<FieldGuid, NodeBase> tempOldItems =
                    CastToBase(OldItems);
                return (TParent)m_parent.CopyWithNewChildren(
                    m_parent.ReplaceChildren(tempOldItems, null)
                    );
            }
        }

        /// <summary>
        /// Removes a list of child objects from the collection, and 
        /// replaces them with a list of new child objects.
        /// </summary>
        /// <param name="OldItems"></param>
        /// <param name="NewItems"></param>
        /// <returns></returns>
        public TParent Replace(
            ReadOnlyDictionary<FieldGuid, TChild> OldItems,
            ReadOnlyDictionary<FieldGuid, TChild> NewItems)
        {
            if (OldItems == null && NewItems == null)
            {
                return m_parent;
            }
            else if (OldItems.Count == 0 && NewItems.Count == 0)
            {
                return m_parent;
            }
            else
            {
                ReadOnlyDictionary<FieldGuid, NodeBase> tempNewItems =
                    CastToBase(NewItems);
                ReadOnlyDictionary<FieldGuid, NodeBase> tempOldItems =
                    CastToBase(OldItems);
                return (TParent)m_parent.CopyWithNewChildren(
                    m_parent.ReplaceChildren(tempOldItems, tempNewItems)
                    );
            }
        }

        /// <summary>
        /// Creates a ReadOnlyCollection of NodeBase from any 
        /// ReadOnlyCollection of objects derived from NodeBase
        /// </summary>
        /// <param name="Items">Collection to copy</param>
        /// <returns>New Collection of Base Objects</returns>
        private ReadOnlyDictionary<FieldGuid, NodeBase> CastToBase(
            ReadOnlyDictionary<FieldGuid, TChild> Items)
        {
            Dictionary<FieldGuid, NodeBase> mutableItems = 
                new Dictionary<FieldGuid, NodeBase>();
            foreach (KeyValuePair<FieldGuid, TChild> item in Items)
            {
                mutableItems.Add(item.Key, (NodeBase)item.Value);
            }
            ReadOnlyDictionary<FieldGuid, NodeBase> tempItems =
                new ReadOnlyDictionary<FieldGuid, NodeBase>(mutableItems);
            return tempItems;
        }
        #endregion

        #region " IMMITATE A COLLECTION "
        public IEnumerator<TChild> GetEnumerator()
        {
            foreach (TChild item in Items.Values)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count
        {
            get
            {
                return Items.Count;
            }
        }

        public TChild this[int index]
        {
            get
            {
                return Items.ElementAt(index).Value;
            }
        }
        #endregion

    }
}
