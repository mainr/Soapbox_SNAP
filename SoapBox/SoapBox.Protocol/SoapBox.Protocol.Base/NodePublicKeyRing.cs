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
using SoapBox.Protocol.Base;

namespace SoapBox.Protocol.Base
{
    /// <summary>
    /// Implements a collection of Public Key objects.  
    /// </summary>
    public sealed class NodePublicKeyRing : NodeBase
    {

        public readonly TypedChildCollection<NodePublicKey, NodePublicKeyRing> Keys = null;

        #region " CONSTRUCTORS "
        private NodePublicKeyRing(ReadOnlyDictionary<FieldIdentifier, FieldBase> Fields,
            ReadOnlyCollection<NodeBase> Children)
            : base(Fields, Children)
        {
            Keys = new TypedChildCollection<NodePublicKey, NodePublicKeyRing>(this);
        }

        protected override NodeBase CopyWithNewChildren(ReadOnlyCollection<NodeBase> NewChildren)
        {
            return new NodePublicKeyRing(Fields, NewChildren);
        }
        #endregion

        #region " BUILDER(S) "

        private static NodeBase Resurrect(
            ReadOnlyDictionary<FieldIdentifier, FieldBase> Fields,
            ReadOnlyCollection<NodeBase> Children)
        {
            return new NodePublicKeyRing(Fields, Children);
        }

        public static NodePublicKeyRing BuildWith()
        {
            //build fields
            Dictionary<FieldIdentifier, FieldBase> mutableFields =
                new Dictionary<FieldIdentifier, FieldBase>();

            //build children
            KeyedNodeCollection<NodeBase> mutableChildren =
                new KeyedNodeCollection<NodeBase>();

            //build node
            NodePublicKeyRing Builder = new NodePublicKeyRing(
                new ReadOnlyDictionary<FieldIdentifier, FieldBase>(mutableFields),
                new ReadOnlyCollection<NodeBase>(mutableChildren));

            return Builder;
        }

        #endregion

    }
}
