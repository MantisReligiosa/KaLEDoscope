using System.Net;
using System.Net.Sockets;
using System.Text;
using BaseDevice;
using Newtonsoft.Json;
using ServiceInterfaces;

namespace CommandProcessing
{
    public class CommandProcessor
    {
        private readonly ILogger _logger;
        private readonly DeviceFactory.DeviceFactory _deviceFactory;

        public CommandProcessor(ILogger logger, DeviceFactory.DeviceFactory deviceFactory)
        {
            _logger = logger;
            _deviceFactory = deviceFactory;
        }

        public void UploadSettings(Device d)
        {
            _logger.Info(this, $"Применение конфигурации устройства {d.Name}");
            var request = new DTO.Request
            {
                Device = d
            };
            var endPoint = new IPEndPoint(IPAddress.Parse(d.Network.IpAddress), d.Network.Port);
            var _tcpClient = new TcpClient();
            _tcpClient.Connect(endPoint);
            var requestString = JsonConvert.SerializeObject(request);
            var bytes = Encoding.UTF8.GetBytes(requestString);
            _logger.Debug(this, $"Запрос к {d.Network.IpAddress}:{d.Network.Port} {requestString}");
            var stream = _tcpClient.GetStream();
            stream.Write(bytes, 0, bytes.Length);
            _tcpClient.Close();
        }
    }
}