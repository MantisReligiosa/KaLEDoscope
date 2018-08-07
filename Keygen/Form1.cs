using Activation;
using Common;
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
            var licenseManager = new ActivationManager(null, null, null);
            var requestCode = serialNumberInputControl1.SerialNumber;
            var data = DateTime.Now;
            if (radioButton1.Checked)
                data = data.AddDays((int)numericUpDown1.Value);
            if (radioButton2.Checked)
                data = data.AddMonths((int)numericUpDown2.Value);
            if (radioButton3.Checked)
                data = data.AddYears((int)numericUpDown3.Value);
            var licenseInfo = new LicenseInfo
            {
                ExpirationDate = data,
                RequestCode = requestCode
            };
            serialNumberInputControl2.SerialNumber = licenseManager.GetActivationKey(licenseInfo);
        }

        private void CheckedChanged(object sender, EventArgs e)
        {
            numericUpDown1.Enabled = radioButton1.Checked;
            numericUpDown2.Enabled = radioButton2.Checked;
            numericUpDown3.Enabled = radioButton3.Checked;
        }
    }
}
