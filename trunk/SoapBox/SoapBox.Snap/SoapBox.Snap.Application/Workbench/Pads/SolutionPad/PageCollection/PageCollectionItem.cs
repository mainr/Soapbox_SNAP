#region "SoapBox.Snap License"
/// <header module="SoapBox.Snap"> 
/// Copyright (C) 2009 SoapBox Automation Inc., All Rights Reserved.
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
using System.ComponentModel.Composition;
using SoapBox.Core;
using System.Collections.ObjectModel;
using SoapBox.Protocol.Base;
using SoapBox.Protocol.Automation;
using SoapBox.Utilities;
using System.ComponentModel;
using System.Windows.Input;

namespace SoapBox.Snap.Application
{
    [Export(ExtensionPoints.Workbench.Pads.SolutionPad.SolutionItems, typeof(ISolutionItem))]
    public class PageCollectionItem : AbstractSolutionItem
    {
        /// <summary>
        /// Called by MEF (just so we can do imports)
        /// </summary>
        private PageCollectionItem()
            : base(null, string.Empty)
        {
        }

        public PageCollectionItem(ISolutionItem parent, NodePageCollection pageCollection)
            : base(parent, pageCollection.PageCollectionName.ToString())
        {
            ContextMenu = extensionService.Sort(contextMenu);
            PageCollection = pageCollection;
            SetIconFromBitmap(Resources.Images.PageCollection);
        }

        #region "contextMenuSingleton"
        [ImportMany(ExtensionPoints.Workbench.Pads.SolutionPad.PageCollectionItem.ContextMenu, typeof(IMenuItem))]
        private IEnumerable<IMenuItem> contextMenu
        {
            get
            {
                return contextMenuSingleton;
            }
            set
            {
                contextMenuSingleton = value;
            }
        }
        private static IEnumerable<IMenuItem> contextMenuSingleton = null;
        #endregion

        #region "PageCollection"

        public NodePageCollection PageCollection
        {
            get
            {
                return Node as NodePageCollection;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(m_PageCollectionName);
                }
                Node = value;
                setItems();
                NotifyPropertyChanged(m_PageCollectionArgs);
            }
        }
        private static PropertyChangedEventArgs m_PageCollectionArgs
            = NotifyPropertyChangedHelper.CreateArgs<PageCollectionItem>(o => o.PageCollection);
        private static string m_PageCollectionName
            = NotifyPropertyChangedHelper.GetPropertyName<PageCollectionItem>(o => o.PageCollection);

        #endregion

        #region "Header"

        public override void HeaderEditAccept()
        {
            base.HeaderEditAccept();
            bool accepted = FieldPageCollectionName.CheckSyntax(HeaderEdit);
            if (accepted)
            {
                PageCollection = PageCollection.SetPageCollectionName(new FieldPageCollectionName(HeaderEdit));
            }
            else
            {
                HeaderEdit = PageCollection.PageCollectionName.ToString();
            }
        }

        public override void HeaderEditCancel()
        {
            base.HeaderEditCancel();
            HeaderEdit = PageCollection.PageCollectionName.ToString();
        }

        #endregion

        private void setItems()
        {
            if (PageCollection.LogicRoot.BoolValue)
            {
                HeaderIsEditable = false;
                Header = Resources.Strings.Solution_Pad_PageCollectionItem_LogicRootName;
            }
            else
            {
                HeaderIsEditable = true;
                Header = PageCollection.PageCollectionName.ToString();
            }

            var newCollection = new ObservableCollection<INodeWrapper>();
            foreach (var nPageCollection in PageCollection.NodePageCollectionChildren.Items)
            {
                var pageCollection = FindItemByNodeId(nPageCollection.ID) as PageCollectionItem;
                if (pageCollection == null)
                {
                    pageCollection = new PageCollectionItem(this, nPageCollection);
                    HookupHandlers(pageCollection);
                }
                newCollection.Add(pageCollection);
            }
            foreach (var nPage in PageCollection.NodePageChildren.Items)
            {
                var page = FindItemByNodeId(nPage.ID) as PageItem;
                if (page == null)
                {
                    page = new PageItem(this, nPage);
                    HookupHandlers(page);
                }
                newCollection.Add(page);
            }
            Items = newCollection;
        }

        void HookupHandlers(PageCollectionItem pageCollection)
        {
            pageCollection.Parent = this;
            pageCollection.Edited += new EditedHandler(PageCollection_Edited);
            pageCollection.Deleted += new DeletedHandler(PageCollection_Deleted);
            pageCollection.MoveUpRequest += new EventHandler(pageCollection_MoveUpRequest);
            pageCollection.MoveDownRequest += new EventHandler(pageCollection_MoveDownRequest);
        }

        void pageCollection_MoveDownRequest(object sender, EventArgs e)
        {
            var senderPageCollection = sender as PageCollectionItem;
            if (senderPageCollection != null)
            {
                var pageCollection = senderPageCollection.PageCollection;
                var indexOfNext = PageCollection.NodePageCollectionChildren.Items.IndexOf(pageCollection) + 1;
                if (indexOfNext < PageCollection.NodePageCollectionChildren.Items.Count)
                {
                    var nextPageCollection = PageCollection.NodePageCollectionChildren.Items[indexOfNext];
                    var intermediate = PageCollection.NodePageCollectionChildren.Remove(pageCollection);
                    PageCollection = intermediate.NodePageCollectionChildren.InsertAfter(nextPageCollection, pageCollection);
                }
            }
        }

        void pageCollection_MoveUpRequest(object sender, EventArgs e)
        {
            var senderPageCollection = sender as PageCollectionItem;
            if (senderPageCollection != null)
            {
                var pageCollection = senderPageCollection.PageCollection;
                var indexOfPrevious = PageCollection.NodePageCollectionChildren.Items.IndexOf(pageCollection) - 1;
                if (indexOfPrevious >= 0)
                {
                    var previousPageCollection = PageCollection.NodePageCollectionChildren.Items[indexOfPrevious];
                    var intermediate = PageCollection.NodePageCollectionChildren.Remove(pageCollection);
                    PageCollection = intermediate.NodePageCollectionChildren.InsertBefore(previousPageCollection, pageCollection);
                }
            }
        }

        void PageCollection_Edited(INodeWrapper sender, NodeBase oldNode, NodeBase newNode)
        {
            var oldPageCollection = oldNode as NodePageCollection;
            var newPageCollection = newNode as NodePageCollection;
            if (oldPageCollection != null && newPageCollection != null)
            {
                PageCollection = PageCollection.NodePageCollectionChildren.Replace(oldPageCollection, newPageCollection);
            }
        }

        void PageCollection_Deleted(INodeWrapper sender, NodeBase deletedNode)
        {
            var deletedPageCollection = deletedNode as NodePageCollection;
            var solutionItem = sender as PageCollectionItem;
            if (deletedPageCollection != null && solutionItem != null)
            {
                if (solutionItem.Parent == this)
                {
                    solutionItem.Parent = null;
                }
                PageCollection = PageCollection.NodePageCollectionChildren.Remove(deletedPageCollection);
                solutionItem.Edited -= new EditedHandler(PageCollection_Edited);
                solutionItem.Deleted -= new DeletedHandler(PageCollection_Deleted);
                solutionItem.MoveUpRequest -= new EventHandler(pageCollection_MoveUpRequest);
                solutionItem.MoveDownRequest -= new EventHandler(pageCollection_MoveDownRequest); 
                Items.Remove(solutionItem);
            }
        }

        void HookupHandlers(PageItem page)
        {
            page.Parent = this;
            page.Edited += new EditedHandler(Page_Edited);
            page.Deleted += new DeletedHandler(Page_Deleted);
            page.InsertAfter += new InsertAfterHandler(Page_InsertAfter);
            page.MoveDownRequest += new EventHandler(page_MoveDownRequest);
            page.MoveUpRequest += new EventHandler(page_MoveUpRequest);
        }

        void Page_Edited(INodeWrapper sender, NodeBase oldNode, NodeBase newNode)
        {
            var oldPage = oldNode as NodePage;
            var newPage = newNode as NodePage;
            if (oldPage != null && newPage != null)
            {
                PageCollection = PageCollection.NodePageChildren.Replace(oldPage, newPage);
            }
        }

        void Page_Deleted(INodeWrapper sender, NodeBase deletedNode)
        {
            var deletedPage = deletedNode as NodePage;
            var solutionItem = sender as PageItem;
            if (deletedPage != null && solutionItem != null)
            {
                if (solutionItem.Parent == this)
                {
                    solutionItem.Parent = null;
                }
                PageCollection = PageCollection.NodePageChildren.Remove(deletedPage);
                solutionItem.Edited -= new EditedHandler(Page_Edited);
                solutionItem.Deleted -= new DeletedHandler(Page_Deleted);
                solutionItem.InsertAfter -= new InsertAfterHandler(Page_InsertAfter);
                solutionItem.MoveDownRequest -= new EventHandler(page_MoveDownRequest);
                solutionItem.MoveUpRequest -= new EventHandler(page_MoveUpRequest);
                Items.Remove(solutionItem);
            }
        }

        void Page_InsertAfter(ISolutionItem sender, ISolutionItem newItem)
        {
            var senderPage = sender as PageItem;
            var dropPage = newItem as PageItem;
            if (senderPage != null && dropPage != null
                && senderPage != dropPage)
            {
                var page = dropPage.Page;
                dropPage.Delete();
                Items.Add(dropPage);
                HookupHandlers(dropPage);
                PageCollection = PageCollection.NodePageChildren.InsertAfter(senderPage.Page, page);
            }
        }

        void page_MoveUpRequest(object sender, EventArgs e)
        {
            var senderPage = sender as PageItem;
            if (senderPage != null)
            {
                var page = senderPage.Page;
                var indexOfPrevious = PageCollection.NodePageChildren.Items.IndexOf(page) - 1;
                if (indexOfPrevious >= 0)
                {
                    var previousPage = PageCollection.NodePageChildren.Items[indexOfPrevious];
                    var intermediate = PageCollection.NodePageChildren.Remove(page);
                    PageCollection = intermediate.NodePageChildren.InsertBefore(previousPage, page);
                }
            }
        }

        void page_MoveDownRequest(object sender, EventArgs e)
        {
            var senderPage = sender as PageItem;
            if (senderPage != null)
            {
                var page = senderPage.Page;
                var indexOfNext = PageCollection.NodePageChildren.Items.IndexOf(page) + 1;
                if (indexOfNext < PageCollection.NodePageChildren.Items.Count)
                {
                    var nextPage = PageCollection.NodePageChildren.Items[indexOfNext];
                    var intermediate = PageCollection.NodePageChildren.Remove(page);
                    PageCollection = intermediate.NodePageChildren.InsertAfter(nextPage, page);
                }
            }
        }

        public void AddPage()
        {
            if (runtimeService.DisconnectDialog(this))
            {
                PageCollection = PageCollection.NodePageChildren.Append(
                    NodePage.BuildWith(new FieldPageName(
                        Resources.Strings.Solution_Pad_PageItem_NewPageName)));
            }
        }

        public void AddPageCollection()
        {
            if (runtimeService.DisconnectDialog(this))
            {
                PageCollection = PageCollection.NodePageCollectionChildren.Append(
                   NodePageCollection.BuildWith(new FieldPageCollectionName(
                       Resources.Strings.Solution_Pad_PageCollectionItem_NewPageCollectionName)));
            }
       }

        public bool Delete()
        {
            if (runtimeService.DisconnectDialog(this))
            {
                if (!PageCollection.LogicRoot.BoolValue)
                {
                    bool needsVerification = false;
                    if (PageCollection.NodePageChildren.Count > 0 || PageCollection.NodePageCollectionChildren.Count > 0)
                    {
                        needsVerification = true;
                    }

                    if (needsVerification)
                    {
                        if (messagingService.Value.ShowDialog(Resources.Strings.Solution_Pad_PageCollectionItem_DeleteConfirmation,
                            Resources.Strings.Solution_Pad_PageCollectionItem_DeleteTitle, System.Windows.Forms.MessageBoxButtons.OKCancel)
                                == System.Windows.Forms.DialogResult.Cancel)
                        {
                            return false;
                        }
                    }
                    var childPages = FindChildrenOfType<PageItem>();
                    foreach (var pg in childPages)
                    {
                        pg.DeleteConditional(false); // deletes it without asking the user
                    }
                    FireDeletedEvent(this, Node);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public override void KeyDown(object sender, KeyEventArgs e)
        {
            base.KeyDown(sender, e);
            switch (e.Key)
            {
                case Key.Delete:
                    if (Delete())
                    {
                        e.Handled = true;
                    }
                    break;
            }
        }

        protected override bool QueryCanDrag()
        {
            return !PageCollection.LogicRoot.BoolValue;
        }

        protected override bool QueryCanDrop(ISolutionItem source)
        {
            bool canDrop = (source is PageItem || source is PageCollectionItem);
            if (canDrop)
            {
                // Make sure we're not a child of the node being dropped
                var sourcePageCollection = source as PageCollectionItem;
                if (sourcePageCollection != null)
                {
                    if (sourcePageCollection.PageCollection.GetChildrenRecursive().ContainsKey(PageCollection.ID))
                    {
                        canDrop = false;
                    }
                }
            }
            if (canDrop)
            {
                IsSelected = true; // gives some nice visual feedback to the user
            }
            return canDrop;
        }

        protected override void OnDrop(ISolutionItem source)
        {
            base.OnDrop(source);

            if (source != this)
            {
                if (runtimeService.DisconnectDialog(this))
                {
                    var sourcePage = source as PageItem;
                    var sourcePageCollection = source as PageCollectionItem;
                    if (sourcePage != null)
                    {
                        var page = sourcePage.Page;
                        sourcePage.DeleteConditional(false);
                        Items.Add(sourcePage);
                        HookupHandlers(sourcePage);
                        if (PageCollection.NodePageChildren.Count > 0)
                        {
                            PageCollection = PageCollection.NodePageChildren.InsertBefore(
                                PageCollection.NodePageChildren.Items[0], page);
                        }
                        else
                        {
                            PageCollection = PageCollection.NodePageChildren.Append(page);
                        }
                    }
                    if (sourcePageCollection != null)
                    {
                        var pageCollection = sourcePageCollection.PageCollection;
                        sourcePageCollection.Delete();
                        Items.Add(sourcePageCollection);
                        HookupHandlers(sourcePageCollection);
                        if (PageCollection.NodePageCollectionChildren.Count > 0)
                        {
                            PageCollection = PageCollection.NodePageCollectionChildren.InsertBefore(
                                PageCollection.NodePageCollectionChildren.Items[0], pageCollection);
                        }
                        else
                        {
                            PageCollection = PageCollection.NodePageCollectionChildren.Append(pageCollection);
                        }
                    }
                }
            }
        }
    }
}
