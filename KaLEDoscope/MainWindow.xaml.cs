using Compression;
using Logger;
using System;
using System.Windows;

namespace KaLEDoscope
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ApplicationInfrastructure _infrastructure = new ApplicationInfrastructure();
        public MainWindow()
        {
            InitializeComponent();
            var logger = new SeviceLog();
            var compressor = new Compressor();
            var viewModel = new MainViewModel(logger, compressor);
            viewModel.ShowOptions += new EventHandler(OnShowOptions);
            viewModel.QuitApplication += new EventHandler(OnQuitApplication);
            DataContext = trvMenu.DataContext = lbLog.DataContext = viewModel;
            _infrastructure.Tabs = tabControl.Items;
        }

        private void OnQuitApplication(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void OnShowOptions(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
