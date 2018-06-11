using BaseDevice;
using CommandProcessing;
using CommandProcessing.Requests;
using ServiceInterfaces;
using SevenSegmentBoardDevice.DTO;
using SevenSegmentBoardDevice.Responces;
using System.Collections.Generic;
using System.Linq;

namespace SevenSegmentBoardDevice.Commands
{
    public class DownloadFramesCommand : RequestingCommand<ConfigurationRequest, FramesResponce, List<ActiveFrameDTO>>
    {
        public DownloadFramesCommand(Device device, INetworkAgent networkAgent, ILogger logger)
            : base(device, networkAgent, logger) { }

        public override string Name => "Запрос списка фреймов";

        public override object GetRequestData() => 0x12;

        public override void ProcessRecievedData(List<ActiveFrameDTO> responceDTO)
        {
            var device = _device as SevenSegmentBoard;
            foreach (var frame in Refs.DisplayFrames)
            {
                var activeFrame = responceDTO.FirstOrDefault(f => f.Id == frame.Id);
                if (activeFrame != null)
                {
                    frame.IsChecked = true;
                    frame.DisplayPeriod = activeFrame.Duration;
                }
                else
                {
                    frame.IsChecked = false;
                }
                device.DisplayFrames.Add(frame);
            }
            _device = device;
        }
    }
}
