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
using System.Linq.Expressions;

namespace SoapBox.Utilities
{
    /// <summary>
    /// Adapted from http://stackoverflow.com/questions/1565734/is-it-possible-to-set-private-property-via-reflection
    /// </summary>
    public static class ReflectionHelper
    {
        public static T GetPrivatePropertyValue<T,U>(this U obj, string propName)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            PropertyInfo pi = obj.GetType().GetProperty(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (pi == null)
            {
                throw new ArgumentOutOfRangeException("propName", string.Format("Property {0} was not found in Type {1}", propName, obj.GetType().FullName));
            }
            return (T)pi.GetValue(obj, null);
        }

        public static T GetPrivateFieldValue<T, U>(this U obj, string propName)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            Type t = obj.GetType();
            FieldInfo fi = null;
            while (fi == null && t != null)
            {
                fi = t.GetField(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                t = t.BaseType;
            }
            if (fi == null)
            {
                throw new ArgumentOutOfRangeException("propName", string.Format("Field {0} was not found in Type {1}", propName, obj.GetType().FullName));
            }
            return (T)fi.GetValue(obj);
        }

        public static void SetPrivatePropertyValue<T, U>(this U obj, string propName, T val)
        {
            Type t = obj.GetType();
            if (t.GetProperty(propName, BindingFlags.Public | BindingFlags.NonPublic | 
                BindingFlags.Instance) == null)
            {
                throw new ArgumentOutOfRangeException("propName", 
                    string.Format("Property {0} was not found in Type {1}", propName, obj.GetType().FullName));
            }
            t.InvokeMember(propName, BindingFlags.Public | BindingFlags.NonPublic | 
                BindingFlags.SetProperty | BindingFlags.Instance, null, obj, new object[] { val });
        }

        public static void SetPrivateFieldValue<T, U>(this U obj, string propName, T val)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            Type t = obj.GetType();
            FieldInfo fi = null;
            while (fi == null && t != null)
            {
                fi = t.GetField(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                t = t.BaseType;
            }
            if (fi == null)
            {
                throw new ArgumentOutOfRangeException("propName", string.Format("Field {0} was not found in Type {1}", 
                    propName, obj.GetType().FullName));
            }
            fi.SetValue(obj, val);
        }
    }
}
