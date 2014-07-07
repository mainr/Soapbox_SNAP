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
    public abstract class AbstractToggleButton : AbstractButton, IToggleButton
    {

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
            NotifyPropertyChangedHelper.CreateArgs<AbstractToggleButton>(o => o.IsChecked);


        /// <summary>
        /// This method is called only if IsCheckable is true and
        /// the user changes the IsChecked property. Override it in 
        /// the derived class to take an action when it changes.
        /// </summary>
        protected virtual void OnIsCheckedChanged() { }

        #endregion

    }
}
