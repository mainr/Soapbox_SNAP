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
    public sealed class FieldConstant : FieldBase
    {
        public static class Constants
        {
            public static class BOOL
            {
                public readonly static FieldConstant HIGH = new FieldConstant(FieldDataType.DataTypeEnum.BOOL, true);
                public readonly static FieldConstant LOW = new FieldConstant(FieldDataType.DataTypeEnum.BOOL, false);
            }
            public static class NUMBER
            {
                public readonly static FieldConstant ZERO = new FieldConstant(FieldDataType.DataTypeEnum.NUMBER, 0);
            }
            public static class STRING
            {
                public readonly static FieldConstant EMPTY = new FieldConstant(FieldDataType.DataTypeEnum.STRING, string.Empty);
            }
        }

        public const string SEPARATOR = ",";

        public FieldConstant(String resurrect)
            : base(resurrect)
        {
        }

        public FieldConstant(FieldDataType.DataTypeEnum DataType, object Constant)
            : base(buildField(DataType, Constant))
        {
        }

        protected override bool DerivedCheckSyntax(String Field)
        {
            return CheckSyntax(Field);
        }

        private static string buildField(FieldDataType.DataTypeEnum DataType, object Constant)
        {
            return DataType.ToString() + SEPARATOR + Constant.ToString();
        }

        public static bool CheckSyntax(FieldDataType.DataTypeEnum DataType, object Constant)
        {
            return CheckSyntax(buildField(DataType, Constant));
        }

        public static bool CheckSyntax(String Field)
        {
            if (!Field.Contains(SEPARATOR))
            {
                return false;
            }
            string[] parts = Field.Split(new string[] {SEPARATOR}, StringSplitOptions.None);
            if (Enum.IsDefined(typeof(FieldDataType.DataTypeEnum), parts[0]))
            {
                FieldDataType.DataTypeEnum dtEnum = 
                    (FieldDataType.DataTypeEnum)Enum.Parse(typeof(FieldDataType.DataTypeEnum), parts[0]);

                FieldDataType dt = new FieldDataType(dtEnum);

                object o;
                return dt.TryParse(parts[1], out o) && BaseCheckSyntax(Field);
            }
            else
            {
                return false;
            }
        }

        #region "Value"
        /// <summary>
        /// Lazy evaluated, but immutable
        /// </summary>
        public object Value
        {
            get
            {
                object o;
                lock (m_ValueLock)
                {
                    if (m_Value == null)
                    {
                        string[] parts = this.ToString().Split(new string[] {SEPARATOR}, StringSplitOptions.None);

                        FieldDataType dt = new FieldDataType(DataType);

                        if (!dt.TryParse(parts[1],out  m_Value))
                        {
                            m_Value = FieldDataType.DefaultValue(DataType);
                        }
                    }
                    o = m_Value;
                }
                return o;
            }
        }
        private object m_Value = null;
        private readonly object m_ValueLock = new object();
        #endregion

        #region "DataType"
        public FieldDataType.DataTypeEnum DataType
        {
            get
            {
                FieldDataType.DataTypeEnum dtEnum = FieldDataType.DataTypeEnum.ANY;
                lock (m_DataTypeLock)
                {
                    if (m_DataType == FieldDataType.DataTypeEnum.ANY)
                    {
                        string[] parts = this.ToString().Split(new string[] { SEPARATOR }, StringSplitOptions.None);
                        m_DataType = (FieldDataType.DataTypeEnum)Enum.Parse(typeof(FieldDataType.DataTypeEnum), parts[0]);
                    }
                    dtEnum = m_DataType;
                }
                return dtEnum;
            }
        }
        private FieldDataType.DataTypeEnum m_DataType = FieldDataType.DataTypeEnum.ANY;
        private readonly object m_DataTypeLock = new object();
        #endregion
    }
}
