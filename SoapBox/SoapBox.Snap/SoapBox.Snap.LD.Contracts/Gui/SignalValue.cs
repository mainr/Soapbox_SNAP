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
using SoapBox.Core;
using SoapBox.Utilities;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using SoapBox.Protocol.Base;
using SoapBox.Protocol.Automation;

namespace SoapBox.Snap.LD
{
    public class SignalValue : AbstractSignalValueTextBlock
    {
        public SignalValue(INodeWrapper nodeItem, NodeSignal initialSignal, double maxWidth, TextAlignment textAlignment, string formatString)
        {
            Signal = initialSignal;
            NodeItem = nodeItem;
            MaxWidth = maxWidth;
            TextAlignment = textAlignment;
            FormatString = formatString;
        }

        protected override SignalValueTextBlock buildSignalValueTextBlock()
        {
            var newSignalValueTextBlock = new SignalValueTextBlock();
            newSignalValueTextBlock.Background = new SolidColorBrush(Colors.Transparent);
            newSignalValueTextBlock.MaxHeight = double.PositiveInfinity;
            return newSignalValueTextBlock;
        }

    }
}
