#region "SoapBox.Core License"
/// <header module="SoapBox.Core"> 
/// Copyright (C) 2009 SoapBox Automation, All Rights Reserved.
/// Contact: SoapBox Automation Licencing (license@soapboxautomation.com)
/// 
/// This file is part of SoapBox Core.
/// 
/// Commercial Usage
/// Licensees holding valid SoapBox Automation Commercial licenses may use  
/// this file in accordance with the SoapBox Automation Commercial License
/// Agreement provided with the Software or, alternatively, in accordance 
/// with the terms contained in a written agreement between you and
/// SoapBox Automation Inc.
/// 
/// GNU Lesser General Public License Usage
/// SoapBox Core is free software: you can redistribute it and/or modify 
/// it under the terms of the GNU Lesser General Public License
/// as published by the Free Software Foundation, either version 3 of the
/// License, or (at your option) any later version.
/// 
/// SoapBox Core is distributed in the hope that it will be useful, 
/// but WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
/// GNU Lesser General Public License for more details.
/// 
/// You should have received a copy of the GNU Lesser General Public License 
/// along with SoapBox Core. If not, see <http://www.gnu.org/licenses/>.
/// </header>
#endregion
        
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using SoapBox.Utilities;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Collections.ObjectModel;

namespace SoapBox.Core
{
    /// <summary>
    /// Helper class for implementing a TreeViewItem ViewModel
    /// </summary>
    public abstract class AbstractTreeViewItem : AbstractLabel, ITreeViewItem
    {
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        #region " IsSelected "
        /// <summary>
        /// True if the item is selected, false otherwise.
        /// Calls OnIsSelectedChanged(), which can be overridden in the
        /// derived class to take an action when the status
        /// is toggled.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return m_IsSelected;
            }
            set
            {
                if (m_IsSelected != value)
                {
                    m_IsSelected = value;
                    NotifyPropertyChanged(m_IsSelectedArgs);
                    OnIsSelectedChanged();
                }
            }
        }
        private bool m_IsSelected = false;
        static readonly PropertyChangedEventArgs m_IsSelectedArgs =
            NotifyPropertyChangedHelper.CreateArgs<AbstractTreeViewItem>(o => o.IsSelected);


        /// <summary>
        /// This method is called if
        /// the user changes the IsSelected property. Override it in 
        /// the derived class to take an action when it changes.
        /// </summary>
        protected virtual void OnIsSelectedChanged() { }

        #endregion

        #region " IsExpanded "
        /// <summary>
        /// True if the item is expanded, false otherwise.
        /// </summary>
        public bool IsExpanded
        {
            get
            {
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
            NotifyPropertyChangedHelper.CreateArgs<AbstractTreeViewItem>(o => o.IsExpanded);

        #endregion

        #region " Items "
        public IEnumerable<ITreeViewItem> Items
        {
            get
            {
                return m_Items;
            }
            protected set
            {
                if (m_Items != value)
                {
                    m_Items = value;
                    NotifyPropertyChanged(m_ItemsArgs);
                }
            }
        }
        protected IEnumerable<ITreeViewItem> m_Items = null;
        static readonly PropertyChangedEventArgs m_ItemsArgs =
            NotifyPropertyChangedHelper.CreateArgs<AbstractTreeViewItem>(o => o.Items);
        #endregion

        #region " Icons "
        public ObservableCollection<object> Icons
        {
            get
            {
                return m_Icons;
            }
            protected set
            {
                if (m_Icons != value)
                {
                    m_Icons = value;
                    NotifyPropertyChanged(m_IconsArgs);
                }
            }
        }
        protected ObservableCollection<object> m_Icons = new ObservableCollection<object>();
        static readonly PropertyChangedEventArgs m_IconsArgs =
            NotifyPropertyChangedHelper.CreateArgs<AbstractTreeViewItem>(o => o.Icons);

        /// <summary>
        /// This is a helper function so you can add an Icon directly
        /// from a Bitmap, such as one from a resources file.
        /// Returns a reference to the new Icon in the Icons collection,
        /// so you can remove it later if you want.
        /// </summary>
        /// <param name="value"></param>
        public System.Windows.Controls.Image AddIconFromBitmap(System.Drawing.Bitmap value)
        {
            IntPtr hBitmap = value.GetHbitmap();
            System.Windows.Controls.Image img;
            try
            {
                BitmapSource b = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                img = new System.Windows.Controls.Image();
                img.Source = b;
            }
            finally
            {
                DeleteObject(hBitmap);
            }
            Icons.Add(img);
            return img;
        }
        #endregion


    }
}
