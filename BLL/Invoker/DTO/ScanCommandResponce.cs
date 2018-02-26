using Newtonsoft.Json;

namespace CommandProcessing.DTO
{
    public class ScanCommandResponce
    {
        [JsonProperty("network")]
        public NetworkParameters NetworkParameters { get; set; }

        [JsonProperty("device")]
        public DeviceParameters DeviceParameters { get; set; }
    }

    public class NetworkParameters
    {
        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("port")]
        public int Port { get; set; }
    }

    public class DeviceParameters
    {
        [JsonProperty("model")]
        public string Model { get; set; }

        [JsonProperty("firmware")]
        public string FirmwareVersion { get; set; }
    }
}