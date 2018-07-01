using Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SerialNumberInput
{
    /// <summary>
    /// Interaction logic for SerialNumberInputControl.xaml
    /// </summary>
    public partial class SerialNumberInputControl : UserControl
    {
        private int _textBoxIndex = 0;
        private List<TextBox> textBoxes;

        public static DependencyProperty SerialNumberProperty;

        static SerialNumberInputControl()
        {
            SerialNumberProperty = DependencyProperty.Register(
                nameof(SerialNumber),
                typeof(string),
                typeof(SerialNumberInputControl),
                new PropertyMetadata(
                    string.Empty, OnSerialNumberPropertyChanged));
        }

        private static void OnSerialNumberPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as SerialNumberInputControl;
            control.OnSerialPropertyChanged(e);
        }

        public SerialNumberInputControl()
        {
            InitializeComponent();
            textBoxes = new List<TextBox>
            {
                textBox1, textBox2, textBox3, textBox4, textBox5, textBox6, textBox7, textBox8
            };
            textBoxes.ForEach((textBox) =>
            {
                textBox.MouseLeftButtonDown += new MouseButtonEventHandler(TextBox_MouseClick);
                textBox.KeyDown += new KeyEventHandler(TextBox_KeyDown);
            });
            textBoxes.First().Focus();
        }

        public void OnSerialPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue.IsNull())
                return;
            var serial = e.NewValue as string;
            serial = Regex.Replace(serial, @"[^\dABCDEF]", string.Empty).ToUpper();

            var focusedTextBox = textBoxes.FirstOrDefault(t => t.IsFocused);
            var caretIndex = focusedTextBox.CaretIndex;

            var counter = 0;
            foreach (var textBox in textBoxes)
            {
                textBox.Text = string.Empty;
                var stringBuilder = new StringBuilder();
                for (int i = 0; i < 4 && counter <= serial.Length - 1; i++)
                {
                    stringBuilder.Append(serial[counter]);
                    counter++;
                }
                textBox.Text = stringBuilder.ToString();
            }

            focusedTextBox.Focus();
            focusedTextBox.CaretIndex = caretIndex == 0 ? 1 : caretIndex;
        }

        private bool _readOnly = false;
        [DefaultValue(false), Description("Controls whether the serial number in the edit control can be changed or not")]
        public bool ReadOnly
        {
            get
            {
                return _readOnly;
            }
            set
            {
                if (_readOnly != value)
                {
                    _readOnly = value;
                    textBoxes.ForEach(t => t.IsReadOnly = _readOnly);
                }
            }
        }

        [DefaultValue(""), Description("The serial number associated with the control")]
        public string SerialNumber
        {
            get
            {
                return GetValue(SerialNumberProperty) as string;
                //var text = String.Join("-", textBoxes.Where(t => !String.IsNullOrEmpty(t.Text)).Select(t => t.Text)).Replace("--", "-    -");
                //SetValue(SerialNumberProperty, text);
                //return GetValue(SerialNumberProperty).ToString();
            }
            set
            {
                SetValue(SerialNumberProperty, value);

                //SetValue(SerialNumberProperty, serial);
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            var textBox = sender as TextBox;

            var keys = new Dictionary<Key, char>
            {
                { Key.A, 'A' },
                { Key.B, 'B' },
                { Key.C, 'C' },
                { Key.D, 'D' },
                { Key.E, 'E' },
                { Key.F, 'F' },
                { Key.D0, '0' },
                { Key.D1, '1' },
                { Key.D2, '2' },
                { Key.D3, '3' },
                { Key.D4, '4' },
                { Key.D5, '5' },
                { Key.D6, '6' },
                { Key.D7, '7' },
                { Key.D8, '8' },
                { Key.D9, '9' },
                { Key.NumPad0, '0' },
                { Key.NumPad1, '1' },
                { Key.NumPad2, '2' },
                { Key.NumPad3, '3' },
                { Key.NumPad4, '4' },
                { Key.NumPad5, '5' },
                { Key.NumPad6, '6' },
                { Key.NumPad7, '7' },
                { Key.NumPad8, '8' },
                { Key.NumPad9, '9' },
            };

            e.Handled = true;

            if (!keys.Any(kvp => kvp.Key == e.Key))
            {

                return;
            }

            var pair = keys.FirstOrDefault(kvp => kvp.Key == e.Key);

            var keyChar = pair.Value;

            if (!String.IsNullOrEmpty(textBox.SelectedText))
            {
                textBox.Text = keyChar.ToString().ToUpper();
                textBox.SelectionStart = 1;
            }
            else
            if (textBox.Text.Length < 4)
            {
                keyChar = Char.ToUpper(keyChar);
                var position = textBox.CaretIndex;
                textBox.Text = textBox.Text.Insert(position, keyChar.ToString());
                textBox.CaretIndex = 4;
            }
            else
            {
                _textBoxIndex++;
                if (_textBoxIndex < textBoxes.Count)
                {
                    var nextTextBox = textBoxes[_textBoxIndex];
                    nextTextBox.Focus();
                    nextTextBox.Text = Char.ToUpper(keyChar).ToString();
                    textBox.CaretIndex = 4;
                }
            }
            SerialNumber = String.Join("-", textBoxes.Where(t => !String.IsNullOrEmpty(t.Text)).Select(t => t.Text)).Replace("--", "-    -");
        }

        private void TextBox_MouseClick(object sender, MouseButtonEventArgs e)
        {
            var textBox = sender as TextBox;
            _textBoxIndex = textBoxes.IndexOf(textBox);
            textBox.SelectAll();
        }
    }
}
