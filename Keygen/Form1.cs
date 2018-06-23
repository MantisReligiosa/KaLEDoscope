using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SetupManagers = Setups.Common.Managers;

namespace Keygen
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Обработчик нажатия на кнопку "Paste"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PasteFromClipboardHandler(object sender, EventArgs e)
        {
            var iData = Clipboard.GetDataObject();
            if (iData.GetDataPresent(DataFormats.Text))
            {
                var text = (String)iData.GetData(DataFormats.Text);
                serialNumberInputControl1.SerialNumber = text;
            }
        }

        /// <summary>
        /// Обработчик нажатия на кнопку "Copy"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CopyToClipboardHandler(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(serialNumberInputControl2.SerialNumber);
        }

        /// <summary>
        /// Обработчик нажатия на кнопку "Generate"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GenerateActivationCodeHahdler(object sender, EventArgs e)
        {
            var licenseManager = new SetupManagers.LicenseManager();
            serialNumberInputControl2.SerialNumber = licenseManager.GetActivationCode(serialNumberInputControl1.SerialNumber);
        }
    }
}
