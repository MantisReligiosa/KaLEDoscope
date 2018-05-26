using Compression;
using JsonExchange;
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
        private ApplicationInfrastructure _infrastructure = new ApplicationInfrastructure();
        public MainWindow()
        {
            InitializeComponent();
            var logger = new SeviceLog();
            var compressor = new Compressor();
            var networkScanAgent = new UdpAgent();
            var networkExcangeAgent = new TcpAgent();
            var requestBuilder = new JsonRequestBuilder();
            var responceProcessor = new JsonResponceProcessor();
            var viewModel = new MainViewModel(logger, compressor, networkScanAgent, networkExcangeAgent, requestBuilder, responceProcessor);
            viewModel.ShowOptions += new EventHandler(OnShowOptions);
            viewModel.QuitApplication += new EventHandler(OnQuitApplication);
            DataContext = trvMenu.DataContext = viewModel;
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
