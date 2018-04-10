using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using Xceed.Wpf.Toolkit;

namespace SevenSegmentBoardDevice.UI
{
    /// <summary>
    /// Interaction logic for TimerControl.xaml
    /// </summary>
    public partial class TimerControl : UserControl
    {
        public TimerControl()
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
