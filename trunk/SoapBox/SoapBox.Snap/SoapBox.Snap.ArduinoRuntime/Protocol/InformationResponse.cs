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
using SoapBox.Snap.ArduinoRuntime.Protocol.Helpers;

namespace SoapBox.Snap.ArduinoRuntime.Protocol
{
    class InformationResponse
    {
        public InformationResponse(SendReceiveResult response)
        {
            if (response == null) throw new ArgumentNullException("response");
            if (!response.Success)
            {
                throw new ProtocolException(response.Error);
            }
            if (response.Lines.Count != 7)
            {
                throw new ProtocolException("Response to \"information\" command wasn't 7 lines long.");
            }
            this.RuntimeName = response.Lines[0];

            var protocolVersionLine = new KeyAndValue(response.Lines[1]);
            if (protocolVersionLine.Key != "Protocol Version") throw new ProtocolException("Key of Protocol Version line isn't right.");
            this.ProtocolVersion = protocolVersionLine.Value;

            var booleansLine = new KeyAndValue(response.Lines[2]);
            if (booleansLine.Key != "Booleans") throw new ProtocolException("Key of Booleans line isn't right.");
            this.Booleans = Int32.Parse(booleansLine.Value);

            var numericsLine = new KeyAndValue(response.Lines[3]);
            if (numericsLine.Key != "Numerics") throw new ProtocolException("Key of Numerics line isn't right.");
            this.Numerics = Int32.Parse(numericsLine.Value);

            var stringsLine = new KeyAndValue(response.Lines[4]);
            if (stringsLine.Key != "Strings") throw new ProtocolException("Key of Strings line isn't right.");
            this.Strings = Int32.Parse(stringsLine.Value);

            var maxProgramSizeLine = new KeyAndValue(response.Lines[5]);
            if (maxProgramSizeLine.Key != "Max Program Size") throw new ProtocolException("Key of Max Program Size line isn't right.");
            this.MaxProgramSize = Int32.Parse(maxProgramSizeLine.Value);

            var currentProgramSizeLine = new KeyAndValue(response.Lines[6]);
            if (currentProgramSizeLine.Key != "Current Program Size") throw new ProtocolException("Key of Current Program Size line isn't right.");
            this.CurrentProgramSize = Int32.Parse(currentProgramSizeLine.Value);
        }

        public string RuntimeName { get; private set; }
        public string ProtocolVersion { get; private set; }
        public Int32 Booleans { get; private set; }
        public Int32 Numerics { get; private set; }
        public Int32 Strings { get; private set; }
        public Int32 MaxProgramSize { get; private set; }
        public Int32 CurrentProgramSize { get; private set; }
    }
}
