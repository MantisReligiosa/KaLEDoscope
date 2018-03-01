using BaseDevice;
using CommandProcessing.DTO;
using DeviceFactory;
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
    public class DirectConnectDownloadSettingsCommand : Command<Device>
    {
        private readonly DeviceFactory.DeviceFactory _deviceFactory;
        private readonly int _timeout;
        private System.Timers.Timer _timer;

        public event Action<Device> OnConfigurationDownloaded;

        public DirectConnectDownloadSettingsCommand(Device device, DeviceFactory.DeviceFactory deviceFactory, ILogger logger, int timeout = 10000) : base(device, logger)
        {
            _deviceFactory = deviceFactory;
            _timeout = timeout;
        }

        public override void Execute()
        {
            _logger.Info(this, $"Запрос конфигурации от устройства {_device.Name}");

            var ipEndpoint = new IPEndPoint(IPAddress.Parse(_device.Network.IpAddress), _device.Network.Port);
            var _tcpClient = new TcpClient();
            _tcpClient.Connect(ipEndpoint);
            var requestString = JsonConvert.SerializeObject(new Request
            {
                GetConfig = new object()
            });
            var bytes = Encoding.UTF8.GetBytes(requestString);

            _logger.Debug(this, $"Запрос к {_device.Network.IpAddress}:{_device.Network.Port}: {requestString}");
            var stream = _tcpClient.GetStream();
            stream.Write(bytes, 0, bytes.Length);
            _tcpClient.Close();

            _logger.Debug(this, $"Жду ответ {_timeout} мс");

            var _tcpListener = new TcpListener(new IPEndPoint(IPAddress.Any, _device.Network.Port));
            var s = new TcpState
            {
                e = ipEndpoint,
                l = _tcpListener
            };
            _tcpListener.Start();
            _tcpListener.BeginAcceptSocket(new AsyncCallback(DoAcceptSocketCallback), s);

            _timer = new System.Timers.Timer()
            {
                AutoReset = false,
                Interval = _timeout
            };
            _timer.Elapsed += (o, e) =>
            {
                EndCommand(_tcpListener);
            };
            _timer.Start();
        }

        private void EndCommand(TcpListener listener)
        {
            listener.Stop();
            _logger.Debug(this, $"Завершение сканирования");
            OnConfigurationDownloaded?.Invoke(_device);
        }

        public void DoAcceptSocketCallback(IAsyncResult ar)
        {
            TcpListener listener = ((TcpState)(ar.AsyncState)).l;
            try
            {
                IPEndPoint e = ((TcpState)(ar.AsyncState)).e;

                if (!e.Address.ToString().Equals(_device.Network.IpAddress))
                {
                    var s = new TcpState
                    {
                        e = e,
                        l = listener
                    };
                    listener.BeginAcceptSocket(new AsyncCallback(DoAcceptSocketCallback), s);
                    return;
                }

                var client = listener.EndAcceptTcpClient(ar);
                var stream = client.GetStream();
                var data = new byte[1024];
                var recievedBytesAmount = stream.Read(data, 0, data.Length);
                var recievedBytes = new byte[recievedBytesAmount];
                Array.Copy(data, recievedBytes, recievedBytesAmount);
                var recieveString = Encoding.UTF8.GetString(recievedBytes);
                var d = JsonConvert.DeserializeObject(recieveString, _device.GetType());
                _device = _deviceFactory.Customize(d);
                _timer.Stop();
                EndCommand(listener);
            }
            catch (ObjectDisposedException)
            {
                EndCommand(listener);
            }
            catch (Exception ex)
            {
                EndCommand(listener);
                _logger.Error(this, "Ошибка при получении ответа", ex);
                _timer.Stop();
            }
        }
    }

    class TcpState
    {
        internal IPEndPoint e;
        internal TcpListener l;

        public TcpState()
        {
        }
    }
}
