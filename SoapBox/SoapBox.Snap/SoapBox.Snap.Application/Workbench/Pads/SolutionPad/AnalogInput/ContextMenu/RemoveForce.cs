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
using SoapBox.Core;
using System.ComponentModel.Composition;
using SoapBox.Protocol.Base;

namespace SoapBox.Snap.Application.Workbench.Pads.SolutionPad.AnalogInput.ContextMenu
{
    [Export(ExtensionPoints.Workbench.Pads.SolutionPad.AnalogInputItem.ContextMenu, typeof(IMenuItem))]
    class RemoveForce : AbstractMenuItem
    {
        public RemoveForce()
        {
            ID = Extensions.Workbench.Pads.SolutionPad_.AnalogInputItem.ContextMenu.RemoveForce;
            Header = Resources.Strings.Solution_Pad_AnalogInputItem_RemoveForce;
            ToolTip = Resources.Strings.Solution_Pad_AnalogInputItem_RemoveForce_ToolTip;

            InsertRelativeToID = Extensions.Workbench.Pads.SolutionPad_.AnalogInputItem.ContextMenu.Force;
            BeforeOrAfter = RelativeDirection.After;

            SetIconFromBitmap(Resources.Images.RemoveForce);
        }

        protected override void Run()
        {
            base.Run();
            var di = Context as SoapBox.Snap.Application.AnalogInputItem;
            if (di != null)
            {
                di.AnalogInput = di.AnalogInput.SetForced(new FieldBool(false));
            }
        }
    }
}
