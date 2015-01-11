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
using System.Windows.Media;
using System.Windows;
using System.Collections.ObjectModel;

namespace SoapBox.Snap.LD
{
    class LDParallelPanel : StackPanel
    {
        public LDParallelPanel()
            : base()
        {
            ClipToBounds = false;
        }

        protected override void OnRender(System.Windows.Media.DrawingContext dc)
        {
            base.OnRender(dc);
            m_lineInPoints = new Collection<Point>();
            m_lineOutPoints = new Collection<Point>();
            var children = InternalChildren;
            foreach (var child in children)
            {
                findRungPoints(child as UIElement);
            }
            double minY = ActualHeight;
            double maxInY = 0;
            double maxOutY = 0;
            foreach (var pt in m_lineInPoints)
            {
                minY = Math.Min(minY, pt.Y);
                maxInY = Math.Max(maxInY, pt.Y);
            }
            foreach (var pt in m_lineOutPoints)
            {
                //minY = Math.Min(minY, pt.Y);
                maxOutY = Math.Max(maxOutY, pt.Y);
            }

            Pen renderPen = new Pen(new SolidColorBrush(Colors.Black), 1.5);
            dc.DrawLine(renderPen, new Point(0, minY), new Point(0, maxInY));
            dc.DrawLine(renderPen, new Point(ActualWidth, minY), new Point(ActualWidth, maxOutY));
        }


        private Collection<Point> m_lineInPoints = null;
        private Collection<Point> m_lineOutPoints =null;

        private void findRungPoints(UIElement ctrl)
        {
            Nullable<Point> inPoint = getPoint(findRungInOut<LDRungIn>(ctrl));
            recordInPoint(inPoint);
            Nullable<Point> outPoint = getPoint(findRungInOut<LDRungOut>(ctrl));
            recordOutPoint(outPoint);
        }

        private void recordInPoint(Nullable<Point> p)
        {
            if (p.HasValue)
            {
                m_lineInPoints.Add(p.Value);
            }
        }

        private void recordOutPoint(Nullable<Point> p)
        {
            if (p.HasValue)
            {
                m_lineOutPoints.Add(p.Value);
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
                    if (child != null && !(child is LDParallelPanel))
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
