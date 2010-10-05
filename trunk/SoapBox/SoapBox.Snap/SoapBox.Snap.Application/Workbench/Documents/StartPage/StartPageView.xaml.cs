#region "SoapBox.Snap License"
/// <header module="SoapBox.Snap"> 
/// </header>
#endregion
        
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.ComponentModel;
using SoapBox.Snap.MarkdownUtility;
using SoapBox.Core;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace SoapBox.Snap.Application
{
    [Export(SoapBox.Core.ExtensionPoints.Host.Views, typeof(ResourceDictionary))]
    public partial class StartPageView : ResourceDictionary
    {
        public StartPageView()
        {
            InitializeComponent();
        }

        private void Frame_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            var fr = sender as Frame;
            // If we're navigating to the place where we're already at, the user might have already
            // clicked on a link and navigated somewhere else, so we need to do a refresh, but 
            // be careful of that creating an infinite loop
            if (fr != null && fr.CurrentSource == fr.Source)
            {
                // need to refresh
                if (e.NavigationMode != NavigationMode.Refresh)
                {
                    e.Cancel = true;
                    fr.NavigationService.Refresh();
                }
            }
        }
    }
}
