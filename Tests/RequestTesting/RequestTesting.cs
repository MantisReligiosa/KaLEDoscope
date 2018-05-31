using Xunit;

namespace RequestTesting
{
    public class RequestTesting
    {
        [Fact]
        public void TestByteSequence()
        {
            var mock = new MockRequest
            {
                DeviceID = 0x1234
            };
            var bytes = mock.GetBytes();
            Assert.Equal(new byte[] { 0x12, 0x34, 0x01, 0x00, 0x06, 0xaa, 0xbb, 0xcc, 0xdd, 0xee, 0xff }, bytes);
        }

        [Fact]
        public void TestToString()
        {
            var mock = new MockRequest
            {
                DeviceID = 0x1234
            };
            var requestString = mock.ToString();
            Assert.Equal("[12][34][01][00][06][AA][BB][CC][DD][EE][FF]", requestString);
        }
    }
}
