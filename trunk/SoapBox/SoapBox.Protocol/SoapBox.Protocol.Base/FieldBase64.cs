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
using System.Text.RegularExpressions;

namespace SoapBox.Protocol.Base
{
    /// <summary>
    /// Implements the concept of a Base64 encoded string.
    /// Includes utility functions to encode and decode.
    /// </summary>
    public sealed class FieldBase64 : FieldBase
    {

        public FieldBase64(String base64String)
            : base(base64String)
        {
        }

        public FieldBase64()
            : base(String.Empty)
        {
        }

        public static FieldBase64 Encode(string input)
        {
            byte[] encbuf;

            encbuf = System.Text.Encoding.Unicode.GetBytes(input);
            string encoded = Convert.ToBase64String(encbuf);

            return new FieldBase64(encoded);
        }

        public string Decode()
        {
            if (this.ToString() == string.Empty)
            {
                return string.Empty;
            }
            else
            {
                byte[] decbuff;

                decbuff = Convert.FromBase64String(this.ToString());
                return System.Text.Encoding.Unicode.GetString(decbuff);
            }
        }

        protected override bool DerivedCheckSyntax(String Field)
        {
            return CheckSyntax(Field);
        }

        public static bool CheckSyntax(String Field)
        {
            // "The base-64 digits in ascending order from zero are the uppercase 
            // characters "A" to "Z", the lowercase characters "a" to "z", 
            // the numerals "0" to "9", and the symbols "+" and "/". 
            // The valueless character, "=", is used for trailing padding."
            Match m = Regex.Match(Field, "^[A-Za-z0-9+/=]*$");
            if (m.Success)
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
