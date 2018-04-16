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
                AllowFormat = true,
                Renderer = ( g,zone,binaryFont) =>
                {
                    var selectedClockFormat = _clockFormats.FirstOrDefault(cf=>cf.Id == zone.ClockFormat);
                    if (selectedClockFormat.IsNull())
                    {
                        return;
                    }
                    RenderText(g, binaryFont, selectedClockFormat.Sample, zone.X, zone.Y, zone.Width, zone.Height);
                }
            },
            new ClockType
            {
                Id = 2,
                Name = "Графический",
                AllowFormat = false,
                Renderer = ( g,zone,binaryFont) =>
                {
                    var diameter = ((zone.Width < zone.Height) ? zone.Width : zone.Height) - 3;
                    g.DrawEllipse(new Pen(Color.Red), new RectangleF(zone.X + 1, zone.Y + 1, diameter, diameter));
                    g.DrawLine(new Pen(Color.Red),zone.X+1+diameter/2,zone.Y+1+diameter/2,zone.X+1+diameter/2,zone.Y+1);
                    g.DrawLine(new Pen(Color.Red),zone.X+1+diameter/2,zone.Y+1+diameter/2,zone.X+1+diameter,zone.Y+1+diameter/2);
                }
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
            _device = (PixelBoard)d;
            _logger = l;
            ZoneTypes = new ObservableCollection<ZoneType>(_zoneTypes);
            Programs = new ObservableCollection<Program>(_device.Programs);
            Fonts = new ObservableCollection<Font.FontFamily>(new InstalledFontCollection().Families.Select(f => new Font.FontFamily(f.Name)));
            Zones = new ObservableCollection<Zone>();
            ClockFormats = new ObservableCollection<ClockFormat>(_clockFormats);
            ClockTypes = new ObservableCollection<ClockType>(_clockTypes);
            Zones.CollectionChanged += (s, e) => ValidateAndInvokePreview();
            SelectedProgram = Programs.FirstOrDefault();
            FontSizes = new ObservableCollection<int>(_fontSizes);
            AllowChangeBoardSize = allowChangeBoardSize;
            DeviceHeight = _device.BoardSize.Height;
            DeviceWidth = _device.BoardSize.Width;
            AllowChangeBoardSize = _device.IsStandaloneConfiguration;
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
                },
                {
                    (z) => z.ZoneType==(int)DomainObjects.ZoneTypes.Picture,
                    (zone,_device,g)=>
                    {
                        RenderBitmap(g,zone);
                    }
                },
                {
                    (z) => z.ZoneType==(int)DomainObjects.ZoneTypes.Clock,
                    (zone,_device,g)=>
                    {
                        var clockType=_clockTypes.FirstOrDefault(c=>c.Id==zone.ClockType);
                        clockType?.Renderer?.Invoke(g,zone,_device.Fonts.FirstOrDefault(f => f.Id == zone.FontId));
                    }
                }
            };

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

        private static void RenderText(Graphics g, BinaryFont binaryFont, string text, int x, int y, int width, int height)
        {
            if (binaryFont.IsNull())
            {
                return;
            }
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


        private void RedrawPreview()
        {

            /*
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
                                    var pen = new Pen(Color.Gray);
                                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                                    if (!zone.IsValid)
                                    {
                                        pen.Color = Color.Red;
                                    }
                                    if (zone.Id == (SelectedZone?.Id ?? int.MinValue))
                                    {
                                        pen.Color = Color.Yellow;
                                    }
                                    g.DrawRectangle(pen, zone.X, zone.Y, zone.Width - 1, zone.Height - 1);
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
                        */
        }

        private void ValidateZones()
        {
            if (Zones.IsNull())
                return;
            foreach (var zone in Zones)
            {
                zone.IsValid = true;
                foreach (var concurrentZone in Zones.Except(new List<Zone> { zone }))
                {
                    if (zone.IntersectWith(concurrentZone))
                    {
                        zone.IsValid = false;
                        break;
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

        //public object PreviewImage { get; set; }

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
            var existBinaryFont = _device.Fonts.FirstOrDefault(bf => bf.Id == deviceZone.FontId);
            if (!existBinaryFont.IsNull())
            {
                var numberOfFontEntry = _device.Programs.Sum(s => s.Zones.Count(f => f.IsFonted && f.FontId == existBinaryFont.Id));
                if (numberOfFontEntry <= 1)
                {
                    _device.Fonts.Remove(existBinaryFont);
                }
            }
            var newBinaryFont = _device.Fonts
                .FirstOrDefault(bf => bf.Height == newFontSize && bf.Source == newFont.Source && bf.Italic == italic && bf.Bold == bold);
            if (newBinaryFont.IsNull())
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
            GetDeviceZone(program.Id, zone.Id).FontId = newBinaryFont.Id;
        }

        private Zone GetDeviceZone(int programId, int zoneId)
        {
            return _device?.Programs?.FirstOrDefault(s => s.Id == programId)?.Zones?.FirstOrDefault(z => z.Id == zoneId);
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
                    var binaryFont = _device.Fonts.FirstOrDefault(bf => bf.Id == fontId);
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
                    var devicePrograms = _device.Programs.FirstOrDefault(s => s.Id == SelectedProgram.Id);
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
                        var nextOrderValue = (_device.Programs.Any()) ? _device.Programs.Max(s => s.Order) + 1 : 1;
                        var nextId = (_device.Programs.Any()) ? _device.Programs.Max(x => x.Id) + 1 : 0;
                        var program = new Program
                        {
                            Order = nextOrderValue,
                            Id = nextId,
                            Name = $"Программа{nextOrderValue}",
                            Period = 10,
                            Zones = new List<Zone>()
                        };
                        Programs.Add(program);
                        _device.Programs.Add(program);
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
                        _device.Programs.Remove(_device.Programs.FirstOrDefault(s => s.Id == SelectedProgram.Id));
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

        public ILogger Logger => _logger;
    }
}
