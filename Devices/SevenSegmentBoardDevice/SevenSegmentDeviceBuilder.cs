using BaseDevice;
using DeviceBuilding;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServiceInterfaces;
using SevenSegmentBoardDevice.Commands;
using SevenSegmentBoardDevice.Serialization;
using SevenSegmentBoardDevice.UI;
using SmartTechnologiesM.Base.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace SevenSegmentBoardDevice
{
    public class SevenSegmentDeviceBuilder : IDeviceBuilder
    {
        public string Model => "boardClock";
        public string DisplayName => "Семисегментные часы";
        private TimerDeviceViewModel _model;
        private PreviewViewModel _designViewModel;
        private PreviewViewModel _previewViewModel;

        public ControlsPack GetControlsPack(Device device, ILogger logger)
        {
            var pack = new ControlsPack();
            var previewController = new PreviewController();
            _model = new TimerDeviceViewModel(device);
            previewController.Duration = _model.DisplayFrames
                    .Where(f => f.IsChecked && f.IsEnabled).Sum(f => f.DisplayPeriod) * 1000;
            _model.PropertyChanged += (o, args) =>
            {
                _previewViewModel.IsDigit = _model.DisplayType.Id != 0;
                pack.NotifyThatModelChanged();
                previewController.Duration = _model.DisplayFrames
                    .Where(f => f.IsChecked && f.IsEnabled).Sum(f => f.DisplayPeriod) * 1000;
                RedrawDesignPreview();
            };
            var timerControl = new TimerControl
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                DataContext = _model
            };

            _designViewModel = new PreviewViewModel
            {
                IsDigit = true,
                Text = string.Empty
            };
            _previewViewModel = new PreviewViewModel
            {
                IsDigit = true,
                Text = string.Empty
            };
            var designViewControl = new Preview
            {
                DataContext = _designViewModel
            };
            var previewViewControl = new Preview
            {
                DataContext = _previewViewModel
            };
            previewController.NeedRedrawPosition += (o, position) =>
            {
                var actualFrames = _model.DisplayFrames.Where(f => f.IsChecked).ToList();
                if (!actualFrames.Any())
                    return;
                DisplayFrame actualFrame = null;
                var periodStart = 0;
                _previewViewModel.IsDigit = (_model.DisplayType.Id != 0);
                foreach (var frame in actualFrames)
                {
                    if (position.Between(periodStart * 1000, (periodStart + frame.DisplayPeriod) * 1000))
                    {
                        actualFrame = frame;
                        break;
                    }
                    periodStart += frame.DisplayPeriod;
                }
                if (!actualFrame.IsNull())
                {
                    _previewViewModel.Text = actualFrame?.Preview(_model.DisplayFormat.Capacity);
                }
            };
            pack.CustomizationControl = timerControl;
            pack.DesignPreviewControl = designViewControl;
            pack.PreviewPreviewControl = previewViewControl;
            pack.PreviewController = previewController;
            pack.Device = device;
            RedrawDesignPreview();
            return pack;
        }

        private void RedrawDesignPreview()
        {
            _designViewModel.IsDigit = (_model.DisplayType?.Id != 0);
            switch (_model.DisplayFormat.Capacity)
            {
                case 3:
                    _designViewModel.Text = "123";
                    break;
                case 4:
                    _designViewModel.Text = DateTime.Now.ToString("HH:mm");
                    return;
                case 6:
                    _designViewModel.Text = DateTime.Now.ToString("HH:mm:ss");
                    return;
                case 9:
                    _designViewModel.Text = DateTime.Now.ToString("HH:mm:ss.fff");
                    return;
            }

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
                BoardType = castedDevice?.BoardType ?? new BoardType
                {
                    DisplayFormat = new DisplayFormat(),
                    DisplayType = new DisplayType(),
                    FontType = new FontType()
                },
                StopWatchParameters = castedDevice?.StopWatchParameters ?? new StopWatchParameters(),
                TimeSyncParameters = castedDevice?.TimeSyncParameters ?? new TimeSyncParameters
                {
                    ServerAddress = string.Empty
                },
                DisplayFrames = castedDevice?.DisplayFrames ?? new List<DisplayFrame>()
            };
            sevenSegmentBoard.Name = string.IsNullOrEmpty(device.Name) ? DisplayName : device.Name;
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

        public List<Func<Device, INetworkAgent, ILogger, IConfig, IDeviceCommand<Device>>> GetUploadCommands()
            => new List<Func<Device, INetworkAgent, ILogger, IConfig, IDeviceCommand<Device>>>
            {
                (d, n, l, c) => new UploadBoardTypeCommand(d, n, l, c),
                (d, n, l, c) => new UploadAlarmsCommand(d, n, l, c),
                (d, n, l, c) => new UploadFramesCommand(d, n, l, c),
                (d, n, l, c) => new UploadTimeSyncConfigCommand(d, n, l, c)
            };
    }
}
