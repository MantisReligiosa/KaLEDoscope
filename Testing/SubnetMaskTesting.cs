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
            Assert.Equal(26, bytes.SubnetToByte(1));
        }

        [Fact]
        public void ByteToSubnetMask()
        {
            var bytes = ((byte)26).ByteToSubnetMask();
            Assert.Equal(new byte[] { 255, 255, 255, 0b11000000 }, bytes);
        }
    }
}
