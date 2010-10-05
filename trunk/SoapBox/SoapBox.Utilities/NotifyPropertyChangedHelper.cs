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
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace SoapBox.Utilities
{
    /// <summary>
    /// This class is used along with classes that implement or hook into
    /// the INotifyPropertyChanged interface.  This class lets you use
    /// reflection to create and cache the PropertyChangedEventArgs object
    /// or the Property Name (string) at runtime rather than hard coding
    /// property names throughout the program.
    /// 
    /// Design goals:
    ///  - Improve maintainability by making everything strongly typed
    ///  - Improve performance by caching the PropertyChangedEventArgs
    /// 
    /// Based on ideas found here, but rewritten to include GetPropertyName:
    /// http://compositeextensions.codeplex.com/Thread/View.aspx?ThreadId=53731
    /// </summary>
    public static class NotifyPropertyChangedHelper
    {
        /// <summary>
        /// Use this to create and cache a PropertyChangedEventArgs object
        /// as a static member of a class which you can use as the 
        /// parameter to the PropertyChanged event.  Usage:
        /// 
        /// static readonly PropertyChangedEventArgs m_$PropertyName$Args = 
        ///     NotifyPropertyChangedHelper.CreateArgs<$ClassName$>(o => o.$PropertyName$);
        /// 
        /// In your property setter:
        ///     PropertyChanged(this, m_$PropertyName$Args)
        /// 
        /// </summary>
        /// <typeparam name="T">The type that has the property</typeparam>
        /// <param name="expression"></param>
        /// <returns>A PropertyChangedEventArgs object for caching</returns>
        public static PropertyChangedEventArgs CreateArgs<T>(Expression<Func<T, object>> expression)
        {
            return new PropertyChangedEventArgs(GetPropertyName<T>(expression));
        }

        /// <summary>
        /// Use this to create and cache a string of the property name
        /// as a static member of a class which you can use to 
        /// compare against the PropertyChangedEventArgs.PropertyName
        /// In a PropertyChanged event handler.  Usage:
        /// 
        /// static readonly string m_$PropertyName$Name = 
        ///     NotifyPropertyChangedHelper.GetPropertyName<$ClassName$>(o => o.$PropertyName$);
        /// 
        /// In your PropertyChanged event handler:
        ///     if (e.PropertyName == m_$PropertyName$Name)
        /// 
        /// </summary>
        /// <typeparam name="T">The type that has the property</typeparam>
        /// <param name="expression"></param>
        /// <returns>A PropertyChangedEventArgs object for caching</returns>
        public static string GetPropertyName<T>(Expression<Func<T, object>> expression)
        {
            PropertyInfo propertyInfo = GetPropertyInfo<T>(expression);
            return propertyInfo.Name;
        }

        private static PropertyInfo GetPropertyInfo<T>(Expression<Func<T, object>> expression)
        {
            var lambda = expression as LambdaExpression;
            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = lambda.Body as UnaryExpression;
                memberExpression = unaryExpression.Operand as MemberExpression;
            }
            else
            {
                memberExpression = lambda.Body as MemberExpression;
            }

            return memberExpression.Member as PropertyInfo;
        }
    }
}
