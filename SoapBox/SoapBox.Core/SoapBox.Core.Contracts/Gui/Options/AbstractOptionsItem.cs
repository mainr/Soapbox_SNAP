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
using System.ComponentModel;
using System.Collections.ObjectModel;
using SoapBox.Utilities;

namespace SoapBox.Core
{
    public abstract class AbstractOptionsItem
        : AbstractControl, IOptionsItem
    {

        public AbstractOptionsItem()
            : base()
        {
        }

        #region " IOptionsItem Implementation "

        #region " Header "
        /// <summary>
        /// This is the text displayed in the options tree view for this item.
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
            NotifyPropertyChangedHelper.CreateArgs<AbstractOptionsItem>(o => o.Header);
        #endregion

        #region " Items "
        /// <summary>
        /// If the item being defined has a subtree, then replace this
        /// with a collection of the option items in the subtree.
        /// </summary>
        public IEnumerable<IOptionsItem> Items
        {
            get
            {
                return m_Items;
            }
            protected set
            {
                if (m_Items != value)
                {
                    if (m_Items != null)
                    {
                        foreach (IOptionsItem item in m_Items)
                        {
                            item.OptionChanged -= Items_OptionChanged;
                        }
                    }
                    m_Items = value;
                    if (m_Items != null)
                    {
                        foreach (IOptionsItem item in m_Items)
                        {
                            item.OptionChanged += new EventHandler(Items_OptionChanged);
                        }
                    }
                    NotifyPropertyChanged(m_ItemsArgs);
                }
            }
        }
        private IEnumerable<IOptionsItem> m_Items = new Collection<IOptionsItem>();
        static readonly PropertyChangedEventArgs m_ItemsArgs =
            NotifyPropertyChangedHelper.CreateArgs<AbstractOptionsItem>(o => o.Items);

        void Items_OptionChanged(object sender, EventArgs e)
        {
            NotifyOptionChanged();
        }

        #endregion

        #region " Pad "
        /// <summary>
        /// This is the pad displayed in the options dialog's
        /// content control.
        /// </summary>
        public IOptionsPad Pad
        {
            get
            {
                return m_Pad;
            }
            protected set
            {
                if (m_Pad != value)
                {
                    if (m_Pad != null)
                    {
                        m_Pad.OptionChanged -= m_Pad_OptionChanged;
                    }
                    m_Pad = value;
                    if (m_Pad != null)
                    {
                        m_Pad.OptionChanged += new EventHandler(m_Pad_OptionChanged);
                    }
                    NotifyPropertyChanged(m_PadArgs);
                }
            }
        }
        private IOptionsPad m_Pad = null;
        static readonly PropertyChangedEventArgs m_PadArgs =
            NotifyPropertyChangedHelper.CreateArgs<AbstractOptionsItem>(o => o.Pad);

        void m_Pad_OptionChanged(object sender, EventArgs e)
        {
            NotifyOptionChanged();
        }

        #endregion

        public event EventHandler OptionChanged;

        protected void NotifyOptionChanged()
        {
            var evt = OptionChanged;
            if (evt != null)
            {
                evt(this, new EventArgs());
            }
        }

        #endregion

        /// <summary>
        /// If overriding this, you should still call base.CommitChanges()
        /// </summary>
        public virtual void CommitChanges() 
        {
            if (Pad != null)
            {
                Pad.Commit();
            }
            foreach (IOptionsItem item in Items)
            {
                item.CommitChanges();
            }
        }


        /// <summary>
        /// If overriding this, you should still call base.CancelChanges()
        /// </summary>
        public virtual void CancelChanges()
        {
            if (Pad != null)
            {
                Pad.Cancel();
            }
            foreach (IOptionsItem item in Items)
            {
                item.CancelChanges();
            }
        }

    }
}

