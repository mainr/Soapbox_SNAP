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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SoapBox.Protocol.Base
{
    /// <summary>
    /// Implements a strongly typed single NodeBase child object.
    /// </summary>
    /// <typeparam name="TChild"></typeparam>
    /// <typeparam name="TParent"></typeparam>
    public sealed class SingleChild<TChild, TParent> 
        where TChild : NodeBase 
        where TParent : NodeBase
    {

        #region " INSTANTIATION AND MEMBERS "
        //readonly members
        private readonly TParent m_parent;

        public SingleChild(TParent Parent)
        {
            m_parent = Parent;
        }
        #endregion

        #region " INNER CHILD "
        //Lazy initialization
        //Represents the child object of the parent that
        //matches the TChild type.
        private ReadOnlyDictionary<FieldGuid, TChild> m_Items = null;
        public TChild Item
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
                if (m_Items.Count == 1)
                {
                    return (TChild)m_Items.First().Value;
                }
                else if (m_Items.Count > 1)
                {
                    throw new InvalidOperationException();
                }
                else
                {
                    return null;
                }
            }
        }
        #endregion

        #region " SET "
        /// <summary>
        /// Replaces the existing child with this new one.
        /// </summary>
        /// <param name="NewItem"></param>
        /// <returns></returns>
        public TParent Set(TChild NewItem)
        {
            return (TParent)m_parent.CopyWithNewChildren(
                m_parent.ReplaceChild(Item, NewItem)
                );
        }

        #endregion

    }
}
