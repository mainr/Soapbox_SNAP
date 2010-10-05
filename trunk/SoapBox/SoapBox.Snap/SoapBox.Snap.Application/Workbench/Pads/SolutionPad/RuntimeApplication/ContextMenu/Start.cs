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

namespace SoapBox.Snap.Application.Workbench.Pads.SolutionPad.RuntimeApplication.ContextMenu
{
    [Export(ExtensionPoints.Workbench.Pads.SolutionPad.RuntimeApplicationItem.ContextMenu, typeof(IMenuItem))]
    class Start : AbstractMenuItem
    {
        public Start()
        {
            ID = Extensions.Workbench.Pads.SolutionPad_.RuntimeApplicationItem.ContextMenu.Start;
            Header = Resources.Strings.Solution_Pad_RuntimeApplicationItem_Start;
            ToolTip = Resources.Strings.Solution_Pad_RuntimeApplicationItem_Start_ToolTip;

            InsertRelativeToID = Extensions.Workbench.Pads.SolutionPad_.RuntimeApplicationItem.ContextMenu.Disconnect;
            BeforeOrAfter = RelativeDirection.After;

            SetIconFromBitmap(Resources.Images.RuntimeRunning);
        }

        protected override void Run()
        {
            base.Run();
            var rt = Context as SoapBox.Snap.Application.RuntimeApplicationItem;
            if (rt != null)
            {
                rt.Start();
            }
        }
    }
}
