using CommandProcessing;
using PixelBoardDevice.DTO;

namespace PixelBoardDevice.Requests
{
    public class GetStorageItemRequest : Request
    {
        public override byte RequestID => 0x24;

        public override byte[] MakeData(object o)
        {
            var index = o as StorageItemIndex;
            return new byte[]
            {
                (byte)index.StorageId,
                (byte)index.ItemId
            };
        }
    }
}
