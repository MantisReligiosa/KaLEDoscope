using ServiceInterfaces;
using System.Collections.ObjectModel;

namespace KaLEDoscope
{
    public class ProtocolNode
    {
        public string Name { get; set; }

        public IProtocol Protocol{get;set;}

        public ObservableCollection<DeviceNode> Members { get; set; } = new ObservableCollection<DeviceNode>();
    }
}
