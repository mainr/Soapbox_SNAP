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
    public abstract class AbstractSignalChooserTextBlock : AbstractControl
    {
        public AbstractSignalChooserTextBlock()
        {
            this.PropertyChanged += new PropertyChangedEventHandler(AbstractSignalChooserTextBlock_PropertyChanged);
        }

        void AbstractSignalChooserTextBlock_PropertyChanged(object sender, PropertyChangedEventArgs e)
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
            NotifyPropertyChangedHelper.CreateArgs<AbstractSignalChooserTextBlock>(o => o.NodeItem);
        private static string m_NodeItemName =
            NotifyPropertyChangedHelper.GetPropertyName<AbstractSignalChooserTextBlock>(o => o.NodeItem);
        #endregion

        #region " SignalIn "
        public NodeSignalIn SignalIn
        {
            get
            {
                return m_SignalIn;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(m_SignalInName);
                }
                if (m_SignalIn != value)
                {
                    m_SignalIn = value;
                    NotifyPropertyChanged(m_SignalInArgs);
                }
            }
        }
        private NodeSignalIn m_SignalIn = null;
        private static readonly PropertyChangedEventArgs m_SignalInArgs =
            NotifyPropertyChangedHelper.CreateArgs<AbstractSignalChooserTextBlock>(o => o.SignalIn);
        private static string m_SignalInName =
            NotifyPropertyChangedHelper.GetPropertyName<AbstractSignalChooserTextBlock>(o => o.SignalIn);
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
            NotifyPropertyChangedHelper.CreateArgs<AbstractSignalChooserTextBlock>(o => o.MaxWidth);
        private static string m_MaxWidthName =
            NotifyPropertyChangedHelper.GetPropertyName<AbstractSignalChooserTextBlock>(o => o.MaxWidth);
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
            NotifyPropertyChangedHelper.CreateArgs<AbstractSignalChooserTextBlock>(o => o.MaxHeight);
        private static string m_MaxHeightName =
            NotifyPropertyChangedHelper.GetPropertyName<AbstractSignalChooserTextBlock>(o => o.MaxHeight);
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
            NotifyPropertyChangedHelper.CreateArgs<AbstractSignalChooserTextBlock>(o => o.TextAlignment);
        private static string m_TextAlignmentName =
            NotifyPropertyChangedHelper.GetPropertyName<AbstractSignalChooserTextBlock>(o => o.TextAlignment);
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
            NotifyPropertyChangedHelper.CreateArgs<AbstractSignalChooserTextBlock>(o => o.ActualHeight);
        private static string m_ActualHeightName =
            NotifyPropertyChangedHelper.GetPropertyName<AbstractSignalChooserTextBlock>(o => o.ActualHeight);
        #endregion

        #region " VmSignalChooserTextBlock "
        // We actually need to be able to measure the controls in order to render a rung
        // in LD, which is why we need to create the control itself in the ViewModel 
        public SignalChooserTextBlock VmSignalChooserTextBlock
        {
            get
            {
                if (m_VmSignalChooserTextBlock == null)
                {
                    m_VmSignalChooserTextBlock = buildSignalChooserTextBlock();

                    // Bind nodeItem property
                    Binding nodeItemBinding = new Binding(m_NodeItemName);
                    nodeItemBinding.Source = this;
                    m_VmSignalChooserTextBlock.SetBinding(SignalChooserTextBlock.NodeItemProperty, nodeItemBinding);

                    // Bind signalIn property
                    Binding signalInBinding = new Binding(m_SignalInName);
                    signalInBinding.Source = this;
                    signalInBinding.Mode = BindingMode.TwoWay;
                    m_VmSignalChooserTextBlock.SetBinding(SignalChooserTextBlock.SignalInProperty, signalInBinding);

                    // Bind MaxWidth property
                    Binding maxWidthBinding = new Binding(m_MaxWidthName);
                    maxWidthBinding.Source = this;
                    m_VmSignalChooserTextBlock.SetBinding(SignalChooserTextBlock.MaxWidthProperty, maxWidthBinding);

                    // Bind MaxHeight property
                    Binding maxHeightBinding = new Binding(m_MaxHeightName);
                    maxHeightBinding.Source = this;
                    m_VmSignalChooserTextBlock.SetBinding(SignalChooserTextBlock.MaxHeightProperty, maxHeightBinding);

                    // Bind TextAlignment property
                    Binding textAlignmentBinding = new Binding(m_TextAlignmentName);
                    textAlignmentBinding.Source = this;
                    m_VmSignalChooserTextBlock.SetBinding(SignalChooserTextBlock.TextAlignmentProperty, textAlignmentBinding);

                    calculateActualHeight();

                }
                return m_VmSignalChooserTextBlock;
            }
        }
        private SignalChooserTextBlock m_VmSignalChooserTextBlock = null;

        #endregion

        private void calculateActualHeight()
        {
            // We can't just measure the VmSignalChooserTextBlock because
            // this method gets called before the bindings catch up
            // with the actual TextBlocks in the user control itself,
            // but if we just build one here, it works.
            var measurementTest = buildSignalChooserTextBlock();
            measurementTest.NodeItem = NodeItem;
            measurementTest.SignalIn = SignalIn;
            measurementTest.MaxWidth = MaxWidth;
            measurementTest.MaxHeight = MaxHeight;
            measurementTest.TextAlignment = TextAlignment;

            measurementTest.Measure(new Size(MaxWidth, MaxHeight));
            ActualHeight = measurementTest.DesiredSize.Height;
        }

        protected abstract SignalChooserTextBlock buildSignalChooserTextBlock(); // must be overridden in derived class
    }
}
