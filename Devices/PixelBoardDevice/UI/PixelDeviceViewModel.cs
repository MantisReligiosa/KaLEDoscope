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
using Microsoft.Win32;
using System.Collections;
using Extensions;

namespace PixelBoardDevice.UI
{
    public class PixelDeviceViewModel : Notified
    {
        public event EventHandler ModelChanged;
        public readonly PixelBoard Device;
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
                AllowTextEditing=true,
                AllowClock = false,
                ZoneCondition = (z) => (!z.IsNull()) && z.ZoneType==(int)DomainObjects.ZoneTypes.Text,
                Customize = () => new Zone
                {
                    ZoneType=(int)DomainObjects.ZoneTypes.Text,
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
                AllowText=true,
                AllowMQTT=false,
                AllowTextEditing=false,
                AllowClock = false,
                ZoneCondition = (z) => (!z.IsNull())&&z.ZoneType==(int)DomainObjects.ZoneTypes.Sensor,
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
                AllowClock = false,
                ZoneCondition = (z) => (!z.IsNull())&&z.ZoneType==(int)DomainObjects.ZoneTypes.MQTT,
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
                AllowClock = false,
                ZoneCondition = (z) => (!z.IsNull())&&z.ZoneType==(int)DomainObjects.ZoneTypes.Picture,
                Customize = () => new Zone{
                    ZoneType=(int)DomainObjects.ZoneTypes.Picture,
                    IsValid =true,
                    Name = "Изображение",
                }
            },
            new ZoneType
            {
                Id = 5,
                Name = "Часы",
                AllowAnimation = false,
                AllowBitmap = false,
                AllowFont = true,
                AllowText = false,
                AllowClock = true,
                ZoneCondition = (z) => (!z.IsNull())&&z.ZoneType==(int)DomainObjects.ZoneTypes.Clock,
                Customize = () => new Zone{
                    IsValid=true,
                    ZoneType=(int)DomainObjects.ZoneTypes.Clock,
                    Name = "Часы",
                }
            },
        };
        private static readonly List<ClockType> _clockTypes = new List<ClockType>
        {
            new ClockType
            {
                Id = 1,
                Name = "Текстовый",
                AllowFormat = true
            },
            new ClockType
            {
                Id = 2,
                Name = "Графический",
                AllowFormat = false
            }
        };
        private static readonly List<ClockFormat> _clockFormats = new List<ClockFormat>
        {
            new ClockFormat
            {
                Id = 1,
                Name = "ЧЧ(24):ММ",
                Sample="13:45"
            },
            new ClockFormat
            {
                Id = 2,
                Name = "ЧЧ(24):ММ:CC",
                Sample = "13:45:30"
            },
            new ClockFormat
            {
                Id = 3,
                Name = "ЧЧ(12):ММ",
                Sample="1:45"
            },
            new ClockFormat
            {
                Id = 4,
                Name = "ЧЧ(12):ММ:CC",
                Sample="1:45:30"
            }
        };

        private readonly List<int> _fontSizes = new List<int>
        {
            8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72
        };

        public ObservableCollection<ZoneType> ZoneTypes { get; set; }
        public ObservableCollection<Program> Programs { get; set; }
        public ObservableCollection<Zone> Zones { get; set; }
        public ObservableCollection<Font.FontFamily> Fonts { get; set; }
        public ObservableCollection<int> FontSizes { get; set; }
        public ObservableCollection<ClockFormat> ClockFormats { get; set; }
        public ObservableCollection<ClockType> ClockTypes { get; set; }

        public PixelDeviceViewModel(Device d, ILogger l, bool allowChangeBoardSize = false)
        {
            PropertyChanged += (s, e) => ValidateAndInvokePreview();
            Device = (PixelBoard)d;
            _logger = l;
            ZoneTypes = new ObservableCollection<ZoneType>(_zoneTypes);
            Programs = new ObservableCollection<Program>(Device.Programs);
            Fonts = new ObservableCollection<Font.FontFamily>(new InstalledFontCollection().Families.Select(f => new Font.FontFamily(f.Name)));
            Zones = new ObservableCollection<Zone>();
            ClockFormats = new ObservableCollection<ClockFormat>(_clockFormats);
            ClockTypes = new ObservableCollection<ClockType>(_clockTypes);
            Zones.CollectionChanged += (s, e) => ValidateAndInvokePreview();
            SelectedProgram = Programs.FirstOrDefault();
            FontSizes = new ObservableCollection<int>(_fontSizes);
            AllowChangeBoardSize = allowChangeBoardSize;
            PreviewScale = 1;
            PreviewScaleMinRate = .2;
            PreviewScaleMaxRate = 5;
            DeviceHeight = Device.BoardSize.Height;
            DeviceWidth = Device.BoardSize.Width;
            AllowChangeBoardSize = Device.IsStandaloneConfiguration;
            AllowAnimation(false);
            AllowBitmap(false);
            AllowFont = false;
            AllowExternalTag(false);
            AllowText(false);
        }

        private void ValidateAndInvokePreview()
        {
            ValidateZones();
            ModelChanged?.Invoke(this, EventArgs.Empty);
        }

        private static void RenderBitmap(Graphics g, Zone zone)
        {
            var base64 = zone.BitmapBase64;
            if (String.IsNullOrEmpty(base64))
            {
                return;
            }
            var bytes = Convert.FromBase64String(base64);
            var bitArray = new BitArray(bytes);
            var bitmapHeight = zone.BitmapHeight;
            var bitmapWidth = bitArray.Length / bitmapHeight;
            var x = 1;
            var y = 1;
            foreach (var bit in bitArray)
            {
                if ((bool)bit)
                {
                    g.FillRectangle(new SolidBrush(Color.Red), new Rectangle(zone.X + x, zone.Y + y, 1, 1));
                }
                y++;
                if (y > bitmapHeight)
                {
                    y = 1;
                    x++;
                    if (x > bitmapWidth)
                    {
                        break;
                    }
                }
            }
        }

        private void ValidateZones()
        {
            if (Zones.IsNull())
                return;
            var incorrectZonesId = new List<int>();
            foreach (var zone in Zones)
            {
                if (incorrectZonesId.Any(id=>id==zone.Id))
                {
                    zone.IsValid = false;
                    continue;
                }
                zone.IsValid = true;
                foreach (var concurrentZone in Zones.Except(new List<Zone> { zone }))
                {
                    if (zone.IntersectWith(concurrentZone))
                    {
                        zone.IsValid = false;
                        incorrectZonesId.Add(zone.Id);
                        incorrectZonesId.Add(concurrentZone.Id);
                    }
                }
            }
        }

        private int _zoneLeft;
        public int ZoneLeft
        {
            get => _zoneLeft;
            set
            {
                _zoneLeft = value;
                var deviceZone = GetDeviceZone(SelectedProgram.Id, SelectedZone.Id);
                if (!deviceZone.IsNull())
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
                var deviceZone = GetDeviceZone(SelectedProgram.Id, SelectedZone.Id);
                if (!deviceZone.IsNull())
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
                var deviceZone = GetDeviceZone(SelectedProgram.Id, SelectedZone.Id);
                if (!deviceZone.IsNull())
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
                var deviceZone = GetDeviceZone(SelectedProgram.Id, SelectedZone.Id);
                if (!deviceZone.IsNull())
                {
                    deviceZone.Width = value;
                }
                OnPropertyChanged(nameof(ZoneWidth));
            }
        }

        private Font.FontFamily _selectedFont;
        public Font.FontFamily SelectedFont
        {
            get => _selectedFont;
            set
            {
                _selectedFont = value;
                if (!_selectedFont.IsNull() && SelectedFontSize != 0)
                {
                    UpdateZoneFont(SelectedProgram, SelectedZone, value, SelectedFontSize, IsItalic, IsBold);
                }
                OnPropertyChanged(nameof(SelectedFont));
            }
        }

        private void UpdateZoneFont(Program program, Zone zone, Font.FontFamily newFont, int newFontSize, bool italic, bool bold)
        {
            var deviceZone = GetDeviceZone(program.Id, zone.Id);
            if (!deviceZone.IsFonted)
            {
                return;
            }
            var existBinaryFont = Device.Fonts.FirstOrDefault(bf => bf.Id == deviceZone.FontId);
            if (!existBinaryFont.IsNull())
            {
                var numberOfFontEntry = Device.Programs.Sum(s => s.Zones.Count(f => f.IsFonted && f.FontId == existBinaryFont.Id));
                if (numberOfFontEntry <= 1)
                {
                    Device.Fonts.Remove(existBinaryFont);
                }
            }
            var newBinaryFont = Device.Fonts
                .FirstOrDefault(bf => bf.Height == newFontSize && bf.Source == newFont.Source && bf.Italic == italic && bf.Bold == bold);
            if (newBinaryFont.IsNull())
            {
                var newBinaryFontId = Device.Fonts.Any() ? Device.Fonts.Max(f => f.Id) + 1 : 0;
                newBinaryFont = new BinaryFont
                {
                    Id = newBinaryFontId,
                    Source = newFont.Source,
                    Height = newFontSize,
                    Bold = bold,
                    Italic = italic,
                    Base64Bitmap = BitmapProcessor.GenerateBase64FontMono(Device.Alphabet, newFont, italic, bold, newFontSize)
                };
                Device.Fonts.Add(newBinaryFont);
            }
            deviceZone.FontId = newBinaryFont.Id;
            GetDeviceZone(program.Id, zone.Id).FontId = newBinaryFont.Id;
        }

        private Zone GetDeviceZone(int programId, int zoneId)
        {
            return Device?.Programs?.FirstOrDefault(s => s.Id == programId)?.Zones?.FirstOrDefault(z => z.Id == zoneId);
        }

        private int _selectedFontSize;
        public int SelectedFontSize
        {
            get => _selectedFontSize;
            set
            {
                _selectedFontSize = value;
                if (!SelectedFont.IsNull() && _selectedFontSize != 0)
                {
                    UpdateZoneFont(SelectedProgram, SelectedZone, SelectedFont, value, IsItalic, IsBold);
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
                if (!SelectedFont.IsNull() && _selectedFontSize != 0)
                {
                    UpdateZoneFont(SelectedProgram, SelectedZone, SelectedFont, SelectedFontSize, IsItalic, value);
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
                if (!SelectedFont.IsNull() && _selectedFontSize != 0)
                {
                    UpdateZoneFont(SelectedProgram, SelectedZone, SelectedFont, SelectedFontSize, value, IsBold);
                }
                OnPropertyChanged(nameof(IsItalic));
                OnPropertyChanged(nameof(FontStyle));
            }
        }

        public System.Windows.FontStyle FontStyle => IsItalic ? FontStyles.Italic : FontStyles.Normal;

        public bool AllowChangeBoardSize { get; set; }

        public bool AllowZoneCoordinates => !SelectedZone.IsNull();

        public bool AllowFont { get; set; }
        public bool AllowTextEditing { get; set; }

        private void AllowAnimation(bool value)
        {
            AnimationVisibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        public Visibility AnimationVisibility { get; set; }

        private void AllowBitmap(bool value)
        {
            BitmapVisibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        public Visibility BitmapVisibility { get; set; }

        private void AllowExternalTag(bool value)
        {
            ExternalTagVisibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        public Visibility ExternalTagVisibility { get; set; }

        public Visibility TextVisibility { get; set; }
        public void AllowText(bool value)
        {
            TextVisibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        public Visibility ClockVisibility { get; set; }
        public void AllowClock(bool value)
        {
            ClockVisibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        private int _deviceHeight;
        public int DeviceHeight
        {
            get => _deviceHeight;
            set
            {
                _deviceHeight = value;
                Device.BoardSize.Height = value;
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
                Device.BoardSize.Width = value;
                OnPropertyChanged(nameof(DeviceWidth));
            }
        }

        private Program _selectedProgram;
        public Program SelectedProgram
        {
            get => _selectedProgram;
            set
            {
                _selectedProgram = value;
                Zones.Clear();
                if (!_selectedProgram.IsNull())
                {
                    _selectedProgram.Zones.ForEach(z => Zones.Add(z));
                }
                SelectedZone = null;
                OnPropertyChanged(nameof(SelectedProgram));
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
                if (!_selectedZone.IsNull())
                {
                    ZoneLeft = _selectedZone.X;
                    ZoneTop = _selectedZone.Y;
                    ZoneHeight = _selectedZone.Height;
                    ZoneWidth = _selectedZone.Width;
                }
                OnPropertyChanged(nameof(SelectedZone));
                OnPropertyChanged(nameof(AllowZoneCoordinates));
                var currentZoneType = ZoneTypes.FirstOrDefault(z => z.ZoneCondition(_selectedZone));
                if (currentZoneType.IsNull() || SelectedZoneType.IsNull() || SelectedZoneType.Id != currentZoneType.Id)
                {
                    SelectedZoneType = currentZoneType;
                    AllowAnimation(currentZoneType?.AllowAnimation ?? false);
                    AllowBitmap(currentZoneType?.AllowBitmap ?? false);
                    AllowFont = currentZoneType?.AllowFont ?? false;
                    AllowExternalTag(currentZoneType?.AllowMQTT ?? false);
                    AllowText(currentZoneType?.AllowText ?? false);
                    AllowTextEditing = currentZoneType?.AllowTextEditing ?? false;
                    AllowClock(currentZoneType?.AllowClock ?? false);
                    OnPropertyChanged(nameof(SelectedZoneType));
                }
                if (!_selectedZone.IsNull() && _selectedZone.IsFonted)
                {
                    var fontId = _selectedZone.FontId;
                    var binaryFont = Device.Fonts.FirstOrDefault(bf => bf.Id == fontId);
                    if (binaryFont.IsNull())
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
                    if (_selectedZone.ZoneType == (int)DomainObjects.ZoneTypes.Clock)
                    {
                        SelectedClockType = _clockTypes.FirstOrDefault(ct => ct.Id == _selectedZone.ClockType);
                        SelectedClockFormat = _clockFormats.FirstOrDefault(cf => cf.Id == _selectedZone.ClockFormat);
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
                var ticker = GetDeviceZone(SelectedProgram.Id, SelectedZone.Id);
                if (ticker.ZoneType == (int)DomainObjects.ZoneTypes.Text)
                {
                    ticker.Text = value;
                }
                OnPropertyChanged(nameof(Text));
            }
        }

        private ClockType _selectedClockType;
        public ClockType SelectedClockType
        {
            get => _selectedClockType;
            set
            {
                _selectedClockType = value;
                var zone = GetDeviceZone(SelectedProgram.Id, SelectedZone.Id);
                if (zone.ZoneType == (int)DomainObjects.ZoneTypes.Clock)
                {
                    zone.ClockType = value?.Id ?? 0;
                }
                OnPropertyChanged(nameof(SelectedClockType));
                AllowClockFormat = value?.AllowFormat ?? false;
                AllowText(value?.AllowFormat ?? false);
            }
        }

        private bool _allowClockFormat;
        public bool AllowClockFormat
        {
            get
            {
                return _allowClockFormat;
            }
            set
            {
                _allowClockFormat = value;
                AllowText(true);
                AllowFont = value;
                OnPropertyChanged(nameof(AllowClockFormat));
            }
        }

        private ClockFormat _selectedClockFormat;
        public ClockFormat SelectedClockFormat
        {
            get => _selectedClockFormat;
            set
            {
                _selectedClockFormat = value;
                var zone = GetDeviceZone(SelectedProgram.Id, SelectedZone.Id);
                if (zone.ZoneType == (int)DomainObjects.ZoneTypes.Clock)
                {
                    zone.ClockFormat = value?.Id ?? 0;
                    zone.Text = value?.Sample ?? string.Empty;
                }
                OnPropertyChanged(nameof(SelectedClockFormat));
            }
        }

        private string _externalSourceTag;
        public string ExternalSourceTag
        {
            get => _externalSourceTag;
            set
            {
                _externalSourceTag = value;
                var zone = GetDeviceZone(SelectedProgram.Id, SelectedZone.Id);
                if (zone.ZoneType == (int)DomainObjects.ZoneTypes.MQTT)
                {
                    zone.ExternalSourceTag = value;
                }
                OnPropertyChanged(nameof(ExternalSourceTag));
            }
        }

        private ZoneType _selectedZoneType;
        public ZoneType SelectedZoneType
        {
            get => _selectedZoneType;
            set
            {
                _selectedZoneType = value;
                OnPropertyChanged(nameof(SelectedZoneType));

                if (SelectedZone.IsNull())
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
                    var devicePrograms = Device.Programs.FirstOrDefault(s => s.Id == SelectedProgram.Id);
                    var deviceProgramZones = new List<Zone>();
                    foreach (var zone in devicePrograms.Zones)
                    {
                        if (zone.Id == newZone.Id)
                        {
                            deviceProgramZones.Add(newZone);
                        }
                        else
                        {
                            deviceProgramZones.Add(zone);
                        }
                    }
                    devicePrograms.Zones = deviceProgramZones;
                }
            }
        }

        private DelegateCommand _loadBitmap;
        public Input.ICommand LoadBitmap
        {
            get
            {
                if (_loadBitmap.IsNull())
                {
                    _loadBitmap = new DelegateCommand((o) =>
                      {
                          var dialog = new OpenFileDialog
                          {
                              Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|All files (*.*)|*.*"
                          };
                          if (dialog.ShowDialog() != true)
                          {
                              return;
                          }
                          var image = Image.FromFile(dialog.FileName);
                          UpdateZoneImage(SelectedProgram, SelectedZone, image);
                      });
                }
                return _loadBitmap;
            }
        }

        private void UpdateZoneImage(Program program, Zone zone, Image image)
        {
            if (zone.Width == 0 && zone.Height == 0)
            {
                return;
            }
            var deviceZone = GetDeviceZone(program.Id, zone.Id);
            var bitmap = new Bitmap(image);
            var width = (bitmap.Width > zone.Width) ? zone.Width : bitmap.Width;
            var height = (bitmap.Height > zone.Height) ? zone.Height : bitmap.Height;
            var trimmedBitmap = bitmap.Clone(new Rectangle(0, 0, width, height), bitmap.PixelFormat);
            var base64String = BitmapProcessor.GenerateBase64ImageMono(new Bitmap(trimmedBitmap));
            deviceZone.BitmapHeight = height;
            deviceZone.BitmapBase64 = base64String;
            OnPropertyChanged("");
        }

        private DelegateCommand _addProgram;
        public Input.ICommand AddProgram
        {
            get
            {
                if (_addProgram.IsNull())
                {
                    _addProgram = new DelegateCommand((o) =>
                    {
                        var nextOrderValue = (Device.Programs.Any()) ? Device.Programs.Max(s => s.Order) + 1 : 1;
                        var nextId = (Device.Programs.Any()) ? Device.Programs.Max(x => x.Id) + 1 : 0;
                        var program = new Program
                        {
                            Order = nextOrderValue,
                            Id = nextId,
                            Name = $"Программа{nextOrderValue}",
                            Period = 10,
                            Zones = new List<Zone>()
                        };
                        Programs.Add(program);
                        Device.Programs.Add(program);
                    });
                }
                return _addProgram;
            }
        }

        private DelegateCommand _deleteProgram;
        public Input.ICommand DeleteProgram
        {
            get
            {
                if (_deleteProgram.IsNull())
                {
                    _deleteProgram = new DelegateCommand((o) =>
                    {
                        Device.Programs.Remove(Device.Programs.FirstOrDefault(s => s.Id == SelectedProgram.Id));
                        Programs.Remove(SelectedProgram);
                        SelectedProgram = Programs.FirstOrDefault();
                    });
                }
                return _deleteProgram;
            }
        }

        private DelegateCommand _addZone;
        public Input.ICommand AddZone
        {
            get
            {
                if (_addZone.IsNull())
                {
                    _addZone = new DelegateCommand((o) =>
                    {
                        var nextId = Programs.Max(s => s.Zones.Any() ? s.Zones.Max(z => z.Id) : 0) + 1;
                        var zone = new Zone
                        {
                            ZoneType = (int)DomainObjects.ZoneTypes.Text,
                            Id = nextId,
                            Name = "Текст"
                        };
                        SelectedProgram.Zones.Add(zone);
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
                if (_deleteZone.IsNull())
                {
                    _deleteZone = new DelegateCommand((o) =>
                    {
                        var zone = SelectedZone;
                        SelectedProgram.Zones.Remove(zone);
                        Zones.Remove(zone);
                        OnPropertyChanged(nameof(Zones));
                    });
                }
                return _deleteZone;
            }
        }

        private DelegateCommand _resetScale;
        public Input.ICommand ResetScale
        {
            get
            {
                if (_resetScale.IsNull())
                {
                    _resetScale = new DelegateCommand((o) =>
                    {
                        PreviewScale = 1;
                    });
                }
                return _resetScale;
            }
        }

        public ILogger Logger => _logger;

        private double _previewScale;
        public double PreviewScale
        {
            get
            {
                return _previewScale;
            }
            set
            {
                if (_previewScale != value)
                {
                    _previewScale = value;
                    PreviewScalePercents = Convert.ToInt32(value * 100);
                    OnPropertyChanged(nameof(PreviewScale));
                }
            }
        }

        private int _previewScalePercents;
        public int PreviewScalePercents
        {
            get
            {
                return _previewScalePercents;
            }
            set
            {
                if (_previewScalePercents != value)
                {
                    _previewScalePercents = value;
                    PreviewScale = (double)value / 100;
                    OnPropertyChanged(nameof(PreviewScalePercents));
                }
            }
        }


        public readonly double PreviewScaleMinRate;
        public readonly double PreviewScaleMaxRate;
    }
}
