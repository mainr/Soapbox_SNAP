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

namespace SoapBox.Core
{
    public abstract class AbstractOptionsPad : AbstractPad, IOptionsPad
    {
        #region " Commit "

        /// <summary>
        /// If overriding this method, make sure to call base.Commit first.
        /// </summary>
        public virtual void Commit()
        {
            foreach (var commitAction in CommitActions)
            {
                commitAction();
            }
            CommitActions.Clear();
            CancelActions.Clear();
        }

        protected IList<Action> CommitActions
        {
            get
            {
                return m_commitActions;
            }
        }
        private readonly IList<Action> m_commitActions = new List<Action>();

        #endregion

        #region " Cancel "

        /// <summary>
        /// If overriding this method, make sure to call base.Cancel first.
        /// </summary>
        public virtual void Cancel()
        {
            foreach (var cancelAction in CancelActions)
            {
                cancelAction();
            }
            CancelActions.Clear();
            CommitActions.Clear();
        }

        protected IList<Action> CancelActions
        {
            get
            {
                return m_cancelActions;
            }
        }
        private readonly IList<Action> m_cancelActions = new List<Action>();

        #endregion

        public event EventHandler OptionChanged;

        protected void NotifyOptionChanged()
        {
            var evt = OptionChanged;
            if (evt != null)
            {
                evt(this, new EventArgs());
            }
        }
    }
}
