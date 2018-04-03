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
using KaLEDoscope.Properties;
using System.Windows.Media;

namespace KaLEDoscope
{
    public class MainViewModel : Notified
    {
        private ILogger _logger { get; set; }
        private readonly Dispatcher _dispatcher;
        private readonly DeviceFactory _deviceFactory;
        private readonly Invoker _invoker;

        public ObservableCollection<NodeItem> StructureNodes { get; set; } = new ObservableCollection<NodeItem>();
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

        public bool IsScanEnabled { get; set; }

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
            _deviceFactory.Builders.Add(new SevenSegmentDeviceBuilder());
            _deviceFactory.Builders.Add(new PixelDeviceBuilder());

            var folderNode = new FolderNode
            {
                Name = "Папка",
            };

            var aggregationNode = new AggregationNode
            {
                Name = "Аггрегатор"
            };

            StructureNodes.Add(folderNode);
            StructureNodes.Add(aggregationNode);
            var id = int.MaxValue;
            _deviceFactory.Builders.ForEach(builder =>
            {
                var device = builder.UpdateCustomSettings(new Device
                {
                    Id = id--,
                    Model = builder.Model,
                    IsStandaloneConfiguration = true
                });
                folderNode.AddChildNode(new DeviceNode
                {
                    Device = device,
                    Name = builder.GetControls(device, logger).CustomizationControls.FirstOrDefault().Key,
                    AllowDownload = false,
                    AllowLoad = true,
                    AllowSave = true,
                    AllowUpload = false
                });
                device = builder.UpdateCustomSettings(new Device
                {
                    Id = id--,
                    Model = builder.Model,
                    IsStandaloneConfiguration = true
                });
                aggregationNode.AddChildNode(new DeviceNode
                {
                    Device = device,
                    Name = builder.GetControls(device, logger).CustomizationControls.FirstOrDefault().Key,
                    AllowDownload = false,
                    AllowLoad = true,
                    AllowSave = true,
                    AllowUpload = false
                });
                device = builder.UpdateCustomSettings(new Device
                {
                    Id = id--,
                    Model = builder.Model,
                    IsStandaloneConfiguration = true
                });
                StructureNodes.Add(new DeviceNode
                {
                    Device = device,
                    Name = builder.GetControls(device, logger).CustomizationControls.FirstOrDefault().Key,
                    AllowDownload = false,
                    AllowLoad = true,
                    AllowSave = true,
                    AllowUpload = false
                });
            });
        }

        public void MakeNodes()
        {
            var directConnectDeviceScanner = new DeviceScanner(_logger, _deviceFactory);
            directConnectDeviceScanner.OnScanCompleted += DirectConnectDeviceScanner_OnScanCompleted;
            directConnectDeviceScanner.StartSearch();
            IsScanEnabled = false;
        }

        private void DirectConnectDeviceScanner_OnScanCompleted(List<Device> devices)
        {
            _dispatcher.Invoke(() =>
            {
                foreach (var device in devices)
                {
                    StructureNodes.Add(new DeviceNode
                    {
                        Device = device,
                        Name = device.Name,
                        AllowDownload = true,
                        AllowLoad = true,
                        AllowSave = true,
                        AllowUpload = true
                    });
                }
            });
            IsScanEnabled = true;
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
                        var nodeItem = o as NodeItem;
                        if (nodeItem is AggregationNode aggregationNode)
                        {
                            ProcessAggregator(aggregationNode);
                        }
                        else if (nodeItem.Parent is AggregationNode parentAggregationNode)
                        {
                            ProcessAggregator(parentAggregationNode);
                        }
                        else if (nodeItem is DeviceNode deviceNode)
                        {
                            var existTab = DeviceTabs.FirstOrDefault(t => (Device)t.DataContext == deviceNode.Device);
                            if (existTab != null)
                            {
                                SelectedTabItem = existTab;
                                return;
                            }
                            var grid = GetGrid(deviceNode);
                            var newTabItem = new ClosableTab
                            {
                                Title = GetTabItemTitle(deviceNode),
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
                    });
                }
                return showDevicePlugin;
            }
        }

        private void ProcessAggregator(AggregationNode aggregationNode)
        {
        }

        private static string GetTabItemTitle(DeviceNode deviceNode)
        {
            return $"{deviceNode.Device.Name} id:{deviceNode.Device.Id}";
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
            baseDeviceViewModel.OnRenamed += ((device) =>
            {
                deviceNode.Name = device.Name;
                var tab = DeviceTabs.FirstOrDefault(t => (Device)t.DataContext == deviceNode.Device);
                if (tab != null)
                {
                    ((ClosableTab)tab).Title = GetTabItemTitle(deviceNode);
                }
            });
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
                CommandParameter = deviceNode,
                IsEnabled = deviceNode.AllowDownload
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
                IsEnabled = deviceNode.AllowSave
            });
            toolbar.Items.Add(new Button
            {
                Content = "Загрузить конфигурацию из файла",
                Command = ImportSettings,
                CommandParameter = deviceNode,
                IsEnabled = deviceNode.AllowLoad
            });
            grid.Children.Add(tabControl);
            grid.Children.Add(toolbar);
            Grid.SetRow(tabControl, 1);
            Grid.SetRow(toolbar, 0);
            var deviceBuilder = _deviceFactory.Builders.FirstOrDefault(b => b.Model.Equals(deviceNode.Device.Model));
            if (deviceBuilder != null)
            {
                var pack = deviceBuilder.GetControls(deviceNode.Device, _logger);
                foreach (var customDevicesControlsKeyValuePair in pack.CustomizationControls)
                {
                    var customTabItem = new TabItem();
                    var customControl = customDevicesControlsKeyValuePair.Value;
                    customTabItem.Content = customControl;
                    customTabItem.Header = customDevicesControlsKeyValuePair.Key;
                    customTabItem.Background = new SolidColorBrush(Color.FromArgb(255, 0, 176, 180));
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
                          throw new NotImplementedException();
                          //_rootNode.Devices.Clear();
                          //MakeNodes();
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
                        _updatedNode = d;
                        _tabItem = DeviceTabs.FirstOrDefault(t => (Device)t.DataContext == d.Device);
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
