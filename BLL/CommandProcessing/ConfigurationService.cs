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

        private readonly List<Func<Device, INetworkAgent, ILogger, IDeviceCommand<Device>>> _downloadingCommandProvider
            = new List<Func<Device, INetworkAgent, ILogger, IDeviceCommand<Device>>>
            {
                (d, n, l) => new IdentityCommand(d, n, l),
                (d, n, l) => new WorkScheduleCommand(d, n, l),
                (d, n, l) => new BrightnessCommand(d, n, l)
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
            _downloadingCommandProvider.AddRange(_deviceFactory.GetDownloadCommands(device.Model));
            _logger.Info(this, "Начало загрузки конфигурации");
            var command = _downloadingCommandProvider.First().Invoke(device, _networkAgent, _logger);
            ProcessCommand(command);
        }

        private int _counter = 0;
        private void ProcessCommand(IDeviceCommand<Device> command)
        {
            command.Error += Command_Error;
            command.Success += Command_Success;
            _invoker.Invoke(command);
        }

        private void Command_Success(object sender, SuccessCommendEventArgs e)
        {
            _counter++;
            if (_downloadingCommandProvider.Count > _counter)
            {
                var command = _downloadingCommandProvider.ElementAt(_counter).Invoke(e.Device, _networkAgent, _logger);
                ProcessCommand(command);
            }
            else
            {
                _logger.Info(this, "Конфигурация загружена");
            }
        }

        private void Command_Error(object sender, ExceptionEventArgs e)
        {
            _logger.Error(this, "Ошибка получения конфигурации устройства", e.Exception);
        }
    }
}