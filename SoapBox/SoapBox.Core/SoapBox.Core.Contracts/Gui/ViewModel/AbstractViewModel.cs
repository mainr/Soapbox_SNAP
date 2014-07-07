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
using System.ComponentModel.Composition;
using SoapBox.Utilities;

namespace SoapBox.Core
{
    [Export(SoapBox.Core.ExtensionPoints.Host.Void, typeof(Object))] // us to load so we get MEF to run
    public class AbstractViewModel : IViewModel
    {
        #region " logger singleton "
        /// <summary>
        /// Anyone who inherits from AbstractViewModel gets a free
        /// reference to the logging service.
        /// </summary>
        [Import(Services.Logging.LoggingService, typeof(ILoggingService))]
        protected ILoggingService logger
        {
            get
            {
                return m_logger;
            }
            set
            {
                m_logger = value;
            }
        }
        private static ILoggingService m_logger = null;
        public static string m_logger_Name = NotifyPropertyChangedHelper.GetPropertyName<AbstractViewModel>(o => o.logger);
        #endregion

        #region " Implement INotifyPropertyChanged "
        /// <summary>
        /// Call this method to raise the PropertyChanged event when
        /// a property changes.  Note that you should use the
        /// NotifyPropertyChangedHelper class to create a cached
        /// copy of the PropertyChangedEventArgs object to pass
        /// into this method.  Usage:
        /// 
        /// static readonly PropertyChangedEventArgs m_$PropertyName$Args = 
        ///     NotifyPropertyChangedHelper.CreateArgs<$ClassName$>(o => o.$PropertyName$);
        /// 
        /// In your property setter:
        ///     PropertyChanged(this, m_$PropertyName$Args)
        /// 
        /// </summary>
        /// <param name="e">A cached event args object</param>
        protected void NotifyPropertyChanged(PropertyChangedEventArgs e)
        {
            var evt = PropertyChanged;
            if (evt != null)
            {
                evt(this, e);
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
