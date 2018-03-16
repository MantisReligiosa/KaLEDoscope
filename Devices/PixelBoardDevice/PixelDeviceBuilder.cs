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

        public Dictionary<string, UserControl> GetControls(Device device, ILogger logger)
        {
            var model = new PixelDeviceViewModel(device, logger);
            var pixelControl = new PixelControl
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                DataContext = model
            };
            model.OnNeedRedraw += pixelControl.OnNeedRedraw;
            model.DeviceHeight = 200;
            model.DeviceWidth = 800;
            return new Dictionary<string, UserControl>
            {
                { "Пиксельная плата",pixelControl }
            };
        }

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
                BoardSize = new BoardSize
                {
                    Height = 80,
                    Width = 120
                },
                Fonts = new List<BinaryFont>(),
                Screens = new List<Screen>
                {
                    new Screen
                    {
                        Order=1,
                        Period=10,
                        Zones=new List<Zone>()
                    }
                }
            };
            pixelBoard.Name = String.IsNullOrEmpty(device.Name) ? "Пиксельная плата" : device.Name;
            return pixelBoard;
        }
    }
}
