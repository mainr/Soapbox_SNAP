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
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;

namespace SoapBox.Core
{

    /// <summary>
    /// Utility methods and functions to work with IExtension objects.
    /// </summary>
    [Export((Services.Host.ExtensionService), typeof(IExtensionService))]
    public class ExtensionService : IExtensionService 
    {
        [Import(Services.Logging.LoggingService, typeof(ILoggingService))]
        private ILoggingService logger { get; set; }

        /// <summary>
        /// Joins two extension collections and puts another item between them.
        /// Very handy for joining two imported collections of IMenuItems with a separator
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="extensionCollection1"></param>
        /// <param name="joinItem"></param>
        /// <param name="extensionCollection2"></param>
        /// <returns></returns>
        public IEnumerable<T> SortAndJoin<T>(IEnumerable<T> extensionCollection1, T joinItem, IEnumerable<T> extensionCollection2) where T : IExtension
        {
            IEnumerable<T> sorted1 = Sort(extensionCollection1);
            IEnumerable<T> sorted2 = Sort(extensionCollection2);

            foreach (T t in sorted1)
            {
                yield return t;
            }

            yield return joinItem;

            foreach (T t in sorted2)
            {
                yield return t;
            }
        }

        /// <summary>
        /// Takes a collection of extensions and returns a sorted list
        /// of those extensions based on the InsertBeforeID  
        /// property of each extension.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="extensions"></param>
        /// <returns></returns>
        public IList<T> Sort<T>(IEnumerable<T> extensionCollection) where T : IExtension
        {
            List<T> extensions = new List<T>(extensionCollection);
            List<T> sortedExtensions = new List<T>();
            List<T> unsortedExtensions = new List<T>();
            foreach (T newExtension in extensions)
            {
                if (newExtension.InsertRelativeToID == null)
                {
                    sortedExtensions.Add(newExtension);
                }
                else if(FindByID(newExtension.InsertRelativeToID, extensions) == -1)
                {
                    // found a configuration error
                    logger.ErrorWithFormat("Configuration error with extension ID {0}, InsertBeforeID of {1} doesn't exist.", 
                        newExtension.ID, newExtension.InsertRelativeToID);
                    sortedExtensions.Add(newExtension);
                }
                else
                {
                    unsortedExtensions.Add(newExtension);
                }
            }
            while (unsortedExtensions.Count > 0)
            {
                List<T> stillUnsortedExtensions = new List<T>();
                int startingCount = unsortedExtensions.Count;
                foreach (T newExtension in unsortedExtensions)
                {
                    int index = FindByID(newExtension.InsertRelativeToID, sortedExtensions);
                    if (index > -1)
                    {
                        if (newExtension.BeforeOrAfter == RelativeDirection.Before)
                        {
                            sortedExtensions.Insert(index, newExtension);
                        }
                        else
                        {
                            if (index == sortedExtensions.Count - 1)
                            {
                                //it's to be inserted after the last item in the list
                                sortedExtensions.Add(newExtension);
                            }
                            else
                            {
                                sortedExtensions.Insert(index + 1, newExtension);
                            }
                        }
                    }
                    else
                    {
                        stillUnsortedExtensions.Add(newExtension);
                    }
                }
                if (startingCount == stillUnsortedExtensions.Count)
                {
                    // We didn't make any progress
                    logger.Error("Configuration error with one of these extensions:");
                    foreach(IExtension ext in stillUnsortedExtensions)
                    {
                        logger.ErrorWithFormat("ID = {0}, InsertBeforeID = {1}", ext.ID, ext.InsertRelativeToID);
                    }
                    // Pick one and add it at the end.
                    sortedExtensions.Add(stillUnsortedExtensions[0]);
                    stillUnsortedExtensions.RemoveAt(0);
                }
                unsortedExtensions = stillUnsortedExtensions;
            }
            return sortedExtensions;
        }

        /// <summary>
        /// Returns the index of the extension with the given ID,
        /// or -1 if not found.
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="extensions"></param>
        /// <returns></returns>
        private int FindByID<T>(string ID, IList<T> extensions) where T : IExtension 
        {
            for (int i = 0; i < extensions.Count; i++)
            {
                if (extensions[i].ID == ID)
                {
                    return i;
                }
            }
            return -1;
        }

    }
}
