using BaseDevice;
using CommandProcessing.Commands;
using Configuration;
using KaLEDoscope.ViewModel;
using KaLEDoscope.Views;
using Logger;
using ServiceInterfaces;
using SmartTechnologiesM.Activation;
using System;
using System.Collections.Generic;
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
        private readonly IActivationManager _activationManager;
        private readonly MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            var logger = new SeviceLog();
            var config = Config.GetConfig();
            var compressor = new Compressor();
            var networkExcangeAgent = new TcpAgent();
            var networkScanAgent = new UdpAgent(config.ScanInterfaceNumber);
            var activationFile = new ActivationFile();
            var hardwareInfoProvider = new HardwareInfoProvider();
            _activationManager = new ActivationManager(config.Key, config.IV, compressor, activationFile, hardwareInfoProvider);
            var uploadingCommandsContainer = new List<Func<Device, INetworkAgent, ILogger, IConfig, IDeviceCommand<Device>>>
                        {
                            //(d, n, l, c) => new UploadIdentityCommand(d, n, l, c),
                            (d, n, l, c) => new UploadNetworkCommand(d, n, l, c),
                            (d, n, l, c) => new UploadWorkScheduleCommand(d, n, l, c),
                            (d, n, l, c) => new UploadBrightnessCommand(d, n, l, c),
                            (d, n, l, c) => new SyncTimeCommand(d, n, l, c)
                        };
            _viewModel = new MainViewModel(uploadingCommandsContainer, logger, config, compressor, networkExcangeAgent, networkScanAgent,
                _activationManager);
            _viewModel.ActivationRequired += new EventHandler(OnActivationRequired);
            _viewModel.TrialExpired += new EventHandler(OnTrialExpired);
            _viewModel.ShowOptions += new EventHandler(OnShowOptions);
            _viewModel.NewStructure += new EventHandler(OnNewStructure);
            _viewModel.QuitApplication += new EventHandler(OnQuitApplication);
            _viewModel.ShowAbout += new EventHandler<ShowAboutEventArgs>(OnShowAbout);
            _viewModel.ShowPreview += new EventHandler<ShowPreviewEventArgs>(OnShowPreview);
            DataContext = trvMenu.DataContext = _viewModel;
            _viewModel.CheckActivation();
        }

        private void OnNewStructure(object sender, EventArgs e)
        {
            if (MessageBox.Show("Создать новую структуру?\r\n" +
                "Все несохранённые данные могут быть утеряны", "Требуется подверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning)
                == MessageBoxResult.Yes)
            {
                _viewModel.ProceedClearStructure();
            }
        }

        private void OnShowPreview(object sender, ShowPreviewEventArgs e)
        {
            var previewWindow = new PreviewWindow
            {
                DataContext = e.ViewModel,
                Owner = this
            };
            previewWindow.LoadPreviewControl(e.PreviewControl);
            previewWindow.ShowDialog();
        }

        private void OnShowAbout(object sender, ShowAboutEventArgs e)
        {
            var aboutWindow = new AboutWindow(e.Version);
            aboutWindow.ShowDialog();
        }

        private void OnTrialExpired(object sender, EventArgs e)
        {
            if (MessageBox.Show("Срок активации истек.\r\n" +
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
            var configModel = new ConfigViewModel
            {
                AutosavePeriod = _viewModel.AutosavePeriod / 1000,
                AutosaveFilename = _viewModel.AutosaveFileName,
                ScanPort = Config.GetConfig().ScanPort,
                ScanPeriod = Config.GetConfig().ScanPeriod,
                RequestPort = Config.GetConfig().RequestPort,
                ResponceTimeout = Config.GetConfig().ResponceTimeout
            };
            var configWindow = new ConfigWindow
            {
                DataContext = configModel,
                Owner = this
            };
            if (configWindow.ShowDialog() == true)
            {
                _viewModel.AutosavePeriod = configModel.AutosavePeriod * 1000;
                _viewModel.AutosaveFileName = configModel.AutosaveFilename;
                Config.GetConfig().ScanPort = configModel.ScanPort;
                Config.GetConfig().ScanPeriod = configModel.ScanPeriod;
                Config.GetConfig().RequestPort = configModel.RequestPort;
                Config.GetConfig().ResponceTimeout = configModel.ResponceTimeout;
            }
        }
    }
}
