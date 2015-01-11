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

namespace SoapBox.Snap.ArduinoRuntime.Protocol.Compiler
{
    class CompiledProgram
    {
        const int GUID_LENGTH = 16;

        const int RUNTIME_ID_INDEX = 0;
        const int VERSION_ID_INDEX = RUNTIME_ID_INDEX + GUID_LENGTH;
        const int BITS_PER_BOOL_INDEX = VERSION_ID_INDEX + GUID_LENGTH;
        const int BITS_PER_NUMERIC_INDEX = BITS_PER_BOOL_INDEX + 1;
        const int OPCODES_INDEX = BITS_PER_NUMERIC_INDEX + 1;

        private readonly Guid m_runtimeGuid;
        private readonly Guid m_versionGuid;
        private readonly byte m_bitsPerBooleanAddress;
        private readonly byte m_bitsPerNumericAddress;
        private readonly byte[] m_opcodes;

        public CompiledProgram(
            Guid runtimeGuid,
            Guid versionGuid,
            byte bitsPerBooleanAddress,
            byte bitsPerNumericAddress,
            byte[] opcodes)
        {
            if (opcodes == null) throw new ArgumentNullException("opcodes");
            this.m_runtimeGuid = runtimeGuid;
            this.m_versionGuid = versionGuid;
            this.m_bitsPerBooleanAddress = bitsPerBooleanAddress;
            this.m_bitsPerNumericAddress = bitsPerNumericAddress;
            this.m_opcodes = opcodes;
        }

        public byte[] ToByteArray()
        {
            var length = OPCODES_INDEX + this.m_opcodes.Length;
            var result = new byte[length];

            var runtimeGuid = this.m_runtimeGuid.ToByteArray();
            Array.Copy(runtimeGuid, 0, result, RUNTIME_ID_INDEX, GUID_LENGTH);

            var versionGuid = this.m_versionGuid.ToByteArray();
            Array.Copy(versionGuid, 0, result, VERSION_ID_INDEX, GUID_LENGTH);

            result[BITS_PER_BOOL_INDEX] = this.m_bitsPerBooleanAddress;
            result[BITS_PER_NUMERIC_INDEX] = this.m_bitsPerNumericAddress;

            Array.Copy(this.m_opcodes, 0, result, OPCODES_INDEX, this.m_opcodes.Length);

            return result;
        }
    }
}
