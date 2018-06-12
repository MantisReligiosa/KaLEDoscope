using CommandProcessing;
using Extensions;
using PixelBoardDevice.DomainObjects;

namespace PixelBoardDevice.Responces
{
    public class BoardConfigResponce : Responce<BoardSize>
    {
        public override byte ResponceID => 0x20;

        public override BoardSize Cast()
        {
            return new BoardSize
            {
                Width = _bytes.ExtractUshort(5),
                Height = _bytes.ExtractUshort(7)
            };
        }
    }
}
