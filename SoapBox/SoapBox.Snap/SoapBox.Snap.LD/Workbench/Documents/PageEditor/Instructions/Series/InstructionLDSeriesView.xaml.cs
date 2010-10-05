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
using SoapBox.Core;

namespace SoapBox.Snap.LD
{
    [Export(SoapBox.Core.ExtensionPoints.Host.Views, typeof(ResourceDictionary))]
    public partial class InstructionLDSeriesView : ResourceDictionary
    {
        public InstructionLDSeriesView()
        {
            InitializeComponent();
        }

        private void itControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                var dp = sender as DockPanel;
                if(dp != null)
                {
                    var seriesInstruction = dp.DataContext as InstructionLDSeries;
                    if (seriesInstruction != null)
                    {
                        seriesInstruction.DeleteSelectedChildren();
                    }
                }
                e.Handled = true;
            }
        }

        #region " Track all series ListBoxes "
        //private Dictionary<ListBox, object> m_listboxes = new Dictionary<ListBox, object>();
        private Dictionary<object, ListBox> m_listboxes = new Dictionary<object, ListBox>();

        private void ListBox_Initialized(object sender, EventArgs e)
        {
            // We want to register a list of all the series listboxes so they can co-ordinate
            var lst = sender as ListBox;
            if (lst != null)
            {
                var dc = lst.DataContext;
                if (!m_listboxes.ContainsKey(dc))
                {
                    m_listboxes.Add(dc, lst);
                }
                else
                {
                    m_listboxes[dc] = lst;
                }
            }
        }
        #endregion

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // We should only allow one series instruction to have selections active at one time
            var lst = sender as ListBox;
            if (lst != null)
            {
                if (lst.SelectedItems.Count > 0)
                {
                    foreach (var dcIterator in m_listboxes.Keys)
                    {
                        var lstIterator = m_listboxes[dcIterator];
                        if (lst != lstIterator)
                        {
                            if (lstIterator.SelectedItems.Count > 0)
                            {
                                lstIterator.SelectedItems.Clear();
                            }
                        }
                    }
                }
            }
        }

        #region " Drag & Drop "
        // These can be shared by all controls that use this data template
        // because only one drag can be happening at once
        private Point m_startPoint;
        private bool m_dragging = false;
        private object m_sender = null;
        private List<IInstructionItem> m_draggedItems = null;

        private void ListBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            m_startPoint = e.GetPosition(null);
            m_sender = sender;
            var lst = sender as ListBox;
            if (lst != null)
            {
                m_draggedItems = new List<IInstructionItem>();
                foreach (var obj in lst.SelectedItems)
                {
                    var item = obj as IInstructionItem;
                    if (item != null)
                    {
                        m_draggedItems.Add(item);
                    }
                }
            }
        }

        private void ListBox_PreviewMouseMove(object sender, MouseEventArgs e)
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
            var lst = sender as ListBox;
            if (lst != null)
            {
                foreach (var item in m_draggedItems)
                {
                    item.IsSelected = true;
                }

                if (m_draggedItems.Count > 0)
                {
                    m_dragging = true;
                    var data = new DataObject(typeof(IEnumerable<IInstructionItem>),m_draggedItems);
                    if (data != null)
                    {
                        DragDropEffects de = DragDrop.DoDragDrop(lst, data, DragDropEffects.Move);
                    }
                    m_dragging = false;
                }
            }
        }
        #endregion

    }
}
