using BaseDevice;
using CommandProcessing;
using CommandProcessing.Exceptions;
using Configuration;
using PixelBoardDevice.DTO;
using PixelBoardDevice.Requests;
using PixelBoardDevice.Responces;
using ServiceInterfaces;
using System;
using System.Linq;
using System.Timers;

namespace PixelBoardDevice.Commands
{
    public abstract class DownloadStorageItemsCommand<TStorageItemResponce, TStorageItem> : DeviceCommand<Device>
        where TStorageItemResponce : Responce<TStorageItem>, new()
        where TStorageItem : class
    {
        private Timer _timer;
        private readonly int _port;
        private readonly int _timeout;

        protected DownloadStorageItemsCommand(Device device, INetworkAgent networkAgent, ILogger logger)
            : base(device, networkAgent, logger)
        {
            _port = Config.GetConfig().RequestPort;
            _timeout = Config.GetConfig().ResponceTimeout;
        }

        public abstract byte StorageId { get; }

        public override void Execute()
        {
            var request = new GetIdListRequest()
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
                _networkAgent.Listen<IdListResponce, IdList>(_port, OnIdentityRecieved);
            }
            catch (Exception ex)
            {
                RaiseError(ex);
            }
        }

        private IdList _idList;
        private int _currentItemIndex = 0;

        private void OnIdentityRecieved(IdListResponce responce)
        {
            if (responce.Resultativity == Resultativity.Busy)
            {
                _logger.Debug(this, "Устройстов занято");
                ProcessStoreItemIndex();
                return;
            }
            if (responce.Resultativity != Resultativity.DataRequest
                || responce.Resultativity != Resultativity.Accepted)
            {
                _logger.Debug(this, $"Устройство вернуло код {responce.Resultativity}");
                RaiseError(new ExchangeException("Ошибка запроса данных"));
                return;
            }
            _idList = responce.Cast();
            if (StorageId != _idList.StorageId)
            {
                _logger.Debug(this, $"Устройство вернуло StorageId {_idList.StorageId}" +
                    $" вместо ожидаемого {StorageId}");
                RaiseError(new ExchangeException("Ошибка получения списка идентификаторов"));
                return;
            }
            _timer.Stop();
            _networkAgent.Close();
            CleanupItemListBeforeRecievingItems();
            if (!_idList.Items.Any())
            {
                RaiseSuccess();
                return;
            }
            ProcessStoreItemIndex();
        }

        private void ProcessStoreItemIndex()
        {
            var currentId = _idList.Items[_currentItemIndex];
            var request = new GetStorageItemRequest()
            {
                DeviceID = (ushort)_device.Id
            };
            request.SetRequestData(new StorageItemIndex
            {
                StorageId = _idList.StorageId,
                ItemId = currentId
            });
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
                _networkAgent.Listen<TStorageItemResponce, TStorageItem>(_port, OnItemRecieved);
            }
            catch (Exception ex)
            {
                RaiseError(ex);
            }
        }

        private void OnItemRecieved(TStorageItemResponce responce)
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
            var item = responce.Cast();
            ProcessRecievedItem(item);
            _timer.Stop();
            _networkAgent.Close();
            _currentItemIndex++;
            if (_currentItemIndex == _idList.Items.Count)
            {
                RaiseSuccess();
            }
            else
            {
                ProcessStoreItemIndex();
            }
        }

        public abstract void ProcessRecievedItem(TStorageItem item);
        public abstract void CleanupItemListBeforeRecievingItems();
    }
}
