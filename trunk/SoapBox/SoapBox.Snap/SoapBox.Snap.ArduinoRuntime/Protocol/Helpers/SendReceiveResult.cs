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
using System.Collections.ObjectModel;

namespace SoapBox.Snap.ArduinoRuntime.Protocol.Helpers
{
    class SendReceiveResult
    {
        public SendReceiveResult(
            bool success,
            string error,
            List<string> lines)
        {
            if (lines == null) throw new ArgumentNullException("lines");
            if (error == null) throw new ArgumentNullException("error");
            this.Success = success;
            this.Error = error;
            this.Lines = lines.AsReadOnly();
        }

        public bool Success { get; private set; }
        public string Error { get; private set; }
        public ReadOnlyCollection<string> Lines { get; private set; }
    }
}
