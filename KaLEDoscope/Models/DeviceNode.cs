using DomainData;
using ServiceInterfaces;

namespace KaLEDoscope
{
    public class DeviceNode
    {
        public IPlugin Plugin { get; set; }
        public Device Device { get; set; }
    }
}
