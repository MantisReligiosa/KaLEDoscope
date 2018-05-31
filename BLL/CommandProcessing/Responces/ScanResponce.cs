using BaseDevice;
using CommandProcessing.Exceptions;
using Extensions;
using ServiceInterfaces;

namespace CommandProcessing.Responces
{
    public class ScanResponce : Responce<Device>
    {
        public override Device Cast()
        {
            var responceId = _bytes[2];
            if (responceId != 1)
                throw new InvalidByteSequenceException("Неверный ID ответа");
            var dataLength = _bytes.ExtractUshort(3);
            if (dataLength != _bytes.Length - 5)
                throw new InvalidByteSequenceException("Неверная длина данных");
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
