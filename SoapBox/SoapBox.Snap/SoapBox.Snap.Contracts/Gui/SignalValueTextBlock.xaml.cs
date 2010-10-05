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
using System.ComponentModel.Composition;
using SoapBox.Protocol.Automation;
using SoapBox.Protocol.Base;

namespace SoapBox.Snap
{
    /// <summary>
    /// Interaction logic for SignalValueTextBlock.xaml
    /// </summary>
    [Export(SoapBox.Core.ExtensionPoints.Host.Void, typeof(object))]
    public partial class SignalValueTextBlock : UserControl
    {
        public SignalValueTextBlock()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(SignalValueTextBlock_Loaded);
            this.Unloaded += new RoutedEventHandler(SignalValueTextBlock_Unloaded);
        }

        void SignalValueTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            runtimeService.ValueChanged += new ValueChangedHandler(runtimeService_ValuesChanged);
            runtimeService.RegisterValueWatcher(NodeItem, Signal);
        }

        void SignalValueTextBlock_Unloaded(object sender, RoutedEventArgs e)
        {
            runtimeService.DeregisterValueWatcher(NodeItem, Signal);
            runtimeService.ValueChanged -= new ValueChangedHandler(runtimeService_ValuesChanged);
        }

        void runtimeService_ValuesChanged(NodeSignal signal, object value)
        {
            if (Signal != null && Signal.SignalId == signal.SignalId)
            {
                setText(value);
            }
        }

        #region " runtimeServiceSingleton "
        [Import(Services.Solution.RuntimeService, typeof(IRuntimeService))]
        private IRuntimeService runtimeService
        {
            get
            {
                return m_runtimeService;
            }
            set
            {
                m_runtimeService = value;
            }
        }
        private static IRuntimeService m_runtimeService = null;

        #endregion

        #region Signal
        public NodeSignal Signal
        {
            get { return (NodeSignal)GetValue(SignalProperty); }
            set { SetValue(SignalProperty, value); }
        }
        private static readonly string m_SignalName =
            Utilities.NotifyPropertyChangedHelper.GetPropertyName<SignalValueTextBlock>(o => o.Signal);
        public static readonly DependencyProperty SignalProperty =
            DependencyProperty.Register(m_SignalName, typeof(NodeSignal), typeof(SignalValueTextBlock), new UIPropertyMetadata(null, OnSignalChanged));

        public static void OnSignalChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            //((SignalValueTextBlock)source).setText();
        }
        #endregion

        /// <summary>
        /// Sets the text 
        /// </summary>
        private void setText(object value)
        {
            if (FormatString != string.Empty)
            {
                Text = string.Format(FormatString, value);
            }
            else
            {
                Text = value != null ? value.ToString() : " ";
            }
        }

        #region Text
        private string Text
        {
            get { return (string)GetValue(TextProperty); }
            set {  SetValue(TextProperty, value); }
        }
        private static readonly string m_TextName =
            Utilities.NotifyPropertyChangedHelper.GetPropertyName<SignalValueTextBlock>(o => o.Text);
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(m_TextName, typeof(string), typeof(SignalValueTextBlock), new UIPropertyMetadata(string.Empty,null));
        #endregion

        #region EditedText
        private string EditedText
        {
            get { return (string)GetValue(EditedTextProperty); }
            set { SetValue(EditedTextProperty, value); }
        }
        private static readonly string m_EditedTextName =
            Utilities.NotifyPropertyChangedHelper.GetPropertyName<SignalValueTextBlock>(o => o.EditedText);
        public static readonly DependencyProperty EditedTextProperty =
            DependencyProperty.Register(m_EditedTextName, typeof(string), typeof(SignalValueTextBlock), new UIPropertyMetadata(string.Empty, null));

        #endregion

        private NodeSignal m_editedSignal = null;

        #region Editing
        public bool Editing
        {
            get { return (bool)GetValue(EditingProperty); }
            set
            {
                bool prevValue = (bool)GetValue(EditingProperty); // unbox
                if (prevValue == false && value == true)
                {
                    m_editedSignal = Signal;
                }
                else if (prevValue == true && value == false)
                {
                     m_editedSignal = null;
                }
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
            Utilities.NotifyPropertyChangedHelper.GetPropertyName<SignalValueTextBlock>(o => o.Editing);
        public static readonly DependencyProperty EditingProperty =
            DependencyProperty.Register(m_EditingName, typeof(bool), typeof(SignalValueTextBlock), new UIPropertyMetadata(false));
        #endregion

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //EditedText = Text;
            //Editing = true;
            //skipFirstLostFocus = true;
            //textBox.Focus();
        }

        private bool skipFirstLostFocus = false;

        private void TextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!skipFirstLostFocus)
            {
                if (Editing)
                {
                    Signal = m_editedSignal;
                    Editing = false;
                }
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
            if (e.Key == Key.Enter)
            {
                readTextBox(); // do this in case the last key was a backspace
                Signal = m_editedSignal;
                e.Handled = true;
                Editing = false;
            }
            else if (e.Key == Key.Escape)
            {
                Editing = false;
            }
        }

        private bool ignoreKey(Key k)
        {
            switch (k)
            {
                case Key.Escape:
                case Key.Enter:
                case Key.Delete:
                case Key.Home:
                case Key.End:
                case Key.PageUp:
                case Key.PageDown:
                case Key.Back:
                case Key.Down:
                case Key.Up:
                case Key.Right:
                case Key.Left:
                    return true;

            }
            return false;
        }

        private void readTextBox()
        {
            var tb = textBox;
            var selectedText = tb.SelectedText;
            string unselectedText;
            if (selectedText.Length > 0)
            {
                unselectedText = tb.Text.Replace(selectedText, string.Empty);
            }
            else
            {
                unselectedText = tb.Text;
            }

            if (unselectedText.Length > 0)
            {
                // check to see if the value entered is a valid literal
                object parseResult;
                if (Signal.DataType.TryParse(unselectedText, out parseResult))
                {
                    tb.Text = parseResult.ToString();
                }
                else
                {
                    m_editedSignal = Signal;
                }
            }
            else
            {
                m_editedSignal = Signal;
            }
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (!ignoreKey(e.Key))
            {
                readTextBox();
            }
        }

        #region TextAlignment
        public TextAlignment TextAlignment
        {
            get { return (TextAlignment)GetValue(TextAlignmentProperty); }
            set { SetValue(TextAlignmentProperty, value); }
        }
        private static readonly string m_TextAlignmentName =
            Utilities.NotifyPropertyChangedHelper.GetPropertyName<SignalValueTextBlock>(o => o.TextAlignment);
        public static readonly DependencyProperty TextAlignmentProperty =
            DependencyProperty.Register(m_TextAlignmentName, typeof(TextAlignment), typeof(SignalValueTextBlock), new UIPropertyMetadata(TextAlignment.Left));
        #endregion

        #region FormatString
        public string FormatString
        {
            get { return (string)GetValue(FormatStringProperty); }
            set { SetValue(FormatStringProperty, value); }
        }
        private static readonly string m_FormatStringName =
            Utilities.NotifyPropertyChangedHelper.GetPropertyName<SignalValueTextBlock>(o => o.FormatString);
        public static readonly DependencyProperty FormatStringProperty =
            DependencyProperty.Register(m_FormatStringName, typeof(string), typeof(SignalValueTextBlock), new UIPropertyMetadata(string.Empty));
        #endregion

        #region NodeItem
        public INodeWrapper NodeItem
        {
            get { return (INodeWrapper)GetValue(NodeItemProperty); }
            set { SetValue(NodeItemProperty, value); }
        }
        private static readonly string m_NodeItemName =
            Utilities.NotifyPropertyChangedHelper.GetPropertyName<SignalValueTextBlock>(o => o.NodeItem);
        public static readonly DependencyProperty NodeItemProperty =
            DependencyProperty.Register(m_NodeItemName, typeof(INodeWrapper), typeof(SignalValueTextBlock), new UIPropertyMetadata(null, OnNodeItemChanged));

        public static void OnNodeItemChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            //((SignalValueTextBlock)source).setText();
        }
        #endregion

    }
}
