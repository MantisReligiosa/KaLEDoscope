using BaseDevice;
using DeviceBuilding;
using ServiceInterfaces;
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

        public Device DeserializeDevice(string text)
        {
            throw new NotImplementedException();
        }

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
                Content=new Label
                {
                    Content="Тут будет предпросмотр для часов"
                }
            };
            return pack;
        }

        public string SerializeDevice(Device device)
        {
            throw new NotImplementedException();
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
                    Mode = Mode.Auto
                },
                WorkSchedule = castedDevice?.WorkSchedule ?? new WorkSchedule(),
                BoardType = castedDevice?.BoardType ?? new BoardType(),
                StopWatchParameters = castedDevice?.StopWatchParameters ?? new StopWatchParameters(),
                TimeSyncParameters = castedDevice?.TimeSyncParameters ?? new TimeSyncParameters
                {
                    ServerAddress = string.Empty
                }
            };
            sevenSegmentBoard.Name = String.IsNullOrEmpty(device.Name) ? "Семисегментные часы" : device.Name;
            return sevenSegmentBoard;
        }
    }
}
