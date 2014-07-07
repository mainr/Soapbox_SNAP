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

namespace SoapBox.Protocol.Base
{
    /// <summary>
    /// Implements a concrete KeyedCollection of Nodes.  The intent is
    /// to wrap this in either a ReadOnlyCollection or ReadOnlyDictionary
    /// depending on the need.
    /// </summary>
    /// <typeparam name="T">Any class derived from NodeBase</typeparam>
    public class KeyedNodeCollection<T> 
        : KeyedCollection<FieldGuid, T> 
        where T : NodeBase 
    {
        public KeyedNodeCollection()
            : base()
        {
        }

        public KeyedNodeCollection(IList<T> Items)
            : base()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                this.Add(Items[i]);
            }
        }

        protected override FieldGuid GetKeyForItem(T item)
        {
            return item.ID;
        }

        //I'm not sure if we need this, but ran across this:
        //http://www.devnewsgroups.net/group/microsoft.public.dotnet.framework/topic62318.aspx
        public new bool Contains(T item)
        {
            return this.Contains(item.ID);
        }

        public new IDictionary<FieldGuid, T> Dictionary
        {
            get
            {
                if (base.Dictionary == null)
                {
                    return new Dictionary<FieldGuid, T>();
                }
                else
                {
                    return (IDictionary<FieldGuid, T>)base.Dictionary;
                }
            }
        }
    }
}
