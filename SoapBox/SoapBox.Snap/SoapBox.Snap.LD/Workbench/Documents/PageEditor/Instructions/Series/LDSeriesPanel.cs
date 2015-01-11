#region "SoapBox.Snap License"
/// <header module="SoapBox.Snap"> 
/// Copyright (C) 2009-2015 SoapBox Automation, All Rights Reserved.
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
using System.Windows.Controls;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Data;
using SoapBox.Core;
using System.Windows.Media;
using System.Diagnostics;
using SoapBox.Utilities;

namespace SoapBox.Snap.LD
{
    /// <summary>
    /// A custom panel for drawing an LD Series element
    /// Implements drag & drop
    /// </summary>
    class LDSeriesPanel : AbstractInstructionDropPanel
    {
        public const double MIN_HEIGHT = 15;
        public const double MIN_WIDTH = 15;
        private const double VERTICAL_EMPTY_LINE_OFFSET = 3; // applies to nested branches
        private const double HEADER_WIDTH = 80;
        private const double NESTING_OFFSET = 20;
        private const double LEFT_RIGHT_MARGIN = 6;
        private const double LINE_SPACING = 3; // pixels between wrapped lines

        public LDSeriesPanel()
            : base()
        {
            var newDropPoints = new Collection<Point>();
            newDropPoints.Add(new Point());
            DropPoints = newDropPoints;
            ClipToBounds = false;

            this.Initialized += new EventHandler(LDSeriesPanel_Initialized);

        }

        void LDSeriesPanel_Initialized(object sender, EventArgs e)
        {
            // Determine if we're the main rung, or a nested rung inside a parallel branch
            parentPanel = UIHelper.TryFindParent<LDSeriesPanel>(this);
            if (parentPanel == null)
            {
                topSeriesElement = true;
            }
            else
            {
                parent = UIHelper.TryFindParent<ItemsControl>(this);
            }
            
            var element = UIHelper.TryFindParentByName<FrameworkElement>(this, AbstractEditorItem.PAGE_EDITOR_ROOT_NAME);
            if (element != null)
            {
                editorSize = new Size(element.ActualWidth, element.ActualHeight);
                element.SizeChanged += new SizeChangedEventHandler(editorRoot_SizeChanged);
                OnEditorSizeChanged();
            }

        }

        #region VerticalRungOffset
        public double VerticalRungOffset
        {
            get { return (double)GetValue(VerticalRungOffsetProperty); }
            set { SetValue(VerticalRungOffsetProperty, value); }
        }

        public static readonly DependencyProperty VerticalRungOffsetProperty =
            DependencyProperty.Register("VerticalRungOffset", typeof(double), typeof(LDSeriesPanel), new UIPropertyMetadata(0.0));
        #endregion

        #region VerticalRungOutOffset
        public double VerticalRungOutOffset
        {
            get { return (double)GetValue(VerticalRungOutOffsetProperty); }
            set { SetValue(VerticalRungOutOffsetProperty, value); }
        }

        public static readonly DependencyProperty VerticalRungOutOffsetProperty =
            DependencyProperty.Register("VerticalRungOutOffset", typeof(double), typeof(LDSeriesPanel), new UIPropertyMetadata(0.0));
        #endregion

        private LDSeriesPanel parentPanel = null;
        private bool topSeriesElement = false;
        private FrameworkElement parent = null;

        /// <summary>
        /// Accounts for width variance due to nesting
        /// </summary>
        private double nestingOffset
        {
            get
            {
                if (topSeriesElement)
                {
                    return 0;
                }
                else
                {
                    return parentPanel.nestingOffset + NESTING_OFFSET;
                }
            }
        }

        private double widthLimit
        {
            get
            {
                return Math.Max(editorSize.Width - HEADER_WIDTH - nestingOffset, 0);
            }
        }

        private Size editorSize = new Size();

        void editorRoot_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            editorSize = e.NewSize;
            OnEditorSizeChanged();
        }

        void OnEditorSizeChanged()
        {
            this.InvalidateMeasure();
            this.InvalidateArrange();
            if (topSeriesElement)
            {
                MinWidth = widthLimit;
            }
            else
            {
                MinWidth = MIN_WIDTH;
            }
        }

        private Dictionary<Point, int> m_dropPointLookup = new Dictionary<Point, int>();

        public override bool CanDrop(IDataObject data, Point dropPoint)
        {
            var instructionItem = DataContext as InstructionLDSeries;
            if (instructionItem != null
                && data.GetDataPresent(typeof(IInstructionItem)))
            {
                //Incoming instruction from the instruction Pad
                var ldInstructionItem = data.GetData(typeof(IInstructionItem)) as ILDInstructionItem;
                if (ldInstructionItem != null)
                {
                    var instructionCollection = new Collection<ILDInstructionItem>();
                    instructionCollection.Add(ldInstructionItem);
                    return instructionItem.CanDrop(instructionCollection, m_dropPointLookup[dropPoint]);
                }
                else
                {
                    return false;
                }
            }
            else if (instructionItem != null
                && data.GetDataPresent(typeof(IEnumerable<IInstructionItem>)))
            {
                //Probably a drag and drop between rungs
                var draggedItems = data.GetData(typeof(IEnumerable<IInstructionItem>)) as IEnumerable<IInstructionItem>;
                if (draggedItems != null)
                {
                    var instructionCollection = new Collection<ILDInstructionItem>();
                    foreach (var instructionItemIterator in draggedItems)
                    {
                        var ldInstructionItem = instructionItemIterator as ILDInstructionItem;
                        if (ldInstructionItem != null)
                        {
                            instructionCollection.Add(ldInstructionItem);
                        }
                    }
                    return instructionItem.CanDrop(instructionCollection, m_dropPointLookup[dropPoint]);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public override void DropAtPoint(IDataObject data, Point dropPoint)
        {
            base.DropAtPoint(data, dropPoint);
            var instructionItem = DataContext as InstructionLDSeries;
            if (instructionItem != null)
            {
                if (data.GetDataPresent(typeof(IInstructionItem)))
                {
                    //Drop from instruction pad
                    var ldInstructionItem = data.GetData(typeof(IInstructionItem)) as ILDInstructionItem;
                    if (ldInstructionItem != null)
                    {
                        var instructionCollection = new Collection<ILDInstructionItem>();
                        instructionCollection.Add(ldInstructionItem);
                        instructionItem.Drop(instructionCollection, m_dropPointLookup[dropPoint]);
                    }
                }
                else if (data.GetDataPresent(typeof(IEnumerable<IInstructionItem>)))
                {
                    //Drag and drop between rungs
                    var draggedItems = data.GetData(typeof(IEnumerable<IInstructionItem>)) as IEnumerable<IInstructionItem>;
                    if (draggedItems != null)
                    {
                        var instructionCollection = new Collection<ILDInstructionItem>();
                        foreach (var instructionItemIterator in draggedItems)
                        {
                            var ldInstructionItem = instructionItemIterator as ILDInstructionItem;
                            if (ldInstructionItem != null)
                            {
                                instructionCollection.Add(ldInstructionItem);
                            }
                        }
                        instructionItem.Drop(instructionCollection, m_dropPointLookup[dropPoint]);
                    }
                }
            }
        }

        /// <summary>
        /// Measures all child elements and figures out how much
        /// space we would like to have
        /// </summary>
        protected override Size MeasureOverride(Size constraint)
        {
            Size myConstraint = constraint;
            myConstraint.Width = widthLimit;
            var myChildren = InternalChildren;
            var panelSize = new Size();
            var lineSize = new Size();
            double maxChildWidth = 0;
            double totalChildWidth = 0;
            bool isRight = false;
            double maxRungOffsetThisLine = 0;
            var verticalOffsetsAndHeights = new Collection<Tuple<double, double>>(); // stores verticalrungoffset and child height
            bool firstLine = true;

            for (int iChild = 0; iChild < myChildren.Count; iChild++)
            {
                var child = myChildren[iChild] as UIElement;
                if (child != null)
                {
                    double childVerticalRungOffset = 0;
                    var ctrl = child as Control;
                    if (ctrl != null)
                    {
                        var instructionItem = ctrl.DataContext as ILDInstructionItem;
                        if (instructionItem != null)
                        {
                            childVerticalRungOffset = instructionItem.VerticalRungOffset;
                            if (instructionItem.IsRight)
                            {
                                isRight = true;
                            }
                        }
                    }
                    child.Measure(myConstraint);
                    var childSize = child.DesiredSize;
                    maxChildWidth = Math.Max(maxChildWidth, childSize.Width);
                    totalChildWidth += childSize.Width;
                    if (lineSize.Width + childSize.Width + (2 * LEFT_RIGHT_MARGIN) > myConstraint.Width)
                    {
                        // new line - may have to add in extra height due to rung offsets
                        foreach (var tpl in verticalOffsetsAndHeights)
                        {
                            var verticalRungOffset = tpl.Item1;
                            var childHeight = tpl.Item2;
                            var offsetFromTop = maxRungOffsetThisLine - verticalRungOffset;
                            lineSize.Height = Math.Max(lineSize.Height, childHeight + offsetFromTop);
                        }
                        // switch to a new line
                        panelSize.Width = Math.Max(panelSize.Width, lineSize.Width + (2 * LEFT_RIGHT_MARGIN));
                        // remember first child's vertical rung offset
                        if (firstLine)
                        {
                            VerticalRungOffset = maxRungOffsetThisLine;
                            firstLine = false;
                        }
                        panelSize.Height += lineSize.Height + LINE_SPACING;
                        lineSize = childSize;
                        maxRungOffsetThisLine = childVerticalRungOffset;
                        verticalOffsetsAndHeights.Clear();
                        verticalOffsetsAndHeights.Add(Tuple.Create(childVerticalRungOffset, childSize.Height));
                    }
                    else
                    {
                        // goes on the same line
                        lineSize.Width += childSize.Width;
                        lineSize.Height = Math.Max(lineSize.Height, childSize.Height);
                        maxRungOffsetThisLine = Math.Max(maxRungOffsetThisLine, childVerticalRungOffset);
                        verticalOffsetsAndHeights.Add(Tuple.Create(childVerticalRungOffset, childSize.Height));
                   }
                }
            }

            // may have to add in extra height due to rung offsets
            foreach (var tpl in verticalOffsetsAndHeights)
            {
                var verticalRungOffset = tpl.Item1;
                var childHeight = tpl.Item2;
                var offsetFromTop = maxRungOffsetThisLine - verticalRungOffset;
                lineSize.Height = Math.Max(lineSize.Height, childHeight + offsetFromTop);
            }
            panelSize.Width = Math.Max(panelSize.Width, lineSize.Width + (2 * LEFT_RIGHT_MARGIN));
            // remember first child's vertical rung offset
            if (firstLine)
            {
                VerticalRungOffset = maxRungOffsetThisLine;
                firstLine = false;
            }
            panelSize.Height += lineSize.Height + LINE_SPACING;

            if (isRight && topSeriesElement)
            {
                panelSize.Width = myConstraint.Width;
            }

            if (!topSeriesElement)
            {
                if (isRight)
                {
                    HorizontalAlignment = HorizontalAlignment.Right;
                }
                else
                {
                    HorizontalAlignment = HorizontalAlignment.Left;
                }
                MinWidth = Math.Max(MIN_WIDTH, parent.ActualWidth - 3);
            }

            return panelSize;
        }

        /// <summary>
        /// Given how much space we actually have, it gives
        /// a certain amount of space to each child element,
        /// and positions each element in the Canvas.
        /// </summary>
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            Size myArrangeSize = arrangeSize;
            myArrangeSize.Width = widthLimit;
            m_dropPointLookup = new Dictionary<Point, int>();
            var myChildren = InternalChildren;
            var totalSize = new Size();
            var lineSize = new Size();
            int lineStartIndex = 0;
            Size arrangeLineSize;
            m_linePoints = new Collection<Point>();

            for (int iChild = 0; iChild < myChildren.Count; iChild++)
            {
                var child = myChildren[iChild] as UIElement;
                if (child != null)
                {
                    var childSize = child.DesiredSize;
                    if (lineSize.Width + childSize.Width + (2 * LEFT_RIGHT_MARGIN) > myArrangeSize.Width)
                    {
                        // switch to a new line
                        arrangeLineSize = arrangeLine(totalSize.Height, lineSize.Height, lineStartIndex, iChild - 1, myArrangeSize.Width);
                        totalSize.Height += arrangeLineSize.Height + LINE_SPACING;
                        totalSize.Width = Math.Max(totalSize.Width, arrangeLineSize.Width);
                        lineSize = childSize;
                        lineStartIndex = iChild;
                    }
                    else
                    {
                        // goes on the same line
                        lineSize.Width += childSize.Width;
                        lineSize.Height = Math.Max(lineSize.Height, childSize.Height);
                    }
                }
            }

            if (lineStartIndex < myChildren.Count)
            {
                arrangeLineSize = arrangeLine(totalSize.Height, lineSize.Height, lineStartIndex, myChildren.Count - 1, myArrangeSize.Width);
                totalSize.Height += arrangeLineSize.Height + LINE_SPACING;
                totalSize.Width = Math.Max(totalSize.Width, arrangeLineSize.Width);
            }

            totalSize.Width = Math.Max(totalSize.Width, MinWidth);
            totalSize.Height = Math.Max(totalSize.Height, MinHeight);

            if (m_dropPointLookup.Count == 0)
            {
                m_dropPointLookup.Add(new Point(LEFT_RIGHT_MARGIN, VERTICAL_EMPTY_LINE_OFFSET), 0);
            }
            // make sure we have one at the very end
            var lastDropPoint = m_dropPointLookup.Last();
            var endDropPoint = new Point(totalSize.Width - LEFT_RIGHT_MARGIN, lastDropPoint.Key.Y);
            if(!m_dropPointLookup.ContainsKey(endDropPoint))
            {
                m_dropPointLookup.Add(endDropPoint, lastDropPoint.Value);
            }

            DropPoints = m_dropPointLookup.Keys;

            return totalSize;
        }

        /// <summary>
        /// Returns the line width, actual
        /// </summary>
        private Size arrangeLine(double top, double lineHeight, int startIndex, int endIndex, double lineWidth)
        {
            Size actualSize = new Size();
            Control ctrl = null; ;
            bool firstPoint = false; // registered starting point of first instruction
            double maxRungOffset = 0;
            var myChildren = InternalChildren;
            // First, find the max rung offset
            for (int i = startIndex; i <= endIndex; i++)
            {
                var child = myChildren[i];
                ctrl = child as Control;
                if (ctrl != null)
                {
                    var instructionItem = ctrl.DataContext as ILDInstructionItem;
                    if (instructionItem != null)
                    {
                        maxRungOffset = Math.Max(maxRungOffset, instructionItem.VerticalRungOffset);
                    }
                }
            }
            // Next, arrange all the child UI elements
            double maxOffsetFromTop = 0;
            double left = LEFT_RIGHT_MARGIN;
            for (int i = startIndex; i <= endIndex; i++)
            {
                var child = myChildren[i];
                double rungOffset = 0;
                ctrl = child as Control;
                if (ctrl != null)
                {
                    var instructionItem = ctrl.DataContext as ILDInstructionItem;
                    if (instructionItem != null)
                    {
                        rungOffset = instructionItem.VerticalRungOffset;
                        if (instructionItem.IsRight && i == endIndex)
                        {
                            left = MinWidth - child.DesiredSize.Width - LEFT_RIGHT_MARGIN; // right align it
                        }
                    }
                }
                double offsetFromTop = 0;
                if (rungOffset < 0)
                {
                    offsetFromTop = 0;
                }
                else
                {
                    offsetFromTop = maxRungOffset - rungOffset;
                }
                maxOffsetFromTop = Math.Max(maxOffsetFromTop, offsetFromTop);
                actualSize.Height = Math.Max(actualSize.Height, offsetFromTop + child.DesiredSize.Height);
                child.Arrange(new Rect(left, top + offsetFromTop, child.DesiredSize.Width, child.DesiredSize.Height));
                if (!firstPoint)
                {
                    double X = 0;
                    if (top > 0)
                    {
                        X = 2;
                    }
                    recordPoint(getPointWithX(findRungInOut<LDRungIn>(ctrl), X));
                    firstPoint = true;
                }
                findRungPoints(child, i);
                left += child.DesiredSize.Width;
            }
            left += LEFT_RIGHT_MARGIN;
            if (ctrl != null)
            {
                if (endIndex < (InternalChildren.Count - 1))
                {
                    var lastPoint = getPoint(findRungInOut<LDRungOut>(ctrl));
                    if (lastPoint.HasValue)
                    {
                        recordPoint(new Point(lastPoint.Value.X + LEFT_RIGHT_MARGIN, lastPoint.Value.Y)); // to the right
                        recordPoint(new Point(lastPoint.Value.X + LEFT_RIGHT_MARGIN, top + lineHeight + maxOffsetFromTop)); // down
                    }
                    recordPoint(new Point(2, top + lineHeight + maxOffsetFromTop)); // back left
                }
                else
                {
                    // last element, connect it to the right side
                    var pt = getPointWithX(findRungInOut<LDRungIn>(ctrl), lineWidth);
                    if (pt.HasValue)
                    {
                        VerticalRungOutOffset = pt.Value.Y;
                    }
                    else
                    {
                        VerticalRungOutOffset = 0;
                    }
                }
            }
            actualSize.Width = left;
            return actualSize;
        }

        private void registerDropPoint(double left, double top, int index)
        {
            var pt = new Point(left, top);
            if (!m_dropPointLookup.ContainsKey(pt))
            {
                m_dropPointLookup.Add(pt, index);
            }
        }

        private Collection<Point> m_linePoints = new Collection<Point>();

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            Pen renderPen;
            renderPen = new Pen(new SolidColorBrush(Colors.Black), 1.5);
            renderPen.EndLineCap = PenLineCap.Round;

            var moreLinePoints = new Collection<Point>();
            if (m_linePoints.Count == 0)
            {
                moreLinePoints.Add(new Point(0, VERTICAL_EMPTY_LINE_OFFSET));
                moreLinePoints.Add(new Point(this.ActualWidth, VERTICAL_EMPTY_LINE_OFFSET));
            }
            else
            {
                var firstPoint = m_linePoints.First();
                if (!moreLinePoints.Contains(new Point(0, firstPoint.Y)))
                {
                    moreLinePoints.Add(new Point(0, firstPoint.Y));
                }
                foreach (var pt in m_linePoints)
                {
                    moreLinePoints.Add(pt);
                }
                var lastPoint = m_linePoints.Last();
                moreLinePoints.Add(new Point(this.ActualWidth, lastPoint.Y));
            }

            Point last = new Point();
            bool skipFirst = false;
            foreach (var pt in moreLinePoints)
            {
                if (skipFirst)
                {
                    dc.DrawLine(renderPen, last, pt);
                }
                else
                {
                    skipFirst = true;
                }
                last = pt;
            }
        }

        private void findRungPoints(UIElement ctrl, int index)
        {
            Nullable<Point> inPoint = getPoint(findRungInOut<LDRungIn>(ctrl));
            recordPoint(inPoint);
            Nullable<Point> outPoint = getPoint(findRungInOut<LDRungOut>(ctrl));
            if (inPoint.HasValue && outPoint.HasValue)
            {
                outPoint = new Point(outPoint.Value.X, inPoint.Value.Y); // rung should be straight

                // add drop points before and after each element
                registerDropPoint(inPoint.Value.X, inPoint.Value.Y, index);
                registerDropPoint(outPoint.Value.X, outPoint.Value.Y, index + 1);
            }
            recordPoint(outPoint);
        }

        private void recordPoint(Nullable<Point> p)
        {
            if (p.HasValue)
            {
                m_linePoints.Add(p.Value);
            }
        }

        private Nullable<Point> getPointWithX<T>(T rungPoint, double X) where T : UIElement
        {
            Nullable<Point> p = getPoint<T>(rungPoint);
            if (p.HasValue)
            {
                return new Point(X, p.Value.Y);
            }
            else
            {
                return null;
            }
        }

        private Nullable<Point> getPointWithY<T>(T rungPoint, double Y) where T : UIElement
        {
            Nullable<Point> p = getPoint<T>(rungPoint);
            if (p.HasValue)
            {
                return new Point(p.Value.X, Y);
            }
            else
            {
                return null;
            }
        }

        private Nullable<Point> getPoint<T>(T rungPoint) where T : UIElement
        {
            Point screenPoint;
            if (rungPoint != null)
            {
                screenPoint = rungPoint.PointToScreen(new Point());
                return this.PointFromScreen(screenPoint);
            }
            else
            {
                return null;
            }
        }

        private T findRungInOut<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                    if (child != null && !(child is LDSeriesPanel))
                    {
                        if (child is T)
                        {
                            return (T)child;
                        }

                        T childOfChild = findRungInOut<T>(child);
                        if (childOfChild != null)
                        {
                            return childOfChild;
                        }
                    }
                }
            }
            return null;
        }
    }
}
