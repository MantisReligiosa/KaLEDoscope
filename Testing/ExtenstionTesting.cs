using Extensions;
using Xunit;

namespace Testing
{
    public class ExtenstionTesting
    {
        [Fact]
        public void UshortToByteTesting()
        {
            ushort value = 0xabcd;
            var bytes = value.ToBytes();
            Assert.Equal(new byte[] { 0xab, 0xcd }, bytes);
        }

        [Fact]
        public void UintToByteTesting()
        {
            uint value = 0xabcdef12;
            var bytes = value.ToBytes();
            Assert.Equal(new byte[] { 0xab, 0xcd, 0xef, 0x12 }, bytes);
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

        [Fact]
        public void ExtractUintTesting()
        {
            var bytes = new byte[] { 0xab, 0xcd, 0xef, 0x12 };
            var value = bytes.ExtractUint(0);
            Assert.Equal(0xabcdef12, value);
        }

        [Fact]
        public void ExtractBitTexting()
        {
            var b = (byte)0b11110000;
            Assert.True(b.GetBit(0));
            Assert.True(b.GetBit(1));
            Assert.True(b.GetBit(2));
            Assert.True(b.GetBit(3));
            Assert.False(b.GetBit(4));
            Assert.False(b.GetBit(5));
            Assert.False(b.GetBit(6));
            Assert.False(b.GetBit(7));
        }
    }
}
