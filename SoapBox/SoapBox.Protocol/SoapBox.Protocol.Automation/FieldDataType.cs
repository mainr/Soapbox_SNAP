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
    /// Implements the concept of a DataType.  It has to belong to a 
    /// defined list of DataTypes.
    /// </summary>
    public sealed class FieldDataType : FieldBase
    {
        /// <summary>
        /// Represents a list of all Data Types
        /// </summary>
        public enum DataTypeEnum
        {
            ANY,
            BOOL,
            NUMBER,
            DATETIME,
            STRING
        }

        /// <summary>
        /// Represents a list of only "Generic" types
        /// </summary>
        private enum DataTypeEnumGeneric
        {
            ANY
        }

        //Readonly member variables
        private readonly DataTypeEnum m_dataType;

        public DataTypeEnum DataType
        {
            get
            {
                return m_dataType;
            }
        }

        public FieldDataType(String DataType) : base(DataType)
        {
            m_dataType = 
                (DataTypeEnum) System.Enum.Parse(typeof(DataTypeEnum), DataType);
        }

        public FieldDataType(DataTypeEnum DataType) 
            : base(System.Enum.GetName(typeof(DataTypeEnum), DataType))
        {
            //Compiler will check validity
            m_dataType = DataType;
        }

        protected override bool DerivedCheckSyntax(String Field)
        {
            return CheckSyntax(Field);
        }

        /// <summary>
        /// Given a string, checks if it represents a 
        /// valid  data type
        /// </summary>
        /// <param name="DataType">String representation of a data type, 
        /// like "BOOL"</param>
        /// <returns>True if valid, false otherwise</returns>
        public static bool CheckSyntax(String Field)
        {
            bool valid = true;
            try
            {
                DataTypeEnum dataTypeTemp =
                    (DataTypeEnum)System.Enum.Parse(typeof(DataTypeEnum), Field);
            }
            catch
            {
                valid = false;
            }
            return valid && BaseCheckSyntax(Field);
        }

        /// <summary>
        /// True if this instance represents a "Generic" data type, 
        /// like "ANY"
        /// </summary>
        /// <returns>True if Generic, false otherwise</returns>
        public bool IsGeneric()
        {
            return IsGeneric(m_dataType);
        }

        public static bool IsGeneric(DataTypeEnum check)
        {
            if(System.Enum.GetNames(typeof(DataTypeEnumGeneric)).Contains(
                check.ToString()
                ))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the default value for a given data type
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns></returns>
        public static object DefaultValue(DataTypeEnum dataType)
        {
            //validation (throws exception if it fails - better now than later)
            switch (dataType)
            {
                case FieldDataType.DataTypeEnum.ANY:
                case FieldDataType.DataTypeEnum.BOOL:
                    return (Boolean)false;
                case FieldDataType.DataTypeEnum.NUMBER:
                    return (Decimal)0;
                case FieldDataType.DataTypeEnum.DATETIME:
                    return DateTime.MinValue;
                case FieldDataType.DataTypeEnum.STRING:
                    return string.Empty;
                default:
                    return null;
            }
        }

        public bool IsOfType(DataTypeEnum filter)
        {
            if (!IsGeneric(filter))
            {
                // they have to equal exactly
                return m_dataType == filter;
            }
            else
            {
                // can be a subset
                switch (filter)
                {
                    case DataTypeEnum.ANY:
                        return true;
                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// Checks that the given object is valid for the given data type
        /// </summary>
        /// <param name="value"></param>
        /// <param name="dataType"></param>
        /// <returns></returns>
        public static bool CheckType(object value, DataTypeEnum dataType)
        {
            if (value == null)
            {
                return false;
            }
            else
            {
                //validation (throws exception if it fails - better now than later)
                switch (dataType)
                {
                    case FieldDataType.DataTypeEnum.ANY:
                        if (value.GetType() != typeof(Boolean) && value.GetType() != typeof(Decimal) && 
                            value.GetType() != typeof(DateTime) && value.GetType() != typeof(String))
                        {
                            return false;
                        }
                        break;
                    case FieldDataType.DataTypeEnum.BOOL:
                        if (value.GetType() != typeof(Boolean))
                        {
                            return false;
                        }
                        break;
                    case FieldDataType.DataTypeEnum.DATETIME:
                        if (value.GetType() != typeof(DateTime))
                        {
                            return false;
                        }
                        break;
                    case FieldDataType.DataTypeEnum.NUMBER:
                        if (value.GetType() != typeof(Decimal))
                        {
                            return false;
                        }
                        break;
                    case FieldDataType.DataTypeEnum.STRING:
                        if (value.GetType() != typeof(String))
                        {
                            return false;
                        }
                        break;
                }
                return true;
            }
        }

        public bool TryParse(string value, out object result)
        {
            result = null;
            if (value == null || IsGeneric())
            {
                return false;
            }
            else
            {
                bool success = false;
                switch (m_dataType)
                {
                    case FieldDataType.DataTypeEnum.BOOL:
                        Boolean resultBoolean;
                        success = Boolean.TryParse(value, out resultBoolean);
                        if (success)
                        {
                            result = resultBoolean;
                        }
                        break;
                    case FieldDataType.DataTypeEnum.DATETIME:
                        DateTime resultDateTime;
                        success = DateTime.TryParse(value, out resultDateTime);
                        if (success)
                        {
                            result = resultDateTime;
                        }
                        break;
                    case FieldDataType.DataTypeEnum.NUMBER:
                        Decimal resultInt32;
                        success = Decimal.TryParse(value, out resultInt32);
                        if (success)
                        {
                            result = resultInt32;
                        }
                        break;
                    case FieldDataType.DataTypeEnum.STRING:
                        result = value;
                        success = true;
                        break;
                }
                return success;
            }
        }
    }
}
