using Activation;
using Compression;
using KaLEDoscope.ViewModel;
using KaLEDoscope.Views;
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
        private readonly ActivationManager _activationManager;

        public MainWindow()
        {
            InitializeComponent();
            var logger = new SeviceLog();
            var compressor = new Compressor();
            var networkScanAgent = new UdpAgent();
            var networkExcangeAgent = new TcpAgent();
            _activationManager = new ActivationManager(compressor);
            var viewModel = new MainViewModel(logger, compressor, networkScanAgent, networkExcangeAgent,
                _activationManager);
            viewModel.ActivationRequired += new EventHandler(OnActivationRequired);
            viewModel.TrialExpired += new EventHandler(OnTrialExpired);
            viewModel.ShowOptions += new EventHandler(OnShowOptions);
            viewModel.QuitApplication += new EventHandler(OnQuitApplication);
            viewModel.ShowAbout += new EventHandler<ShowAboutEventArgs>(OnShowAbout);
            DataContext = trvMenu.DataContext = viewModel;
            viewModel.CheckActivation();
        }

        private void OnShowAbout(object sender, ShowAboutEventArgs e)
        {
            var aboutWindow = new AboutWindow(e.Version);
            aboutWindow.ShowDialog();
        }

        private void OnTrialExpired(object sender, EventArgs e)
        {
            if (MessageBox.Show("Пробный период истек.\r\n" +
                "Необходимо активировать приложение\r\n" +
                "Перейти к активации?", "Требуется активация", MessageBoxButton.YesNo, MessageBoxImage.Warning)
                == MessageBoxResult.No)
            {
                Close();
            }
            ProceedActivation();
        }

        private void OnActivationRequired(object sender, EventArgs e)
        {
            if (MessageBox.Show("Приложение не активировано.\r\n" +
                "Без активации работа приложения невозможна\r\n" +
                "Перейти к активации?", "Требуется активация", MessageBoxButton.YesNo, MessageBoxImage.Warning)
                == MessageBoxResult.No)
            {
                Close();
            }
            ProceedActivation();
        }

        private void ProceedActivation()
        {
            var activationViewModel = new ActivationViewModel(_activationManager);
            var activationWindow = new ActivationWindow(activationViewModel);
            activationViewModel.GenerateRequestCode();
            if (activationWindow.ShowDialog() == false)
                Close();
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
