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
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Controls;
using SoapBox.Utilities;

namespace SoapBox.Snap.Application
{
    /// <summary>
    /// Takes an IInstructionItemMeta as an a value and reads the bitmap from the
    /// resources file (specified by the SpriteType and SpriteName properties)
    /// and creates a WPF Image object and returns that.
    /// </summary>
    public class InstructionSpriteConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var meta = value as IInstructionItemMeta;
            if (meta != null)
            {
                var testSprite = ResourceHelper.GetResourceLookup<System.Drawing.Bitmap>(meta.SpriteType, meta.SpriteKey);
                if(testSprite != null)
                {
                    BitmapSource b = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                        testSprite.GetHbitmap(),
                        IntPtr.Zero,
                        Int32Rect.Empty,
                        System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                    var img = new Image();
                    img.Source = b;
                    img.ToolTip = ResourceHelper.GetResourceLookup<string>(meta.ToolTipType, meta.ToolTipKey);
                    return img;
                }
                else
                {
                    return meta.SpriteKey;
                }
            }
            else
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
