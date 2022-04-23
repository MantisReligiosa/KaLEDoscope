using BaseDevice;
using CommandProcessing.Commands;
using CommandProcessing.Responces;
using NSubstitute;
using ServiceInterfaces;
using System;
using Xunit;

namespace Testing
{
    public class CommandTesting
    {
        [Fact]
        public void UploadNetworkCommandTesting()
        {
            var initialIp = "initialIp";
            var editedIp = "editedIp";

            var editedPort = 12;

            var device = new Device();
            var initialPort = device.Network.Port;
            device.Network.IpAddress = initialIp;

            Assert.Equal(initialIp, device.Network.IpAddress);
            Assert.Equal(initialIp, device.Network.ActualIpAddress);
            Assert.Equal(string.Empty, device.Network.EditedIpAddress);

            device.Network.IpAddress = editedIp;

            Assert.Equal(editedIp, device.Network.IpAddress);
            Assert.Equal(initialIp, device.Network.ActualIpAddress);
            Assert.Equal(editedIp, device.Network.EditedIpAddress);


            device.Network.Port = initialPort;

            Assert.Equal(initialPort, device.Network.Port);
            Assert.Equal(initialPort, device.Network.ActualPort);
            Assert.Equal(initialPort, device.Network.EditedPort);

            device.Network.Port = editedPort;

            Assert.Equal(editedPort, device.Network.Port);
            Assert.Equal(initialPort, device.Network.ActualPort);
            Assert.Equal(editedPort, device.Network.EditedPort);


            var networkAgent = Substitute.For<INetworkAgent>();


            var logger = Substitute.For<ILogger>();
            var config = Substitute.For<IConfig>();
            config.ResponceTimeout = 5;
            var cmd = new UploadNetworkCommand(device, networkAgent, logger, config);

            networkAgent.WhenForAnyArgs(n => n.Listen<AcceptanceResponce, object>(Arg.Any<int>(), Arg.Any<Action<AcceptanceResponce>>()))
                .Do(n =>
                {
                    var responce = new AcceptanceResponce();
                    responce.SetByteSequence(new byte[]
                    {
                       0x00, 0xFF, 0xF0, 0x00, 0x00
                    });
                    cmd.OnIdentityRecieved(responce);
                });

            networkAgent.WhenForAnyArgs(n => n.Send(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<IRequest>()))
                .Do(x =>
                {
                    Assert.Equal(initialIp, x.Arg<string>());
                    Assert.Equal(initialPort, x.Arg<int>());
                });

            cmd.Execute();

            Assert.Equal(editedIp, device.Network.IpAddress);
            Assert.Equal(editedIp, device.Network.ActualIpAddress);
            Assert.Equal(string.Empty, device.Network.EditedIpAddress);

            Assert.Equal(editedPort, device.Network.Port);
            Assert.Equal(editedPort, device.Network.ActualPort);
            Assert.Equal(default, device.Network.EditedPort);
        }

        [Fact]
        public void UploadNetworkCommandTesting_WithNoChanges()
        {
            var initialIp = "initialIp";

            var device = new Device();
            var initialPort = device.Network.Port;
            device.Network.IpAddress = initialIp;

            Assert.Equal(initialIp, device.Network.IpAddress);
            Assert.Equal(initialIp, device.Network.ActualIpAddress);
            Assert.Equal(string.Empty, device.Network.EditedIpAddress);

            device.Network.Port = initialPort;

            Assert.Equal(initialPort, device.Network.Port);
            Assert.Equal(initialPort, device.Network.ActualPort);
            Assert.Equal(initialPort, device.Network.EditedPort);

            var networkAgent = Substitute.For<INetworkAgent>();


            var logger = Substitute.For<ILogger>();
            var config = Substitute.For<IConfig>();
            config.ResponceTimeout = 5;
            var cmd = new UploadNetworkCommand(device, networkAgent, logger, config);

            networkAgent.WhenForAnyArgs(n => n.Listen<AcceptanceResponce, object>(Arg.Any<int>(), Arg.Any<Action<AcceptanceResponce>>()))
                .Do(n =>
                {
                    var responce = new AcceptanceResponce();
                    responce.SetByteSequence(new byte[]
                    {
                       0x00, 0xFF, 0xF0, 0x00, 0x00
                    });
                    cmd.OnIdentityRecieved(responce);
                });

            networkAgent.WhenForAnyArgs(n => n.Send(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<IRequest>()))
                .Do(x =>
                {
                    Assert.Equal(initialIp, x.Arg<string>());
                    Assert.Equal(initialPort, x.Arg<int>());
                });

            cmd.Execute();

            Assert.Equal(initialIp, device.Network.IpAddress);
            Assert.Equal(initialIp, device.Network.ActualIpAddress);
            Assert.Equal(string.Empty, device.Network.EditedIpAddress);

            Assert.Equal(initialPort, device.Network.Port);
            Assert.Equal(initialPort, device.Network.ActualPort);
            Assert.Equal(default, device.Network.EditedPort);
        }

        [Fact]
        public void UploadWorkScheduleTesting()
        {
            var ipAddress = "initialIp";

            var port = 10;

            var device = new Device();
            var actualPort = device.Network.ActualPort;
            device.Network.IpAddress = ipAddress;

            Assert.Equal(ipAddress, device.Network.IpAddress);
            Assert.Equal(ipAddress, device.Network.ActualIpAddress);

            device.Network.Port = port;

            Assert.Equal(port, device.Network.Port);
            Assert.Equal(actualPort, device.Network.ActualPort);

            var networkAgent = Substitute.For<INetworkAgent>();

            var logger = Substitute.For<ILogger>();
            var config = Substitute.For<IConfig>();
            config.ResponceTimeout = 5;
            var cmd = new UploadWorkScheduleCommand(device, networkAgent, logger, config);

            networkAgent.WhenForAnyArgs(n => n.Listen<AcceptanceResponce, object>(Arg.Any<int>(), Arg.Any<Action<AcceptanceResponce>>()))
                .Do(n =>
                {
                    var responce = new AcceptanceResponce();
                    responce.SetByteSequence(new byte[]
                    {
                       0x00, 0xFF, 0xF0, 0x00, 0x00
                    });
                    cmd.OnIdentityRecieved(responce);
                });

            networkAgent.WhenForAnyArgs(n => n.Send(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<IRequest>()))
                .Do(x =>
                {
                    Assert.Equal(ipAddress, x.Arg<string>());
                    Assert.Equal(port, x.Arg<int>());
                });

            cmd.Execute();
        }

        [Fact]
        public void SyncTimeTesting()
        {
            var device = new Device();

            var networkAgent = Substitute.For<INetworkAgent>();


            var logger = Substitute.For<ILogger>();
            var config = Substitute.For<IConfig>();
            config.ResponceTimeout = 5;
            var cmd = new SyncTimeCommand(device, networkAgent, logger, config);

            networkAgent.WhenForAnyArgs(n => n.Listen<AcceptanceResponce, object>(Arg.Any<int>(), Arg.Any<Action<AcceptanceResponce>>()))
                .Do(n =>
                {
                    var responce = new AcceptanceResponce();
                    responce.SetByteSequence(new byte[]
                    {
                       0x00, 0xFF, 0xF0, 0x00, 0x00
                    });
                    cmd.OnIdentityRecieved(responce);
                });

            networkAgent.WhenForAnyArgs(n => n.Send(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<IRequest>()))
                .Do(x =>
                {
                });

            cmd.Execute();
        }
    }
}
