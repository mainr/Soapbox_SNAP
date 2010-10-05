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
using System.ComponentModel;
using SoapBox.Utilities;

namespace SoapBox.Core
{
    public abstract class AbstractExtension : AbstractViewModel, IExtension
    {

        #region " ID "
        /// <summary>
        /// This is a unique string used to identify the extension.
        /// The point of this is so that other extensions can insert themselves
        /// before or after this item in the list of extensions.  Example: "File"
        /// Should *always* be set in the derived class's constructor.
        /// </summary>
        public string ID
        {
            get
            {
                return m_ID;
            }
            protected set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(m_IDName);
                }
                if (value == string.Empty)
                {
                    throw new ArgumentException(m_IDName);
                }
                if (m_ID != value)
                {
                    m_ID = value;
                    NotifyPropertyChanged(m_IDArgs);
                }
            }
        }
        // Defaults to a Guid so at least it will be unique.
        private string m_ID = Guid.NewGuid().ToString();
        static readonly PropertyChangedEventArgs m_IDArgs = 
            NotifyPropertyChangedHelper.CreateArgs<AbstractExtension>(o => o.ID);
        static readonly string m_IDName = 
            NotifyPropertyChangedHelper.GetPropertyName<AbstractExtension>(o => o.ID);
        #endregion

        #region " InsertRelativeToID "
        /// <summary>
        /// If specified, the extension list will try to insert this extension 
        /// before or after the extension with this ID.
        /// </summary>
        public string InsertRelativeToID
        {
            get
            {
                return m_InsertRelativeToID;
            }
            protected set
            {
                if (m_InsertRelativeToID != value)
                {
                    m_InsertRelativeToID = value;
                    NotifyPropertyChanged(m_InsertRelativeToIDArgs);
                }
            }
        }
        private string m_InsertRelativeToID = null;
        static readonly PropertyChangedEventArgs m_InsertRelativeToIDArgs =
            NotifyPropertyChangedHelper.CreateArgs<AbstractExtension>(o => o.InsertRelativeToID);
        #endregion

        #region " BeforeOrAfter "
        /// <summary>
        /// If specified, the extension list will try to insert this extension 
        /// before or after the extension with this ID.
        /// </summary>
        public RelativeDirection BeforeOrAfter
        {
            get
            {
                return m_BeforeOrAfter;
            }
            protected set
            {
                if (m_BeforeOrAfter != value)
                {
                    m_BeforeOrAfter = value;
                    NotifyPropertyChanged(m_BeforeOrAfterArgs);
                }
            }
        }
        private RelativeDirection m_BeforeOrAfter = RelativeDirection.Before;
        static readonly PropertyChangedEventArgs m_BeforeOrAfterArgs =
            NotifyPropertyChangedHelper.CreateArgs<AbstractExtension>(o => o.BeforeOrAfter);
        #endregion


    }
}
