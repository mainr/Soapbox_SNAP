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

namespace SoapBox.Snap
{
    /// <summary>
    /// Interaction logic for EditableTextBlock.xaml
    /// </summary>
    public partial class EditableTextBlock : UserControl
    {
        public EditableTextBlock()
        {
            InitializeComponent();
        }

        #region Text
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        private static readonly string m_TextName =
            Utilities.NotifyPropertyChangedHelper.GetPropertyName<EditableTextBlock>(o => o.Text);
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(m_TextName, typeof(string), typeof(EditableTextBlock), new UIPropertyMetadata(string.Empty));
        #endregion

        #region EditedText
        public string EditedText
        {
            get { return (string)GetValue(EditedTextProperty); }
            set { SetValue(EditedTextProperty, value); }
        }
        private static readonly string m_EditedTextName =
            Utilities.NotifyPropertyChangedHelper.GetPropertyName<EditableTextBlock>(o => o.EditedText);
        public static readonly DependencyProperty EditedTextProperty =
            DependencyProperty.Register(m_EditedTextName, typeof(string), typeof(EditableTextBlock), new UIPropertyMetadata(string.Empty));
        #endregion

        #region Editing
        public bool Editing
        {
            get { return (bool)GetValue(EditingProperty); }
            set 
            {
                if (value)
                {
                    Width = MaxWidth;
                }
                else
                {
                    Width = double.NaN;
                }
                SetValue(EditingProperty, value); 
            }
        }
        private static readonly string m_EditingName =
            Utilities.NotifyPropertyChangedHelper.GetPropertyName<EditableTextBlock>(o => o.Editing);
        public static readonly DependencyProperty EditingProperty =
            DependencyProperty.Register(m_EditingName, typeof(bool), typeof(EditableTextBlock), new UIPropertyMetadata(false));
        #endregion

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Editable)
            {
                EditedText = Text;
                Editing = true;
                skipFirstLostFocus = true;
                textBox.Focus();
            }
        }

        private bool skipFirstLostFocus = false;

        private void TextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!skipFirstLostFocus)
            {
                Text = EditedText;
                Editing = false;
            }
            else
            {
                textBox.Focus();
                textBox.SelectAll();
            }
            skipFirstLostFocus = false;
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                if (!AcceptsReturn)
                {
                    Text = EditedText;
                    e.Handled = true;
                    Editing = false;
                }
            }
            else if (e.Key == System.Windows.Input.Key.Escape)
            {
                Editing = false;
            }
        }

        #region TextWrapping
        public TextWrapping TextWrapping
        {
            get { return (TextWrapping)GetValue(TextWrappingProperty); }
            set { SetValue(TextWrappingProperty, value); }
        }
        private static readonly string m_TextWrappingName =
            Utilities.NotifyPropertyChangedHelper.GetPropertyName<EditableTextBlock>(o => o.TextWrapping);
        public static readonly DependencyProperty TextWrappingProperty =
            DependencyProperty.Register(m_TextWrappingName, typeof(TextWrapping), typeof(EditableTextBlock), new UIPropertyMetadata(TextWrapping.NoWrap));
        #endregion

        #region TextAlignment
        public TextAlignment TextAlignment
        {
            get { return (TextAlignment)GetValue(TextAlignmentProperty); }
            set { SetValue(TextAlignmentProperty, value); }
        }
        private static readonly string m_TextAlignmentName =
            Utilities.NotifyPropertyChangedHelper.GetPropertyName<EditableTextBlock>(o => o.TextAlignment);
        public static readonly DependencyProperty TextAlignmentProperty =
            DependencyProperty.Register(m_TextAlignmentName, typeof(TextAlignment), typeof(EditableTextBlock), new UIPropertyMetadata(TextAlignment.Left));
        #endregion

        #region AcceptsReturn
        public bool AcceptsReturn
        {
            get { return (bool)GetValue(AcceptsReturnProperty); }
            set
            {
                SetValue(AcceptsReturnProperty, value);
            }
        }
        private static readonly string m_AcceptsReturnName =
            Utilities.NotifyPropertyChangedHelper.GetPropertyName<EditableTextBlock>(o => o.AcceptsReturn);
        public static readonly DependencyProperty AcceptsReturnProperty =
            DependencyProperty.Register(m_AcceptsReturnName, typeof(bool), typeof(EditableTextBlock), new UIPropertyMetadata(false));
        #endregion

        #region Editable
        public bool Editable
        {
            get { return (bool)GetValue(EditableProperty); }
            set
            {
                SetValue(EditableProperty, value);
            }
        }
        private static readonly string m_EditableName =
            Utilities.NotifyPropertyChangedHelper.GetPropertyName<EditableTextBlock>(o => o.Editable);
        public static readonly DependencyProperty EditableProperty =
            DependencyProperty.Register(m_EditableName, typeof(bool), typeof(EditableTextBlock), new UIPropertyMetadata(false));
        #endregion
    }
}
