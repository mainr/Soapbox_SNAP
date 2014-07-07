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
using SoapBox.Utilities;

namespace SoapBox.Core
{
    public abstract class AbstractProgressBar : AbstractControl, IProgressBar
    {
        #region " Minimum "
        /// <summary>
        /// This is the Minimum value of the progress bar - default is zero
        /// </summary>
        public double Minimum
        {
            get
            {
                return m_Minimum;
            }
            protected set
            {
                if (m_Minimum != value)
                {
                    m_Minimum = value;
                    NotifyPropertyChanged(m_MinimumArgs);
                }
            }
        }
        private double m_Minimum = 0;
        static readonly PropertyChangedEventArgs m_MinimumArgs =
            NotifyPropertyChangedHelper.CreateArgs<AbstractProgressBar>(o => o.Minimum);
        #endregion

        #region " Maximum "
        /// <summary>
        /// This is the Maximum value of the progress bar - default is 100
        /// </summary>
        public double Maximum
        {
            get
            {
                return m_Maximum;
            }
            protected set
            {
                if (m_Maximum != value)
                {
                    m_Maximum = value;
                    NotifyPropertyChanged(m_MaximumArgs);
                }
            }
        }
        private double m_Maximum = 100;
        static readonly PropertyChangedEventArgs m_MaximumArgs =
            NotifyPropertyChangedHelper.CreateArgs<AbstractProgressBar>(o => o.Maximum);
        #endregion

        #region " Value "
        /// <summary>
        /// This is the Value value of the progress bar - default is zero
        /// </summary>
        public double Value
        {
            get
            {
                return m_Value;
            }
            protected set
            {
                if (m_Value != value)
                {
                    m_Value = value;
                    NotifyPropertyChanged(m_ValueArgs);
                }
            }
        }
        private double m_Value = 0;
        static readonly PropertyChangedEventArgs m_ValueArgs =
            NotifyPropertyChangedHelper.CreateArgs<AbstractProgressBar>(o => o.Value);
        #endregion

    }
}
