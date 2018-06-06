using BaseDevice;
using Extensions;
using System;

namespace CommandProcessing.Requests
{
    public class UploadNetworkRequest : Request
    {
        public override byte RequestID => 0x02;

        public override byte[] MakeData(object o)
        {
            var network = o as Network;
            var bytes = new byte[22];
            var ip = network.IpAddress.Split('.');
            bytes[0] = Convert.ToByte(ip[0]);
            bytes[1] = Convert.ToByte(ip[1]);
            bytes[2] = Convert.ToByte(ip[2]);
            bytes[3] = Convert.ToByte(ip[3]);
            Array.Copy(((ushort)network.Port).ToBytes(), 0, bytes, 4, 2);
            Array.Copy(network.SubnetMask.ByteToSubnetMask(), 0, bytes, 6, 4);
            var gateway = network.Gateway.Split('.');
            bytes[10] = Convert.ToByte(gateway[0]);
            bytes[11] = Convert.ToByte(gateway[1]);
            bytes[12] = Convert.ToByte(gateway[2]);
            bytes[13] = Convert.ToByte(gateway[3]);
            var dns = network.DnsServer.Split('.');
            bytes[14] = Convert.ToByte(dns[0]);
            bytes[15] = Convert.ToByte(dns[1]);
            bytes[16] = Convert.ToByte(dns[2]);
            bytes[17] = Convert.ToByte(dns[3]);
            var alternateDns = network.AlternativeDnsServer.Split('.');
            bytes[18] = Convert.ToByte(alternateDns[0]);
            bytes[19] = Convert.ToByte(alternateDns[1]);
            bytes[20] = Convert.ToByte(alternateDns[2]);
            bytes[21] = Convert.ToByte(alternateDns[3]);
            return bytes;
        }
    }
}
