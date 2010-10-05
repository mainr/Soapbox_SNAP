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
    /// Implements the concept of a boolean (True/False).
    /// </summary>
    public sealed class FieldBool : FieldBase
    {

        private readonly bool m_boolValue;
        public bool BoolValue
        {
            get
            {
                return m_boolValue;
            }
        }

        public FieldBool(String Field)
            : base(BoolParser.GetValue(Field).ToString())
        {
            m_boolValue = BoolParser.GetValue(Field);
        }

        public FieldBool(bool Field)
            : base(Field.ToString())
        {
            m_boolValue = Field;
        }

        protected override bool DerivedCheckSyntax(String Field)
        {
            return CheckSyntax(Field);
        }

        public static bool CheckSyntax(String Field)
        {
            if (Field == null)
            {
                return false;
            }
            //It has to be some form of True or False, or 0 or 1
            else if (Field.ToLower().Contains("true")
                || Field.ToLower().Contains("false")
                || Field == "1"
                || Field == "0"
                || Field == "-1")
            {
                return true && BaseCheckSyntax(Field);
            }
            else
            {
                return false;
            }
        }

    }
}
