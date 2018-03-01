using BaseDevice;
using Newtonsoft.Json;
using ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CommandProcessing
{
    public class DirectConnectUploadSettingsCommand : Command<Device>
    {
        public DirectConnectUploadSettingsCommand(Device device, ILogger logger) : base(device, logger)
        {
        }

        public override void Execute()
        {

            _logger.Info(this, $"Применение конфигурации устройства {_device.Name}");
            var request = new DTO.Request
            {
                Device = _device
            };
            var endPoint = new IPEndPoint(IPAddress.Parse(_device.Network.IpAddress), _device.Network.Port);
            var _tcpClient = new TcpClient();
            _tcpClient.Connect(endPoint);
            var requestString = JsonConvert.SerializeObject(request);
            var bytes = Encoding.UTF8.GetBytes(requestString);
            _logger.Debug(this, $"Запрос к {_device.Network.IpAddress}:{_device.Network.Port} {requestString}");
            var stream = _tcpClient.GetStream();
            stream.Write(bytes, 0, bytes.Length);
            _tcpClient.Close();
        }
    }
}
