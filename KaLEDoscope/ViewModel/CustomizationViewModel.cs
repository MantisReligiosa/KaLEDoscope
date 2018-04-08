using Abstractions;
using BaseDevice;
using CommandProcessing;
using DeviceBuilding;
using KaLEDoscope.Views;
using Microsoft.Win32;
using Newtonsoft.Json;
using ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Input = System.Windows.Input;

namespace KaLEDoscope.ViewModel
{
    public class CustomizationViewModel : Notified
    {
        public DeviceNode DeviceNode { get; private set; }
        private readonly ILogger _logger;
        private readonly DeviceFactory _deviceFactory;
        private readonly Invoker _invoker;

        public event EventHandler<DeviceNode> OnNodeRenamed;
        public event EventHandler<DeviceNode> BeforeGettingSettings;
        public event EventHandler<Device> AfterGetingSettings;

        public CustomizationViewModel(DeviceNode deviceNode, DeviceFactory deviceFactory, Invoker invoker, ILogger logger)
        {
            DeviceNode = deviceNode;
            _logger = logger;
            _deviceFactory = deviceFactory;
            _invoker = invoker;
        }

        private DelegateCommand _commonSettings;
        public Input.ICommand CommonSettings
        {
            get
            {
                if (_commonSettings == null)
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
                if (_downloadSettings == null)
                {
                    _downloadSettings = new DelegateCommand<DeviceNode>((d) =>
                    {
                    d.AllowUpload = true;
                    BeforeGettingSettings?.Invoke(this, d);
                    var command = new DirectConnectDownloadSettingsCommand(d.Device, _deviceFactory, _logger);
                    command.OnConfigurationDownloaded += ((sender, device) => AfterGetingSettings?.Invoke(this, device));
                        _invoker.Invoke(command);
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
                if (_uploadSettings == null)
                {
                    _uploadSettings = new DelegateCommand<DeviceNode>((d) =>
                    {
                        var command = new DirectConnectUploadSettingsCommand(d.Device, _logger);
                        _invoker.Invoke(command);
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
                if (_saveSettings == null)
                {
                    _saveSettings = new DelegateCommand<DeviceNode>((d) =>
                    {
                        var dialog = new SaveFileDialog
                        {
                            FileName = d.Device.Name,
                            DefaultExt = ".json",
                            Filter = "JSON configuration (.json)|*.json"
                        };
                        if (dialog.ShowDialog() == true)
                        {
                            var fileName = dialog.FileName;
                            var serialized = JsonConvert.SerializeObject(d.Device);
                            System.IO.File.WriteAllText(fileName, serialized);
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
                if (_loadSettings == null)
                {
                    _loadSettings = new DelegateCommand<DeviceNode>((d) =>
                    {
                        BeforeGettingSettings?.Invoke(this, d);
                        d.AllowUpload = true;
                        var dialog = new OpenFileDialog
                        {
                            DefaultExt = ".json",
                            Filter = "JSON configuration (.json)|*.json"
                        };
                        if (dialog.ShowDialog() == true)
                        {
                            var fileName = dialog.FileName;
                            var text = System.IO.File.ReadAllText(fileName);
                            var device = JsonConvert.DeserializeObject(text, d.Device.GetType());
                            d.Device = _deviceFactory.Customize(device);
                            AfterGetingSettings?.Invoke(this, d.Device);
                        }
                    });
                }
                return _loadSettings;
            }
        }
    }
}
