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

namespace SoapBox.Snap
{
    public interface IRuntime
    {
        IRuntimeType Type { get; }
        bool Start(); // true = success
        bool Stop(); // true = success
        bool Running { get; }
        bool RuntimeApplicationDownload(NodeRuntimeApplication runtimeApplication); // true = success
        NodeRuntimeApplication RuntimeApplicationUpload(); // return null if it couldn't read
        FieldGuid RuntimeId(); // returns null if there is no runtime application loaded in the engine
        FieldGuid RuntimeVersionId(); // returns null if there is no runtime application loaded in the engine
        NodeDeviceConfiguration ReadConfiguration(); // return null if it couldn't read
        NodePeer Peer { get; }
        void MessageReceivedFromPeer(NodeBase message);
        NodeBase DeltaReceivedFromPeer(FieldGuid basedOnMessageID);
        void ReadSignalValues(IEnumerable<NodeSignal> signals);
    }
}
