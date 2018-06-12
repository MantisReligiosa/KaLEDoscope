using CommandProcessing;
using PixelBoardDevice.DTO;
using System.Collections.Generic;

namespace PixelBoardDevice.Responces
{
    public class IdListResponce : Responce<IdList>
    {
        public override byte ResponceID => 0x23;

        public override IdList Cast()
        {
            var idList = new IdList
            {
                StorageId = _bytes[5],
                Items = new List<byte>()
            };
            for (int i = 0; i < _bytes[6]; i++)
            {
                idList.Items.Add(_bytes[7 + i]);
            }
            return idList;
        }
    }
}
