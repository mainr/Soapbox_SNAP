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
using SoapBox.Protocol.Automation;
using System.ComponentModel.Composition;
using SoapBox.Core;
using System.Collections.ObjectModel;
using System.ComponentModel;
using SoapBox.Utilities;
using SoapBox.Protocol.Base;

namespace SoapBox.Snap.Application
{
    [Export(ExtensionPoints.Workbench.Pads.SolutionPad.SolutionItems, typeof(ISolutionItem))]
    public class AnalogInputItem : AbstractSolutionItem
    {
        /// <summary>
        /// Called by MEF (just so we can do imports)
        /// </summary>
        private AnalogInputItem()
            : base(null, string.Empty)
        {
        }

        public AnalogInputItem(ISolutionItem parent, NodeAnalogInput analogInput)
            : base(parent, string.Empty)
        {
            ContextMenu = extensionService.Sort(contextMenu);
            AnalogInput = analogInput;
        }

        #region "contextMenuSingleton"
        [ImportMany(ExtensionPoints.Workbench.Pads.SolutionPad.AnalogInputItem.ContextMenu, typeof(IMenuItem))]
        private IEnumerable<IMenuItem> contextMenu
        {
            get
            {
                return contextMenuSingleton;
            }
            set
            {
                contextMenuSingleton = value;
            }
        }
        private static IEnumerable<IMenuItem> contextMenuSingleton = null;
        #endregion

        #region "AnalogInput"

        public NodeAnalogInput AnalogInput
        {
            get
            {
                return Node as NodeAnalogInput;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(m_AnalogInputName);
                }
                Node = value;
                setHeader();
                setItems();
                NotifyPropertyChanged(m_AnalogInputArgs);
            }
        }
        private static PropertyChangedEventArgs m_AnalogInputArgs
            = NotifyPropertyChangedHelper.CreateArgs<AnalogInputItem>(o => o.AnalogInput);
        private static string m_AnalogInputName
            = NotifyPropertyChangedHelper.GetPropertyName<AnalogInputItem>(o => o.AnalogInput);

        #endregion

        private void setHeader()
        {
            Header = AnalogInput.InputName.ToString();
            if (AnalogInput.Forced.BoolValue)
            {
                SetIconFromBitmap(Resources.Images.Forced);
            }
            else
            {
                Icon = null;
            }
        }

        private void setItems()
        {
        }

    }
}
