#region "SoapBox.Snap License"
/// <header module="SoapBox.Snap"> 
/// Copyright (C) 2009-2014 SoapBox Automation, All Rights Reserved.
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
    public class StringOutputItem : AbstractSolutionItem
    {
        /// <summary>
        /// Called by MEF (just so we can do imports)
        /// </summary>
        private StringOutputItem()
            : base(null, string.Empty)
        {
        }

        public StringOutputItem(ISolutionItem parent, NodeStringOutput stringOutput)
            : base(parent, string.Empty)
        {
            ContextMenu = extensionService.Sort(contextMenu);
            StringOutput = stringOutput;
        }

        #region "contextMenuSingleton"
        [ImportMany(ExtensionPoints.Workbench.Pads.SolutionPad.StringOutputItem.ContextMenu, typeof(IMenuItem))]
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

        #region "StringOutput"

        public NodeStringOutput StringOutput
        {
            get
            {
                return Node as NodeStringOutput;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(m_StringOutputName);
                }
                Node = value;
                setHeader();
                setItems();
                NotifyPropertyChanged(m_StringOutputArgs);
            }
        }
        private static PropertyChangedEventArgs m_StringOutputArgs
            = NotifyPropertyChangedHelper.CreateArgs<StringOutputItem>(o => o.StringOutput);
        private static string m_StringOutputName
            = NotifyPropertyChangedHelper.GetPropertyName<StringOutputItem>(o => o.StringOutput);

        #endregion

        private void setHeader()
        {
            Header = StringOutput.OutputName.ToString();
            if (StringOutput.Forced.BoolValue)
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
            var signalIn = FindItemByNodeId(StringOutput.SignalIn.ID) as SignalInItem;
            if (signalIn == null)
            {
                signalIn = new SignalInItem(this, StringOutput.SignalIn);
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
                StringOutput = StringOutput.SetSignalIn(newSignalIn);
            }
        }
    }
}
