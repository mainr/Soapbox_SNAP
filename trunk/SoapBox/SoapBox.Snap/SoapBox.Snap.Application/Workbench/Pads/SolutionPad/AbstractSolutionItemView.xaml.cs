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
using SoapBox.Core;
using System.Windows.Input;
using SoapBox.Protocol.Base;
using SoapBox.Utilities;

namespace SoapBox.Snap.Application
{
    [Export(SoapBox.Core.ExtensionPoints.Host.Views, typeof(ResourceDictionary))]
    public partial class AbstractSolutionItemView : ResourceDictionary
    {
        public AbstractSolutionItemView()
        {
            InitializeComponent();
            
        }

        Dictionary<FrameworkElement, bool> m_rememberIsSelected = new Dictionary<FrameworkElement, bool>();

        private static DateTime m_clickTime;

        private void FrameworkElement_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var header = sender as FrameworkElement;
            if (header != null)
            {
                AbstractSolutionItem solutionItem = header.DataContext as AbstractSolutionItem;
                if (solutionItem != null)
                {
                    if (e.ClickCount == 2)
                    {
                        solutionItem.Open();
                        e.Handled = true;
                    }
                }
            }
        }

        private void TextBlock_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var header = sender as FrameworkElement;
            if (header != null)
            {
                TreeViewItem tvi = UIHelper.TryFindParent<TreeViewItem>(header);
                if (tvi != null)
                {
                    if (m_rememberIsSelected.ContainsKey(header))
                    {
                        m_rememberIsSelected[header] = tvi.IsSelected;
                    }
                    else
                    {
                        m_rememberIsSelected.Add(header, tvi.IsSelected);
                        AbstractSolutionItem solutionItem = header.DataContext as AbstractSolutionItem;
                        if (solutionItem != null)
                        {
                            tvi.KeyDown += new KeyEventHandler(solutionItem.KeyDown);
                        }
                    }
                    if (!tvi.IsSelected)
                    {
                        m_clickTime = DateTime.Now;
                    }
                }
            }
        }

        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var header = sender as FrameworkElement;
            if (header != null)
            {
                AbstractSolutionItem solutionItem = header.DataContext as AbstractSolutionItem;
                if (solutionItem != null)
                {
                    if (DateTime.Now.Subtract(m_clickTime).Milliseconds >= 400)
                    {
                        if (m_rememberIsSelected.ContainsKey(header)
                            && m_rememberIsSelected[header])
                        {
                            solutionItem.HeaderBeingEdited = true;
                            TextBox txtHeaderEdit = UIHelper.TryFindSibling<TextBox>(header);
                            if (txtHeaderEdit != null)
                            {
                                txtHeaderEdit.Focus();
                            }
                        }
                    }
                }
            }
        }

        private void TextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox headerEdit = sender as TextBox;
            if (headerEdit != null)
            {
                AbstractSolutionItem solutionItem = headerEdit.DataContext as AbstractSolutionItem;
                if (solutionItem != null && solutionItem.HeaderBeingEdited)
                {
                    solutionItem.HeaderEditAccept();
                }
            }
        }

        private void TextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            TextBox headerEdit = sender as TextBox;
            if (headerEdit != null)
            {
                AbstractSolutionItem solutionItem = headerEdit.DataContext as AbstractSolutionItem;
                if (solutionItem != null)
                {
                    if (e.Key == System.Windows.Input.Key.Enter)
                    {
                        solutionItem.HeaderEditAccept();
                    }
                    else if (e.Key == System.Windows.Input.Key.Escape)
                    {
                        solutionItem.HeaderEditCancel();
                    }
                }
            }
        }

        #region " Drag and Drop "

        // These can be shared by all controls that use this data template
        // because only one drag can be happening at once
        private Point m_startPoint;
        private bool m_dragging = false;
        private object m_sender = null;

        /// <summary>
        /// Remember where we started to drag
        /// </summary>
        private void stackPanel_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            m_startPoint = e.GetPosition(null);
            m_sender = sender;
        }

        /// <summary>
        /// Detect a drag by seeing if we've dragged far enough
        /// </summary>
        private void stackPanel_PreviewMouseMove(object sender, MouseEventArgs e)
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
            var stackPanel = sender as StackPanel;
            if (stackPanel != null)
            {
                var solutionItem = stackPanel.DataContext as ISolutionItem;
                if (solutionItem != null && !solutionItem.HeaderBeingEdited && solutionItem.CanDrag())
                {
                    m_dragging = true;
                    var data = solutionItem.Drag();
                    if (data != null)
                    {
                        DragDropEffects de = DragDrop.DoDragDrop(stackPanel, data, DragDropEffects.Move);
                    }
                    m_dragging = false;
                }
            }
        }

        /// <summary>
        /// Give visual feedback if we're over a valid drop target
        /// </summary>
        private void stackPanel_DragOver(object sender, DragEventArgs e)
        {
            IDataObject data = e.Data;
            var stackPanel = sender as StackPanel;
            if (stackPanel != null)
            {
                var solutionItem = stackPanel.DataContext as ISolutionItem;
                if (solutionItem != null)
                {
                    if(!solutionItem.CanDrop(e.Data))
                    {
                        e.Effects = DragDropEffects.None;
                        e.Handled = true;
                    }
                }
            }
        }

        /// <summary>
        /// Handle the actual drop
        /// </summary>
        private void stackPanel_Drop(object sender, DragEventArgs e)
        {
            IDataObject data = e.Data;
            var stackPanel = sender as StackPanel;
            if (stackPanel != null)
            {
                var solutionItem = stackPanel.DataContext as ISolutionItem;
                if (solutionItem != null)
                {
                    solutionItem.Drop(data);
                }
            }
        }

        #endregion

    }
}
