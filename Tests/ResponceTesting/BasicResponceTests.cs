using Xunit;

namespace ResponceTesting
{
    public class BasicResponceTests
    {
        [Fact]
        public void TestToString()
        {
            var responce = new SomeResponce();
            responce.SetByteSequence(new byte[] { 0x00, 0xaa, 0xbb, 0xcc });
            Assert.Equal("[00][AA][BB][CC]", responce.ToString());
        }
    }
}
