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
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Collections.ObjectModel;
using SoapBox.Utilities;
using SoapBox.Protocol.Base;
using System.Windows.Input;
using System.ComponentModel.Composition;

namespace SoapBox.Snap.Application
{
    /// <summary>
    /// A handy helper class for building a header object to put in the
    /// Solution tree.  Includes header text, an icon, and sub-items.
    /// </summary>
    [Export(SoapBox.Core.ExtensionPoints.Host.Void, typeof(object))]
    public class AbstractSolutionItem: AbstractNodeWrapper, ISolutionItem, IContextMenu
    {
        // Just here for MEF to call
        public AbstractSolutionItem()
            : base(null)
        {
        }

        public AbstractSolutionItem(ISolutionItem parent, string header)
            : base(parent)
        {
            Header = header;
        }

        public AbstractSolutionItem(ISolutionItem parent, string header, System.Drawing.Bitmap icon)
            : base(parent)
        {
            if (icon == null)
            {
                throw new ArgumentNullException(m_IconName);
            }

            Header = header;
            SetIconFromBitmap(icon);
        }

        #region "layoutManagerSingleton"
        [Import(SoapBox.Core.Services.Layout.LayoutManager, typeof(ILayoutManager))]
        protected Lazy<ILayoutManager> layoutManager
        {
            get
            {
                return layoutManagerSingleton;
            }
            set
            {
                layoutManagerSingleton = value;
            }
        }
        private static Lazy<ILayoutManager> layoutManagerSingleton = null;
        #endregion

        #region "extensionServiceSingleton"
        [Import(SoapBox.Core.Services.Host.ExtensionService)]
        protected IExtensionService extensionService
        {
            get
            {
                return extensionServiceSingleton;
            }
            set
            {
                extensionServiceSingleton = value;
            }
        }
        private static IExtensionService extensionServiceSingleton = null;
        #endregion

        #region "messagingServiceSingleton"
        [Import(SoapBox.Core.Services.Messaging.MessagingService)]
        protected Lazy<IMessagingService> messagingService
        {
            get
            {
                return messagingServiceSingleton;
            }
            set
            {
                messagingServiceSingleton = value;
            }
        }
        private static Lazy<IMessagingService> messagingServiceSingleton = null;
        #endregion

        #region "runtimeServiceSingleton"
        [Import(SoapBox.Snap.Services.Solution.RuntimeService)]
        protected IRuntimeService runtimeService
        {
            get
            {
                return runtimeServiceSingleton;
            }
            set
            {
                runtimeServiceSingleton = value;
            }
        }
        private static IRuntimeService runtimeServiceSingleton = null;
        #endregion

        public event InsertAfterHandler InsertAfter = delegate { };

        protected void FireInsertAfterEvent(ISolutionItem newItem)
        {
            if (runtimeService.DisconnectDialog(this))
            {
                InsertAfter(this, newItem);
            }
        }

        #region " MoveUp "
        public event EventHandler MoveUpRequest = delegate { };

        public void MoveUp()
        {
            if (runtimeService.DisconnectDialog(this))
            {
                MoveUpRequest(this, EventArgs.Empty);
            }
        }
        #endregion

        #region " MoveDown "
        public event EventHandler MoveDownRequest = delegate { };

        public void MoveDown()
        {
            if (runtimeService.DisconnectDialog(this))
            {
                MoveDownRequest(this, EventArgs.Empty);
            }
        }
        #endregion

        public virtual void Open() { }

        public virtual void KeyDown(object sender, KeyEventArgs e) { }

        #region " Header "
        /// <summary>
        /// This is the text displayed in the item itself.
        /// </summary>
        public string Header
        {
            get
            {
                return m_Header;
            }
            protected set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(m_HeaderName);
                }
                if (m_Header != value)
                {
                    m_Header = value;
                    NotifyPropertyChanged(m_HeaderArgs);
                }
                HeaderEdit = value;
            }
        }
        private string m_Header = string.Empty;
        static readonly PropertyChangedEventArgs m_HeaderArgs =
            NotifyPropertyChangedHelper.CreateArgs<AbstractSolutionItem>(o => o.Header);
        static readonly string m_HeaderName =
            NotifyPropertyChangedHelper.GetPropertyName<AbstractSolutionItem>(o => o.Header);

        #endregion

        #region " HeaderIsEditable "
        /// <summary>
        /// True if the user can click on the item and edit the text directly.
        /// Default is false.
        /// </summary>
        public bool HeaderIsEditable
        {
            get
            {
                return m_HeaderIsEditable;
            }
            protected set
            {
                if (m_HeaderIsEditable != value)
                {
                    m_HeaderIsEditable = value;
                    NotifyPropertyChanged(m_HeaderIsEditableArgs);
                }
            }
        }
        private bool m_HeaderIsEditable = false;
        static readonly PropertyChangedEventArgs m_HeaderIsEditableArgs =
            NotifyPropertyChangedHelper.CreateArgs<AbstractSolutionItem>(o => o.HeaderIsEditable);
        static readonly string m_HeaderIsEditableName =
            NotifyPropertyChangedHelper.GetPropertyName<AbstractSolutionItem>(o => o.HeaderIsEditable);

        #endregion

        #region " HeaderNotBeingEdited "
        /// <summary>
        /// Just the opposite of HeaderBeingEdited (for binding)
        /// </summary>
        public bool HeaderNotBeingEdited
        {
            get
            {
                return !m_HeaderBeingEdited;
            }
        }
        static readonly PropertyChangedEventArgs m_HeaderNotBeingEditedArgs =
            NotifyPropertyChangedHelper.CreateArgs<AbstractSolutionItem>(o => o.HeaderNotBeingEdited);
        static readonly string m_HeaderNotBeingEditedName =
            NotifyPropertyChangedHelper.GetPropertyName<AbstractSolutionItem>(o => o.HeaderNotBeingEdited);

        #endregion

        #region " HeaderBeingEdited "
        /// <summary>
        /// True if the user can click on the item and edit the text directly.
        /// Default is false.
        /// </summary>
        public bool HeaderBeingEdited
        {
            get
            {
                return m_HeaderBeingEdited;
            }
            set
            {
                if (m_HeaderBeingEdited != value)
                {
                    if (value == false || HeaderIsEditable) // business rule
                    {
                        m_HeaderBeingEdited = value;
                        NotifyPropertyChanged(m_HeaderBeingEditedArgs);
                        NotifyPropertyChanged(m_HeaderNotBeingEditedArgs);
                    }
                }
            }
        }
        private bool m_HeaderBeingEdited = false;
        static readonly PropertyChangedEventArgs m_HeaderBeingEditedArgs =
            NotifyPropertyChangedHelper.CreateArgs<AbstractSolutionItem>(o => o.HeaderBeingEdited);
        static readonly string m_HeaderBeingEditedName =
            NotifyPropertyChangedHelper.GetPropertyName<AbstractSolutionItem>(o => o.HeaderBeingEdited);

        #endregion

        #region " HeaderEdit "
        /// <summary>
        /// The text during the edit operation.
        /// </summary>
        public string HeaderEdit
        {
            get
            {
                return m_HeaderEdit;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(m_HeaderEditName);
                }
                if (m_HeaderEdit != value)
                {
                    m_HeaderEdit = value;
                    NotifyPropertyChanged(m_HeaderEditArgs);
                }
            }
        }
        private string m_HeaderEdit = string.Empty;
        static readonly PropertyChangedEventArgs m_HeaderEditArgs =
            NotifyPropertyChangedHelper.CreateArgs<AbstractSolutionItem>(o => o.HeaderEdit);
        static readonly string m_HeaderEditName =
            NotifyPropertyChangedHelper.GetPropertyName<AbstractSolutionItem>(o => o.HeaderEdit);

        #endregion

        public virtual void HeaderEditAccept()
        {
            HeaderBeingEdited = false;
        }

        public virtual void HeaderEditCancel()
        {
            HeaderBeingEdited = false;
        }

        #region " Icon "
        /// <summary>
        /// Optional icon that can be displayed in the header.
        /// </summary>
        public object Icon
        {
            get
            {
                return m_Icon;
            }
            set
            {
                if (m_Icon != value)
                {
                    m_Icon = value;
                    NotifyPropertyChanged(m_IconArgs);
                }
            }
        }
        private object m_Icon = null;
        static readonly PropertyChangedEventArgs m_IconArgs =
            NotifyPropertyChangedHelper.CreateArgs<AbstractSolutionItem>(o => o.Icon);
        static readonly string m_IconName =
            NotifyPropertyChangedHelper.GetPropertyName<AbstractSolutionItem>(o => o.Icon);

        /// <summary>
        /// This is a helper function so you can assign the Icon directly
        /// from a Bitmap, such as one from a resources file.
        /// </summary>
        /// <param name="value"></param>
        public void SetIconFromBitmap(System.Drawing.Bitmap value)
        {
            try
            {
                BitmapSource b = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    value.GetHbitmap(),
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                System.Windows.Controls.Image img = new System.Windows.Controls.Image();
                img.Source = b;
                Icon = img;
            }
            catch (Exception ex)
            {
                logger.Error("Error setting icon from bitmap", ex);
            }
        }

        #endregion

        #region " Icon2 "
        /// <summary>
        /// Optional Icon2 that can be displayed in the header.
        /// </summary>
        public object Icon2
        {
            get
            {
                return m_Icon2;
            }
            set
            {
                if (m_Icon2 != value)
                {
                    m_Icon2 = value;
                    NotifyPropertyChanged(m_Icon2Args);
                }
            }
        }
        private object m_Icon2 = null;
        static readonly PropertyChangedEventArgs m_Icon2Args =
            NotifyPropertyChangedHelper.CreateArgs<AbstractSolutionItem>(o => o.Icon2);
        static readonly string m_Icon2Name =
            NotifyPropertyChangedHelper.GetPropertyName<AbstractSolutionItem>(o => o.Icon2);

        /// <summary>
        /// This is a helper function so you can assign the Icon2 directly
        /// from a Bitmap, such as one from a resources file.
        /// </summary>
        /// <param name="value"></param>
        public void SetIcon2FromBitmap(System.Drawing.Bitmap value)
        {
            BitmapSource b = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                value.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            System.Windows.Controls.Image img = new System.Windows.Controls.Image();
            img.Source = b;
            Icon2 = img;
        }

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
            NotifyPropertyChangedHelper.CreateArgs<AbstractSolutionItem>(o => o.ContextMenu);

        #endregion

        #region " ContextMenuEnabled "
        /// <summary>
        /// Allows control of whether or not the context menu is enabled.
        /// True by default.
        /// </summary>
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
        static readonly PropertyChangedEventArgs m_ContextMenuEnabledArgs =
            NotifyPropertyChangedHelper.CreateArgs<AbstractSolutionItem>(o => o.ContextMenuEnabled);
        static readonly string m_ContextMenuEnabledName =
            NotifyPropertyChangedHelper.GetPropertyName<AbstractSolutionItem>(o => o.ContextMenuEnabled);

        #endregion

        #region " IsExpanded "

        private bool m_IsExpanded_Edited_Hooked = false;

        public bool IsExpanded
        {
            get
            {
                if(!m_IsExpanded_Edited_Hooked)
                {
                    this.Edited += new EditedHandler(IsExpanded_EditedHandler);
                    m_IsExpanded_Edited_Hooked = true;
                }
                return m_IsExpanded;
            }
            set
            {
                if (m_IsExpanded != value)
                {
                    m_IsExpanded = value;
                    NotifyPropertyChanged(m_IsExpandedArgs);
                }
            }
        }
        private bool m_IsExpanded = false;
        static readonly PropertyChangedEventArgs m_IsExpandedArgs =
           NotifyPropertyChangedHelper.CreateArgs<ISolutionItem>(o => o.IsExpanded);

        void IsExpanded_EditedHandler(INodeWrapper sender, NodeBase oldNode, NodeBase newNode)
        {
            IsExpanded = true;
        }

        #endregion

        #region " Drag & Drop "

        public bool CanDrag()
        {
            if (Node != null)
            {
                return QueryCanDrag();
            }
            else
            {
                return false;
            }
        }

        protected virtual bool QueryCanDrag()
        {
            return false;
        }

        public IDataObject Drag()
        {
            if (Node != null)
            {
                DataObject data = new DataObject(typeof(ISolutionItem), this);
                data.SetData(DataFormats.UnicodeText.ToString(), Node.ToXml()); // useful for debugging - drag/drop to Word
                return data;
            }
            else
            {
                return null;
            }
        }

        public bool CanDrop(IDataObject source)
        {
            if (Node != null && source.GetDataPresent(typeof(ISolutionItem)))
            {
                var sourceItem = (ISolutionItem)source.GetData(typeof(ISolutionItem));
                return QueryCanDrop(sourceItem);
            }
            else
            {
                return false;
            }
        }

        protected virtual bool QueryCanDrop(ISolutionItem source)
        {
            return false;
        }

        public void Drop(IDataObject source)
        {
            if (source.GetDataPresent(typeof(ISolutionItem)))
            {
                var sourceItem = (ISolutionItem)source.GetData(typeof(ISolutionItem));
                OnDrop(sourceItem);
            }
        }

        protected virtual void OnDrop(ISolutionItem source) { }

        #endregion

    }
}
