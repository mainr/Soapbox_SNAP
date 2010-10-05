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
using System.Windows.Controls;
using System.Windows.Input;

namespace SoapBox.Snap.Application
{
    [Export(SoapBox.Core.ExtensionPoints.Host.Views, typeof(ResourceDictionary))]
    public partial class InstructionPadView : ResourceDictionary
    {
        public InstructionPadView()
        {
            InitializeComponent();
        }

        // These can be shared by all controls that use this data template
        // because only one drag can be happening at once
        private Point m_startPoint;
        private bool m_dragging = false;
        private object m_sender = null;

        /// <summary>
        /// Remember where we started to drag
        /// </summary>
        private void ContentControl_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            m_startPoint = e.GetPosition(null);
            m_sender = sender;
        }

        /// <summary>
        /// Detect a drag by seeing if we've dragged far enough
        /// </summary>
        private void ContentControl_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && !m_dragging)
            {
                Point newPoint = e.GetPosition(null);

                if (Math.Abs(newPoint.X - m_startPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(newPoint.Y - m_startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    initiateDrag(m_sender, e);
                }
            }
        }

        /// <summary>
        /// Package up the data and send it off to the drag/drop subsystem
        /// </summary>
        private void initiateDrag(object sender, MouseEventArgs e)
        {
            var contentControl = sender as ContentControl;
            if (contentControl != null)
            {
                var lazyInstructionItem = contentControl.DataContext as Lazy<IInstructionItem, IInstructionItemMeta>;
                if (lazyInstructionItem != null)
                {
                    m_dragging = true;
                    var data = new DataObject(typeof(IInstructionItem), lazyInstructionItem.Value.Create(null,null));
                    if (data != null)
                    {
                        DragDropEffects de = DragDrop.DoDragDrop(contentControl, data, DragDropEffects.Move);
                    }
                    m_dragging = false;
                }
            }
        }
    }
}
