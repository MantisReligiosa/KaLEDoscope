using Extensions;
using Xunit;

namespace BaseCommandsTesting
{
    public class BytesExtensionsTesting
    {
        [Fact]
        public void UshortToByteTesting()
        {
            ushort target = 0xabcd;
            Assert.Equal(target.ToBytes(), new byte[] { 0xab, 0xcd });
        }

        [Fact]
        public void BytesToByteStringTesting()
        {
            var target = new byte[] { 0x33,0xff};
            Assert.Equal("[33][FF]",target.ToByteString());
        }
    }
}
