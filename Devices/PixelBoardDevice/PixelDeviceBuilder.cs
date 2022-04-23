using BaseDevice;
using DeviceBuilding;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PixelBoardDevice.BLL;
using PixelBoardDevice.Commands;
using PixelBoardDevice.DomainObjects;
using PixelBoardDevice.Serialization;
using PixelBoardDevice.UI;
using ServiceInterfaces;
using SmartTechnologiesM.Base.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace PixelBoardDevice
{
    public class PixelDeviceBuilder : IDeviceBuilder
    {
        public string Model => "pixelBoard";
        public string DisplayName => "Электронное табло";

        public ControlsPack GetControlsPack(Device device, ILogger logger)
        {
            var deviceController = new DeviceController(device);
            var model = new PixelDeviceViewModel(deviceController, logger);
            var previewController = new PreviewController()
            {
                Duration = model.Programs.Sum(p => p.Period) * 1000
            };
            previewController.NeedRedrawPosition += (o, position) =>
            {
                var programs = model.Programs.ToList();
                Program actualProgram = null;
                var periodStart = 0;
                foreach (var program in programs)
                {
                    if (position.Between(periodStart * 1000, (periodStart + program.Period) * 1000))
                    {
                        actualProgram = program;
                        break;
                    }
                    periodStart += program.Period;
                }
                if (!actualProgram.IsNull())
                {
                    model.PreviewedProgram = actualProgram;
                }
            };
            var customizationControl = new PixelControl
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                DataContext = model
            };
            customizationControl.TextEditControl.Loaded += (o, e) =>
            {
                model.TextEditWidth = (int)customizationControl.TextEditControl.ActualWidth;
            };
            var pack = new ControlsPack
            {
                CustomizationControl = customizationControl,
                Device = device,
                PreviewController = previewController
            };
            var designViewModel = new ProgramPreviewViewModel(model);
            var _previewViewModel = new ProgramPreviewViewModel(model, true);
            model.PropertyChanged += (o, args) =>
            {
                previewController.Duration = model.Programs.Sum(p => p.Period) * 1000;
                if (args.PropertyName == nameof(model.Programs))
                {
                    var collectionViewSource = customizationControl.ProgramGrid.Resources["cvs"] as CollectionViewSource;
                    collectionViewSource.View.Refresh();
                }
                pack.NotifyThatModelChanged();
            };
            pack.OnPreviewAreaMouseDown = () =>
            {
                designViewModel.OnMouseUp();
            };
            var designViewControl = new ProgramPreviewControl
            {
                DataContext = designViewModel
            };
            var previewViewControl = new ProgramPreviewControl
            {
                DataContext = _previewViewModel
            };
            pack.DesignPreviewControl = designViewControl;
            pack.PreviewPreviewControl = previewViewControl;
            var slider = new Slider
            {
                Width = 300,
                TickFrequency = .1,
                IsSnapToTickEnabled = true,
                Maximum = model.PreviewScaleMaxRate,
                Minimum = model.PreviewScaleMinRate,
                TickPlacement = System.Windows.Controls.Primitives.TickPlacement.BottomRight,
            };
            var sliderBinding = new Binding(nameof(PixelDeviceViewModel.PreviewScale))
            {
                Source = model
            };
            slider.SetBinding(RangeBase.ValueProperty, sliderBinding);
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
                Source = model
            };
            combobox.SetBinding(Selector.SelectedValueProperty, comboboxBinding);
            for (double i = model.PreviewScaleMinRate; i < model.PreviewScaleMaxRate; i += .1)
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
                BinaryImages = castedDevice?.BinaryImages ?? new List<BinaryImage>(),
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
                Name = string.IsNullOrEmpty(device.Name) ? DisplayName : device.Name,
                Hardware = castedDevice?.Hardware ?? new BoardHardware()
            };
            return pixelBoard;
        }

        public string SerializeDevice(Device device)
        {
            var serializablePixelDevice = GetSerializable(device);
            var result = JsonConvert.SerializeObject(serializablePixelDevice);
            return result;
        }

        public Device DeserializeDevice(string text)
        {
            var serializablePixelDevice = JsonConvert.DeserializeObject<SerializablePixelDevice>(text);
            var device = FromSerializable(serializablePixelDevice) as PixelBoard;
            if (device != null)
            {
                device.CleanupUnusedFonts();
                device.CleanupUnusedImages();
            }
            return device;
        }

        public object GetSerializable(Device device) => (SerializablePixelDevice)(PixelBoard)device;

        public Device FromSerializable(object serializableDevice)
        {
            if (serializableDevice is SerializablePixelDevice serializablePixelDevice)
            {
                var device = (PixelBoard)serializablePixelDevice;
                device.CleanupUnusedFonts();
                device.CleanupUnusedImages();
                return device;
            }
            if (serializableDevice is JObject jObject)
            {
                var device = (PixelBoard)((jObject).ToObject<SerializablePixelDevice>());
                if (device != null)
                {
                    device.CleanupUnusedFonts();
                    device.CleanupUnusedImages();
                }
                return device;
            }
            return null;
        }

        public List<Func<Device, INetworkAgent, ILogger, IConfig, IDeviceCommand<Device>>> GetUploadCommands()
            => new List<Func<Device, INetworkAgent, ILogger, IConfig, IDeviceCommand<Device>>>
            {
                (d, n, l, c) => new UploadBoardConfigCommand(d, n, l, c),
                (d, n, l, c) => new UploadBoardHardwareConfigCommand(d, n, l, c),
                (d, n, l, c) => new UploadFontsCommand(d, n, l, c),
                (d, n, l, c) => new UploadProgramsCommand(d, n, l, c),
                (d, n, l, c) => new UploadZonesCommand(d, n, l, c),
                (d, n, l, c) => new UploadBinaryImageCommand(d, n, l, c)
            };
    }
}
