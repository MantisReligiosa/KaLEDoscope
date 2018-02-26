using CommandProcessing;
using DomainData;
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
using Timer;
using Input = System.Windows.Input;

namespace KaLEDoscope
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ILogger _logger { get; set; }
        private ProtocolNode _directConnect;
        private readonly Dispatcher _dispatcher;

        public ObservableCollection<ProtocolNode> ProtocolNodes { get; set; } = new ObservableCollection<ProtocolNode>();
        public ObservableCollection<TabItem> DeviceTabs { get; set; } = new ObservableCollection<TabItem>();

        private readonly Dictionary<Func<Device, bool>, Func<Device, ILogger, UserControl>> _customDevicesControls
            = new Dictionary<Func<Device, bool>, Func<Device, ILogger, UserControl>>
        {
            {d=> d is BoardClock,
             (d,l)=>   new TimerControl
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    DataContext = new TimerDeviceViewModel(d, l)
            }}
        };

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
        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel(ILogger logger)
        {
            _logger = logger;
            _logger.InfoRaised += (sender, message) => logMessage($"{sender}: Info: {message}");
            _logger.DebugRaised += (sender, message) => logMessage($"{sender}: Debug: {message}");
            _logger.WarnRaised += (sender, message) => logMessage($"{sender}: Warn: {message}");
            _logger.ErrorRaised += (sender, message) => logMessage($"{sender}: Error: {message}");
            _dispatcher = Dispatcher.CurrentDispatcher;
            IsScanEnabled = true;
        }

        public void MakeNodes()
        {
            var directConnectDeviceScanner = new UdpDeviceScanner(_logger);
            directConnectDeviceScanner.DeviceFactory.AddTransformation("boardClock", (d) =>
            {
                BoardClock device = new BoardClock
                {
                    AlarmSchedule = new List<Alarm>(),
                    Model = d.Model,
                    Network = d.Network,
                    Brightness = new Brightness
                    {
                        BrightnessPeriods = new List<BrightnessPeriod>(),
                        Mode = Mode.Auto
                    },
                    WorkSchedule = new WorkSchedule
                    {

                    }
                };
                device.Name = "Семисегментные часы";
                return device;
            });

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
                        var deviceNode = (DeviceNode)o;
                        var existTab = DeviceTabs.FirstOrDefault(t => (Device)t.DataContext == deviceNode.Device);
                        if (existTab == null)
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


                            baseTabItem.Content = new BaseDeviceControl
                            {
                                HorizontalAlignment = HorizontalAlignment.Stretch,
                                VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
                                DataContext = new BaseDeviceViewModel(deviceNode.Device, _logger)
                            };
                            baseTabItem.Header = "Базовые настройки";
                            tabControl.Items.Add(baseTabItem);
                            tabControl.TabStripPlacement = Dock.Left;
                            var toolbar = new ToolBar();
                            toolbar.Items.Add(new Button
                            { Content = "Синхронизировать", DataContext = deviceNode.Device });
                            toolbar.Items.Add(new Button
                            { Content = "Применить конфигурацию", DataContext = deviceNode.Device });
                            toolbar.Items.Add(new Button
                            { Content = "Экспортировать настройки как скрипт" });
                            toolbar.Items.Add(new Button
                            { Content = "Применить скрипт" });
                            grid.Children.Add(tabControl);
                            grid.Children.Add(toolbar);
                            Grid.SetRow(tabControl, 1);
                            Grid.SetRow(toolbar, 0);
                            var newTabItem = new ClosableTab
                            {
                                Title = $"{deviceNode.Device.Name} id:{deviceNode.Device.Model}",
                                Content = grid,
                                DataContext = deviceNode.Device
                            };

                            newTabItem.OnTabCloseClick += (sender, arguments) =>
                            {
                                var tab = (ClosableTab)sender;
                                DeviceTabs.Remove(tab);
                            };
                            DeviceTabs.Add(newTabItem);

                            foreach (var customDevicesControlsKeyValuePair in _customDevicesControls)
                            {
                                if (customDevicesControlsKeyValuePair.Key(deviceNode.Device))
                                {
                                    var customTabItem = new TabItem();
                                    var customControl = customDevicesControlsKeyValuePair.Value(deviceNode.Device, _logger);
                                    customTabItem.Content = customControl;
                                    customTabItem.Header = "Специальные настройки";
                                    tabControl.Items.Add(customTabItem);
                                }
                            }
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

        private DelegateCommand scanDevices;
        public Input.ICommand ScanDevices
        {
            get
            {
                if (scanDevices == null)
                {
                    scanDevices = new DelegateCommand((o) =>
                      {
                          ClearNodes();
                          MakeNodes();
                      });
                }
                return scanDevices;
            }
        }

        private DelegateCommand saveConfig;
        public Input.ICommand SaveConfig
        {
            get
            {
                if (saveConfig == null)
                {
                    saveConfig = new DelegateCommand((o) =>
                      {
                          var dialog = new SaveFileDialog
                          {
                              FileName = "Config",
                              DefaultExt = ".json",
                              Filter = "JSON configuration (.json)|*.json"
                          };
                          if (dialog.ShowDialog() == true)
                          {
                              var fileName = dialog.FileName;
                              var serialized = JsonConvert.SerializeObject(ProtocolNodes);
                              System.IO.File.WriteAllText(fileName, serialized);
                          }
                      });
                }
                return saveConfig;
            }
        }

        private Input.ICommand loadConfig;
        public Input.ICommand LoadConfig
        {
            get
            {
                if (loadConfig == null)
                {
                    loadConfig = new DelegateCommand((o) =>
                      {
                          ClearNodes();
                          var dialog = new OpenFileDialog
                          {
                              FileName = "Config",
                              DefaultExt = ".json",
                              Filter = "JSON configuration (.json)|*.json"
                          };
                          if (dialog.ShowDialog() == true)
                          {
                              var fileName = dialog.FileName;
                              var serialized = System.IO.File.ReadAllText(fileName);
                              var nodes = JsonConvert.DeserializeObject<List<ProtocolNode>>(serialized);
                              foreach (var node in nodes)
                              {
                                  ProtocolNodes.Add(node);
                              }
                          }
                      });
                }
                return loadConfig;
            }
        }

        private void logMessage(string Message)
        {
            _log.AppendLine(Message);
            OnPropertyChanged(nameof(Log));
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
