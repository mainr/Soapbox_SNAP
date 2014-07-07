#region "SoapBox.Snap License"
/// <header module="SoapBox.Snap"> 
/// Copyright (C) 2009-2014 SoapBox Automation, All Rights Reserved.
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
using System.Windows;
using SoapBox.Protocol.Automation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.ComponentModel.Composition;
using System.Windows.Media;

namespace SoapBox.Snap
{
    /// <summary>
    /// This is a class for declaring attached behaviors to WPF
    /// objects, where the behaviors depend on a Signal Value.
    /// For instance, controlling the color of a UIElement via
    /// the value of a signal.
    /// </summary>
    [Export(SoapBox.Core.ExtensionPoints.Host.Void, typeof(object))]
    public class SignalValueBehavior
    {
        /// <summary>
        /// For MEF to call
        /// </summary>
        public SignalValueBehavior()
        {
        }

        #region " runtimeServiceSingleton "
        [Import(Services.Solution.RuntimeService, typeof(IRuntimeService))]
        private IRuntimeService runtimeService
        {
            get
            {
                IRuntimeService retVal;
                lock (m_runtimeService_Lock)
                {
                    retVal = m_runtimeService;
                }
                return retVal;
            }
            set
            {
                lock (m_runtimeService_Lock)
                {
                    m_runtimeService = value;
                }
            }
        }
        private static IRuntimeService m_runtimeService = null;
        private static object m_runtimeService_Lock = new object();

        private static IRuntimeService staticRuntimeService()
        {
            IRuntimeService retVal;
            lock (m_runtimeService_Lock)
            {
                retVal = m_runtimeService;
            }
            return retVal;
        }
        #endregion

        #region NodeItem
        public static DependencyProperty NodeItemProperty =
            DependencyProperty.RegisterAttached("NodeItem",
            typeof(INodeWrapper),
            typeof(SignalValueBehavior),
            new UIPropertyMetadata(null));
        public static void SetNodeItem(DependencyObject target, INodeWrapper value)
        {
            target.SetValue(SignalValueBehavior.NodeItemProperty, value);
        }
        public static INodeWrapper GetNodeItem(DependencyObject target)
        {
            return target.GetValue(SignalValueBehavior.NodeItemProperty) as INodeWrapper;
        }
        #endregion

        #region TrueColor
        public static DependencyProperty TrueColorProperty =
            DependencyProperty.RegisterAttached("TrueColor",
            typeof(Color),
            typeof(SignalValueBehavior),
            new UIPropertyMetadata(null));
        public static void SetTrueColor(DependencyObject target, Color value)
        {
            target.SetValue(SignalValueBehavior.TrueColorProperty, value);
        }
        public static Color GetTrueColor(DependencyObject target)
        {
            var testColor = target.GetValue(SignalValueBehavior.TrueColorProperty);
            return testColor != null ? (Color)testColor : Colors.Transparent;
        }
        #endregion

        #region FalseColor
        public static DependencyProperty FalseColorProperty =
            DependencyProperty.RegisterAttached("FalseColor",
            typeof(Color),
            typeof(SignalValueBehavior),
            new UIPropertyMetadata(null));
        public static void SetFalseColor(DependencyObject target, Color value)
        {
            target.SetValue(SignalValueBehavior.FalseColorProperty, value);
        }
        public static Color GetFalseColor(DependencyObject target)
        {
            var testColor = target.GetValue(SignalValueBehavior.FalseColorProperty);
            return testColor != null ? (Color)testColor : Colors.Transparent;
        }
        #endregion

        #region OnLoadedHandler
        public static DependencyProperty OnLoadedHandlerProperty =
            DependencyProperty.RegisterAttached("OnLoadedHandler",
            typeof(Action<object, RoutedEventArgs>),
            typeof(SignalValueBehavior),
            new UIPropertyMetadata(null));
        public static void SetOnLoadedHandler(DependencyObject target, Action<object, RoutedEventArgs> value)
        {
            target.SetValue(SignalValueBehavior.OnLoadedHandlerProperty, value);
        }
        public static Action<object, RoutedEventArgs> GetOnLoadedHandler(DependencyObject target)
        {
            return target.GetValue(SignalValueBehavior.OnLoadedHandlerProperty) as Action<object, RoutedEventArgs>;
        }
        #endregion

        #region OnUnloadedHandler
        public static DependencyProperty OnUnloadedHandlerProperty =
            DependencyProperty.RegisterAttached("OnUnloadedHandler",
            typeof(Action<object, RoutedEventArgs>),
            typeof(SignalValueBehavior),
            new UIPropertyMetadata(null));
        public static void SetOnUnloadedHandler(DependencyObject target, Action<object, RoutedEventArgs> value)
        {
            target.SetValue(SignalValueBehavior.OnUnloadedHandlerProperty, value);
        }
        public static Action<object, RoutedEventArgs> GetOnUnloadedHandler(DependencyObject target)
        {
            return target.GetValue(SignalValueBehavior.OnUnloadedHandlerProperty) as Action<object, RoutedEventArgs>;
        }
        #endregion

        #region ControlFillColorBySignal
        public static DependencyProperty ControlFillColorBySignalProperty =
            DependencyProperty.RegisterAttached("ControlFillColorBySignal",
            typeof(NodeSignal),
            typeof(SignalValueBehavior),
            new UIPropertyMetadata(SignalValueBehavior.ControlFillColorBySignalChanged));
        public static void SetControlFillColorBySignal(DependencyObject target, NodeSignal value)
        {
            target.SetValue(SignalValueBehavior.ControlFillColorBySignalProperty, value);
        }
        private static void ControlFillColorBySignalChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            var element = target as Shape;
            if (element == null)
            {
                throw new InvalidOperationException("This behavior only be attached to Shape items.");
            }
            var nodeItem = GetNodeItem(element);
            var oldSignal = e.OldValue as NodeSignal;
            var newSignal = e.NewValue as NodeSignal;
            if (nodeItem != null && newSignal != oldSignal)
            {
                // Define the handler for when the signal value changes
                Action<NodeSignal, object> onValueChanged;
                if (newSignal.DataType == FieldDataType.DataTypeEnum.BOOL)
                {
                    onValueChanged = new Action<NodeSignal, object>((signal, value) =>
                    {
                        if (signal.SignalId == newSignal.SignalId)
                        {
                            if ((bool)value)
                            {
                                element.Fill = new SolidColorBrush(GetTrueColor(element));
                            }
                            else
                            {
                                element.Fill = new SolidColorBrush(GetFalseColor(element));
                            }
                        }
                    });
                }
                else
                {
                    onValueChanged = new Action<NodeSignal, object>((signal, value) => { });
                }

                var onLoaded = new Action<object, RoutedEventArgs>((sender, routedEventArgs) =>
                {
                    staticRuntimeService().ValueChanged += new ValueChangedHandler(onValueChanged);
                    staticRuntimeService().RegisterValueWatcher(nodeItem, newSignal);
                });

                var onUnloaded = new Action<object, RoutedEventArgs>((sender, routedEventArgs) =>
                {
                    staticRuntimeService().DeregisterValueWatcher(nodeItem, newSignal);
                    staticRuntimeService().ValueChanged -= new ValueChangedHandler(onValueChanged);
                });

                if (oldSignal != null)
                {
                    // detaching
                    var oldOnLoaded = GetOnLoadedHandler(element);
                    var oldOnUnloaded = GetOnLoadedHandler(element);
                    element.Loaded -= new RoutedEventHandler(oldOnLoaded);
                    element.Unloaded -= new RoutedEventHandler(oldOnUnloaded); 
                    if (element.IsLoaded)
                    {
                        oldOnUnloaded(element, null);
                        SetOnLoadedHandler(element, null);
                        SetOnUnloadedHandler(element, null);
                    }
                }

                if (newSignal != null)
                {
                    // attaching
                    SetOnLoadedHandler(element, onLoaded);
                    SetOnUnloadedHandler(element, onUnloaded);
                    element.Loaded += new RoutedEventHandler(onLoaded);
                    element.Unloaded += new RoutedEventHandler(onUnloaded);
                    if (element.IsLoaded)
                    {
                        onLoaded(element, null);
                    }
                }
            }

        }
        #endregion

        #region ControlFillColorBySignalIn
        public static DependencyProperty ControlFillColorBySignalInProperty =
            DependencyProperty.RegisterAttached("ControlFillColorBySignalIn",
            typeof(NodeSignalIn),
            typeof(SignalValueBehavior),
            new UIPropertyMetadata(SignalValueBehavior.ControlFillColorBySignalInChanged));
        public static void SetControlFillColorBySignalIn(DependencyObject target, NodeSignalIn value)
        {
            target.SetValue(SignalValueBehavior.ControlFillColorBySignalInProperty, value);
        }
        private static void ControlFillColorBySignalInChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            var element = target as Shape;
            if (element == null)
            {
                throw new InvalidOperationException("This behavior only be attached to Shape items.");
            }
            var nodeItem = GetNodeItem(element);
            if (nodeItem != null)
            {
                var oldSignalIn = e.OldValue as NodeSignalIn;
                var newSignalIn = e.NewValue as NodeSignalIn;
                if (newSignalIn != oldSignalIn && newSignalIn != null)
                {
                    var tpl = staticRuntimeService().FindSignal(nodeItem, newSignalIn.SignalId);
                    if (tpl != null)
                    {
                        SetControlFillColorBySignal(target, tpl.Item2);
                    }
                    else
                    {
                        SetControlFillColorBySignal(target, null);
                    }
                }
            }

        }
        #endregion
    }
}
