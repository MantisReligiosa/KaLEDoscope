using PixelBoardDevice.DomainObjects;
using BaseDevice;
using DeviceBuilding;
using PixelBoardDevice.UI;
using ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Windows;
using PixelBoardDevice.Serialization;
using Newtonsoft.Json;

namespace PixelBoardDevice
{
    public class PixelDeviceBuilder : IDeviceBuilder
    {
        public string Model => "pixelBoard";

        public ControlsPack GetControlsPack(Device device, ILogger logger)
        {
            var _model = new PixelDeviceViewModel(device, logger);
            var pack = new ControlsPack
            {
                CustomizationControl = new PixelControl
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    DataContext = _model
                }
            };
            var _previewControl = new ProgramPreviewControl
            {
                DataContext = new ProgramPreviewViewModel(_model)
            };
            pack.PreviewControl = _previewControl;
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
                Programs = castedDevice?.Programs ?? new List<Program>
                {
                    new Program
                    {
                        Order = 1,
                        Period = 10,
                        Name = "Программа1",
                        Zones = new List<Zone>()
                    }
                },
                Name = String.IsNullOrEmpty(device.Name) ? "Электронное табло" : device.Name
            };
            return pixelBoard;
        }

        public string SerializeDevice(Device device)
        {
            var pixelBoard = (PixelBoard)device;
            var serializablePixelDevice = (SerializablePixelDevice)pixelBoard;
            return JsonConvert.SerializeObject(serializablePixelDevice);
        }

        public Device DeserializeDevice(string text)
        {
            throw new NotImplementedException();
        }
    }
}
