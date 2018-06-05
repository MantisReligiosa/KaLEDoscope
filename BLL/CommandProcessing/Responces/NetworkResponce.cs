using BaseDevice;
using Extensions;

namespace CommandProcessing.Responces
{
    public class NetworkResponce : Responce<Network>
    {
        public override byte ResponceID => 2;

        public override Network Cast()
        {
            return new Network
            {
                IpAddress = $"{_bytes[5]}.{_bytes[6]}.{_bytes[7]}.{_bytes[8]}",
                Port = _bytes.ExtractUshort(9),
                SubnetMask = _bytes.SubnetToByte(11),
                Gateway = $"{_bytes[15]}.{_bytes[16]}.{_bytes[17]}.{_bytes[18]}",
                DnsServer = $"{_bytes[19]}.{_bytes[20]}.{_bytes[21]}.{_bytes[22]}",
                AlternativeDnsServer = $"{_bytes[23]}.{_bytes[24]}.{_bytes[25]}.{_bytes[26]}"
            };
        }
    }
}
