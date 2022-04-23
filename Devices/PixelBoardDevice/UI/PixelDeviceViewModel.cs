using BitmapProcessing;
using Microsoft.Win32;
using PixelBoardDevice.BLL;
using PixelBoardDevice.DomainObjects;
using PixelBoardDevice.DomainObjects.Zones;
using PixelBoardDevice.Interfaces;
using PixelBoardDevice.POCO;
using ServiceInterfaces;
using SmartTechnologiesM.Base;
using SmartTechnologiesM.Base.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Windows;
using UiCommands;
using Font = System.Windows.Media;
using Input = System.Windows.Input;

namespace PixelBoardDevice.UI
{
#warning Нужен рефакторинг - нужно вынести в отдельные контролы настройки зон. И сделать им свои вьюмодели
    public class PixelDeviceViewModel : Notified
    {
        private readonly IDeviceController _deviceController;

        public ImmutableList<ZoneType> ZoneTypes { get; } = ImmutableList.Create(References.ZoneTypes);
        public ImmutableList<int> FontSizes { get; } = ImmutableList.Create(References.FontSizes);
        public ImmutableList<Font.FontFamily> Fonts { get; } = ImmutableList.Create(new InstalledFontCollection().Families.Select(f => new Font.FontFamily(f.Name)).ToArray());
        public ImmutableList<ClockFormat> ClockFormats { get; } = ImmutableList.Create(References.ClockFormats);
        public ImmutableList<ClockType> ClockTypes { get; } = ImmutableList.Create(References.ClockTypes);
        public ImmutableList<TickerType> TickerTypes { get; } = ImmutableList.Create(References.TickerTypes);
        public ImmutableList<AnimationType> AnimationTypes { get; } = ImmutableList.Create(References.AnimationTypes);
        public ImmutableList<POCO.BoardHardwareType> BoardHardwareTypes { get; } = ImmutableList.Create(References.BoardHardwareTypes);

        public ObservableCollection<Program> Programs { get; set; }
        public ObservableCollection<Zone> Zones { get; set; } = new ObservableCollection<Zone>();
        

        public event EventHandler ModelChanged;

        public PixelBoard Device => _deviceController.Device;
        public bool IsAnyZones => Zones.Any();
        public bool IsAnyPrograms => Programs.Any();
        public FontWeight FontWeight => IsBold ? FontWeights.Bold : FontWeights.Normal;
        public System.Windows.FontStyle FontStyle => IsItalic ? FontStyles.Italic : FontStyles.Normal;
        public bool AllowZoneCoordinates => !SelectedZone.IsNull();

        public readonly double PreviewScaleMinRate;
        public readonly double PreviewScaleMaxRate;

        public ILogger Logger { get; }
        public int TextEditWidth { get; set; }
        public int RectX { get; set; }
        public bool IsImageProcessingEnabled { get; set; } = false;
        public bool AllowChangeBoardSize { get; set; }
        public bool AllowTickerCountDown { get; set; }
        public Visibility AnimationVisibility { get; set; }
        public Visibility BitmapVisibility { get; set; }
        public Visibility ExternalTagVisibility { get; set; }
        public Visibility TextVisibility { get; set; }
        public Visibility TextEditingVisibility { get; set; }
        public Visibility ClockVisibility { get; set; }
        public Visibility TickerVisibility { get; set; }
        public Program PreviewedProgram { get; set; }

        public Rect ZoneRect
        {
            get
            {
                if (SelectedZone.IsNull())
                    return new Rect(0, 0, 0, 0);
                RectX = 0;
                if (AlignmentCenter)
                    RectX = (TextEditWidth - SelectedZone.Width) / 2;
                if (AlignmentRight)
                    RectX = TextEditWidth - SelectedZone.Width;
                OnPropertyChanged(nameof(RectX));
                return new Rect(new System.Windows.Size(SelectedZone.Width, SelectedZone.Height));
            }
        }

        public bool AlignmentLeft
        {
            get
            {
                return TextAlignment == TextAlignment.Left;
            }
            set
            {
                if (!value)
                    return;
                TextAlignment = TextAlignment.Left;
                OnPropertyChanged(nameof(AlignmentLeft));
                OnPropertyChanged(nameof(TextAlignment));
            }
        }

        public bool AlignmentCenter
        {
            get
            {
                return TextAlignment == TextAlignment.Center;
            }
            set
            {
                if (!value)
                    return;
                TextAlignment = TextAlignment.Center;
                OnPropertyChanged(nameof(AlignmentCenter));
                OnPropertyChanged(nameof(TextAlignment));
            }
        }

        public bool AlignmentRight
        {
            get
            {
                return TextAlignment == TextAlignment.Right;
            }
            set
            {
                if (!value)
                    return;
                TextAlignment = TextAlignment.Right;
                OnPropertyChanged(nameof(AlignmentRight));
                OnPropertyChanged(nameof(TextAlignment));
            }
        }

        private int _zoneLeft;
        public int ZoneLeft
        {
            get => _zoneLeft;
            set
            {
                _zoneLeft = value;
                if (!_selectedDeviceZone.IsNull())
                {
                    _selectedDeviceZone.X = (ushort)value;
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
                if (!_selectedDeviceZone.IsNull())
                {
                    _selectedDeviceZone.Y = (ushort)value;
                }
                OnPropertyChanged(nameof(ZoneTop));
            }
        }

        private bool? _allowPeriodicSync;
        public bool? AllowPeriodicSync
        {
            get => _allowPeriodicSync;
            set
            {
                if (_allowPeriodicSync == value)
                    return;
                _allowPeriodicSync = value;
                if (_selectedDeviceZone is ClockZone deviceZone)
                {
                    deviceZone.AllowPeriodicTimeSync = value.Value;
                }
                OnPropertyChanged(nameof(AllowPeriodicSync));
                if (value.Value)
                {
                    AllowScheduledSync = false;
                    AllowNoSync = false;
                }
            }
        }

        private int _PeriodSyncInterval;
        public int PeriodicSyncInterval
        {
            get => _PeriodSyncInterval;
            set
            {
                _PeriodSyncInterval = value;
                if (_selectedDeviceZone is ClockZone deviceZone)
                {
                    deviceZone.PeriodicSyncInterval = (ushort)value;
                }
                OnPropertyChanged(nameof(PeriodicSyncInterval));
            }
        }

        private bool? _allowScheduledSync;
        public bool? AllowScheduledSync
        {
            get => _allowScheduledSync;
            set
            {
                if (_allowScheduledSync == value)
                    return;
                _allowScheduledSync = value;
                if (_selectedDeviceZone is ClockZone deviceZone)
                {
                    deviceZone.AllowScheduledSync = value.Value;
                }
                OnPropertyChanged(nameof(AllowScheduledSync));
                if (value.Value)
                {
                    AllowPeriodicSync = false;
                    AllowNoSync = false;
                }
            }
        }

        private bool? _allowNoSync;
        public bool? AllowNoSync
        {
            get => _allowNoSync;
            set
            {
                if (_allowNoSync == value)
                    return;
                _allowNoSync = value;
                if ((_selectedDeviceZone is ClockZone deviceZone) && (value ?? true))
                {
                    deviceZone.AllowPeriodicTimeSync = false;
                    deviceZone.AllowScheduledSync = false;
                }
                OnPropertyChanged(nameof(AllowNoSync));
            }
        }

        private string _scheduledTimeSync;
        public string ScheduledTimeSync
        {
            get
            {
                return _scheduledTimeSync;
            }
            set
            {
                if (!TimeSpan.TryParse(value, out TimeSpan timeSpan))
                {
                    timeSpan = new TimeSpan(0, 0, 0);
                }
                _scheduledTimeSync = timeSpan.ToString(@"hh\:mm");
                if (_selectedDeviceZone is ClockZone deviceZone)
                {
                    deviceZone.ScheduledTimeSync = timeSpan;
                }
                OnPropertyChanged(nameof(ScheduledTimeSync));
            }
        }

        private string _tickerCountDownStartValue;
        public string TickerCountDownStartValue
        {
            get
            {
                return _tickerCountDownStartValue;
            }
            set
            {
                if (!TimeSpan.TryParse(value, out TimeSpan timeSpan))
                {
                    timeSpan = new TimeSpan(0, 0, 0);
                }
                _tickerCountDownStartValue = timeSpan.ToString(@"mm\:ss\.ffff");
                if (_selectedDeviceZone is TickerZone deviceZone)
                {
                    deviceZone.TickerCountDownStartValue = timeSpan;
                }
                OnPropertyChanged(nameof(TickerCountDownStartValue));
            }
        }

        private int _zoneHeight;
        public int ZoneHeight
        {
            get => _zoneHeight;
            set
            {
                _zoneHeight = value;
                if (!_selectedDeviceZone.IsNull())
                {
                    _selectedDeviceZone.Height = (ushort)value;
                }
                OnPropertyChanged(nameof(ZoneHeight));
                OnPropertyChanged(nameof(ZoneRect));
                ReformatText(true);
            }
        }

        private int _zoneWidth;
        public int ZoneWidth
        {
            get => _zoneWidth;
            set
            {
                _zoneWidth = value;
                if (!_selectedDeviceZone.IsNull())
                {
                    _selectedDeviceZone.Width = (ushort)value;
                }
                OnPropertyChanged(nameof(ZoneWidth));
                OnPropertyChanged(nameof(ZoneRect));
                ReformatText(true);
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
                    UpdateZoneFont(SelectedZone, value, (byte)SelectedFontSize, IsItalic, IsBold);
                }
                ReformatText(true);
                OnPropertyChanged(nameof(SelectedFont));
            }
        }

        private string _selectedFontSizeStr;
        public string SelectedFontSizeStr
        {
            get
            {
                return _selectedFontSizeStr;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(_selectedFontSizeStr) && _selectedFontSizeStr.Equals(value))
                    return;
                if (int.TryParse(value, out int fontSize))
                {
                    _selectedFontSizeStr = value;
                    SelectedFontSize = fontSize;
                    OnPropertyChanged(nameof(SelectedFontSizeStr));
                }
            }
        }

        private int _selectedFontSize;
        public int SelectedFontSize
        {
            get => _selectedFontSize;
            set
            {
                if (_selectedFontSize == value)
                    return;
                _selectedFontSize = value;
                SelectedFontSizeStr = value.ToString();
                if (!SelectedFont.IsNull() && _selectedFontSize != 0)
                {
                    UpdateZoneFont(SelectedZone, SelectedFont, (byte)value, IsItalic, IsBold);
                }
                ReformatText(true);
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
                    UpdateZoneFont(SelectedZone, SelectedFont, (byte)SelectedFontSize, IsItalic, value);
                }
                OnPropertyChanged(nameof(IsBold));
                OnPropertyChanged(nameof(FontWeight));
            }
        }

        private bool _isItalic;
        public bool IsItalic
        {
            get => _isItalic;
            set
            {
                _isItalic = value;
                if (!SelectedFont.IsNull() && _selectedFontSize != 0)
                {
                    UpdateZoneFont(SelectedZone, SelectedFont, (byte)SelectedFontSize, value, IsBold);
                }
                OnPropertyChanged(nameof(IsItalic));
                OnPropertyChanged(nameof(FontStyle));
            }
        }

        private TextAlignment _textAlignment;
        public TextAlignment TextAlignment
        {
            get
            {
                return _textAlignment;
            }
            set
            {
                if (_textAlignment == value)
                    return;
                _textAlignment = value;
                var zone = _selectedDeviceZone as IFontableZone;
                zone.Alignment = value;
                OnPropertyChanged(nameof(TextAlignment));
                OnPropertyChanged(nameof(AlignmentLeft));
                OnPropertyChanged(nameof(AlignmentCenter));
                OnPropertyChanged(nameof(AlignmentRight));
                OnPropertyChanged(nameof(ZoneRect));
            }
        }

        private AnimationType _animationType;
        public AnimationType AnimationType
        {
            get
            {
                return _animationType;
            }
            set
            {
                _animationType = value;
                if (_selectedDeviceZone is TextZone zone)
                {
                    zone.AnimationId = (byte)value.Id;
                    OnPropertyChanged(nameof(AnimationType));
                    OnPropertyChanged(nameof(AnimationEnabled));
                    OnPropertyChanged(nameof(AllowAnimation));
                    ReformatText();
                }
            }
        }

        public bool AnimationEnabled => !AnimationType?.NoAnimationType ?? false;

        private int _animationSpeed;
        public int AnimationSpeed
        {
            get
            {
                return _animationSpeed;
            }
            set
            {
                _animationSpeed = value;
                if (_selectedDeviceZone is TextZone zone)
                {
                    zone.AnimationSpeed = (byte)value;
                    OnPropertyChanged(nameof(AnimationSpeed));
                }
            }
        }

        private int _animationTimeout;
        public int AnimationTimeout
        {
            get
            {
                return _animationTimeout;
            }
            set
            {
                _animationTimeout = value;
                if (_selectedDeviceZone is TextZone zone)
                {
                    zone.AnimationTimeout = (byte)value;
                    OnPropertyChanged(nameof(AnimationTimeout));
                }
            }
        }

        private int _deviceHeight;
        public int DeviceHeight
        {
            get => _deviceHeight;
            set
            {
                _deviceHeight = value;
                _deviceController.Device.BoardSize.Height = (ushort)value;
                OnPropertyChanged(nameof(DeviceHeight));
            }
        }

        private POCO.BoardHardwareType _boardHardwareType;
        public POCO.BoardHardwareType BoardHardwareType
        {
            get => _boardHardwareType;
            set
            {
                _boardHardwareType = value;
                _deviceController.Device.Hardware.Type = (DomainObjects.BoardHardwareType)(value.Id);
                OnPropertyChanged(nameof(BoardHardwareType));
            }
        }

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

        private int _deviceWidth;
        public int DeviceWidth
        {
            get => _deviceWidth;
            set
            {
                _deviceWidth = value;
                _deviceController.Device.BoardSize.Width = (ushort)value;
                OnPropertyChanged(nameof(DeviceWidth));
            }
        }

        private string _prevText;
        private string _text;
        public string Text
        {
            get => _text;
            set
            {
                if (_text?.Equals(value) ?? false)
                    return;
                _prevText = _text;
                _text = value;
                if (_selectedDeviceZone is TextZone textZone)
                {
                    textZone.Text = value;
                    UpdateZoneFont(SelectedZone, SelectedFont, (byte)SelectedFontSize, IsItalic, IsBold);
                }
                ReformatText();
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
                if (_selectedDeviceZone is ClockZone clockZone)
                {
                    clockZone.ClockType = value?.Type ?? 0;
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
                if (_selectedDeviceZone is ClockZone clockZone)
                {
                    clockZone.ClockFormat = value?.Id ?? 0;
                    clockZone.Sample = value?.Sample ?? string.Empty;
                }
                OnPropertyChanged(nameof(SelectedClockFormat));
            }
        }

        private TickerType _selectedTickerType;
        public TickerType SelectedTickerType
        {
            get => _selectedTickerType;
            set
            {
                _selectedTickerType = value;
                if (_selectedDeviceZone is TickerZone tickerZone)
                {
                    tickerZone.TickerType = value?.Id ?? 0;
                    AllowTickerCountDown = value?.AllowStartValue ?? false;
                }
                OnPropertyChanged(nameof(SelectedTickerType));
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

        private Zone _selectedDeviceZone;
        private Zone _selectedZone;
        public Zone SelectedZone
        {
            get => _selectedZone;
            set
            {
                _selectedZone = value;
                _selectedDeviceZone = _deviceController.BindZone(value);
                if (_selectedZone.IsNull())
                {
                    OnPropertyChanged(nameof(AllowZoneCoordinates));
                    ShowAllowedTunes(null);
                    SelectedZoneType = null;
                    return;
                }
                ZoneLeft = _selectedZone.X;
                ZoneTop = _selectedZone.Y;
                ZoneHeight = _selectedZone.Height;
                ZoneWidth = _selectedZone.Width;
                OnPropertyChanged(nameof(SelectedZone));
                OnPropertyChanged(nameof(AllowZoneCoordinates));
                var currentZoneType = ZoneTypes.FirstOrDefault(z => z.ZoneCondition(_selectedZone));
                if (currentZoneType.IsNull() || SelectedZoneType.IsNull() || SelectedZoneType.Id != currentZoneType.Id)
                {
                    SelectedZoneType = currentZoneType;
                    ShowAllowedTunes(currentZoneType);
                    OnPropertyChanged(nameof(SelectedZoneType));
                }
                if (_selectedZone is IFontableZone fontableZone)
                {
                    var fontId = fontableZone.FontId;
                    var binaryFont = _deviceController.Device.Fonts.FirstOrDefault(bf => bf.Id == fontId);
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
                    var alignment = fontableZone.Alignment ?? 0;
                    TextAlignment = alignment;
                }
                if (_selectedZone is TextZone textZone)
                {
                    Text = textZone.Text;
                    AnimationType = References.AnimationTypes.FirstOrDefault(t => t.Id == (textZone.AnimationId ?? 0));
                }
                else
                {
                    Text = string.Empty;
                }
                if (_selectedZone is ClockZone clockZone)
                {
                    SelectedClockType = References.ClockTypes.FirstOrDefault(ct => ct.Type == clockZone.ClockType);
                    SelectedClockFormat = References.ClockFormats.FirstOrDefault(cf => cf.Id == clockZone.ClockFormat);
                    AllowPeriodicSync = clockZone.AllowPeriodicTimeSync;
                    AllowScheduledSync = clockZone.AllowScheduledSync;
                    AllowNoSync = !(clockZone.AllowPeriodicTimeSync || clockZone.AllowScheduledSync);
                    PeriodicSyncInterval = clockZone.PeriodicSyncInterval;
                    ScheduledTimeSync = clockZone.ScheduledTimeSync.ToString(@"hh\:mm");
                }
                if (_selectedZone is TagZone tagZone)
                {
                    ExternalSourceTag = tagZone.ExternalSourceTag;
                }
                if (_selectedZone is TickerZone tickerZone)
                {
                    SelectedTickerType = References.TickerTypes.FirstOrDefault(tt => tt.Id == tickerZone.TickerType);
                    TickerCountDownStartValue = tickerZone.TickerCountDownStartValue.ToString(@"mm\:ss\.ffff");
                }

            }
        }

        private string _externalSourceTag;
        public string ExternalSourceTag
        {
            get => _externalSourceTag;
            set
            {
                _externalSourceTag = value;
                if (_selectedDeviceZone is TagZone tagZone)
                {
                    tagZone.ExternalSourceTag = value;
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
                ShowAllowedTunes(value);
                newZone.Id = SelectedZone.Id;
                newZone.X = SelectedZone.X;
                newZone.Y = SelectedZone.Y;
                newZone.ProgramId = SelectedZone.ProgramId;
                newZone.Width = SelectedZone.Width;
                newZone.Height = SelectedZone.Height;
                if (newZone is IFontableZone fontableZone)
                {
                    var selectedFontableZone = SelectedZone as IFontableZone;
                    fontableZone.FontId = selectedFontableZone?.FontId;
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
                    var devicePrograms = _deviceController.Device.Programs.FirstOrDefault(s => s.Id == SelectedProgram.Id);
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
                    _selectedDeviceZone = _deviceController.BindZone(newZone);
                }
            }
        }



        private DelegateCommand _invertBitmap;
        public Input.ICommand InvertBitmap
        {
            get
            {
                if (_invertBitmap.IsNull())
                {
                    _invertBitmap = new DelegateCommand((o) =>
                    {
                        var deviceZone = _selectedDeviceZone as BitmapZone;
                        var bitmapId = deviceZone.BinaryImageId;
                        var binaryImage = _deviceController.Device.BinaryImages.FirstOrDefault(b => b.Id == bitmapId);
                        var bytes = binaryImage.Bytes;
                        var invertedbase64 = BitmapProcessor.InvertByteImageMono(bytes);
                        binaryImage.Bytes = invertedbase64;
                        OnPropertyChanged("");
                    });
                }
                return _invertBitmap;
            }
        }

        private DelegateCommand _clearBitmap;
        public Input.ICommand ClearBitmap
        {
            get
            {
                if (_clearBitmap.IsNull())
                {
                    _clearBitmap = new DelegateCommand((o) =>
                    {
                        UpdateZoneImage(_selectedDeviceZone as BitmapZone, null);
                        IsImageProcessingEnabled = false;
                    });
                }
                return _clearBitmap;
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
                        IsImageProcessingEnabled = true;
                        var image = Image.FromFile(dialog.FileName);
                        UpdateZoneImage(_selectedDeviceZone as BitmapZone, image);
                    });
                }
                return _loadBitmap;
            }
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
                        var nextOrderValue = (byte)(_deviceController.Device.Programs.Any() ? _deviceController.Device.Programs.Max(s => s.Order) + 1 : 1);
                        var nextId = (byte)(_deviceController.Device.Programs.Any() ? _deviceController.Device.Programs.Max(x => x.Id) + 1 : 0);
                        var program = new Program
                        {
                            Order = nextOrderValue,
                            Id = nextId,
                            Name = $"Программа{nextOrderValue}",
                            Period = 10,
                            Zones = new List<Zone>()
                        };
                        if (!Programs.Any())
                        {
                            SelectedProgram = program;
                        }
                        Programs.Add(program);
                        _deviceController.Device.Programs.Add(program);
                        OnPropertyChanged(nameof(IsAnyPrograms));
                    });
                }
                return _addProgram;
            }
        }

        private DelegateCommand _downCommand;
        public Input.ICommand DownCommand
        {
            get
            {
                if (_downCommand.IsNull())
                {
                    _downCommand = new DelegateCommand((o) =>
                    {
                        var program = o as Program;
                        if (program.Order == Programs.Max(p => p.Order))
                            return;
                        var neighbourProgram = Programs.First(p => p.Order == program.Order + 1);
                        var programOrder = program.Order;
                        program.Order = neighbourProgram.Order;
                        neighbourProgram.Order = programOrder;
                        _deviceController.Device.Programs.Find(p => p.Id == program.Id).Order = program.Order;
                        _deviceController.Device.Programs.Find(p => p.Id == neighbourProgram.Id).Order = neighbourProgram.Order;
                        OnPropertyChanged(nameof(Programs));
                    });
                }
                return _downCommand;
            }
        }

        private DelegateCommand _upCommand;
        public Input.ICommand UpCommand
        {
            get
            {
                if (_upCommand.IsNull())
                {
                    _upCommand = new DelegateCommand((o) =>
                    {
                        var program = o as Program;
                        if (program.Order == Programs.Min(p => p.Order))
                            return;
                        var neighbourProgram = Programs.First(p => p.Order == program.Order - 1);
                        var programOrder = program.Order;
                        program.Order = neighbourProgram.Order;
                        neighbourProgram.Order = programOrder;
                        _deviceController.Device.Programs.Find(p => p.Id == program.Id).Order = program.Order;
                        _deviceController.Device.Programs.Find(p => p.Id == neighbourProgram.Id).Order = neighbourProgram.Order;
                        OnPropertyChanged(nameof(Programs));
                    });
                }
                return _upCommand;
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
                        _deviceController.Device.Programs.Remove(_deviceController.Device.Programs.FirstOrDefault(s => s.Id == SelectedProgram.Id));
                        Programs.Remove(SelectedProgram);
                        var order = 1;
                        foreach (var program in Programs.OrderBy(p => p.Order))
                        {
                            program.Order = (byte)order++;
                        }
                        order = 1;
                        foreach (var program in _deviceController.Device.Programs.OrderBy(p => p.Order))
                        {
                            program.Order = (byte)order++;
                        }
                        SelectedProgram = Programs.FirstOrDefault();
                        SelectedZone = SelectedProgram?.Zones.FirstOrDefault();
                        CleanupUnusedFonts();
                        CleanupUnusedImages();
                        OnPropertyChanged(nameof(IsAnyPrograms));
                        OnPropertyChanged(nameof(IsAnyZones));
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
                        byte nextId;
                        if (Programs.Any())
                        {
                            nextId = Programs.Max(s => s.Zones.Any() ? s.Zones.Max(z => z.Id) : (byte)0);
                        }
                        else
                        {
                            nextId = 0;
                        }
                        nextId++;
                        var zone = new TextZone
                        {
                            Id = nextId,
                            ProgramId = SelectedProgram.Id,
                            X = 0,
                            Y = 0,
                            Height = 10,
                            Width = 10,
                            Alignment = 0,
                            AnimationId = 0
                        };
                        SelectedProgram.Zones.Add(zone);
                        Zones.Add(zone);
                        SelectedZone = zone;
                        OnPropertyChanged(nameof(Zones));
                        OnPropertyChanged(nameof(IsAnyZones));
                        SelectedFont = Fonts.First();
                        SelectedFontSize = 10;
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
                        SelectedProgram?.Zones?.Remove(zone);
                        Zones.Remove(zone);
                        if (zone is IFontableZone fontableZone)
                        {
                            if (Programs.SelectMany(p => p.Zones).OfType<TextZone>().Count(tz => tz.FontId == fontableZone.FontId) <= 1)
                            {
                                var font = _deviceController.Device.Fonts.FirstOrDefault(f => f.Id == fontableZone.FontId);
                                if (font != null)
                                {
                                    _deviceController.Device.Fonts.Remove(font);
                                }
                            }
                            CleanupUnusedFonts();
                        }
                        if (zone is BitmapZone bitmapZone)
                        {
                            if (Programs.SelectMany(p => p.Zones).OfType<BitmapZone>().Count(bz => bz.BinaryImageId == bitmapZone.BinaryImageId) <= 1)
                            {
                                var image = _deviceController.Device.BinaryImages.FirstOrDefault(i => i.Id == bitmapZone.BinaryImageId);
                                if (image != null)
                                {
                                    _deviceController.Device.BinaryImages.Remove(image);
                                }
                            }
                            CleanupUnusedImages();
                        }
                        OnPropertyChanged(nameof(Zones));
                        OnPropertyChanged(nameof(IsAnyZones));
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

        public PixelDeviceViewModel(IDeviceController deviceController, ILogger logger, bool allowChangeBoardSize = true)
        {
            PropertyChanged += (s, e) => ValidateAndInvokePreview();
            _deviceController = deviceController;
            Logger = logger;
            Programs = new ObservableCollection<Program>(deviceController.Device.Programs);
            Zones.CollectionChanged += (s, e) => ValidateAndInvokePreview();
            SelectedProgram = Programs.FirstOrDefault();
            TextAlignment = TextAlignment.Left;
            AllowChangeBoardSize = allowChangeBoardSize;
            PreviewScale = 1;
            PreviewScaleMinRate = .2;
            PreviewScaleMaxRate = 5;
            DeviceHeight = deviceController.Device.BoardSize.Height;
            DeviceWidth = deviceController.Device.BoardSize.Width;
            BoardHardwareType = BoardHardwareTypes.FirstOrDefault(t => t.Id.Equals((int)(_deviceController.Device.Hardware?.Type ?? DomainObjects.BoardHardwareType.Hub12)));
            AllowAnimation(false);
            AllowBitmap(false);
            AllowExternalTag(false);
            AllowText(false);
            AllowClock(false);
            AllowTicker(false);
            CleanupUnusedFonts();
            CleanupUnusedImages();
            OnPropertyChanged(nameof(IsAnyPrograms));
        }

        private void ValidateAndInvokePreview()
        {
            _deviceController.ValidateZones(Zones);
            ModelChanged?.Invoke(this, EventArgs.Empty);
        }

        private void UpdateZoneFont(Zone zone, Font.FontFamily newFont, byte newFontSize, bool italic, bool bold)
        {
            var deviceZone = _selectedDeviceZone as IFontableZone;
            if (deviceZone.IsNull())
            {
                return;
            }
            if (newFont.IsNull())
                return;

            _deviceController.UpdateZoneFont(zone, newFont, newFontSize, italic, bold, out byte newBinaryFontId);
            CleanupUnusedFonts();
            deviceZone.FontId = newBinaryFontId;
        }

        private void CleanupUnusedFonts()
        {
            _deviceController.Device.CleanupUnusedFonts();
        }

        private void CleanupUnusedImages()
        {
            _deviceController.Device.CleanupUnusedImages();
        }

        private void AllowAnimation(bool value)
        {
            AnimationVisibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        private void AllowBitmap(bool value)
        {
            BitmapVisibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        private void AllowExternalTag(bool value)
        {
            ExternalTagVisibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        private void AllowText(bool value)
        {
            TextVisibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        private void AllowTextEditing(bool value)
        {
            TextEditingVisibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        private void AllowClock(bool value)
        {
            ClockVisibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        private void AllowTicker(bool value)
        {
            TickerVisibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ShowAllowedTunes(ZoneType currentZoneType)
        {
            AllowAnimation(currentZoneType?.AllowAnimation ?? false);
            AllowBitmap(currentZoneType?.AllowBitmap ?? false);
            AllowExternalTag(currentZoneType?.AllowMQTT ?? false);
            AllowText(currentZoneType?.AllowText ?? false);
            AllowTextEditing(currentZoneType?.AllowTextEditing ?? false);
            AllowClock(currentZoneType?.AllowClock ?? false);
            AllowTicker(currentZoneType?.AllowTicker ?? false);
        }

        private void UpdateZoneImage(BitmapZone zone, Image image)
        {
            if (image.IsNull())
            {
                var deletedBitmapImage = _deviceController.Device.BinaryImages.FirstOrDefault(i => i.Id == zone.BinaryImageId);
                _deviceController.Device.BinaryImages.Remove(deletedBitmapImage);
                zone.BinaryImageId = 0;
            }
            else
            {
                var bitmap = new Bitmap(image);
                var bytes = BitmapProcessor.GenerateByteImageMono(bitmap);
                var nextId = (byte)(_deviceController.Device.BinaryImages.Any() ? _deviceController.Device.BinaryImages.Max(x => x.Id) + 1 : 1);
                _deviceController.Device.BinaryImages.Add(new BinaryImage
                {
                    Id = nextId,
                    Bytes = bytes,
                    Height = (ushort)bitmap.Height,
                    Width = (ushort)bitmap.Width
                });
                zone.BinaryImageId = nextId;
            }
            CleanupUnusedImages();
            OnPropertyChanged("");
        }

        private void ReformatText(bool trimText = false)
        {
            var textZone = SelectedZone as TextZone;
            if (textZone.IsNull())
                return;

            var result = _deviceController.ReformatText(textZone, trimText);
            if (result.IsNull())
                return;
            if (AnimationEnabled)
                return;
            if (result.NeedToResize)
            {
                var maxHeight = DeviceHeight - textZone.Y;
                var neededHeight = (maxHeight > result.NeededHeight) ? result.NeededHeight : maxHeight;
                textZone.Height = (ushort)neededHeight;
                ZoneHeight = textZone.Height;
            }
            var text = result.NeedUndo ? _prevText : result.Text;
            textZone.Text = text;
            _text = text.Replace(Constants.LineSplitString, "\r\n");
            OnPropertyChanged(nameof(Text));
            OnPropertyChanged(nameof(ZoneRect));
        }
    }
}
