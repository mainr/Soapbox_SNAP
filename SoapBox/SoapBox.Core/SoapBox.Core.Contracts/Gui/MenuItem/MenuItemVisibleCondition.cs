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
using SoapBox.Utilities;

namespace SoapBox.Core
{
    /// <summary>
    /// Controls the visibility of a MenuItem based on whether or
    /// not there are any items in its submenu.
    /// </summary>
    public sealed class MenuItemVisibleCondition : AbstractCondition 
    {
        public MenuItemVisibleCondition(IMenuItem menuItem)
        {
            m_menuItem = menuItem;
            SetCondition();

            // Register a callback for when the menuItem Items 
            // property changes
            menuItem.PropertyChanged += 
                new System.ComponentModel.PropertyChangedEventHandler(
                    menuItem_PropertyChanged);
        }

        void menuItem_PropertyChanged(object sender, 
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == m_ItemsName)
            {
                SetCondition();
            }
        }
        static readonly string m_ItemsName =
            NotifyPropertyChangedHelper.GetPropertyName<IMenuItem>(o => o.Items);

        IMenuItem m_menuItem = null;

        private void SetCondition()
        {
            bool visible = false;
            if (m_menuItem != null)
            {
                if (m_menuItem.Items == null)
                {
                    // not supposed to have a submenu
                    visible = true;
                }
                else
                {
                    // it's supposed to have a submenu, see if any exist
                    foreach (IMenuItem item in m_menuItem.Items)
                    {
                        visible = true;
                        break;
                    }
                }
            }
            Condition = visible;
        }
    }
}
