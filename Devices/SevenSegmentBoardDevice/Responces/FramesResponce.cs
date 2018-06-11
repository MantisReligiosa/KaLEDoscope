using CommandProcessing;
using Extensions;
using SevenSegmentBoardDevice.DTO;
using System.Collections.Generic;
using System.Linq;

namespace SevenSegmentBoardDevice.Responces
{
    public class FramesResponce : Responce<List<ActiveFrameDTO>>
    {
        public override byte ResponceID => 0x12;

        public override List<ActiveFrameDTO> Cast()
        {
            return GetFrames().ToList();
        }

        private IEnumerable<ActiveFrameDTO> GetFrames()
        {
            for (int i = 0; i < _bytes[5]; i++)
            {
                yield return new ActiveFrameDTO
                {
                    Id = _bytes[6 + i * 3],
                    Duration = _bytes.ExtractUshort(7 + i * 3)
                };
            }
        }
    }
}
