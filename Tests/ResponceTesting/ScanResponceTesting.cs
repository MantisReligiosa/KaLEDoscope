using CommandProcessing.Responces;
using Xunit;

namespace ResponceTesting
{
    public class ScanResponceTesting
    {
        [Fact]
        public void TestByteSequence()
        {
            var scanResponce = new ScanResponce();
            scanResponce.SetByteSequence(new byte[]
            {
                0xAB,0xCD,0x01,0x00,0x20,0xC0,0xA8,0x00,0x09,0x01,0xF4,0x0B,0x64,0x65,0x76,0x69,0x63,0x65,0x4D,0x6F,0x64,0x65,0x6C,0xAA,0x00,0x05,0x0A,0x64,0x65,0x76,0x69,0x63,0x65,0x4E,0x61,0x6D,0x65
            });
            var device = scanResponce.Cast();
            Assert.Equal(0xabcd, device.Id);
            Assert.Equal("192.168.0.9", device.Network.IpAddress);
            Assert.Equal(500, device.Network.Port);
            Assert.Equal("deviceModel", device.Model);
            Assert.Equal("170.0.5", device.Firmware);
            Assert.Equal("deviceName", device.Name);
        }
    }
}
