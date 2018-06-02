using CommandProcessing.Requests;
using Xunit;

namespace Testing
{
    public class RequestTesting
    {
        [Fact]
        public void Request_GetBytes()
        {
            var mock = new MockRequest
            {
                DeviceID = 0x1234
            };
            var bytes = mock.GetBytes();
            Assert.Equal(new byte[] { 0x12, 0x34, 0x01, 0x00, 0x06, 0xaa, 0xbb, 0xcc, 0xdd, 0xee, 0xff }, bytes);
        }

        [Fact]
        public void Request_ToString()
        {
            var mock = new MockRequest
            {
                DeviceID = 0x1234
            };
            var requestString = mock.ToString();
            Assert.Equal("[12][34][01][00][06][AA][BB][CC][DD][EE][FF]", requestString);
        }

        [Fact]
        public void ScanRequest_ByteSequence()
        {
            var scanRequest = new ScanRequest();
            Assert.Equal(new byte[] { 0, 0, 0, 0, 0 }, scanRequest.GetBytes());
        }

        [Fact]
        public void ConfigurationRequest_ByteSequence()
        {
            var request = new ConfigurationRequest
            {
                DeviceID = 0xf00
            };
            request.SetRequestData(5);
            Assert.Equal(new byte[] { 0x0F, 0x00, 0x0A, 0x00, 0x01, 0x05 }, request.GetBytes());
        }
    }
}
