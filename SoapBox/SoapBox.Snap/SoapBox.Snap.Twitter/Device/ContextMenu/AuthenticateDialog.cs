#region "SoapBox.Snap License"
/// <header module="SoapBox.Snap"> 
/// Copyright (C) 2009 SoapBox Automation Inc., All Rights Reserved.
/// Contact: SoapBox Automation Licencing (license@soapboxautomation.com)
/// 
/// This file is part of SoapBox Snap.
/// 
/// SoapBox Snap is free software: you can redistribute it and/or modify it
/// under the terms of the GNU General Public License as published by the 
/// Free Software Foundation, either version 3 of the License, or 
/// (at your option) any later version.
/// 
/// SoapBox Snap is distributed in the hope that it will be useful, but 
/// WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
/// GNU General Public License for more details.
/// 
/// You should have received a copy of the GNU General Public License along
/// with SoapBox Snap. If not, see <http://www.gnu.org/licenses/>.
/// </header>
#endregion
        
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SoapBox.Core;
using System.ComponentModel.Composition;
using SoapBox.Protocol.Base;
using System.ComponentModel;
using SoapBox.Utilities;
using TweetSharp.Twitter.Fluent;
using TweetSharp.Twitter.Extensions;
using System.Security;
using System.Windows;

namespace SoapBox.Snap.Twitter
{
    [Export(CompositionPoints.Dialogs.AuthenticateDialog, typeof(AuthenticateDialog))]
    class AuthenticateDialog : AbstractViewModel
    {
        public AuthenticateDialog()
        {
        }

        [Import(SoapBox.Core.Services.Messaging.MessagingService)]
        private IMessagingService messagingService { get; set; }

        [Import(SoapBox.Core.CompositionPoints.Host.MainWindow)]
        private Lazy<Window> mainWindowExport { get; set; }

        /// <summary>
        /// Returns the new Address field for the NodeDevice
        /// </summary>
        public FieldString ShowDialog()
        {
            Window dlg = new AuthenticateDialogView();
            dlg.Owner = mainWindowExport.Value;
            dlg.DataContext = this;
            dlg.ShowDialog();

            FieldString retVal = null;

            if (Username != string.Empty)
            {
                var twitter = FluentTwitter.CreateRequest()
                    .Configuration.UseHttps()
                    .Authentication.GetClientAuthAccessToken(TwitterConsumer.ConsumerKey,
                                                            TwitterConsumer.ConsumerSecret,
                                                            Username,
                                                            Password.ConvertToUnsecureString());

                var response = twitter.Request();
                try
                {
                    var token = response.AsToken();
                    // the token and token secret seem to be base-64 encoded already, but there's no guarantee, so let's encode them again
                    var item1 = FieldBase64.Encode(token.Token).ToString();
                    var item2 = FieldBase64.Encode(token.TokenSecret).ToString();
                    retVal = new FieldString(item1 + AbstractTwitterDevice.ADDRESS_SEPARATOR + item2);
                    messagingService.ShowMessage(Resources.Strings.ContextMenu_Authenticate_Success, Resources.Strings.ContextMenu_Authenticate_Success_Title);
                }
                catch (Exception ex)
                {
                    logger.Error("Error authenticating Twitter", ex);
                    messagingService.ShowMessage(Resources.Strings.ContextMenu_Authenticate_Fail, Resources.Strings.ContextMenu_Authenticate_Fail_Title);
                }
            }

            return retVal;
        }

        #region " Username "
        public string Username
        {
            get
            {
                return m_Username;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(m_UsernameName);
                }
                if (m_Username != value)
                {
                    m_Username = value;
                    NotifyPropertyChanged(m_UsernameArgs);
                }
            }
        }
        private string m_Username = string.Empty;
        private static readonly PropertyChangedEventArgs m_UsernameArgs =
            NotifyPropertyChangedHelper.CreateArgs<AuthenticateDialog>(o => o.Username);
        private static string m_UsernameName =
            NotifyPropertyChangedHelper.GetPropertyName<AuthenticateDialog>(o => o.Username);
        #endregion

        #region " Password "
        public SecureString Password
        {
            get
            {
                return m_Password;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(m_PasswordName);
                }
                if (m_Password != value)
                {
                    m_Password = value;
                    NotifyPropertyChanged(m_PasswordArgs);
                }
            }
        }
        private SecureString m_Password = new SecureString();
        private static readonly PropertyChangedEventArgs m_PasswordArgs =
            NotifyPropertyChangedHelper.CreateArgs<AuthenticateDialog>(o => o.Password);
        private static string m_PasswordName =
            NotifyPropertyChangedHelper.GetPropertyName<AuthenticateDialog>(o => o.Password);
        #endregion

    }
}
