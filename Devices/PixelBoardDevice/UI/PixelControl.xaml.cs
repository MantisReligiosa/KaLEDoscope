using System;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit;

namespace PixelBoardDevice.UI
{
    /// <summary>
    /// Interaction logic for PixelControl.xaml
    /// </summary>
    public partial class PixelControl : UserControl
    {
        public PixelControl()
        {
            InitializeComponent();
        }


        private void ButtonSpinner_Spin(object sender, SpinEventArgs e)
        {
            ButtonSpinner spinner = (ButtonSpinner)sender;
            TextBox txtBox = (TextBox)spinner.Content;

            int value = 0;
            if (Int32.TryParse(txtBox.Text, out int parsed))
            {
                value = parsed;
            }

            if (e.Direction == SpinDirection.Increase)
                value++;
            else
                value--;
            txtBox.Text = value.ToString();
        }
    }
}
