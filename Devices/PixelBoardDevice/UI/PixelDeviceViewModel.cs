using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Abstractions;
using BaseDevice;
using PixelBoardDevice.UI.POCO;
using ServiceInterfaces;

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
                AllowText = true
            },
            new ZoneType
            {
                Id = 2,
                Name = "Сенсор",
                AllowAnimation=false,
                AllowBitmap=false,
                AllowFont=true,
                AllowText=false,
            },
            new ZoneType
            {
                Id = 3,
                Name = "Тэг MQTT",
                AllowAnimation=false,
                AllowBitmap=false,
                AllowFont=true,
                AllowText=false,
            },
        };
        public event EventHandler OnNeedRedraw;
        public PixelDeviceViewModel(Device d, ILogger l)
        {
            _device = (PixelBoard)d;
            _logger = l;
            ZoneTypes = new ObservableCollection<ZoneType>(_zoneTypes);
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

        public ObservableCollection<ZoneType> ZoneTypes { get; set; }
    }
}
