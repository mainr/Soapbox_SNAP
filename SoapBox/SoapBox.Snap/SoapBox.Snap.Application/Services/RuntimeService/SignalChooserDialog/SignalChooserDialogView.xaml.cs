#region "SoapBox.Snap License"
/// <header module="SoapBox.Snap"> 
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
using System.Diagnostics;

namespace SoapBox.Snap.Application
{
    public partial class SignalChooserDialogView : Window
    {
        public SignalChooserDialogView()
        {
            InitializeComponent();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void txtLiteral_GotFocus(object sender, RoutedEventArgs e)
        {
            var txt = sender as TextBox;
            if (txt != null)
            {
                var dc = txt.DataContext as SignalChooserDialog;
                if (dc != null)
                {
                    dc.LiteralSelected = true;
                }
            }
        }

        private void ccSignalTree_GotFocus(object sender, RoutedEventArgs e)
        {
            var cc = sender as ContentControl;
            if (cc != null)
            {
                var dc = cc.DataContext as SignalChooserDialog;
                if (dc != null)
                {
                    dc.SignalSelected = true;
                }
            }
        }
    }
}
