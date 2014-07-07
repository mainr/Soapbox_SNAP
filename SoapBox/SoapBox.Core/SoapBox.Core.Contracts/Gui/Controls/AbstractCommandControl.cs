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
using System.Windows.Input;
using System.ComponentModel;
using SoapBox.Utilities;

namespace SoapBox.Core
{
    public abstract class AbstractCommandControl : AbstractControl, ICommandControl
    {

        #region " ICommand Implementation "

        public event EventHandler CanExecuteChanged = delegate { };

        public bool CanExecute(object parameter)
        {
            return EnableCondition.Condition;
        }

        public void Execute(object parameter)
        {
            Run();
        }

        #endregion

        /// <summary>
        /// This method is called when the command is executed.
        /// Override this in the derived class to actually do something.
        /// </summary>
        protected virtual void Run() { }

        #region " EnableCondition "
        /// <summary>
        /// Defaults to AlwaysTrueCondition.
        /// Set this to any ISoapBoxCondition object, and it will control
        /// the CanExecute property from the ICommand interface, and 
        /// will raise the CanExecuteChanged event when appropriate.
        /// </summary>
        public ICondition EnableCondition
        {
            get
            {
                if (m_EnableCondition == null)
                {
                    // Lazy initialize this property.
                    // We could do this in the constructor, but 
                    // I like having it all contained in one
                    // section of code.
                    EnableCondition = new AlwaysTrueCondition();
                }
                return m_EnableCondition;
            }
            protected set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(m_EnableConditionName);
                }
                if (m_EnableCondition != value)
                {
                    if (m_EnableCondition != null)
                    {
                        //remove the old event handler
                        m_EnableCondition.ConditionChanged -= OnEnableConditionChanged;
                    }
                    m_EnableCondition = value;
                    //add the new event handler
                    m_EnableCondition.ConditionChanged += OnEnableConditionChanged;
                    CanExecuteChanged(this, new EventArgs());

                    NotifyPropertyChanged(m_EnableConditionArgs);
                }
            }
        }
        private ICondition m_EnableCondition = null;
        static readonly PropertyChangedEventArgs m_EnableConditionArgs =
            NotifyPropertyChangedHelper.CreateArgs<AbstractCommandControl>(o => o.EnableCondition);
        static readonly string m_EnableConditionName =
            NotifyPropertyChangedHelper.GetPropertyName<AbstractCommandControl>(o => o.EnableCondition);
        private void OnEnableConditionChanged(object sender, EventArgs e)
        {
            CanExecuteChanged(sender, e);
        }
        #endregion


    }
}
