namespace BaseDevice
{
    public class Network
    {
        public string IpAddress
        {
            get => EditedIpAddress.Equals(string.Empty) ? ActualIpAddress : EditedIpAddress;
            set
            {
                if (ActualIpAddress.Equals(string.Empty))
                {
                    ActualIpAddress = value;
                }
                else
                {
                    EditedIpAddress = value;
                }
            }
        }

        public int Port
        {
            get => EditedPort.Equals(default(int)) ? ActualPort : EditedPort;
            set
            {
                if (ActualPort.Equals(default(int)))
                {
                    ActualPort = value;
                }
                else
                {
                    EditedPort = value;
                }
            }
        }

        public void ApplyChangingIpAddress()
        {
            ActualIpAddress = EditedIpAddress;
            EditedIpAddress = string.Empty;
        }

        public void ApplyChangingPort()
        {
            ActualPort = EditedPort;
            EditedPort = default(int);
        }

        public string EditedIpAddress { get; set; } = string.Empty;
        public string ActualIpAddress { get; set; } = string.Empty;
        public int EditedPort { get; set; } = default(int);
        public int ActualPort { get; set; } = default(int);
        public byte SubnetMask { get; set; }
        public string Gateway { get; set; }
        public string DnsServer { get; set; }
        public string AlternativeDnsServer { get; set; }
    }
}
