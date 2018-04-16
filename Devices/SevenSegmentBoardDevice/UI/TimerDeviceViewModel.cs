using Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using ServiceInterfaces;
using System.Collections.ObjectModel;
using KaLEDoscope.POCO.Timer;
using System.Globalization;
using Input = System.Windows.Input;
using System.Text.RegularExpressions;
using BaseDevice;
using CommandProcessing;
using SevenSegmentBoardDevice.UI.POCO;
using SevenSegmentBoardDevice.Commands;
using UiCommands;
using Extensions;

namespace SevenSegmentBoardDevice.UI
{
    public class TimerDeviceViewModel : Notified
    {
        private readonly SevenSegmentBoard _device;
        private readonly ILogger _logger;
        private readonly Invoker _invoker;

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
                Name = "Встроенный источник точного времени",
                AllowTimezones = false,
            },
            new SyncSource
            {
                Id = 2,
                IsCutomized = true,
                AllowTimezones = true,
                Name = "NTP сервер"
            },
            new SyncSource
            {
                Id = 3,
                IsCutomized = true,
                AllowTimezones = false,
                Name = "Альтернативный источник"
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
                _device.BoardType.TypeId = value?.Id ?? default(int);
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
                _device.BoardType.FontTypeId = value?.Id ?? default(int);
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
                _device.BoardType.DisplayFormatId = value?.Id ?? default(int);
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
                _device.StopWatchParameters.CountdownTypeId = value?.Id ?? default(int);
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
                if (TimeSpan.TryParseExact(value, @"hh\:mm", CultureInfo.CurrentCulture, out TimeSpan timeSpan))
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
                _device.TimeSyncParameters.SourceId = value?.Id ?? default(int);
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
                    Regex.IsMatch(value, @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$");
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

        public TimerDeviceViewModel(Device device, ILogger logger)
        {
            _device = (SevenSegmentBoard)device;
            _logger = logger;
            _invoker = new Invoker(_logger);
            DisplayTypes = new ObservableCollection<DisplayType>(_displayTypes);
            DisplayType = _displayTypes.FirstOrDefault(d => d.Id == _device.BoardType?.TypeId);
            FontTypes = new ObservableCollection<FontType>(_fontTypes);
            FontType = _fontTypes.FirstOrDefault(f => f.Id == _device.BoardType.FontTypeId);
            DisplayFrames = new ObservableCollection<DisplayFrame>(_displayFrames);
            DisplayFormats = new ObservableCollection<DisplayFormat>(_displayFormats);
            DisplayFormat = _displayFormats.FirstOrDefault(d => d.Id == _device.BoardType.DisplayFormatId);
            CountdownTypes = new ObservableCollection<CountdownType>(_countdownTypes);
            CountdownType = _countdownTypes.FirstOrDefault(c => c.Id == _device.StopWatchParameters.CountdownTypeId);
            CountdownStartValue = _device.StopWatchParameters.CountdownStartValue;
            SyncSources = new ObservableCollection<SyncSource>(_syncSources);
            SyncSource = _syncSources.FirstOrDefault(s => s.Id == _device.TimeSyncParameters.SourceId);
            TimeZones = new ObservableCollection<TimeZoneInfo>(_timeZones);
            TimeZone = _timeZones.FirstOrDefault(t => t.Id.Equals(_device.TimeSyncParameters.ZoneId));
            TimeSyncPeriodValue = _device.TimeSyncParameters.SyncPeriod;
            TimeSyncServerIp = _device.TimeSyncParameters.ServerAddress;
            TimeSyncServerPort = _device.TimeSyncParameters.ServerPort;
            AlarmSchedule = new ObservableCollection<Alarm>(_device.AlarmSchedule);
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

        private DelegateCommand _startTimer;
        public Input.ICommand StartTimer
        {
            get
            {
                if (_startTimer == null)
                {
                    _startTimer = new DelegateCommand((o) =>
                      {
                          var command = new DirectConnectStartTimer(_device, _logger);
                          _invoker.Invoke(command);
                      });
                }
                return _startTimer;
            }
        }

        private DelegateCommand _pauseTimer;
        public Input.ICommand PauseTimer
        {
            get
            {
                if (_pauseTimer == null)
                {
                    _pauseTimer = new DelegateCommand((o) =>
                    {
                        var command = new DirectConnectPauseTimer(_device, _logger);
                        _invoker.Invoke(command);
                    });
                }
                return _pauseTimer;
            }
        }

        private DelegateCommand _resetTimer;
        public Input.ICommand ResetTimer
        {
            get
            {
                if (_resetTimer == null)
                {
                    _resetTimer = new DelegateCommand((o) =>
                    {
                        var command = new DirectConnectResetTimer(_device, _logger);
                        _invoker.Invoke(command);
                    });
                }
                return _resetTimer;
            }
        }

        private DelegateCommand _stopResetTimer;
        public Input.ICommand StopResetTimer
        {
            get
            {
                if (_stopResetTimer == null)
                {
                    _stopResetTimer = new DelegateCommand((o) =>
                    {
                        var command = new DirectConnectStopResetTimer(_device, _logger);
                        _invoker.Invoke(command);
                    });
                }
                return _stopResetTimer;
            }
        }
    }
}
