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
using SoapBox.Snap.ArduinoRuntime.Protocol.Helpers;

namespace SoapBox.Snap.ArduinoRuntime.Protocol
{
    class DownloadOrPatchResponse
    {
        public DownloadOrPatchResponse(SendReceiveResult response)
        {
            if (response == null) throw new ArgumentNullException("response");
            if (!response.Success)
            {
                throw new ProtocolException(response.Error);
            }
            if (response.Lines.Count != 2)
            {
                throw new ProtocolException("Response to \"download\" or \"patch\" command wasn't 2 lines long.");
            }

            var successLine = new KeyAndValue(response.Lines[0]);
            if (successLine.Key != "Success") throw new ProtocolException("Key of Success line isn't right.");
            this.Success = bool.Parse(successLine.Value);

            var bytesLine = new KeyAndValue(response.Lines[1]);
            if (bytesLine.Key != "Bytes") throw new ProtocolException("Key of Bytes line isn't right.");
            this.Bytes = Int32.Parse(bytesLine.Value);
        }

        public DownloadOrPatchResponse()
        {
            this.Success = false;
            this.Bytes = 0;
        }

        public bool Success { get; private set; }
        public int Bytes { get; private set; }
    }
}
