using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;

namespace Keygen
{
    /// <summary>
    /// Элемент управления, позволяющий отображать серийный номер
    /// </summary>
    public partial class SerialNumberInputControl : UserControl
    {
        private string _allowedCharsPattern = @"\dabcdef";


        public SerialNumberInputControl()
        {
            InitializeComponent();
            textBoxes = new List<TextBox>
            {
            textBox1,textBox2,textBox3,textBox4,textBox5,textBox6,textBox7,textBox8
            };
            textBoxes.ForEach((textBox) =>
            {
                textBox.MouseClick += new MouseEventHandler(TextBox_MouseClick);
                textBox.KeyPress += new KeyPressEventHandler(TextBox_KeyPress);
            });
        }

        private bool _readOnly = false;

        /// <summary>
        /// Признак доступности данных только для чтения
        /// </summary>
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
                    textBoxes.ForEach(t => t.ReadOnly = _readOnly);
                }
            }
        }



        /// <summary>
        /// Содержит введённый серийный номер
        /// </summary>
        [DefaultValue(""), Description("The serial number associated with the control")]
        public string SerialNumber
        {
            get
            {
                var text = String.Join("-", textBoxes.Where(t => !String.IsNullOrEmpty(t.Text)).Select(t => t.Text)).Replace("--", "-    -");
                return text;
            }
            set
            {
                var serial = value.ToLower();
                serial = Regex.Replace(serial, $"[^{_allowedCharsPattern}]", string.Empty).ToUpper();

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
            }
        }

        private int _textBoxIndex = 0;
        private List<TextBox> textBoxes;

        /// <summary>
        /// Обработчик щелчка мышью по одному из полей ввода
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_MouseClick(object sender, MouseEventArgs e)
        {
            var textBox = sender as TextBox;
            _textBoxIndex = textBoxes.IndexOf(textBox);
            textBox.SelectAll();
        }

        /// <summary>
        /// Обработчик ввода очередного символа в одно из полей
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            var textBox = sender as TextBox;
            var keyChar = e.KeyChar;
            if (keyChar == (char)8)
            {
                return;
            }
            if (!Regex.IsMatch(keyChar.ToString().ToLower(), $"[{_allowedCharsPattern}]"))
            {
                e.Handled = true;
                return;
            }
            if (!String.IsNullOrEmpty(textBox.SelectedText))
            {
                textBox.Text = keyChar.ToString().ToUpper();
                textBox.SelectionStart = 1;
                e.Handled = true;
                return;
            }
            if (textBox.Text.Length < 4)
            {
                e.KeyChar = Char.ToUpper(keyChar);
                return;
            }
            else
            {
                _textBoxIndex++;
                if (_textBoxIndex < textBoxes.Count)
                {
                    var nextTextBox = textBoxes[_textBoxIndex];
                    nextTextBox.Focus();
                    nextTextBox.Text = Char.ToUpper(keyChar).ToString();
                    nextTextBox.SelectionStart = 1;
                }
            }
            e.Handled = true;
        }
    }
}
