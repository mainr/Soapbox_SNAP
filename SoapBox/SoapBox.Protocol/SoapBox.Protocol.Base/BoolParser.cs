#region "SoapBox.Protocol License"
/// <header module="SoapBox.Protocol"> 
/// Copyright (C) 2010 SoapBox Automation, All Rights Reserved.
/// Contact: SoapBox Automation Licencing (license@soapboxautomation.com)
/// 
/// This file is part of SoapBox Protocol.
///
/// SoapBox Protocol is available under your choice of these licenses:
///  - GPLv3
///  - CDDLv1.0
///
/// GNU General Public License Usage
/// SoapBox Protocol is free software: you can redistribute it and/or modify it
/// under the terms of the GNU General Public License as published by the 
/// Free Software Foundation, either version 3 of the License, or 
/// (at your option) any later version.
/// 
/// SoapBox Protocol is distributed in the hope that it will be useful, but 
/// WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
/// GNU General Public License for more details.
/// 
/// You should have received a copy of the GNU General Public License along
/// with SoapBox Protocol. If not, see <http://www.gnu.org/licenses/>.
/// 
/// Common Development and Distribution License Usage
/// SoapBox Protocol is subject to the CDDL Version 1.0. 
/// You should have received a copy of the CDDL Version 1.0 along
/// with SoapBox Protocol.  If not, see <http://www.sun.com/cddl/cddl.html>.
/// The CDDL is a royalty free, open source, file based license.
/// </header>
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoapBox.Protocol.Base
{
    /// <summary>
    /// Parse strings into true or false bools using relaxed parsing rules
    /// Copied from here: http://dotnetperls.com/Content/bool-Parse.aspx
    /// Date: 29 MAR 2009
    /// License is here (bottom of page): http://dotnetperls.com/Content/About.aspx
    /// "7. Legal note
    /// Using code derived from the examples in the articles is encouraged, 
    /// and is allowed without attribution. The articles themselves are 
    /// original work and the author uses the full extent of the law to 
    /// protect them."
    /// </summary>
    public static class BoolParser
    {
        /// <summary>
        /// Get the boolean value for this string
        /// </summary>
        public static bool GetValue(string value)
        {
            return IsTrue(value);
        }

        /// <summary>
        /// Determine whether the string is not True
        /// </summary>
        public static bool IsFalse(string value)
        {
            return !IsTrue(value);
        }

        /// <summary>
        /// Determine whether the string is equal to True
        /// </summary>
        public static bool IsTrue(string value)
        {
            try
            {
                // 1
                // Avoid exceptions
                if (value == null)
                {
                    return false;
                }

                // 2
                // Remove whitespace from string
                value = value.Trim();

                // 3
                // Lowercase the string
                value = value.ToLower();

                // 4
                // Check for word true
                if (value == "true")
                {
                    return true;
                }

                // 5
                // Check for letter true
                if (value == "t")
                {
                    return true;
                }

                // 6
                // Check for one
                if (value == "1")
                {
                    return true;
                }

                // 7
                // Check for word yes
                if (value == "yes")
                {
                    return true;
                }

                // 8
                // Check for letter yes
                if (value == "y")
                {
                    return true;
                }

                // 9
                // It is false
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
