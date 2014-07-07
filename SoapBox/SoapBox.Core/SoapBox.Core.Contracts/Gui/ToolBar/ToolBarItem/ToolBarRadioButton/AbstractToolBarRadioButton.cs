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
    public abstract class AbstractToolBarRadioButton : AbstractRadioButton, IToolBarItem
    {

        public AbstractToolBarRadioButton()
        {
            this.PropertyChanged += OnPropertyChanged;
        }

        #region " ToolBarItems "
        /// <summary>
        /// NOT UNIT TESTED
        /// 
        /// This set-only property and the OnPropertyChanged event handler
        /// below only exist to help fix a bug in WPF.  The problem is that
        /// if two or more radio buttons from the same group are split
        /// in a toolbar with some in the toolbar and some in the overflow
        /// box, then the user could select more than one item.
        /// The derived class should set this property to the list of 
        /// all toolbar items in the same toolbar.  It then filters out
        /// any peer radio buttons in the same group, and makes sure they
        /// are always set to IsChecked = false when this button is checked.
        /// More Info: http://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=322929
        /// </summary>
        protected IEnumerable<IToolBarItem> ToolBarItems
        {
            set
            {
                if (value == null)
                {
                    m_peerRadioButtons = null;
                }
                else
                {
                    List<AbstractToolBarRadioButton> radioButtons = 
                        new List<AbstractToolBarRadioButton>();
                    foreach (IToolBarItem tb in value)
                    {
                        AbstractToolBarRadioButton rb = tb as AbstractToolBarRadioButton;
                        if (rb != null)
                        {
                            if (rb != this && rb.GroupName == GroupName)
                            {
                                radioButtons.Add(rb);
                            }
                        }
                    }
                    m_peerRadioButtons = radioButtons;
                    CheckOtherRadioButtons();
                }
            }
        }

        private List<AbstractToolBarRadioButton> m_peerRadioButtons = null;

        private void CheckOtherRadioButtons()
        {
            if (IsChecked)
            {
                if (m_peerRadioButtons != null)
                {
                    foreach (AbstractToolBarRadioButton rb in m_peerRadioButtons)
                    {
                        rb.IsChecked = false;
                    }
                }
            }
        }

        #endregion

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == m_IsCheckedName)
            {
                CheckOtherRadioButtons();
            }
        }
        static readonly string m_IsCheckedName =
            NotifyPropertyChangedHelper.GetPropertyName<AbstractRadioButton>(o => o.IsChecked);
    }
}
