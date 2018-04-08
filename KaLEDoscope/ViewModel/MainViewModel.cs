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
using System.Drawing;

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
                    Name = device.Name,
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
                    Name = device.Name,
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
                    Name = device.Name,
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
#warning Добавить апдейт, если устройства были заведены руками
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
                            ProcessAggregator(aggregationNode, aggregationNode.Nodes.FirstOrDefault() as DeviceNode);
                        }
                        else if (nodeItem is DeviceNode deviceNode)
                        {
                            if (nodeItem.Parent is AggregationNode aggregation)
                            {
                                ProcessAggregator(aggregation, deviceNode);
                            }
                            else
                            {
                                ProcessDevice(deviceNode);
                            }
                        }
                    });
                }
                return showDevicePlugin;
            }
        }

        private void ProcessDevice(DeviceNode deviceNode)
        {
            var existTab = DeviceTabs.FirstOrDefault(t => t.DataContext == deviceNode.Device);
            if (existTab != null)
            {
                SelectedTabItem = existTab;
                return;
            }
            var deviceBuilder = _deviceFactory.Builders.FirstOrDefault(b => b.Model.Equals(deviceNode.Device.Model));
            var previewControl = new UserControl();
            var customizationComtrol = new UserControl();
            IEnumerable<object> menuItems = null;
            if (deviceBuilder != null)
            {
                var pack = deviceBuilder.GetControlsPack(deviceNode.Device, _logger);
                previewControl = pack.PreviewControl;
                customizationComtrol = pack.CustomizationControl;
                menuItems = pack.MenuItems;
            }
            var grid = GetDeviceItemGrid(deviceNode, previewControl, customizationComtrol, menuItems);
            var newTabItem = new ClosableTab
            {
                Title = GetDeviceTabItemTitle(deviceNode),
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

        private void ProcessAggregator(AggregationNode aggregationNode, DeviceNode selectedDeviceNode)
        {
            var grid = GetAggregationGrid(aggregationNode, selectedDeviceNode);
            var existTab = DeviceTabs.FirstOrDefault(t => t.DataContext == aggregationNode);
            if (existTab != null)
            {
                existTab.Content = grid;
                SelectedTabItem = existTab;
                return;
            }
            var newTabItem = new ClosableTab
            {
                Title = GetAggregationTabItemTitle(aggregationNode),
                Content = grid,
                DataContext = aggregationNode,
            };
            newTabItem.OnTabCloseClick += (sender, arguments) =>
            {
                var tab = (ClosableTab)sender;
                DeviceTabs.Remove(tab);
            };
            DeviceTabs.Add(newTabItem);
            SelectedTabItem = newTabItem;

        }

        private UserControl GetAggregationGrid(AggregationNode aggregationNode, DeviceNode selectedDeviceNode)
        {
            var deviceBuilder = _deviceFactory.Builders.FirstOrDefault(b => b.Model.Equals(selectedDeviceNode.Device.Model));
            var previewControl = GetAggregationPreviewGrid(aggregationNode, selectedDeviceNode);
            var customizationComtrol = new UserControl();
            IEnumerable<object> menuItems = null;
            if (deviceBuilder != null)
            {
                var pack = deviceBuilder.GetControlsPack(selectedDeviceNode.Device, _logger);
                customizationComtrol = pack.CustomizationControl;
                menuItems = pack.MenuItems;
            }
            return GetDeviceItemGrid(selectedDeviceNode, previewControl, customizationComtrol, menuItems);
        }

        private UserControl GetAggregationPreviewGrid(AggregationNode aggregationNode, DeviceNode selectedDeviceNode)
        {
            if (!aggregationNode.Nodes.Any())
            {
                return new UserControl();
            }
            if (aggregationNode.Nodes.Count == 1)
            {
                var deviceBuilder = _deviceFactory.Builders.FirstOrDefault(b => b.Model.Equals(selectedDeviceNode.Device.Model));
                var pack = deviceBuilder.GetControlsPack(selectedDeviceNode.Device, _logger);
                return pack.PreviewControl;
            }
            var control = new UserControl();
            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star)
            });
            int column = 1;
            foreach (var node in aggregationNode.Nodes)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = GridLength.Auto
                });
                var splitter = new GridSplitter
                {
                    Width = 5,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    ResizeDirection = GridResizeDirection.Columns
                };
                grid.Children.Add(splitter);
                Grid.SetRow(splitter, column++);

                grid.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = GridLength.Auto
                });
                var deviceBuilder = _deviceFactory.Builders.FirstOrDefault(b => b.Model.Equals(((DeviceNode)node).Device.Model));
                var pack = deviceBuilder.GetControlsPack(((DeviceNode)node).Device, _logger);
                var devicePreview = pack.PreviewControl;
                if (selectedDeviceNode==node)
                {
                    devicePreview.BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 255, 0));
                }
                grid.Children.Add(devicePreview);
                Grid.SetRow(devicePreview, column++);
            }
            grid.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = GridLength.Auto
            });
            var finalSplitter = new GridSplitter
            {
                Width = 5,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Center,
                ResizeDirection = GridResizeDirection.Columns
            };
            grid.Children.Add(finalSplitter);
            Grid.SetRow(finalSplitter, column++);
            grid.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star)
            });
            control.Content = grid;
            return control;
        }

        private static string GetDeviceTabItemTitle(DeviceNode deviceNode)
        {
            return $"{deviceNode.Device.Name} id:{deviceNode.Device.Id}";
        }

        private static string GetAggregationTabItemTitle(AggregationNode aggregationNode)
        {
            return $"{aggregationNode.Name}";
        }


        private UserControl GetDeviceItemGrid(DeviceNode deviceNode, UserControl previewControl, UserControl customizationControl, IEnumerable<object> toolbarItems)
        {
            var model = new CustomizationViewModel(deviceNode, _deviceFactory, _invoker, _logger);
            model.OnNodeRenamed += ((sender, node) =>
            {
                var tab = DeviceTabs.FirstOrDefault(t => (Device)t.DataContext == node.Device);
                if (tab != null)
                {
                    ((ClosableTab)tab).Title = GetDeviceTabItemTitle(node);
                }
            });
            model.BeforeGettingSettings += ((sender, node) =>
            {
                _updatedNode = node;
                _tabItem = DeviceTabs.FirstOrDefault(t => (Device)t.DataContext == node.Device);
            });
            model.AfterGetingSettings += ((sender, device) =>
              {
                  _updatedNode.Device = device;
                  var deviceBuilder = _deviceFactory.Builders.FirstOrDefault(b => b.Model.Equals(device.Model));
                  previewControl = new UserControl();
                  var customizationComtrol = new UserControl();
                  IEnumerable<object> menuItems = null;
                  if (deviceBuilder != null)
                  {
                      var pack = deviceBuilder.GetControlsPack(_updatedNode.Device, _logger);
                      previewControl = pack.PreviewControl;
                      customizationComtrol = pack.CustomizationControl;
                      menuItems = pack.MenuItems;
                  }
                  _dispatcher.Invoke(() => _tabItem.Content = GetDeviceItemGrid(_updatedNode, previewControl, customizationComtrol, menuItems));
                  _tabItem.DataContext = device;
              });
            var control = new CustomizationControl();
            control.DataContext = model;
            control.SetPreviewControl(previewControl);
            control.SetCustomizationControl(customizationControl);
            control.AddToolbarItems(toolbarItems);
            return control;
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
                          //_rootNode.Devices.Clear();
                          MakeNodes();
                      });
                }
                return _scanDevices;
            }
        }

        private DeviceNode _updatedNode;
        private TabItem _tabItem;

        private void LogMessage(string Message)
        {
            _log.AppendLine(Message);
            OnPropertyChanged(nameof(Log));
        }
    }
}
