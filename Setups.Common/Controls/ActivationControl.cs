using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Setups.Common.Controls
{
    /// <summary>
    /// Элемент управления, предоставляющий поля для ввода данных активации
    /// </summary>
    public partial class ActivationControl : UserControl
    {
        /// <summary>
        /// Поле, содержащее код запроса
        /// </summary>
        [Description("The request ID associated with control"), Category("Activation")]
        public string RequestId
        {
            get { return serialNumberInputControl1.SerialNumber; }
            set { serialNumberInputControl1.SerialNumber = value; }
        }

        /// <summary>
        /// Поле, содержащее код активации
        /// </summary>
        [Description("The activation code associated with control"), Category("Activation")]
        public string ActivationCode
        {
            get { return serialNumberInputControl2.SerialNumber; }
            set { serialNumberInputControl2.SerialNumber = value; }
        }

        public ActivationControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Обработчик копирования кода запроса на активацию в буфер обмена
        /// </summary>
        /// <param name="sender">Объект инициатора события</param>
        /// <param name="e">Аргументы события</param>
        private void CopyRequestCodeToClipboardHandler(object sender, System.EventArgs e)
        {
            Clipboard.SetDataObject(serialNumberInputControl1.SerialNumber);
        }

        /// <summary>
        /// Обработчик вставки кода запроса из буфера обмена
        /// </summary>
        /// <param name="sender">Объект инициатора события</param>
        /// <param name="e">Аргументы события</param>
        private void PasteActivationCodeFromClipboardHandler(object sender, System.EventArgs e)
        {
            var iData = Clipboard.GetDataObject();
            if (iData.GetDataPresent(DataFormats.Text))
            {
                var text = (String)iData.GetData(DataFormats.Text);
                serialNumberInputControl2.SerialNumber = text;
            }
        }
    }
}
