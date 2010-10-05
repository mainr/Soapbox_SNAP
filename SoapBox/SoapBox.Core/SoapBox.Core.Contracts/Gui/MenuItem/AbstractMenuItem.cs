#region "SoapBox.Core License"
/// <header module="SoapBox.Core"> 
/// Copyright (C) 2009 SoapBox Automation Inc., All Rights Reserved.
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
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Media;
using SoapBox.Utilities;

namespace SoapBox.Core
{
    public abstract class AbstractMenuItem
        : AbstractCommandControl, IMenuItem
    {

        public AbstractMenuItem()
            : base()
        {
            VisibleCondition = new MenuItemVisibleCondition(this);

            CanExecuteChanged += delegate
            {
                if (IconFull != null)
                {
                    NotifyPropertyChanged(m_IconArgs);
                }
            };
        }

        #region " IMenuItem Implementation "

        #region " Header "
        /// <summary>
        /// This is the text displayed in the menu item itself.
        /// Use the _ (underscore) notation to specify shortcut keys.
        /// For instance: "_File".
        /// Best to set this property in the derived class's constructor.
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
                    throw new ArgumentNullException();
                }
                if (m_Header != value)
                {
                    m_Header = value;
                    NotifyPropertyChanged(m_HeaderArgs);
                }
            }
        }
        private string m_Header = string.Empty;
        static readonly PropertyChangedEventArgs m_HeaderArgs =
            NotifyPropertyChangedHelper.CreateArgs<AbstractMenuItem>(o => o.Header);
        #endregion

        #region " Items "
        /// <summary>
        /// If the menu item being defined has a submenu, then replace this
        /// with a collection of the menu items in the submenu.
        /// </summary>
        public IEnumerable<IMenuItem> Items
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
        private IEnumerable<IMenuItem> m_Items = null;
        static readonly PropertyChangedEventArgs m_ItemsArgs =
            NotifyPropertyChangedHelper.CreateArgs<AbstractMenuItem>(o => o.Items);
        #endregion

        #region " Icon "
        /// <summary>
        /// Optional icon that can be displayed in the button.
        /// </summary>
        public object Icon
        {
            get
            {
                if (IconFull != null)
                {
                    System.Windows.Controls.Image img = new System.Windows.Controls.Image();
                    if (EnableCondition.Condition)
                    {
                        img.Source = IconFull;
                    }
                    else
                    {
                        img.Source = IconGray;
                    }
                    return img;
                }
                else
                {
                    return null;
                }
            }
        }
        private BitmapSource IconFull
        {
            get
            {
                return m_IconFull;
            }
            set
            {
                if (m_IconFull != value)
                {
                    m_IconFull = value;
                    if (m_IconFull != null)
                    {
                        IconGray = ConvertFullToGray(m_IconFull);
                    }
                    else
                    {
                        IconGray = null;
                    }
                    NotifyPropertyChanged(m_IconArgs);
                }
            }
        }
        private BitmapSource m_IconFull = null;
        static readonly PropertyChangedEventArgs m_IconArgs =
            NotifyPropertyChangedHelper.CreateArgs<AbstractMenuItem>(o => o.Icon);

        private BitmapSource IconGray { get; set; }

        private BitmapSource ConvertFullToGray(BitmapSource full)
        {
            FormatConvertedBitmap gray = new FormatConvertedBitmap();

            gray.BeginInit();
            gray.Source = full;
            gray.DestinationFormat = PixelFormats.Gray32Float;
            gray.EndInit();

            return gray;
        }

        /// <summary>
        /// This is a helper function so you can assign the Icon directly
        /// from a Bitmap, such as one from a resources file.
        /// </summary>
        /// <param name="value"></param>
        protected void SetIconFromBitmap(System.Drawing.Bitmap value)
        {
            BitmapSource b = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                value.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            IconFull = b;
        }

        #endregion

        #region " IsCheckable "
        /// <summary>
        /// By default this is false.  Set it to true in the constructor of the
        /// derived class to turn this menu item into a checkable item.
        /// Then override OnIsCheckedChanged and check the IsChecked property
        /// in the derived class to react when the user checks or unchecks it.
        /// </summary>
        public bool IsCheckable
        {
            get
            {
                return m_IsCheckable;
            }
            protected set
            {
                if (m_IsCheckable != value)
                {
                    m_IsCheckable = value;
                    NotifyPropertyChanged(m_IsCheckableArgs);
                }
            }
        }
        private bool m_IsCheckable = false;
        static readonly PropertyChangedEventArgs m_IsCheckableArgs =
            NotifyPropertyChangedHelper.CreateArgs<AbstractMenuItem>(o => o.IsCheckable);
        #endregion

        #region " IsChecked "
        /// <summary>
        /// True if the item is checked, false otherwise.
        /// Calls OnIsCheckedChanged(), which can be overridden in the
        /// derived class to take an action when the status
        /// is toggled.
        /// </summary>
        public bool IsChecked
        {
            get
            {
                return m_IsChecked;
            }
            set
            {
                if (m_IsChecked != value)
                {
                    m_IsChecked = value;
                    NotifyPropertyChanged(m_IsCheckedArgs);
                    OnIsCheckedChanged();
                }
            }
        }
        private bool m_IsChecked = false;
        static readonly PropertyChangedEventArgs m_IsCheckedArgs =
            NotifyPropertyChangedHelper.CreateArgs<AbstractMenuItem>(o => o.IsChecked);

        /// <summary>
        /// This method is called only if IsCheckable is true and
        /// the user changes the IsChecked property. Override it in 
        /// the derived class to take an action when it changes.
        /// </summary>
        protected virtual void OnIsCheckedChanged() { }

        #endregion

        #region " IsSeparator "
        /// <summary>
        /// Defaults to false. Set to true in the constructor to make this a 
        /// separator instead of a menu item.
        /// </summary>
        public bool IsSeparator
        {
            get
            {
                return m_IsSeparator;
            }
            protected set
            {
                if (m_IsSeparator != value)
                {
                    m_IsSeparator = value;
                    NotifyPropertyChanged(m_IsSeparatorArgs);
                }
            }
        }
        private bool m_IsSeparator = false;
        static readonly PropertyChangedEventArgs m_IsSeparatorArgs =
            NotifyPropertyChangedHelper.CreateArgs<AbstractMenuItem>(o => o.IsSeparator);

        #endregion

        #region " Context "
        /// <summary>
        /// In a context menu, this can be set to the ViewModel of the item
        /// that was clicked on to open the ContextMenu.  Needs some hooking
        /// up in the xaml though (hint: handle the ContextMenuOpening event 
        /// on the control that the ContextMenu is attached to).
        /// Note that setting this property will set the Context property
        /// on all child menu items (following the Items collection property).
        /// </summary>
        public object Context
        {
            get
            {
                return m_Context;
            }
            set
            {
                if (m_Context != value)
                {
                    m_Context = value;
                    if (Items != null)
                    {
                        foreach (IMenuItem item in Items)
                        {
                            item.Context = m_Context;
                        }
                    }
                    NotifyPropertyChanged(m_ContextArgs);
                }
            }
        }
        private object m_Context = null;
        static readonly PropertyChangedEventArgs m_ContextArgs =
            NotifyPropertyChangedHelper.CreateArgs<AbstractMenuItem>(o => o.Context);
        #endregion

        #region " IsSubmenuOpen "
        public bool IsSubmenuOpen
        {
            get
            {
                return m_IsSubmenuOpen;
            }
            set
            {
                if (m_IsSubmenuOpen != value)
                {
                    m_IsSubmenuOpen = value;
                    NotifyPropertyChanged(m_IsSubmenuOpenArgs);
                    OnIsSubmenuOpenChanged();
                }
            }
        }
        private bool m_IsSubmenuOpen = false;
        private static readonly PropertyChangedEventArgs m_IsSubmenuOpenArgs =
            NotifyPropertyChangedHelper.CreateArgs<AbstractMenuItem>(o => o.IsSubmenuOpen);
        private static string m_IsSubmenuOpenName =
            NotifyPropertyChangedHelper.GetPropertyName<AbstractMenuItem>(o => o.IsSubmenuOpen);

        /// <summary>
        /// This method is called only if 
        /// the user changes the IsSubmenuOpen property. Override it in 
        /// the derived class to take an action when it changes.
        /// </summary>
        protected virtual void OnIsSubmenuOpenChanged() { }

        #endregion

        #endregion

    }
}
