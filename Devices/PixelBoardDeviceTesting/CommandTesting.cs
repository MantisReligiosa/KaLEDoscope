using BaseDevice;
using NSubstitute;
using PixelBoardDevice.Commands;
using PixelBoardDevice.DTO;
using PixelBoardDevice.Responces;
using ServiceInterfaces;
using System;
using Xunit;

namespace PixelBoardDeviceTesting
{
    public class CommandTesting
    {
        private readonly Device _device;
        private readonly INetworkAgent _networkAgent;
        private readonly ILogger _logger;
        private readonly IConfig _config;

        public CommandTesting()
        {
            _device = new Device()
            {
                Id = 256
            };
            _networkAgent = Substitute.For<INetworkAgent>();
            _logger = Substitute.For<ILogger>();
            _config = Substitute.For<IConfig>();
            _config.ResponceTimeout = int.MaxValue;
        }

        [Fact]
        public void GetFontsTesting()
        {
            _networkAgent.When(x => x.Send(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<IRequest>())).Do(x =>
                {
                    var request = x.Arg<IRequest>();
                    var bytes = request.GetBytes();
                    Assert.Equal(new byte[]{
                        0x01, 0x00, 0x22, 0x00, 0x01, 0x01
                    }, request.GetBytes());
                });
            _networkAgent.When(x => x.Listen<IdListResponce, IdList>(Arg.Any<int>(), Arg.Any<Action<IdListResponce>>())).Do(x =>
            {
                var action = x.Arg<Action<IdListResponce>>();
                var responce = new IdListResponce();
                action.Invoke(responce);
            });
            var cmd = new DownloadFontsCommand(_device, _networkAgent, _logger, _config);
            cmd.Execute();
        }
    }
}
