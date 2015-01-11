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
    class CompiledBooleanSignal : CompiledSignal
    {
        private readonly bool isLiteral;
        private readonly Int16 address;
        private readonly byte addressBits;
        private readonly bool literalValue;
        private readonly bool isInput;

        public CompiledBooleanSignal(bool isInput, Int16 address, byte addressBits)
        {
            if (address < 0) throw new ArgumentOutOfRangeException("address");
            if (addressBits <= 0) throw new ArgumentOutOfRangeException("addressBits");
            this.isLiteral = false;
            this.isInput = isInput;
            this.address = address;
            this.addressBits = addressBits;
        }

        public CompiledBooleanSignal(bool literalValue)
        {
            this.isInput = true;
            this.isLiteral = true;
            this.literalValue = literalValue;
        }

        public override IEnumerable<bool> ToBits()
        {
            if (this.isLiteral)
            {
                return new bool[] { false, this.literalValue };
            }
            else
            {
                var result = new List<bool>() {  };
                if (this.isInput)
                {
                    result.Add(true);
                }
                this.IntToBits(this.address, result, this.addressBits);
                return result;
            }
        }
    }
}
