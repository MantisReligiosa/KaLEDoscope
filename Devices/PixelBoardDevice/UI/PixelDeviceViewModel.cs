using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Abstractions;
using BaseDevice;
using PixelBoardDevice.UI.POCO;
using ServiceInterfaces;
using Input = System.Windows.Input;
using UiCommands;
using System.Linq;
using PixelBoardDevice.DomainObjects;

namespace PixelBoardDevice.UI
{
    public class PixelDeviceViewModel : Notified
    {
        private readonly PixelBoard _device;
        private readonly ILogger _logger;
        private readonly List<ZoneType> _zoneTypes = new List<ZoneType>
        {
            new ZoneType
            {
                Id = 1,
                Name = "Текст",
                AllowAnimation = true,
                AllowBitmap = false,
                AllowFont = true,
                AllowText = true
            },
            new ZoneType
            {
                Id = 2,
                Name = "Сенсор",
                AllowAnimation=false,
                AllowBitmap=false,
                AllowFont=true,
                AllowText=false,
            },
            new ZoneType
            {
                Id = 3,
                Name = "Тэг MQTT",
                AllowAnimation=false,
                AllowBitmap=false,
                AllowFont=true,
                AllowText=false,
            },
        };

        public event EventHandler OnNeedRedraw;
        public ObservableCollection<ZoneType> ZoneTypes { get; set; }
        public ObservableCollection<Screen> Screens { get; set; }
        public ObservableCollection<Zone> Zones { get; set; }

        public PixelDeviceViewModel(Device d, ILogger l, bool allowChangeBoardSize = false)
        {
            _device = (PixelBoard)d;
            _logger = l;
            ZoneTypes = new ObservableCollection<ZoneType>(_zoneTypes);
            Screens = new ObservableCollection<Screen>(_device.Screens);
            Zones = new ObservableCollection<Zone>();
            AllowChangeBoardSize = allowChangeBoardSize;
            DeviceHeight = _device.BoardSize.Height;
            DeviceWidth = _device.BoardSize.Width;
            AllowChangeBoardSize = _device.IsStandaloneConfiguration;
        }

        private bool _allowChangeBoardSize;
        public bool AllowChangeBoardSize
        {
            get
            {
                return _allowChangeBoardSize;
            }
            set
            {
                _allowChangeBoardSize = value;
                OnPropertyChanged(nameof(AllowChangeBoardSize));
            }
        }

        private int _deviceHeight;
        public int DeviceHeight
        {
            get
            {
                return _deviceHeight;
            }
            set
            {
                _deviceHeight = value;
                _device.BoardSize.Height = value;
                OnPropertyChanged(nameof(DeviceHeight));
                OnNeedRedraw?.Invoke(this, EventArgs.Empty);
            }
        }

        private int _deviceWidth;
        public int DeviceWidth
        {
            get
            {
                return _deviceWidth;
            }
            set
            {
                _deviceWidth = value;
                _device.BoardSize.Width = value;
                OnPropertyChanged(nameof(DeviceWidth));
                OnNeedRedraw?.Invoke(this, EventArgs.Empty);
            }
        }

        private Screen _selectedScreen;
        public Screen SelectedScreen
        {
            get
            {
                return _selectedScreen;
            }
            set
            {
                _selectedScreen = value;
                Zones.Clear();
                if (_selectedScreen != null)
                {
                    _selectedScreen.Zones.ForEach(z => Zones.Add(z));
                }
                SelectedZone = null;
                OnPropertyChanged(nameof(SelectedScreen));
                OnPropertyChanged(nameof(Zones));
            }
        }

        private Zone _selectedZone;
        public Zone SelectedZone
        {
            get
            {
                return _selectedZone;
            }
            set
            {
                _selectedZone = value;
                OnPropertyChanged(nameof(SelectedZone));
            }
        }

        private ZoneType _currentZoneType;
        public ZoneType CurrentZoneType
        {
            get
            {
                return _currentZoneType;
            }
            set
            {
                _currentZoneType = value;
                OnPropertyChanged(nameof(CurrentZoneType));
            }
        }

        private DelegateCommand _addScreen;
        public Input.ICommand AddScreen
        {
            get
            {
                if (_addScreen == null)
                {
                    _addScreen = new DelegateCommand((o) =>
                    {
                        var nextOrderValue = _device.Screens.Max(s => s.Order) + 1;
                        var nextId = _device.Screens.Max(x => x.Id) + 1;
                        var screen = new Screen
                        {
                            Order = nextOrderValue,
                            Id = nextId,
                            Name = $"Экран{nextOrderValue}",
                            Period = 10,
                            Zones = new List<Zone>()
                        };
                        Screens.Add(screen);
                        _device.Screens.Add(screen);
                    });
                }
                return _addScreen;
            }
        }

        private DelegateCommand _deleteScreen;
        public Input.ICommand DeleteScreen
        {
            get
            {
                if (_deleteScreen == null)
                {
                    _deleteScreen = new DelegateCommand((o) =>
                    {
                        Screens.Remove(SelectedScreen);
                        _device.Screens.Remove(SelectedScreen);
                        SelectedScreen = null;
                    });
                }
                return _deleteScreen;
            }
        }

        private DelegateCommand _addZone;
        public Input.ICommand AddZone
        {
            get
            {
                if (_addZone == null)
                {
                    _addZone = new DelegateCommand((o) =>
                    {
                        var zone = new Ticker
                        {
                            Name = "Бегущая строка"
                        };
                        SelectedScreen.Zones.Add(zone);
                        Zones.Add(zone);
                        var deviceScreen = _device.Screens.FirstOrDefault(s => s.Id == SelectedScreen.Id);
                        deviceScreen.Zones.Add(zone);
                        OnPropertyChanged(nameof(Zones));
                    });
                }
                return _addZone;
            }
        }

        private DelegateCommand _deleteZone;
        public Input.ICommand DeleteZone
        {
            get
            {
                if (_deleteZone == null)
                {
                    _deleteZone = new DelegateCommand((o) =>
                    {
                        var zone = SelectedZone;
                        SelectedScreen.Zones.Remove(zone);
                        Zones.Remove(zone);
                        var deviceScreen = _device.Screens.FirstOrDefault(s => s.Id == SelectedScreen.Id);
                        deviceScreen.Zones.Remove(zone);
                        OnPropertyChanged(nameof(Zones));
                    });
                }
                return _deleteZone;
            }
        }
    }
}
