using PixelBoardDevice.DTO;
using PixelBoardDevice.Requests;
using Xunit;

namespace PixelBoardDeviceTesting
{
    public class RequestTesting
    {
        [Fact]
        public void GetIdListRequest_ToByteSequence()
        {
            var request = new GetIdListRequest
            {
                DeviceID = 0x14
            };
            request.SetRequestData(2);
            Assert.Equal(new byte[]
            {
                0x00, 0x14, 0x22, 0x00, 0x01, 0x02
            }, request.GetBytes());
        }

        [Fact]
        public void GetStorageItemRequest_ToByteSequence()
        {
            var request = new GetStorageItemRequest
            {
                DeviceID = 0x2222
            };
            request.SetRequestData(new StorageItemIndex
            {
                StorageId = 1,
                ItemId = 170
            });
            Assert.Equal(new byte[]
            {
                0x22, 0x22, 0x24, 0x00, 0x02, 0x01, 0xAA
            }, request.GetBytes());
        }
    }
}
