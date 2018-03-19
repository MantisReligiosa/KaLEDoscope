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
using System.Runtime.InteropServices.ComTypes;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Drawing.Imaging;
using System.Net.Http;
using System.Collections;

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

        private Font.FontFamily _selectedFont;
        public Font.FontFamily SelectedFont
        {
            get
            {
                return _selectedFont;
            }
            set
            {
                _selectedFont = value;
                if (_selectedFont != null && SelectedFontSize != 0)
                {
                    UpdateZoneFont(SelectedScreen, SelectedZone, value, SelectedFontSize);
                }
                OnPropertyChanged(nameof(SelectedFont));
            }
        }

        private void UpdateZoneFont(Screen screen, Zone zone, Font.FontFamily newFont, int newFontSize)
        {
            var deviceZone = _device.Screens.FirstOrDefault(s => s.Id == screen.Id).Zones.FirstOrDefault(z => z.Id == zone.Id) as IFonted;
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
            var newBinaryFont = _device.Fonts.FirstOrDefault(bf => bf.Height == newFontSize && bf.Source == newFont.Source);
            if (newBinaryFont == null)
            {
                var newBinaryFontId = _device.Fonts.Any() ? _device.Fonts.Max(f => f.Id) + 1 : 0;
                newBinaryFont = new BinaryFont
                {
                    Height = newFontSize,
                    Source = newFont.Source,
                    Id = newBinaryFontId,
                    Base64Bitmap = GenerateBase64FontMono(newFont, newFontSize)
                };
            }
            deviceZone.FontId = newBinaryFont.Id;
        }

        private string GenerateBase64FontMono(Font.FontFamily newFont, int newFontSize)
        {
            var abc = _device.Alphabet;
            List<bool[,]> bitmapChars = new List<bool[,]>();
            foreach (var c in abc)
            {
                var font = new System.Drawing.Font(newFont.Source, (float)newFontSize, GraphicsUnit.Pixel);
                var image = DrawTextImage(c.ToString(), font, Color.White, Color.Black, Size.Empty) as Bitmap;
                var trimmedImage = image.Clone(new Rectangle(0, image.Height - newFontSize, image.Width, newFontSize), image.PixelFormat);
                var bytes = BitmapToBoolMono(trimmedImage);
                bitmapChars.Add(bytes);
            }
            var separatorLength = newFontSize * 3;
            var separator = new bool[newFontSize * 3];
            var indicator = 1;
            for (int i = 0; i < separatorLength; i++)
            {
                separator[i] = (indicator > 0);
                if (indicator == 2)
                {
                    indicator = 0;
                }
                else
                {
                    indicator++;
                }
            }
            var bitList = new List<bool>();
            foreach (var bitmapChar in bitmapChars)
            {
                var rows = bitmapChar.GetLength(0);
                var columns = bitmapChar.GetLength(1);
                for (int column = columns - 1; column >= 0; column--)
                {
                    for (int row = 0; row < rows; row++)
                    {
                        var value = bitmapChar[row, column];
                        bitList.Add(value);
                    }
                }
                bitList.AddRange(separator);
            }
            var needBytesForOctet = bitList.Count % 8;
            for (int i = 0; i < needBytesForOctet; i++)
            {
                bitList.Add(false);
            }
            var bitArray = new BitArray(bitList.ToArray());
            var bytesTotal = bitList.Count / 8;
            var resultBytes = new byte[bytesTotal];
            bitArray.CopyTo(resultBytes, 0);
            var base64String = Convert.ToBase64String(resultBytes);
            return base64String;
        }

        private unsafe static byte[,,] BitmapToByteRgb(Bitmap bmp)
        {
            int width = bmp.Width,
                height = bmp.Height;
            byte[,,] res = new byte[3, height, width];
            byte[,] mono = new byte[height, width];
            BitmapData bd = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly,
                PixelFormat.Format24bppRgb);
            try
            {
                byte* curpos;
                fixed (byte* _res = res)
                fixed (byte* _mono = mono)
                {
                    byte* _r = _res,
                          _g = _res + width * height,
                          _b = _res + 2 * width * height,
                          _m = _mono;
                    for (int h = 0; h < height; h++)
                    {
                        curpos = ((byte*)bd.Scan0) + h * bd.Stride;
                        for (int w = 0; w < width; w++)
                        {
                            *_b = *(curpos++);
                            *_g = *(curpos++);
                            *_r = *(curpos++);
                            *_m = (Byte)(0.3f * (float)(*_r) + 0.59f * (float)(*_g) + 0.11f * (float)(*_b));
                            ++_b;
                            ++_g;
                            ++_r;
                            ++_m;
                        }
                    }
                }
            }
            finally
            {
                bmp.UnlockBits(bd);
            }
            return res;
        }

        private unsafe static byte[,] BitmapToByteGrayscale(Bitmap bmp)
        {
            int width = bmp.Width,
                height = bmp.Height;
            byte[,,] res = new byte[3, height, width];
            byte[,] gray = new byte[height, width];
            BitmapData bd = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly,
                PixelFormat.Format24bppRgb);
            try
            {
                byte* curpos;
                fixed (byte* _res = res)
                fixed (byte* _gray = gray)
                {
                    byte* _r = _res,
                          _g = _res + width * height,
                          _b = _res + 2 * width * height,
                          _m = _gray;
                    for (int h = 0; h < height; h++)
                    {
                        curpos = ((byte*)bd.Scan0) + h * bd.Stride;
                        for (int w = 0; w < width; w++)
                        {
                            *_b = *(curpos++);
                            *_g = *(curpos++);
                            *_r = *(curpos++);
                            *_m = (Byte)(0.3f * (float)(*_r) + 0.59f * (float)(*_g) + 0.11f * (float)(*_b));
                            ++_b;
                            ++_g;
                            ++_r;
                            ++_m;
                        }
                    }
                }
            }
            finally
            {
                bmp.UnlockBits(bd);
            }
            return gray;
        }

        private unsafe static bool[,] BitmapToBoolMono(Bitmap bmp)
        {
            int width = bmp.Width,
                height = bmp.Height;
            byte[,,] res = new byte[3, height, width];
            bool[,] mono = new bool[height, width];
            BitmapData bd = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly,
                PixelFormat.Format24bppRgb);
            try
            {
                byte* curpos;
                fixed (byte* _res = res)
                fixed (bool* _mono = mono)
                {
                    byte* _r = _res,
                          _g = _res + width * height,
                          _b = _res + 2 * width * height;
                    bool* _m = _mono;
                    for (int h = 0; h < height; h++)
                    {
                        curpos = ((byte*)bd.Scan0) + h * bd.Stride;
                        for (int w = 0; w < width; w++)
                        {
                            *_b = *(curpos++);
                            *_g = *(curpos++);
                            *_r = *(curpos++);
                            *_m = (Byte)(0.3f * (float)(*_r) + 0.59f * (float)(*_g) + 0.11f * (float)(*_b)) > 128;
                            ++_b;
                            ++_g;
                            ++_r;
                            ++_m;
                        }
                    }
                }
            }
            finally
            {
                bmp.UnlockBits(bd);
            }
            return mono;
        }

        private Image DrawTextImage(String currencyCode, System.Drawing.Font font, Color textColor, Color backColor, Size minSize)
        {
            //first, create a dummy bitmap just to get a graphics object
            SizeF textSize;
            using (Image img = new Bitmap(1, 1))
            {
                using (Graphics drawing = Graphics.FromImage(img))
                {
                    //measure the string to see how big the image needs to be
                    textSize = drawing.MeasureString(currencyCode, font);
                    if (!minSize.IsEmpty)
                    {
                        textSize.Width = textSize.Width > minSize.Width ? textSize.Width : minSize.Width;
                        textSize.Height = textSize.Height > minSize.Height ? textSize.Height : minSize.Height;
                    }
                }
            }
            Image retImg = new Bitmap((int)textSize.Width, (int)textSize.Height);
            using (var drawing = Graphics.FromImage(retImg))
            {
                drawing.Clear(backColor);
                using (Brush textBrush = new SolidBrush(textColor))
                {
                    drawing.DrawString(currencyCode, font, textBrush, 0, 0);
                    drawing.Save();
                }
            }
            return retImg;
        }

        private int _selectedFontSize;
        public int SelectedFontSize
        {
            get
            {
                return _selectedFontSize;
            }
            set
            {
                _selectedFontSize = value;
                if (SelectedFont != null && _selectedFontSize != 0)
                {
                    UpdateZoneFont(SelectedScreen, SelectedZone, SelectedFont, value);
                }
                OnPropertyChanged(nameof(SelectedFontSize));
            }
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

        private bool _allowFont;
        public bool AllowFont
        {
            get
            {
                return _allowFont;
            }
            set
            {
                _allowFont = value;
                OnPropertyChanged(nameof(AllowFont));
            }
        }

        private bool _allowAnimation;
        public bool AllowAnimation
        {
            get
            {
                return _allowAnimation;
            }
            set
            {
                _allowAnimation = value;
                OnPropertyChanged(nameof(AllowAnimation));
            }
        }

        private bool _allowBitmap;
        public bool AllowBitmap
        {
            get
            {
                return _allowBitmap;
            }
            set
            {
                _allowBitmap = value;
                OnPropertyChanged(nameof(AllowBitmap));
            }
        }

        private bool _allowMQTT;
        public bool AllowMQTT
        {
            get
            {
                return _allowMQTT;
            }
            set
            {
                _allowMQTT = value;
                OnPropertyChanged(nameof(AllowMQTT));
            }
        }

        private bool _allowText;
        public bool AllowText
        {
            get
            {
                return _allowText;
            }
            set
            {
                _allowText = value;
                OnPropertyChanged(nameof(AllowText));
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
                    }
                }
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
    }
}
