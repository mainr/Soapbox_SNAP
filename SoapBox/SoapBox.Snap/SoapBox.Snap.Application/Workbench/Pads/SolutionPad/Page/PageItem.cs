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
    public class PageItem : AbstractSolutionItem
    {
        /// <summary>
        /// Called by MEF (just so we can do imports)
        /// </summary>
        private PageItem()
            : base(null, string.Empty)
        {
        }

        public PageItem(ISolutionItem parent, NodePage page)
            : base(parent, page.PageName.ToString())
        {
            ContextMenu = extensionService.Sort(contextMenu);
            Page = page;
            HeaderIsEditable = true;
            SetIconFromBitmap(Resources.Images.Page);
        }

        #region "pageEditorSingleton"
        [Import(CompositionPoints.Workbench.Documents.PageEditor, typeof(PageEditor))]
        private Lazy<PageEditor> pageEditor
        {
            get
            {
                return pageEditorSingleton;
            }
            set
            {
                pageEditorSingleton = value;
            }
        }
        private static Lazy<PageEditor> pageEditorSingleton = null;
        #endregion

        #region "contextMenuSingleton"
        [ImportMany(ExtensionPoints.Workbench.Pads.SolutionPad.PageItem.ContextMenu, typeof(IMenuItem))]
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

        #region "Page"

        public NodePage Page
        {
            get
            {
                return Node as NodePage;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(m_PageName);
                }
                Node = value;
                setItems();
                NotifyPropertyChanged(m_PageArgs);
            }
        }
        private static PropertyChangedEventArgs m_PageArgs
            = NotifyPropertyChangedHelper.CreateArgs<PageItem>(o => o.Page);
        private static string m_PageName
            = NotifyPropertyChangedHelper.GetPropertyName<PageItem>(o => o.Page);

        #endregion

        #region "Header"

        public override void HeaderEditAccept()
        {
            base.HeaderEditAccept();
            bool accepted = FieldPageName.CheckSyntax(HeaderEdit);
            if (accepted)
            {
                Page = Page.SetPageName(new FieldPageName(HeaderEdit));
            }
            else
            {
                HeaderEdit = Page.PageName.ToString();
            }
        }

        public override void HeaderEditCancel()
        {
            base.HeaderEditCancel();
            HeaderEdit = Page.PageName.ToString();
        }

        #endregion

        private void setItems()
        {
            Header = Page.PageName.ToString();
        }

        public void DeleteConditional(bool verify)
        {
            if (runtimeService.DisconnectDialog(this))
            {
                bool needsVerification = false;
                PageEditor pageEditorForThis = null;
                foreach (var d in layoutManager.Value.Documents)
                {
                    var pageEditor = d as PageEditor;
                    if (pageEditor != null)
                    {
                        if (pageEditor.PageItemParent == this)
                        {
                            pageEditorForThis = pageEditor;
                            if (pageEditor.EditorRoot.WorkingCopy.NodeInstructionGroupChildren.Count > 0)
                            {
                                needsVerification = true && verify;
                                break;
                            }
                        }
                    }
                }
                if (Page.NodeInstructionGroupChildren.Count > 0)
                {
                    needsVerification = true && verify;
                }

                if (needsVerification)
                {
                    if (messagingService.Value.ShowDialog(Resources.Strings.Solution_Pad_PageItem_DeleteConfirmation,
                        Resources.Strings.Solution_Pad_PageItem_DeleteTitle, System.Windows.Forms.MessageBoxButtons.OKCancel)
                            == System.Windows.Forms.DialogResult.Cancel)
                    {
                        return;
                    }
                }
                if (pageEditorForThis != null)
                {
                    layoutManager.Value.CloseDocument(pageEditorForThis);
                }
                FireDeletedEvent(this, Node);
            }
        }

        public void Delete()
        {
            DeleteConditional(true);
        }

        public override void KeyDown(object sender, KeyEventArgs e)
        {
            base.KeyDown(sender, e);
            switch (e.Key)
            {
                case Key.Delete:
                    Delete();
                    e.Handled = true;
                    break;
            }
        }

        protected override bool QueryCanDrag()
        {
            return true;
        }

        protected override bool QueryCanDrop(ISolutionItem source)
        {
            bool canDrop = (source is PageItem);
            if (canDrop)
            {
                IsSelected = true; // gives some nice visual feedback to the user
            }
            return canDrop;
        }

        protected override void OnDrop(ISolutionItem source)
        {
            base.OnDrop(source);

            var sourcePage = source as PageItem;
            if (sourcePage != null)
            {
                FireInsertAfterEvent(sourcePage);
            }
        }

        public override void Open()
        {
            base.Open();
            layoutManager.Value.ShowDocument(
                pageEditor.Value, Page.ID.ToString());
        }

    }
}
