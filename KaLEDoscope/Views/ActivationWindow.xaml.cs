using KaLEDoscope.ViewModel;
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
using System.Windows.Shapes;

namespace KaLEDoscope.Views
{
    /// <summary>
    /// Interaction logic for ActivationWindow.xaml
    /// </summary>
    public partial class ActivationWindow : Window
    {
        private ActivationViewModel _activationViewModel;
        public ActivationWindow(ActivationViewModel viewModel)
        {
            InitializeComponent();
            _activationViewModel = viewModel;
            _activationViewModel.TrialActivationSucceeded += TrialActivationSucceeded;
            _activationViewModel.CopyToClipboard += CopyToClipboard;
            _activationViewModel.PasteFromClipboard += PasteFromClipboard;
            DataContext = _activationViewModel;
        }

        private void PasteFromClipboard(object sender, EventArgs e)
        {
            var iData = Clipboard.GetDataObject();
            if (iData.GetDataPresent(DataFormats.Text))
            {
                var text = (String)iData.GetData(DataFormats.Text);
                _activationViewModel.ActivationKey = text;
            }
        }

        private void CopyToClipboard(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(_activationViewModel.RequestCode);
        }

        private void TrialActivationSucceeded(object sender, TrialExpirationEventArgs e)
        {
            MessageBox.Show("Активация выполнена успешна!\r\n" +
                $"Ключ активации действителен до {e.ExpirationDate:dd.MM.yyyy}",
                "Успешно", 
                MessageBoxButton.OK, 
                MessageBoxImage.Information);
            DialogResult = true;
            Close();
        }
    }
}
