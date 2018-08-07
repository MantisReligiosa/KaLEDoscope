using System.Linq;
using System.Management;

namespace Activation
{
    public class HardwareInfoProvider : IHardwareInfoProvider
    {
        public string ProcessorId => GetHardwareParameter("Win32_Processor", "ProcessorId");

        public string MotherboardSerial => GetHardwareParameter("Win32_BaseBoard", "SerialNumber");

        public string MemorySerial => GetHardwareParameter("Win32_PhysicalMemory", "SerialNumber");

        public string DriveSerial => GetHardwareParameter("Win32_DiskDrive", "SerialNumber");

        private string GetHardwareParameter(string hardwareArea, string parameterName)
        {
            using (var searcher = new ManagementObjectSearcher($"SELECT * FROM {hardwareArea}"))
                return searcher.Get()
                    .Cast<ManagementObject>()
                    .First()[parameterName]
                    ?.ToString() ?? string.Empty;
        }
    }
}
