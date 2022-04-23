using BaseDevice;
using SmartTechnologiesM.Base;
using SmartTechnologiesM.Base.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using UiCommands;
using Input = System.Windows.Input;

namespace KaLEDoscope.ViewModel
{
    public class CommonSettingsViewModel : Notified
    {
        private readonly Device _device;
        public event Action<Device> OnRenamed;

        public ushort Id
        {
            get
            {
                return _device.Id;
            }
            set
            {
                _device.Id = value;
                OnPropertyChanged(nameof(Id));
                OnRenamed?.Invoke(_device);
            }
        }

        public string Name
        {
            get
            {
                return _device.Name;
            }
            set
            {
                _device.Name = value;
                OnPropertyChanged(nameof(Name));
                OnRenamed?.Invoke(_device);
            }
        }

        public string IpAddress
        {
            get
            {
                return _device.Network.IpAddress;
            }
            set
            {
                if (!IsIpValid(value))
                    throw new ArgumentException("Incorrect IP");
                _device.Network.IpAddress = value;
                OnPropertyChanged(nameof(IpAddress));
            }
        }

        private static bool IsIpValid(string value)
        {
            return Regex.IsMatch(value, @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$");
        }

        public int Port
        {
            get
            {
                return _device.Network.Port;
            }
            set
            {
                _device.Network.Port = value;
                OnPropertyChanged(nameof(Port));
            }
        }

        public ObservableCollection<BrightnessPeriod> BrightnessPeriods { get; set; }

        public BrightnessPeriod SelectedBrightnessPeriod { get; set; }

        private string _subnetMask;
        public string SubnetMask
        {
            get
            {
                return _subnetMask;
            }
            set
            {
                if (_subnetMask?.Equals(value) ?? false)
                {
                    return;
                }
                if (!ParceSubnet(value, out byte maskByte))
                {
                    throw new ArgumentException("Incorrect subnet mask");
                }
                _subnetMask = value;
                SubnetMaskByte = maskByte;
                OnPropertyChanged(nameof(SubnetMask));
                OnPropertyChanged(nameof(SubnetMaskByte));
            }
        }

        private bool ParceSubnet(string mask, out byte subnetMaskByte)
        {
            subnetMaskByte = default;
            var bytesStr = mask.Split('.');
            if (bytesStr.Length != 4)
            {
                return false;
            }
            var bytes = new List<byte>();
            foreach (var byteStr in bytesStr)
            {
                if (byte.TryParse(byteStr, out byte b))
                {
                    bytes.Add(b);
                }
                else
                {
                    return false;
                }
            }
            return bytes.ToArray().TrySubnetToByte(0, out subnetMaskByte);
        }

        public byte SubnetMaskByte
        {
            get
            {
                return _device.Network.SubnetMask;
            }
            set
            {
                if (_device.Network.SubnetMask == value)
                {
                    return;
                }
                _device.Network.SubnetMask = value;
                SubnetMask = FormatSubnet(value);
                OnPropertyChanged(nameof(SubnetMask));
                OnPropertyChanged(nameof(SubnetMaskByte));
            }
        }

        public string Gateway
        {
            get
            {
                return _device.Network.Gateway;
            }
            set
            {
                if (!IsIpValid(value))
                    throw new ArgumentException("Incorrect Gateway");
                _device.Network.Gateway = value;
                OnPropertyChanged(nameof(Gateway));
            }
        }

        public string DnsServer
        {
            get
            {
                return _device.Network.DnsServer;
            }
            set
            {
                if (!IsIpValid(value))
                    throw new ArgumentException("Incorrect DNS");
                _device.Network.DnsServer = value;
                OnPropertyChanged(nameof(DnsServer));
            }
        }

        public string AlternativeDnsServer
        {
            get
            {
                return _device.Network.AlternativeDnsServer;
            }
            set
            {
                _device.Network.AlternativeDnsServer = value;
                OnPropertyChanged(nameof(AlternativeDnsServer));
            }
        }

        public bool AutomaticBrightness
        {
            get
            {
                return _device.Brightness.Mode == BrightnessMode.Auto;
            }
            set
            {
                if (value)
                {
                    _device.Brightness.Mode = BrightnessMode.Auto;
                }
                OnPropertyChanged(nameof(AutomaticBrightness));
            }
        }

        public bool ManualBrightness
        {
            get
            {
                return _device.Brightness.Mode == BrightnessMode.Manual;
            }
            set
            {
                if (value)
                {
                    _device.Brightness.Mode = BrightnessMode.Manual;
                }
                OnPropertyChanged(nameof(ManualBrightness));
            }
        }

        public int ManualBrightnessValue
        {
            get
            {
                return _device.Brightness.ManualValue;
            }
            set
            {
                _device.Brightness.ManualValue = (ushort)value;
                OnPropertyChanged(nameof(ManualBrightnessValue));
            }
        }

        public int MaxBrightnessValue => _device.Brightness.MaxValue;
        public int BrightnessIncrement => _device.Brightness.Increment;

        public bool ScheduledBrightness
        {
            get
            {
                return _device.Brightness.Mode == BrightnessMode.Scheduled;
            }
            set
            {
                if (value)
                {
                    _device.Brightness.Mode = BrightnessMode.Scheduled;
                }
                OnPropertyChanged(nameof(ScheduledBrightness));
            }
        }

        public bool ScheduleWorkAroundTheClock
        {
            get
            {
                return _device.WorkSchedule.AroundTheClock;
            }
            set
            {
                _device.WorkSchedule.AroundTheClock = value;
                OnPropertyChanged(nameof(ScheduleWorkAroundTheClock));
                OnPropertyChanged(nameof(IsScheduledWork));
            }
        }

        public bool IsScheduledWork
        {
            get
            {
                return !ScheduleWorkAroundTheClock;
            }
        }

        public string ScheduledWorkStart
        {
            get
            {
                return _device.WorkSchedule.StartFrom.ToString(@"hh\:mm");
            }
            set
            {
                if (!TimeSpan.TryParse(value, out TimeSpan timeSpan))
                {
                    timeSpan = new TimeSpan(0, 0, 0);
                }
                _device.WorkSchedule.StartFrom = timeSpan;
                OnPropertyChanged(nameof(ScheduledWorkStart));
            }
        }

        public string ScheduledWorkEnd
        {
            get
            {
                return _device.WorkSchedule.FinishTo.ToString(@"hh\:mm");
            }
            set
            {
                if (!TimeSpan.TryParse(value, out TimeSpan timeSpan))
                {
                    timeSpan = new TimeSpan(0, 0, 0);
                }
                _device.WorkSchedule.FinishTo = timeSpan;
                OnPropertyChanged(nameof(ScheduledWorkEnd));
            }
        }

        public bool RunAllWeek
        {
            get
            {
                return _device.WorkSchedule.AllWeek;
            }
            set
            {
                _device.WorkSchedule.AllWeek = value;
                OnPropertyChanged(nameof(RunAllWeek));
                OnPropertyChanged(nameof(RunByDaysOfWeek));
            }
        }

        public bool RunByDaysOfWeek
        {
            get
            {
                return !RunAllWeek;
            }
        }

        public bool RunInSun
        {
            get
            {
                return _device.WorkSchedule.RunInSun;
            }
            set
            {
                _device.WorkSchedule.RunInSun = value;
                OnPropertyChanged(nameof(RunInSun));
            }
        }

        public bool RunInMon
        {
            get
            {
                return _device.WorkSchedule.RunInMon;
            }
            set
            {
                _device.WorkSchedule.RunInMon = value;
                OnPropertyChanged(nameof(RunInMon));
            }
        }

        public bool RunInTue
        {
            get
            {
                return _device.WorkSchedule.RunInTue;
            }
            set
            {
                _device.WorkSchedule.RunInTue = value;
                OnPropertyChanged(nameof(RunInTue));
            }
        }

        public bool RunInWed
        {
            get
            {
                return _device.WorkSchedule.RunInWed;
            }
            set
            {
                _device.WorkSchedule.RunInWed = value;
                OnPropertyChanged(nameof(RunInWed));
            }
        }

        public bool RunInThu
        {
            get
            {
                return _device.WorkSchedule.RunInThu;
            }
            set
            {
                _device.WorkSchedule.RunInThu = value;
                OnPropertyChanged(nameof(RunInThu));
            }
        }

        public bool RunInFri
        {
            get
            {
                return _device.WorkSchedule.RunInFri;
            }
            set
            {
                _device.WorkSchedule.RunInFri = value;
                OnPropertyChanged(nameof(RunInFri));
            }
        }

        public bool RunInSat
        {
            get
            {
                return _device.WorkSchedule.RunInSat;
            }
            set
            {
                _device.WorkSchedule.RunInSat = value;
                OnPropertyChanged(nameof(RunInSat));
            }
        }

        public CommonSettingsViewModel(Device device)
        {
            _device = device;
            SubnetMask = FormatSubnet(device.Network.SubnetMask);
            BrightnessPeriods = new ObservableCollection<BrightnessPeriod>(device.Brightness.BrightnessPeriods);
        }

        private string FormatSubnet(byte value)
        {
            int mask = 0;
            for (int i = 0; i < value; i++)
            {
                mask <<= 1;
                mask |= 1;
            }
            mask <<= (32 - value);
            var byte1 = (mask & 0xff000000) >> 24;
            var byte2 = (mask & 0x00ff0000) >> 16;
            var byte3 = (mask & 0x0000ff00) >> 8;
            var byte4 = (mask & 0x000000ff);
            var result = $"{byte1}.{byte2}.{byte3}.{byte4}";
            return result;
        }

        private DelegateCommand _addBrightnessItem;
        public Input.ICommand AddBrightnessItem
        {
            get
            {
                if (_addBrightnessItem.IsNull())
                {
                    _addBrightnessItem = new DelegateCommand((o) =>
                    {
                        var from = BrightnessPeriods.Any() ? BrightnessPeriods.Max(p => p.To) : new TimeSpan(0, 0, 0);
                        var period = new BrightnessPeriod
                        {
                            From = from,
                            To = new TimeSpan(0, 0, 0),
                            Value = 1
                        };
                        BrightnessPeriods.Add(period);
                        _device.Brightness.BrightnessPeriods.Add(period);
                    });
                }
                return _addBrightnessItem;
            }
        }

        private DelegateCommand _removeBrightnessItem;
        public Input.ICommand RemoveBrightnessItem
        {
            get
            {
                if (_removeBrightnessItem == null)
                {
                    _removeBrightnessItem = new DelegateCommand((o) =>
                    {
                        BrightnessPeriods.Remove(SelectedBrightnessPeriod);
                        SelectedBrightnessPeriod = null;
                    });
                }
                return _removeBrightnessItem;
            }
        }
    }
}
