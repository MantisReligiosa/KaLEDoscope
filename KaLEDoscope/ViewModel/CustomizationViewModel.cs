using BaseDevice;
using CommandProcessing;
using CommandProcessing.Commands;
using DeviceBuilding;
using KaLEDoscope.Views;
using Microsoft.Win32;
using ServiceInterfaces;
using SmartTechnologiesM.Activation;
using SmartTechnologiesM.Base;
using SmartTechnologiesM.Base.Extensions;
using System;
using System.Collections.Generic;
using UiCommands;
using Input = System.Windows.Input;

namespace KaLEDoscope.ViewModel
{
    public class CustomizationViewModel : Notified
    {
        public DeviceNode DeviceNode { get; private set; }
        private readonly List<Func<Device, INetworkAgent, ILogger, IConfig, IDeviceCommand<Device>>> _uploadingCommandsContainer;
        private readonly ILogger _logger;
        private readonly IConfig _config;
        private readonly DeviceFactory _deviceFactory;
        private readonly ICompressor _compressor;
        private readonly INetworkAgent _networkAgent;
        private const string _defaultStructureFileExtension = ".device";
        private const string _defaultStructureFilter = "Device file (.device)|*.device|All files (*.*)|*.*";

        public event EventHandler<DeviceNode> NodeRenamed;
        public event EventHandler<DeviceNode> BeforeGettingSettings;
        public event EventHandler<Device> AfterGetingSettings;
        public event EventHandler MouseUpEvent;
        public event EventHandler<Device> ShowPreview;

        public CustomizationViewModel(
            DeviceNode deviceNode,
            DeviceFactory deviceFactory,
            List<Func<Device, INetworkAgent, ILogger, IConfig, IDeviceCommand<Device>>> uploadingCommandsContainer,
            ICompressor compressor,
            INetworkAgent networkAgent,
            ILogger logger,
            IConfig config)
        {
            DeviceNode = deviceNode;
            _logger = logger;
            _config = config;
            _networkAgent = networkAgent;
            _deviceFactory = deviceFactory;
            _compressor = compressor;
            _uploadingCommandsContainer = uploadingCommandsContainer;
        }

        private DelegateCommand _preview;
        public Input.ICommand Preview
        {
            get
            {
                if (_preview.IsNull())
                {
                    _preview = new DelegateCommand((o) =>
                    {
                        var deviceNode = o as DeviceNode;
                        ShowPreview?.Invoke(this, deviceNode.Device);
                    });
                }
                return _preview;
            }
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
                        var baseDeviceViewModel = new CommonSettingsViewModel(deviceNode.Device);
                        var commonSettingsWindow = new CommonSettingsWindow(baseDeviceViewModel);
                        baseDeviceViewModel.OnRenamed += ((device) =>
                        {
                            deviceNode.Name = device.Name;
                            NodeRenamed?.Invoke(this, deviceNode);
                        });
                        commonSettingsWindow.ShowDialog();
                    });
                }
                return _commonSettings;
            }
        }

        private DelegateCommand<DeviceNode> _uploadSettings;
        public Input.ICommand UploadSettings
        {
            get
            {
                if (_uploadSettings.IsNull())
                {
                    _uploadSettings = new DelegateCommand<DeviceNode>((deviceNode) =>
                    {
                        var configurationService = new ConfigurationService(_uploadingCommandsContainer, _networkAgent, _deviceFactory, _logger, _config);
                        configurationService.UploadSettings(deviceNode.Device);
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

        private DelegateCommand _mouseUp;
        public Input.ICommand MouseUp
        {
            get
            {
                if (_mouseUp.IsNull())
                {
                    _mouseUp = new DelegateCommand((d) =>
                    {
                        MouseUpEvent?.Invoke(this, EventArgs.Empty);
                    });
                }
                return _mouseUp;
            }
        }
    }
}
