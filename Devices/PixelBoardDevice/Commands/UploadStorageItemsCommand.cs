using BaseDevice;
using CommandProcessing;
using CommandProcessing.Exceptions;
using CommandProcessing.Responces;
using PixelBoardDevice.Requests;
using ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace PixelBoardDevice.Commands
{
    public abstract class UploadStorageItemsCommand<TUploadStorageItemRequest, TItem> : DeviceCommand<Device>
        where TUploadStorageItemRequest : Request, new()
        where TItem : class
    {
        private readonly int _port;
        private readonly int _timeout;
        private Timer _timer;

        protected UploadStorageItemsCommand(Device device, INetworkAgent networkAgent, ILogger logger, IConfig config)
            : base(device, networkAgent, logger, config)
        {
            _port = config.RequestPort;
            _timeout = config.ResponceTimeout;
        }

        public abstract byte StorageId { get; }

        public abstract List<TItem> GetItems();

        private int itemIndex = 0;
        protected override void CommandExecute()
        {
            var request = new CleanupStorageRequest()
            {
                DeviceID = _device.Id
            };
            request.SetRequestData(StorageId);
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
                _networkAgent.Listen<AcceptanceResponce, object>(_port, OnStorageCleanedUp);
            }
            catch (Exception ex)
            {
                RaiseError(ex);
            }
        }

        private void OnStorageCleanedUp(AcceptanceResponce responce)
        {
            if (responce.Resultativity == Resultativity.Busy)
            {
                _logger.Debug(this, "Устройстов занято");
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
            _timer?.Stop();
            _networkAgent.Close();
            if (GetItems().Any())
            {
                SendItem();
            }
            else
            {
                RaiseSuccess();
            }
        }

        private void SendItem()
        {
            var request = new TUploadStorageItemRequest()
            {
                DeviceID = _device.Id
            };
            request.SetRequestData(GetItems()[itemIndex]);
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
                _networkAgent.Listen<AcceptanceResponce, object>(_port, OnItemSended);
            }
            catch (Exception ex)
            {
                RaiseError(ex);
            }
        }

        private void OnItemSended(AcceptanceResponce responce)
        {
            _timer?.Stop();
            if (responce.Resultativity == Resultativity.Busy)
            {
                _logger.Info(this, "Устройство занято. Повторная отправка");
                SendItem();
                return;
            }
            if (responce.Resultativity != Resultativity.DataReturned
                && responce.Resultativity != Resultativity.Accepted)
            {
                _logger.Debug(this, $"Устройство вернуло код {responce.Resultativity}");
                RaiseError(new ExchangeException("Ошибка запроса данных"));
                return;
            }
            
            _networkAgent.Close();
            itemIndex++;
            if (GetItems().Count == itemIndex)
            {
                RaiseSuccess();
            }
            else
            {
                SendItem();
            }
        }
    }
}
