using BaseDevice;
using CommandProcessing.Requests;
using CommandProcessing.Responces;
using ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Timers;

namespace CommandProcessing.Commands
{
    public class ScanCommand : DeviceCommand<Device>
    {
        private readonly int _port;
        private readonly int _timeout;
        private readonly List<Device> _devices;

        public event Action<List<Device>> OnScanCompleted;
        public override string Name => "Поиск устройств";

        public ScanCommand(INetworkAgent networkAgent, ILogger logger, IConfig config)
            : base(null, networkAgent, logger, config)
        {
            _devices = new List<Device>();
            _port = config.ScanPort;
            _timeout = config.ScanPeriod;
        }

        public override void Execute()
        {
            var request = new ScanRequest();
            _networkAgent.SendBroadcast(_port, request);
            _logger.Debug(this, $"Жду ответы {_timeout} мс");
            var timer = new Timer()
            {
                AutoReset = false,
                Interval = _timeout
            };
            timer.Elapsed += (o, e) =>
            {
                _networkAgent.Close();
                _logger.Info(this, $"Поиск завершен");
                OnScanCompleted?.Invoke(_devices);
            };
            timer.Start();

            /*
             //Имитация устройства
            var tmp = new ScanResponce();
            tmp.SetByteSequence(new byte[]{
                0xAB, 0xCD, 0x01, 0x00, 0x1F, 0xC0, 0xA8, 0x00, 0x46, 0x01, 0xF4, 0x0A, 0x71, 0x69, 0x78, 0x65,
                0x6C, 0x42, 0x6F, 0x61, 0x72, 0x64, 0xAA, 0x00, 0x05, 0x0A, 0x64, 0x65, 0x76, 0x69, 0x63, 0x65,
                0x4E, 0x61, 0x6D, 0x65
            });
            OnDeviceAnswered(tmp);
            */

            StartListen();
        }

        private void StartListen()
        {
            try
            {
                _networkAgent.Listen<ScanResponce, Device>(_port, OnDeviceAnswered);

            }
            catch (Exception ex)
            {
                RaiseError(ex);
            }
        }

        private void OnDeviceAnswered(ScanResponce responce)
        {
            var device = responce.Cast();
            _devices.Add(device);
            StartListen();
        }
    }
}
