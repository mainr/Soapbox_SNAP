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
    public class DiscreteInputItem : AbstractSolutionItem
    {
        /// <summary>
        /// Called by MEF (just so we can do imports)
        /// </summary>
        private DiscreteInputItem()
            : base(null, string.Empty)
        {
        }

        public DiscreteInputItem(ISolutionItem parent, NodeDiscreteInput discreteInput)
            : base(parent, string.Empty)
        {
            SetIconFromBitmap(Resources.Images.On);
            onIcon = Icon;
            SetIconFromBitmap(Resources.Images.Off);
            offIcon = Icon;
            ContextMenu = extensionService.Sort(contextMenu);
            DiscreteInput = discreteInput;
            runtimeService.ValueChanged += new ValueChangedHandler(runtimeService_ValueChanged);
            runtimeService.RegisterValueWatcher(this, DiscreteInput.Signal);
        }

        private readonly object onIcon = null;
        private readonly object offIcon = null;

        void runtimeService_ValueChanged(NodeSignal signal, object value)
        {
            if (signal.SignalId == DiscreteInput.Signal.SignalId && value.GetType() == typeof(bool))
            {
                setIcons((bool)value);
            }
        }

        private void setIcons(bool signalValue)
        {
            if (DiscreteInput.Forced.BoolValue)
            {
                SetIcon2FromBitmap(Resources.Images.Forced);
                if (DiscreteInput.ForcedValue.BoolValue)
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
                if (signalValue)
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
        [ImportMany(ExtensionPoints.Workbench.Pads.SolutionPad.DiscreteInputItem.ContextMenu, typeof(IMenuItem))]
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

        #region "DiscreteInput"

        public NodeDiscreteInput DiscreteInput
        {
            get
            {
                return Node as NodeDiscreteInput;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(m_DiscreteInputName);
                }
                Node = value;
                setHeader();
                setItems();
                NotifyPropertyChanged(m_DiscreteInputArgs);
            }
        }
        private static PropertyChangedEventArgs m_DiscreteInputArgs
            = NotifyPropertyChangedHelper.CreateArgs<DiscreteInputItem>(o => o.DiscreteInput);
        private static string m_DiscreteInputName
            = NotifyPropertyChangedHelper.GetPropertyName<DiscreteInputItem>(o => o.DiscreteInput);

        #endregion

        private void setHeader()
        {
            Header = DiscreteInput.InputName.ToString();
            setIcons(false);
        }

        private void setItems()
        {
        }

    }
}
