namespace BaseDevice
{
    public class Network
    {
        public string IpAddress
        {
            get => !IsIpAddressEdited ? ActualIpAddress : EditedIpAddress;
            set
            {
                if (string.IsNullOrEmpty(ActualIpAddress))
                {
                    ActualIpAddress = value;
                }
                else
                {
                    EditedIpAddress = value;
                }
            }
        }

        public bool IsIpAddressEdited => !string.IsNullOrEmpty(EditedIpAddress);

        public int Port
        {
            get => !IsPortEdited ? ActualPort : EditedPort;
            set
            {
                if (ActualPort.Equals(default))
                {
                    ActualPort = value;
                }
                else
                {
                    EditedPort = value;
                }
            }
        }

        public bool IsPortEdited => !EditedPort.Equals(default);

        public string EditedIpAddress { get; set; } = string.Empty;
        public string ActualIpAddress { get; set; } = string.Empty;
        public int EditedPort { get; set; } = default;
        public int ActualPort { get; set; } = default;
        public byte SubnetMask { get; set; }
        public string Gateway { get; set; }
        public string DnsServer { get; set; }
        public string AlternativeDnsServer { get; set; }
    }
}
