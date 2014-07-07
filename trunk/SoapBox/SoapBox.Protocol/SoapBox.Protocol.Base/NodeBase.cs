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
using System.Xml.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Text;
using System.IO;
using System.Reflection;
using System.Collections;
using SoapBox.Utilities;

namespace SoapBox.Protocol.Base
{
    /// <summary>
    /// An immutable base class for SoapBox objects that are passed from
    /// node to node over the SoapBox Protocol.
    /// </summary>
    public abstract class NodeBase
    {
        #region " INSTANTIATION AND MEMBERS "
        //readonly properties
        /* I really hate the fact that I have to make m_id non-readonly.  This
         * is because when we reconstruct a node from XML, we need to set the
         * ID from the value in the XML.  The static ConstructANode function 
         * uses reflection to call the constructor on the *derived* class and
         * this means for m_id to be readonly, the derived class needs a public
         * constructor that takes the ID as an argument. I definitely don't 
         * want to allow arbitrary code to be able to construct nodes with
         * an ID that they provide.  This is a better alternative, for now. */
        private FieldGuid m_id = null; // note that there is no Setter
        private readonly ReadOnlyDictionary<FieldIdentifier, FieldBase> m_fields = null;
        private readonly ReadOnlyCollection<NodeBase> m_children = null;

        /// <summary>
        /// A Guid generated when the object is created
        /// </summary>
        public FieldGuid ID
        {
            get
            {
                return m_id;
            }
        }

        /// <summary>
        /// Allows the object to define it's own type.  Future versions may
        /// have types that are not known now, so this may be a value that 
        /// doesn't correspond to an existing class derived from NodeBase
        /// in this particular version of the software, but we must handle
        /// that situation gracefully (in case we open a file saved by
        /// a later version of this software).
        /// </summary>
        public FieldNodeType NodeType
        {
            get
            {
                return (FieldNodeType)m_fields[new FieldIdentifier(m_NodeTypeName)];
            }
        }
        static readonly string m_NodeTypeName =
            NotifyPropertyChangedHelper.GetPropertyName<NodeBase>(o => o.NodeType);

        public ReadOnlyCollection<NodeBase> ChildCollection
        {
            get
            {
                return m_children;
            }
        }

        public ReadOnlyDictionary<FieldIdentifier, FieldBase> Fields
        {
            get
            {
                return m_fields;
            }
        }

        protected NodeBase(ReadOnlyDictionary<FieldIdentifier, FieldBase> Fields,
            ReadOnlyCollection<NodeBase> Children)
        {
            if (Fields == null)
            {
                throw new ArgumentNullException("Fields");
            }
            if (Children == null)
            {
                throw new ArgumentNullException("Children");
            }

            //Generate new Guid
            m_id = new FieldGuid();

            //Make sure there's a NodeType field - create one if it doesn't exist.
            ReadOnlyDictionary<FieldIdentifier, FieldBase> setFields = null;
            FieldIdentifier fieldNodeTypeID = new FieldIdentifier(m_NodeTypeName);
            if (!Fields.ContainsKey(fieldNodeTypeID))
            {
                //doesn't have a NodeType field, so we have to add one
                Dictionary<FieldIdentifier, FieldBase> mutableFields = new Dictionary<FieldIdentifier, FieldBase>();
                foreach (KeyValuePair<FieldIdentifier, FieldBase> fldPair in Fields)
                {
                    mutableFields.Add(fldPair.Key, fldPair.Value);
                }
                mutableFields.Add(new FieldIdentifier(m_NodeTypeName), new FieldNodeType(this.GetType().FullName));
                setFields = new ReadOnlyDictionary<FieldIdentifier, FieldBase>(mutableFields);
            }
            else
            {
                setFields = Fields;
            }

            m_fields = setFields;
            m_children = Children;
        }
        #endregion

        #region " COMPARISON "
        public override bool Equals(object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to NodeBase
            // then return false.
            NodeBase o = obj as NodeBase;
            if ((System.Object)o == null)
            {
                return false;
            }

            return this.ID == o.ID;
        }

        public override int GetHashCode()
        {
            return this.ID.GetHashCode();
        }

        public static bool operator ==(NodeBase x, NodeBase y)
        {
            if ((object)x == null && (object)y == null)
            {
                return true;
            }
            else if ((object)x == null || (object)y == null)
            {
                return false;
            }
            else
            {
                return (x.ID == y.ID);
            }
        }

        public static bool operator !=(NodeBase x, NodeBase y)
        {
            return !(x == y);
        }

        #endregion

        #region " FILTER CHILDREN BY TYPE "
        //Lazy initialized filtered set of ReadOnlyCollections that
        //filter the children nodes by type.
        private ReadOnlyDictionary<Type, ReadOnlyCollection<NodeBase>> m_filteredChildren = null;
        internal ReadOnlyCollection<NodeBase> GetChildrenByType(Type t)
        {
            if (null==m_filteredChildren)
            {
                //one pass bucket sort by type
                Dictionary<Type, KeyedNodeCollection<NodeBase>> filteredChildrenMutable =
                    new Dictionary<Type, KeyedNodeCollection<NodeBase>>();
                foreach (NodeBase item in m_children)
                {
                    Type itemType = item.GetType();
                    if (!filteredChildrenMutable.ContainsKey(itemType))
                    {
                        //it's a new type
                        filteredChildrenMutable.Add(itemType, new KeyedNodeCollection<NodeBase>());
                    }

                    filteredChildrenMutable[itemType].Add(item);
                }

                //now make a mutable dictionary of ReadOnlyCollections
                Dictionary<Type, ReadOnlyCollection<NodeBase>> filteredChildrenIntermediate =
                    new Dictionary<Type, ReadOnlyCollection<NodeBase>>();
                foreach (Type itemType in filteredChildrenMutable.Keys)
                {
                    filteredChildrenIntermediate.Add(itemType,
                        new ReadOnlyCollection<NodeBase>(filteredChildrenMutable[itemType]));
                }

                //encapsulate it in a temporary ReadOnlyDictionary, and assign it to the cache
                System.Threading.Interlocked.CompareExchange(
                    ref m_filteredChildren, 
                    new ReadOnlyDictionary<Type,
                        ReadOnlyCollection<NodeBase>>(filteredChildrenIntermediate), 
                    null);
            }

            if (m_filteredChildren.ContainsKey(t))
            {
                return m_filteredChildren[t];
            }
            else
            {
                //return an empty collection
                ReadOnlyCollection<NodeBase> temp =
                    new ReadOnlyCollection<NodeBase>(
                        new KeyedNodeCollection<NodeBase>()
                        );
                return temp;
            }
        }
        #endregion

        #region " RECURSIVE DICTIONARY OF CHILDREN "
        //Lazy initialized set of children including children's children, etc.
        private ReadOnlyDictionary<FieldGuid, NodeBase> m_childrenRecursive = null;
        public ReadOnlyDictionary<FieldGuid, NodeBase> GetChildrenRecursive()
        {
            if (m_childrenRecursive == null)
            {
                KeyedNodeCollection<NodeBase> mutableChildren =
                    new KeyedNodeCollection<NodeBase>();

                foreach (NodeBase child in ChildCollection)
                {
                    if (!mutableChildren.Contains(child))
                    {
                        mutableChildren.Add(child);
                    }
                    foreach (KeyValuePair<FieldGuid, NodeBase> grandchild in child.GetChildrenRecursive())
                    {
                        if (!mutableChildren.Contains(grandchild.Key))
                        {
                            mutableChildren.Add(grandchild.Value);
                        }
                    }
                }

                System.Threading.Interlocked.CompareExchange(ref m_childrenRecursive,
                    new ReadOnlyDictionary<FieldGuid, NodeBase>(mutableChildren.Dictionary), null);
            }
            return m_childrenRecursive;
        }
        #endregion

        #region " MANIPULATE THE COLLECTION OF CHILDREN "

        /// <summary>
        /// This function helps the NodeChildren class be generic.
        /// </summary>
        /// <param name="NewChildren">New child collection to replace the 
        /// existing one.</param>
        /// <returns></returns>
        protected abstract NodeBase CopyWithNewChildren(
            ReadOnlyCollection<NodeBase> NewChildren);

        public ReadOnlyCollection<NodeBase> ReplaceChild(
            NodeBase OldChild, NodeBase NewChild)
        {
            KeyedNodeCollection<NodeBase> newChildrenMutable = 
                new KeyedNodeCollection<NodeBase>();

            if(null != NewChild)
            {
                newChildrenMutable.Add(NewChild);
            }

            ReadOnlyCollection<NodeBase> newChildren =
                new ReadOnlyCollection<NodeBase>(newChildrenMutable);

            return ReplaceChild(OldChild, newChildren);
        }

    public ReadOnlyCollection<NodeBase> ReplaceChild(
            NodeBase OldChild,
            ReadOnlyCollection<NodeBase> NewChildren)
        {
            //Make sure the parameters exist
            //If the parameters are null, then make empty lists.
            if (NewChildren == null)
            {
                KeyedNodeCollection<NodeBase> temp = new KeyedNodeCollection<NodeBase>();
                NewChildren = new ReadOnlyCollection<NodeBase>(temp);
            }

            if (OldChild == null && NewChildren.Count == 0)
            {
                return ChildCollection; //no change
            }
            else
            {
                //Make a new collection of children from this collection
                //and add the new children where the given "Old" one is.  
                //Make sure there are no duplicates of at the same time.
                KeyedNodeCollection<NodeBase> newChildrenMutable = new KeyedNodeCollection<NodeBase>();
                bool found = false;
                foreach (NodeBase item in m_children)
                {
                    if (OldChild != null 
                        && OldChild.ID != item.ID 
                        && NewChildren.Contains(item))
                    {
                        throw new InvalidOperationException();
                    }

                    if (OldChild != null && OldChild.ID == item.ID)
                    {
                        found = true;
                        foreach(NodeBase newItem in NewChildren)
                        {
                            newChildrenMutable.Add(newItem);
                        }
                    }
                    else
                    {
                        newChildrenMutable.Add(item);
                    }
                }
                if (found == false)
                {
                    //add it at the end
                    foreach(NodeBase newItem in NewChildren)
                    {
                        newChildrenMutable.Add(newItem);
                    }
                }
                ReadOnlyCollection<NodeBase> newChildren =
                    new ReadOnlyCollection<NodeBase>(newChildrenMutable);
                return newChildren;

            }
        }

        public ReadOnlyCollection<NodeBase> Remove(
            ReadOnlyCollection<NodeBase> OldChildren)
        {
            //Make sure the parameters exist
            //If the parameters are null, then make empty lists.
            if (OldChildren == null)
            {
                KeyedNodeCollection<NodeBase> temp = 
                    new KeyedNodeCollection<NodeBase>();
                OldChildren = new ReadOnlyCollection<NodeBase>(temp);
            }

            //make a copy with O(1) lookup time
            KeyedNodeCollection<NodeBase> oldChildrenKeyed = 
                new KeyedNodeCollection<NodeBase>(OldChildren);

            if (OldChildren.Count == 0)
            {
                return ChildCollection; //no change
            }
            else
            {
                //Make a new dictionary of children from this dictionary
                //and remove the old children. 
                KeyedNodeCollection<NodeBase> newChildrenMutable = 
                    new KeyedNodeCollection<NodeBase>();
                foreach (NodeBase item in m_children)
                {
                    if (!oldChildrenKeyed.Contains(item))
                    {
                        newChildrenMutable.Add(item);
                    }
                }
                ReadOnlyCollection<NodeBase> newChildren =
                    new ReadOnlyCollection<NodeBase>(newChildrenMutable);
                return newChildren;

            }
        }
        #endregion

        #region " MANIPULATE THE FIELDS "
        /// <summary>
        /// "Sets" a field by returning a new ReadOnlyDictionary of fields
        /// with the existing field replaced with the given one.
        /// </summary>
        /// <param name="Name">Field Name</param>
        /// <param name="Field">Field Instance</param>
        /// <returns>ReadOnlyDictionary of String and FieldBase</returns>
        public ReadOnlyDictionary<FieldIdentifier, FieldBase> SetField(
            FieldIdentifier Name, FieldBase Field)
        {
            return ReplaceField(m_fields, Name, Field);
        }

        private static ReadOnlyDictionary<FieldIdentifier, FieldBase> ReplaceField(
            ReadOnlyDictionary<FieldIdentifier, FieldBase> OldFields,
            FieldIdentifier Name, FieldBase Field)
        {
            Dictionary<FieldIdentifier, FieldBase> mutableFields =
                new Dictionary<FieldIdentifier, FieldBase>();
            foreach (FieldIdentifier key in OldFields.Keys)
            {
                if (Name != null && key != Name)
                {
                    mutableFields.Add(key, OldFields[key]);
                }
            }
            if (null != Field)
            {
                mutableFields.Add(Name, Field);
            }

            ReadOnlyDictionary<FieldIdentifier, FieldBase> newFields =
                new ReadOnlyDictionary<FieldIdentifier, FieldBase>(mutableFields);

            return newFields;
        }

        protected static ReadOnlyDictionary<FieldIdentifier, FieldBase> SetFieldDefaults(
            ReadOnlyDictionary<FieldIdentifier, FieldBase> Fields, 
            ReadOnlyDictionary<FieldIdentifier, FieldBase> DefaultFields)
        {
            ReadOnlyDictionary<FieldIdentifier, FieldBase> returnFields = Fields;
            foreach (FieldIdentifier id in DefaultFields.Keys)
            {
                if (!returnFields.ContainsKey(id))
                {
                    returnFields = returnFields.Add(id, DefaultFields[id]);
                }
            }
            return returnFields;
        }

        #endregion

        #region " XML "

        public string ToXml()
        {
            return ToXml(null, null, null);
        }

        internal string ToXml(NodeBase BasedOn)
        {
            return ToXml(BasedOn, null, null);
        }

        internal string ToXml(NodePeer ToPeer, NodePeer FromPeer)
        {
            return ToXml(null, ToPeer, FromPeer);
        }

        internal string ToXml(NodeBase BasedOn, NodePeer ToPeer, NodePeer FromPeer)
        {
            //Do a depth first search of this node.
            //Only include nodes that are not in
            //the BasedOn node tree.
            //Always include this node last, even
            //if this node == BasedOn.

            //Do a depth first search and push each node
            //onto a stack as soon as we see it (if we
            //haven't visited it already).
            Stack<NodeBase> nodeStack = new Stack<NodeBase>();
            ReadOnlyDictionary<FieldGuid, NodeBase> excludeNodes = null;
            if (BasedOn == null)
            {
                excludeNodes = new ReadOnlyDictionary<FieldGuid, NodeBase>(
                    new Dictionary<FieldGuid, NodeBase>());
            }
            else
            {
                excludeNodes = BasedOn.GetChildrenRecursive();
            }
            KeyedNodeCollection<NodeBase> nodeLookup = 
                new KeyedNodeCollection<NodeBase>();

            nodeStack.Push(this); //always include the top node
            nodeLookup.Add(this);
            DepthFirstSearch(this, nodeStack, excludeNodes, nodeLookup);

            XElement xml = new XElement("Message",
                   from n in nodeStack
                   select XElement.Parse(n.ToXmlSingleNode())
                       );
            if (BasedOn != null)
            {
                xml.Add(new XAttribute("BasedOnNodeID", BasedOn.ID));
            }
            if (ToPeer != null)
            {
                xml.Add(new XAttribute("ToPeer", ToPeer.ID));
            }
            if (FromPeer != null)
            {
                xml.Add(new XAttribute("FromPeer", FromPeer.ID));
            }
            String tempXml = xml.ToString();
            return tempXml;
        }

        private void DepthFirstSearch(
            NodeBase node, 
            Stack<NodeBase> nodeStack,
            ReadOnlyDictionary<FieldGuid, NodeBase> excludeNodes,
            KeyedNodeCollection<NodeBase> nodeLookup)
        {
            if (!nodeLookup.Contains(node))
            {
                if (!excludeNodes.ContainsKey(node.ID))
                {
                    nodeStack.Push(node);
                }
                nodeLookup.Add(node);
            }

            //recursion
            foreach (NodeBase item in node.ChildCollection)
            {
                DepthFirstSearch(item, nodeStack, excludeNodes, nodeLookup);
            }
        }

        //Lazy Initialized
        private String m_xml = null;
        internal String ToXmlSingleNode()
        {
            if (m_xml == null)
            {
                XElement xml = new XElement("Node",
                    new XElement("ID", this.ID.ToString()),
                    new XElement("Fields",
                        from f in this.m_fields
                            select new XElement("Field",
                                new XAttribute("Name", f.Key.ToString()),
                                new XAttribute("Type", f.Value.GetType().FullName),
                                new XCData(f.Value.ToString())
                                )),
                    new XElement("Children",
                        from n in this.m_children
                            select new XElement("ID",n.ID)
                            ));
                String tempXml = xml.ToString();
                System.Threading.Interlocked.CompareExchange(ref m_xml, tempXml, null);
            }
            return m_xml;
        }

        private static XmlReaderSettings getReaderSettings()
        {
            lock (m_settings_Lock)
            {
                if (m_settings == null)
                {
                    TextReader schemaStream = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("SoapBox.Protocol.Base.SoapBoxProtocolV1.xsd"));
                    XmlSchemaSet schemaSet = new XmlSchemaSet();
                    schemaSet.Add("", XmlReader.Create(schemaStream));

                    XmlReaderSettings settings = new XmlReaderSettings();
                    settings.Schemas.Add(schemaSet);
                    settings.ValidationType = ValidationType.Schema;
                    m_settings = settings;
                }
            }
            return m_settings;
        }
        private static XmlReaderSettings m_settings = null;
        private static object m_settings_Lock = new object();

        internal static bool ValidateXmlToSchema(String Xml)
        {
            bool isValid = true;
            try
            {
                XmlReader rdr = XmlReader.Create(new StringReader(Xml), getReaderSettings());
                while (rdr.Read()) { }
            }
            catch
            {
                isValid = false;
            }
            return isValid;
        }

        internal static FieldGuid ToPeerFromXML(string Xml)
        {
            if (Xml == null)
            {
                throw new ArgumentNullException("Xml");
            }

            if (!ValidateXmlToSchema(Xml))
            {
                throw new InvalidDataException();
            }
            else
            {
                XElement element = XElement.Parse(Xml);
                if (element.Name == "Message")
                {
                    XElement elemMessage = element;

                    FieldGuid field = null; //return value
                    XAttribute attrToPeer = elemMessage.Attribute("ToPeer");
                    if (attrToPeer != null)
                    {
                        field = new FieldGuid(attrToPeer.Value);
                    }
                    return field;
                }
                else
                {
                    throw new InvalidDataException();
                }
            }
        }

        internal static FieldGuid FromPeerFromXML(string Xml)
        {
            if (Xml == null)
            {
                throw new ArgumentNullException("Xml");
            }

            if (!ValidateXmlToSchema(Xml))
            {
                throw new InvalidDataException();
            }
            else
            {
                XElement element = XElement.Parse(Xml);
                if (element.Name == "Message")
                {
                    XElement elemMessage = element;

                    FieldGuid field = null; //return value
                    XAttribute attrFromPeer = elemMessage.Attribute("FromPeer");
                    if (attrFromPeer != null)
                    {
                        field = new FieldGuid(attrFromPeer.Value);
                    }
                    return field;
                }
                else
                {
                    throw new InvalidDataException();
                }
            }
        }

        internal static FieldGuid BasedOnNodeIDFromXML(string Xml)
        {
            if (Xml == null)
            {
                throw new ArgumentNullException("Xml");
            }

            if (!ValidateXmlToSchema(Xml))
            {
                throw new InvalidDataException();
            }
            else
            {
                XElement element = XElement.Parse(Xml);
                if (element.Name == "Message")
                {
                    XElement elemMessage = element;

                    FieldGuid field = null; //return value
                    XAttribute attrBasedOnNodeID = elemMessage.Attribute("BasedOnNodeID");
                    if (attrBasedOnNodeID != null)
                    {
                        field = new FieldGuid(attrBasedOnNodeID.Value);
                    }
                    return field;
                }
                else
                {
                    throw new InvalidDataException();
                }
            }
        }

        internal static FieldNodeType NodeTypeFromXML(string Xml)
        {
            if (Xml == null)
            {
                throw new ArgumentNullException("Xml");
            }

            if (!ValidateXmlToSchema(Xml))
            {
                throw new InvalidDataException();
            }
            else
            {
                XElement element = XElement.Parse(Xml);
                if (element.Name == "Message")
                {
                    XElement elemMessage = element;
                    XElement LastNode = elemMessage.Elements("Node").Last();
                    XElement elemFields = LastNode.Element("Fields");

                    FieldNodeType field = null; //return value
                    foreach (XElement elemField in elemFields.Elements("Field"))
                    {
                        //these 2 are guaranteed to be there by the schema
                        XAttribute attrType = elemField.Attribute("Type");
                        if (attrType.Value == typeof(FieldNodeType).FullName)
                        {
                            XAttribute attrName = elemField.Attribute("Name");
                            field = (FieldNodeType)ConstructAField(attrType.Value, elemField.Value);
                            break;
                        }
                    }
                    return field;
                }
                else
                {
                    throw new InvalidDataException();
                }
            }
       }

        /// <summary>
        /// Given an XML string, will return a Node of the correct type
        /// based on the XML.  Throws InvalidDataException.
        /// </summary>
        /// <param name="Xml">XML representing the Node element</param>
        /// <param name="NodeLibrary">Dictionary of nodes where child nodes
        /// of this node can be found.</param>
        /// <returns></returns>
        public static NodeBase NodeFromXML(String Xml, 
            ReadOnlyDictionary<FieldGuid, NodeBase> NodeLibrary)
        {
            if (Xml == null)
            {
                throw new ArgumentNullException("Xml");
            }
            if (NodeLibrary == null)
            {
                NodeLibrary = new ReadOnlyDictionary<FieldGuid, NodeBase>(
                    new Dictionary<FieldGuid, NodeBase>());
            }

            if (!ValidateXmlToSchema(Xml))
            {
                throw new InvalidDataException();
            }
            else
            {
                XElement element = XElement.Parse(Xml);
                if (element.Name == "Node")
                {
                    XElement elemNode = element;

                    //these 3 are guaranteed to be there by the schema
                    XElement elemID = elemNode.Element("ID");
                    XElement elemFields = elemNode.Element("Fields");
                    XElement elemChildren = elemNode.Element("Children");

                    //parse the ID
                    FieldGuid ID = null;
                    if (FieldGuid.CheckSyntax(elemID.Value))
                    {
                        ID = new FieldGuid(elemID.Value);
                    }
                    else
                    {
                        throw new InvalidDataException();
                    }

                    //parse the field collection
                    Dictionary<FieldIdentifier, FieldBase> mutableFields =
                        new Dictionary<FieldIdentifier, FieldBase>();
                    foreach (XElement elemField in elemFields.Elements("Field"))
                    {
                        //these 2 are guaranteed to be there by the schema
                        XAttribute attrName = elemField.Attribute("Name");
                        XAttribute attrType = elemField.Attribute("Type");

                        FieldBase field = ConstructAField(attrType.Value, elemField.Value);
                        mutableFields.Add(new FieldIdentifier(attrName.Value), field);
                    }
                    ReadOnlyDictionary<FieldIdentifier, FieldBase> NewFields =
                        new ReadOnlyDictionary<FieldIdentifier, FieldBase>(
                            mutableFields
                            );

                    //parse the child collection
                    KeyedNodeCollection<NodeBase> mutableChildren =
                        new KeyedNodeCollection<NodeBase>();
                    foreach (XElement elemChild in elemChildren.Elements("ID"))
                    {
                        //parse the ID
                        FieldGuid ChildID = null;
                        if (FieldGuid.CheckSyntax(elemChild.Value))
                        {
                            ChildID = new FieldGuid(elemChild.Value);
                        }
                        else
                        {
                            throw new InvalidDataException();
                        }

                        if (NodeLibrary.ContainsKey(ChildID))
                        {
                            mutableChildren.Add(NodeLibrary[ChildID]);
                        }
                        else
                        {
                            throw new InvalidDataException(ChildID.ToString());
                        }
                    }
                    ReadOnlyCollection<NodeBase> NewChildren =
                        new ReadOnlyCollection<NodeBase>(mutableChildren);

                    return ConstructANode(ID, NewFields, NewChildren);
                }
                else if (element.Name == "Message")
                {
                    XElement elemMessage = element;
                    KeyedNodeCollection<NodeBase> mutableNodeLibrary =
                        new KeyedNodeCollection<NodeBase>();
                    foreach (NodeBase item in NodeLibrary.Values)
                    {
                        mutableNodeLibrary.Add(item);
                    }
                    NodeBase LastNode = null;
                    foreach (XElement elemNode in elemMessage.Elements("Node"))
                    {
                        //recursion
                        NodeBase newNode = 
                            NodeFromXML(elemNode.ToString(), 
                            new ReadOnlyDictionary<FieldGuid, NodeBase>(
                                mutableNodeLibrary.Dictionary));

                        //It's possible that the node already existed on this
                        //end, so don't return a duplicate node, just return
                        //the one that already existed.
                        if (mutableNodeLibrary.Contains(newNode))
                        {
                            LastNode = mutableNodeLibrary[newNode.ID];
                        }
                        else
                        {
                            mutableNodeLibrary.Add(newNode);
                            LastNode = newNode;
                        }
                    }
                    return LastNode;
                }
                else
                {
                    throw new InvalidDataException();
                }
            }
        }

        /// <summary>
        /// Given a string, checks if it represents a 
        /// known node type
        /// </summary>
        /// <param name="DataType">String representation of a data type, 
        /// like "NodeTag"</param>
        /// <returns>True if valid, false otherwise</returns>
        internal static bool KnownType(String NodeType)
        {
            AppDomain MyDomain = AppDomain.CurrentDomain;
            Assembly[] AssembliesLoaded = MyDomain.GetAssemblies();
            bool found = false;
            foreach (Assembly asm in AssembliesLoaded)
            {
                Type t = asm.GetType(NodeType, false, true); //ignores case
                if (t != null 
                    && typeof(NodeBase).IsAssignableFrom(t) 
                    && t != typeof(NodeBase))
                {
                    found = true;
                }
            }
            return found;
        }

        /// <summary>
        /// Uses reflection to instantiate the right type of field based
        /// on the fieldType parameter.  If it can't find it, then it 
        /// returns a field of type FieldUnknown.
        /// </summary>
        /// <param name="fieldType">A type that derives from FieldBase</param>
        /// <param name="fieldValue">The field value</param>
        /// <returns>Something that derives from FieldBase</returns>
        private static FieldBase ConstructAField(
            String fieldType, String fieldValue)
        {
            FieldBase retVal = null;
            AppDomain MyDomain = AppDomain.CurrentDomain;
            Assembly[] AssembliesLoaded = MyDomain.GetAssemblies();
            foreach (Assembly asm in AssembliesLoaded)
            {
                Type t = asm.GetType(fieldType, false, true); //ignores case
                if (t != null)
                {
                    //found it - now find the constructor
                    Type[] types = new Type[1];
                    types[0] = typeof(String);
                    ConstructorInfo ctorInfo = t.GetConstructor(types);
                    if (ctorInfo != null)
                    {
                        //found the constructor - use it
                        object[] argVals =
                            new object[] { fieldValue };
                        try
                        {
                            retVal = (FieldBase)ctorInfo.Invoke(argVals);
                            break;
                        }
                        catch
                        {
                            retVal = null;
                            //keep going
                        }
                    }
                }
            }
            if (retVal != null)
            {
                return retVal;
            }
            else
            {
                return new FieldUnknown(fieldValue, fieldType);
            }
        }

        /// <summary>
        /// Uses reflection to instantiate the right node type based on
        /// the NodeType field in the list of fields (if it exists).
        /// If it can't figure out what type of node this should be, 
        /// then it returns a NodeUnknown.
        /// </summary>
        /// <param name="NewFields">ReadOnlyDictionary of fields</param>
        /// <param name="NewChildren">ReadOnlyDictionary of child nodes</param>
        /// <returns>Something that derives from NodeBase</returns>
        private static NodeBase ConstructANode(FieldGuid NewID, 
            ReadOnlyDictionary<FieldIdentifier, FieldBase> NewFields,
            ReadOnlyCollection<NodeBase> NewChildren)
        {
            // have to make sure this ref doesn't leak out! (until the return)
            NodeBase retVal = null;

            FieldIdentifier newNodeTypeID = new FieldIdentifier(m_NodeTypeName);
            if (NewFields.ContainsKey(newNodeTypeID)
                && KnownType(NewFields[newNodeTypeID].ToString()))
            {
                //it exists somewhere in this app domain, so go find it
                AppDomain MyDomain = AppDomain.CurrentDomain;
                Assembly[] AssembliesLoaded = MyDomain.GetAssemblies();
                foreach (Assembly asm in AssembliesLoaded)
                {
                    Type t = asm.GetType(NewFields[newNodeTypeID].ToString(), 
                        false, true); //ignores case
                    if (t != null)
                    {
                        //found it - now find the resurrection technology
                        Type[] types = new Type[2];
                        types[0] = typeof(
                            ReadOnlyDictionary<FieldIdentifier, FieldBase>
                            );
                        types[1] = typeof(
                            ReadOnlyCollection<NodeBase>
                            );
                        MethodInfo MethodInfo = t.GetMethod("Resurrect", BindingFlags.NonPublic | BindingFlags.Static, null, types, null);
                        if (MethodInfo != null)
                        {
                            //found the resurrect Method - use it
                            object[] argVals = 
                                new object[] { NewFields, NewChildren };
                            try
                            {
                                retVal = (NodeBase)MethodInfo.Invoke(null, argVals);
                                break;
                            }
                            catch
                            {
                                retVal = null;
                                //keep going
                            }
                        }
                    }
                }
                if (retVal == null)
                {
                    retVal = new NodeUnknown(NewFields, NewChildren);
                }
            }
            else
            {
                retVal = new NodeUnknown(NewFields, NewChildren);
            }

            retVal.m_id = NewID; //ugly, ugly, ugly

            return retVal;
        }

        #endregion

        #region " TypedChildCollection Class "
        /// <summary>
        /// Implements a strongly typed collection of NodeBase derived
        /// objects, to represent a child collection of another NodeBase
        /// derived object.
        /// </summary>
        /// <typeparam name="TChild"></typeparam>
        /// <typeparam name="TParent"></typeparam>
        public sealed class TypedChildCollection<TChild, TParent> : IEnumerable<TChild>
            where TChild : NodeBase
            where TParent : NodeBase
        {

            #region " INSTANTIATION AND MEMBERS "
            //readonly members
            private readonly TParent m_parent;

            public TypedChildCollection(TParent Parent)
            {
                m_parent = Parent;
            }
            #endregion

            #region " INNER COLLECTION "
            //Lazy initialization
            //Represents the collection of child objects of the parent that
            //match the TChild type.
            private ReadOnlyCollection<TChild> m_Items = null;
            public ReadOnlyCollection<TChild> Items
            {
                get
                {
                    if (m_Items == null)
                    {
                        ReadOnlyCollection<NodeBase> ChildTags =
                            m_parent.GetChildrenByType(typeof(TChild));
                        KeyedNodeCollection<TChild> MutableChildTags =
                            new KeyedNodeCollection<TChild>();
                        foreach (NodeBase item in ChildTags)
                        {
                            MutableChildTags.Add((TChild)item);
                        }
                        System.Threading.Interlocked.CompareExchange(ref m_Items,
                            new ReadOnlyCollection<TChild>(MutableChildTags), null);
                    }
                    return m_Items;
                }
            }
            #endregion

            #region " APPEND, REMOVE, REPLACE, INSERTBEFORE, INSERTAFTER "
            /// <summary>
            /// Adds a new child object to the end of the collection.
            /// </summary>
            /// <param name="NewItem"></param>
            /// <returns></returns>
            public TParent Append(TChild NewItem)
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
                        m_parent.ReplaceChild(OldItem, (TChild)null)
                        );
                }
            }

            /// <summary>
            /// Removes one child object from the collection, and
            /// adds a new one at the same place.
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
            /// Appends a list of child objects to the end of the collection.
            /// </summary>
            /// <param name="NewItems"></param>
            /// <returns></returns>
            public TParent Append(ReadOnlyCollection<TChild> NewItems)
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
                    ReadOnlyCollection<NodeBase> tempNewItems =
                        CastToBase(NewItems);
                    return (TParent)m_parent.CopyWithNewChildren(
                        m_parent.ReplaceChild(null, tempNewItems)
                        );
                }
            }

            /// <summary>
            /// Removes a list of child objects from the collection.
            /// </summary>
            /// <param name="OldItems"></param>
            /// <returns></returns>
            public TParent Remove(ReadOnlyCollection<TChild> OldItems)
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
                    ReadOnlyCollection<NodeBase> tempOldItems =
                        CastToBase(OldItems);
                    return (TParent)m_parent.CopyWithNewChildren(
                        m_parent.Remove(tempOldItems)
                        );
                }
            }

            /// <summary>
            /// Removes a child object from the collection, and 
            /// replaces it with a list of new child objects.
            /// </summary>
            /// <param name="OldItem"></param>
            /// <param name="NewItems"></param>
            /// <returns></returns>
            public TParent Replace(
                TChild OldItem,
                ReadOnlyCollection<TChild> NewItems)
            {
                if (OldItem == null && NewItems == null)
                {
                    return m_parent;
                }
                else if (OldItem == null && NewItems.Count == 0)
                {
                    return m_parent;
                }
                else
                {
                    ReadOnlyCollection<NodeBase> tempNewItems =
                        CastToBase(NewItems);
                    return (TParent)m_parent.CopyWithNewChildren(
                        m_parent.ReplaceChild(OldItem, tempNewItems)
                        );
                }
            }

            /// <summary>
            /// Inserts a new child object in the collection
            /// before the given child object.
            /// </summary>
            /// <param name="BeforeItem"></param>
            /// <param name="NewItem"></param>
            /// <returns></returns>
            public TParent InsertBefore(TChild BeforeItem, TChild NewItem)
            {
                if (NewItem == null)
                {
                    return m_parent;
                }
                else
                {
                    KeyedNodeCollection<TChild> mutableNewItems = 
                        new KeyedNodeCollection<TChild>();
                    mutableNewItems.Add(NewItem);
                    ReadOnlyCollection<TChild> tempNewItems =
                        new ReadOnlyCollection<TChild>(mutableNewItems);
                    return InsertBefore(BeforeItem, tempNewItems);
                }
            }

            /// <summary>
            /// Inserts a list of child objects int the collection 
            /// before the given item.
            /// </summary>
            /// <param name="BeforeItem"></param>
            /// <param name="NewItems"></param>
            /// <returns></returns>
            public TParent InsertBefore(
                TChild BeforeItem,
                ReadOnlyCollection<TChild> NewItems)
            {
                if (BeforeItem == null)
                {
                    throw new ArgumentNullException("BeforeItem");
                }
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
                    ReadOnlyCollection<NodeBase> tempNewItems =
                        CastToBase(NewItems, BeforeItem);
                    return (TParent)m_parent.CopyWithNewChildren(
                        m_parent.ReplaceChild(BeforeItem, tempNewItems)
                        );
                }
            }

            /// <summary>
            /// Inserts a new child object in the collection
            /// after the given child object.
            /// </summary>
            /// <param name="AfterItem"></param>
            /// <param name="NewItem"></param>
            /// <returns></returns>
            public TParent InsertAfter(TChild AfterItem, TChild NewItem)
            {
                if (NewItem == null)
                {
                    return m_parent;
                }
                else
                {
                    KeyedNodeCollection<TChild> mutableNewItems =
                        new KeyedNodeCollection<TChild>();
                    mutableNewItems.Add(NewItem);
                    ReadOnlyCollection<TChild> tempNewItems =
                        new ReadOnlyCollection<TChild>(mutableNewItems);
                    return InsertAfter(AfterItem, tempNewItems);
                }
            }

            /// <summary>
            /// Inserts a list of child objects int the collection 
            /// after the given item.
            /// </summary>
            /// <param name="AfterItem"></param>
            /// <param name="NewItems"></param>
            /// <returns></returns>
            public TParent InsertAfter(
                TChild AfterItem,
                ReadOnlyCollection<TChild> NewItems)
            {
                if (AfterItem == null)
                {
                    throw new ArgumentNullException("AfterItem");
                }
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
                    ReadOnlyCollection<NodeBase> tempNewItems =
                        CastToBase(AfterItem, NewItems);
                    return (TParent)m_parent.CopyWithNewChildren(
                        m_parent.ReplaceChild(AfterItem, tempNewItems)
                        );
                }
            }

            /// <summary>
            /// Moves a new child object in the collection
            /// before the given child object.
            /// </summary>
            /// <param name="BeforeItem"></param>
            /// <param name="MoveItem"></param>
            /// <returns></returns>
            public TParent MoveBefore(TChild BeforeItem, TChild MoveItem)
            {
                if (MoveItem == null)
                {
                    return m_parent;
                }
                else
                {
                    KeyedNodeCollection<TChild> mutableMoveItems =
                        new KeyedNodeCollection<TChild>();
                    mutableMoveItems.Add(MoveItem);
                    ReadOnlyCollection<TChild> tempMoveItems =
                        new ReadOnlyCollection<TChild>(mutableMoveItems);
                    return MoveBefore(BeforeItem, tempMoveItems);
                }
            }

            /// <summary>
            /// Moves a list of child objects in the collection 
            /// before the given item.
            /// </summary>
            /// <param name="BeforeItem"></param>
            /// <param name="MoveItems"></param>
            /// <returns></returns>
            public TParent MoveBefore(
                TChild BeforeItem,
                ReadOnlyCollection<TChild> MoveItems)
            {
                if (BeforeItem == null)
                {
                    throw new ArgumentNullException("BeforeItem");
                }
                if (MoveItems == null)
                {
                    return m_parent;
                }
                else if (MoveItems.Count == 0)
                {
                    return m_parent;
                }
                else
                {
                    if (MoveItems.Contains(BeforeItem))
                    {
                        throw new InvalidOperationException();
                    }
                    TParent tempParent = Remove(MoveItems);
                    ReadOnlyCollection<NodeBase> tempNewItems =
                        CastToBase(MoveItems, BeforeItem);
                    return (TParent)tempParent.CopyWithNewChildren(
                        tempParent.ReplaceChild(BeforeItem, tempNewItems)
                        );
                }
            }

            /// <summary>
            /// Moves a new child object in the collection
            /// After the given child object.
            /// </summary>
            /// <param name="AfterItem"></param>
            /// <param name="MoveItem"></param>
            /// <returns></returns>
            public TParent MoveAfter(TChild AfterItem, TChild MoveItem)
            {
                if (MoveItem == null)
                {
                    return m_parent;
                }
                else
                {
                    KeyedNodeCollection<TChild> mutableMoveItems =
                        new KeyedNodeCollection<TChild>();
                    mutableMoveItems.Add(MoveItem);
                    ReadOnlyCollection<TChild> tempMoveItems =
                        new ReadOnlyCollection<TChild>(mutableMoveItems);
                    return MoveAfter(AfterItem, tempMoveItems);
                }
            }

            /// <summary>
            /// Moves a list of child objects in the collection 
            /// After the given item.
            /// </summary>
            /// <param name="AfterItem"></param>
            /// <param name="MoveItems"></param>
            /// <returns></returns>
            public TParent MoveAfter(
                TChild AfterItem,
                ReadOnlyCollection<TChild> MoveItems)
            {
                if (AfterItem == null)
                {
                    throw new ArgumentNullException("AfterItem");
                }
                if (MoveItems == null)
                {
                    return m_parent;
                }
                else if (MoveItems.Count == 0)
                {
                    return m_parent;
                }
                else
                {
                    if (MoveItems.Contains(AfterItem))
                    {
                        throw new InvalidOperationException();
                    }
                    TParent tempParent = Remove(MoveItems);
                    ReadOnlyCollection<NodeBase> tempNewItems =
                        CastToBase(AfterItem, MoveItems);
                    return (TParent)tempParent.CopyWithNewChildren(
                        tempParent.ReplaceChild(AfterItem, tempNewItems)
                        );
                }
            }

            /// <summary>
            /// Creates a ReadOnlyCollection of NodeBase from any 
            /// ReadOnlyCollection of objects derived from NodeBase
            /// </summary>
            /// <param name="Items">Collection to copy</param>
            /// <returns>New Collection of Base Objects</returns>
            private ReadOnlyCollection<NodeBase> CastToBase(
                ReadOnlyCollection<TChild> Items)
            {
                KeyedNodeCollection<NodeBase> mutableItems =
                    new KeyedNodeCollection<NodeBase>();
                foreach (TChild item in Items)
                {
                    mutableItems.Add((NodeBase)item);
                }
                ReadOnlyCollection<NodeBase> tempItems =
                    new ReadOnlyCollection<NodeBase>(mutableItems);
                return tempItems;
            }

            /// <summary>
            /// Creates a ReadOnlyCollection of NodeBase from any 
            /// ReadOnlyCollection of objects derived from NodeBase.
            /// Allows the insertion of a node before
            /// </summary>
            /// <param name="Items">Collection to copy</param>
            /// <param name="BeforeItem">Item to insert before</param>
            /// <returns>New Collection of Base Objects</returns>
            private ReadOnlyCollection<NodeBase> CastToBase(
                ReadOnlyCollection<TChild> Items, 
                TChild BeforeItem)
            {
                KeyedNodeCollection<NodeBase> mutableItems =
                    new KeyedNodeCollection<NodeBase>();
                foreach (TChild item in Items)
                {
                    mutableItems.Add((NodeBase)item);
                }
                if (BeforeItem != null)
                {
                    mutableItems.Add((NodeBase)BeforeItem);
                }
                ReadOnlyCollection<NodeBase> tempItems =
                    new ReadOnlyCollection<NodeBase>(mutableItems);
                return tempItems;
            }

            /// <summary>
            /// Creates a ReadOnlyCollection of NodeBase from any 
            /// ReadOnlyCollection of objects derived from NodeBase.
            /// Allows the insertion of a node after
            /// </summary>
            /// <param name="AfterItem">Item to insert after</param>
            /// <param name="Items">Collection to copy</param>
            /// <returns>New Collection of Base Objects</returns>
            private ReadOnlyCollection<NodeBase> CastToBase(
                TChild AfterItem,
                ReadOnlyCollection<TChild> Items)
            {
                KeyedNodeCollection<NodeBase> mutableItems =
                    new KeyedNodeCollection<NodeBase>();
                if (AfterItem != null)
                {
                    mutableItems.Add((NodeBase)AfterItem);
                }
                foreach (TChild item in Items)
                {
                    mutableItems.Add((NodeBase)item);
                }
                ReadOnlyCollection<NodeBase> tempItems =
                    new ReadOnlyCollection<NodeBase>(mutableItems);
                return tempItems;
            }
            #endregion

            #region " CONTAINS "
            /// <summary>
            /// Tests if the given child item exists in the child collection.
            /// </summary>
            /// <param name="item">Child item to search for</param>
            /// <returns>True if found, false otherwise</returns>
            public bool Contains(TChild item)
            {
                if (Items.Contains(item)) // O(n) operation
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            #endregion

            #region " IMMITATE A COLLECTION "
            public IEnumerator<TChild> GetEnumerator()
            {
                foreach (TChild item in Items)
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
                    return Items[index];
                }
            }
            #endregion

        }
        #endregion

        #region " SingleChild Class "
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
            private ReadOnlyCollection<TChild> m_Items = null;
            public TChild Item
            {
                get
                {
                    if (m_Items == null)
                    {
                        ReadOnlyCollection<NodeBase> ChildTags =
                            m_parent.GetChildrenByType(typeof(TChild));
                        KeyedNodeCollection<TChild> MutableChildTags =
                            new KeyedNodeCollection<TChild>();
                        foreach (NodeBase item in ChildTags)
                        {
                            MutableChildTags.Add((TChild)item);
                        }
                        System.Threading.Interlocked.CompareExchange(ref m_Items,
                            new ReadOnlyCollection<TChild>(MutableChildTags), null);
                    }
                    if (m_Items.Count == 1)
                    {
                        return (TChild)m_Items[0];
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
        #endregion

    }
}
