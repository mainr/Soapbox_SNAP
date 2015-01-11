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
    public class AnalogOutputItem : AbstractSolutionItem
    {
        /// <summary>
        /// Called by MEF (just so we can do imports)
        /// </summary>
        private AnalogOutputItem()
            : base(null, string.Empty)
        {
        }

        public AnalogOutputItem(ISolutionItem parent, NodeAnalogOutput analogOutput)
            : base(parent, string.Empty)
        {
            ContextMenu = extensionService.Sort(contextMenu);
            AnalogOutput = analogOutput;
        }

        #region "contextMenuSingleton"
        [ImportMany(ExtensionPoints.Workbench.Pads.SolutionPad.AnalogOutputItem.ContextMenu, typeof(IMenuItem))]
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

        #region "AnalogOutput"

        public NodeAnalogOutput AnalogOutput
        {
            get
            {
                return Node as NodeAnalogOutput;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(m_AnalogOutputName);
                }
                Node = value;
                setHeader();
                setItems();
                NotifyPropertyChanged(m_AnalogOutputArgs);
            }
        }
        private static PropertyChangedEventArgs m_AnalogOutputArgs
            = NotifyPropertyChangedHelper.CreateArgs<AnalogOutputItem>(o => o.AnalogOutput);
        private static string m_AnalogOutputName
            = NotifyPropertyChangedHelper.GetPropertyName<AnalogOutputItem>(o => o.AnalogOutput);

        #endregion

        private void setHeader()
        {
            Header = AnalogOutput.OutputName.ToString();
            if (AnalogOutput.Forced.BoolValue)
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
            var newCollection = new ObservableCollection<INodeWrapper>();
            var signalIn = FindItemByNodeId(AnalogOutput.SignalIn.ID) as SignalInItem;
            if (signalIn == null)
            {
                signalIn = new SignalInItem(this, AnalogOutput.SignalIn);
                HookupHandlers(signalIn);
            }
            newCollection.Add(signalIn);
            Items = newCollection;
        }

        void HookupHandlers(SignalInItem signalIn)
        {
            signalIn.Parent = this;
            signalIn.Edited += new EditedHandler(SignalIn_Edited);
        }

        void SignalIn_Edited(INodeWrapper sender, NodeBase oldNode, NodeBase newNode)
        {
            var oldSignalIn = oldNode as NodeSignalIn;
            var newSignalIn = newNode as NodeSignalIn;
            if (oldSignalIn != null && newSignalIn != null)
            {
                AnalogOutput = AnalogOutput.SetSignalIn(newSignalIn);
            }
        }
    }
}
