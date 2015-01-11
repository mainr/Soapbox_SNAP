#region "SoapBox.Snap License"
/// <header module="SoapBox.Snap"> 
/// Copyright (C) 2009-2015 SoapBox Automation, All Rights Reserved.
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

namespace SoapBox.Snap.ArduinoRuntime.Protocol.Compiler.Helpers
{
    abstract class CompiledSignal
    {
        public abstract IEnumerable<bool> ToBits();

        protected void IntToBits(Int32 value, List<bool> result, byte bits)
        {
            var skipBits = 32 - bits;
            var bitCounter = 0;
            var bytes = BitConverter.GetBytes(value);
            for (var b = 3; b >= 0; b--)
            {
                var shifter = bytes[b];
                for (var i = 0; i < 8; i++)
                {
                    bitCounter++;
                    if (bitCounter > skipBits)
                    {
                        var bit = Convert.ToBoolean(shifter & 128);
                        result.Add(bit);
                    }
                    var newShifter = (Byte)((shifter << 1) & (Byte)255);
                    //Console.WriteLine("{0}.{1}: 0x{2:X} to 0x{3:X}", b, i, shifter, newShifter); // for debug
                    shifter = newShifter;
                }
            }
        }
    }
}
