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
using Font = System.Windows.Media;
using System.Drawing.Text;
using BitmapProcessing;
using System.Windows;

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
                AllowText = true,
                ZoneCondition = (z) => z is Ticker,
                Customize = () => new Ticker()
            },
            new ZoneType
            {
                Id = 2,
                Name = "Сенсор",
                AllowAnimation=false,
                AllowBitmap=false,
                AllowFont=true,
                AllowText=false,
                ZoneCondition = (z) => z is Sensor,
                Customize = () => new Sensor()
            },
            new ZoneType
            {
                Id = 3,
                Name = "Тэг MQTT",
                AllowAnimation = false,
                AllowBitmap = false,
                AllowFont = true,
                AllowText = false,
                ZoneCondition = (z) => z is MQTTSensor,
                Customize = () => new MQTTSensor()
            },
            new ZoneType
            {
                Id = 4,
                Name = "Изображение",
                AllowAnimation=false,
                AllowBitmap=false,
                AllowFont=true,
                AllowText=false,
                ZoneCondition = (z) => z is Picture,
                Customize = () => new Picture()
            },
        };
        private readonly List<int> _fontSizes = new List<int>
        {
            8,9,10,11,12,14,16,18,20,22,24,26,28,36,48,72
        };

        public event EventHandler OnNeedRedraw;
        public ObservableCollection<ZoneType> ZoneTypes { get; set; }
        public ObservableCollection<Screen> Screens { get; set; }
        public ObservableCollection<Zone> Zones { get; set; }
        public ObservableCollection<Font.FontFamily> Fonts { get; set; }
        public ObservableCollection<int> FontSizes { get; set; }

        public PixelDeviceViewModel(Device d, ILogger l, bool allowChangeBoardSize = false)
        {
            _device = (PixelBoard)d;
            _logger = l;
            ZoneTypes = new ObservableCollection<ZoneType>(_zoneTypes);
            Screens = new ObservableCollection<Screen>(_device.Screens);
            Fonts = new ObservableCollection<Font.FontFamily>(new InstalledFontCollection().Families.Select(f => new Font.FontFamily(f.Name)));
            Zones = new ObservableCollection<Zone>();
            FontSizes = new ObservableCollection<int>(_fontSizes);
            AllowChangeBoardSize = allowChangeBoardSize;
            DeviceHeight = _device.BoardSize.Height;
            DeviceWidth = _device.BoardSize.Width;
            AllowChangeBoardSize = _device.IsStandaloneConfiguration;
        }

        private int _zoneLeft;
        public int ZoneLeft
        {
            get => _zoneLeft;
            set
            {
                _zoneLeft = value;
                var deviceZone = GetDeviceZone(SelectedScreen.Id, SelectedZone.Id);
                if (deviceZone != null)
                {
                    deviceZone.X = value;
                }
                OnPropertyChanged(nameof(ZoneLeft));
            }
        }

        private Font.FontFamily _selectedFont;
        public Font.FontFamily SelectedFont
        {
            get => _selectedFont;
            set
            {
                _selectedFont = value;
                if (_selectedFont != null && SelectedFontSize != 0)
                {
                    UpdateZoneFont(SelectedScreen, SelectedZone, value, SelectedFontSize, IsItalic, IsBold);
                }
                OnPropertyChanged(nameof(SelectedFont));
            }
        }

        private void UpdateZoneFont(Screen screen, Zone zone, Font.FontFamily newFont, int newFontSize, bool italic, bool bold)
        {
            var deviceZone = GetDeviceZone(screen.Id, zone.Id) as IFonted;
            if (deviceZone == null)
            {
                return;
            }
            var existBinaryFont = _device.Fonts.FirstOrDefault(bf => bf.Id == deviceZone.FontId);
            if (existBinaryFont != null)
            {
                var numberOfFontEntry = _device.Screens.Sum(s => s.Zones.OfType<IFonted>().Count(f => f.FontId == existBinaryFont.Id));
                if (numberOfFontEntry <= 1)
                {
                    _device.Fonts.Remove(existBinaryFont);
                }
            }
            var newBinaryFont = _device.Fonts
                .FirstOrDefault(bf => bf.Height == newFontSize && bf.Source == newFont.Source && bf.Italic == italic && bf.Bold == bold);
            if (newBinaryFont == null)
            {
                var newBinaryFontId = _device.Fonts.Any() ? _device.Fonts.Max(f => f.Id) + 1 : 0;
                newBinaryFont = new BinaryFont
                {
                    Id = newBinaryFontId,
                    Source = newFont.Source,
                    Height = newFontSize,
                    Bold = bold,
                    Italic = italic,
                    Base64Bitmap = BitmapProcessor.GenerateBase64FontMono(_device.Alphabet, newFont, newFontSize)
                };
                _device.Fonts.Add(newBinaryFont);
            }
            deviceZone.FontId = newBinaryFont.Id;
            (GetDeviceZone(screen.Id, zone.Id) as IFonted).FontId = newBinaryFont.Id;
        }

        private Zone GetDeviceZone(int screenId, int zoneId)
        {
            return _device?.Screens?.FirstOrDefault(s => s.Id == screenId)?.Zones?.FirstOrDefault(z => z.Id == zoneId);
        }

        private int _selectedFontSize;
        public int SelectedFontSize
        {
            get => _selectedFontSize;
            set
            {
                _selectedFontSize = value;
                if (SelectedFont != null && _selectedFontSize != 0)
                {
                    UpdateZoneFont(SelectedScreen, SelectedZone, SelectedFont, value, IsItalic, IsBold);
                }
                OnPropertyChanged(nameof(SelectedFontSize));
            }
        }

        private bool _isBold;
        public bool IsBold
        {
            get => _isBold;
            set
            {
                _isBold = value;
                if (SelectedFont != null && _selectedFontSize != 0)
                {
                    UpdateZoneFont(SelectedScreen, SelectedZone, SelectedFont, SelectedFontSize, IsItalic, value);
                }
                OnPropertyChanged(nameof(IsBold));
                OnPropertyChanged(nameof(FontWeight));
            }
        }

        public FontWeight FontWeight => IsBold ? FontWeights.Bold : FontWeights.Normal;

        private bool _isItalic;
        public bool IsItalic
        {
            get => _isItalic;
            set
            {
                _isItalic = value;
                if (SelectedFont != null && _selectedFontSize != 0)
                {
                    UpdateZoneFont(SelectedScreen, SelectedZone, SelectedFont, SelectedFontSize, value, IsBold);
                }
                OnPropertyChanged(nameof(IsItalic));
                OnPropertyChanged(nameof(FontStyle));
            }
        }

        public FontStyle FontStyle => IsItalic ? FontStyles.Italic : FontStyles.Normal;

        private bool _allowChangeBoardSize;
        public bool AllowChangeBoardSize
        {
            get => _allowChangeBoardSize;
            set
            {
                _allowChangeBoardSize = value;
                OnPropertyChanged(nameof(AllowChangeBoardSize));
            }
        }

        private bool _allowFont;
        public bool AllowFont
        {
            get => _allowFont;
            set
            {
                _allowFont = value;
                OnPropertyChanged(nameof(AllowFont));
            }
        }

        private bool _allowAnimation;
        public bool AllowAnimation
        {
            get => _allowAnimation;
            set
            {
                _allowAnimation = value;
                OnPropertyChanged(nameof(AllowAnimation));
            }
        }

        private bool _allowBitmap;
        public bool AllowBitmap
        {
            get => _allowBitmap;
            set
            {
                _allowBitmap = value;
                OnPropertyChanged(nameof(AllowBitmap));
            }
        }

        private bool _allowMQTT;
        public bool AllowMQTT
        {
            get => _allowMQTT;
            set
            {
                _allowMQTT = value;
                OnPropertyChanged(nameof(AllowMQTT));
            }
        }

        private bool _allowText;
        public bool AllowText
        {
            get => _allowText;
            set
            {
                _allowText = value;
                OnPropertyChanged(nameof(AllowText));
            }
        }

        private int _deviceHeight;
        public int DeviceHeight
        {
            get => _deviceHeight;
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
            get => _deviceWidth;
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
            get => _selectedScreen;
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
            get => _selectedZone;
            set
            {
                _selectedZone = value;
                if (_selectedZone != null)
                {
                    ZoneLeft = _selectedZone.X;
                }
                OnPropertyChanged(nameof(SelectedZone));
                var currentZoneType = ZoneTypes.FirstOrDefault(z => z.ZoneCondition(_selectedZone));
                if (currentZoneType == null || CurrentZoneType == null || CurrentZoneType.Id != currentZoneType.Id)
                {
                    CurrentZoneType = currentZoneType;
                    AllowAnimation = currentZoneType?.AllowAnimation ?? false;
                    AllowBitmap = currentZoneType?.AllowBitmap ?? false;
                    AllowFont = currentZoneType?.AllowFont ?? false;
                    AllowMQTT = currentZoneType?.AllowMQTT ?? false;
                    AllowText = currentZoneType?.AllowText ?? false;
                    OnPropertyChanged(nameof(CurrentZoneType));
                }
                if (_selectedZone is IFonted)
                {
                    var fontId = (_selectedZone as IFonted).FontId;
                    var binaryFont = _device.Fonts.FirstOrDefault(bf => bf.Id == fontId);
                    if (binaryFont == null)
                    {
                        SelectedFont = null;
                    }
                    else
                    {
                        SelectedFont = Fonts.FirstOrDefault(f => f.Source.Equals(binaryFont.Source));
                        SelectedFontSize = binaryFont.Height;
                    }
                    if (_selectedZone is Ticker)
                    {
                        Text = (_selectedZone as Ticker).Text;
                    }
                    else
                    {
                        Text = String.Empty;
                    }
                }
            }
        }

        private string _text;
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                var ticker = GetDeviceZone(SelectedScreen.Id, SelectedZone.Id) as Ticker;
                if (ticker != null)
                {
                    ticker.Text = value;
                }
                OnPropertyChanged(nameof(Text));
            }
        }

        private ZoneType _currentZoneType;
        public ZoneType CurrentZoneType
        {
            get => _currentZoneType;
            set
            {
                _currentZoneType = value;
                OnPropertyChanged(nameof(CurrentZoneType));

                if (SelectedZone == null)
                    return;
                var newZone = value.Customize();
                newZone.Id = SelectedZone.Id;
                if (newZone.Name != SelectedZone.Name)
                {
                    var zones = Zones.ToList();
                    Zones.Clear();
                    foreach (var zone in zones)
                    {
                        if (zone.Id == newZone.Id)
                        {
                            Zones.Add(newZone);
                        }
                        else
                        {
                            Zones.Add(zone);
                        }
                    }
                    SelectedZone = newZone;
                    OnPropertyChanged(nameof(SelectedZone));
                    OnPropertyChanged(nameof(Zones));
                    var deviceScreen = _device.Screens.FirstOrDefault(s => s.Id == SelectedScreen.Id);
                    var deviceScreenZones = new List<Zone>();
                    foreach (var zone in deviceScreen.Zones)
                    {
                        if (zone.Id == newZone.Id)
                        {
                            deviceScreenZones.Add(newZone);
                        }
                        else
                        {
                            deviceScreenZones.Add(zone);
                        }
                    }
                    deviceScreen.Zones = deviceScreenZones;
                }
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
                        var nextId = Screens.Max(s => s.Zones.Max(z => z.Id)) + 1;
                        var zone = new Ticker
                        {
                            Id = nextId

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

        public ILogger Logger => _logger;
    }
}
