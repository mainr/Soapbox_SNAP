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

namespace SoapBox.Snap.Application
{
    [Export(SoapBox.Core.ExtensionPoints.Host.Views, typeof(ResourceDictionary))]
    public partial class SolutionPadView : ResourceDictionary
    {
        public SolutionPadView()
        {
            InitializeComponent();
        }
    }
}
