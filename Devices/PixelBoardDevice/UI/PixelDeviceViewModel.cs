using System;
using Abstractions;
using BaseDevice;
using ServiceInterfaces;

namespace PixelBoardDevice.UI
{
    public class PixelDeviceViewModel : Notified
    {
        private readonly PixelBoard _device;
        private readonly ILogger _logger;
        public event EventHandler OnNeedRedraw;
        public PixelDeviceViewModel(Device d, ILogger l)
        {
            _device = (PixelBoard)d;
            _logger = l;
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
    }
}
