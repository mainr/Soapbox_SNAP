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
using System.ComponentModel.Composition;
using System.Windows.Forms;
using System.Text;

namespace SoapBox.Core.FileDialog
{
    [Export(Services.FileDialog.FileDialogService, typeof(IFileDialogService))]
    class FileDialogService : IFileDialogService
    {
        #region IFileDialogService Members

        public string OpenFileDialog(string defaultExtension, string initialDirectory, 
            Dictionary<string, string> filters, string title, 
            bool addExtension, bool checkFileExists, bool checkPathExists)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                PopulateFileDialog(dlg, defaultExtension, initialDirectory,
                    filters, title, addExtension, checkFileExists, checkPathExists);
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    return dlg.FileName;
                }
                else
                {
                    return null;
                }
            }
        }

        public string SaveFileDialog(string defaultExtension, string initialDirectory,
            Dictionary<string, string> filters, string title,
            bool addExtension, bool checkFileExists, bool checkPathExists)
        {
            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                PopulateFileDialog(dlg, defaultExtension, initialDirectory, 
                    filters, title, addExtension, checkFileExists, checkPathExists);
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    return dlg.FileName;
                }
                else
                {
                    return null;
                }
            }
        }

        private void PopulateFileDialog(System.Windows.Forms.FileDialog dlg,
            string defaultExtension, string initialDirectory, Dictionary<string, string> filters,
            string title, bool addExtension, bool checkFileExists, bool checkPathExists)
        {
            dlg.DefaultExt = defaultExtension;
            dlg.InitialDirectory = initialDirectory;
            dlg.Title = title;
            dlg.AddExtension = addExtension;
            dlg.CheckFileExists = checkFileExists;
            dlg.CheckPathExists = checkPathExists;

            dlg.FilterIndex = 0;
            int thisIndex = 0;
            StringBuilder filtValue = new StringBuilder();
            foreach (string k in filters.Keys)
            {
                if (filtValue.Length > 0)
                {
                    filtValue.Append("|");
                }
                filtValue.Append(filters[k] + " (*." + k + ")|*." + k);
                if (k == defaultExtension)
                {
                    dlg.FilterIndex = thisIndex;
                }
                thisIndex++;
            }

            // Add an All filter
            if (filtValue.Length > 0)
            {
                filtValue.Append("|");
            }
            filtValue.Append(Resources.Strings.All_Files + " (*.*)|*.*");

            dlg.Filter = filtValue.ToString();
        }

        #endregion
    }
}
