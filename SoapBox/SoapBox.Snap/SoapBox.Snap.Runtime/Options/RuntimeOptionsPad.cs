#region "SoapBox.Snap License"
/// <header module="SoapBox.Snap"> 
/// Copyright (C) 2009-2015 SoapBox Automation, All Rights Reserved.
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
using System.ComponentModel.Composition;
using SoapBox.Core;
using System.ComponentModel;
using SoapBox.Utilities;

namespace SoapBox.Snap.Runtime
{
    [Export(CompositionPoints.Options.OptionsPad, typeof(RuntimeOptionsPad))]
    class RuntimeOptionsPad : AbstractOptionsPad
    {
        public RuntimeOptionsPad()
        {
            Name = "RuntimeOptionsPad";
        }

        public override void Commit()
        {
            base.Commit();
            Properties.Settings.Default.Save();
        }

        #region "RunAsService"

        public bool RunAsServiceEdit
        {
            get
            {
                return m_RunAsServiceEdit;
            }
            set
            {
                if (m_RunAsServiceEdit != value)
                {
                    m_RunAsServiceEdit = value;
                    CommitActions.Add(
                        () =>
                        {
                            Properties.Settings.Default.RunAsService = m_RunAsServiceEdit;
                            NotifyPropertyChanged(m_RunAsServiceArgs);
                        });
                    CancelActions.Add(
                        () =>
                        {
                            m_RunAsServiceEdit = Properties.Settings.Default.RunAsService;
                            NotifyPropertyChanged(m_RunAsServiceEditArgs);
                        });
                    NotifyOptionChanged();
                    NotifyPropertyChanged(m_RunAsServiceEditArgs);
                }
            }
        }
        private bool m_RunAsServiceEdit = Properties.Settings.Default.RunAsService;
        static readonly PropertyChangedEventArgs m_RunAsServiceEditArgs =
            NotifyPropertyChangedHelper.CreateArgs<RuntimeOptionsPad>(o => o.RunAsServiceEdit);

        public bool RunAsService
        {
            get
            {
                return Properties.Settings.Default.RunAsService;
            }
        }
        static readonly PropertyChangedEventArgs m_RunAsServiceArgs =
            NotifyPropertyChangedHelper.CreateArgs<RuntimeOptionsPad>(o => o.RunAsService);
        #endregion

        #region "PortNumber"

        public string PortNumberEdit
        {
            get
            {
                return m_PortNumberEdit;
            }
            set
            {
                int parsedValue;
                if (m_PortNumberEdit != value)
                {
                    parsedValue = int.Parse(value);
                    m_PortNumberEdit = parsedValue.ToString();
                    CommitActions.Add(
                        () =>
                        {
                            Properties.Settings.Default.PortNumber = parsedValue;
                            NotifyPropertyChanged(m_PortNumberArgs);
                        });
                    CancelActions.Add(
                        () =>
                        {
                            m_PortNumberEdit = Properties.Settings.Default.PortNumber.ToString();
                            NotifyPropertyChanged(m_PortNumberEditArgs);
                        });

                    NotifyOptionChanged();
                    NotifyPropertyChanged(m_PortNumberEditArgs);
                }
            }
        }
        private string m_PortNumberEdit = Properties.Settings.Default.PortNumber.ToString();
        static readonly PropertyChangedEventArgs m_PortNumberEditArgs =
            NotifyPropertyChangedHelper.CreateArgs<RuntimeOptionsPad>(o => o.PortNumberEdit);

        public int PortNumber
        {
            get
            {
                return Properties.Settings.Default.PortNumber;
            }
        }
        static readonly PropertyChangedEventArgs m_PortNumberArgs =
            NotifyPropertyChangedHelper.CreateArgs<RuntimeOptionsPad>(o => o.PortNumber);
        #endregion

    }
}
