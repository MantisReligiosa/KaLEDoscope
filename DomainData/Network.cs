namespace DomainData
{
    public class Network
    {
        public string IpAddress { get; set; }
        public byte SubnetMask { get; set; }
        public string Gateway { get; set; }
        public string DnsServer { get; set; }
        public string AlternativeDnsServer { get; set; }
        public int Port { get; set; }
    }
}