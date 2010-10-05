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
    /// Interaction logic for SignalChooserTextBlock.xaml
    /// </summary>
    [Export(SoapBox.Core.ExtensionPoints.Host.Void, typeof(object))]
    public partial class SignalChooserTextBlock : UserControl
    {
        public SignalChooserTextBlock()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(SignalChooserTextBlock_Loaded);
            this.Unloaded += new RoutedEventHandler(SignalChooserTextBlock_Unloaded);
        }

        void SignalChooserTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            setText();
            runtimeService.SignalChanged += new SignalChangedHandler(runtimeService_SignalChanged);
        }

        void SignalChooserTextBlock_Unloaded(object sender, RoutedEventArgs e)
        {
            runtimeService.SignalChanged -= new SignalChangedHandler(runtimeService_SignalChanged);
        }

        void runtimeService_SignalChanged(NodeSignal signal)
        {
            if (SignalIn != null && SignalIn.SignalId == signal.SignalId)
            {
                setText();
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

        private Dictionary<string, NodeSignal> signalList { get; set; }

        #region SignalIn
        public NodeSignalIn SignalIn
        {
            get { return (NodeSignalIn)GetValue(SignalInProperty); }
            set { SetValue(SignalInProperty, value); }
        }
        private static readonly string m_SignalInName =
            Utilities.NotifyPropertyChangedHelper.GetPropertyName<SignalChooserTextBlock>(o => o.SignalIn);
        public static readonly DependencyProperty SignalInProperty =
            DependencyProperty.Register(m_SignalInName, typeof(NodeSignalIn), typeof(SignalChooserTextBlock), new UIPropertyMetadata(null, OnSignalInChanged));

        public static void OnSignalInChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ((SignalChooserTextBlock)source).setText();
        }
        #endregion

        /// <summary>
        /// Sets the text based on SignalId (and NodeItem)
        /// </summary>
        private void setText()
        {
            if (SignalIn != null && NodeItem != null && SignalIn.SignalId != null)
            {
                var tpl = runtimeService.FindSignal(NodeItem, SignalIn.SignalId);
                if (tpl != null)
                {
                    Text = tpl.Item1;
                }
                else if (SignalIn.Literal != null)
                {
                    Text = SignalIn.Literal.Value.ToString();
                }
                else
                {
                    Text = "---";
                }
            }
            else if (SignalIn != null && SignalIn.Literal != null)
            {
                Text = SignalIn.Literal.Value.ToString();
            }
            else
            {
                Text = "---";
            }
        }

        #region Text
        private string Text
        {
            get { return (string)GetValue(TextProperty); }
            set {  SetValue(TextProperty, value); }
        }
        private static readonly string m_TextName =
            Utilities.NotifyPropertyChangedHelper.GetPropertyName<SignalChooserTextBlock>(o => o.Text);
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(m_TextName, typeof(string), typeof(SignalChooserTextBlock), new UIPropertyMetadata(string.Empty,null,OnCoerceText));

        public static object OnCoerceText(DependencyObject source, object value)
        {
            var incoming = value as string;
            return incoming.Replace("/", "\r\n");
        }
        #endregion

        #region EditedText
        private string EditedText
        {
            get { return (string)GetValue(EditedTextProperty); }
            set { SetValue(EditedTextProperty, value); }
        }
        private static readonly string m_EditedTextName =
            Utilities.NotifyPropertyChangedHelper.GetPropertyName<SignalChooserTextBlock>(o => o.EditedText);
        public static readonly DependencyProperty EditedTextProperty =
            DependencyProperty.Register(m_EditedTextName, typeof(string), typeof(SignalChooserTextBlock), new UIPropertyMetadata(string.Empty, null, OnCoerceEditedText));

        public static object OnCoerceEditedText(DependencyObject source, object value)
        {
            var incoming = value as string;
            return incoming.Replace("\r\n","/");
        }
        #endregion

        private NodeSignalIn m_editedSignalIn = null;

        #region Editing
        public bool Editing
        {
            get { return (bool)GetValue(EditingProperty); }
            set
            {
                bool prevValue = (bool)GetValue(EditingProperty); // unbox
                if (prevValue == false && value == true)
                {
                    if (runtimeService.DisconnectDialog(NodeItem))
                    {
                        // we're starting an edit, refresh the autocomplete list
                        if (NodeItem != null)
                        {
                            signalList = runtimeService.SignalList(NodeItem, SignalIn.CompatibleTypes.DataType);
                        }
                        else
                        {
                            signalList = new Dictionary<string, NodeSignal>();
                        }
                        m_editedSignalIn = SignalIn;
                    }
                    else
                    {
                        value = false;
                    }
                }
                else if (prevValue == true && value == false)
                {
                    signalList = null; // don't want to hold on to all that memory
                    m_editedSignalIn = null;
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
            Utilities.NotifyPropertyChangedHelper.GetPropertyName<SignalChooserTextBlock>(o => o.Editing);
        public static readonly DependencyProperty EditingProperty =
            DependencyProperty.Register(m_EditingName, typeof(bool), typeof(SignalChooserTextBlock), new UIPropertyMetadata(false));
        #endregion

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            EditedText = Text;
            Editing = true;
            skipFirstLostFocus = true;
            textBox.Focus();
        }

        private bool skipFirstLostFocus = false;

        private void TextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!skipFirstLostFocus)
            {
                if (Editing)
                {
                    if (btnSignalChooserDialog.IsKeyboardFocused)
                    {
                        // User clicked "..." button
                        SignalIn = runtimeService.SignalDialog(NodeItem, SignalIn);
                    }
                    else
                    {
                        SignalIn = m_editedSignalIn;
                    }
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
                SignalIn = m_editedSignalIn;
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
                if (SignalIn.CompatibleTypes.TryParse(unselectedText, out parseResult))
                {
                    tb.Text = parseResult.ToString();
                    m_editedSignalIn = NodeSignalIn.BuildWith(
                            SignalIn.DataType, SignalIn.CompatibleTypes,
                            new FieldConstant(SignalIn.CompatibleTypes.DataType, parseResult));
                }
                else
                {
                    Tuple<string, NodeSignal> closestSignal = findClosestMatchingSignal(unselectedText);
                    if (closestSignal != null)
                    {
                        var signalName = closestSignal.Item1;
                        var nSignal = closestSignal.Item2;
                        tb.Text = signalName;
                        tb.SelectionStart = tb.Text.ToLower().IndexOf(unselectedText.ToLower()) + unselectedText.Length;
                        tb.SelectionLength = tb.Text.Length - unselectedText.Length;
                        m_editedSignalIn = NodeSignalIn.BuildWith(
                            SignalIn.DataType, SignalIn.CompatibleTypes,
                            closestSignal.Item2.SignalId);
                    }
                    else
                    {
                        m_editedSignalIn = SignalIn;
                    }
                }
            }
            else
            {
                m_editedSignalIn = SignalIn;
            }
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (!ignoreKey(e.Key))
            {
                readTextBox();
            }
        }

        private Tuple<string, NodeSignal> findClosestMatchingSignal(string unselectedText)
        {
            var identical = from k in signalList.Keys where unselectedText.ToLower() == k.ToLower() select k;
            if (identical.Count() > 0)
            {
                return new Tuple<string, NodeSignal>(identical.First(), signalList[identical.First()]);
            }
            else
            {
                var startsWith = from k in signalList.Keys where k.ToLower().StartsWith(unselectedText.ToLower()) select k;
                if (startsWith.Count() > 0)
                {
                    return new Tuple<string, NodeSignal>(startsWith.First(), signalList[startsWith.First()]);
                }
                else
                {
                    return null;
                }
            }
        }

        #region TextAlignment
        public TextAlignment TextAlignment
        {
            get { return (TextAlignment)GetValue(TextAlignmentProperty); }
            set { SetValue(TextAlignmentProperty, value); }
        }
        private static readonly string m_TextAlignmentName =
            Utilities.NotifyPropertyChangedHelper.GetPropertyName<SignalChooserTextBlock>(o => o.TextAlignment);
        public static readonly DependencyProperty TextAlignmentProperty =
            DependencyProperty.Register(m_TextAlignmentName, typeof(TextAlignment), typeof(SignalChooserTextBlock), new UIPropertyMetadata(TextAlignment.Left));
        #endregion

        #region NodeItem
        public INodeWrapper NodeItem
        {
            get { return (INodeWrapper)GetValue(NodeItemProperty); }
            set { SetValue(NodeItemProperty, value); }
        }
        private static readonly string m_NodeItemName =
            Utilities.NotifyPropertyChangedHelper.GetPropertyName<SignalChooserTextBlock>(o => o.NodeItem);
        public static readonly DependencyProperty NodeItemProperty =
            DependencyProperty.Register(m_NodeItemName, typeof(INodeWrapper), typeof(SignalChooserTextBlock), new UIPropertyMetadata(null, OnNodeItemChanged));

        public static void OnNodeItemChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ((SignalChooserTextBlock)source).setText();
        }
        #endregion



    }
}
