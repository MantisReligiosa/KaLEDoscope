using System;
using System.Collections.Generic;
using System.Linq;
using BaseDevice;
using CommandProcessing.Commands;
using DeviceBuilding;
using ServiceInterfaces;

namespace CommandProcessing
{
    public class ConfigurationService
    {
        private readonly INetworkAgent _networkAgent;
        private readonly DeviceFactory _deviceFactory;
        private readonly Invoker _invoker;
        private readonly ILogger _logger;

        private readonly List<Func<Device, INetworkAgent, ILogger, IDeviceCommand<Device>>> _downloadingCommandContainer
            = new List<Func<Device, INetworkAgent, ILogger, IDeviceCommand<Device>>>
            {
                (d, n, l) => new DownloadIdentityCommand(d, n, l),
                (d, n, l) => new DownloadWorkScheduleCommand(d, n, l),
                (d, n, l) => new DownloadBrightnessCommand(d, n, l)
            };

        private readonly List<Func<Device, INetworkAgent, ILogger, IDeviceCommand<Device>>> _uploadingCommandContainer
            = new List<Func<Device, INetworkAgent, ILogger, IDeviceCommand<Device>>>
            {
                (d, n, l) => new UploadIdentityCommand(d, n, l)
            };

        public ConfigurationService(INetworkAgent networkAgent, DeviceFactory deviceFactory, ILogger logger)
        {
            _networkAgent = networkAgent;
            _deviceFactory = deviceFactory;
            _logger = logger;
            _invoker = new Invoker(_logger);
        }

        public void DownloadSettings(Device device)
        {
            _downloadingCommandContainer.AddRange(_deviceFactory.GetDownloadCommands(device.Model));
            _logger.Info(this, "Начало получения конфигурации от устройства");
            var command = _downloadingCommandContainer.First().Invoke(device, _networkAgent, _logger);
            ProcessDownloadCommand(command);
        }

        public void UploadSettings(Device device)
        {
            _uploadingCommandContainer.AddRange(_deviceFactory.GetUploadCommands(device.Model));
            _logger.Info(this, "Начало отправки конфигурации устройству");
            var command = _uploadingCommandContainer.First().Invoke(device, _networkAgent, _logger);
            ProcessUploadCommand(command);
        }

        private void ProcessUploadCommand(IDeviceCommand<Device> command)
        {
            command.Error += Command_Error;
            command.Success += UploadCommand_Success;
            _invoker.Invoke(command);
        }

        private int _counter = 0;
        private void ProcessDownloadCommand(IDeviceCommand<Device> command)
        {
            command.Error += Command_Error;
            command.Success += DownloadCommand_Success;
            _invoker.Invoke(command);
        }

        private void DownloadCommand_Success(object sender, SuccessCommendEventArgs e)
        {
            _counter++;
            if (_downloadingCommandContainer.Count > _counter)
            {
                var command = _downloadingCommandContainer.ElementAt(_counter).Invoke(e.Device, _networkAgent, _logger);
                ProcessDownloadCommand(command);
            }
            else
            {
                _logger.Info(this, "Конфигурация получена");
            }
        }

        private void UploadCommand_Success(object sender, SuccessCommendEventArgs e)
        {
            _counter++;
            if (_uploadingCommandContainer.Count > _counter)
            {
                var command = _uploadingCommandContainer.ElementAt(_counter).Invoke(e.Device, _networkAgent, _logger);
                ProcessUploadCommand(command);
            }
            else
            {
                _logger.Info(this, "Конфигурация отправлена");
            }
        }

        private void Command_Error(object sender, ExceptionEventArgs e)
        {
            _logger.Error(this, "Ошибка работы с конфигурацией устройства", e.Exception);
        }
    }
}