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

namespace SoapBox.Snap.CompositionPoints
{
    public static class Workbench
    {
        public static class Pads
        {
            public const string SolutionPad = "SoapBox.Snap.CompositionPoints.Workbench.Pads.SolutionPad";
            public const string InstructionPad = "SoapBox.Snap.CompositionPoints.Workbench.Pads.InstructionPad";
            public static class SolutionPad_
            {
                public const string RootSolutionItem = "SoapBox.Snap.CompositionPoints.Workbench.Pads.SolutionPad.RootSolutionItem";
            }
        }
        public static class Documents
        {
            public const string RuntimeApplicationProperties = "SoapBox.Snap.CompositionPoints.Workbench.Documents.RuntimeApplicationProperties";
            public const string PageEditor = ".PageEditor";
            public const string StartPage = "SoapBox.Snap.CompositionPoints.Workbench.Documents.StartPage";
        }
        public static class Dialogs
        {
            public const string SignalChooserDialog = "SoapBox.Snap.CompositionPoints.Workbench.Dialogs.SignalChooserDialog";
            public const string GetConstantDialog = "SoapBox.Snap.CompositionPoints.Workbench.Dialogs.GetConstantDialog";
            public const string UploadDownloadDialog = "SoapBox.Snap.CompositionPoints.Workbench.Dialogs.UploadDownloadDialog";
        }
        public static class Options
        {
            public const string ApplicationOptionsPad = "SoapBox.Snap.CompositionPoints.Workbench.Options.ApplicationOptionsPad";
        }
    }
}
