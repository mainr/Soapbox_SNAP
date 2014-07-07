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
using SoapBox.Utilities;

namespace SoapBox.Core
{
    public abstract class AbstractToolBar : AbstractControl, IToolBar 
    {
        #region " Header "
        /// <summary>
        /// This is the name of the toolbar. It's displayed in a label
        /// as the first item in the toolbar.
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
                if (m_Header != value)
                {
                    m_Header = value;
                    NotifyPropertyChanged(m_HeaderArgs);
                }
            }
        }
        private string m_Header = null;
        static readonly PropertyChangedEventArgs m_HeaderArgs =
            NotifyPropertyChangedHelper.CreateArgs<AbstractToolBar>(o => o.Header);
        #endregion

        #region " Name "
        /// <summary>
        /// This is the name of the toolbar. It's displayed in a drop
        /// down list when the user selects/deselects visible toolbars.
        /// Best to set this property in the derived class's constructor.
        /// </summary>
        public string Name
        {
            get
            {
                if (m_Name == null)
                {
                    return ID;
                }
                else
                {
                    return m_Name;
                }
            }
            protected set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(m_NameName);
                }
                if (value == string.Empty)
                {
                    throw new ArgumentException(m_NameName);
                }
                if (m_Name != value)
                {
                    m_Name = value;
                    NotifyPropertyChanged(m_NameArgs);
                }
            }
        }
        private string m_Name = null;
        static readonly PropertyChangedEventArgs m_NameArgs =
            NotifyPropertyChangedHelper.CreateArgs<AbstractToolBar>(o => o.Name);
        static readonly string m_NameName =
            NotifyPropertyChangedHelper.GetPropertyName<AbstractToolBar>(o => o.Name);
        #endregion

        #region " Items "
        /// <summary>
        /// Replace this
        /// with a collection of the tool bar items in the toolbar.
        /// </summary>
        public IEnumerable<IToolBarItem> Items
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
        private IEnumerable<IToolBarItem> m_Items = null;
        static readonly PropertyChangedEventArgs m_ItemsArgs =
            NotifyPropertyChangedHelper.CreateArgs<AbstractToolBar>(o => o.Items);
        #endregion

    }
}
