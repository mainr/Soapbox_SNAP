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
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using SoapBox.Core;
using System.Windows.Input;
using System.Diagnostics;

namespace SoapBox.Core
{
    [Export(SoapBox.Core.ExtensionPoints.Host.Styles, typeof(ResourceDictionary))]
    public partial class ContextMenuWrapperView : ResourceDictionary
    {
        public ContextMenuWrapperView()
        {
            InitializeComponent();
        }

        private ContextMenuEventArgs m_args = null;

        private void contentPresenter_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (m_args != e)
            {
                m_args = e; // anti-repeat when these context menu wrappers are nested              
                ContentPresenter cp = e.Source as ContentPresenter;
                if (cp != null)
                {

                    IContextMenu contextMenuViewModel =
                        cp.DataContext as IContextMenu;
                    if (contextMenuViewModel != null)
                    {
                        if (contextMenuViewModel.ContextMenuEnabled)
                        {
                            IEnumerable<IMenuItem> items =
                                contextMenuViewModel.ContextMenu as IEnumerable<IMenuItem>;
                            if (items != null)
                            {
                                foreach (IMenuItem item in items)
                                {
                                    // will automatically set all 
                                    // child menu items' context as well
                                    item.Context = contextMenuViewModel;
                                }
                            }
                            else
                            {
                                e.Handled = true;
                            }
                        }
                        else
                        {
                            //e.Handled = true;
                        }
                    }
                    else
                    {
                        e.Handled = true;
                    }
                }
                else
                {
                    e.Handled = true;
                }
            }
        }
    }
}
