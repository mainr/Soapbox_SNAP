#region "SoapBox.Snap License"
/// <header module="SoapBox.Snap"> 
/// Copyright (C) 2009-2014 SoapBox Automation, All Rights Reserved.
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
using SoapBox.Utilities;
using System.Windows;
using SoapBox.Protocol.Base;
using SoapBox.Protocol.Automation;
using System.ComponentModel.Composition;

namespace SoapBox.Snap
{
    [Export(SoapBox.Core.ExtensionPoints.Host.Void, typeof(object))]
    public class AbstractEditorItem : AbstractNodeWrapper, IEditorItem
    {
        public const string PAGE_EDITOR_ROOT_NAME = "PageEditorRoot"; // element name of the page editor root FrameworkElement
                                                                      // IF YOU CHANGE THIS, CHANGE THE PageEditorView.xaml TOO!

        // For MEF to call
        public AbstractEditorItem()
            : base(null)
        {
        }

        public AbstractEditorItem(IEditorItem parent)
            : base(parent)
        {
            PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(AbstractEditorItem_PropertyChanged);
            parentChanged();
        }

        #region " runtimeServiceSingleton "
        [Import(Services.Solution.RuntimeService, typeof(IRuntimeService))]
        protected IRuntimeService runtimeService
        {
            get
            {
                return m_runtimeService;
            }
            set
            {
                m_runtimeService = value;
            }
        }
        private static IRuntimeService m_runtimeService = null;
        public static string m_runtimeService_Name = 
            NotifyPropertyChangedHelper.GetPropertyName<AbstractEditorItem>(o => o.runtimeService);
        #endregion

        private void parentChanged()
        {
            if (oldParent != Parent)
            {
                if (oldParent != null)
                {
                    this.UndoableAction -= new UndoableActionHandler(oldParent.FireUndoableActionEvent);
                }
                oldParent = Parent as IEditorItem;
                if (Parent != null)
                {
                    this.UndoableAction += new UndoableActionHandler(oldParent.FireUndoableActionEvent);
                }
            }
        }

        private IEditorItem oldParent = null;

        // Makes all undoable action events go up the tree
        void AbstractEditorItem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == m_ParentName)
            {
                parentChanged();
            }
        }
        static readonly string m_ParentName =
            NotifyPropertyChangedHelper.GetPropertyName<AbstractEditorItem>(o => o.Parent);

        #region " Implement IUndoRedo "
        public event UndoableActionHandler UndoableAction;
        public void FireUndoableActionEvent(IUndoMemento memento)
        {
            var evt = UndoableAction;
            if (evt != null)
            {
                logger.Info("Firing UndoableActionEvent in item " + this.GetType().ToString());
                // When a re-do action happens, we're doing the edit again *at the source of the edit*
                // so all the parent, grandparent, etc. nodes have new ID's generated, when really we
                // want them to revert to their old IDs.  This just does a quick set of them back
                // to the original node values that existed before the edit was made.
                var origNode = Node;
                memento.UndoActions.Add(() => StealthSetNode(origNode));
                evt(memento);
            }
        }

        // Called when we want to execute an Undo at this node
        public virtual void Undo(IUndoMemento memento)
        {
            disableEditedDeletedEvents = true;
            NotifySignals(() => DoUndoActions(this, memento));
            disableEditedDeletedEvents = false;
        }

        // Called when we want to execute a Redo at this node
        public virtual void Redo(IUndoMemento memento)
        {
            disableEditedDeletedEvents = true;
            NotifySignals(() => DoRedoActions(this, memento));
            disableEditedDeletedEvents = false;
        }

        /// <summary>
        /// Takes a snapshot of the child signals before making a change, then makes the change, then takes another
        /// snapshot, does a comparison, and fires a signal changed event for any signals that are different.
        /// </summary>
        /// <param name="act">Action that does changes to the tree</param>
        public void NotifySignals(Action act)
        {
            var oldChildren = new ReadOnlyDictionary<FieldGuid, NodeBase>(new Dictionary<FieldGuid, NodeBase>());
            if (Node != null)
            {
                oldChildren = Node.GetChildrenRecursive();
            }

            act();

            var newChildren = new ReadOnlyDictionary<FieldGuid, NodeBase>(new Dictionary<FieldGuid, NodeBase>());
            if (Node != null)
            {
                newChildren = Node.GetChildrenRecursive();
            }

            foreach (var newChildKey in newChildren.Keys)
            {
                if (!oldChildren.ContainsKey(newChildKey))
                {
                    // it's an edited node
                    var sig = newChildren[newChildKey] as NodeSignal;
                    runtimeService.NotifySignalChanged(sig);
                }
            }
        }

        /// <summary>
        /// This is really a helper function so the derived Editor Item 
        /// can just build up the memento and call Do() with it.  That will
        /// execute the Redo Actions and also fire off the event.
        /// </summary>
        protected void Do(IUndoMemento memento)
        {
            NotifySignals(() => Do(memento, null));
        }

        protected void Do(IUndoMemento memento, IEditorItem extraUndoNode)
        {
            FireUndoableActionEvent(memento);
            DoRedoActions(this, memento);

            // Have to save the state after the edit is complete
            // so that subsequent ReDo commands can always go back to 
            // this exact same state
            if (extraUndoNode != null)
            {
                saveNodeState(extraUndoNode, memento);
                var theirParent = extraUndoNode.Parent;
                while (theirParent != null)
                {
                    saveNodeState(theirParent, memento);
                    theirParent = theirParent.Parent;
                }
            }

            saveNodeState(this, memento);
            var myParent = Parent;
            while (myParent != null)
            {
                saveNodeState(myParent, memento);
                myParent = myParent.Parent;
            }
        }

        private void saveNodeState(INodeWrapper nodeWrapper, IUndoMemento memento)
        {
            if (nodeWrapper != null && memento != null)
            {
                var saveNodeWrapper = nodeWrapper;
                var saveNode = saveNodeWrapper.Node;
                memento.RedoActions.Add(() => saveNodeWrapper.StealthSetNode(saveNode));
            }
        }

        public static void DoUndoActions(IUndoRedo context, IUndoMemento memento)
        {
            if (memento.Sender == context)
            {
                foreach (var undoAction in memento.UndoActions)
                {
                    undoAction();
                }
            }
        }

        public static void DoRedoActions(IUndoRedo context, IUndoMemento memento)
        {
            if (memento.Sender == context)
            {
                foreach (var redoAction in memento.RedoActions)
                {
                    redoAction();
                }
            }
        }
        #endregion

    }
}
