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
using System.ComponentModel.Composition;
using SoapBox.Core;
using System.ComponentModel;
using SoapBox.Utilities;

namespace SoapBox.Snap.Application
{
    [Export(CompositionPoints.Workbench.Options.ApplicationOptionsPad, typeof(ApplicationOptionsPad))]
    class ApplicationOptionsPad : AbstractOptionsPad
    {
        public ApplicationOptionsPad()
        {
            Name = "ApplicationOptionsPad";
        }

        public override void Commit()
        {
            base.Commit();
            Properties.Settings.Default.Save();
        }

        #region "AutoOpenStartPage"

        public bool AutoOpenStartPageEdit
        {
            get
            {
                return m_AutoOpenStartPageEdit;
            }
            set
            {
                if (m_AutoOpenStartPageEdit != value)
                {
                    m_AutoOpenStartPageEdit = value;
                    CommitActions.Add(
                        () =>
                        {
                            Properties.Settings.Default.AutoOpenStartPage = m_AutoOpenStartPageEdit;
                            NotifyPropertyChanged(m_AutoOpenStartPageArgs);
                        });
                    CancelActions.Add(
                        () =>
                        {
                            m_AutoOpenStartPageEdit = Properties.Settings.Default.AutoOpenStartPage;
                            NotifyPropertyChanged(m_AutoOpenStartPageEditArgs);
                        });
                    NotifyOptionChanged();
                    NotifyPropertyChanged(m_AutoOpenStartPageEditArgs);
                }
            }
        }
        private bool m_AutoOpenStartPageEdit = Properties.Settings.Default.AutoOpenStartPage;
        static readonly PropertyChangedEventArgs m_AutoOpenStartPageEditArgs =
            NotifyPropertyChangedHelper.CreateArgs<ApplicationOptionsPad>(o => o.AutoOpenStartPageEdit);

        public bool AutoOpenStartPage
        {
            get
            {
                return Properties.Settings.Default.AutoOpenStartPage;
            }
        }
        static readonly PropertyChangedEventArgs m_AutoOpenStartPageArgs =
            NotifyPropertyChangedHelper.CreateArgs<ApplicationOptionsPad>(o => o.AutoOpenStartPage);
        #endregion


    }
}
