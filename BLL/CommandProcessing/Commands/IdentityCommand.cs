using BaseDevice;
using CommandProcessing.DTO;
using CommandProcessing.Exceptions;
using CommandProcessing.Responces;
using ServiceInterfaces;
using System;
using System.Timers;

namespace CommandProcessing
{
    public class IdentityCommand : DeviceCommand<Device>
    {
        public override string Name => throw new NotImplementedException();
        private readonly int _port;
        private readonly int _timeout;
        private Timer _timer;

        public IdentityCommand(Device device, INetworkAgent networkAgent, ILogger logger,
            int port = 500,
            int timeout = 100)
            : base(device, networkAgent, logger)
        {
            _port = port;
            _timeout = timeout;
        }

        public override void Execute()
        {
            var request = new ConfigurationRequest
            {
                DeviceID = (ushort)_device.Id
            };
            request.SetRequestData(3);
            try
            {
                _networkAgent.Send(_device.Network.IpAddress, _device.Network.Port, request);
            }
            catch (Exception ex)
            {
                RaiseError(ex);
            }
            _timer = new Timer()
            {
                AutoReset = false,
                Interval = _timeout
            };
            _timer.Elapsed += (o, e) =>
            {
                _networkAgent.Close();
                RaiseError(new ExchangeException("Тайм-аут ответа"));
            };
            _timer.Start();
            try
            {
                _networkAgent.Listen<IdentityResponce, Identity>(_port, OnIdentityRecieved);
            }
            catch (Exception ex)
            {
                RaiseError(ex);
            }
        }

        private void OnIdentityRecieved(IdentityResponce responce)
        {
            if (responce.Resultativity == Resultativity.Busy)
            {
                _logger.Debug(this, "Устройстов занято");
                RaiseRepeat();
                return;
            }
            if (responce.Resultativity != Resultativity.DataRequest)
            {
                _logger.Debug(this, $"Устройство вернуло код {responce.Resultativity}");
                RaiseError(new ExchangeException("Ошибка запроса данных"));
                return;
            }
            var identity = responce.Cast();
            _device.Id = identity.Id;
            _device.Name = identity.Name;
            _timer.Stop();
            _networkAgent.Close();
            RaiseSuccess();
        }
    }
}
