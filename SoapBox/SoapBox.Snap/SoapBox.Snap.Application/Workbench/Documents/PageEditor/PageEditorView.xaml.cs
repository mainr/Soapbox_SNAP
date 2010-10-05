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
using System.Diagnostics;
using System.ComponentModel;
using SoapBox.Snap.MarkdownUtility;
using SoapBox.Core;
using System.Windows.Media;
using System.Windows.Documents;

namespace SoapBox.Snap.Application
{
    [Export(SoapBox.Core.ExtensionPoints.Host.Views, typeof(ResourceDictionary))]
    public partial class PageEditorView : ResourceDictionary
    {
        public PageEditorView()
        {
            InitializeComponent();
        }

        private void DockPanel_Undo(object sender, ExecutedRoutedEventArgs e)
        {
            var dp = sender as DockPanel;
            if (dp != null)
            {
                var dc = dp.DataContext as PageEditor;
                if (dc != null)
                {
                    dc.Undo();
                }
            }
        }

        private void DockPanel_Redo(object sender, ExecutedRoutedEventArgs e)
        {
            var dp = sender as DockPanel;
            if (dp != null)
            {
                var dc = dp.DataContext as PageEditor;
                if (dc != null)
                {
                    dc.Redo();
                }
            }
        }

        /// <summary>
        /// Unfortunately the SelectedItems property of a ListView is not a dependency property,
        /// so we have to manually copy the selection status to the viewmodel when the 
        /// selection changes.  Not a big deal...
        /// </summary>
        private void pageEditor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var ctrl = sender as ListView;
            if (ctrl != null)
            {
                var vm = ctrl.DataContext as SoapBox.Snap.Application.PageEditor.PageEditorItem;
                if (vm != null)
                {
                    foreach (INodeWrapper nodeWrapper in vm.Items)
                    {
                        nodeWrapper.IsSelected = ctrl.SelectedItems.Contains(nodeWrapper);
                    }
                }
            }
        }




    }
}
