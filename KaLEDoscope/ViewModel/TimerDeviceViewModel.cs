using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainData;
using ServiceInterfaces;
using Timer;
using System.Collections.ObjectModel;
using KaLEDoscope.POCO.Timer;
using System.ComponentModel;
using System.Globalization;

namespace KaLEDoscope.ViewModel
{
    public class TimerDeviceViewModel : INotifyPropertyChanged
    {
        private readonly BoardClock _device;
        private readonly ILogger _logger;

        private readonly List<DisplayType> _displayTypes = new List<DisplayType>
        {
            new DisplayType
            {
                Id = 1,
                Name = "Цифровая",
                IsFontEnabled = false
            },
            new DisplayType
            {
                Id = 2,
                Name = "Пиксельная",
                IsFontEnabled = true
            }
        };

        private readonly List<FontType> _fontTypes = new List<FontType>
        {
            new FontType
            {
                Id = 1,
                Name = "0-Arial8Narrow"
            },
            new FontType
            {
                Id = 2,
                Name = "1-Другой"
            }
        };

        private readonly List<DisplayFormat> _displayFormats = new List<DisplayFormat>
        {
            new DisplayFormat
            {
                Id = 1,
                Name = "000 (3 сегмента)",
                Capacity = 3
            },
            new DisplayFormat
            {
                Id = 2,
                Name = "00:00 (4 сегмента)",
                Capacity = 4
            },
            new DisplayFormat
            {
                Id=3,
                Name="00:00:00 (6 сегментов)",
                Capacity = 6
            },
            new DisplayFormat
            {
                Id = 4,
                Name = "00:00:00.000 (9 сегментов)",
                Capacity = 9
            },
        };

        private readonly List<CountdownType> _countdownTypes = new List<CountdownType>
        {
            new CountdownType
            {
                Id = 1,
                Name = "Секундомер"
            },
            new CountdownType
            {
                Id = 2,
                Name = "Обратный отсчет"
            }
        };

        private readonly List<DisplayFrame> _displayFrames = new List<DisplayFrame>
        {
            new DisplayFrame
            {
                Id = 1,
                Name = "Текущее время",
                DisplayPeriod = 45,
                IsEnabled = true,
                IsChecked = true,
                CharLenght = 4
            },
            new DisplayFrame
            {
                Id = 2,
                Name = "Дата дд/мм/гг",
                DisplayPeriod = 35,
                IsEnabled = false,
                IsChecked = false,
                CharLenght = 6
            },
            new DisplayFrame
            {
                Id = 3,
                Name = "Дата дд/мм",
                DisplayPeriod = 35,
                IsEnabled = true,
                IsChecked = false,
                CharLenght = 4
            },
            new DisplayFrame
            {
                Id = 4,
                Name = "гггг",
                DisplayPeriod = 20,
                IsEnabled = true,
                IsChecked = false,
                CharLenght = 4
            },
            new DisplayFrame
            {
                Id = 5,
                Name = "Температура",
                DisplayPeriod = 25,
                IsEnabled = true,
                IsChecked = false,
                CharLenght = 2
            },
            new DisplayFrame
            {
                Id = 6,
                Name = "Влажность",
                DisplayPeriod = 25,
                IsEnabled = true,
                IsChecked = false,
                CharLenght = 3
            },
            new DisplayFrame
            {
                Id = 7,
                Name = "Давление",
                DisplayPeriod = 5,
                IsEnabled = true,
                IsChecked = false,
                CharLenght = 3
            },
            new DisplayFrame
            {
                Id = 8,
                Name = "Уровень радиации",
                DisplayPeriod = 5,
                IsEnabled = true,
                IsChecked = false,
                CharLenght = 4
            },
            new DisplayFrame
            {
                Id = 9,
                Name = "Датчик XXXXX",
                DisplayPeriod = 5,
                IsEnabled = false,
                IsChecked = false,
                CharLenght = 5
            }
        };

        private readonly List<SyncSource> _syncSources = new List<SyncSource>
        {
            new SyncSource
            {
                Id = 1,
                IsCutomized = false,
                Name = "Встроенный источник точного времени"
            },
            new SyncSource
            {
                Id = 2,
                IsCutomized = true,
                Name = "NTP сервер"
            }
        };

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
                _device.BoardTypeId = value?.Id ?? default(int);
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
                _device.FontTypeId = value?.Id ?? default(int);
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
                _device.DisplayFormatId = value?.Id ?? default(int);
                if (value == null)
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
                _device.CountdownTypeId = value?.Id ?? default(int);
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
                TimeSpan timeSpan;
                if (TimeSpan.TryParseExact(value, @"mm\:ss", CultureInfo.CurrentCulture, out timeSpan))
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

        private TimeSpan _countdownStartValue;
        public TimeSpan CountdownStartValue
        {
            get
            {
                return _device.CountdownStartValue;
            }
            set
            {
                if (_countdownStartValue == value)
                {
                    return;
                }
                _countdownStartValue = value;
                _device.CountdownStartValue = value;
                CountdownStart = value.ToString(@"mm\:ss");
                OnPropertyChanged(nameof(CountdownStartValue));
                OnPropertyChanged(nameof(CountdownStart));
            }
        }

        private string _timeSyncPeriod;
        public string TimeSyncPeriod
        {
            get
            {
                return _timeSyncPeriod;
            }
            set
            {
                if (_timeSyncPeriod?.Equals(value) ?? false)
                {
                    return;
                }
                TimeSpan timeSpan;
                if (TimeSpan.TryParseExact(value, @"hh\:mm", CultureInfo.CurrentCulture, out timeSpan))
                {
                    _timeSyncPeriod = value;
                    TimeSyncPeriodValue = timeSpan;
                }
                else
                {
                    _timeSyncPeriod = "12:00";
                    TimeSyncPeriodValue = new TimeSpan(12, 0, 0);
                }
                OnPropertyChanged(nameof(TimeSyncPeriod));
                OnPropertyChanged(nameof(TimeSyncPeriodValue));
            }
        }

        private TimeSpan _timeSyncPeriodValue;
        public TimeSpan TimeSyncPeriodValue
        {
            get
            {
                return _device.TimeSyncPeriod;
            }
            set
            {
                if (_timeSyncPeriodValue == value)
                {
                    return;
                }
                _timeSyncPeriodValue = value;
                _device.TimeSyncPeriod = value;
                TimeSyncPeriod = value.ToString(@"hh\:mm");
                OnPropertyChanged(nameof(TimeSyncPeriodValue));
                OnPropertyChanged(nameof(TimeSyncPeriod));
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
                _device.SyncSourceId = value?.Id ?? default(int);
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
                _device.TimeZoneId = value?.Id ?? string.Empty;
                OnPropertyChanged(nameof(TimeZone));
            }
        }

        private string _timeSyncServerIp;
        public string TimeSyncServerIp
        {
            get
            {
                return _timeSyncServerIp;
            }
            set
            {
                _timeSyncServerIp = value;
                _device.TimeSyncServerIp = value;
                OnPropertyChanged(nameof(TimeSyncServerIp));
            }
        }

        private int _timeSyncServerPort;
        public int TimeSyncServerPort
        {
            get
            {
                return _timeSyncServerPort;
            }
            set
            {
                _timeSyncServerPort = value;
                _device.TimeSyncServerPort = value;
                OnPropertyChanged(nameof(TimeSyncServerPort));
            }
        }

        public TimerDeviceViewModel(Device device, ILogger logger)
        {
            _device = (BoardClock)device;
            _logger = logger;
            DisplayTypes = new ObservableCollection<DisplayType>(_displayTypes);
            DisplayType = _displayTypes.FirstOrDefault(d => d.Id == _device.BoardTypeId);
            FontTypes = new ObservableCollection<FontType>(_fontTypes);
            FontType = _fontTypes.FirstOrDefault(f => f.Id == _device.FontTypeId);
            DisplayFrames = new ObservableCollection<DisplayFrame>(_displayFrames);
            DisplayFormats = new ObservableCollection<DisplayFormat>(_displayFormats);
            DisplayFormat = _displayFormats.FirstOrDefault(d => d.Id == _device.DisplayFormatId);
            CountdownTypes = new ObservableCollection<CountdownType>(_countdownTypes);
            CountdownType = _countdownTypes.FirstOrDefault(c => c.Id == _device.CountdownTypeId);
            CountdownStartValue = _device.CountdownStartValue;
            SyncSources = new ObservableCollection<SyncSource>(_syncSources);
            SyncSource = _syncSources.FirstOrDefault(s => s.Id == _device.SyncSourceId);
            TimeZones = new ObservableCollection<TimeZoneInfo>(_timeZones);
            TimeZone = _timeZones.FirstOrDefault(t => t.Id.Equals(_device.TimeZoneId));
            TimeSyncPeriodValue = _device.TimeSyncPeriod;
            TimeSyncServerIp = _device.TimeSyncServerIp;
            TimeSyncServerPort = _device.TimeSyncServerPort;
            AlarmSchedule = new ObservableCollection<Alarm>(_device.AlarmSchedule);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
