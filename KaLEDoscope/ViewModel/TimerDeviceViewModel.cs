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

namespace KaLEDoscope.ViewModel
{
    public class TimerDeviceViewModel : INotifyPropertyChanged
    {
        private readonly TimerDevice _device;
        private readonly ILogger _logger;

        private readonly List<DisplayType> _displayTypes = new List<DisplayType>
        {
            new DisplayType
            {
                Id=1,
                Name="Цифровая",
                IsFontEnabled=false
            },
            new DisplayType
            {
                Id=2,
                Name="Пиксельная",
                IsFontEnabled=true
            }
        };

        private readonly List<FontType> _fontTypes = new List<FontType>
        {
            new FontType
            {
                Id=1,
                Name="0-Arial8Narrow"
            },
            new FontType
            {
                Id=2,
                Name="1-Другой"
            }
        };

        private readonly List<DisplayFormat> _displayFormats = new List<DisplayFormat>
        {
            new DisplayFormat
            {
                Id=1,
                Name="00:00"
            },
            new DisplayFormat
            {
                Id=2,
                Name="00:00:00"
            },
            new DisplayFormat
            {
                Id=3,
                Name="00:00:00.000"
            },

        };

        private readonly List<CountdownType> _countdownTypes = new List<CountdownType>
        {
            new CountdownType
            {
                Id=1,
                Name="Секундомер"
            },
            new CountdownType
            {
                Id=2,
                Name="Обратный отсчет"
            }
        };

        public ObservableCollection<DisplayType> DisplayTypes { get; set; }
        public ObservableCollection<FontType> FontTypes { get; set; }
        public ObservableCollection<DisplayFormat> DisplayFormats { get; set; }

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
                _device.BoardTypeId = value.Id;
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
                _device.FontTypeId = value.Id;
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
                _device.DisplayFormatId = value.Id;
                OnPropertyChanged(nameof(DisplayFormat));
            }
        }

        public TimerDeviceViewModel(Device device, ILogger logger)
        {
            _device = (TimerDevice)device;
            _logger = logger;
            DisplayTypes = new ObservableCollection<DisplayType>(_displayTypes);
            DisplayType = _displayTypes.First();
            FontTypes = new ObservableCollection<FontType>(_fontTypes);
            FontType = _fontTypes.First();
            DisplayFormats = new ObservableCollection<DisplayFormat>(_displayFormats);
            DisplayFormat = _displayFormats.First();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
