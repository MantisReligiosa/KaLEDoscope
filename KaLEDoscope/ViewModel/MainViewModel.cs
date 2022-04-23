using BaseDevice;
using BaseDeviceSerialization;
using CommandProcessing;
using CommandProcessing.Commands;
using Configuration;
using DeviceBuilding;
using GongSolutions.Wpf.DragDrop;
using KaLEDoscope.Serialization;
using KaLEDoscope.ViewModel;
using KaLEDoscope.Views;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PixelBoardDevice;
using ServiceInterfaces;
using SevenSegmentBoardDevice;
using SmartTechnologiesM.Activation;
using SmartTechnologiesM.Base;
using SmartTechnologiesM.Base.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using UiCommands;
using Input = System.Windows.Input;

namespace KaLEDoscope
{
    public class MainViewModel : Notified, IDropTarget
    {
        private readonly ILogger _logger;
        private readonly IConfig _config;
        private readonly ICompressor _compressor;
        private readonly IActivationManager _activationManager;
        private readonly Dispatcher _dispatcher;
        private readonly DeviceFactory _deviceFactory;
        private DeviceNode _updatedNode;
        private TabItem _tabItem;
        private NodeItem _selectedNode = null;
        private readonly ObservableCollection<LogItem> _logItems = new ObservableCollection<LogItem>();
        private readonly INetworkAgent _networkScanAgent;
        private readonly INetworkAgent _networkExchangeAgent;
        private readonly Timer _autosaveTimer;
        private readonly List<Func<Device, INetworkAgent, ILogger, IConfig, IDeviceCommand<Device>>> _uploadingCommandsContainer;

        private const string _defaultStructureFileName = "Структура";
        private const string _defaultStructureFileExtension = ".struct";
        private const string _defaultStructureFilter = "Structure file (.struct)|*.struct|All files (*.*)|*.*";
        private const string _appName = "KaLEDoscope";

        public ObservableCollection<NodeItem> StructureNodes { get; set; } = new ObservableCollection<NodeItem>();
        public ObservableCollection<TabItem> DeviceTabs { get; set; } = new ObservableCollection<TabItem>();
        public ObservableCollection<MenuItem> DeviceItems { get; set; } = new ObservableCollection<MenuItem>();
        public ObservableCollection<LogItem> FilteredLogItems { get; set; } = new ObservableCollection<LogItem>();

        public event EventHandler ShowOptions;
        public event EventHandler QuitApplication;
        public event EventHandler ActivationRequired;
        public event EventHandler TrialExpired;
        public event EventHandler NewStructure;
        public event EventHandler<ShowAboutEventArgs> ShowAbout;
        public event EventHandler<ShowPreviewEventArgs> ShowPreview;

        public TabItem SelectedTabItem { get; set; }
        public string StructureFileName { get; set; } = string.Empty;
        public bool IsScanEnabled { get; set; }
        public NodeItem SelectedNode
        {
            get
            {
                return _selectedNode;
            }
            set
            {
                if (_selectedNode == value)
                    return;
                _selectedNode = value;
                ProcessDevicePlugin();
                OnPropertyChanged(nameof(SelectedNode));
                OnPropertyChanged(nameof(AllowRename));
            }
        }
        public bool AllowDebugLog { get; set; } = false;
        public int Debugs { get; set; }
        public bool AllowInfoLog { get; set; } = true;
        public int Infos { get; set; }
        public bool AllowWarnLog { get; set; } = true;
        public int Warnings { get; set; }
        public bool AllowErrorLog { get; set; } = true;
        public int Errors { get; set; }
        public bool AllowRename
        {
            get
            {
                return SelectedNode is AggregationNode || SelectedNode is FolderNode;
            }
        }
        public string Title
        {
            get
            {
                return (string.IsNullOrEmpty(StructureFileName) ? string.Empty : $"{StructureFileName} - ") +
                    $"{_appName}" + $"{(HaveUnsavedData ? "*" : string.Empty)}";
            }
        }

        private bool _haveUnsavedData = false;
        public bool HaveUnsavedData
        {
            get
            {
                return _haveUnsavedData;
            }
            set
            {
                _haveUnsavedData = value;
                OnPropertyChanged(nameof(HaveUnsavedData));
                OnPropertyChanged(nameof(Title));
            }
        }

        public string AutosaveFileName { get; set; }

        private int _autosavePeriod;
        public int AutosavePeriod
        {
            get
            {
                return _autosavePeriod;
            }
            set
            {
                _autosavePeriod = value;
                if (!_autosaveTimer.IsNull() && _autosaveTimer.Interval != value)
                {
                    _autosaveTimer.Interval = value;
                }
            }
        }

        public MainViewModel(
            List<Func<Device, INetworkAgent, ILogger, IConfig, IDeviceCommand<Device>>> uploadingCommandsContainer,
            ILogger logger,
            IConfig config,
            ICompressor compressor,
            INetworkAgent networkExchangeAgent,
            INetworkAgent networkScanAgent,
            IActivationManager activationManager)
        {
            _uploadingCommandsContainer = uploadingCommandsContainer;
            _logger = logger;
            _config = config;
            _compressor = compressor;
            _networkScanAgent = networkScanAgent;
            _networkExchangeAgent = networkExchangeAgent;
            _activationManager = activationManager;
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
            IsScanEnabled = true;
            _deviceFactory = new DeviceFactory(_logger);
            _deviceFactory.AddBuilder(new SevenSegmentDeviceBuilder());
            _deviceFactory.AddBuilder(new PixelDeviceBuilder());

            _logger.Info(this, "Started");

            foreach (var deviceItem in _deviceFactory.GetBuilderList())
            {
                DeviceItems.Add(new MenuItem
                {
                    Header = deviceItem.DisplayName,
                    Command = AddNewDevice,
                    CommandParameter = deviceItem.Model
                });
            }
            AutosaveFileName = _config.AutosaveFilename;
            LoadStructure(string.Empty);
            StructureNodes.CollectionChanged += (s, e) => HaveUnsavedData = true;
            AutosavePeriod = _config.AutosavePeriod;
            AutosaveFileName = _config.AutosaveFilename;

            _autosaveTimer = new Timer(AutosavePeriod);
            _autosaveTimer.Elapsed += (s, e) =>
            {
                if (HaveUnsavedData)
                    SaveExistStructure();
            };
            _autosaveTimer.Start();
        }

        public void CheckActivation()
        {
            var expectedRequestCode = _activationManager.GetRequestCode();
            if (string.IsNullOrEmpty(_activationManager.ActualLicenseInfo.RequestCode)
                || _activationManager.ActualLicenseInfo.RequestCode != expectedRequestCode)
            {
                ActivationRequired?.Invoke(this, EventArgs.Empty);
            }
            else if (_activationManager.ActualLicenseInfo.ExpirationDate < DateTime.Now)
            {
                TrialExpired?.Invoke(this, EventArgs.Empty);
            }
        }

        private void MakeNodes()
        {
            var directConnectDeviceScanner = new DeviceScanner(_deviceFactory, _networkScanAgent, _config, _logger);
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
                    var existsDeviceNode = FindDeviceNodeFor(device);
                    if (!existsDeviceNode.IsNull())
                    {
                        existsDeviceNode.Device = device;
                        existsDeviceNode.Name = device.Name;
                        existsDeviceNode.AllowDownload = true;
                        existsDeviceNode.AllowLoad = true;
                        existsDeviceNode.AllowSave = true;
                        existsDeviceNode.AllowUpload = true;
                    }
                    else
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
                }
                HaveUnsavedData = devices.Any();
            });
            IsScanEnabled = true;
        }

        private DeviceNode FindDeviceNodeFor(Device device)
        {
            DeviceNode deviceNode;
            foreach (var aggregationNode in StructureNodes.OfType<AggregationNode>())
            {
                deviceNode = aggregationNode.Nodes.OfType<DeviceNode>().FirstOrDefault(n => n.Device.Id == device.Id);
                if (!deviceNode.IsNull())
                    return deviceNode;
            }
            foreach (var folderNode in StructureNodes.OfType<FolderNode>())
            {
                deviceNode = folderNode.Nodes.OfType<DeviceNode>().FirstOrDefault(n => n.Device.Id == device.Id);
                if (!deviceNode.IsNull())
                    return deviceNode;
            }
            deviceNode = StructureNodes.OfType<DeviceNode>().FirstOrDefault(n => n.Device.Id == device.Id);
            if (!deviceNode.IsNull())
                return deviceNode;
            return null;
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
                        if (SelectedNode is FolderNode folderNode)
                        {
                            renameDialog.NameField = folderNode.Name;
                            if (renameDialog.ShowDialog() == true)
                            {
                                HaveUnsavedData = !folderNode.Name.Equals(renameDialog.NameField);
                                folderNode.Name = renameDialog.NameField;
                                folderNode.Folder.Name = renameDialog.NameField;
                            }
                        }
                        else if (SelectedNode is AggregationNode aggregationNode)
                        {
                            renameDialog.NameField = aggregationNode.Name;
                            if (renameDialog.ShowDialog() == true)
                            {
                                HaveUnsavedData = !aggregationNode.Name.Equals(renameDialog.NameField);
                                aggregationNode.Name = renameDialog.NameField;
                                aggregationNode.Aggregation.Name = renameDialog.NameField;
                            }
                        }
                        else if (SelectedNode is DeviceNode deviceNode)
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
                        if (HaveUnsavedData)
                            SaveExistStructure();
                        Config.GetConfig().AutosavePeriod = AutosavePeriod;
                        Config.GetConfig().AutosaveFilename = AutosaveFileName;
                        Config.GetConfig().Save();
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
                        var device = _deviceFactory.GetNewDevice(model, (ushort)id);
                        StructureNodes.Add(new DeviceNode
                        {
                            Device = device,
                            Name = device.Name,
                            AllowDownload = false,
                            AllowLoad = true,
                            AllowSave = true,
                            // Раскомментить, при доступности синхронизации
                            //AllowUpload = false
                            AllowUpload = true
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
                        CloseTabs(node);
                    });
                }
                return _removeNode;
            }
        }

        private void CloseTabs(NodeItem node)
        {
            if (node is FolderNode folderNode)
            {
                foreach (var childNode in folderNode.Nodes)
                {
                    CloseTabs(childNode);
                }
            }
            if (node is AggregationNode aggregationNode)
            {
                var tab = DeviceTabs.FirstOrDefault(t => t.DataContext == aggregationNode.Aggregation);
                if (!tab.IsNull())
                {
                    DeviceTabs.Remove(tab);
                }
            }
            if (node is DeviceNode deviceNode)
            {
                var tab = DeviceTabs.FirstOrDefault(t => t.DataContext == deviceNode.Device);
                if (!tab.IsNull())
                {
                    DeviceTabs.Remove(tab);
                }
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
                                Id = (byte)id,
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
                        if (string.IsNullOrEmpty(StructureFileName))
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

                        LoadStructure(dialog.FileName);
                    });
                }
                return _openStructure;
            }
        }

        private void LoadStructure(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                filename = string.Concat(AutosaveFileName, _defaultStructureFileExtension);
            }
            else
            {
                StructureFileName = filename;
            }
            if (!System.IO.File.Exists(filename))
                return;
            DeviceTabs.Clear();
            StructureNodes.Clear();

            var bytes = System.IO.File.ReadAllBytes(filename);
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
                    var device = _deviceFactory.FromSerializable(baseDevice.Model, serializableContainer.Content);
                    var deviceNode = new DeviceNode
                    {
                        Device = device,
                        Name = device.Name,
                        AllowDownload = false,
                        AllowLoad = true,
                        AllowSave = true,
                        // Раскомментить, при доступности синхронизации
                        //AllowUpload = false
                        AllowUpload = true
                    };
                    if (device.AggregationId.HasValue)
                    {
                        var aggregationNode = StructureNodes.OfType<AggregationNode>()
                            .FirstOrDefault(n => n.Aggregation.Id == device.AggregationId);
                        deviceNode.Parent = aggregationNode;
                        aggregationNode.Nodes.Add(deviceNode);
                        if (!device.AggregationOrder.HasValue)
                        {
                            var order = aggregationNode.Nodes.OfType<DeviceNode>().Max(d => d.Device.AggregationOrder ?? 0) + 1;
                            device.AggregationOrder = (ushort)order;
                        }
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
        }

        private void SaveExistStructure()
        {
            try
            {
                _logger.Debug(this, "Подготовка контейнеров");
                var serializableContainers = new List<SerializableContainer>();
                foreach (var node in StructureNodes)
                {
                    var deviceNode = node as DeviceNode;
                    var folderNode = node as FolderNode;
                    var aggregationNode = node as AggregationNode;
                    if (!deviceNode.IsNull())
                    {
                        _logger.Debug(this, $"Устройство [{deviceNode.Device.Name}]");
                        serializableContainers.Add(GetDeviceSerializableContainer(deviceNode));
                    }
                    else if (!folderNode.IsNull())
                    {
                        _logger.Debug(this, $"Папка [{folderNode.Folder.Name}]");
                        serializableContainers.Add(
                            new SerializableContainer
                            {
                                ContentType = ContentType.Folder,
                                Content = (SerializableFolder)folderNode.Folder
                            });
                        foreach (var deviceSubNode in folderNode.Nodes)
                        {
                            var deviceNodeInFolder = deviceSubNode as DeviceNode;
                            _logger.Debug(this, $"Устройство [{folderNode.Folder.Name}]/[{deviceNodeInFolder.Device.Name}]");
                            serializableContainers.Add(GetDeviceSerializableContainer(deviceNodeInFolder));
                        }
                    }
                    else if (!aggregationNode.IsNull())
                    {
                        _logger.Debug(this, $"Агрегация [{aggregationNode.Aggregation.Name}]");
                        serializableContainers.Add(
                            new SerializableContainer
                            {
                                ContentType = ContentType.Aggregator,
                                Content = (SerializableAggregation)aggregationNode.Aggregation
                            });
                        foreach (var deviceSubNode in aggregationNode.Nodes)
                        {
                            var deviceInAgregation = deviceSubNode as DeviceNode;
                            _logger.Debug(this, $"Устройство [{aggregationNode.Aggregation.Name}]/[{deviceInAgregation.Device.Name}]");
                            serializableContainers.Add(GetDeviceSerializableContainer(deviceInAgregation));
                        }
                    }
                }
                _logger.Debug(this, "Сериализация");
                var serialized = JsonConvert.SerializeObject(serializableContainers);
                _logger.Debug(this, "Упаковка");
                var datas = _compressor.Zip(serialized);
                var fileName = string.IsNullOrEmpty(StructureFileName) ?
                    string.Concat(AutosaveFileName, _defaultStructureFileExtension) :
                    StructureFileName;
                _logger.Debug(this, "Запись");
                System.IO.File.WriteAllBytes(fileName, datas);
                _logger.Info(this, $"Структура сохранена в {fileName}");
                HaveUnsavedData = false;
            }
            catch (Exception ex)
            {
                _logger.Error(this, $"Ошибка записи! {ex.Message}", ex);
            }
        }

        private SerializableContainer GetDeviceSerializableContainer(DeviceNode node)
        {
            var deviceSerializableContainer = new SerializableContainer
            {
                ContentType = ContentType.Device,
                Content = _deviceFactory.GetSerializable(node.Device)
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
                        ProcessDevicePlugin();
                    });
                }
                return _showDevicePlugin;
            }
        }

        private void ProcessDevicePlugin()
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
        }

        private DelegateCommand _about;
        public Input.ICommand About
        {
            get
            {
                if (_about.IsNull())
                {
                    _about = new DelegateCommand((o) =>
                    {
                        ShowAbout?.Invoke(this, new ShowAboutEventArgs
                        {
                            Version = Assembly.GetExecutingAssembly().GetName().Version.ToString()

                        });
                    });
                }
                return _about;
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
            IEnumerable<object> menuItems = null;
            var pack = _deviceFactory.GetControlsPack(deviceNode.Device, _logger);
            if (pack.IsNull())
            {
                return;
            }

            pack.DataChanged += (o, args) => HaveUnsavedData = true;
            menuItems = pack.MenuItems;

            var grid = GetDeviceItemGrid(deviceNode, pack.DesignPreviewControl, pack.CustomizationControl, menuItems, pack.OnPreviewAreaMouseDown);
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

        private ControlsPack _activeControlsPackInAggregation;
        private UserControl GetAggregationGrid(AggregationNode aggregationNode, DeviceNode selectedDeviceNode)
        {
            var pack = _deviceFactory.GetControlsPack(selectedDeviceNode.Device, _logger);
            pack.DataChanged += (o, args) => HaveUnsavedData = true;
            _activeControlsPackInAggregation = pack;
            var previewControl = GetAggregationPreviewGrid(aggregationNode, selectedDeviceNode);
            return GetDeviceItemGrid(selectedDeviceNode, previewControl, pack.CustomizationControl, pack.MenuItems, pack.OnPreviewAreaMouseDown);
        }

        private UserControl GetAggregationPreviewGrid(AggregationNode aggregationNode, DeviceNode selectedDeviceNode)
        {
            if (!aggregationNode.Nodes.Any())
            {
                return new UserControl();
            }
            if (aggregationNode.Nodes.Count == 1)
            {
                var pack = _activeControlsPackInAggregation;
                return pack.DesignPreviewControl;
            }
            var control = new UserControl();
            var grid = new Grid();
            control.Content = grid;
            grid.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star)
            });
            grid.RowDefinitions.Add(new RowDefinition
            {
                Height = new GridLength(1, GridUnitType.Star)
            });
            grid.RowDefinitions.Add(new RowDefinition
            {
                Height = GridLength.Auto
            });
            var column = 1;
            var nodeCount = aggregationNode.Nodes.Count();
            var deviceIndex = 1;
            foreach (var node in aggregationNode.Nodes.OrderBy(n => ((DeviceNode)n).Device.AggregationOrder))
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
                Grid.SetRow(splitter, 0);
                Grid.SetRowSpan(splitter, 2);

                grid.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star)
                });
                grid.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star)
                });
                var pack = _deviceFactory.GetControlsPack(((DeviceNode)node).Device, _logger);
                if (pack.Equals(_activeControlsPackInAggregation))
                {
                    pack = _activeControlsPackInAggregation;
                }
                var devicePreview = pack.DesignPreviewControl;
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
                    Grid.SetColumn(controlWrapper, column);
                    Grid.SetColumnSpan(controlWrapper, 2);
                    Grid.SetRow(controlWrapper, 0);
                }
                else
                {
                    grid.Children.Add(devicePreview);
                    Grid.SetColumn(devicePreview, column);
                    Grid.SetColumnSpan(devicePreview, 2);
                    Grid.SetRow(devicePreview, 0);
                }
                var toLeftButton = new Button
                {
                    Height = 27,
                    Content = new Image
                    {
                        Source = new BitmapImage(new Uri($@"pack://application:,,,/{
                            Assembly.GetExecutingAssembly().GetName().Name};component/Resources/previous.png", UriKind.Absolute))
                    },
                    IsEnabled = deviceIndex != 1
                };
                toLeftButton.Click += (o, e) => ChangeOrder(node, aggregationNode, -1);
                grid.Children.Add(toLeftButton);
                Grid.SetColumn(toLeftButton, column++);
                Grid.SetRow(toLeftButton, 1);
                var toRightButton = new Button
                {
                    Height = 27,
                    Content = new Image
                    {
                        Source = new BitmapImage(new Uri($@"pack://application:,,,/{
                        Assembly.GetExecutingAssembly().GetName().Name};component/Resources/next.png", UriKind.Absolute))
                    },
                    IsEnabled = deviceIndex != nodeCount
                };
                toRightButton.Click += (o, e) => ChangeOrder(node, aggregationNode, 1);
                grid.Children.Add(toRightButton);
                Grid.SetColumn(toRightButton, column++);
                Grid.SetRow(toRightButton, 1);
                deviceIndex++;
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

        private void ChangeOrder(NodeItem node, AggregationNode aggregationNode, int increment)
        {
            var currentDevide = ((DeviceNode)node).Device;
            var currentDeviceOrder = currentDevide.AggregationOrder.Value;
            var targetDevice = aggregationNode.Nodes.Select(n => ((DeviceNode)n).Device).FirstOrDefault(d => d.AggregationOrder == currentDevide.AggregationOrder + increment);
            currentDevide.AggregationOrder = targetDevice.AggregationOrder;
            targetDevice.AggregationOrder = currentDeviceOrder;
            HaveUnsavedData = true;
            if (DeviceTabs.Any(t => t.DataContext == aggregationNode))
            {
                ProcessAggregator(aggregationNode, aggregationNode.Nodes.FirstOrDefault() as DeviceNode);
            }
        }

        private static string GetDeviceTabItemTitle(DeviceNode deviceNode)
        {
            return $"{deviceNode.Device.Name} id:{deviceNode.Device.Id}";
        }

        private static string GetAggregationTabItemTitle(AggregationNode aggregationNode)
        {
            return $"{aggregationNode.Name}";
        }

        private UserControl GetDeviceItemGrid(DeviceNode deviceNode, UserControl previewControl, UserControl customizationControl, IEnumerable<object> toolbarItems, Action OnMouseUp)
        {
            var model = new CustomizationViewModel(deviceNode, _deviceFactory, _uploadingCommandsContainer, _compressor, _networkExchangeAgent, _logger, _config);
            model.NodeRenamed += ((sender, node) =>
            {
                var tab = DeviceTabs.FirstOrDefault(t => (t.DataContext is Device device) && device == node.Device);
                if (!tab.IsNull())
                {
                    ((ClosableTab)tab).Title = GetDeviceTabItemTitle(node);
                }
            });
            model.BeforeGettingSettings += ((sender, node) =>
            {
                _updatedNode = node;
                _tabItem = DeviceTabs.FirstOrDefault(t => (t.DataContext is Device device) && device == node.Device);
            });
            model.AfterGetingSettings += ((sender, device) =>
            {
                _updatedNode.Device = device;
                var pack = _deviceFactory.GetControlsPack(_updatedNode.Device, _logger);
                _dispatcher.Invoke(() => _tabItem.Content = GetDeviceItemGrid(_updatedNode, pack.DesignPreviewControl, pack.CustomizationControl, pack.MenuItems, pack.OnPreviewAreaMouseDown));
                _tabItem.DataContext = device;
            });
            model.ShowPreview += ((sender, device) =>
            {
                var pack = _deviceFactory.GetControlsPack(device, _logger);
                var previewViewModel = new PreviewViewModel(pack.PreviewController);
                ShowPreview?.Invoke(this, new ShowPreviewEventArgs
                {
                    ViewModel = previewViewModel,
                    PreviewControl = pack.PreviewPreviewControl
                });
            });
            var control = new CustomizationControl
            {
                DataContext = model
            };
            control.SetPreviewControl(previewControl);
            control.MouseUp += (s, e) => { OnMouseUp?.Invoke(); };
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
                          NewStructure?.Invoke(this, EventArgs.Empty);
                      });
                }
                return _clearStructure;
            }
        }

        public void ProceedClearStructure()
        {
            StructureNodes.Clear();
            DeviceTabs.Clear();
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
                var order = aggregationNode.Nodes.OfType<DeviceNode>().Where(d => d.Device.Id != deviceNode.Device.Id).Max(d => d.Device.AggregationOrder) ?? 0;
                deviceNode.Device.AggregationId = (ushort)aggregationNode.Aggregation.Id;
                deviceNode.Device.AggregationOrder = (ushort)(order + 1);
                if (DeviceTabs.Any(t => t.DataContext == aggregationNode))
                {
                    ProcessAggregator(aggregationNode, aggregationNode.Nodes.FirstOrDefault() as DeviceNode);
                }
            }
            if (targetNode is FolderNode folderNode)
            {
                deviceNode.Device.FolderId = (ushort)folderNode.Folder.Id;
            }
        }
    }
}
