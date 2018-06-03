using BaseDevice;
using CommandProcessing.Exceptions;
using ServiceInterfaces;
using System;
using System.Timers;

namespace CommandProcessing
{
    public abstract class RequestingCommand<TRequest, TResponce, TResponceDTO> : DeviceCommand<Device>
        where TRequest : Request, new()
        where TResponce : Responce<TResponceDTO>, new()
        where TResponceDTO : class, new()
    {
        private readonly int _port;
        private readonly int _timeout;
        private Timer _timer;

        public RequestingCommand(Device device, INetworkAgent networkAgent, ILogger logger,
            int port = 500,
            int timeout = 100)
            : base(device, networkAgent, logger)
        {
            _port = port;
            _timeout = timeout;
        }

        public abstract object GetRequestData();

        public override void Execute()
        {
            var request = new TRequest
            {
                DeviceID = (ushort)_device.Id
            };
            request.SetRequestData(GetRequestData());
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
                _networkAgent.Listen<TResponce, TResponceDTO>(_port, OnIdentityRecieved);
            }
            catch (Exception ex)
            {
                RaiseError(ex);
            }
        }

        private void OnIdentityRecieved(TResponce responce)
        {
            if (responce.Resultativity == Resultativity.Busy)
            {
                _logger.Debug(this, "Устройстов занято");
                RaiseRepeat();
                return;
            }
            if (responce.Resultativity != Resultativity.DataRequest
                || responce.Resultativity != Resultativity.Accepted)
            {
                _logger.Debug(this, $"Устройство вернуло код {responce.Resultativity}");
                RaiseError(new ExchangeException("Ошибка запроса данных"));
                return;
            }
            var dto = responce.Cast();
            ProcessRecievedData(dto);
            _timer.Stop();
            _networkAgent.Close();
            RaiseSuccess();
        }

        public abstract void ProcessRecievedData(TResponceDTO responceDTO);
    }
}
