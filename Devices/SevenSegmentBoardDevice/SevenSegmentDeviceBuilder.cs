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

        Dictionary<string, Func<Device, ILogger, UserControl>> IDeviceBuilder.Controls => new Dictionary<string, Func<Device, ILogger, UserControl>>
            {
                {"Настройки часов",(d,l)=>   new TimerControl
                            {
                                HorizontalAlignment = HorizontalAlignment.Stretch,
                                VerticalAlignment = VerticalAlignment.Stretch,
                                DataContext = new TimerDeviceViewModel(d, l)
                            }
                }
        };

        public Device UpdateCustomSettings(Device device)
        {
            var castedDevice = device as SevenSegmentBoard;
            var sevenSegmentBoard = new SevenSegmentBoard
            {
                Id = device.Id,
                AlarmSchedule = castedDevice?.AlarmSchedule ?? new List<Alarm>(),
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
