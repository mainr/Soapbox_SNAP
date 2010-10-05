#region "SoapBox.Snap License"
/// <header module="SoapBox.Snap"> 
/// Copyright (C) 2009 SoapBox Automation Inc., All Rights Reserved.
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
using SoapBox.Protocol.Automation;
using SoapBox.Protocol.Base;

namespace SoapBox.Snap.Runtime
{
    class EncodedSignalValue
    {
        private const string SEPARATOR = ",";
        private const string END_OF_LINE = "\r\n";

        // Encodes it as:
        // SignalId (guid) followed by a comma (,)
        // DataType (BOOL, NUMBER, DATETIME, STRING) followed by a comma (,)
        // Value Length (uint) followed by a comma (,)
        // Data Value as string - fixed length
        public static string EncodeSignalValue(NodeSignal signal)
        {
            if (signal != null)
            {
                string value = signal.Value.ToString();
                var retVal = new StringBuilder();
                retVal.Append(signal.SignalId.ToString());
                retVal.Append(SEPARATOR);
                retVal.Append(signal.DataType.DataType.ToString());
                retVal.Append(SEPARATOR);
                retVal.Append(value.Length.ToString());
                retVal.Append(SEPARATOR);
                retVal.Append(value);
                // don't really need this, but it makes the resultant string a big easier to work with and debug
                retVal.Append(END_OF_LINE); 
                return retVal.ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        public static IEnumerable<Tuple<NodeSignal, object>> ParseEncodedSignals(string encoded, IEnumerable<NodeSignal> signals)
        {
            // parse it all into values
            var dict = new Dictionary<FieldGuid, object>();
            int index = 0;
            bool found = true;
            while (found)
            {
                found = false;
                int commaPos1 = encoded.IndexOf(SEPARATOR, index);
                if (commaPos1 > 0) // signalId
                {
                    int commaPos2 = encoded.IndexOf(SEPARATOR, commaPos1 + 1);
                    if (commaPos2 > 0) // DataType
                    {
                        int commaPos3 = encoded.IndexOf(SEPARATOR, commaPos2 + 1);
                        if (commaPos3 > 0) // value length
                        {
                            string valueLengthString = encoded.Substring(commaPos2 + 1, commaPos3 - commaPos2 - 1);
                            int valueLength;
                            if (int.TryParse(valueLengthString, out valueLength))
                            {
                                string valueString = encoded.Substring(commaPos3 + 1, valueLength);
                                if (valueString.Length == valueLength)
                                {
                                    string guidString = encoded.Substring(index, commaPos1 - index);
                                    if (FieldGuid.CheckSyntax(guidString))
                                    {
                                        var signalId = new FieldGuid(guidString);
                                        string dataTypeString = encoded.Substring(commaPos1 + 1, commaPos2 - commaPos1 - 1);
                                        FieldDataType.DataTypeEnum dataType;
                                        if (Enum.TryParse<FieldDataType.DataTypeEnum>(dataTypeString, out dataType))
                                        {
                                            if (FieldConstant.CheckSyntax(dataType + FieldConstant.SEPARATOR + valueString))
                                            {
                                                var constant = new FieldConstant(dataType + FieldConstant.SEPARATOR + valueString);
                                                found = true;
                                                index = commaPos3 + valueLength + 1 + END_OF_LINE.Length;
                                                dict.Add(signalId, constant.Value);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // match it up to the signals
            var retVal = new List<Tuple<NodeSignal, object>>();
            foreach (var signal in signals)
            {
                if (dict.ContainsKey(signal.SignalId))
                {
                    retVal.Add(new Tuple<NodeSignal, object>(signal, dict[signal.SignalId]));
                }
            }
            return retVal;
        }
    }
}
