using KaLEDoscope.ViewModel;
using System.Windows;

namespace KaLEDoscope.Views
{
    /// <summary>
    /// Interaction logic for RenameDialog.xaml
    /// </summary>
    public partial class RenameDialog : Window
    {
        private readonly RenameDialogViewModel _model;
        public RenameDialog()
        {
            InitializeComponent();
            _model = new RenameDialogViewModel();
            DataContext = _model;
        }
        public string NameField
        {
            get
            {
                return _model.NameField;
            }
            set
            {
                _model.NameField = value;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
