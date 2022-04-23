using BaseDevice;
using DeviceBuilding;
using ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandProcessing
{
    public class ConfigurationService
    {
        public event EventHandler<IDeviceCommand<Device>> CommandExecuted;

        private readonly INetworkAgent _networkAgent;
        private readonly IDeviceFactory _deviceFactory;
        private readonly ILogger _logger;
        private readonly IConfig _config;

        private readonly List<Func<Device, INetworkAgent, ILogger, IConfig, IDeviceCommand<Device>>> _uploadingCommandContainer;

        public ConfigurationService(List<Func<Device, INetworkAgent, ILogger, IConfig, IDeviceCommand<Device>>> uploadingCommandContainer, INetworkAgent networkAgent, IDeviceFactory deviceFactory, ILogger logger, IConfig config)
        {
            _networkAgent = networkAgent;
            _deviceFactory = deviceFactory;
            _logger = logger;
            _config = config;
            _uploadingCommandContainer = uploadingCommandContainer;
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
            command.Repeat += (o, e) =>
            {
                _logger.Info(this, "Устройство занято. Повторная отправка");
                command.Execute();
            };
            command.Execute();
        }

        private int _counter = 0;
        private void UploadCommand_Success(object sender, SuccessCommandEventArgs e)
        {
            CommandExecuted?.Invoke(this, e.Command);
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
