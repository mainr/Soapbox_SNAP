#region "SoapBox.Snap License"
/// <header module="SoapBox.Snap"> 
/// Copyright (C) 2009-2016 SoapBox Automation, All Rights Reserved.
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
    class CompiledNumericSignal : CompiledSignal
    {
        private readonly bool isLiteral;
        private readonly Byte address;
        private readonly byte addressBits;
        private readonly decimal literalValue;
        private readonly bool isInput;

        public CompiledNumericSignal(bool isInput, Byte address, byte addressBits)
        {
            if (address < 0) throw new ArgumentOutOfRangeException("address");
            if (addressBits <= 0) throw new ArgumentOutOfRangeException("addressBits");
            this.isInput = isInput;
            this.isLiteral = false;
            this.address = address;
            this.addressBits = addressBits;
        }

        public CompiledNumericSignal(decimal literalValue)
        {
            this.isLiteral = true;
            this.literalValue = literalValue;
        }

        public override IEnumerable<bool> ToBits()
        {
            if (this.isLiteral)
            {
                var result = new List<bool>() { false };
                var integralPart = Convert.ToInt32(Math.Truncate(this.literalValue));
                var isFloat = (integralPart != this.literalValue);
                result.Add(isFloat);
                if (isFloat)
                {
                    // float part is 4 bytes (single)
                    var floatValue = Convert.ToSingle(this.literalValue);
                    floatToBits(floatValue, result);
                }
                else
                {
                    if (integralPart >= 0 && integralPart <= 255)
                    {
                        // 00 size = 1 byte
                        result.Add(false);
                        result.Add(false);
                        IntToBits(integralPart, result, 8);
                    }
                    else if (integralPart >= Int16.MinValue && integralPart <= Int16.MaxValue)
                    {
                        // 01 size = 2 bytes
                        result.Add(false);
                        result.Add(true);
                        IntToBits(integralPart, result, 16);
                    }
                    else if (integralPart >= Int16.MinValue*256 && integralPart <= Int16.MaxValue*256)
                    {
                        // 10 size = 3 bytes
                        result.Add(true);
                        result.Add(false);
                        IntToBits(integralPart, result, 24);
                    }
                    else if (integralPart >= Int32.MinValue && integralPart <= Int32.MaxValue)
                    {
                        // 11 size = 4 bytes
                        result.Add(true);
                        result.Add(true);
                        IntToBits(integralPart, result, 32);
                    }
                    else
                    {
                        throw new Exception("Can't store this literal value as 32-bit int (too big): " + this.literalValue);
                    }
                }
                return result;
            }
            else
            {
                var result = new List<bool>() { };
                if (this.isInput)
                {
                    result.Add(true);
                }
                this.IntToBits(this.address, result, this.addressBits);
                return result;
            }
        }

        private void floatToBits(Single floatValue, List<bool> result)
        {
            var bytes = BitConverter.GetBytes(floatValue);
            for (var b = 3; b >= 0; b--)
            {
                var shifter = bytes[b];
                for (var i = 0; i < 8; i++)
                {
                    var bit = Convert.ToBoolean(shifter & 128);
                    result.Add(bit);
                    shifter = (Byte)((shifter << 1) & (Byte)255);
                }
            }
        }
    }
}
