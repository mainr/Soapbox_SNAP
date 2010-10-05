#region "SoapBox.Snap License"
/// <header module="SoapBox.Snap"> 
/// </header>
#endregion
        
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace SoapBox.Snap.Application
{
    [Export(SoapBox.Core.ExtensionPoints.Host.Views, typeof(ResourceDictionary))]
    public partial class InstructionGroupDummyView : ResourceDictionary
    {
        public InstructionGroupDummyView()
        {
            InitializeComponent();
        }

    }
}
