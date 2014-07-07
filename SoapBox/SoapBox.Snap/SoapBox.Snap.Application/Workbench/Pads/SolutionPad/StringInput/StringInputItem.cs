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
    public class StringInputItem : AbstractSolutionItem
    {
        /// <summary>
        /// Called by MEF (just so we can do imports)
        /// </summary>
        private StringInputItem()
            : base(null, string.Empty)
        {
        }

        public StringInputItem(ISolutionItem parent, NodeStringInput stringInput)
            : base(parent, string.Empty)
        {
            ContextMenu = extensionService.Sort(contextMenu);
            StringInput = stringInput;
        }

        #region "contextMenuSingleton"
        [ImportMany(ExtensionPoints.Workbench.Pads.SolutionPad.StringInputItem.ContextMenu, typeof(IMenuItem))]
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

        #region "StringInput"

        public NodeStringInput StringInput
        {
            get
            {
                return Node as NodeStringInput;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(m_StringInputName);
                }
                Node = value;
                setHeader();
                setItems();
                NotifyPropertyChanged(m_StringInputArgs);
            }
        }
        private static PropertyChangedEventArgs m_StringInputArgs
            = NotifyPropertyChangedHelper.CreateArgs<StringInputItem>(o => o.StringInput);
        private static string m_StringInputName
            = NotifyPropertyChangedHelper.GetPropertyName<StringInputItem>(o => o.StringInput);

        #endregion

        private void setHeader()
        {
            Header = StringInput.InputName.ToString();
            if (StringInput.Forced.BoolValue)
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
