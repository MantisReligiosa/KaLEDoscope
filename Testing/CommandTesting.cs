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

            var initialPort = 10;
            var editedPort = 12;

            var device = new Device();
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
            Assert.Equal(default(int), device.Network.EditedPort);

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

            cmd.Execute();

            Assert.Equal(editedIp, device.Network.IpAddress);
            Assert.Equal(editedIp, device.Network.ActualIpAddress);
            Assert.Equal(string.Empty, device.Network.EditedIpAddress);

            Assert.Equal(editedPort, device.Network.Port);
            Assert.Equal(editedPort, device.Network.ActualPort);
            Assert.Equal(default(int), device.Network.EditedPort);
        }
    }
}
