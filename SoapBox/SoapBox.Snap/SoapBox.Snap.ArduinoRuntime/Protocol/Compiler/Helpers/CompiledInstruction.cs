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
    class CompiledInstruction
    {
        private readonly Byte m_opCode;
        private readonly Byte m_opCodeBits;
        private readonly CompiledSignal[] m_signals;

        public CompiledInstruction(
            Byte opCode,
            Byte opCodeBits,
            params CompiledSignal[] signals)
        {
            if (signals == null) throw new ArgumentNullException("signals");
            this.m_opCode = opCode;
            this.m_opCodeBits = opCodeBits;
            this.m_signals = signals;
        }

        public IEnumerable<bool> ToBits()
        {
            var result = new List<bool>();
            var skipBits = 8 - this.m_opCodeBits;
            var bitCounter = 0;
            var shifter = this.m_opCode;
            for (var i = 0; i < 8; i++)
            {
                bitCounter++;
                if (bitCounter > skipBits)
                {
                    var bit = Convert.ToBoolean(shifter & 128);
                    result.Add(bit);
                }
                shifter = (Byte)((shifter << 1) & (Byte)255);
            }
            foreach (var signal in this.m_signals)
            {
                result.AddRange(signal.ToBits());
            }
            return result;
        }
    }
}
