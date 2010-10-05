#region "SoapBox.Snap License"
/// <header module="SoapBox.Snap"> 
/// Copyright (C) 2009 SoapBox Automation Inc., All Rights Reserved.
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
using SoapBox.Core;
using SoapBox.Utilities;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using SoapBox.Protocol.Base;
using SoapBox.Protocol.Automation;

namespace SoapBox.Snap.LD
{
    public abstract class AbstractSignalValueTextBlock : AbstractControl
    {
        public AbstractSignalValueTextBlock()
        {
            this.PropertyChanged += new PropertyChangedEventHandler(AbstractSignalValueTextBlock_PropertyChanged);
        }

        void AbstractSignalValueTextBlock_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != m_ActualHeightName)
            {
                calculateActualHeight();
            }
        }

        #region " NodeItem "
        public INodeWrapper NodeItem
        {
            get
            {
                return m_NodeItem;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(m_NodeItemName);
                }
                if (m_NodeItem != value)
                {
                    m_NodeItem = value;
                    NotifyPropertyChanged(m_NodeItemArgs);
                }
            }
        }
        private INodeWrapper m_NodeItem = null;
        private static readonly PropertyChangedEventArgs m_NodeItemArgs =
            NotifyPropertyChangedHelper.CreateArgs<AbstractSignalValueTextBlock>(o => o.NodeItem);
        private static string m_NodeItemName =
            NotifyPropertyChangedHelper.GetPropertyName<AbstractSignalValueTextBlock>(o => o.NodeItem);
        #endregion

        #region " Signal "
        public NodeSignal Signal
        {
            get
            {
                return m_Signal;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(m_SignalName);
                }
                if (m_Signal != value)
                {
                    m_Signal = value;
                    NotifyPropertyChanged(m_SignalArgs);
                }
            }
        }
        private NodeSignal m_Signal = null;
        private static readonly PropertyChangedEventArgs m_SignalArgs =
            NotifyPropertyChangedHelper.CreateArgs<AbstractSignalValueTextBlock>(o => o.Signal);
        private static string m_SignalName =
            NotifyPropertyChangedHelper.GetPropertyName<AbstractSignalValueTextBlock>(o => o.Signal);
        #endregion

        #region " MaxWidth "
        public double MaxWidth
        {
            get
            {
                return m_MaxWidth;
            }
            set
            {
                if (m_MaxWidth != value)
                {
                    m_MaxWidth = value;
                    NotifyPropertyChanged(m_MaxWidthArgs);
                }
            }
        }
        private double m_MaxWidth = double.PositiveInfinity;
        private static readonly PropertyChangedEventArgs m_MaxWidthArgs =
            NotifyPropertyChangedHelper.CreateArgs<AbstractSignalValueTextBlock>(o => o.MaxWidth);
        private static string m_MaxWidthName =
            NotifyPropertyChangedHelper.GetPropertyName<AbstractSignalValueTextBlock>(o => o.MaxWidth);
        #endregion

        #region " MaxHeight "
        public double MaxHeight
        {
            get
            {
                return m_MaxHeight;
            }
            set
            {
                if (m_MaxHeight != value)
                {
                    m_MaxHeight = value;
                    NotifyPropertyChanged(m_MaxHeightArgs);
                }
            }
        }
        private double m_MaxHeight = double.PositiveInfinity;
        private static readonly PropertyChangedEventArgs m_MaxHeightArgs =
            NotifyPropertyChangedHelper.CreateArgs<AbstractSignalValueTextBlock>(o => o.MaxHeight);
        private static string m_MaxHeightName =
            NotifyPropertyChangedHelper.GetPropertyName<AbstractSignalValueTextBlock>(o => o.MaxHeight);
        #endregion

        #region " TextAlignment "
        public System.Windows.TextAlignment TextAlignment
        {
            get
            {
                return m_TextAlignment;
            }
            set
            {
                if (m_TextAlignment != value)
                {
                    m_TextAlignment = value;
                    NotifyPropertyChanged(m_TextAlignmentArgs);
                }
            }
        }
        private System.Windows.TextAlignment m_TextAlignment = System.Windows.TextAlignment.Left;
        private static readonly PropertyChangedEventArgs m_TextAlignmentArgs =
            NotifyPropertyChangedHelper.CreateArgs<AbstractSignalValueTextBlock>(o => o.TextAlignment);
        private static string m_TextAlignmentName =
            NotifyPropertyChangedHelper.GetPropertyName<AbstractSignalValueTextBlock>(o => o.TextAlignment);
        #endregion

        #region " ActualHeight "
        public double ActualHeight
        {
            get
            {
                return m_ActualHeight;
            }
            set
            {
                if (m_ActualHeight != value)
                {
                    m_ActualHeight = value;
                    NotifyPropertyChanged(m_ActualHeightArgs);
                }
            }
        }
        private double m_ActualHeight = 0;
        private static readonly PropertyChangedEventArgs m_ActualHeightArgs =
            NotifyPropertyChangedHelper.CreateArgs<AbstractSignalValueTextBlock>(o => o.ActualHeight);
        private static string m_ActualHeightName =
            NotifyPropertyChangedHelper.GetPropertyName<AbstractSignalValueTextBlock>(o => o.ActualHeight);
        #endregion

        #region " FormatString "
        public string FormatString
        {
            get
            {
                return m_FormatString;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(m_FormatStringName);
                }
                if (m_FormatString != value)
                {
                    m_FormatString = value;
                    NotifyPropertyChanged(m_FormatStringArgs);
                }
            }
        }
        private string m_FormatString = string.Empty;
        private static readonly PropertyChangedEventArgs m_FormatStringArgs =
            NotifyPropertyChangedHelper.CreateArgs<AbstractSignalValueTextBlock>(o => o.FormatString);
        private static string m_FormatStringName =
            NotifyPropertyChangedHelper.GetPropertyName<AbstractSignalValueTextBlock>(o => o.FormatString);
        #endregion

        #region " VmSignalValueTextBlock "
        // We actually need to be able to measure the controls in order to render a rung
        // in LD, which is why we need to create the control itself in the ViewModel 
        public SignalValueTextBlock VmSignalValueTextBlock
        {
            get
            {
                if (m_VmSignalValueTextBlock == null)
                {
                    m_VmSignalValueTextBlock = buildSignalValueTextBlock();

                    // Bind nodeItem property
                    Binding nodeItemBinding = new Binding(m_NodeItemName);
                    nodeItemBinding.Source = this;
                    m_VmSignalValueTextBlock.SetBinding(SignalValueTextBlock.NodeItemProperty, nodeItemBinding);

                    // Bind Signal property
                    Binding SignalBinding = new Binding(m_SignalName);
                    SignalBinding.Source = this;
                    SignalBinding.Mode = BindingMode.TwoWay;
                    m_VmSignalValueTextBlock.SetBinding(SignalValueTextBlock.SignalProperty, SignalBinding);

                    // Bind MaxWidth property
                    Binding maxWidthBinding = new Binding(m_MaxWidthName);
                    maxWidthBinding.Source = this;
                    m_VmSignalValueTextBlock.SetBinding(SignalValueTextBlock.MaxWidthProperty, maxWidthBinding);

                    // Bind MaxHeight property
                    Binding maxHeightBinding = new Binding(m_MaxHeightName);
                    maxHeightBinding.Source = this;
                    m_VmSignalValueTextBlock.SetBinding(SignalValueTextBlock.MaxHeightProperty, maxHeightBinding);

                    // Bind TextAlignment property
                    Binding textAlignmentBinding = new Binding(m_TextAlignmentName);
                    textAlignmentBinding.Source = this;
                    m_VmSignalValueTextBlock.SetBinding(SignalValueTextBlock.TextAlignmentProperty, textAlignmentBinding);

                    // Bind FormatString property
                    Binding FormatStringBinding = new Binding(m_FormatStringName);
                    FormatStringBinding.Source = this;
                    m_VmSignalValueTextBlock.SetBinding(SignalValueTextBlock.FormatStringProperty, FormatStringBinding);

                    calculateActualHeight();

                }
                return m_VmSignalValueTextBlock;
            }
        }
        private SignalValueTextBlock m_VmSignalValueTextBlock = null;

        #endregion

        private void calculateActualHeight()
        {
            // We can't just measure the VmSignalValueTextBlock because
            // this method gets called before the bindings catch up
            // with the actual TextBlocks in the user control itself,
            // but if we just build one here, it works.
            var measurementTest = buildSignalValueTextBlock();
            measurementTest.Signal = Signal;
            measurementTest.NodeItem = NodeItem;
            measurementTest.MaxWidth = MaxWidth;
            measurementTest.MaxHeight = MaxHeight;
            measurementTest.TextAlignment = TextAlignment;
            measurementTest.FormatString = FormatString;

            measurementTest.Measure(new Size(MaxWidth, MaxHeight));
            ActualHeight = measurementTest.DesiredSize.Height;
        }

        protected abstract SignalValueTextBlock buildSignalValueTextBlock(); // must be overridden in derived class
    }
}
