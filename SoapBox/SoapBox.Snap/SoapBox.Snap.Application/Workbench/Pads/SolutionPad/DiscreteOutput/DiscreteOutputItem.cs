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
    public class DiscreteOutputItem : AbstractSolutionItem
    {
        /// <summary>
        /// Called by MEF (just so we can do imports)
        /// </summary>
        private DiscreteOutputItem()
            : base(null, string.Empty)
        {
        }

        public DiscreteOutputItem(ISolutionItem parent, NodeDiscreteOutput discreteOutput)
            : base(parent, string.Empty)
        {
            SetIconFromBitmap(Resources.Images.On);
            onIcon = Icon;
            SetIconFromBitmap(Resources.Images.Off);
            offIcon = Icon;
            ContextMenu = extensionService.Sort(contextMenu);
            DiscreteOutput = discreteOutput;
            runtimeService.ValueChanged += new ValueChangedHandler(runtimeService_ValueChanged);
        }

        private readonly object onIcon = null;
        private readonly object offIcon = null;

        void runtimeService_ValueChanged(NodeSignal signal, object value)
        {
            if (signal.SignalId == DiscreteOutput.SignalIn.SignalId && value.GetType() == typeof(bool))
            {
                setIcons((bool)value);
            }
        }

        private void setIcons(bool signalInValue)
        {
            if (DiscreteOutput.Forced.BoolValue)
            {
                SetIcon2FromBitmap(Resources.Images.Forced);
                if (DiscreteOutput.ForcedValue.BoolValue)
                {
                    Icon = onIcon;
                }
                else
                {
                    Icon = offIcon;
                }
            }
            else
            {
                Icon2 = null;
                if (signalInValue)
                {
                    Icon = onIcon;
                }
                else
                {
                    Icon = offIcon;
                }
            }
        }

        #region "contextMenuSingleton"
        [ImportMany(ExtensionPoints.Workbench.Pads.SolutionPad.DiscreteOutputItem.ContextMenu, typeof(IMenuItem))]
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

        #region "DiscreteOutput"

        public NodeDiscreteOutput DiscreteOutput
        {
            get
            {
                return Node as NodeDiscreteOutput;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(m_DiscreteOutputName);
                }
                Node = value;
                setHeader();
                setItems();
                NotifyPropertyChanged(m_DiscreteOutputArgs);
            }
        }
        private static PropertyChangedEventArgs m_DiscreteOutputArgs
            = NotifyPropertyChangedHelper.CreateArgs<DiscreteOutputItem>(o => o.DiscreteOutput);
        private static string m_DiscreteOutputName
            = NotifyPropertyChangedHelper.GetPropertyName<DiscreteOutputItem>(o => o.DiscreteOutput);

        #endregion

        private void setHeader()
        {
            Header = DiscreteOutput.OutputName.ToString();
            setIcons(false);
            if(DiscreteOutput.SignalIn.SignalId != null)
            {
                var signal = runtimeService.FindSignal(this, DiscreteOutput.SignalIn.SignalId).Item2;
                runtimeService.RegisterValueWatcher(this, signal);
            }
        }

        private void setItems()
        {
            var newCollection = new ObservableCollection<INodeWrapper>();
            var signalIn = FindItemByNodeId(DiscreteOutput.SignalIn.ID) as SignalInItem;
            if (signalIn == null)
            {
                signalIn = new SignalInItem(this, DiscreteOutput.SignalIn);
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
                DiscreteOutput = DiscreteOutput.SetSignalIn(newSignalIn);
            }
        }
    }
}
