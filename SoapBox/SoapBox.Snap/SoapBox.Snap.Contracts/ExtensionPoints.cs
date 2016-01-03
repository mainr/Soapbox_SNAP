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

namespace SoapBox.Snap.ExtensionPoints
{
    public static class Workbench
    {
        public static class ToolBars
        {
            public const string SolutionToolBar = "SoapBox.Snap.ExtensionPoints.Workbench.ToolBars.SolutionToolBar";
        }
        public static class Options
        {
            public const string ApplicationOptionItems = "SoapBox.Snap.ExtensionPoints.Workbench.Options.ApplicationOptionItems";
        }
        public static class Documents
        {
            public static class PageEditor
            {
                public const string InstructionGroupItems = "SoapBox.Snap.ExtensionPoints.Workbench.Documents.PageEditor.InstructionGroupItems";
                public const string InstructionItems = "SoapBox.Snap.ExtensionPoints.Workbench.Documents.PageEditor.InstructionItems";
                public const string ContextMenu = "SoapBox.Snap.ExtensionPoints.Workbench.Documents.PageEditor.ContextMenu";
            }
            public static class StartPage
            {
                public const string GettingStartedItems = "SoapBox.Snap.ExtensionPoints.Workbench.Documents.StartPage.GettingStartedItems";
            }
        }
        public static class Pads
        {
            public static class SolutionPad
            {
                public const string SolutionItems = "SoapBox.Snap.ExtensionPoints.Workbench.Pads.SolutionPad.SolutionItems";

                public static class RootSolutionItem
                {
                    public const string ContextMenu = "SoapBox.Snap.ExtensionPoints.Workbench.Pads.SolutionPad.RootSolutionItem.ContextMenu";
                    public static class ContextMenu_
                    {
                        public const string Add = "SoapBox.Snap.ExtensionPoints.Workbench.Pads.SolutionPad.RootSolutionItem.ContextMenu.Add";
                    }
                }
                public static class RuntimeApplicationItem
                {
                    public const string ContextMenu = "SoapBox.Snap.ExtensionPoints.Workbench.Pads.SolutionPad.RuntimeApplicationItem.ContextMenu";
                    public static class ContextMenu_
                    {
                        public const string Add = "SoapBox.Snap.ExtensionPoints.Workbench.Pads.SolutionPad.RuntimeApplicationItem.ContextMenu.Add";
                    }
                }
                public static class DeviceConfigurationItem
                {
                    public const string ContextMenu = "SoapBox.Snap.ExtensionPoints.Workbench.Pads.SolutionPad.DeviceConfigurationItem.ContextMenu";
                    public static class ContextMenu_
                    {
                    }
                }
                public static class PageCollectionItem
                {
                    public const string ContextMenu = "SoapBox.Snap.ExtensionPoints.Workbench.Pads.SolutionPad.PageCollectionItem.ContextMenu";
                    public static class ContextMenu_
                    {
                        public const string Add = "SoapBox.Snap.ExtensionPoints.Workbench.Pads.SolutionPad.PageCollectionItem.ContextMenu.Add";
                    }
                }
                public static class PageItem
                {
                    public const string ContextMenu = "SoapBox.Snap.ExtensionPoints.Workbench.Pads.SolutionPad.PageItem.ContextMenu";
                    public static class ContextMenu_
                    {
                    }
                }
                public static class DriverItem
                {
                    public const string ContextMenu = "SoapBox.Snap.ExtensionPoints.Workbench.Pads.SolutionPad.DriverItem.ContextMenu";
                    public static class ContextMenu_
                    {
                    }
                }
                public static class DeviceItem
                {
                    public const string ContextMenu = "SoapBox.Snap.ExtensionPoints.Workbench.Pads.SolutionPad.DeviceItem.ContextMenu";
                    public static class ContextMenu_
                    {
                    }
                }
                public static class DiscreteInputItem
                {
                    public const string ContextMenu = "SoapBox.Snap.ExtensionPoints.Workbench.Pads.SolutionPad.DiscreteInputItem.ContextMenu";
                    public static class ContextMenu_
                    {
                    }
                }
                public static class DiscreteOutputItem
                {
                    public const string ContextMenu = "SoapBox.Snap.ExtensionPoints.Workbench.Pads.SolutionPad.DiscreteOutputItem.ContextMenu";
                    public static class ContextMenu_
                    {
                    }
                }
                public static class SignalInItem
                {
                    public const string ContextMenu = "SoapBox.Snap.ExtensionPoints.Workbench.Pads.SolutionPad.SignalInItem.ContextMenu";
                    public static class ContextMenu_
                    {
                    }
                }
                public static class AnalogInputItem
                {
                    public const string ContextMenu = "SoapBox.Snap.ExtensionPoints.Workbench.Pads.SolutionPad.AnalogInputItem.ContextMenu";
                    public static class ContextMeu_
                    {
                    }
                }
                public static class AnalogOutputItem
                {
                    public const string ContextMenu = "SoapBox.Snap.ExtensionPoints.Workbench.Pads.SolutionPad.AnalogOutputItem.ContextMenu";
                    public static class ContextMeu_
                    {
                    }
                }
                public static class StringInputItem
                {
                    public const string ContextMenu = "SoapBox.Snap.ExtensionPoints.Workbench.Pads.SolutionPad.StringInputItem.ContextMenu";
                    public static class ContextMeu_
                    {
                    }
                }
                public static class StringOutputItem
                {
                    public const string ContextMenu = "SoapBox.Snap.ExtensionPoints.Workbench.Pads.SolutionPad.StringOutputItem.ContextMenu";
                    public static class ContextMeu_
                    {
                    }
                }
            }
        }
        
    }
    public static class Runtime
    {
        public const string Types = "SoapBox.Snap.ExtensionPoints.Runtime.Types";
    }
    public static class Driver
    {
        public const string Types = "SoapBox.Snap.ExtensionPoints.Driver.Types";
    }
    public static class Device
    {
        public const string Types = "SoapBox.Snap.ExtensionPoints.Device.Types";
    }
}
