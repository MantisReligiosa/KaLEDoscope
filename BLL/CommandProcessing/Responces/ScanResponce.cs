using BaseDevice;
using SmartTechnologiesM.Base.Extensions;

namespace CommandProcessing.Responces
{
    public class ScanResponce : Responce<Device>
    {
        public override byte ResponceID => 1;

        public override Device Cast()
        {
            var modelNameLenght = _bytes[11];
            var deviceLenght = _bytes[modelNameLenght + 15];
            return new Device
            {
                Id = _bytes.ExtractUshort(0),
                Network = new Network
                {
                    IpAddress = $"{_bytes[5]}.{_bytes[6]}.{_bytes[7]}.{_bytes[8]}",
                    Port = _bytes.ExtractUshort(9)
                },
                Model = _bytes.ExtractString(12, modelNameLenght),
                Firmware = $"{_bytes[modelNameLenght + 12]}.{_bytes[modelNameLenght + 13]}.{_bytes[modelNameLenght + 14]}",
                Name = _bytes.ExtractString(modelNameLenght + 16, deviceLenght)
            };
        }
    }
}
