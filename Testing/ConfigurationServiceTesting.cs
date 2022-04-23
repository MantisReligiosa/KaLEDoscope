using BaseDevice;
using CommandProcessing;
using CommandProcessing.Responces;
using DeviceBuilding;
using NSubstitute;
using ServiceInterfaces;
using System;
using System.Collections.Generic;
using Xunit;

namespace Testing
{
    public class ConfigurationServiceTesting
    {
        private readonly List<Func<Device, INetworkAgent, ILogger, IConfig, IDeviceCommand<Device>>> _uploadingCommandsContainer;
        private readonly INetworkAgent _networkAgent;
        private readonly IDeviceFactory _deviceFactory;
        private readonly ILogger _logger;
        private readonly IConfig _config;
        private readonly Device _device;

        public ConfigurationServiceTesting()
        {
            _uploadingCommandsContainer = new List<Func<Device, INetworkAgent, ILogger, IConfig, IDeviceCommand<Device>>>
            {
                (d, n, l, c) => new FirstCommand(d, n, l, c),
                (d, n, l, c) => new SecondCommand(d, n, l, c),
                (d, n, l, c) => new ThirdCommand(d, n, l, c)
            };

            _networkAgent = Substitute.For<INetworkAgent>();

            _deviceFactory = Substitute.For<IDeviceFactory>();
            _deviceFactory.GetUploadCommands(Arg.Any<string>())
                .Returns(new List<Func<Device, INetworkAgent, ILogger, IConfig, IDeviceCommand<Device>>>());

            _logger = Substitute.For<ILogger>();
            _config = Substitute.For<IConfig>();
            _config.ResponceTimeout.Returns(100);
            _device = new Device
            {
                Id = 1,
                Network = new Network
                {
                    IpAddress = "1.1.1.1"
                }
            };
        }

        [Fact]
        public void TestBusyResponce()
        {
            var service = new ConfigurationService(_uploadingCommandsContainer, _networkAgent, _deviceFactory, _logger, _config);

            var commandCounter = 0;
            service.CommandExecuted += (s, e) => commandCounter++;

            var sendedAttempts = new Dictionary<Type, int>();
            IRequest currentRequest = null;

            _networkAgent.When(x => x.Send(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<IRequest>())).Do(x =>
            {
                currentRequest = x.Arg<IRequest>();
                if (!sendedAttempts.ContainsKey(currentRequest.GetType()))
                {
                    sendedAttempts[currentRequest.GetType()] = 0;
                }
                else
                {
                    sendedAttempts[currentRequest.GetType()] = sendedAttempts[currentRequest.GetType()] + 1;
                }
            });

            _networkAgent.When(x => x.Listen<AcceptanceResponce, object>(Arg.Any<int>(), Arg.Any<Action<AcceptanceResponce>>())).Do(x =>
            {
                var currentAttempt = sendedAttempts[currentRequest.GetType()];
                var action = x.Arg<Action<AcceptanceResponce>>();
                var responce = new AcceptanceResponce();
                if (currentAttempt < 3)
                {
                    responce.SetByteSequence(new byte[]
                    {
                    0x00, 0xFF, 0xF1, 0x00, 0x00
                    });
                }
                else
                {
                    responce.SetByteSequence(new byte[]
                    {
                    0x00, 0xFF, 0xF0, 0x00, 0x00
                    });
                }
                action.Invoke(responce);
            });

            service.UploadSettings(_device);

            Assert.Equal(3, commandCounter);
        }
    }

    internal class FirstCommand : RequestingCommand<FirstCommandRequest, AcceptanceResponce, object>
    {
        public FirstCommand(Device device, INetworkAgent networkAgent, ILogger logger, IConfig config)
            : base(device, networkAgent, logger, config) { }

        public override string Name => "1";

        public override object GetRequestData() => 1;

        public override void ProcessRecievedData(object responceDTO) { }
    }

    internal class SecondCommand : RequestingCommand<SecondCommandRequest, AcceptanceResponce, object>
    {
        public SecondCommand(Device device, INetworkAgent networkAgent, ILogger logger, IConfig config)
            : base(device, networkAgent, logger, config) { }

        public override string Name => "2";

        public override object GetRequestData() => 2;

        public override void ProcessRecievedData(object responceDTO) { }
    }

    internal class ThirdCommand : RequestingCommand<ThirdCommandRequest, AcceptanceResponce, object>
    {
        public ThirdCommand(Device device, INetworkAgent networkAgent, ILogger logger, IConfig config)
            : base(device, networkAgent, logger, config) { }

        public override string Name => "3";

        public override object GetRequestData() => 3;

        public override void ProcessRecievedData(object responceDTO) { }
    }

    internal class FirstCommandRequest : Request
    {
        public override byte RequestID => 0x01;

        public override byte[] MakeData(object o) => new byte[] { (byte)o };
    }

    internal class SecondCommandRequest : Request
    {
        public override byte RequestID => 0x02;

        public override byte[] MakeData(object o) => new byte[] { (byte)o };
    }

    internal class ThirdCommandRequest : Request
    {
        public override byte RequestID => 0x03;

        public override byte[] MakeData(object o) => new byte[] { (byte)o };
    }
}
