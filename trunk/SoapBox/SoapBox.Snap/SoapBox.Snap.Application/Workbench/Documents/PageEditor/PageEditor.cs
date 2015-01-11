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
using System.Linq;
using System.Text;
using SoapBox.Core;
using System.ComponentModel.Composition;
using SoapBox.Protocol.Base;
using System.ComponentModel;
using SoapBox.Utilities;
using SoapBox.Protocol.Automation;
using System.Collections.ObjectModel;
using System.Windows;

namespace SoapBox.Snap.Application
{
    [Export(SoapBox.Core.ExtensionPoints.Workbench.Documents, typeof(IDocument))]
    [Export(CompositionPoints.Workbench.Documents.PageEditor, typeof(PageEditor))]
    [Document(Name = Extensions.Workbench.Documents.PageEditor)]
    public class PageEditor : AbstractDocument
    {
        private const int UNDO_UPPER_LIMIT = 100; // when do we trigger a dump
        private const int UNDO_RESIZE_SIZE = 90; // how many items we go down to

        #region " CONSTRUCTORS "
        /// <summary>
        /// Called by MEF (to satisfy the imports)
        /// </summary>
        private PageEditor()
        {
            Name = Extensions.Workbench.Documents.PageEditor;
            Title = Resources.Strings.Workbench_Documents_Editor_Title;
        }

        public PageEditor(PageItem pageItem)
        {
            if (pageItem == null)
            {
                throw new ArgumentNullException();
            }
            Name = Extensions.Workbench.Documents.PageEditor;

            m_pageItem = pageItem;
            m_EditorRoot = new PageEditorItem(this,pageItem.Page);
            m_EditorRoot.UndoableAction += new UndoableActionHandler(EditorRoot_UndoableAction);
            m_EditorRoot.Edited += new EditedHandler(EditorRoot_Edited);
            m_EditorRoot.ContextMenu = extensionService.Sort(contextMenu);

            m_pageItem.PropertyChanged += new PropertyChangedEventHandler(m_pageItem_PropertyChanged);
            OnNodeChanged();

            solutionPad.Saving += new EventHandler(solutionPad_Saving);

        }

        void solutionPad_Saving(object sender, EventArgs e)
        {
            if (IsDirty)
            {
                m_pageItem.Page = EditorRoot.WorkingCopy;
            }
        }

        void EditorRoot_Edited(INodeWrapper sender, NodeBase oldNode, NodeBase newNode)
        {
            // If we want to cache the page edits at the page level before saving, then
            // use IsDirty = true.  Otherwise, set m_pageItem.Page = EditorRoot.WorkingCopy
            //IsDirty = true;
            m_pageItem.Page = EditorRoot.WorkingCopy;
        }

        public override void OnClosed(object sender, EventArgs e)
        {
            base.OnClosed(sender, e);

            // clean up event handlers
            solutionPad.Saving -= new EventHandler(solutionPad_Saving);
            m_pageItem.PropertyChanged -= new PropertyChangedEventHandler(m_pageItem_PropertyChanged);
            m_EditorRoot.Edited -= new EditedHandler(EditorRoot_Edited);
            m_EditorRoot.UndoableAction -= new UndoableActionHandler(EditorRoot_UndoableAction);
            m_pageItem = null;
            var docsToRemove = new List<string>();
            foreach (var k in m_docs.Keys)
            {
                if (this == m_docs[k])
                {
                    docsToRemove.Add(k);
                }
            }
            foreach (var k in docsToRemove)
            {
                m_docs.Remove(k);
            }
        }
        #endregion

        #region " solutionPadSingleton "
        [Import(CompositionPoints.Workbench.Pads.SolutionPad, typeof(SolutionPad))]
        private SolutionPad solutionPad
        {
            get
            {
                return m_solutionPad;
            }
            set
            {
                m_solutionPad = value;
            }
        }
        private static SolutionPad m_solutionPad = null;
        #endregion

        #region " instructionGroupItemsSingleton "
        [ImportMany(ExtensionPoints.Workbench.Documents.PageEditor.InstructionGroupItems,
            typeof(IInstructionGroupItem), AllowRecomposition = true)]
        private IEnumerable<Lazy<IInstructionGroupItem, IInstructionGroupItemMeta>> instructionGroupItems
        {
            get
            {
                return m_instructionGroupItems;
            }
            set
            {
                m_instructionGroupItems = value;
            }
        }
        private static IEnumerable<Lazy<IInstructionGroupItem, IInstructionGroupItemMeta>> m_instructionGroupItems = null;
        #endregion

        #region " contextMenuSingleton "
        [ImportMany(ExtensionPoints.Workbench.Documents.PageEditor.ContextMenu,
            typeof(IMenuItem), AllowRecomposition = true)]
        private IEnumerable<IMenuItem> contextMenu
        {
            get
            {
                return m_contextMenu;
            }
            set
            {
                m_contextMenu = value;
            }
        }
        private static IEnumerable<IMenuItem> m_contextMenu = null;
        #endregion

        #region " extensionServiceSingleton "
        [Import(SoapBox.Core.Services.Host.ExtensionService)]
        private IExtensionService extensionService
        {
            get
            {
                return m_extensionService;
            }
            set
            {
                m_extensionService = value;
            }
        }
        private static IExtensionService m_extensionService = null;
        #endregion

        #region " messagingServiceSingleton "
        [Import(SoapBox.Core.Services.Messaging.MessagingService)]
        private IMessagingService messagingService
        {
            get
            {
                return m_messagingService;
            }
            set
            {
                m_messagingService = value;
            }
        }
        private static IMessagingService m_messagingService = null;
        #endregion

        #region " pageItem "
        private PageItem m_pageItem = null;
        static readonly string m_PageName = 
            NotifyPropertyChangedHelper.GetPropertyName<PageItem>(o => o.Page);

        void m_pageItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == m_PageName)
            {
                OnNodeChanged();
            }
        }

        private void OnNodeChanged()
        {
            Memento = m_pageItem.Page.ID.ToString();
            if (!m_docs.ContainsKey(Memento))
            {
                m_docs.Add(Memento, this);
            }
            IsDirty = false;
        }

        public PageItem PageItemParent
        {
            get
            {
                return m_pageItem;
            }
        }
        #endregion

        public PageEditorItem EditorRoot
        {
            get
            {
                return m_EditorRoot;
            }
        }
        private readonly PageEditorItem m_EditorRoot = null;

        #region " IsDirty "
        public bool IsDirty
        {
            get
            {
                return m_IsDirty;
            }
            set
            {
                if (m_IsDirty != value)
                {
                    m_IsDirty = value;
                    NotifyPropertyChanged(m_IsDirtyArgs);
                }
                if (m_IsDirty)
                {
                    Title = m_pageItem.Page.PageName.ToString() + " *";
                    solutionPad.RequiresSave(this);
                }
                else
                {
                    Title = m_pageItem.Page.PageName.ToString();
                    solutionPad.NoLongerRequiresSave(this);
                }
            }
        }
        private bool m_IsDirty = false;
        private static readonly PropertyChangedEventArgs m_IsDirtyArgs =
            NotifyPropertyChangedHelper.CreateArgs<PageEditor>(o => o.IsDirty);
        private static string m_IsDirtyName =
            NotifyPropertyChangedHelper.GetPropertyName<PageEditor>(o => o.IsDirty);
        #endregion

        #region "CreateDocument"

        private static Dictionary<string, PageEditor> m_docs =
            new Dictionary<string, PageEditor>();

        /// <summary>
        /// Assumes the memento is the Node ID (FieldGuid)
        /// </summary>
        /// <param name="memento"></param>
        /// <returns></returns>
        public override IDocument CreateDocument(string memento)
        {
            if (memento == null)
            {
                throw new ArgumentNullException();
            }

            if (!m_docs.ContainsKey(memento))
            {
                ISolutionItem item = null;
                if (FieldGuid.CheckSyntax(memento))
                {
                    item = solutionPad.FindItemByNodeId(new Guid(memento)) as ISolutionItem;
                }

                if (item != null && item is PageItem)
                {
                    var pe = new PageEditor(item as PageItem); // adds itself to m_docs
                }
                else
                {
                    return null;
                }
            }
            return m_docs[memento];
        }
        #endregion

        #region " Undo/Redo "

        private readonly Stack<IUndoMemento> m_undoStack = new Stack<IUndoMemento>();
        private readonly Stack<IUndoMemento> m_redoStack = new Stack<IUndoMemento>();

        private void initUndoRedo()
        {
            m_redoStack.Clear();
            m_undoStack.Clear();
        }

        /// <summary>
        /// This gets called whenever there's *any* UndoableAction
        /// event below this in the page.  We record it in case
        /// the user wants to undo.
        /// </summary>
        void EditorRoot_UndoableAction(IUndoMemento memento)
        {
            m_redoStack.Clear();
            if (m_undoStack.Count > 0)
            {
                var last = m_undoStack.Peek();
                memento.BindWithPrevious = last.BindWithNext;
            }
            m_undoStack.Push(memento);
            boundUndoStackSize();
        }

        /// <summary>
        /// Call this from anywhere to execute an Undo on the page
        /// </summary>
        public void Undo()
        {
            if (m_undoStack.Count > 0)
            {
                IsDirty = true;
                var undoMemento = m_undoStack.Pop();
                m_redoStack.Push(undoMemento);
                undoMemento.Sender.Undo(undoMemento);
                if (undoMemento.BindWithPrevious)
                {
                    Undo();
                }
            }
        }

        /// <summary>
        /// Call this from anywhere to execute a Redo on the page
        /// </summary>
        public void Redo()
        {
            if (m_redoStack.Count > 0)
            {
                IsDirty = true;
                var undoMemento = m_redoStack.Pop();
                m_undoStack.Push(undoMemento);
                undoMemento.Sender.Redo(undoMemento);
                if (undoMemento.BindWithNext)
                {
                    Redo();
                }
            }
        }

        private void boundUndoStackSize()
        {
            if (m_undoStack.Count > UNDO_UPPER_LIMIT)
            {
                var tempStack = new Stack<IUndoMemento>();
                for (int i = 0; i < UNDO_RESIZE_SIZE; i++)
                {
                    tempStack.Push(m_undoStack.Pop());
                }
                m_undoStack.Clear();
                while (tempStack.Count > 0)
                {
                    m_undoStack.Push(tempStack.Pop());
                }
            }
        }

        #endregion

        #region " Class: PageEditorItem "
        public class PageEditorItem : AbstractEditorItem, IContextMenu
        {
            public PageEditorItem(PageEditor parent, NodePage page)
                : base(null)
            {
                if (page == null)
                {
                    throw new ArgumentNullException();
                }
                m_parent = parent;
                WorkingCopy = page;
            }

            private PageEditor m_parent = null;

            public PageEditor PageEditorParent
            {
                get
                {
                    return m_parent;
                }
            }

            #region "Working Copy"

            public NodePage WorkingCopy
            {
                get
                {
                    return Node as NodePage;
                }
                set
                {
                    if (value == null)
                    {
                        throw new ArgumentNullException(m_WorkingCopyName);
                    }
                    if (Node != value)
                    {
                        Node = value;
                        setItems();
                        NotifyPropertyChanged(m_WorkingCopyArgs);
                    }
                }
            }
            static readonly string m_WorkingCopyName =
                NotifyPropertyChangedHelper.GetPropertyName<PageEditorItem>(o => o.WorkingCopy);
            static readonly PropertyChangedEventArgs m_WorkingCopyArgs =
                NotifyPropertyChangedHelper.CreateArgs<PageEditorItem>(o => o.WorkingCopy);

            #endregion

            private void setItems()
            {
                foreach (var nodeWrapper in Items)
                {
                    var groupItem = nodeWrapper as IInstructionGroupItem;
                    if (groupItem != null)
                    {
                        groupItem.Edited -= new EditedHandler(InstructionGroupItem_Edited);
                        groupItem.Deleted -= new DeletedHandler(InstructionGroupItem_Deleted);
                    }
                } 
                var newCollection = new ObservableCollection<INodeWrapper>();
                foreach (var n in WorkingCopy.NodeInstructionGroupChildren.Items)
                {
                    var item = FindItemByNodeId(n.ID) as IInstructionGroupItem;
                    if (item == null)
                    {
                        // Need to find the right Instruction Group Editor Factory
                        foreach (var groupEditor in m_parent.instructionGroupItems)
                        {
                            if (groupEditor.Metadata.Language == n.Language.ToString())
                            {
                                item = groupEditor.Value.Create(this, n);
                                HookupHandlers(item);
                                break;
                            }
                        }
                        if (item == null)
                        {
                            // plugin not found
                            item = new InstructionGroupDummy(this, n);
                        }
                    }
                    else
                    {
                        HookupHandlers(item);
                    }
                    newCollection.Add(item);
                }
                foreach (var item in Items)
                {
                    if (!newCollection.Contains(item))
                    {
                        item.Parent = null;
                    }
                }
                Items = newCollection;
            }

            void HookupHandlers(IInstructionGroupItem groupItem)
            {
                groupItem.Parent = this;
                groupItem.Edited += new EditedHandler(InstructionGroupItem_Edited);
                groupItem.Deleted += new DeletedHandler(InstructionGroupItem_Deleted);
            }

            void InstructionGroupItem_Edited(INodeWrapper sender, NodeBase oldNode, NodeBase newNode)
            {
                var oldInstructionGroup = oldNode as NodeInstructionGroup;
                var newInstructionGroup = newNode as NodeInstructionGroup;
                if (oldInstructionGroup != null && newInstructionGroup != null)
                {
                    WorkingCopy = WorkingCopy.NodeInstructionGroupChildren.Replace(oldInstructionGroup, newInstructionGroup);
                }
            }

            void InstructionGroupItem_Deleted(INodeWrapper sender, NodeBase deletedNode)
            {
                var deletedInstructionGroup = deletedNode as NodeInstructionGroup;
                var editorItem = sender as IEditorItem;
                if (deletedInstructionGroup != null && editorItem != null)
                {
                    if (editorItem.Parent == this)
                    {
                        editorItem.Parent = null;
                    }
                    WorkingCopy = WorkingCopy.NodeInstructionGroupChildren.Remove(deletedInstructionGroup);
                    editorItem.Edited -= new EditedHandler(InstructionGroupItem_Edited);
                    editorItem.Deleted -= new DeletedHandler(InstructionGroupItem_Deleted);
                    Items.Remove(editorItem);
                }
            }

            #region " Comment "
            public string Comment
            {
                get
                {
                    return WorkingCopy.Comment.ToString();
                }
                set
                {
                    if (value == null)
                    {
                        throw new ArgumentNullException(m_CommentName);
                    }
                    // Need copies of these to use in the Closures later
                    FieldString existingValue = WorkingCopy.Comment;
                    FieldString newValue = new FieldString(value);
                    if (existingValue != newValue)
                    {
                        var undoActions = new List<Action>();
                        undoActions.Add(() => WorkingCopy = WorkingCopy.SetComment(existingValue));
                        undoActions.Add(() => NotifyPropertyChanged(m_CommentArgs));

                        var redoActions = new List<Action>();
                        redoActions.Add(() => WorkingCopy = WorkingCopy.SetComment(newValue));
                        redoActions.Add(() => NotifyPropertyChanged(m_CommentArgs));

                        Do(new UndoMemento(this, ActionType.EDIT,
                            Resources.Strings.PageEditor_UndoDescription_EditComment,
                            undoActions, redoActions));

                    }
                }
            }
            static readonly string m_CommentName =
                NotifyPropertyChangedHelper.GetPropertyName<PageEditorItem>(o => o.Comment);
            static readonly PropertyChangedEventArgs m_CommentArgs =
                NotifyPropertyChangedHelper.CreateArgs<PageEditorItem>(o => o.Comment);

            public bool CommentBeingEdited
            {
                get
                {
                    return m_CommentBeingEdited;
                }
                set
                {
                    if (m_CommentBeingEdited != value)
                    {
                        m_CommentBeingEdited = value;
                        NotifyPropertyChanged(m_CommentBeingEditedArgs);
                        m_CommentBeingEditedCondition.SetCondition(CommentBeingEdited);
                        m_CommentNotBeingEditedCondition.SetCondition(!CommentBeingEdited);
                    }
                }
            }
            private bool m_CommentBeingEdited = false;
            static readonly PropertyChangedEventArgs m_CommentBeingEditedArgs =
                NotifyPropertyChangedHelper.CreateArgs<PageEditorItem>(o => o.CommentBeingEdited);

            private ConcreteCondition m_CommentBeingEditedCondition = new ConcreteCondition(false);
            private ConcreteCondition m_CommentNotBeingEditedCondition = new ConcreteCondition(true);

            /// <summary>
            /// Scratchpad for the text box
            /// </summary>
            public string CommentEdit
            {
                get
                {
                    return m_CommentEdit;
                }
                set
                {
                    if (m_CommentEdit != value)
                    {
                        m_CommentEdit = value;
                        NotifyPropertyChanged(m_CommentEditArgs);
                    }
                }
            }
            private string m_CommentEdit = string.Empty;
            static readonly PropertyChangedEventArgs m_CommentEditArgs =
                NotifyPropertyChangedHelper.CreateArgs<PageEditorItem>(o => o.CommentEdit);
            #endregion

            #region " CommentButtons "

            /// <summary>
            /// Lazily instantiated collection of buttons
            /// for across the top of the comment editor
            /// </summary>
            public IEnumerable<IButton> CommentButtons
            {
                get
                {
                    if (m_CommentButtons == null)
                    {
                        m_CommentButtons = new Collection<IButton>();
                        m_CommentButtons.Add(new EditButton(this));
                        m_CommentButtons.Add(new CommitButton(this));
                        m_CommentButtons.Add(new CancelButton(this));
                        m_CommentButtons.Add(new MarkdownButton(this));
                    }
                    return m_CommentButtons;
                }
            }
            private Collection<IButton> m_CommentButtons = null;

            #region " Edit Button "
            private class EditButton : AbstractToolBarButton
            {
                public EditButton(PageEditorItem doc)
                {
                    m_doc = doc;
                    Text = Resources.Strings.PageEditor_CommentButtons_Edit;
                    SetIconFromBitmap(Resources.Images.PageEditor_CommentButtons_Edit);
                    VisibleCondition = doc.m_CommentNotBeingEditedCondition;
                }

                private PageEditorItem m_doc = null;

                protected override void Run()
                {
                    m_doc.CommentEdit = m_doc.Comment;
                    m_doc.CommentBeingEdited = true;
                }
            }
            #endregion

            #region " Commit Button "
            private class CommitButton : AbstractToolBarButton
            {
                public CommitButton(PageEditorItem doc)
                {
                    m_doc = doc;
                    Text = Resources.Strings.PageEditor_CommentButtons_Commit;
                    SetIconFromBitmap(Resources.Images.PageEditor_CommentButtons_Commit);
                    VisibleCondition = doc.m_CommentBeingEditedCondition;
                }

                private PageEditorItem m_doc = null;

                protected override void Run()
                {
                    m_doc.Comment = m_doc.CommentEdit;
                    m_doc.CommentBeingEdited = false;
                }
            }
            #endregion

            #region " Cancel Button "
            private class CancelButton : AbstractToolBarButton
            {
                public CancelButton(PageEditorItem doc)
                {
                    m_doc = doc;
                    Text = Resources.Strings.PageEditor_CommentButtons_Cancel;
                    SetIconFromBitmap(Resources.Images.PageEditor_CommentButtons_Cancel);
                    VisibleCondition = doc.m_CommentBeingEditedCondition;
                }

                private PageEditorItem m_doc = null;

                protected override void Run()
                {
                    m_doc.CommentBeingEdited = false;
                }
            }
            #endregion

            #region " Markdown Button "
            private class MarkdownButton : AbstractToolBarButton
            {
                public MarkdownButton(PageEditorItem doc)
                {
                    m_doc = doc;
                    Text = Resources.Strings.PageEditor_CommentButtons_Markdown;
                    ToolTip = Resources.Strings.PageEditor_CommentButtons_Markdown_ToolTip;
                    SetIconFromBitmap(Resources.Images.PageEditor_CommentButtons_Markdown);
                    VisibleCondition = doc.m_CommentBeingEditedCondition;
                }

                private PageEditorItem m_doc = null;

                protected override void Run()
                {
                    MarkdownUtility.MarkdownBinding.NavigateToMarkdownSyntax();
                }
            }
            #endregion

            #endregion

            #region " ContextMenu "

            public IEnumerable<IMenuItem> ContextMenu
            {
                get
                {
                    return m_ContextMenu;
                }
                set
                {
                    if (m_ContextMenu != value)
                    {
                        m_ContextMenu = value;
                        NotifyPropertyChanged(m_ContextMenuArgs);
                    }
                }
            }
            private IEnumerable<IMenuItem> m_ContextMenu = null;
            static readonly PropertyChangedEventArgs m_ContextMenuArgs =
                NotifyPropertyChangedHelper.CreateArgs<PageEditorItem>(o => o.ContextMenu);

            #endregion

            #region " ContextMenuEnabled "
            public bool ContextMenuEnabled
            {
                get
                {
                    return m_ContextMenuEnabled;
                }
                set
                {
                    if (m_ContextMenuEnabled != value)
                    {
                        m_ContextMenuEnabled = value;
                        NotifyPropertyChanged(m_ContextMenuEnabledArgs);
                    }
                }
            }
            private bool m_ContextMenuEnabled = true;
            private static readonly PropertyChangedEventArgs m_ContextMenuEnabledArgs =
                NotifyPropertyChangedHelper.CreateArgs<PageEditorItem>(o => o.ContextMenuEnabled);
            private static string m_ContextMenuEnabledName =
                NotifyPropertyChangedHelper.GetPropertyName<PageEditorItem>(o => o.ContextMenuEnabled);
            #endregion

            public void DeleteSelectedGroups()
            {
                DeleteSelectedGroups(false);
            }
                
            public void DeleteSelectedGroups(bool bindWithNext)
            {
                if (runtimeService.DisconnectDialog(this))
                {
                    var origPage = WorkingCopy;
                    var newPage = WorkingCopy;
                    var undoActions = new List<Action>();
                    var doActions = new List<Action>();
                    foreach (var item in Items)
                    {
                        if (item.IsSelected)
                        {
                            undoActions.Add(() => Items.Add(item));
                            newPage = newPage.NodeInstructionGroupChildren.Remove(item.Node as NodeInstructionGroup);
                        }
                    }
                    doActions.Add(() => WorkingCopy = newPage);
                    undoActions.Add(() => WorkingCopy = origPage);
                    Do(new UndoMemento(this, ActionType.DELETE,
                        Resources.Strings.PageEditor_UndoDescription_DeleteSelectedGroups, undoActions, doActions, bindWithNext));
                }
            }

            /// <summary>
            /// Helper method
            /// </summary>
            /// <param name="newItem">Item created by factory</param>
            /// <param name="newPage">NodePage with node already inserted at the desired position</param>
            private void insertNewGroup(IInstructionGroupItem newItem, NodePage newPage)
            {
                if (runtimeService.DisconnectDialog(this))
                {
                    NodePage origPage = WorkingCopy;
                    var undoActions = new List<Action>();
                    var doActions = new List<Action>();
                    doActions.Add(() => Items.Add(newItem));
                    doActions.Add(() => WorkingCopy = newPage);
                    undoActions.Add(() => WorkingCopy = origPage);
                    Do(new UndoMemento(this, ActionType.ADD,
                        Resources.Strings.PageEditor_UndoDescription_InsertGroup, undoActions, doActions));
                }
            }

            public void AppendInstructionGroup(IInstructionGroupItem groupFactory)
            {
                if (runtimeService.DisconnectDialog(this))
                {
                    var newItem = groupFactory.Create(this, null);
                    NodePage newPage = WorkingCopy.NodeInstructionGroupChildren.Append(newItem.InstructionGroup);
                    insertNewGroup(newItem, newPage);
                }
            }

            // Inserts a new group before the first selected group
            public void InsertBeforeInstructionGroup(IInstructionGroupItem groupFactory)
            {
                if (runtimeService.DisconnectDialog(this))
                {
                    bool found = false;
                    foreach (var instructionGroupIterator in Items)
                    {
                        IInstructionGroupItem instructionGroup = null;
                        instructionGroup = instructionGroupIterator as IInstructionGroupItem;
                        if (instructionGroup != null && instructionGroup.IsSelected)
                        {
                            var newItem = groupFactory.Create(this, null);
                            NodePage newPage = WorkingCopy.NodeInstructionGroupChildren.InsertBefore(instructionGroup.InstructionGroup, newItem.InstructionGroup);
                            insertNewGroup(newItem, newPage);
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        PageEditorParent.messagingService.ShowMessage(
                            Resources.Strings.PageEditor_GroupCommand_NoneSelected,
                            Resources.Strings.PageEditor_GroupCommand_NoneSelected_Title);
                    }
                }
            }

            // Inserts a new group after the last selected group
            public void InsertAfterInstructionGroup(IInstructionGroupItem groupFactory)
            {
                if (runtimeService.DisconnectDialog(this))
                {
                    IInstructionGroupItem foundIG = null;
                    foreach (var instructionGroupIterator in Items)
                    {
                        IInstructionGroupItem instructionGroup = null;
                        instructionGroup = instructionGroupIterator as IInstructionGroupItem;
                        if (instructionGroup != null && instructionGroup.IsSelected)
                        {
                            foundIG = instructionGroup;
                        }
                    }
                    if (foundIG != null)
                    {
                        var newItem = groupFactory.Create(this, null);
                        NodePage newPage = WorkingCopy.NodeInstructionGroupChildren.InsertAfter(foundIG.InstructionGroup, newItem.InstructionGroup);
                        insertNewGroup(newItem, newPage);
                    }
                    else
                    {
                        PageEditorParent.messagingService.ShowMessage(
                            Resources.Strings.PageEditor_GroupCommand_NoneSelected,
                            Resources.Strings.PageEditor_GroupCommand_NoneSelected_Title);
                    }
                }
            }

            private bool onlyOneGroupSelected()
            {
                var t = from i in Items where i.IsSelected select i;
                if (t.Count() == 1)
                {
                    return true;
                }
                else
                {
                    PageEditorParent.messagingService.ShowMessage(
                        Resources.Strings.PageEditor_GroupCommand_NoneSelected,
                        Resources.Strings.PageEditor_GroupCommand_NoneSelected_Title);
                    return false;
                }
            }

            private IInstructionGroupItem findFirstSelectedInstructionGroup()
            {
                var t = from i in Items where i.IsSelected select i;
                return t.First() as IInstructionGroupItem;
            }

            private void simplePageEdit(NodePage newPage, string undoDescription)
            {
                NodePage origPage = WorkingCopy;
                var undoActions = new List<Action>();
                var doActions = new List<Action>();
                doActions.Add(() => WorkingCopy = newPage);
                undoActions.Add(() => WorkingCopy = origPage);
                Do(new UndoMemento(this, ActionType.EDIT,
                    undoDescription, undoActions, doActions));
            }

            public void MoveSelectedUp()
            {
                if (runtimeService.DisconnectDialog(this))
                {
                    if (onlyOneGroupSelected())
                    {
                        var ig = findFirstSelectedInstructionGroup();
                        if (ig != null)
                        {
                            var igNode = ig.InstructionGroup; // the node getting moved
                            NodeInstructionGroup igBeforeNode = null;
                            foreach (var igTest in WorkingCopy.NodeInstructionGroupChildren.Items)
                            {
                                if (igTest == igNode)
                                {
                                    break;
                                }
                                else
                                {
                                    igBeforeNode = igTest;
                                }
                            }
                            if (igBeforeNode != null)
                            {
                                NodePage newPage;
                                newPage = WorkingCopy.NodeInstructionGroupChildren.Remove(igNode);
                                newPage = newPage.NodeInstructionGroupChildren.InsertBefore(igBeforeNode, igNode);
                                simplePageEdit(newPage, Resources.Strings.PageEditor_UndoDescription_MoveGroupUp);
                            }
                        }
                    }
                }
            }

            public void MoveSelectedDown()
            {
                if (runtimeService.DisconnectDialog(this))
                {
                    if (onlyOneGroupSelected())
                    {
                        var ig = findFirstSelectedInstructionGroup();
                        if (ig != null)
                        {
                            var igNode = ig.InstructionGroup; // the node getting moved
                            NodeInstructionGroup igAfterNode = null;
                            bool found = false;
                            foreach (var igTest in WorkingCopy.NodeInstructionGroupChildren.Items)
                            {
                                if (igTest == igNode)
                                {
                                    found = true;
                                }
                                else if (found)
                                {
                                    igAfterNode = igTest;
                                    break;
                                }
                            }
                            if (igAfterNode != null)
                            {
                                NodePage newPage;
                                newPage = WorkingCopy.NodeInstructionGroupChildren.Remove(igNode);
                                newPage = newPage.NodeInstructionGroupChildren.InsertAfter(igAfterNode, igNode);
                                simplePageEdit(newPage, Resources.Strings.PageEditor_UndoDescription_MoveGroupDown);
                            }
                        }
                    }
                }
            }

            private IList<NodeInstructionGroup> getSelectedNodes()
            {
                var t = from i in Items where i.IsSelected select ((IInstructionGroupItem)i).InstructionGroup as NodeInstructionGroup;
                return new List<NodeInstructionGroup>(t);
            }

            public void MoveSelectedBefore(IInstructionGroupItem instructionGroup)
            {
                if (runtimeService.DisconnectDialog(this))
                {
                    var nodesToMove = new ReadOnlyCollection<NodeInstructionGroup>(getSelectedNodes());
                    NodePage newPage = WorkingCopy;
                    newPage = newPage.NodeInstructionGroupChildren.Remove(nodesToMove);
                    newPage = newPage.NodeInstructionGroupChildren.InsertBefore(instructionGroup.InstructionGroup, nodesToMove);
                    simplePageEdit(newPage, Resources.Strings.PageEditor_UndoDescription_MoveGroups);
                }
            }

            public void MoveSelectedAfter(IInstructionGroupItem instructionGroup)
            {
                if (runtimeService.DisconnectDialog(this))
                {
                    var nodesToMove = new ReadOnlyCollection<NodeInstructionGroup>(getSelectedNodes());
                    NodePage newPage = WorkingCopy;
                    newPage = newPage.NodeInstructionGroupChildren.Remove(nodesToMove);
                    newPage = newPage.NodeInstructionGroupChildren.InsertAfter(instructionGroup.InstructionGroup, nodesToMove);
                    simplePageEdit(newPage, Resources.Strings.PageEditor_UndoDescription_MoveGroups);
                }
            }

        }
        #endregion

    }
}
