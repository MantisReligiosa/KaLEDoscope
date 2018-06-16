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

        protected UploadStorageItemsCommand(Device device, INetworkAgent networkAgent, ILogger logger,
            int port = 500,
            int timeout = 100)
            : base(device, networkAgent, logger)
        {
            _port = port;
            _timeout = timeout;
        }

        public abstract byte StorageId { get; }

        public abstract List<TItem> Items { get; }

        private int itemIndex = 0;
        public override void Execute()
        {
            var request = new CleanupStorageRequest()
            {
                DeviceID = (ushort)_device.Id
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
            if (responce.Resultativity != Resultativity.DataRequest
                || responce.Resultativity != Resultativity.Accepted)
            {
                _logger.Debug(this, $"Устройство вернуло код {responce.Resultativity}");
                RaiseError(new ExchangeException("Ошибка запроса данных"));
                return;
            }
            _timer.Stop();
            _networkAgent.Close();
            if (Items.Any())
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
                DeviceID = (ushort)_device.Id
            };
            request.SetRequestData(Items[itemIndex]);
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
            if (responce.Resultativity == Resultativity.Busy)
            {
                _logger.Debug(this, "Устройстов занято");
                SendItem();
                return;
            }
            if (responce.Resultativity != Resultativity.DataRequest
                || responce.Resultativity != Resultativity.Accepted)
            {
                _logger.Debug(this, $"Устройство вернуло код {responce.Resultativity}");
                RaiseError(new ExchangeException("Ошибка запроса данных"));
                return;
            }
            _timer.Stop();
            _networkAgent.Close();
            itemIndex++;
            if (Items.Count == itemIndex)
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
