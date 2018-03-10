using System.Linq;
using DeviceBuilding;
using BaseDevice;
using CommandProcessing.DTO;
using devFactory = DeviceFactory;
using Newtonsoft.Json;
using ServiceInterfaces;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Timers;
using System.Collections.Generic;

namespace CommandProcessing
{
    public class DirectConnectDownloadSettingsCommand : Command<Device>
    {
        private readonly List<IDeviceBuilder> _deviceBuilders;
        private readonly int _timeout;
        private Timer _timer;

        public override string Name => "Запрос конфигурации";

        public event Action<Device> OnConfigurationDownloaded;

        public DirectConnectDownloadSettingsCommand(Device device,
            List<IDeviceBuilder> deviceBuilders, ILogger logger, int timeout = 10000) : base(device, logger)
        {
            _deviceBuilders = deviceBuilders;
            _timeout = timeout;
        }

        private TcpClient _tcpClient;

        public override void Execute()
        {
            var ipEndpoint = new IPEndPoint(IPAddress.Parse(_device.Network.IpAddress), _device.Network.Port);
            _tcpClient = new TcpClient();
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
            var tcpListener = new TcpListener(new IPEndPoint(IPAddress.Any, _device.Network.Port));
            var s = new TcpState
            {
                Endpoint = ipEndpoint,
                TcpListener = tcpListener
            };
            tcpListener.Start();
            tcpListener.BeginAcceptSocket(new AsyncCallback(DoAcceptSocketCallback), s);

            _timer = new Timer()
            {
                AutoReset = false,
                Interval = _timeout
            };
            _timer.Elapsed += (o, e) =>
            {
                EndCommand(tcpListener);
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
            var listener = ((TcpState)(ar.AsyncState)).TcpListener;
            try
            {
                var ipEndPoint = ((TcpState)(ar.AsyncState)).Endpoint;
                if (!ipEndPoint.Address.ToString().Equals(_device.Network.IpAddress))
                {
                    var tcpState = new TcpState
                    {
                        Endpoint = ipEndPoint,
                        TcpListener = listener
                    };
                    listener.BeginAcceptSocket(new AsyncCallback(DoAcceptSocketCallback), tcpState);
                    return;
                }
                var client = listener.EndAcceptTcpClient(ar);
                var stream = client.GetStream();
                var data = new byte[1024];
                var recievedBytesAmount = stream.Read(data, 0, data.Length);
                var recievedBytes = new byte[recievedBytesAmount];
                Array.Copy(data, recievedBytes, recievedBytesAmount);
                var recieveString = Encoding.UTF8.GetString(recievedBytes);
                var d = (Device)JsonConvert.DeserializeObject(recieveString, _device.GetType());

                var deviceBuilder = _deviceBuilders.FirstOrDefault(builder => builder.Model.Equals(d.Model));
                if (deviceBuilder != null)
                {
                    _device = deviceBuilder.UpdateCustomSettings(d);
                }
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

        public override void Finally()
        {
            _tcpClient.Close();
        }
    }
}
