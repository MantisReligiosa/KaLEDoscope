using BaseDevice;
using CommandProcessing;
using CommandProcessing.Responces;
using DeviceBuilding;
using NSubstitute;
using PixelBoardDevice;
using PixelBoardDevice.DomainObjects;
using PixelBoardDevice.DomainObjects.Zones;
using ServiceInterfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace PixelBoardDeviceTesting
{
    public class ConfigurationServiceTesting
    {
        private readonly INetworkAgent _networkAgent;
        private readonly IDeviceFactory _deviceFactory;
        private readonly IDeviceBuilder _deviceBuilder;
        private readonly ILogger _logger;
        private readonly IConfig _config;

        public ConfigurationServiceTesting()
        {

            _networkAgent = Substitute.For<INetworkAgent>();

            _deviceFactory = new DeviceFactory(_logger);
            _deviceBuilder = new PixelDeviceBuilder();
            _deviceFactory.AddBuilder(_deviceBuilder);

            _logger = Substitute.For<ILogger>();
            _config = Substitute.For<IConfig>();
            _config.ResponceTimeout.Returns(100);
        }

        [Theory]
        [ClassData(typeof(PixelBoards))]
        public void TestBusyResponce(PixelBoard device)
        {
            var commands = _deviceFactory.GetUploadCommands(_deviceBuilder.Model);
            var service = new ConfigurationService(commands, _networkAgent, _deviceFactory, _logger, _config);

            var commandCounter = 0;
            service.CommandExecuted += (s, e) => commandCounter++;

            var sendedCommands = new Dictionary<Type, int>();
            device.Model = _deviceBuilder.Model;
            IRequest currentRequest = null;

            _networkAgent.When(x => x.Send(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<IRequest>())).Do(x =>
            {
                currentRequest = x.Arg<IRequest>();
                if (!sendedCommands.ContainsKey(currentRequest.GetType()))
                {
                    sendedCommands[currentRequest.GetType()] = 0;
                }
                else
                {
                    sendedCommands[currentRequest.GetType()] = sendedCommands[currentRequest.GetType()] + 1;
                }
            });

            _networkAgent.When(x => x.Listen<AcceptanceResponce, object>(Arg.Any<int>(), Arg.Any<Action<AcceptanceResponce>>())).Do(x =>
            {
                var currentAttempt = sendedCommands[currentRequest.GetType()];
                var action = x.Arg<Action<AcceptanceResponce>>();
                var responce = new AcceptanceResponce();
                if (currentAttempt < 3)
                {
                    responce.SetByteSequence(ResponceBytes.Busy);
                }
                else
                {
                    responce.SetByteSequence(ResponceBytes.Accepted);
                }
                action.Invoke(responce);
            });

            service.UploadSettings(device);

            Assert.Equal(12, commandCounter);
        }
    }

    public class PixelBoards : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] {
                new PixelBoard
                {
                    Id = 1,
                    Network = new Network
                    {
                        IpAddress = "1.1.1.1"
                    },
                    Fonts = new List<BinaryFont>
                    {
                        new BinaryFont {Id=1, Alphabet = new Glyph[] { new Glyph {Image = new byte[] {0x01 },Symbol = 'A',Width = 12 } }, GlyphHeight = 12, Source = "Font" }
                    },
                    Programs = new List<Program>
                    {
                        new Program {Id = 1, Name = "Program1", Order = 1, Period = 1, Zones = new List<Zone>{ new TextZone {Id = 1, ProgramId = 1,FontId = 1, IsValid = true } } },
                        new Program {Id = 1, Name = "Program1", Order = 1, Period = 1, Zones = new List<Zone>() }
                    }
                }
            };

            yield return new object[] {
                new PixelBoard
                {
                    Id = 1,
                    Network = new Network
                    {
                        IpAddress = "1.1.1.1"
                    },
                    Fonts = new List<BinaryFont>(),
                    Programs = new List<Program>()
                }
            };

            yield return new object[] {
                new PixelBoard
                {
                    Id = 1,
                    Network = new Network
                    {
                        IpAddress = "1.1.1.1"
                    }
                }
            };

        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
