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

namespace SoapBox.Snap.LD
{
    public abstract class AbstractEditableTextBlock : AbstractControl
    {
        public AbstractEditableTextBlock()
        {
            this.PropertyChanged += new PropertyChangedEventHandler(AbstractEditableTextBlock_PropertyChanged);
        }

        void AbstractEditableTextBlock_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != m_ActualHeightName)
            {
                calculateActualHeight();
            }
        }

        #region " Editable "
        public bool Editable
        {
            get
            {
                return m_Editable;
            }
            set
            {
                if (m_Editable != value)
                {
                    m_Editable = value;
                    NotifyPropertyChanged(m_EditableArgs);
                }
            }
        }
        private bool m_Editable = true;
        private static readonly PropertyChangedEventArgs m_EditableArgs =
            NotifyPropertyChangedHelper.CreateArgs<AbstractEditableTextBlock>(o => o.Editable);
        private static string m_EditableName =
            NotifyPropertyChangedHelper.GetPropertyName<AbstractEditableTextBlock>(o => o.Editable);
        #endregion

        #region " Text "
        public string Text
        {
            get
            {
                return m_Text;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(m_TextName);
                }
                if (m_Text != value)
                {
                    m_Text = value;
                    NotifyPropertyChanged(m_TextArgs);
                }
            }
        }
        private string m_Text = string.Empty;
        private static readonly PropertyChangedEventArgs m_TextArgs =
            NotifyPropertyChangedHelper.CreateArgs<AbstractEditableTextBlock>(o => o.Text);
        private static string m_TextName =
            NotifyPropertyChangedHelper.GetPropertyName<AbstractEditableTextBlock>(o => o.Text);
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
            NotifyPropertyChangedHelper.CreateArgs<AbstractEditableTextBlock>(o => o.MaxWidth);
        private static string m_MaxWidthName =
            NotifyPropertyChangedHelper.GetPropertyName<AbstractEditableTextBlock>(o => o.MaxWidth);
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
            NotifyPropertyChangedHelper.CreateArgs<AbstractEditableTextBlock>(o => o.MaxHeight);
        private static string m_MaxHeightName =
            NotifyPropertyChangedHelper.GetPropertyName<AbstractEditableTextBlock>(o => o.MaxHeight);
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
            NotifyPropertyChangedHelper.CreateArgs<AbstractEditableTextBlock>(o => o.TextAlignment);
        private static string m_TextAlignmentName =
            NotifyPropertyChangedHelper.GetPropertyName<AbstractEditableTextBlock>(o => o.TextAlignment);
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
            NotifyPropertyChangedHelper.CreateArgs<AbstractEditableTextBlock>(o => o.ActualHeight);
        private static string m_ActualHeightName =
            NotifyPropertyChangedHelper.GetPropertyName<AbstractEditableTextBlock>(o => o.ActualHeight);
        #endregion

        #region " VmEditableTextBlock "
        // We actually need to be able to measure the controls in order to render a rung
        // in LD, which is why we need to create the control itself in the ViewModel 
        public EditableTextBlock VmEditableTextBlock
        {
            get
            {
                if (m_VmEditableTextBlock == null)
                {
                    m_VmEditableTextBlock = buildEditableTextBlock();

                    // Bind text property
                    Binding textBinding = new Binding(m_TextName);
                    textBinding.Source = this;
                    textBinding.Mode = BindingMode.TwoWay;
                    m_VmEditableTextBlock.SetBinding(EditableTextBlock.TextProperty, textBinding);

                    // Bind MaxWidth property
                    Binding maxWidthBinding = new Binding(m_MaxWidthName);
                    maxWidthBinding.Source = this;
                    m_VmEditableTextBlock.SetBinding(EditableTextBlock.MaxWidthProperty, maxWidthBinding);

                    // Bind MaxHeight property
                    Binding maxHeightBinding = new Binding(m_MaxHeightName);
                    maxHeightBinding.Source = this;
                    m_VmEditableTextBlock.SetBinding(EditableTextBlock.MaxHeightProperty, maxHeightBinding);

                    // Bind TextAlignment property
                    Binding textAlignmentBinding = new Binding(m_TextAlignmentName);
                    textAlignmentBinding.Source = this;
                    m_VmEditableTextBlock.SetBinding(EditableTextBlock.TextAlignmentProperty, textAlignmentBinding);

                    // Bind Editable property
                    Binding editableBinding = new Binding(m_EditableName);
                    editableBinding.Source = this;
                    m_VmEditableTextBlock.SetBinding(EditableTextBlock.EditableProperty, editableBinding);

                    calculateActualHeight();

                }
                return m_VmEditableTextBlock;
            }
        }
        private EditableTextBlock m_VmEditableTextBlock = null;

        #endregion

        private void calculateActualHeight()
        {
            // We can't just measure the VmEditableTextBlock because
            // this method gets called before the bindings catch up
            // with the actual TextBlocks in the user control itself,
            // but if we just build one here, it works.
            var measurementTest = buildEditableTextBlock();
            measurementTest.Text = Text;
            measurementTest.MaxWidth = MaxWidth;
            measurementTest.MaxHeight = MaxHeight;
            measurementTest.TextAlignment = TextAlignment;

            measurementTest.Measure(new Size(MaxWidth, MaxHeight));
            ActualHeight = measurementTest.DesiredSize.Height;
        }

        protected abstract EditableTextBlock buildEditableTextBlock(); // must be overridden in derived class
    }
}
