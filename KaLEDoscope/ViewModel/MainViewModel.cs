using BaseDevice;
using CommandProcessing;
using KaLEDoscope.ViewModel;
using KaLEDoscope.Views;
using Microsoft.Win32;
using Newtonsoft.Json;
using ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Input = System.Windows.Input;
using SevenSegmentBoardDevice;
using DeviceBuilding;
using Abstractions;
using PixelBoardDevice;

namespace KaLEDoscope
{
    public class MainViewModel : Notified
    {
        private ILogger _logger { get; set; }
        private ProtocolNode _directConnect;
        private readonly Dispatcher _dispatcher;
        private readonly DeviceFactory _deviceFactory;
        private readonly Invoker _invoker;

        public ObservableCollection<ProtocolNode> ProtocolNodes { get; set; } = new ObservableCollection<ProtocolNode>();
        public ObservableCollection<TabItem> DeviceTabs { get; set; } = new ObservableCollection<TabItem>();

        private TabItem _selectedTabItem;
        public TabItem SelectedTabItem
        {
            get
            {
                return _selectedTabItem;
            }
            set
            {
                _selectedTabItem = value;
                OnPropertyChanged(nameof(SelectedTabItem));
            }
        }

        private StringBuilder _log = new StringBuilder();
        public string Log
        {
            get
            {
                return _log.ToString();
            }
            set
            {
                _log = new StringBuilder(value);
                OnPropertyChanged(nameof(Log));
            }
        }

        private bool _isScanEnabled;
        public bool IsScanEnabled
        {
            get
            {
                return _isScanEnabled;
            }
            set
            {
                _isScanEnabled = value;
                OnPropertyChanged(nameof(IsScanEnabled));
            }
        }

        public Dictionary<Func<DeviceNode>, Func<CustomDeviceControl>> CustomControls { get; set; }

        public MainViewModel(ILogger logger)
        {
            _logger = logger;
            _logger.InfoRaised += (sender, message) => LogMessage($"{sender}: Info: {message}");
            _logger.DebugRaised += (sender, message) => LogMessage($"{sender}: Debug: {message}");
            _logger.WarnRaised += (sender, message) => LogMessage($"{sender}: Warn: {message}");
            _logger.ErrorRaised += (sender, message) => LogMessage($"{sender}: Error: {message}");
            _dispatcher = Dispatcher.CurrentDispatcher;
            _invoker = new Invoker(_logger);
            IsScanEnabled = true;
            _deviceFactory = new DeviceFactory(_logger);
            _deviceFactory.AddBuilder(new SevenSegmentDeviceBuilder());
            _deviceFactory.AddBuilder(new PixelDeviceBuilder());
        }

        public void MakeNodes()
        {
            var directConnectDeviceScanner = new DeviceScanner(_logger, _deviceFactory);
            directConnectDeviceScanner.OnScanCompleted += DirectConnectDeviceScanner_OnScanCompleted;
            directConnectDeviceScanner.StartSearch();
            IsScanEnabled = false;

            var mqtt = new ProtocolNode
            {
                Name = "MQTT",
            };
            _directConnect = new ProtocolNode
            {
                Name = "DirectConnect",
            };

            ProtocolNodes.Add(mqtt);
            ProtocolNodes.Add(_directConnect);
        }

        private void DirectConnectDeviceScanner_OnScanCompleted(List<Device> devices)
        {
            _dispatcher.Invoke(() =>
            {
                foreach (var device in devices)
                {
                    _directConnect.Members.Add(new DeviceNode
                    {
                        Device = device,
                        Name = device.Name
                    });
                }
            });
            IsScanEnabled = true;
        }

        public void ClearNodes()
        {
            ProtocolNodes.Clear();
        }

        private DelegateCommand showDevicePlugin;
        public Input.ICommand ShowDevicePlugin
        {
            get
            {
                if (showDevicePlugin == null)
                {
                    showDevicePlugin = new DelegateCommand((o) =>
                    {
                        if (!(o is DeviceNode))
                            return;
                        var deviceNode = (DeviceNode)o;
                        var existTab = DeviceTabs.FirstOrDefault(t => (Device)t.DataContext == deviceNode.Device);
                        if (existTab == null)
                        {
                            var grid = GetGrid(deviceNode);
                            var newTabItem = new ClosableTab
                            {
                                Title = $"{deviceNode.Device.Name} id:{deviceNode.Device.Id}",
                                Content = grid,
                                DataContext = deviceNode.Device,
                            };
                            newTabItem.OnTabCloseClick += (sender, arguments) =>
                            {
                                var tab = (ClosableTab)sender;
                                DeviceTabs.Remove(tab);
                            };
                            DeviceTabs.Add(newTabItem);
                            SelectedTabItem = newTabItem;
                        }
                        else
                        {
                            SelectedTabItem = existTab;
                        }
                    });
                }
                return showDevicePlugin;
            }
        }

        private Grid GetGrid(DeviceNode deviceNode)
        {
            var tabControl = new TabControl();
            var baseTabItem = new TabItem();
            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition
            {
                Height = GridLength.Auto
            });
            grid.RowDefinitions.Add(new RowDefinition
            {
                Height = new GridLength(1, GridUnitType.Star)
            });

            var baseDeviceViewModel = new BaseDeviceViewModel(deviceNode.Device, _logger);
            baseDeviceViewModel.OnRenamed += ((device) => deviceNode.Name = device.Name);
            baseTabItem.Content = new BaseDeviceControl
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                DataContext = baseDeviceViewModel
            };
            baseTabItem.Header = "Базовые настройки";
            tabControl.Items.Add(baseTabItem);
            tabControl.TabStripPlacement = Dock.Left;
            var toolbar = new ToolBar();
            toolbar.Items.Add(new Button
            {
                Content = "Синхронизировать",
                Command = DownloadSettings,
                CommandParameter = deviceNode
            });
            toolbar.Items.Add(new Button
            {
                Content = "Применить конфигурацию",
                Command = UploadSettings,
                CommandParameter = deviceNode,
                IsEnabled = deviceNode.AllowUpload
            });
            toolbar.Items.Add(new Button
            {
                Content = "Сохранить конфигурацию в файл",
                Command = ExportSettings,
                CommandParameter = deviceNode,
                IsEnabled = deviceNode.AllowUpload
            });
            toolbar.Items.Add(new Button
            {
                Content = "Загрузить конфигурацию из файла",
                Command = ImportSettings,
                CommandParameter = deviceNode
            });
            grid.Children.Add(tabControl);
            grid.Children.Add(toolbar);
            Grid.SetRow(tabControl, 1);
            Grid.SetRow(toolbar, 0);

            var deviceBuilder = _deviceFactory.GetBuilder(deviceNode.Device.Model);
            if (deviceBuilder != null)
            {
                foreach (var customDevicesControlsKeyValuePair in deviceBuilder.Controls)
                {
                    var customTabItem = new TabItem();
                    var customControl = customDevicesControlsKeyValuePair.Value(deviceNode.Device, _logger);
                    customTabItem.Content = customControl;
                    customTabItem.Header = customDevicesControlsKeyValuePair.Key;
                    tabControl.Items.Add(customTabItem);
                }
            }
            return grid;
        }

        private DelegateCommand _scanDevices;
        public Input.ICommand ScanDevices
        {
            get
            {
                if (_scanDevices == null)
                {
                    _scanDevices = new DelegateCommand((o) =>
                      {
                          ClearNodes();
                          MakeNodes();
                      });
                }
                return _scanDevices;
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

        private DelegateCommand<DeviceNode> _exportSettings;
        public Input.ICommand ExportSettings
        {
            get
            {
                if (_exportSettings == null)
                {
                    _exportSettings = new DelegateCommand<DeviceNode>((d) =>
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
                return _exportSettings;
            }
        }

        private DelegateCommand<DeviceNode> _downloadSettings;
        private DeviceNode _updatedNode;
        private TabItem _tabItem;
        public Input.ICommand DownloadSettings
        {
            get
            {
                if (_downloadSettings == null)
                {
                    _downloadSettings = new DelegateCommand<DeviceNode>((d) =>
                    {
                        d.AllowUpload = true;
                        _updatedNode = d;
                        _tabItem = DeviceTabs.FirstOrDefault(t => (Device)t.DataContext == d.Device);
                        var command = new DirectConnectDownloadSettingsCommand(d.Device, _deviceFactory, _logger);
                        command.OnConfigurationDownloaded += Command_OnConfigurationDownloaded;
                        _invoker.Invoke(command);
                    });
                }
                return _downloadSettings;
            }
        }

        private void Command_OnConfigurationDownloaded(Device obj)
        {
            _updatedNode.Device = obj;
            _dispatcher.Invoke(() => _tabItem.Content = GetGrid(_updatedNode));
        }

        private DelegateCommand<DeviceNode> _importSettings;
        public Input.ICommand ImportSettings
        {
            get
            {
                if (_importSettings == null)
                {
                    _importSettings = new DelegateCommand<DeviceNode>((d) =>
                    {
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
                            Command_OnConfigurationDownloaded(d.Device);
                        }
                    });
                }
                return _importSettings;
            }
        }

        private void LogMessage(string Message)
        {
            _log.AppendLine(Message);
            OnPropertyChanged(nameof(Log));
        }
    }
}
