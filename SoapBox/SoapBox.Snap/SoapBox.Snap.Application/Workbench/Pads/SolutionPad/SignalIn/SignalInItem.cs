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
    public class SignalInItem : AbstractSolutionItem
    {
        /// <summary>
        /// Called by MEF (just so we can do imports)
        /// </summary>
        private SignalInItem()
            : base(null, string.Empty)
        {
        }

        public SignalInItem(ISolutionItem parent, NodeSignalIn signalIn)
            : base(parent, string.Empty)
        {
            ContextMenu = extensionService.Sort(contextMenu);
            SignalIn = signalIn;
            runtimeService.SignalChanged += new SignalChangedHandler(runtimeService_SignalChanged);
        }

        void runtimeService_SignalChanged(NodeSignal signal)
        {
            if (SignalIn != null && SignalIn.SignalId != null && SignalIn.SignalId == signal.SignalId)
            {
                setHeader();
            }
        }

        #region "contextMenuSingleton"
        [ImportMany(ExtensionPoints.Workbench.Pads.SolutionPad.SignalInItem.ContextMenu, typeof(IMenuItem))]
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

        #region "SignalIn"

        public NodeSignalIn SignalIn
        {
            get
            {
                return Node as NodeSignalIn;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(m_SignalInName);
                }
                Node = value;
                setHeader();
                setItems();
                NotifyPropertyChanged(m_SignalInArgs);
            }
        }
        private static PropertyChangedEventArgs m_SignalInArgs
            = NotifyPropertyChangedHelper.CreateArgs<SignalInItem>(o => o.SignalIn);
        private static string m_SignalInName
            = NotifyPropertyChangedHelper.GetPropertyName<SignalInItem>(o => o.SignalIn);

        #endregion

        private void setHeader()
        {
            var tpl = runtimeService.FindSignal(this, SignalIn.SignalId);
            if (tpl != null)
            {
                ToolTip = tpl.Item1.Replace("/", "\r\n");
                if (tpl.Item1.Length <= 20)
                {
                    Header = tpl.Item1;
                }
                else
                {
                    Header = "..." + tpl.Item1.Substring(tpl.Item1.Length - 17);
                }               
            }
            else
            {
                ToolTip = string.Empty;
                if (SignalIn.Literal != null)
                {
                    Header = SignalIn.Literal.Value.ToString();
                }
                else
                {
                    Header = string.Empty;
                }
            }
        }

        private void setItems()
        {
        }

        public override void Open()
        {
            SignalIn = runtimeService.SignalDialog(this, SignalIn);
        }
    }
}
