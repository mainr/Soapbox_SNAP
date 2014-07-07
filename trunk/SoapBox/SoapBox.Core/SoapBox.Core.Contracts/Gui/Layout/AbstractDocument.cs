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
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Media;
using SoapBox.Utilities;

namespace SoapBox.Core
{
    public abstract class AbstractDocument : AbstractLayoutItem, IDocument
    {
        /// <summary>
        /// Override this in the derived class to take an action
        /// when the document is opened.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void OnOpened(object sender, EventArgs e) { }

        /// <summary>
        /// Override this in the derived class to take an action
        /// before the document is closed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void OnClosing(object sender, CancelEventArgs e) { }

        /// <summary>
        /// Override this in the derived class to take an action
        /// after the document is closed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void OnClosed(object sender, EventArgs e) { }

        #region " Memento "
        /// <summary>
        /// Used to remember, for instance, the name of the file being edited by this doc.
        /// </summary>
        public string Memento
        {
            get
            {
                return m_Memento;
            }
            protected set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }
                if (m_Memento != value)
                {
                    m_Memento = value;
                    NotifyPropertyChanged(m_MementoArgs);
                }
            }
        }
        private string m_Memento = string.Empty;
        static readonly PropertyChangedEventArgs m_MementoArgs =
            NotifyPropertyChangedHelper.CreateArgs<AbstractDocument>(o => o.Memento);
        static readonly string m_MementoName =
            NotifyPropertyChangedHelper.GetPropertyName<AbstractDocument>(o => o.Memento);

        #endregion


        /// <summary>
        /// This is the factory method.  By default is just returns
        /// the existing instance, but it can be overridden to 
        /// return a new instance based on the memento.
        /// </summary>
        public virtual IDocument CreateDocument(string memento)
        {
            if (Memento != string.Empty && memento != Memento)
            {
                throw new ArgumentException(
                    "Can't create more than one document of this type.", 
                    m_MementoName);
            }
            Memento = memento;
            return this;
        }
    }
}
