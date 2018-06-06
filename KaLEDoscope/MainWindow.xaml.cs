using Compression;
using Logger;
using System;
using System.Windows;
using TcpExcange;
using UdpExcange;

namespace KaLEDoscope
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var logger = new SeviceLog();
            var compressor = new Compressor();
            var networkScanAgent = new UdpAgent();
            var networkExcangeAgent = new TcpAgent();
            var viewModel = new MainViewModel(logger, compressor, networkScanAgent, networkExcangeAgent);
            viewModel.ShowOptions += new EventHandler(OnShowOptions);
            viewModel.QuitApplication += new EventHandler(OnQuitApplication);
            DataContext = trvMenu.DataContext = viewModel;
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
