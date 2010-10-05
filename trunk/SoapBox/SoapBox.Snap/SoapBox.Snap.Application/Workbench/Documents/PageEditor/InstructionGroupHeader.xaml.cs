#region "SoapBox.Snap License"
/// <header module="SoapBox.Snap"> 
/// </header>
#endregion
        
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SoapBox.Core;

namespace SoapBox.Snap.Application
{
    /// <summary>
    /// Interaction logic for InstructionGroupHeader.xaml
    /// </summary>
    public partial class InstructionGroupHeader : UserControl
    {
        public InstructionGroupHeader()
        {
            InitializeComponent();
        }

        #region " Drag & Drop "
        // These can be shared by all controls that use this data template
        // because only one drag can be happening at once
        private Point m_startPoint;
        private bool m_dragging = false;
        private object m_sender = null;
        private List<IInstructionGroupItem> m_draggedItems = null;

        private void Header_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            m_startPoint = e.GetPosition(null);
            m_sender = sender;
            var bdr = sender as Border;
            if (bdr != null)
            {
                var lst = UIHelper.TryFindParent<ListView>(bdr);
                if (lst != null)
                {
                    m_draggedItems = new List<IInstructionGroupItem>();
                    foreach (var obj in lst.SelectedItems)
                    {
                        var item = obj as IInstructionGroupItem;
                        if (item != null)
                        {
                            m_draggedItems.Add(item);
                        }
                    }
                }
            }
        }

        private void Header_PreviewMouseMove(object sender, MouseEventArgs e)
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
            var bdr = sender as Border;
            if (bdr != null)
            {
                foreach (var item in m_draggedItems)
                {
                    item.IsSelected = true; // re-select, as the list tends to deselect them when dragging starts
                }

                if (m_draggedItems.Count > 0)
                {
                    m_dragging = true;
                    var data = new DataObject(typeof(IEnumerable<IInstructionGroupItem>), m_draggedItems);
                    if (data != null)
                    {
                        DragDropEffects de = DragDrop.DoDragDrop(bdr, data, DragDropEffects.Move);
                    }
                    m_dragging = false;
                }
            }
        }

        // Since mouse move events all stop firing during a drag & drop operation, I had to divide the
        // header up into two parts, a top and a bottom.  These are separate text boxes, though the bottom
        // one is invisible.  So, you can use DragOver and DragLeave to keep track of which one we're over.
        private bool m_draggingOverTop = false;
        private bool m_draggingOverBottom = false;

        private void Header_DragOver(object sender, DragEventArgs e)
        {
            IDataObject data = e.Data;
            bool canDrop = false;
            if (data != null && data.GetDataPresent(typeof(IEnumerable<IInstructionGroupItem>)))
            {
                canDrop = !IsSelected;
            }

            m_draggingOverTop = false;
            m_draggingOverBottom = false;
            if (!canDrop)
            {
                e.Effects = DragDropEffects.None;
            }
            else
            {
                var txt = sender as TextBlock;
                if (txt == HeaderTextBlock)
                {
                    m_draggingOverTop = true;
                }
                else
                {
                    m_draggingOverBottom = true;
                }
            }
            updateVisualFeedback();
            e.Handled = true;
        }

        private void Header_DragLeave(object sender, DragEventArgs e)
        {
            m_draggingOverTop = false;
            m_draggingOverBottom = false;
            updateVisualFeedback();
        }

        private void updateVisualFeedback()
        {
            if (m_draggingOverTop)
            {
                TopLine.Visibility = Visibility.Visible;
                BottomLine.Visibility = Visibility.Hidden;
            }
            else if (m_draggingOverBottom)
            {
                TopLine.Visibility = Visibility.Hidden;
                BottomLine.Visibility = Visibility.Visible;
            }
            else
            {
                TopLine.Visibility = Visibility.Hidden;
                BottomLine.Visibility = Visibility.Hidden;
            }
        }

        private void Header_Drop(object sender, DragEventArgs e)
        {
            IDataObject data = e.Data;

            var txt = sender as TextBlock;
            if (txt != null)
            {
                var instructionGroup = txt.DataContext as IInstructionGroupItem;
                if (instructionGroup != null)
                {
                    var pageEditor = instructionGroup.Parent as SoapBox.Snap.Application.PageEditor.PageEditorItem;
                    if (m_draggingOverTop)
                    {
                        pageEditor.MoveSelectedBefore(instructionGroup);
                    }
                    else if (m_draggingOverBottom)
                    {
                        pageEditor.MoveSelectedAfter(instructionGroup);
                    }
                }
            }

            m_draggingOverTop = false;
            m_draggingOverBottom = false;
            updateVisualFeedback();
            e.Handled = true;
        }

        #endregion

        #region " IsSelected "
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }
        private static readonly string m_IsSelectedName =
            Utilities.NotifyPropertyChangedHelper.GetPropertyName<InstructionGroupHeader>(o => o.IsSelected);
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(m_IsSelectedName, typeof(bool), typeof(InstructionGroupHeader), new UIPropertyMetadata(false, OnIsSelectedChanged));

        public static void OnIsSelectedChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            
            var hdr = (InstructionGroupHeader)source;
            if (hdr.IsSelected)
            {
                hdr.Header.SetResourceReference(Border.BackgroundProperty, "PageEditorListViewItemHeaderBrush");
                hdr.Header.BorderBrush = new SolidColorBrush(Colors.SlateBlue);
            }
            else
            {
                hdr.Header.SetResourceReference(Border.BackgroundProperty, string.Empty);
                hdr.Header.BorderBrush = null;
            }
        }
        #endregion

    }
}
