using BaseDevice;
using CommandProcessing.Commands;
using DeviceBuilding;
using ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandProcessing
{
    public class ConfigurationService
    {
        private readonly INetworkAgent _networkAgent;
        private readonly DeviceFactory _deviceFactory;
        private readonly Invoker _invoker;
        private readonly ILogger _logger;
        private readonly IConfig _config;

        private readonly List<Func<Device, INetworkAgent, ILogger, IConfig, IDeviceCommand<Device>>> _downloadingCommandContainer
            = new List<Func<Device, INetworkAgent, ILogger, IConfig, IDeviceCommand<Device>>>
            {
                (d, n, l, c) => new DownloadIdentityCommand(d, n, l, c),
                (d, n, l, c) => new DownloadNetworkCommand(d, n, l, c),
                (d, n, l, c) => new DownloadWorkScheduleCommand(d, n, l, c),
                (d, n, l, c) => new DownloadBrightnessCommand(d, n, l, c)
            };

        private readonly List<Func<Device, INetworkAgent, ILogger, IConfig, IDeviceCommand<Device>>> _uploadingCommandContainer
            = new List<Func<Device, INetworkAgent, ILogger, IConfig, IDeviceCommand<Device>>>
            {
                (d, n, l, c) => new UploadIdentityCommand(d, n, l, c),
                (d, n, l, c) => new UploadNetworkCommand(d, n, l, c),
                (d, n, l, c) => new UploadWorkScheduleCommand(d, n, l, c),
                (d, n, l, c) => new UploadBrightnessCommand(d, n, l, c)
            };

        public ConfigurationService(INetworkAgent networkAgent, DeviceFactory deviceFactory, ILogger logger, IConfig config)
        {
            _networkAgent = networkAgent;
            _deviceFactory = deviceFactory;
            _logger = logger;
            _config = config;
            _invoker = new Invoker(_logger);
        }

        public void DownloadSettings(Device device)
        {
            _downloadingCommandContainer.AddRange(_deviceFactory.GetDownloadCommands(device.Model));
            _logger.Info(this, "Начало получения конфигурации от устройства");
            var command = _downloadingCommandContainer.First().Invoke(device, _networkAgent, _logger, _config);
            ProcessDownloadCommand(command);
        }

        public void UploadSettings(Device device)
        {
            _uploadingCommandContainer.AddRange(_deviceFactory.GetUploadCommands(device.Model));
            _logger.Info(this, "Начало отправки конфигурации устройству");
            var command = _uploadingCommandContainer.First().Invoke(device, _networkAgent, _logger, _config);
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

        private void DownloadCommand_Success(object sender, SuccessCommandEventArgs e)
        {
            _counter++;
            if (_downloadingCommandContainer.Count > _counter)
            {
                var command = _downloadingCommandContainer.ElementAt(_counter).Invoke(e.Device, _networkAgent, _logger, _config);
                ProcessDownloadCommand(command);
            }
            else
            {
                _logger.Info(this, "Конфигурация получена");
            }
        }

        private void UploadCommand_Success(object sender, SuccessCommandEventArgs e)
        {
            _counter++;
            if (_uploadingCommandContainer.Count > _counter)
            {
                var command = _uploadingCommandContainer.ElementAt(_counter).Invoke(e.Device, _networkAgent, _logger, _config);
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
