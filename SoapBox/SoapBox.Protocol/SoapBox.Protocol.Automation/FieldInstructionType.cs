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
using System.Text.RegularExpressions;
using SoapBox.Protocol.Base;

namespace SoapBox.Protocol.Automation
{
    /// <summary>
    /// Represents a unique identifier for an instruction type.
    /// e.g. LD.Standard.Coil might be the Coil instruction 
    /// in the Standard Library for the LD language.
    /// </summary>
    public sealed class FieldInstructionType : FieldBase
    {
        const string SEPARATOR = ".";

        public FieldInstructionType(String resurrect)
            : base(resurrect)
        {
            // The base class checks the syntax, so we can assume this works
            string[] parts = resurrect.Split(new string[] { SEPARATOR }, StringSplitOptions.None);

            m_Language = new FieldIdentifier(parts[0]);
            m_Library = new FieldIdentifier(parts[1]);
            m_Code = new FieldIdentifier(parts[2]);
        }

        public FieldInstructionType(FieldIdentifier language, FieldIdentifier library, FieldIdentifier code)
            : base(
            language.ToString() + SEPARATOR + 
            library.ToString() + SEPARATOR + 
            code.ToString())
        {
            m_Language = language;
            m_Library = library;
            m_Code = code;
        }

        protected override bool DerivedCheckSyntax(String Field)
        {
            return CheckSyntax(Field);
        }

        public static bool CheckSyntax(String Field)
        {
            string[] parts = Field.Split(new string[] { SEPARATOR }, StringSplitOptions.None);

            if (parts.Length != 3)
            {
                return false;
            }
            for (int i = 0; i < 3; i++)
            {
                bool syntaxCheckPassed = FieldIdentifier.CheckSyntax(parts[i]);
                if (!syntaxCheckPassed)
                {
                    return false;
                }
            }
            return true && BaseCheckSyntax(Field);
        }

        #region " Language "
        public FieldIdentifier Language
        {
            get
            {
                return m_Language;
            }
        }
        private readonly FieldIdentifier m_Language = null;
        #endregion

        #region " Library "
        public FieldIdentifier Library
        {
            get
            {
                return m_Library;
            }
        }
        private readonly FieldIdentifier m_Library = null;
        #endregion

        #region " Code "
        public FieldIdentifier Code
        {
            get
            {
                return m_Code;
            }
        }
        private readonly FieldIdentifier m_Code = null;
        #endregion

    }
}
