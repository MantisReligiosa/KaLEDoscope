using BaseDevice;
using DeviceBuilding;
using PixelBoardDevice.UI;
using ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace PixelBoardDevice
{
    public class PixelDeviceBuilder : IDeviceBuilder
    {
        public string Model => "pixelBoard";

        public Dictionary<string, Func<Device, ILogger, UserControl>> Controls => new Dictionary<string, Func<Device, ILogger, UserControl>>
        {
                {"Пиксельная плата",(d,l)=>   new PixelControl
                            {
                                HorizontalAlignment = HorizontalAlignment.Stretch,
                                VerticalAlignment = VerticalAlignment.Stretch,
                                DataContext = new PixelDeviceViewModel(d, l)
                            }
                }
        };

        public Device UpdateCustomSettings(Device device)
        {
            var castedDevice = device as PixelBoard;
            var pixelBoard = new PixelBoard
            {
                Id = device.Id,
                Model = device.Model,
                Network = device.Network,
                Brightness = castedDevice?.Brightness ?? new Brightness
                {
                    BrightnessPeriods = new List<BrightnessPeriod>(),
                    Mode = Mode.Auto
                },
                WorkSchedule = castedDevice?.WorkSchedule ?? new WorkSchedule(),
            };
            pixelBoard.Name = String.IsNullOrEmpty(device.Name) ? "Пиксельная плата" : device.Name;
            return pixelBoard;
        }
    }
}
