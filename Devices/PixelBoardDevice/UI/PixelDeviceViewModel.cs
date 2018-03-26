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
using Font = System.Windows.Media;
using System.Drawing.Text;
using BitmapProcessing;
using System.Windows;
using System.Drawing;
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
                AllowText = true,
                ZoneCondition = (z) => (z!=null) && z.ZoneType==(int)DomainObjects.ZoneTypes.Text,
                Customize = () => new Zone
                {
                    ZoneType=(int)PixelBoardDevice.DomainObjects.ZoneTypes.Text,
                    IsValid =true,
                    Name = "Текст",
                }
            },
            new ZoneType
            {
                Id = 2,
                Name = "Датчик",
                AllowAnimation=false,
                AllowBitmap=false,
                AllowFont=true,
                AllowText=false,
                ZoneCondition = (z) => (z!=null)&&z.ZoneType==(int)DomainObjects.ZoneTypes.Sensor,
                Customize = () => new Zone{
                    ZoneType = (int)DomainObjects.ZoneTypes.Sensor,
                    IsValid=true,
                    Name = "Датчик",
                }
            },
            new ZoneType
            {
                Id = 3,
                Name = "Тэг внешнего сервера",
                AllowAnimation = false,
                AllowBitmap = false,
                AllowFont = true,
                AllowText = false,
                AllowMQTT = true,
                ZoneCondition = (z) => (z!=null)&&z.ZoneType==(int)DomainObjects.ZoneTypes.MQTT,
                Customize = () => new Zone{
                    ZoneType=(int)DomainObjects.ZoneTypes.MQTT,
                    IsValid =true,
                    Name = "Тэг внешнего сервера",
                }
            },
            new ZoneType
            {
                Id = 4,
                Name = "Изображение",
                AllowAnimation=false,
                AllowBitmap=true,
                AllowFont=false,
                AllowText=false,
                ZoneCondition = (z) => (z!=null)&&z.ZoneType==(int)DomainObjects.ZoneTypes.Picture,
                Customize = () => new Zone{
                    ZoneType=(int)DomainObjects.ZoneTypes.Picture,
                    IsValid =true,
                    Name = "Изображение",
                }
            },
            new ZoneType
            {
                Id = 4,
                Name = "Часы",
                AllowAnimation=false,
                AllowBitmap=false,
                AllowFont=true,
                AllowText=false,
                ZoneCondition = (z) => (z!=null)&&z.ZoneType==(int)DomainObjects.ZoneTypes.Clock,
                Customize = () => new Zone{
                    IsValid=true,
                    ZoneType=(int)DomainObjects.ZoneTypes.Clock,
                    Name = "Часы",
                }
            },
        };
        private readonly List<ClockType> _clockTypes = new List<ClockType>
        {
            new ClockType
            {
                Id=1,
                Name="ЧЧ(24):ММ"
            },
            new ClockType
            {
                Id=2,
                Name="ЧЧ(24):ММ:CC"
            },
                        new ClockType
            {
                Id=3,
                Name="ЧЧ(12):ММ"
            },
            new ClockType
            {
                Id=4,
                Name="ЧЧ(12):ММ:CC"
            }
        };

        private readonly List<int> _fontSizes = new List<int>
        {
            8,9,10,11,12,14,16,18,20,22,24,26,28,36,48,72
        };

        public ObservableCollection<ZoneType> ZoneTypes { get; set; }
        public ObservableCollection<Screen> Screens { get; set; }
        public ObservableCollection<Zone> Zones { get; set; }
        public ObservableCollection<Font.FontFamily> Fonts { get; set; }
        public ObservableCollection<int> FontSizes { get; set; }

        public PixelDeviceViewModel(Device d, ILogger l, bool allowChangeBoardSize = false)
        {
            PropertyChanged += RedrawPreviewOnPropertyChanged;
            _device = (PixelBoard)d;
            _logger = l;
            ZoneTypes = new ObservableCollection<ZoneType>(_zoneTypes);
            Screens = new ObservableCollection<Screen>(_device.Screens);
            Fonts = new ObservableCollection<Font.FontFamily>(new InstalledFontCollection().Families.Select(f => new Font.FontFamily(f.Name)));
            Zones = new ObservableCollection<Zone>();
            Zones.CollectionChanged += Zones_CollectionChanged;
            SelectedScreen = Screens.FirstOrDefault();
            FontSizes = new ObservableCollection<int>(_fontSizes);
            AllowChangeBoardSize = allowChangeBoardSize;
            DeviceHeight = _device.BoardSize.Height;
            DeviceWidth = _device.BoardSize.Width;
            AllowChangeBoardSize = _device.IsStandaloneConfiguration;
        }

        private void Zones_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ValidateZones();
            RedrawPreview();
        }

        private void RedrawPreviewOnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(PreviewImage)))
            {
                return;
            }
            ValidateZones();
            RedrawPreview();
        }

        private readonly Dictionary<Func<Zone, bool>, Action<Zone, PixelBoard, Graphics>> zoneRenders
            = new Dictionary<Func<Zone, bool>, Action<Zone, PixelBoard, Graphics>>
            {
                {
                    (z) => z.ZoneType==(int)DomainObjects.ZoneTypes.Text,
                    (zone, _device ,g) =>
                    {
                        RenderText(g, _device.Fonts.FirstOrDefault(f => f.Id == zone.FontId), zone.Text, zone.X, zone.Y, zone.Width, zone.Height);
                    }
                },
                {
                    (z) => z.ZoneType==(int)DomainObjects.ZoneTypes.Sensor,
                    (zone, _device, g) =>
                    {
                        RenderText(g, _device.Fonts.FirstOrDefault(f => f.Id == zone.FontId), "123", zone.X, zone.Y, zone.Width, zone.Height);
                    }
                },
                {
                    (z) => z.ZoneType==(int)DomainObjects.ZoneTypes.MQTT,
                    (zone, _device, g) =>
                    {
                        RenderText(g, _device.Fonts.FirstOrDefault(f => f.Id == zone.FontId), "999", zone.X, zone.Y, zone.Width, zone.Height);
                    }
                }
            };

        private static void RenderText(Graphics g, BinaryFont binaryFont, string text, int x, int y, int width, int height)
        {
            if (binaryFont != null)
            {
                System.Drawing.FontStyle style = System.Drawing.FontStyle.Regular;
                if (binaryFont.Italic && binaryFont.Bold)
                {
                    style = System.Drawing.FontStyle.Italic | System.Drawing.FontStyle.Bold;
                }
                else if (binaryFont.Italic)
                {
                    style = System.Drawing.FontStyle.Italic;
                }
                else if (binaryFont.Bold)
                {
                    style = System.Drawing.FontStyle.Bold;
                }
                var font = new System.Drawing.Font(
                    binaryFont.Source, binaryFont.Height, style, GraphicsUnit.Pixel);
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
                g.DrawString(text, font, new SolidBrush(Color.Red), new RectangleF(x, y, width, height));
            }
        }


        private void RedrawPreview()
        {
            if (DeviceHeight == 0 || DeviceWidth == 0)
            {
                return;
            }
            int width = DeviceWidth;
            int height = DeviceHeight;
            var bitmap = new Bitmap(width, height);

            using (var g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.Black);
                if (Zones?.Any() ?? false)
                {
                    foreach (var zone in Zones)
                    {
                        var pen = new Pen(Color.Yellow);
                        if (zone.Id == (SelectedZone?.Id ?? int.MinValue))
                        {
                            pen = new Pen(Color.Green);
                        }
                        g.DrawRectangle(pen, zone.X, zone.Y, zone.Width, zone.Height);
                        var renderer = zoneRenders.FirstOrDefault(kvp => kvp.Key(zone));
                        if (renderer.Key != null && renderer.Value != null)
                        {
                            renderer.Value(zone, _device, g);
                        }
                    }
                }
            }
            var hBitmap = bitmap.GetHbitmap();
            var bitSrc = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty,
                System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            PreviewImage = bitSrc;
        }

        private void ValidateZones()
        {
#warning тут будет валидация
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

        private int _zoneTop;
        public int ZoneTop
        {
            get => _zoneTop;
            set
            {
                _zoneTop = value;
                var deviceZone = GetDeviceZone(SelectedScreen.Id, SelectedZone.Id);
                if (deviceZone != null)
                {
                    deviceZone.Y = value;
                }
                OnPropertyChanged(nameof(ZoneTop));
            }
        }

        private int _zoneHeight;
        public int ZoneHeight
        {
            get => _zoneHeight;
            set
            {
                _zoneHeight = value;
                var deviceZone = GetDeviceZone(SelectedScreen.Id, SelectedZone.Id);
                if (deviceZone != null)
                {
                    deviceZone.Height = value;
                }
                OnPropertyChanged(nameof(ZoneHeight));
            }
        }

        private int _zoneWidth;
        public int ZoneWidth
        {
            get => _zoneWidth;
            set
            {
                _zoneWidth = value;
                var deviceZone = GetDeviceZone(SelectedScreen.Id, SelectedZone.Id);
                if (deviceZone != null)
                {
                    deviceZone.Width = value;
                }
                OnPropertyChanged(nameof(ZoneWidth));
            }
        }

        private object _previewImage;
        public object PreviewImage
        {
            get
            {
                return _previewImage;
            }
            set
            {
                _previewImage = value;
                OnPropertyChanged(nameof(PreviewImage));
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
            var deviceZone = GetDeviceZone(screen.Id, zone.Id);
            if (!deviceZone.IsFonted)
            {
                return;
            }
            var existBinaryFont = _device.Fonts.FirstOrDefault(bf => bf.Id == deviceZone.FontId);
            if (existBinaryFont != null)
            {
                var numberOfFontEntry = _device.Screens.Sum(s => s.Zones.Count(f => f.IsFonted && f.FontId == existBinaryFont.Id));
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
                    Base64Bitmap = BitmapProcessor.GenerateBase64FontMono(_device.Alphabet, newFont, italic, bold, newFontSize)
                };
                _device.Fonts.Add(newBinaryFont);
            }
            deviceZone.FontId = newBinaryFont.Id;
            GetDeviceZone(screen.Id, zone.Id).FontId = newBinaryFont.Id;
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

        public System.Windows.FontStyle FontStyle => IsItalic ? FontStyles.Italic : FontStyles.Normal;

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

        public bool AllowZoneCoordinates => SelectedZone != null;

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

        private bool AllowAnimation
        {
            set => AnimationVisibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        private Visibility _allowVisibility;
        public Visibility AnimationVisibility
        {
            get => _allowVisibility;
            set
            {
                _allowVisibility = value;
                OnPropertyChanged(nameof(AnimationVisibility));
            }
        }

        private bool AllowBitmap
        {
            set => BitmapVisibility = value ? Visibility.Visible : Visibility.Collapsed;
        }
        private Visibility _allowBitmap;
        public Visibility BitmapVisibility
        {
            get => _allowBitmap;
            set
            {
                _allowBitmap = value;
                OnPropertyChanged(nameof(BitmapVisibility));
            }
        }

        private bool AllowExternalTag
        {
            set => ExternalTagVisibility = value ? Visibility.Visible : Visibility.Collapsed;
        }
        private Visibility _externalTagVisibility;
        public Visibility ExternalTagVisibility
        {
            get => _externalTagVisibility;
            set
            {
                _externalTagVisibility = value;
                OnPropertyChanged(nameof(ExternalTagVisibility));
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
                    ZoneTop = _selectedZone.Y;
                    ZoneHeight = _selectedZone.Height;
                    ZoneWidth = _selectedZone.Width;
                }
                OnPropertyChanged(nameof(SelectedZone));
                OnPropertyChanged(nameof(AllowZoneCoordinates));
                var currentZoneType = ZoneTypes.FirstOrDefault(z => z.ZoneCondition(_selectedZone));
                if (currentZoneType == null || CurrentZoneType == null || CurrentZoneType.Id != currentZoneType.Id)
                {
                    CurrentZoneType = currentZoneType;
                    AllowAnimation = currentZoneType?.AllowAnimation ?? false;
                    AllowBitmap = currentZoneType?.AllowBitmap ?? false;
                    AllowFont = currentZoneType?.AllowFont ?? false;
                    AllowExternalTag = currentZoneType?.AllowMQTT ?? false;
                    AllowText = currentZoneType?.AllowText ?? false;
                    OnPropertyChanged(nameof(CurrentZoneType));
                }
                if (_selectedZone != null && _selectedZone.IsFonted)
                {
                    var fontId = _selectedZone.FontId;
                    var binaryFont = _device.Fonts.FirstOrDefault(bf => bf.Id == fontId);
                    if (binaryFont == null)
                    {
                        SelectedFont = null;
                    }
                    else
                    {
                        SelectedFont = Fonts.FirstOrDefault(f => f.Source.Equals(binaryFont.Source));
                        SelectedFontSize = binaryFont.Height;
                        IsItalic = binaryFont.Italic;
                        IsBold = binaryFont.Bold;
                    }
                    if (_selectedZone.ZoneType == (int)DomainObjects.ZoneTypes.Text)
                    {
                        Text = _selectedZone.Text;
                    }
                    else
                    {
                        Text = String.Empty;
                    }
                    if (_selectedZone.ZoneType == (int)DomainObjects.ZoneTypes.MQTT)
                    {
                        ExternalSourceTag = _selectedZone.ExternalSourceTag;
                    }
                    else
                    {
                        ExternalSourceTag = string.Empty;
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
                var ticker = GetDeviceZone(SelectedScreen.Id, SelectedZone.Id);
                if (ticker.ZoneType == (int)DomainObjects.ZoneTypes.Text)
                {
                    ticker.Text = value;
                }
                OnPropertyChanged(nameof(Text));
            }
        }

        private string _externalSourceTag;
        public string ExternalSourceTag
        {
            get => _externalSourceTag;
            set
            {
                _externalSourceTag = value;
                var zone = GetDeviceZone(SelectedScreen.Id, SelectedZone.Id);
                if (zone.ZoneType == (int)DomainObjects.ZoneTypes.MQTT)
                {
                    zone.ExternalSourceTag = value;
                }
                OnPropertyChanged(nameof(ExternalSourceTag));
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
                newZone.X = SelectedZone.X;
                newZone.Y = SelectedZone.Y;
                newZone.Width = SelectedZone.Width;
                newZone.Height = SelectedZone.Height;
                if (newZone.IsFonted)
                {
                    newZone.FontId = (SelectedZone.IsFonted) ? SelectedZone.FontId : null;
                }
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

        private DelegateCommand _loadBitmap;
        public Input.ICommand LoadBitmap
        {
            get
            {
                if (_loadBitmap == null)
                {
                    _loadBitmap = new DelegateCommand((o) =>
                      {
#warning Дописать
                      });
                }
                return _loadBitmap;
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
                        var nextId = Screens.Max(s => s.Zones.Any() ? s.Zones.Max(z => z.Id) : 0) + 1;
                        var zone = new Zone
                        {
                            ZoneType = (int)DomainObjects.ZoneTypes.Text,
                            Id = nextId,
                            Name = "Текст"
                        };
                        SelectedScreen.Zones.Add(zone);
                        Zones.Add(zone);
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
                        OnPropertyChanged(nameof(Zones));
                    });
                }
                return _deleteZone;
            }
        }

        public ILogger Logger => _logger;
    }
}
