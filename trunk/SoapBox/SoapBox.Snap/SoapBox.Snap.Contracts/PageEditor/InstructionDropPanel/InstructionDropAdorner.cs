#region "SoapBox.Snap License"
/// <header module="SoapBox.Snap"> 
/// Copyright (C) 2009-2016 SoapBox Automation, All Rights Reserved.
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
using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;

namespace SoapBox.Snap
{
    /// <summary>
    /// Implements a UI class that can show the user
    /// where a drop will happen during a drag & drop
    /// </summary>
    class InstructionDropAdorner : Adorner
    {

        private AbstractInstructionDropPanel m_panel = null;

        public InstructionDropAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            if (adornedElement == null)
            {
                throw new ArgumentNullException("adornedElement");
            }
            m_panel = adornedElement as AbstractInstructionDropPanel;
            if (m_panel == null)
            {
                throw new ArgumentOutOfRangeException("adornedElement");
            }

            Visibility = Visibility.Collapsed;

            m_panel.DragOver += new DragEventHandler(m_panel_DragOver);
            m_panel.Drop += new DragEventHandler(m_panel_Drop);
            m_panel.DragLeave += new DragEventHandler(m_panel_DragLeave);
            m_panel.PreviewMouseMove += new System.Windows.Input.MouseEventHandler(m_panel_PreviewMouseMove);

            IsHitTestVisible = false;
        }

        private Point m_mousePosition = new Point();

        void m_panel_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            m_mousePosition = e.GetPosition(m_panel);
        }

        void m_panel_DragLeave(object sender, DragEventArgs e)
        {
            Visibility = Visibility.Collapsed;
        }

        void m_panel_Drop(object sender, DragEventArgs e)
        {
            m_mousePosition = e.GetPosition(m_panel);
            IDataObject data = e.Data;
            Nullable<Point> closestDropPoint = getClosestPoint(m_panel.DropPoints, m_mousePosition);
            if (closestDropPoint.HasValue)
            {
                m_panel.DropAtPoint(data, closestDropPoint.Value);
            }
            Visibility = Visibility.Collapsed;
            e.Handled = true;
        }

        void m_panel_DragOver(object sender, DragEventArgs e)
        {
            if (!canDrop(e))
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
                Visibility = Visibility.Collapsed;
            }
            else
            {
                e.Handled = true;
                Visibility = Visibility.Visible;
            }
        }

        bool canDrop(DragEventArgs e)
        {
            m_mousePosition = e.GetPosition(m_panel);
            IDataObject data = e.Data;
            Nullable<Point> closestDropPoint = getClosestPoint(m_panel.DropPoints, m_mousePosition);
            if (closestDropPoint.HasValue)
            {
                return m_panel.CanDrop(data, closestDropPoint.Value);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns null if no points in the collection
        /// </summary>
        Nullable<Point> getClosestPoint(IEnumerable<Point> points, Point toPoint)
        {
            Nullable<Point> closestPoint = null;
            double distance = double.PositiveInfinity;
            foreach (Point p in points)
            {
                var thisDistance = Math.Sqrt(Math.Pow(p.X - toPoint.X, 2) + Math.Pow(p.Y - toPoint.Y, 2));
                if (thisDistance < distance)
                {
                    closestPoint = p;
                    distance = thisDistance;
                }
            }
            return closestPoint;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            SolidColorBrush renderBrush = new SolidColorBrush(Colors.Green);
            renderBrush.Opacity = 0.2;
            Pen renderPen = new Pen(new SolidColorBrush(Colors.Black), 1.5);
            double renderRadius = 5.0;

            Nullable<Point> closestDropPoint = getClosestPoint(m_panel.DropPoints, m_mousePosition);
            if (closestDropPoint.HasValue)
            {
                drawingContext.DrawEllipse(renderBrush, renderPen, closestDropPoint.Value, renderRadius, renderRadius);
            }

        }
    }
}
