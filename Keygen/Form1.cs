using Activation;
using System;
using System.Windows.Forms;

namespace Keygen
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void PasteFromClipboardHandler(object sender, EventArgs e)
        {
            var iData = Clipboard.GetDataObject();
            if (iData.GetDataPresent(DataFormats.Text))
            {
                var text = (String)iData.GetData(DataFormats.Text);
                serialNumberInputControl1.SerialNumber = text;
            }
        }

        private void CopyToClipboardHandler(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(serialNumberInputControl2.SerialNumber);
        }

        private void GenerateActivationCodeHahdler(object sender, EventArgs e)
        {
            var licenseManager = new ActivationManager(null);
            serialNumberInputControl2.SerialNumber = licenseManager.GetFullyActivationKey(serialNumberInputControl1.SerialNumber);
        }

        private void GenerateTrialCodeHandler(object sender, EventArgs e)
        {
            var licenseManager = new ActivationManager(null);
            serialNumberInputControl2.SerialNumber = licenseManager.GetTrialActivationKey(serialNumberInputControl1.SerialNumber);
        }
    }
}
