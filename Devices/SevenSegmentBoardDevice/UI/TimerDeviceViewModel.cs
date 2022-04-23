using BaseDevice;
using SmartTechnologiesM.Base;
using SmartTechnologiesM.Base.Extensions;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using UiCommands;
using Input = System.Windows.Input;

namespace SevenSegmentBoardDevice.UI
{
    public class TimerDeviceViewModel : Notified
    {
        private readonly SevenSegmentBoard _device;

        private readonly ReadOnlyCollection<TimeZoneInfo> _timeZones = TimeZoneInfo.GetSystemTimeZones();

        public ObservableCollection<DisplayType> DisplayTypes { get; set; }
        public ObservableCollection<FontType> FontTypes { get; set; }
        public ObservableCollection<DisplayFormat> DisplayFormats { get; set; }
        public ObservableCollection<CountdownType> CountdownTypes { get; set; }
        public ObservableCollection<DisplayFrame> DisplayFrames { get; set; }
        public ObservableCollection<SyncSource> SyncSources { get; set; }
        public ObservableCollection<TimeZoneInfo> TimeZones { get; set; }
        public ObservableCollection<Alarm> AlarmSchedule { get; set; }

        private DisplayType _displayType;
        public DisplayType DisplayType
        {
            get
            {
                return _displayType;
            }
            set
            {
                _displayType = value;
                _device.BoardType.DisplayType = value;
                OnPropertyChanged(nameof(DisplayType));
            }
        }

        private FontType _fontType;
        public FontType FontType
        {
            get
            {
                return _fontType;
            }
            set
            {
                _fontType = value;
                _device.BoardType.FontType = value;
                OnPropertyChanged(nameof(FontType));
            }
        }

        private DisplayFormat _displayFormat;
        public DisplayFormat DisplayFormat
        {
            get
            {
                return _displayFormat;
            }
            set
            {
                _displayFormat = value;
                _device.BoardType.DisplayFormat = value;
                if (value.IsNull())
                {
                    return;
                }
                foreach (var displayFrame in DisplayFrames)
                {
                    if (displayFrame.CharLenght > _displayFormat.Capacity)
                    {
                        displayFrame.IsEnabled = false;
                        displayFrame.IsChecked = false;
                    }
                    else
                    {
                        displayFrame.IsEnabled = true;
                    }
                }
                OnPropertyChanged(nameof(DisplayFormat));
            }
        }

        private CountdownType _countdownType;
        public CountdownType CountdownType
        {
            get
            {
                return _countdownType;
            }
            set
            {
                _countdownType = value;
                _device.StopWatchParameters.CountdownTypeId = value?.Id ?? default;
                OnPropertyChanged(nameof(CountdownType));
            }
        }

        private string _countdownStart;
        public string CountdownStart
        {
            get
            {
                return _countdownStart;
            }
            set
            {
                if (_countdownStart?.Equals(value) ?? false)
                {
                    return;
                }
                if (TimeSpan.TryParseExact(value, @"mm\:ss", CultureInfo.CurrentCulture, out TimeSpan timeSpan))
                {
                    _countdownStart = value;
                    CountdownStartValue = timeSpan;
                }
                else
                {
                    _countdownStart = "00:00";
                    CountdownStartValue = new TimeSpan(0, 0, 0);
                }
                OnPropertyChanged(nameof(CountdownStart));
                OnPropertyChanged(nameof(CountdownStartValue));
            }
        }

        public TimeSpan CountdownStartValue
        {
            get
            {
                return _device.StopWatchParameters.CountdownStartValue;
            }
            set
            {
                if (_device.StopWatchParameters.CountdownStartValue == value)
                {
                    return;
                }
                _device.StopWatchParameters.CountdownStartValue = value;
                CountdownStart = value.ToString(@"mm\:ss");
                OnPropertyChanged(nameof(CountdownStartValue));
                OnPropertyChanged(nameof(CountdownStart));
            }
        }

        private string _syncIntervalStr;
        public string SyncIntervalStr
        {
            get
            {
                return _syncIntervalStr;
            }
            set
            {
                if (int.TryParse(value, out int interval))
                {
                    _syncIntervalStr = value;
                }
                else
                {
                    _syncIntervalStr = "30";
                    interval = 30;
                }
                TimeSyncPeriodValue = new TimeSpan(0, interval, 0);
                OnPropertyChanged(nameof(SyncIntervalStr));
                OnPropertyChanged(nameof(TimeSyncPeriodValue));
            }
        }

        public TimeSpan TimeSyncPeriodValue
        {
            get
            {
                return _device.TimeSyncParameters.SyncPeriod;
            }
            set
            {
                if (_device.TimeSyncParameters.SyncPeriod == value)
                {
                    return;
                }
                _device.TimeSyncParameters.SyncPeriod = value;
                SyncIntervalStr = value.TotalMinutes.ToString();
                OnPropertyChanged(nameof(TimeSyncPeriodValue));
                OnPropertyChanged(nameof(SyncIntervalStr));
            }
        }

        private SyncSource _syncSource;
        public SyncSource SyncSource
        {
            get
            {
                return _syncSource;
            }
            set
            {
                _syncSource = value;
                _device.TimeSyncParameters.SourceId = value?.Id ?? default;
                OnPropertyChanged(nameof(SyncSource));
            }
        }

        private TimeZoneInfo _timeZone;
        public TimeZoneInfo TimeZone
        {
            get
            {
                return _timeZone;
            }
            set
            {
                _timeZone = value;
                _device.TimeSyncParameters.ZoneId = value?.Id ?? string.Empty;
                OnPropertyChanged(nameof(TimeZone));
            }
        }

        public string TimeSyncServerIp
        {
            get
            {
                return _device.TimeSyncParameters.ServerAddress;
            }
            set
            {
                _device.TimeSyncParameters.ServerAddress = value;
                _device.TimeSyncParameters.IsIpAddress =
                    Regex.IsMatch(value, @"^((25[0-5]|2[0-4,0x0-9]|[01]?[0-9,0x0-9]?)\.){3}(25[0-5]|2[0-4,0x0-9]|[01]?[0-9,0x0-9]?)$");
                OnPropertyChanged(nameof(TimeSyncServerIp));
            }
        }

        public int TimeSyncServerPort
        {
            get
            {
                return _device.TimeSyncParameters.ServerPort;
            }
            set
            {
                _device.TimeSyncParameters.ServerPort = value;
                OnPropertyChanged(nameof(TimeSyncServerPort));
            }
        }

        public Alarm SelectedAlarm { get; set; }

        public TimerDeviceViewModel(Device device)
        {
            _device = (SevenSegmentBoard)device;
            DisplayTypes = new ObservableCollection<DisplayType>(Refs.DisplayTypes);
            DisplayType = Refs.DisplayTypes.FirstOrDefault(d => d == _device.BoardType.DisplayType) ?? Refs.DisplayTypes.FirstOrDefault();
            FontTypes = new ObservableCollection<FontType>(Refs.FontTypes);
            FontType = Refs.FontTypes.FirstOrDefault(f => f == _device.BoardType?.FontType) ?? (DisplayType.IsFontEnabled ? Refs.FontTypes.FirstOrDefault() : null);
            DisplayFrames = new ObservableCollection<DisplayFrame>();
            DisplayFrames.CollectionChanged += DisplayFrames_CollectionChanged;
            Refs.DisplayFrames.ForEach(f =>
            {
                DisplayFrame displayFrame;
                var deviceFrame = _device.DisplayFrames.FirstOrDefault(df => df.Id == f.Id);
                if (!deviceFrame.IsNull())
                {
                    displayFrame = deviceFrame;
                }
                else
                {
                    displayFrame = f;
                }
                DisplayFrames.Add(displayFrame);
                displayFrame.PropertyChanged += ((s, e) => { OnPropertyChanged(nameof(DisplayFrames)); });
            });
            DisplayFormats = new ObservableCollection<DisplayFormat>(Refs.DisplayFormats);
            DisplayFormat = Refs.DisplayFormats.FirstOrDefault(d => d == _device.BoardType.DisplayFormat) ?? Refs.DisplayFormats.FirstOrDefault();
            CountdownTypes = new ObservableCollection<CountdownType>(Refs.CountdownTypes);
            CountdownType = Refs.CountdownTypes.FirstOrDefault(c => c.Id == _device.StopWatchParameters.CountdownTypeId) ?? Refs.CountdownTypes.FirstOrDefault();
            CountdownStartValue = _device.StopWatchParameters.CountdownStartValue;
            SyncSources = new ObservableCollection<SyncSource>(Refs.SyncSources);
            SyncSource = Refs.SyncSources.FirstOrDefault(s => s.Id == _device.TimeSyncParameters.SourceId) ?? Refs.SyncSources.FirstOrDefault();
            TimeZones = new ObservableCollection<TimeZoneInfo>(_timeZones);
            TimeZone = _timeZones.FirstOrDefault(t => t.Id.Equals(_device.TimeSyncParameters.ZoneId)) ?? _timeZones.FirstOrDefault();
            TimeSyncPeriodValue = _device.TimeSyncParameters.SyncPeriod;
            TimeSyncServerIp = _device.TimeSyncParameters.ServerAddress;
            TimeSyncServerPort = _device.TimeSyncParameters.ServerPort;
            AlarmSchedule = new ObservableCollection<Alarm>(_device.AlarmSchedule);
        }

        private void DisplayFrames_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs args)
        {
            if (args.OldItems != null)
                foreach (var oldItem in args.OldItems)
                    ((Notified)oldItem).PropertyChanged -= YourItem_PropertyChanged;

            if (args.NewItems != null)
                foreach (var newItem in args.NewItems)
                    ((Notified)newItem).PropertyChanged += YourItem_PropertyChanged;
        }

        private void YourItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _device.DisplayFrames = DisplayFrames.Where(f => f.IsChecked).ToList();
        }

        private DelegateCommand _addAlarm;
        public Input.ICommand AddAlarm
        {
            get
            {
                if (_addAlarm == null)
                {
                    _addAlarm = new DelegateCommand((o) =>
                    {
                        var alarm = new Alarm
                        {
                            IsActive = true,
                            Period = new TimeSpan(0, 0, 1),
                            StartTimeSpan = new TimeSpan(8, 0, 0)
                        };
                        AlarmSchedule.Add(alarm);
                        _device.AlarmSchedule.Add(alarm);
                    });
                }
                return _addAlarm;
            }
        }

        private DelegateCommand _removeAlarm;
        public Input.ICommand RemoveAlarm
        {
            get
            {
                if (_removeAlarm == null)
                {
                    _removeAlarm = new DelegateCommand((o) =>
                    {
                        AlarmSchedule.Remove(SelectedAlarm);
                        SelectedAlarm = null;
                    });
                }
                return _removeAlarm;
            }
        }
    }
}
