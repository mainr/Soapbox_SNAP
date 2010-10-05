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

namespace SoapBox.Snap.LD.ExtensionPoints
{
    public static class Runtime
    {
        public static class Snap
        {
            public static class GroupExecutors_
            {
                public static class LD_
                {
                    public const string InstructionExecutors = "SoapBox.Snap.LD.ExtensionPoints.Runtime.Snap.GroupExecutors_.LD_.InstructionExecutors";
                }
            }
        }
    }
    public static class Instructions
    {
        public const string ContextMenu = "SoapBox.Snap.LD.ExtensionPoints.Instructions.ContextMenu"; // context menu items that apply to all LD instructions

        public static class Parallel
        {
            public const string ContextMenu = "SoapBox.Snap.LD.ExtensionPoints.Instructions.Parallel.ContextMenu";
        }
        public static class Coil
        {
            public const string ContextMenu = "SoapBox.Snap.LD.ExtensionPoints.Instructions.Coil.ContextMenu";
        }
        public static class ContactNO
        {
            public const string ContextMenu = "SoapBox.Snap.LD.ExtensionPoints.Instructions.ContactNO.ContextMenu";
        }
        public static class ContactNC
        {
            public const string ContextMenu = "SoapBox.Snap.LD.ExtensionPoints.Instructions.ContactNC.ContextMenu";
        }
        public static class TmrON
        {
            public const string ContextMenu = "SoapBox.Snap.LD.ExtensionPoints.Instructions.TmrON.ContextMenu";
        }
        public static class TmrOFF
        {
            public const string ContextMenu = "SoapBox.Snap.LD.ExtensionPoints.Instructions.TmrOFF.ContextMenu";
        }
        public static class CntUP
        {
            public const string ContextMenu = "SoapBox.Snap.LD.ExtensionPoints.Instructions.CntUP.ContextMenu";
        }
        public static class CntDN
        {
            public const string ContextMenu = "SoapBox.Snap.LD.ExtensionPoints.Instructions.CntDN.ContextMenu";
        }
        public static class SetReset
        {
            public const string ContextMenu = "SoapBox.Snap.LD.ExtensionPoints.Instructions.SetReset.ContextMenu";
        }
        public static class RisingEdge
        {
            public const string ContextMenu = "SoapBox.Snap.LD.ExtensionPoints.Instructions.RisingEdge.ContextMenu";
        }
        public static class FallingEdge
        {
            public const string ContextMenu = "SoapBox.Snap.LD.ExtensionPoints.Instructions.FallingEdge.ContextMenu";
        }
        public static class StringContains
        {
            public const string ContextMenu = "SoapBox.Snap.LD.ExtensionPoints.Instructions.StringContains.ContextMenu";
        }
    }
}
