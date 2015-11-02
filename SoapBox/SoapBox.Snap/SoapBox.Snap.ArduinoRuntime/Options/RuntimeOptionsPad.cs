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

namespace SoapBox.Snap.ArduinoRuntime
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

        #region "ExportSignalTable"

        public bool ExportSignalTableEdit
        {
            get
            {
                return m_ExportSignalTableEdit;
            }
            set
            {
                if (m_ExportSignalTableEdit != value)
                {
                    m_ExportSignalTableEdit = value;
                    CommitActions.Add(
                        () =>
                        {
                            Properties.Settings.Default.ExportSignalTable = m_ExportSignalTableEdit;
                            NotifyPropertyChanged(m_ExportSignalTableArgs);
                        });
                    CancelActions.Add(
                        () =>
                        {
                            m_ExportSignalTableEdit = Properties.Settings.Default.ExportSignalTable;
                            NotifyPropertyChanged(m_ExportSignalTableEditArgs);
                        });
                    NotifyOptionChanged();
                    NotifyPropertyChanged(m_ExportSignalTableEditArgs);
                }
            }
        }
        private bool m_ExportSignalTableEdit = Properties.Settings.Default.ExportSignalTable;
        static readonly PropertyChangedEventArgs m_ExportSignalTableEditArgs =
            NotifyPropertyChangedHelper.CreateArgs<RuntimeOptionsPad>(o => o.ExportSignalTableEdit);

        public bool ExportSignalTable
        {
            get
            {
                return Properties.Settings.Default.ExportSignalTable;
            }
        }
        static readonly PropertyChangedEventArgs m_ExportSignalTableArgs =
            NotifyPropertyChangedHelper.CreateArgs<RuntimeOptionsPad>(o => o.ExportSignalTable);
        #endregion

        #region "ExportSignalTableTo"

        public string ExportSignalTableToEdit
        {
            get
            {
                return m_ExportSignalTableToEdit;
            }
            set
            {
                if (m_ExportSignalTableToEdit != value)
                {
                    m_ExportSignalTableToEdit = value;
                    CommitActions.Add(
                        () =>
                        {
                            Properties.Settings.Default.ExportSignalTableTo = value;
                            NotifyPropertyChanged(m_ExportSignalTableToArgs);
                        });
                    CancelActions.Add(
                        () =>
                        {
                            m_ExportSignalTableToEdit = Properties.Settings.Default.ExportSignalTableTo;
                            NotifyPropertyChanged(m_ExportSignalTableToEditArgs);
                        });

                    NotifyOptionChanged();
                    NotifyPropertyChanged(m_ExportSignalTableToEditArgs);
                }
            }
        }
        private string m_ExportSignalTableToEdit = Properties.Settings.Default.ExportSignalTableTo;
        static readonly PropertyChangedEventArgs m_ExportSignalTableToEditArgs =
            NotifyPropertyChangedHelper.CreateArgs<RuntimeOptionsPad>(o => o.ExportSignalTableToEdit);

        public string ExportSignalTableTo
        {
            get
            {
                return Properties.Settings.Default.ExportSignalTableTo;
            }
        }
        static readonly PropertyChangedEventArgs m_ExportSignalTableToArgs =
            NotifyPropertyChangedHelper.CreateArgs<RuntimeOptionsPad>(o => o.ExportSignalTableTo);
        #endregion

    }
}
