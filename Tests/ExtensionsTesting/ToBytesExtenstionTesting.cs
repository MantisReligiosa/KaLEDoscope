using Extensions;
using Xunit;

namespace ExtensionsTesting
{
    public class ToBytesExtenstionTesting
    {
        [Fact]
        public void UshortToByteTesting()
        {
            ushort value = 0xabcd;
            var bytes = value.ToBytes();
            Assert.Equal(new byte[] { 0xab, 0xcd }, bytes);
        }

        [Fact]
        public void StringToByteTesting()
        {
            var bytes = "test".ToBytes();
            Assert.Equal(new byte[] { 0x74, 0x65, 0x73, 0x74 }, bytes);
        }

        [Fact]
        public void ExtractStringTesting()
        {
            var bytes = new byte[] { 0x74, 0x74, 0x65, 0x73, 0x74, 0x74 };
            var test = bytes.ExtractString(1, 4);
            Assert.Equal("test", test);
        }

        [Fact]
        public void ExtractUshortTesting()
        {
            var bytes = new byte[] { 0xab, 0xcd };
            var value = bytes.ExtractUshort(0);
            Assert.Equal(0xabcd, value);
        }
    }
}
