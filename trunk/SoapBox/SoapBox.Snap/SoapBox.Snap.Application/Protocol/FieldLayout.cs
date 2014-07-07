#region "SoapBox.Snap License"
/// <header module="SoapBox.Snap"> 
/// Copyright (C) 2009-2014 SoapBox Automation, All Rights Reserved.
/// Contact: SoapBox Automation Licencing (license@soapboxautomation.com)
/// 
/// This file is part of SoapBox Snap.
/// 
/// SoapBox Snap is free software: you can redistribute it and/or modify it
/// under the terms of the GNU General Public License as published by the 
/// Free Software Foundation, either version 3 of the License, or 
/// (at your option) any later version.
/// 
/// SoapBox Snap is distributed in the hope that it will be useful, but 
/// WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
/// GNU General Public License for more details.
/// 
/// You should have received a copy of the GNU General Public License along
/// with SoapBox Snap. If not, see <http://www.gnu.org/licenses/>.
/// </header>
#endregion
        
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SoapBox.Protocol.Base;

namespace SoapBox.Snap.Application
{
    /// <summary>
    /// Implements a place to store the solution layout string
    /// so the software can remember what windows, etc., were
    /// open.  Limited to base64 chars and "."
    /// </summary>
    public sealed class FieldLayout : FieldBase
    {
        public FieldLayout(String Layout)
            : base(Layout)
        {
        }

        protected override bool DerivedCheckSyntax(String Field)
        {
            return CheckSyntax(Field);
        }

        public static bool CheckSyntax(String Field)
        {
            if (Field.Contains("\n"))
            {
                return false;
            }

            // Can only contain A-Z, a-z, 0-9, +, /, = and .
            Match m = Regex.Match(Field, "^[A-Za-z0-9+/=.]*$");
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
