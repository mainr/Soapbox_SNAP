﻿#region "SoapBox.Snap License"
/// <header module="SoapBox.Snap"> 
/// </header>
#endregion
        
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using SoapBox.Protocol.Base;

namespace SoapBox.Snap.ArduinoRuntime
{
    [Export(SoapBox.Core.ExtensionPoints.Host.Views, typeof(ResourceDictionary))]
    public partial class RuntimeOptionsPadView : ResourceDictionary
    {
        public RuntimeOptionsPadView()
        {
            InitializeComponent();        
        }
    }
}
