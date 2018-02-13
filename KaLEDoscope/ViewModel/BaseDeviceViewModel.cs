using DomainData;
using ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaLEDoscope.ViewModel
{
    public class BaseDeviceViewModel : INotifyPropertyChanged
    {
        public Device Device { get; private set; }

        public string IpAddress
        {
            get
            {
                return Device.Network.IpAddress;
            }
            set
            {
                Device.Network.IpAddress = value;
                OnPropertyChanged(nameof(IpAddress));
            }
        }

        public ObservableCollection<BrightnessPeriod> BrightnessPeriods { get; set; } = new ObservableCollection<BrightnessPeriod>();

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
                byte maskByte;
                if (ParceSubnet(value, out maskByte))
                {
                    _subnetMask = value;
                    SubnetMaskByte = maskByte;
                }
                else
                {
                    SubnetMask = "255.255.255.0";
                    SubnetMaskByte = 24;
                }
                OnPropertyChanged(nameof(SubnetMask));
                OnPropertyChanged(nameof(SubnetMaskByte));
            }
        }

        private bool ParceSubnet(string mask, out byte subnetMaskByte)
        {
            subnetMaskByte = default(byte);
            var bytesStr = mask.Split('.');
            if (bytesStr.Length != 4)
            {
                subnetMaskByte = 24;
            }
            var fullByte = (byte)0xff;
            Dictionary<byte, byte> allowedBytes = new Dictionary<byte, byte>
                {
                    {0,0 },
                    {1,128 },
                    {2,192 },
                    {3,224 },
                    {4,240 },
                    {5,248 },
                    {6,252 },
                    {7,254 }
                };
            List<byte> bytes = new List<byte>();
            foreach (var byteStr in bytesStr)
            {
                byte b;
                if (byte.TryParse(byteStr, out b))
                {
                    bytes.Add(b);
                }
                else
                {
                    return false;
                }
            }
            bool terminate = false;
            foreach (var b in bytes)
            {
                if (terminate && b != 0)
                {
                    return false;
                }
                if (b == fullByte)
                {
                    subnetMaskByte += 8;
                }
                else
                {
                    if (!allowedBytes.Any(kvp => kvp.Value == b))
                    {
                        return false;
                    }
                    var pair = allowedBytes.FirstOrDefault(kvp => kvp.Value == b);
                    terminate = true;
                    subnetMaskByte += pair.Key;
                }
            }
            return true;
        }

        public byte SubnetMaskByte
        {
            get
            {
                return Device.Network.SubnetMask;
            }
            set
            {
                if (Device.Network.SubnetMask == value)
                {
                    return;
                }
                Device.Network.SubnetMask = value;
                SubnetMask = FormatSubnet(value);
                OnPropertyChanged(nameof(SubnetMask));
                OnPropertyChanged(nameof(SubnetMaskByte));
            }
        }

        public string Gateway
        {
            get
            {
                return Device.Network.Gateway;
            }
            set
            {
                Device.Network.Gateway = value;
                OnPropertyChanged(nameof(Gateway));
            }
        }

        public string DnsServer
        {
            get
            {
                return Device.Network.DnsServer;
            }
            set
            {
                Device.Network.DnsServer = value;
                OnPropertyChanged(nameof(DnsServer));
            }
        }

        public string AlternativeDnsServer
        {
            get
            {
                return Device.Network.AlternativeDnsServer;
            }
            set
            {
                Device.Network.AlternativeDnsServer = value;
                OnPropertyChanged(nameof(AlternativeDnsServer));
            }
        }

        public bool AutomaticBrightness
        {
            get
            {
                return Device.Brightness.Mode == Mode.Auto;
            }
            set
            {
                if (value)
                {
                    Device.Brightness.Mode = Mode.Auto;
                }
                OnPropertyChanged(nameof(AutomaticBrightness));
            }
        }

        public bool ManualBrightness
        {
            get
            {
                return Device.Brightness.Mode == Mode.Manual;
            }
            set
            {
                if (value)
                {
                    Device.Brightness.Mode = Mode.Manual;
                }
                OnPropertyChanged(nameof(ManualBrightness));
            }
        }

        public int ManualBrightnessValue
        {
            get
            {
                return Device.Brightness.ManualValue;
            }
            set
            {
                Device.Brightness.ManualValue = value;
                OnPropertyChanged(nameof(ManualBrightnessValue));
            }
        }

        public bool ScheduledBrightness
        {
            get
            {
                return Device.Brightness.Mode == Mode.Scheduled;
            }
            set
            {
                if (value)
                {
                    Device.Brightness.Mode = Mode.Scheduled;
                }
                OnPropertyChanged(nameof(ScheduledBrightness));
            }
        }

        public bool ScheduleWorkAroundTheClock
        {
            get
            {
                return Device.Schedule.AroundTheClock;
            }
            set
            {
                Device.Schedule.AroundTheClock = value;
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
                return Device.Schedule.StartFrom.ToString(@"hh\:mm");
            }
            set
            {
                TimeSpan timeSpan;
                if (!TimeSpan.TryParse(value, out timeSpan))
                {
                    timeSpan = new TimeSpan(0, 0, 0);
                }
                Device.Schedule.StartFrom = timeSpan;
                OnPropertyChanged(nameof(ScheduledWorkStart));
            }
        }

        public string ScheduledWorkEnd
        {
            get
            {
                return Device.Schedule.FinishTo.ToString(@"hh\:mm");
            }
            set
            {
                TimeSpan timeSpan;
                if (!TimeSpan.TryParse(value, out timeSpan))
                {
                    timeSpan = new TimeSpan(0, 0, 0);
                }
                Device.Schedule.FinishTo = timeSpan;
                OnPropertyChanged(nameof(ScheduledWorkEnd));
            }
        }

        public bool RunAllWeek
        {
            get
            {
                return Device.Schedule.AllWeek;
            }
            set
            {
                Device.Schedule.AllWeek = value;
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
                return Device.Schedule.RunInSun;
            }
            set
            {
                Device.Schedule.RunInSun = value;
                OnPropertyChanged(nameof(RunInSun));
            }
        }

        public bool RunInMon
        {
            get
            {
                return Device.Schedule.RunInMon;
            }
            set
            {
                Device.Schedule.RunInMon = value;
                OnPropertyChanged(nameof(RunInMon));
            }
        }

        public bool RunInTue
        {
            get
            {
                return Device.Schedule.RunInTue;
            }
            set
            {
                Device.Schedule.RunInTue = value;
                OnPropertyChanged(nameof(RunInTue));
            }
        }

        public bool RunInWed
        {
            get
            {
                return Device.Schedule.RunInWed;
            }
            set
            {
                Device.Schedule.RunInWed = value;
                OnPropertyChanged(nameof(RunInWed));
            }
        }

        public bool RunInThu
        {
            get
            {
                return Device.Schedule.RunInThu;
            }
            set
            {
                Device.Schedule.RunInThu = value;
                OnPropertyChanged(nameof(RunInThu));
            }
        }

        public bool RunInFri
        {
            get
            {
                return Device.Schedule.RunInFri;
            }
            set
            {
                Device.Schedule.RunInFri = value;
                OnPropertyChanged(nameof(RunInFri));
            }
        }

        public bool RunInSat
        {
            get
            {
                return Device.Schedule.RunInSat;
            }
            set
            {
                Device.Schedule.RunInSat = value;
                OnPropertyChanged(nameof(RunInSat));
            }
        }

        public BaseDeviceViewModel(Device device, ILogger logger)
        {
            Device = device;
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
