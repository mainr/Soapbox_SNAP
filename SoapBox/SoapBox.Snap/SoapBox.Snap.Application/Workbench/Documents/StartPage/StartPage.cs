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
using System.ComponentModel.Composition;
using System.Collections.ObjectModel;
using SoapBox.Utilities;
using System.ComponentModel;

namespace SoapBox.Snap.Application
{
    [Export(SoapBox.Core.ExtensionPoints.Workbench.Documents, typeof(IDocument))]
    [Export(CompositionPoints.Workbench.Documents.StartPage, typeof(StartPage))]
    [Document(Name = Extensions.Workbench.Documents.StartPage)]
    class StartPage : AbstractDocument, IPartImportsSatisfiedNotification
    {
        public const string SOAPBOXAUTOMATION_URL = "http://soapboxautomation.com";

        /// <summary>
        /// Called by MEF (to satisfy the imports)
        /// </summary>
        private StartPage()
        {
            Name = Extensions.Workbench.Documents.StartPage;
            Title = Resources.Strings.Workbench_Documents_StartPage_Title;
            FrameSource = new Uri(SOAPBOXAUTOMATION_URL);
        }

        [ImportMany(ExtensionPoints.Workbench.Documents.StartPage.GettingStartedItems, typeof(IButton))]
        private IEnumerable<IButton> gettingStartedItems { get; set; }

        [Import(SoapBox.Core.Services.Host.ExtensionService)]
        private IExtensionService extensionService { get; set; }

        public void OnImportsSatisfied()
        {
            m_GettingStartedCommands = extensionService.Sort(gettingStartedItems);
        }

        #region " CreateDocument "
        public override IDocument CreateDocument(string memento)
        {
            return this; // it's a singleton document
        }
        #endregion

        public IEnumerable<IButton> GettingStartedCommands
        {
            get
            {
                return m_GettingStartedCommands;
            }
        }
        private IEnumerable<IButton> m_GettingStartedCommands;

        #region " FrameSource "
        public Uri FrameSource
        {
            get
            {
                return m_FrameSource;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(m_FrameSourceName);
                }
                m_FrameSource = value;
                NotifyPropertyChanged(m_FrameSourceArgs);
            }
        }
        private Uri m_FrameSource = null;
        private static readonly PropertyChangedEventArgs m_FrameSourceArgs =
            NotifyPropertyChangedHelper.CreateArgs<StartPage>(o => o.FrameSource);
        private static string m_FrameSourceName =
            NotifyPropertyChangedHelper.GetPropertyName<StartPage>(o => o.FrameSource);
        #endregion

    }
}
