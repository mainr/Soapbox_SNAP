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
using SoapBox.Protocol.Base;

namespace SoapBox.Snap.Extensions
{
    public static class Host
    {
        public static class StartupCommands
        {
            public const string SetApplicationTitle = "SetApplicationTitle";
            public const string StartRuntimeEngine = "StartRuntimeEngine";
            public const string OpenSolutionFromArgumentOrStartPage = "OpenSolutionFromArgumentOrStartPage";
        }
        public static class ShutdownCommands
        {
            public const string StopRuntimeEngine = "StopRuntimeEngine";
        }
    }
    public static class Workbench
    {
        public static class MainMenu
        {
            public static class FileMenu
            {
                public const string New = "New";
                public const string Open = "Open";
                public const string Close = "Close";
                public const string CloseSeparator = "CloseSeparator";
                public const string Save = "Save";
                public const string SaveAs = "SaveAs";
                public const string ExitSeparator = "ExitSeparator";
            }
            public static class ViewMenu
            {
                public const string SolutionExplorer = "SolutionExplorer";
                public const string Instructions = "Instructions";
                public const string StartPage = "StartPage";
            }
            public static class HelpMenu
            {
                public const string About = "About";
                public const string AboutSeparator = "AboutSeparator";
            }
        }
        public static class ToolBars
        {
            public static class SolutionToolBar
            {
                public const string New = "New";
                public const string Open = "Open";
                public const string Save = "Save";
            }
        }
        public static class Pads
        {
            public const string SolutionPad = "SolutionPad";
            public const string InstructionPad = "InstructionPad";
            public static class SolutionPad_
            {
                public static class PageCollectionItem
                {
                    public static class ContextMenu
                    {
                        public const string MoveUp = "MoveUp";
                        public const string MoveDown = "MoveDown";
                        public const string AddSeparator = "AddSeparator";
                        public const string Add = "Add";
                        public static class Add_
                        {
                            public const string PageCollection = "PageCollection";
                            public const string Page = "Page";
                        }
                        public const string DeleteSeparator = "DeleteSeparator";
                        public const string Delete = "Delete";
                    }
                }
                public static class PageItem
                {
                    public static class ContextMenu
                    {
                        public const string MoveUp = "MoveUp";
                        public const string MoveDown = "MoveDown";
                        public const string DeleteSeparator = "DeleteSeparator";
                        public const string Delete = "Delete";
                    }
                }
                public static class RootSolutionItem
                {
                    public static class ContextMenu
                    {
                        public const string Add = "Add";
                        public static class Add_
                        {
                            public const string RuntimeApplication = "RuntimeApplication";
                        }

                        public const string Properties = "Properties";
                    }
                }
                public static class RuntimeApplicationItem
                {
                    public static class ContextMenu
                    {
                        public const string Connect = "Connect";
                        public const string Disconnect = "Disconnect";
                        public const string Start = "Start";
                        public const string Stop = "Stop";
                        public const string DeleteSeparator = "DeleteSeparator";
                        public const string Delete = "Delete";
                        public const string PropertiesSeparator = "PropertiesSeparator";
                        public const string Properties = "Properties";
                    }
                }
                public static class DeviceConfigurationItem
                {
                    public static class ContextMenu
                    {
                        public const string ReadDeviceConfiguration = "ReadDeviceConfiguration";
                    }
                }
                public static class DiscreteInputItem
                {
                    public static class ContextMenu
                    {
                        public const string ForceOn = "ForceOn";
                        public const string ForceOff = "ForceOff";
                        public const string RemoveForce = "RemoveForce";
                    }
                }
                public static class DiscreteOutputItem
                {
                    public static class ContextMenu
                    {
                        public const string ForceOn = "ForceOn";
                        public const string ForceOff = "ForceOff";
                        public const string RemoveForce = "RemoveForce";
                    }
                }
                public static class AnalogInputItem
                {
                    public static class ContextMenu
                    {
                        public const String Force = "Force";
                        public const string RemoveForce = "RemoveForce";
                    }
                }
                public static class AnalogOutputItem
                {
                    public static class ContextMenu
                    {
                        public const String Force = "Force";
                        public const string RemoveForce = "RemoveForce";
                    }
                }
                public static class StringInputItem
                {
                    public static class ContextMenu
                    {
                        public const String Force = "Force";
                        public const string RemoveForce = "RemoveForce";
                    }
                }
                public static class StringOutputItem
                {
                    public static class ContextMenu
                    {
                        public const String Force = "Force";
                        public const string RemoveForce = "RemoveForce";
                    }
                }
            }
        }
        public static class Documents
        {
            public const string RuntimeApplicationProperties = "RuntimeApplicationProperties";
            public const string PageEditor = "PageEditor";
            public static class PageEditor_
            {
                public static class ContextMenu
                {
                    public const string InsertBefore = "InsertBefore";
                    public const string InsertAfter = "InsertAfter";
                    public const string Append = "Append";
                    public const string AppendSeparator = "AppendSeparator";
                    public const string MoveUp = "MoveUp";
                    public const string MoveDown = "MoveDown";
                    public const string MoveSeparator = "MoveSeparator";
                    public const string Undo = "Undo";
                    public const string Redo = "Redo";
                    public const string DeleteSeparator = "DeleteSeparator";
                    public const string Delete = "Delete";
                }
            }
            public const string StartPage = "StartPage";
            public static class StartPage_
            {
                public static class GettingStartedCommands
                {
                    public const string LoadNewSolution = "LoadNewSolution";
                    public const string LoadExample = "LoadExample";
                    public const string ViewTutorial = "ViewTutorial";
                }
            }
        }
        public static class Options
        {
            public const string ApplicationOptions = "ApplicationOptions";
        }
    }
}
