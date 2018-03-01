using BaseDevice;
using CommandProcessing.DTO;
using Newtonsoft.Json;
using ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace CommandProcessing
{
    public class DirectConnectScanCommand : Command<Device>
    {
        private readonly int _port;
        private readonly int _timeout;
        private readonly List<Device> _devices;

        private event Action<Device> _deviceRecieved;
        public event Action<List<Device>> OnScanCompleted;

        public DirectConnectScanCommand(ILogger logger, int port = 30000, int timeout = 10000) : base(null, logger)
        {
            _devices = new List<Device>();
            _port = port;
            _timeout = timeout;
        }

        public override void Execute()
        {
            //_devices.Add(new Device
            //{
            //    Model = "boardClock",
            //    Network = new Network
            //    {
            //        IpAddress = "192.168.0.88",
            //        Port = 500
            //    }
            //});



            _logger.Info(this, $"Начало сканирования по UDP. Порт {_port}");
            var endPoint = new IPEndPoint(IPAddress.Broadcast, _port);
            var _udpClient = new UdpClient();
            _udpClient.Connect(endPoint);
            var request = new Request();
            request.Scan = new object();
            var requestString = JsonConvert.SerializeObject(request);
            var bytes = Encoding.UTF8.GetBytes(requestString);
            _logger.Debug(this, $"Широковещательный запрос: {requestString}");
            _udpClient.Send(bytes, bytes.Length);
            _udpClient.Close();


            endPoint = new IPEndPoint(IPAddress.Any, _port);
            _udpClient = new UdpClient(endPoint);
            var s = new UdpState();
            s.e = endPoint;
            s.u = _udpClient;
            _logger.Debug(this, $"Жду ответы {_timeout} мс");

            _udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), s);
            _deviceRecieved += (d) => _devices.Add(d);

            var timer = new System.Timers.Timer()
            {
                AutoReset = false,
                Interval = _timeout
            };
            timer.Elapsed += (o, e) =>
            {
                _udpClient.Close();
                _logger.Debug(this, $"Завершение сканирования");
                OnScanCompleted?.Invoke(_devices);
            };
            timer.Start();
        }


        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                UdpClient u = ((UdpState)(ar.AsyncState)).u;
                IPEndPoint e = ((UdpState)(ar.AsyncState)).e;

                var recieveBytes = u.EndReceive(ar, ref e);
                var recieveString = Encoding.UTF8.GetString(recieveBytes);

                var responce = JsonConvert.DeserializeObject<ScanCommandResponce>(recieveString);
                _logger.Debug(this, $"Ответ: {recieveString}");

                var device = new Device
                {
                    Model = responce.DeviceParameters.Model,
                    Network = new Network
                    {
                        IpAddress = responce.NetworkParameters.Address,
                        Port = responce.NetworkParameters.Port
                    }
                };
                _logger.Info(this, $"Ответ устройства {device.Model} IP {device.Network.IpAddress}:{device.Network.Port}");

                _deviceRecieved?.Invoke(device);
                var s = new UdpState();
                s.e = e;
                s.u = u;
                u.BeginReceive(new AsyncCallback(ReceiveCallback), s);
            }
            catch (ObjectDisposedException) { }
            catch (Exception ex)
            {
                _logger.Error(this, "UDP error", ex);
            }
        }
    }
}
