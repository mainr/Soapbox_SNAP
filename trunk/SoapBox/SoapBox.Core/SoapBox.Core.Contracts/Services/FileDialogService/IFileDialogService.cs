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

namespace SoapBox.Core
{
    public interface IFileDialogService
    {
        /// <summary>
        /// Asks the user to select a file
        /// </summary>
        /// <param name="defaultExtension">Example: "txt"</param>
        /// <param name="initialDirectory">Example: @"C:\"</param>
        /// <param name="filters">Examples: 
        ///     "txt", "Text Documents" 
        ///     "bmp", "Bitmaps" 
        ///     (Note: automatically inserts the "All Files" option)
        ///     </param>
        /// <param name="title">Example: "Open file..."</param>
        /// <param name="addExtension">Set to true to auto-add the extension</param>
        /// <param name="checkFileExists">Warn if file doesn't exist.</param>
        /// <param name="checkPathExists">Warn if path doesn't exist.</param>
        /// <returns>null if user cancels, otherwise the filename</returns>
        string OpenFileDialog(
            string defaultExtension,
            string initialDirectory,
            Dictionary<string, string> filters,
            string title,
            bool addExtension,
            bool checkFileExists,
            bool checkPathExists);

        string SaveFileDialog(
            string defaultExtension,
            string initialDirectory, 
            Dictionary<string,string> filters,
            string title,
            bool addExtension,
            bool checkFileExists,
            bool checkPathExists);

    }
}
