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
    public delegate void SignalChangedHandler(NodeSignal signal);
    public delegate void ValueChangedHandler(NodeSignal signal, object value);

    public interface IRuntimeService
    {
        Tuple<string,NodeSignal> FindSignal(INodeWrapper requester, FieldGuid signalId);
        Dictionary<string, NodeSignal> SignalList(INodeWrapper requester, FieldDataType.DataTypeEnum dataTypeFilter);
        NodeSignalIn SignalDialog(INodeWrapper requester, NodeSignalIn defaultSignalIn);
        Tuple<NodePage, NodeRuntimeApplication> FindParentPageAndRuntimeApp(INodeWrapper requester);
        event SignalChangedHandler SignalChanged; // fired when a signal is edited (name, comment, force status, etc.)
        void NotifySignalChanged(NodeSignal signal);
        event ValueChangedHandler ValueChanged; // fired when controls should update their display of the "state"
        void NotifyValueChanged(NodeSignal signal, object value);
        FieldConstant GetConstant(FieldDataType.DataTypeEnum dataType, FieldConstant defaultConstant);
        bool Connected(INodeWrapper requester); // returns true if the given node's parent runtime application is connected
        // returns true if Connected(...) is false, or gives the user a chance to disconnect, returning true if they do
        bool DisconnectDialog(INodeWrapper requester);
        void RegisterValueWatcher(INodeWrapper requester, NodeSignal signal);
        void DeregisterValueWatcher(INodeWrapper requester, NodeSignal signal);
    }
}
