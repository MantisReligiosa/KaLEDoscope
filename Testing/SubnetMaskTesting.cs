using Extensions;
using Xunit;

namespace Testing
{
    public class SubnetMaskTesting
    {
        [Fact]
        public void SubnetToByte()
        {
            var bytes = new byte[] { 0, 255, 255, 255, 0b11000000, 0 };
            Assert.True(bytes.TrySubnetToByte(1, out byte subnetByte));
            Assert.Equal(26, subnetByte);
        }

        [Fact]
        public void SubnetToByte_Fails()
        {
            var bytes = new byte[] { 0, 255, 0, 255, 0b11000000, 0 };
            Assert.False(bytes.TrySubnetToByte(1, out byte subnetByte));
        }

        [Fact]
        public void ByteToSubnetMask()
        {
            var bytes = ((byte)26).ByteToSubnetMask();
            Assert.Equal(new byte[] { 255, 255, 255, 0b11000000 }, bytes);
        }
    }
}
