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
using SoapBox.Core;
using SoapBox.Protocol.Automation;
using System.ComponentModel.Composition;

namespace SoapBox.Snap.Twitter
{
    [Export(SoapBox.Core.ExtensionPoints.Host.Void, typeof(object))]
    public class AbstractTwitterDevice: AbstractExtension, ISnapDevice
    {
        public const string ADDRESS_SEPARATOR = ",";

        /// <summary>
        /// Just here for MEF to call
        /// </summary>
        private AbstractTwitterDevice()
        {
        }

        public AbstractTwitterDevice(string typeId, string name)
        {
            m_TypeId = new Guid(typeId);
            m_Name = name;
        }

        public Guid TypeId
        {
            get
            {
                return m_TypeId;
            }
        }
        private readonly Guid m_TypeId = Guid.Empty;

        public string Name
        {
            get
            {
                return m_Name;
            }
        }
        private readonly string m_Name = string.Empty;

        public virtual NodeDevice Build() { return null; }

        [ImportMany(ExtensionPoints.Device.ContextMenu, typeof(IMenuItem))]
        public IEnumerable<IMenuItem> ContextMenu { get; set; }

    }
}
