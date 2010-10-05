#region "SoapBox.Protocol License"
/// <header module="SoapBox.Protocol"> 
/// Copyright (C) 2010 SoapBox Automation Inc., All Rights Reserved.
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
    /// Implements an Immutable Field objects for storing strings with a 
    /// well defined syntax.  Implements syntax checking on creation.
    /// Throws a ArgumentOutOfRangeException if the syntax check fails.
    /// </summary>
    public abstract class FieldBase
    {
        private readonly String m_field;

        public FieldBase(String Field)
        {
            //Check the syntax for errors
            if (DerivedCheckSyntax(Field) && BaseCheckSyntax(Field))
            {
                m_field = Field;
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Each derived type must override this function to implement the
        /// rules for syntax checking of that type.
        /// </summary>
        /// <param name="Field">The string we want to validate</param>
        /// <returns>True if validation passed, false otherwise</returns>
        protected abstract bool DerivedCheckSyntax(String Field);

        /// <summary>
        /// Checks global field syntax rules.
        /// </summary>
        /// <param name="Field">The string we want to validate</param>
        /// <returns>True if validation passed, false otherwise</returns>
        protected static bool BaseCheckSyntax(String Field)
        {
            if (Field.Contains("]]>"))
            {
                //invalid because we can't store it inside
                //a <![CDATA[ ... ]]> block
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Returns the field as a string.
        /// </summary>
        /// <returns>String</returns>
        public override string ToString()
        {
            return m_field;
        }

        #region " COMPARISON "

        public override bool Equals(Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            FieldBase f = obj as FieldBase;
            if (f != null)
            {
                return this == f;
            }
            else
            {
                return obj.Equals(this);
            }
        }

        public override int GetHashCode()
        {
            return this.m_field.GetHashCode();
        }

        public static bool operator ==(FieldBase x, object y)
        {
            if ((object)x == null && (object)y == null)
            {
                return true;
            }
            else if ((object)x == null || (object)y == null)
            {
                return false;
            }
            else
            {
                return (x.m_field == y.ToString());
            }
        }

        public static bool operator !=(FieldBase x, object y)
        {
            return !(x == y);
        }


        #endregion


    }
}



