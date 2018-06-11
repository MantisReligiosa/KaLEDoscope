using BaseDevice;
using DeviceBuilding;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServiceInterfaces;
using SevenSegmentBoardDevice.Commands;
using SevenSegmentBoardDevice.Serialization;
using SevenSegmentBoardDevice.UI;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace SevenSegmentBoardDevice
{
    public class SevenSegmentDeviceBuilder : IDeviceBuilder
    {
        public string Model => "boardClock";
        public string DisplayName => "Семисегментные часы";

        public ControlsPack GetControlsPack(Device device, ILogger logger)
        {
            var pack = new ControlsPack();
            var model = new TimerDeviceViewModel(device, logger);
            var timerControl = new TimerControl
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                DataContext = model
            };
            pack.CustomizationControl = timerControl;
            pack.PreviewControl = new UserControl
            {
                Content = new Label
                {
                    Content = "Тут будет предпросмотр для часов"
                }
            };
            return pack;
        }

        public Device UpdateCustomSettings(Device device)
        {
            var castedDevice = device as SevenSegmentBoard;
            var sevenSegmentBoard = new SevenSegmentBoard
            {
                Id = device.Id,
                AlarmSchedule = castedDevice?.AlarmSchedule ?? new List<Alarm>(),
                IsStandaloneConfiguration = device.IsStandaloneConfiguration,
                Model = device.Model,
                Network = device.Network,
                Brightness = castedDevice?.Brightness ?? new Brightness
                {
                    BrightnessPeriods = new List<BrightnessPeriod>(),
                    Mode = BrightnessMode.Auto
                },
                WorkSchedule = castedDevice?.WorkSchedule ?? new WorkSchedule(),
                BoardType = castedDevice?.BoardType ?? new BoardType(),
                StopWatchParameters = castedDevice?.StopWatchParameters ?? new StopWatchParameters(),
                TimeSyncParameters = castedDevice?.TimeSyncParameters ?? new TimeSyncParameters
                {
                    ServerAddress = string.Empty
                }
            };
            sevenSegmentBoard.Name = String.IsNullOrEmpty(device.Name) ? DisplayName : device.Name;
            return sevenSegmentBoard;
        }

        public string SerializeDevice(Device device)
        {
            var serializablePixelDevice = GetSerializable(device);
            return JsonConvert.SerializeObject(serializablePixelDevice);
        }

        public Device DeserializeDevice(string text)
        {
            var serializablePixelDevice = JsonConvert.DeserializeObject<SerializableSevenSegmentDevice>(text);
            return FromSerializable(serializablePixelDevice);
        }

        public object GetSerializable(Device device) => (SerializableSevenSegmentDevice)(SevenSegmentBoard)device;

        public Device FromSerializable(object serializableDevice)
        {
            if (serializableDevice is SerializableSevenSegmentDevice serializableSevenSegmentDevice)
            {
                return (SevenSegmentBoard)serializableSevenSegmentDevice;
            }
            if (serializableDevice is JObject jObject)
            {
                return (SevenSegmentBoard)(jObject.ToObject<SerializableSevenSegmentDevice>());
            }
            return null;
        }

        public IEnumerable<Func<Device, INetworkAgent, ILogger, IDeviceCommand<Device>>> GetDownloadCommands()
            => new List<Func<Device, INetworkAgent, ILogger, IDeviceCommand<Device>>>
            {
                (d, n, l) => new DownloadBoardTypeCommand(d, n, l),
                (d, n, l) => new DownloadAlarmsCommand(d, n, l),
                (d, n, l) => new DownloadFramesCommand(d, n, l),
                (d, n, l) => new DownloadTimeSyncConfigCommand(d, n, l)
            };

        public IEnumerable<Func<Device, INetworkAgent, ILogger, IDeviceCommand<Device>>> GetUploadCommands()
            => new List<Func<Device, INetworkAgent, ILogger, IDeviceCommand<Device>>>
            {
                (d, n, l) => new UploadBoardTypeCommand(d, n, l),
                (d, n, l) => new UploadAlarmsCommand(d, n, l),
                (d, n, l) => new UploadFramesCommand(d, n, l),
                (d, n, l) => new UploadTimeSyncConfigCommand(d, n, l)
            };
    }
}
