#region "SoapBox.Core License"
/// <header module="SoapBox.Core"> 
/// Copyright (C) 2009 SoapBox Automation, All Rights Reserved.
/// Contact: SoapBox Automation Licencing (license@soapboxautomation.com)
/// 
/// This file is part of SoapBox Core.
/// 
/// Commercial Usage
/// Licensees holding valid SoapBox Automation Commercial licenses may use  
/// this file in accordance with the SoapBox Automation Commercial License
/// Agreement provided with the Software or, alternatively, in accordance 
/// with the terms contained in a written agreement between you and
/// SoapBox Automation Inc.
/// 
/// GNU Lesser General Public License Usage
/// SoapBox Core is free software: you can redistribute it and/or modify 
/// it under the terms of the GNU Lesser General Public License
/// as published by the Free Software Foundation, either version 3 of the
/// License, or (at your option) any later version.
/// 
/// SoapBox Core is distributed in the hope that it will be useful, 
/// but WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
/// GNU Lesser General Public License for more details.
/// 
/// You should have received a copy of the GNU Lesser General Public License 
/// along with SoapBox Core. If not, see <http://www.gnu.org/licenses/>.
/// </header>
#endregion
        
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SoapBox.Core
{
    public interface ILayoutManager
    {
        event EventHandler Loaded;
        event EventHandler Unloading;
        event EventHandler LayoutUpdated;

        ReadOnlyCollection<IPad> Pads { get; }
        ReadOnlyCollection<IDocument> Documents { get; }

        void ShowPad(IPad pad);
        void HidePad(IPad pad);
        void HideAllPads();
        IDocument ShowDocument(IDocument document, string memento);
        IDocument ShowDocument(IDocument document, string memento, bool makeActive);
        void CloseDocument(IDocument document);
        void CloseAllDocuments();

        bool IsVisible(IPad pad);
        bool IsVisible(IDocument document);

        /// <summary>
        /// Called on recomposition by the Workbench
        /// </summary>
        void SetAllPadsDocuments(
            IEnumerable<Lazy<IPad, IPadMeta>> AllPads,
            IEnumerable<Lazy<IDocument, IDocumentMeta>> AllDocuments);

        /// <summary>
        /// Called by the workbench to notify the LayoutManager
        /// that we're about to unload
        /// </summary>
        void UnloadingWorkbench();

        string SaveLayout(); // returns a blob
        void RestoreLayout(string blob);

        bool IsActive(IDocument doc); // returns true if the given IDocument is active
    }
}
