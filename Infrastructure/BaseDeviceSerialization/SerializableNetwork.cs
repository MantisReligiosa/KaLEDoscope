using BaseDevice;

namespace BaseDeviceSerialization
{
    public class SerializableNetwork
    {
        public string AlternativeDnsServer { get; set; }
        public string DnsServer { get; set; }
        public string Gateway { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public byte SubnetMask { get; set; }

        public static explicit operator SerializableNetwork(Network network)
        {
            return new SerializableNetwork
            {
                AlternativeDnsServer = network.AlternativeDnsServer,
                DnsServer = network.DnsServer,
                Gateway = network.Gateway,
                IpAddress = network.IpAddress,
                Port = network.Port,
                SubnetMask = network.SubnetMask
            };
        }
    }
}