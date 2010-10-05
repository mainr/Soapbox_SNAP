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

namespace SoapBox.Snap.Twitter
{
    public partial class AuthenticateDialogView : Window
    {
        public AuthenticateDialogView()
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

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var box = sender as PasswordBox;
            if (box != null)
            {
                var viewModel = box.DataContext as AuthenticateDialog;
                if (viewModel != null)
                {
                    viewModel.Password = box.SecurePassword;
                }
            }
        }

     }
}
