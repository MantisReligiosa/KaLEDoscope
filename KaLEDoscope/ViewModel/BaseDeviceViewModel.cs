using DomainData;
using ServiceInterfaces;
using System;
using System.Collections.Generic;
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

        public string SubnetMask
        {
            get
            {
                return FormatSubnet(SubnetMaskByte);
            }
        }

        public byte SubnetMaskByte
        {
            get
            {
                return Device.Network.SubnetMask;
            }
            set
            {
                Device.Network.SubnetMask = value;
                OnPropertyChanged(nameof(SubnetMaskByte));
                OnPropertyChanged(nameof(SubnetMask));
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
