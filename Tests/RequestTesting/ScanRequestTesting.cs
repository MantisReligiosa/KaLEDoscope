using CommandProcessing.Requests;
using Xunit;

namespace RequestTesting
{
    public class ScanRequestTesting
    {
        [Fact]
        public void TestByteSequence()
        {
            var scanRequest = new ScanRequest();
            Assert.Equal(new byte[] { 0, 0, 0, 0, 0 }, scanRequest.GetBytes());
        }
    }
}
