using Abstractions;
using BaseDevice;
using CommandProcessing;
using DeviceBuilding;
using Extensions;
using KaLEDoscope.Views;
using Microsoft.Win32;
using ServiceInterfaces;
using System;
using UiCommands;
using Input = System.Windows.Input;

namespace KaLEDoscope.ViewModel
{
    public class CustomizationViewModel : Notified
    {
        public DeviceNode DeviceNode { get; private set; }
        private readonly ILogger _logger;
        private readonly DeviceFactory _deviceFactory;
        private readonly Invoker _invoker;
        private readonly ICompressor _compressor;
        private readonly INetworkAgent _networkAgent;
        private const string _defaultStructureFileExtension = ".device";
        private const string _defaultStructureFilter = "Device file (.device)|*.device|All files (*.*)|*.*";

        public event EventHandler<DeviceNode> OnNodeRenamed;
        public event EventHandler<DeviceNode> BeforeGettingSettings;
        public event EventHandler<Device> AfterGetingSettings;

        public CustomizationViewModel(
            DeviceNode deviceNode,
            DeviceFactory deviceFactory,
            Invoker invoker,
            ICompressor compressor,
            INetworkAgent networkAgent,
            ILogger logger)
        {
            DeviceNode = deviceNode;
            _logger = logger;
            _networkAgent = networkAgent;
            _deviceFactory = deviceFactory;
            _invoker = invoker;
            _compressor = compressor;
        }

        private DelegateCommand _commonSettings;
        public Input.ICommand CommonSettings
        {
            get
            {
                if (_commonSettings.IsNull())
                {
                    _commonSettings = new DelegateCommand((o) =>
                    {
                        var deviceNode = o as DeviceNode;
                        var baseDeviceViewModel = new CommonSettingsViewModel(deviceNode.Device, _logger);
                        var commonSettingsWindow = new CommonSettingsWindow(baseDeviceViewModel);
                        baseDeviceViewModel.OnRenamed += ((device) =>
                        {
                            deviceNode.Name = device.Name;
                            OnNodeRenamed?.Invoke(this, deviceNode);
                        });
                        commonSettingsWindow.ShowDialog();
                    });
                }
                return _commonSettings;
            }
        }

        private DelegateCommand<DeviceNode> _downloadSettings;
        public Input.ICommand DownloadSettings
        {
            get
            {
                if (_downloadSettings.IsNull())
                {
                    _downloadSettings = new DelegateCommand<DeviceNode>((d) =>
                    {
                        d.AllowUpload = true;
                        BeforeGettingSettings?.Invoke(this, d);
                        var configurationService = new ConfigurationService(_networkAgent, _deviceFactory, _logger);
                        configurationService.DownloadSettings(d.Device);
                        AfterGetingSettings?.Invoke(this, d.Device);
                    });
                }
                return _downloadSettings;
            }
        }

        private DelegateCommand<DeviceNode> _uploadSettings;
        public Input.ICommand UploadSettings
        {
            get
            {
                if (_uploadSettings.IsNull())
                {
                    _uploadSettings = new DelegateCommand<DeviceNode>((d) =>
                    {
                        var configurationService = new ConfigurationService(_networkAgent, _deviceFactory, _logger);
                        configurationService.UploadSettings(d.Device);
                    });
                }
                return _uploadSettings;
            }
        }

        private DelegateCommand<DeviceNode> _saveSettings;
        public Input.ICommand SaveSettings
        {
            get
            {
                if (_saveSettings.IsNull())
                {
                    _saveSettings = new DelegateCommand<DeviceNode>((d) =>
                    {
                        var dialog = new SaveFileDialog
                        {
                            FileName = d.Device.Name,
                            DefaultExt = _defaultStructureFileExtension,
                            Filter = _defaultStructureFilter
                        };
                        if (dialog.ShowDialog() == true)
                        {
                            var fileName = dialog.FileName;
                            var serialized = _deviceFactory.SerializeDevice(d.Device);
                            var bytes = _compressor.Zip(serialized);
                            System.IO.File.WriteAllBytes(fileName, bytes);
                        }
                    });
                }
                return _saveSettings;
            }
        }

        private DelegateCommand<DeviceNode> _loadSettings;
        public Input.ICommand LoadSettings
        {
            get
            {
                if (_loadSettings.IsNull())
                {
                    _loadSettings = new DelegateCommand<DeviceNode>((d) =>
                    {
                        BeforeGettingSettings?.Invoke(this, d);
                        d.AllowUpload = true;
                        var dialog = new OpenFileDialog
                        {
                            DefaultExt = _defaultStructureFileExtension,
                            Filter = _defaultStructureFilter
                        };
                        if (dialog.ShowDialog() == true)
                        {
                            var fileName = dialog.FileName;
                            var bytes = System.IO.File.ReadAllBytes(fileName);
                            var text = _compressor.Unzip(bytes);
                            d.Device = _deviceFactory.DeserializeDevice(text);
                            AfterGetingSettings?.Invoke(this, d.Device);
                        }
                    });
                }
                return _loadSettings;
            }
        }
    }
}
