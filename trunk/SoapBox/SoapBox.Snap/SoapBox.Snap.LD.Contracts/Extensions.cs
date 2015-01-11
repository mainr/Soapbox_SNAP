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

namespace SoapBox.Snap.LD.Extensions
{
    public static class Workbench
    {
        public static class Documents
        {
            public static class PageEditor_
            {
                public static class InstructionGroupItems
                {
                    public const string LD = "LD";
                }
                public static class InstructionItems
                {
                    public const string LD = InstructionGroupItems.LD; // language
                    public static class LD_
                    {
                        public const string Snap = "Snap"; // library
                        public static class Snap_
                        {
                            // Instructions
                            public const string Series = "Series";
                            public const string ContactNO = "ContactNO";
                            public const double ContactNO_SortOrder = 1;
                            public const string ContactNC = "ContactNC";
                            public const double ContactNC_SortOrder = 2;
                            public const string Coil = "Coil";
                            public const double Coil_SortOrder = 3;
                            public const string SetReset = "SetReset";
                            public const double SetReset_SortOrder = 4;
                            public const string RisingEdge = "RisingEdge";
                            public const double RisingEdge_SortOrder = 5;
                            public const string FallingEdge = "FallingEdge";
                            public const double FallingEdge_SortOrder = 6;
                            public const string Parallel = "Parallel";
                            public const double Parallel_SortOrder = 7;
                            public const string TmrON = "TmrON";
                            public const double TmrON_SortOrder = 8;
                            public const string TmrOFF = "TmrOFF";
                            public const double TmrOFF_SortOrder = 9;
                            public const string CntUP = "CntUP";
                            public const double CntUP_SortOrder = 10;
                            public const string CntDN = "CntDN";
                            public const double CntDN_SortOrder = 11;
                            public const string StringContains = "StringContains";
                            public const double StringContains_SortOrder = 12;
                        }

                        public const string SnapComparison = "SnapComparison"; // library
                        public static class SnapComparison_
                        {
                            // Instructions
                            public const string Equal = "Equal";
                            public const double Equal_SortOrder = 1;
                            public const string NotEqual = "NotEqual";
                            public const double NotEqual_SortOrder = 2;
                            public const string GreaterThan = "GreaterThan";
                            public const double GreaterThan_SortOrder = 3;
                            public const string GreaterThanOrEqual = "GreaterThanOrEqual";
                            public const double GreaterThanOrEqual_SortOrder = 4;
                            public const string LessThan = "LessThan";
                            public const double LessThan_SortOrder = 5;
                            public const string LessThanOrEqual = "LessThanOrEqual";
                            public const double LessThanOrEqual_SortOrder = 6;
                        }

                        public const string SnapMath = "SnapMath"; // library
                        public static class SnapMath_
                        {
                            // Instructions
                            public const string Add = "Add";
                            public const double Add_SortOrder = 1;
                            public const string Subtract = "Subtract";
                            public const double Subtract_SortOrder = 2;
                            public const string Multiply = "Multiply";
                            public const double Multiply_SortOrder = 3;
                            public const string Divide = "Divide";
                            public const double Divide_SortOrder = 4;
                            public const string ChooseNumber = "ChooseNumber";
                            public const double ChooseNumber_SortOrder = 5;
                        }
                    }
                }
            }
        }
    }
    public static class Runtime
    {
        public static class Snap
        {
            public static class GroupExecutors
            {
                public const string LD = Workbench.Documents.PageEditor_.InstructionGroupItems.LD;
            }
        }
    }
    public static class Instructions
    {
        public static class ContextMenu
        {
            public const string Delete = "Delete";
        }
        public static class Parallel
        {
            public static class ContextMenu
            {
                public const string AppendBranch = "AppendBranch";
                public const string RemoveEmptyBranches = "RemoveEmptyBranches";
            }
        }
    }
}
