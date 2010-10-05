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
using System.ComponentModel.Composition;
using SoapBox.Core;

namespace SoapBox.Snap.Runtime
{
    [Export(SoapBox.Core.ExtensionPoints.Options.OptionsDialog.OptionsItems, typeof(IOptionsItem))]
    class RuntimeOptions : AbstractOptionsItem, IPartImportsSatisfiedNotification
    {
        public RuntimeOptions()
        {
            ID = Extensions.Workbench.Options.RuntimeOptions;
            InsertRelativeToID = SoapBox.Snap.Extensions.Workbench.Options.ApplicationOptions;
            BeforeOrAfter = RelativeDirection.After;
            Header = Resources.Strings.Options_RuntimeOptions_Header;
        }

        [Import(SoapBox.Core.Services.Host.ExtensionService)]
        private IExtensionService extensionService { get; set; }

        [ImportMany(ExtensionPoints.Runtime.Snap.RuntimeOptionItems, typeof(IOptionsItem), AllowRecomposition = true)]
        private IEnumerable<IOptionsItem> items { get; set; }

        [Import(CompositionPoints.Options.OptionsPad, typeof(RuntimeOptionsPad))]
        private RuntimeOptionsPad pad { get; set; }

        public void OnImportsSatisfied()
        {
            Items = extensionService.Sort(items);
            Pad = pad;
        }
    }

    /// <summary>
    /// By default, most options pages in Windows have a first sub-item
    /// that is the "General" one, which is the same options pad as the
    /// parent in the tree.  This just mimics that.
    /// </summary>
    [Export(ExtensionPoints.Runtime.Snap.RuntimeOptionItems, typeof(IOptionsItem))]
    class RuntimeOptionsGeneral : AbstractOptionsItem, IPartImportsSatisfiedNotification
    {
        public RuntimeOptionsGeneral()
        {
            Header = Resources.Strings.Options_RuntimeOptions_General;
        }

        [Import(CompositionPoints.Options.OptionsPad, typeof(RuntimeOptionsPad))]
        private RuntimeOptionsPad pad { get; set; }

        public void OnImportsSatisfied()
        {
            Pad = pad;
        }
    }
}
