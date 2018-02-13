using CommandProcessing;
using DirectConnect;
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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Timer;
using Input = System.Windows.Input;

namespace KaLEDoscope
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ILogger _logger { get; set; }

        public ObservableCollection<ProtocolNode> ProtocolNodes { get; set; } = new ObservableCollection<ProtocolNode>();
        public ObservableCollection<LogItem> LogItems { get; set; } = new ObservableCollection<LogItem>();
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

        public Dictionary<Func<DeviceNode>, Func<CustomDeviceControl>> CustomControls { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel(ILogger logger)
        {
            _logger = logger;
            _logger.InfoRaised += (sender, message) => LogItems.Add(new LogItem { LogLevel = LogLevel.Info, Message = $"{sender}: {message}" });
            _logger.DebugRaised += (sender, message) => LogItems.Add(new LogItem { LogLevel = LogLevel.Debug, Message = $"{sender}: {message}" });
            _logger.WarnRaised += (sender, message) => LogItems.Add(new LogItem { LogLevel = LogLevel.Warn, Message = $"{sender}: {message}" });
            _logger.ErrorRaised += (sender, message) => LogItems.Add(new LogItem { LogLevel = LogLevel.Error, Message = $"{sender}: {message}" });
        }

        public void MakeNodes()
        {
            var directConnectDeviceScanner = new DeviceScanner<DirectConnection>(_logger);
            directConnectDeviceScanner.DeviceDictionary.Add(1, () => new TimerDevice
            {
                Name = "Семисегментные часы"
            });

            var devices = directConnectDeviceScanner.Search();

            var mqtt = new ProtocolNode
            {
                Name = "MQTT",
            };
            var directConnect = new ProtocolNode
            {
                Name = "DirectConnect",
            };

            foreach (var device in devices)
            {
                directConnect.Members.Add(new DeviceNode
                {
                    Device = device,
                });
            }
            ProtocolNodes.Add(mqtt);
            ProtocolNodes.Add(directConnect);
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
                            var customTabItem = new TabItem();
                            var grid = new Grid();
                            grid.RowDefinitions.Add(new RowDefinition
                            {
                                Height = GridLength.Auto
                            });
                            grid.RowDefinitions.Add(new RowDefinition
                            {
                                Height = new GridLength(1, GridUnitType.Star)
                            });


                            customTabItem.Content = new BaseDeviceControl
                            {
                                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                                VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
                                DataContext = new BaseDeviceViewModel(deviceNode.Device, _logger)
                            };
                            customTabItem.Header = "Базовые настройки";
                            tabControl.Items.Add(customTabItem);
                            tabControl.TabStripPlacement = Dock.Left;
                            var toolbar = new ToolBar();
                            toolbar.Items.Add(new Button
                            {Content="Синхронизировать" });
                            grid.Children.Add(tabControl);
                            grid.Children.Add(toolbar);
                            Grid.SetRow(tabControl, 1);
                            Grid.SetRow(toolbar, 0);
                            var newTabItem = new ClosableTab
                            {
                                Title = $"{deviceNode.Device.Name} id:{deviceNode.Device.Id}",
                                Content = grid,
                                DataContext = deviceNode.Device
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

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
