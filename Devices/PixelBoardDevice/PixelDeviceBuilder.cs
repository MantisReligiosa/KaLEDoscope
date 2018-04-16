using PixelBoardDevice.DomainObjects;
using BaseDevice;
using DeviceBuilding;
using PixelBoardDevice.UI;
using ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Resources;
using PixelBoardDevice.Serialization;
using Newtonsoft.Json;

namespace PixelBoardDevice
{
    public class PixelDeviceBuilder : IDeviceBuilder
    {
        public string Model => "pixelBoard";
        private PixelDeviceViewModel _model;
        private UserControl _previewControl;

        public ControlsPack GetControlsPack(Device device, ILogger logger)
        {
            _model = new PixelDeviceViewModel(device, logger);
            var pack = new ControlsPack
            {
                CustomizationControl = new PixelControl
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    DataContext = _model
                }
            };
            _previewControl = new ProgramPreviewControl
            {
                DataContext = new ProgramPreviewViewModel
                {
                    Program = _model.SelectedProgram
                }
            };
            _model.PropertyChanged += Model_PropertyChanged;
            pack.PreviewControl = _previewControl;
            return pack;
        }

        private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(PixelDeviceViewModel.PreviewImage))
            {
                return;
            }
            //_previewControl.Image = _model?.PreviewImage;
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
