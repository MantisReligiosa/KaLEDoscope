using CommandProcessing;
using Extensions;
using PixelBoardDevice.DomainObjects;

namespace PixelBoardDevice.Responces
{
    public class BoardHardwareConfigResponce : Responce<BoardHardware>
    {
        public override byte ResponceID => 0x27;

        public override BoardHardware Cast()
        {
            return new BoardHardware
            {
                Type = (BoardHardwareType)_bytes[5]
            };
        }
    }
}
