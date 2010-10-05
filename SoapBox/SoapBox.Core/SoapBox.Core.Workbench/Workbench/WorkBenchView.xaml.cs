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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel.Composition;

namespace SoapBox.Core.Workbench
{
    /// <summary>
    /// Interaction logic for WorkBenchView.xaml
    /// </summary>
    [Export(CompositionPoints.Host.MainWindow, typeof(Window))]
    public partial class WorkBenchView : Window
    {
        [ImportingConstructor]
        public WorkBenchView([Import(CompositionPoints.Workbench.ViewModel)] Workbench vm)
        {
            InitializeComponent();

            DataContext = vm;

            // ToolBarTray.ToolBars isn't a dependency property, so we
            // have to add the Tool Bars manually
            foreach (IToolBar toolBarViewModel in vm.ToolBars)
            {
                ToolBar toolBar = new ToolBar();
                toolBar.DataContext = toolBarViewModel;

                // Bind the Header Property
                Binding headerBinding = new Binding("Header");
                toolBar.SetBinding(ToolBar.HeaderProperty, headerBinding);

                // Bind the Items Property
                Binding itemsBinding = new Binding("Items");
                toolBar.SetBinding(ToolBar.ItemsSourceProperty, itemsBinding);

                // Bind the Visible Property
                Binding visibleBinding = new Binding("Visible");
                visibleBinding.Converter = new BooleanToVisibilityConverter();
                toolBar.SetBinding(ToolBar.VisibilityProperty, visibleBinding);

                // Bind the ToolTip Property
                Binding toolTipBinding = new Binding("ToolTip");
                toolBar.SetBinding(ToolBar.ToolTipProperty, toolTipBinding);

                tbtToolBar.ToolBars.Add(toolBar);
            }

            // hook up the event handlers so the viewmodel knows when we're closing
            this.Closing += vm.OnClosing;
            this.Closed += vm.OnClosed;
        }


    }

}
