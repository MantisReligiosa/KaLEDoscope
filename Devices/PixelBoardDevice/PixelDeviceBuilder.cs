using PixelBoardDevice.DomainObjects;
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

        public ControlsPack GetControls(Device device, ILogger logger)
        {
            var model = new PixelDeviceViewModel(device, logger);
            var pack = new ControlsPack();
            var pixelControl = new PixelControl
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                DataContext = model
            };
            pack.CustomizationControls = new Dictionary<string, UserControl>
            {
                { "Пиксельная плата",pixelControl }
            };
            return pack;
        }

        public Device UpdateCustomSettings(Device device)
        {
            var castedDevice = device as PixelBoard;
            var pixelBoard = new PixelBoard
            {
                Id = device.Id,
                Model = device.Model,
                IsStandaloneConfiguration = device.IsStandaloneConfiguration,
                Network = device.Network,
                Brightness = castedDevice?.Brightness ?? new Brightness
                {
                    BrightnessPeriods = new List<BrightnessPeriod>(),
                    Mode = Mode.Auto
                },
                WorkSchedule = castedDevice?.WorkSchedule ?? new WorkSchedule(),
                BoardSize = castedDevice?.BoardSize ?? new BoardSize
                {
                    Height = 16,
                    Width = 160
                },
                Fonts = castedDevice?.Fonts ?? new List<BinaryFont>(),
                Screens = castedDevice?.Screens ?? new List<Screen>
                {
                    new Screen
                    {
                        Order = 1,
                        Period = 10,
                        Name = "Экран1",
                        Zones = new List<Zone>()
                    }
                },
                Name = String.IsNullOrEmpty(device.Name) ? "Пиксельная плата" : device.Name
            };
            return pixelBoard;
        }
    }
}
