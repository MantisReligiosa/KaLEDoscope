using CommandProcessing.DTO;
using SmartTechnologiesM.Base.Extensions;

namespace CommandProcessing.Responces
{
    public class IdentityResponce : Responce<Identity>
    {
        public override byte ResponceID => 3;

        public override Identity Cast()
        {
            var deviceNameLenght = _bytes[7];
            return new Identity
            {
                Id = _bytes.ExtractUshort(5),
                Name = _bytes.ExtractString(8, deviceNameLenght)
            };
        }
    }
}
