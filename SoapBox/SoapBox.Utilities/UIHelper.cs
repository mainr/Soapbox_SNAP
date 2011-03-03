#region "SoapBox.Utilities License"
/// <header module="SoapBox.Utilities"> 
/// Copyright (C) 2010 SoapBox Automation Inc., All Rights Reserved.
/// Contact: SoapBox Automation Licencing (license@soapboxautomation.com)
/// 
/// This file is part of SoapBox Utilities.
/// 
/// Commercial Usage
/// Licensees holding valid SoapBox Automation Commercial licenses may use  
/// this file in accordance with the SoapBox Automation Commercial License
/// Agreement provided with the Software or, alternatively, in accordance 
/// with the terms contained in a written agreement between you and
/// SoapBox Automation Inc.
/// 
/// GNU Lesser General Public License Usage
/// SoapBox Utilities is free software: you can redistribute it and/or modify 
/// it under the terms of the GNU Lesser General Public License
/// as published by the Free Software Foundation, either version 3 of the
/// License, or (at your option) any later version.
/// 
/// SoapBox Utilities is distributed in the hope that it will be useful, 
/// but WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
/// GNU Lesser General Public License for more details.
/// 
/// You should have received a copy of the GNU Lesser General Public License 
/// along with SoapBox Utilities. If not, see <http://www.gnu.org/licenses/>.
/// </header>
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace SoapBox.Utilities
{
    public static class UIHelper
    {
        public static T TryFindParentByName<T>(FrameworkElement child, string name) where T : FrameworkElement
        {
            FrameworkElement parentObject = TryFindParent<T>(child);

            if (parentObject == null)
            {
                return null;
            }

            T myParentT = parentObject as T;
            if (myParentT != null && myParentT.Name == name)
            {
                return myParentT;
            }
            else
            {
                return TryFindParentByName<T>(parentObject, name);
            }
        }

        /// <summary>
        /// Uses recursion to find a parent of a given type.
        /// Returns null if not found.
        /// </summary>
        /// <typeparam name="T">Type you're looking for.</typeparam>
        /// <param name="child">Where to start</param>
        /// <returns></returns>
        public static T TryFindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = GetParentObject(child);

            if (parentObject == null)
            {
                return null;
            }

            T myParentT = parentObject as T;
            if (myParentT != null)
            {
                return myParentT;
            }
            else
            {
                return TryFindParent<T>(parentObject);
            }

        }

        public static T TryFindSibling<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            if (parentObject == null)
            {
                return null;
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parentObject); i++)
            {
                T mySiblingT = VisualTreeHelper.GetChild(parentObject, i) as T;
                if (mySiblingT != null)
                {
                    return mySiblingT;
                }
            }
            return null;
        }

        public static DependencyObject GetParentObject(DependencyObject child)
        {
            if (child == null) return null;
            ContentElement contentElement = child as ContentElement;

            if (contentElement != null)
            {
                DependencyObject parent = ContentOperations.GetParent(contentElement);
                if (parent != null) return parent;

                FrameworkContentElement fce = contentElement as FrameworkContentElement;
                return fce != null ? fce.Parent : null;
            }

            //if it's not a ContentElement, rely on VisualTreeHelper
            return VisualTreeHelper.GetParent(child);
        }
    }
}
