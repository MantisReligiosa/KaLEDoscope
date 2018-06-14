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
using System.Windows.Controls;
using System.Windows.Data;
using Newtonsoft.Json.Linq;
using PixelBoardDevice.Commands;

namespace PixelBoardDevice
{
    public class PixelDeviceBuilder : IDeviceBuilder
    {
        public string Model => "pixelBoard";
        public string DisplayName => "Электронное табло";

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
            var slider = new Slider
            {
                Width = 300,
                TickFrequency = .1,
                IsSnapToTickEnabled = true,
                Maximum = _model.PreviewScaleMaxRate,
                Minimum = _model.PreviewScaleMinRate,
                TickPlacement = System.Windows.Controls.Primitives.TickPlacement.BottomRight,
            };
            var sliderBinding = new Binding(nameof(PixelDeviceViewModel.PreviewScale))
            {
                Source = _model
            };
            slider.SetBinding(Slider.ValueProperty, sliderBinding);
            var combobox = new ComboBox
            {
                IsEditable = true,
                IsReadOnly = false,
                DisplayMemberPath = "Caption",
                SelectedValuePath = "Value",
                Width = 60
            };
            var comboboxBinding = new Binding(nameof(PixelDeviceViewModel.PreviewScalePercents))
            {
                Source = _model
            };
            combobox.SetBinding(ComboBox.SelectedValueProperty, comboboxBinding);
            for (double i = _model.PreviewScaleMinRate; i < _model.PreviewScaleMaxRate; i += .1)
            {
                var value = Convert.ToInt32(i * 100);
                combobox.Items.Add(new
                {
                    Value = value,
                    Caption = $"{value} %"
                });
            }
            pack.MenuItems = new List<object>
            {
                new Separator(),
                slider,
                combobox
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
                    Mode = BrightnessMode.Auto
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
                Name = String.IsNullOrEmpty(device.Name) ? DisplayName : device.Name
            };
            return pixelBoard;
        }

        public string SerializeDevice(Device device)
        {
            var serializablePixelDevice = GetSerializable(device);
            return JsonConvert.SerializeObject(serializablePixelDevice);
        }

        public Device DeserializeDevice(string text)
        {
            var serializablePixelDevice = JsonConvert.DeserializeObject<SerializablePixelDevice>(text);
            return FromSerializable(serializablePixelDevice);
        }

        public object GetSerializable(Device device) => (SerializablePixelDevice)(PixelBoard)device;

        public Device FromSerializable(object serializableDevice)
        {
            if (serializableDevice is SerializablePixelDevice serializablePixelDevice)
            {
                return (PixelBoard)serializablePixelDevice;
            }
            if (serializableDevice is JObject jObject)
            {
                return (PixelBoard)((jObject).ToObject<SerializablePixelDevice>());
            }
            return null;
        }

        public IEnumerable<Func<Device, INetworkAgent, ILogger, IDeviceCommand<Device>>> GetDownloadCommands()
            => new List<Func<Device, INetworkAgent, ILogger, IDeviceCommand<Device>>>
            {
                (d, n, l) => new DownloadBoardConfigCommand(d, n, l),
                (d, n, l) => new DownloadFontsCommand(d, n, l),
                (d, n, l) => new DownloadProgramsCommands(d, n, l),
                (d, n, l) => new DownloadZonesCommand(d, n, l)
            };

        public IEnumerable<Func<Device, INetworkAgent, ILogger, IDeviceCommand<Device>>> GetUploadCommands()
            => new List<Func<Device, INetworkAgent, ILogger, IDeviceCommand<Device>>>
            { };
    }
}
