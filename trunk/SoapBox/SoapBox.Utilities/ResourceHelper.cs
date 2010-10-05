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
using System.Reflection;

namespace SoapBox.Utilities
{
    /// <summary>
    /// This helper class allows you to call into resources file (like Resources.Images) and
    /// call one of the resources by name.  This is used when you want to specify something
    /// in a resource file based on Attributes, which have to be static.  Example:
    /// 
    /// var bmp = ResourceHelper.GetResourceLookup<System.Drawing.Bitmap>(typeof(Resources.Images),"MyResourceKey");
    /// </summary>
    public class ResourceHelper
    {
        public static T GetResourceLookup<T>(Type resourceType, string resourceName)
        {
            if ((resourceType != null) && (resourceName != null))
            {
                PropertyInfo property = resourceType.GetProperty(resourceName, BindingFlags.Public | BindingFlags.Static);
                if (property == null)
                {
                    return default(T);
                }
                return (T)property.GetValue(null, null);
            }
            return default(T);
        }
    }

}
