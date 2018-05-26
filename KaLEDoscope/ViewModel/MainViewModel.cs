using BaseDevice;
using CommandProcessing;
using KaLEDoscope.ViewModel;
using KaLEDoscope.Views;
using ServiceInterfaces;
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
using GongSolutions.Wpf.DragDrop;
using Extensions;
using System;
using Microsoft.Win32;
using KaLEDoscope.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using BaseDeviceSerialization;

namespace KaLEDoscope
{
    public class MainViewModel : Notified, IDropTarget
    {
        private ILogger _logger { get; set; }
        private ICompressor _compressor { get; set; }
        private readonly Dispatcher _dispatcher;
        private readonly DeviceFactory _deviceFactory;
        private readonly Invoker _invoker;
        private DeviceNode _updatedNode;
        private TabItem _tabItem;
        private readonly ObservableCollection<LogItem> _logItems = new ObservableCollection<LogItem>();
        private readonly INetworkAgent _networkScanAgent;
        private readonly INetworkAgent _networkExchangeAgent;

        private const string _defaultStructureFileName = "Структура";
        private const string _defaultStructureFileExtension = ".struct";
        private const string _defaultStructureFilter = "Structure file (.struct)|*.struct|All files (*.*)|*.*";


        public ObservableCollection<NodeItem> StructureNodes { get; set; } = new ObservableCollection<NodeItem>();
        public ObservableCollection<TabItem> DeviceTabs { get; set; } = new ObservableCollection<TabItem>();
        public ObservableCollection<MenuItem> DeviceItems { get; set; } = new ObservableCollection<MenuItem>();
        public ObservableCollection<LogItem> FilteredLogItems { get; set; } = new ObservableCollection<LogItem>();

        public event EventHandler ShowOptions;
        public event EventHandler QuitApplication;

        public TabItem SelectedTabItem { get; set; }
        public string StructureFileName { get; set; } = string.Empty;
        public bool IsScanEnabled { get; set; }
        public NodeItem SelectedNode { get; set; }
        public bool AllowDebugLog { get; set; } = false;
        public int Debugs { get; set; }
        public bool AllowInfoLog { get; set; } = true;
        public int Infos { get; set; }
        public bool AllowWarnLog { get; set; } = true;
        public int Warnings { get; set; }
        public bool AllowErrorLog { get; set; } = true;
        public int Errors { get; set; }


        public MainViewModel(ILogger logger, ICompressor compressor, INetworkAgent networkScanAgent, INetworkAgent networkExchangeAgent)
        {
            _logger = logger;
            _compressor = compressor;
            _networkScanAgent = networkScanAgent;
            _networkExchangeAgent = networkExchangeAgent;
            _logger.InfoRaised += (sender, message) => LogMessage(new LogItem
            {
                LogLevel = LogLevel.Info,
                Message = message
            });
            _logger.DebugRaised += (sender, message) => LogMessage(new LogItem
            {
                LogLevel = LogLevel.Debug,
                Message = message
            });
            _logger.WarnRaised += (sender, message) => LogMessage(new LogItem
            {
                LogLevel = LogLevel.Warn,
                Message = message
            });
            _logger.ErrorRaised += (sender, message) => LogMessage(new LogItem
            {
                LogLevel = LogLevel.Error,
                Message = message
            });
            _dispatcher = Dispatcher.CurrentDispatcher;
            _invoker = new Invoker(_logger);
            IsScanEnabled = true;
            _deviceFactory = new DeviceFactory(_logger);
            _deviceFactory.Builders.Add(new SevenSegmentDeviceBuilder());
            _deviceFactory.Builders.Add(new PixelDeviceBuilder());

            _logger.Info(this, "Started");

            foreach (var deviceBuilder in _deviceFactory.Builders)
            {
                DeviceItems.Add(new MenuItem
                {
                    Header = deviceBuilder.DisplayName,
                    Command = AddNewDevice,
                    CommandParameter = deviceBuilder.Model
                });
            }
        }

        private void MakeNodes()
        {
            var directConnectDeviceScanner = new DeviceScanner(_logger, _networkScanAgent, _deviceFactory);
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

        private DelegateCommand _options;
        public Input.ICommand Options
        {
            get
            {
                if (_options.IsNull())
                {
                    _options = new DelegateCommand((o) =>
                    {
                        ShowOptions?.Invoke(this, EventArgs.Empty);
                    });
                }
                return _options;
            }
        }

        private DelegateCommand _editNode;
        public Input.ICommand EditNode
        {
            get
            {
                if (_editNode.IsNull())
                {
                    _editNode = new DelegateCommand((o) =>
                    {
                        if (SelectedNode.IsNull())
                            return;
                        var renameDialog = new RenameDialog();
                        if (!(SelectedNode as FolderNode).IsNull())
                        {
                            var folderNode = SelectedNode as FolderNode;
                            renameDialog.NameField = folderNode.Name;
                            if (renameDialog.ShowDialog() == true)
                            {
                                folderNode.Name = renameDialog.NameField;
                                folderNode.Folder.Name = renameDialog.NameField;
                            }
                        }
                        else if (!(SelectedNode as AggregationNode).IsNull())
                        {
                            var aggregationNode = SelectedNode as AggregationNode;
                            renameDialog.NameField = aggregationNode.Name;
                            if (renameDialog.ShowDialog() == true)
                            {
                                aggregationNode.Name = renameDialog.NameField;
                                aggregationNode.Aggregation.Name = renameDialog.NameField;
                            }
                        }
                        else if (!(SelectedNode as DeviceNode).IsNull())
                        {
                            ShowDevicePlugin.Execute(null);
                        }
                    });
                }
                return _editNode;
            }
        }

        private DelegateCommand _quit;
        public Input.ICommand Quit
        {
            get
            {
                if (_quit.IsNull())
                {
                    _quit = new DelegateCommand((o) =>
                    {
                        QuitApplication?.Invoke(this, EventArgs.Empty);
                    });
                }
                return _quit;
            }
        }

        private DelegateCommand _addNewDevice;
        public Input.ICommand AddNewDevice
        {
            get
            {
                if (_addNewDevice.IsNull())
                {
                    _addNewDevice = new DelegateCommand((o) =>
                    {
                        var model = o.ToString();
                        var deviceBuilder = _deviceFactory.Builders.FirstOrDefault(b => b.Model.Equals(model));
                        var rootDevices = StructureNodes.OfType<DeviceNode>();
                        var aggregations = StructureNodes.OfType<AggregationNode>();
                        var folders = StructureNodes.OfType<FolderNode>();
                        var maxId = 0;
                        if (rootDevices.Any())
                        {
                            var maxDeviceId = rootDevices.Max(d => d.Device.Id);
                            if (maxDeviceId > maxId)
                                maxId = maxDeviceId;
                        }
                        if (aggregations.Any())
                        {
                            var maxDeviceId = 0;
                            foreach (var aggregation in aggregations)
                            {
                                var maxAggregationDeviceId = 0;
                                var devices = aggregation.Nodes.OfType<DeviceNode>();
                                if (devices.Any())
                                {
                                    maxAggregationDeviceId = devices.Max(d => d.Device.Id);
                                }
                                if (maxAggregationDeviceId > maxDeviceId)
                                    maxDeviceId = maxAggregationDeviceId;
                            }
                            if (maxDeviceId > maxId)
                                maxId = maxDeviceId;
                        }
                        if (folders.Any())
                        {
                            var maxDeviceId = 0;
                            foreach (var folder in folders)
                            {
                                var maxAggregationDeviceId = 0;
                                var devices = folder.Nodes.OfType<DeviceNode>();
                                if (devices.Any())
                                {
                                    maxAggregationDeviceId = devices.Max(d => d.Device.Id);
                                }
                                if (maxAggregationDeviceId > maxDeviceId)
                                    maxDeviceId = maxAggregationDeviceId;
                            }
                            if (maxDeviceId > maxId)
                                maxId = maxDeviceId;
                        }
                        var id = maxId + 1;
                        var device = deviceBuilder.UpdateCustomSettings(new Device
                        {
                            Id = id,
                            Name = $"{deviceBuilder.DisplayName}",
                            Model = deviceBuilder.Model,
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
                        _logger.Debug(this, $"Добавлено устройство типа {device.Model}. ID {device.Id}");
                    });
                }
                return _addNewDevice;
            }
        }

        private DelegateCommand _removeNode;
        public Input.ICommand RemoveNode
        {
            get
            {
                if (_removeNode.IsNull())
                {
                    _removeNode = new DelegateCommand((o) =>
                    {
                        var node = SelectedNode;
                        if (node.IsNull())
                            return;
                        if (node.Parent.IsNull())
                        {
                            StructureNodes.Remove(node);
                        }
                        else
                        {
                            node.Parent.Nodes.Remove(node);
                        }
                    });
                }
                return _removeNode;
            }
        }

        private DelegateCommand _newFolder;
        public Input.ICommand NewFolder
        {
            get
            {
                if (_newFolder.IsNull())
                {
                    _newFolder = new DelegateCommand((o) =>
                    {
                        var folderNodes = StructureNodes.OfType<FolderNode>();
                        var maxId = 0;
                        if (folderNodes.Any())
                        {
                            maxId = folderNodes.Max(n => n.Folder.Id);
                        }
                        var id = maxId + 1;
                        var name = $"Папка{id}";
                        var newFolder = new FolderNode
                        {
                            Name = name,
                            Folder = new Folder
                            {
                                Id = id,
                                Name = name
                            }
                        };
                        StructureNodes.Add(newFolder);
                    });
                }
                return _newFolder;
            }
        }

        private DelegateCommand _newAggregator;
        public Input.ICommand NewAggregator
        {
            get
            {
                if (_newAggregator.IsNull())
                {
                    _newAggregator = new DelegateCommand((o) =>
                    {
                        var aggregatorNodes = StructureNodes.OfType<AggregationNode>();
                        var maxId = 0;
                        if (aggregatorNodes.Any())
                        {
                            maxId = aggregatorNodes.Max(n => n.Aggregation.Id);
                        }
                        var id = maxId + 1;
                        var name = $"Агрегатор{id}";
                        var newAggregator = new AggregationNode
                        {
                            Name = name,
                            Aggregation = new Aggregation
                            {
                                Id = id,
                                Name = name
                            }
                        };
                        StructureNodes.Add(newAggregator);
                    });
                }
                return _newAggregator;
            }
        }

        private DelegateCommand _saveStructure;
        public Input.ICommand SaveStructure
        {
            get
            {
                if (_saveStructure.IsNull())
                {
                    _saveStructure = new DelegateCommand((o) =>
                    {
                        if (String.IsNullOrEmpty(StructureFileName))
                        {
                            SaveNewStructure();
                        }
                        else
                        {
                            SaveExistStructure();
                        }
                    });
                }
                return _saveStructure;
            }
        }

        private DelegateCommand _saveStructureAs;
        public Input.ICommand SaveStructureAs
        {
            get
            {
                if (_saveStructureAs.IsNull())
                {
                    _saveStructureAs = new DelegateCommand((o) =>
                    {
                        SaveNewStructure();
                    });
                }
                return _saveStructureAs;
            }
        }

        private DelegateCommand _openStructure;
        public Input.ICommand OpenStructure
        {
            get
            {
                if (_openStructure.IsNull())
                {
                    _openStructure = new DelegateCommand((o) =>
                    {
                        var dialog = new OpenFileDialog
                        {
                            DefaultExt = _defaultStructureFileExtension,
                            Filter = _defaultStructureFilter
                        };
                        if (dialog.ShowDialog() != true)
                            return;
                        DeviceTabs.Clear();
                        StructureNodes.Clear();
                        StructureFileName = dialog.FileName;
                        var bytes = System.IO.File.ReadAllBytes(StructureFileName);
                        var text = _compressor.Unzip(bytes);
                        var serializableContainers = JsonConvert.DeserializeObject<List<SerializableContainer>>(text);
                        foreach (var serializableContainer in serializableContainers)
                        {
                            if (serializableContainer.ContentType == ContentType.Folder)
                            {
                                var serializableFolder = (serializableContainer.Content as JObject)
                                    .ToObject<SerializableFolder>();
                                StructureNodes.Add(new FolderNode
                                {
                                    Folder = new Folder
                                    {
                                        Id = serializableFolder.Id,
                                        Name = serializableFolder.Name
                                    },
                                    Name = serializableFolder.Name
                                });
                            }
                            if (serializableContainer.ContentType == ContentType.Aggregator)
                            {
                                var serializableAggregation = (serializableContainer.Content as JObject)
                                    .ToObject<SerializableAggregation>();
                                StructureNodes.Add(new AggregationNode
                                {
                                    Aggregation = new Aggregation
                                    {
                                        Id = serializableAggregation.Id,
                                        Name = serializableAggregation.Name
                                    },
                                    Name = serializableAggregation.Name
                                });
                            }
                            if (serializableContainer.ContentType == ContentType.Device)
                            {
                                var baseDevice = (serializableContainer.Content as JObject)
                                    .ToObject<SerializableBaseDevice>();
                                var deviceBuilder = _deviceFactory.Builders.FirstOrDefault(b => b.Model.Equals(baseDevice.Model));
                                var device = deviceBuilder.FromSerializable(serializableContainer.Content);
                                var deviceNode = new DeviceNode
                                {
                                    Device = device,
                                    Name = device.Name,
                                    AllowDownload = false,
                                    AllowLoad = true,
                                    AllowSave = true,
                                    AllowUpload = false,
                                };
                                if (device.AggregationId.HasValue)
                                {
                                    var aggregationNode = StructureNodes.OfType<AggregationNode>()
                                        .FirstOrDefault(n => n.Aggregation.Id == device.AggregationId);
                                    deviceNode.Parent = aggregationNode;
                                    aggregationNode.Nodes.Add(deviceNode);
                                }
                                else if (device.FolderId.HasValue)
                                {
                                    var folderNode = StructureNodes.OfType<FolderNode>()
                                        .FirstOrDefault(n => n.Folder.Id == device.FolderId);
                                    deviceNode.Parent = folderNode;
                                    folderNode.Nodes.Add(deviceNode);
                                }
                                else
                                {
                                    StructureNodes.Add(deviceNode);
                                }
                            }
                        }
                        _logger.Info(this, "Структура загружена успешно");
                    });
                }
                return _openStructure;
            }
        }

        private void SaveExistStructure()
        {
            var serializableContainers = new List<SerializableContainer>();
            foreach (var node in StructureNodes)
            {
                var deviceNode = node as DeviceNode;
                var folderNode = node as FolderNode;
                var aggregationNode = node as AggregationNode;
                if (!deviceNode.IsNull())
                {
                    serializableContainers.Add(GetDeviceSerializableContainer(deviceNode));
                }
                else if (!folderNode.IsNull())
                {
                    serializableContainers.Add(
                        new SerializableContainer
                        {
                            ContentType = ContentType.Folder,
                            Content = (SerializableFolder)folderNode.Folder
                        });
                    foreach (var deviceSubNode in folderNode.Nodes)
                    {
                        serializableContainers.Add(GetDeviceSerializableContainer((DeviceNode)deviceSubNode));
                    }
                }
                else if (!aggregationNode.IsNull())
                {
                    serializableContainers.Add(
                        new SerializableContainer
                        {
                            ContentType = ContentType.Aggregator,
                            Content = (SerializableAggregation)aggregationNode.Aggregation
                        });
                    foreach (var deviceSubNode in aggregationNode.Nodes)
                    {
                        serializableContainers.Add(GetDeviceSerializableContainer((DeviceNode)deviceSubNode));
                    }
                }
            }
            var serialized = JsonConvert.SerializeObject(serializableContainers);
            var datas = _compressor.Zip(serialized);
            System.IO.File.WriteAllBytes(StructureFileName, datas);
            _logger.Info(this, $"Структура сохранена в {StructureFileName}");
        }

        private SerializableContainer GetDeviceSerializableContainer(DeviceNode node)
        {
            var deviceBuilder = _deviceFactory.Builders.FirstOrDefault(b => b.Model.Equals(node.Device.Model));
            var deviceSerializableContainer = new SerializableContainer
            {
                ContentType = ContentType.Device,
                Content = deviceBuilder.GetSerializable(node.Device)
            };
            return deviceSerializableContainer;
        }

        private void SaveNewStructure()
        {
            var dialog = new SaveFileDialog
            {
                FileName = _defaultStructureFileName,
                DefaultExt = _defaultStructureFileExtension,
                Filter = _defaultStructureFilter
            };
            if (dialog.ShowDialog() == true)
            {
                StructureFileName = dialog.FileName;
            }
            SaveExistStructure();
        }

        private DelegateCommand _showDevicePlugin;
        public Input.ICommand ShowDevicePlugin
        {
            get
            {
                if (_showDevicePlugin.IsNull())
                {
                    _showDevicePlugin = new DelegateCommand((o) =>
                    {
                        var nodeItem = SelectedNode;
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
                return _showDevicePlugin;
            }
        }

        private void ProcessDevice(DeviceNode deviceNode)
        {
            var existTab = DeviceTabs.FirstOrDefault(t => t.DataContext == deviceNode.Device);
            if (!existTab.IsNull())
            {
                SelectedTabItem = existTab;
                return;
            }
            var deviceBuilder = _deviceFactory.Builders.FirstOrDefault(b => b.Model.Equals(deviceNode.Device.Model));
            var previewControl = new UserControl();
            var customizationControl = new UserControl();
            IEnumerable<object> menuItems = null;
            if (!deviceBuilder.IsNull())
            {
                var pack = deviceBuilder.GetControlsPack(deviceNode.Device, _logger);
                previewControl = pack.PreviewControl;
                customizationControl = pack.CustomizationControl;
                menuItems = pack.MenuItems;
            }
            var grid = GetDeviceItemGrid(deviceNode, previewControl, customizationControl, menuItems);
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
            if (!existTab.IsNull())
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
            if (!deviceBuilder.IsNull())
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
            control.Content = grid;
            grid.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star)
            });
            var column = 1;
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
                Grid.SetColumn(splitter, column++);

                grid.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star)
                });
                var deviceBuilder = _deviceFactory.Builders.FirstOrDefault(b => b.Model.Equals(((DeviceNode)node).Device.Model));
                var pack = deviceBuilder.GetControlsPack(((DeviceNode)node).Device, _logger);
                var devicePreview = pack.PreviewControl;
                devicePreview.HorizontalAlignment = HorizontalAlignment.Stretch;
                devicePreview.VerticalAlignment = VerticalAlignment.Stretch;
                if (selectedDeviceNode == node)
                {
                    var controlWrapper = new UserControl
                    {
                        BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 255, 0)),
                        BorderThickness = new Thickness(2),
                        Content = devicePreview
                    };
                    grid.Children.Add(controlWrapper);
                    Grid.SetColumn(controlWrapper, column++);
                }
                else
                {
                    grid.Children.Add(devicePreview);
                    Grid.SetColumn(devicePreview, column++);
                }
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
            Grid.SetColumn(finalSplitter, column);
            grid.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star)
            });
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
            var model = new CustomizationViewModel(deviceNode, _deviceFactory, _invoker, _compressor, _networkExchangeAgent, _logger);
            model.OnNodeRenamed += ((sender, node) =>
            {
                var tab = DeviceTabs.FirstOrDefault(t => (t.DataContext is Device) && (Device)t.DataContext == node.Device);
                if (!tab.IsNull())
                {
                    ((ClosableTab)tab).Title = GetDeviceTabItemTitle(node);
                }
            });
            model.BeforeGettingSettings += ((sender, node) =>
            {
                _updatedNode = node;
                _tabItem = DeviceTabs.FirstOrDefault(t => (t.DataContext is Device) && (Device)t.DataContext == node.Device);
            });
            model.AfterGetingSettings += ((sender, device) =>
              {
                  _updatedNode.Device = device;
                  var deviceBuilder = _deviceFactory.Builders.FirstOrDefault(b => b.Model.Equals(device.Model));
                  previewControl = new UserControl();
                  var customizationComtrol = new UserControl();
                  IEnumerable<object> menuItems = null;
                  if (!deviceBuilder.IsNull())
                  {
                      var pack = deviceBuilder.GetControlsPack(_updatedNode.Device, _logger);
                      previewControl = pack.PreviewControl;
                      customizationComtrol = pack.CustomizationControl;
                      menuItems = pack.MenuItems;
                  }
                  _dispatcher.Invoke(() => _tabItem.Content = GetDeviceItemGrid(_updatedNode, previewControl, customizationComtrol, menuItems));
                  _tabItem.DataContext = device;
              });
            var control = new CustomizationControl
            {
                DataContext = model
            };
            control.SetPreviewControl(previewControl);
            control.SetCustomizationControl(customizationControl);
            control.AddToolbarItems(toolbarItems);
            return control;
        }

        private DelegateCommand _clearStructure;
        public Input.ICommand ClearStructure
        {
            get
            {
                if (_clearStructure.IsNull())
                {
                    _clearStructure = new DelegateCommand((o) =>
                      {
                          StructureNodes.Clear();
                      });
                }
                return _clearStructure;
            }
        }

        private DelegateCommand _scanDevices;
        public Input.ICommand ScanDevices
        {
            get
            {
                if (_scanDevices.IsNull())
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

        private void LogMessage(LogItem logItem)
        {
            _logItems.Insert(0, logItem);
            if (LogItemCanBeShown(logItem))
            {
                _dispatcher.Invoke(() => FilteredLogItems.Insert(0, logItem));
            }
            Debugs = _logItems.Count(i => i.LogLevel == LogLevel.Debug);
            Infos = _logItems.Count(i => i.LogLevel == LogLevel.Info);
            Warnings = _logItems.Count(i => i.LogLevel == LogLevel.Warn);
            Errors = _logItems.Count(i => i.LogLevel == LogLevel.Error);
        }

        private bool LogItemCanBeShown(LogItem logItem) => (logItem.LogLevel == LogLevel.Debug && AllowDebugLog) ||
                            (logItem.LogLevel == LogLevel.Info && AllowInfoLog) ||
                            (logItem.LogLevel == LogLevel.Warn && AllowWarnLog) ||
                            (logItem.LogLevel == LogLevel.Error && AllowErrorLog);

        private DelegateCommand _ApplyLogFilter;
        public Input.ICommand ApplyLogFilter
        {
            get
            {
                if (_ApplyLogFilter.IsNull())
                    _ApplyLogFilter = new DelegateCommand((o) =>
                    {
                        FilteredLogItems.Clear();
                        foreach (var logItem in _logItems)
                        {
                            if (LogItemCanBeShown(logItem))
                            {
                                FilteredLogItems.Add(logItem);
                            }
                        }
                    });
                return _ApplyLogFilter;
            }
        }

        public void DragOver(IDropInfo dropInfo)
        {
            var sourceNode = dropInfo.Data as NodeItem;
            var targetNode = dropInfo.TargetItem as NodeItem;
            if ((sourceNode is DeviceNode) && ((targetNode is AggregationNode) || (targetNode is FolderNode) || (targetNode is null)))
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = DragDropEffects.Move;
            }
            else
            {
                dropInfo.Effects = DragDropEffects.None;
            }
        }

        public void Drop(IDropInfo dropInfo)
        {
            var deviceNode = dropInfo.Data as DeviceNode;
            deviceNode.Device.AggregationId = null;
            deviceNode.Device.FolderId = null;
            var parentNode = deviceNode.Parent;
            if (parentNode.IsNull())
            {
                StructureNodes.Remove(deviceNode);
            }
            else
            {
                parentNode.Nodes.Remove(deviceNode);
            }
            var targetNode = dropInfo.TargetItem as NodeItem;
            if (targetNode.IsNull())
            {
                StructureNodes.Add(deviceNode);
                deviceNode.Parent = null;
                return;
            }
            targetNode.Nodes.Add(deviceNode);
            deviceNode.Parent = targetNode;
            if (targetNode is AggregationNode aggregationNode)
            {
                deviceNode.Device.AggregationId = aggregationNode.Aggregation.Id;
                if (DeviceTabs.Any(t => t.DataContext == aggregationNode))
                {
                    ProcessAggregator(aggregationNode, aggregationNode.Nodes.FirstOrDefault() as DeviceNode);
                }
            }
            if (targetNode is FolderNode folderNode)
            {
                deviceNode.Device.FolderId = folderNode.Folder.Id;
            }
        }
    }
}
