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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Collections.ObjectModel;

namespace SoapBox.Snap
{
    public class AbstractInstructionDropPanel : Panel
    {
        public AbstractInstructionDropPanel()
            : base()
        {
            AllowDrop = true;
            this.Initialized += new EventHandler(AbstractInstructionDropPanel_Initialized);
        }

        void AbstractInstructionDropPanel_Initialized(object sender, EventArgs e)
        {
            if (!m_adornerAdded)
            {
                var myAdornerLayer = AdornerLayer.GetAdornerLayer(this);
                myAdornerLayer.Add(new InstructionDropAdorner(this));
                m_adornerAdded = true;
            }
        }

        private bool m_adornerAdded = false;

        public IEnumerable<Point> DropPoints { get; protected set; }
        public virtual void DropAtPoint(IDataObject data, Point dropPoint) { }
        public virtual bool CanDrop(IDataObject data, Point dropPoint) { return false; }
    }
}
