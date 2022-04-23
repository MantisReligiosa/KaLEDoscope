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

        protected RequestingCommand(Device device, INetworkAgent networkAgent, ILogger logger, IConfig config)
            : base(device, networkAgent, logger, config)
        {
            _port = config.RequestPort;
            _timeout = config.ResponceTimeout;
        }

        public abstract object GetRequestData();

        protected override void CommandExecute()
        {
            var request = new TRequest
            {
                DeviceID = _device.Id
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

        public void OnIdentityRecieved(TResponce responce)
        {
            _timer.Stop();
            if (responce.Resultativity == Resultativity.Busy)
            {
                RaiseRepeat();
                return;
            }
            if (responce.Resultativity != Resultativity.DataReturned
                && responce.Resultativity != Resultativity.Accepted)
            {
                _logger.Debug(this, $"Устройство вернуло код {responce.Resultativity}");
                RaiseError(new ExchangeException("Ошибка запроса данных"));
                return;
            }
            var dto = responce.Cast();
            ProcessRecievedData(dto);
            _networkAgent.Close();
            RaiseSuccess();
        }

        public abstract void ProcessRecievedData(TResponceDTO responceDTO);
    }
}
