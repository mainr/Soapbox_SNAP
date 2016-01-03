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
using SoapBox.Utilities;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace SoapBox.Snap.LD
{
    public class SignalDescriptionEditor : AbstractEditableTextBlock
    {
        public SignalDescriptionEditor(string initialText, double maxWidth, double maxHeight, TextAlignment textAlignment, bool editable)
            : base()
        {
            Text = initialText;
            MaxWidth = maxWidth;
            MaxHeight = maxHeight;
            TextAlignment = textAlignment;
            Editable = editable;
        }

        protected override EditableTextBlock buildEditableTextBlock()
        {
            var newEditableTextBlock = new EditableTextBlock();

            newEditableTextBlock.TextWrapping = TextWrapping.WrapWithOverflow;
            newEditableTextBlock.AcceptsReturn = true;
            newEditableTextBlock.Background = new SolidColorBrush(Colors.LightBlue);
            newEditableTextBlock.Padding = new Thickness(5, 0, 5, 0);
            newEditableTextBlock.Editable = true;
            return newEditableTextBlock;
        }

    }
}
