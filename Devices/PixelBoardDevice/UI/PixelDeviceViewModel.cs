using BaseDevice;
using ServiceInterfaces;

namespace PixelBoardDevice.UI
{
    public class PixelDeviceViewModel
    {
        private readonly PixelBoard _device;
        private readonly ILogger _logger;

        public PixelDeviceViewModel(Device d, ILogger l)
        {
            _device = (PixelBoard)d;
            _logger = l;
        }
    }
}
