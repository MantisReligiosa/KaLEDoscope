namespace BaseDevice
{
    public class Network
    {
        private string _actualIpAddress = string.Empty;
        private string _editedIpAddress = string.Empty;

        public string IpAddress
        {
            get => _editedIpAddress.Equals(string.Empty) ? _actualIpAddress : _editedIpAddress;
            set
            {
                if (_actualIpAddress.Equals(string.Empty))
                {
                    _actualIpAddress = value;
                }
                else
                {
                    _editedIpAddress = value;
                }
            }
        }
        public void ApplyChangingIpAddress()
        {
            _actualIpAddress = _editedIpAddress;
            _editedIpAddress = string.Empty;
        }

        public byte SubnetMask { get; set; }
        public string Gateway { get; set; }
        public string DnsServer { get; set; }
        public string AlternativeDnsServer { get; set; }
        public int Port { get; set; }
    }
}
