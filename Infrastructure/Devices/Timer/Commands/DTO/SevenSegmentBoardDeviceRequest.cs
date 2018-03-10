using Newtonsoft.Json;

namespace SevenSegmentBoardDevice.Commands.DTO
{
    public class SevenSegmentBoardDeviceRequest
    {
        [JsonProperty("stop", NullValueHandling = NullValueHandling.Ignore)]
        public object Stop { get; set; }

        [JsonProperty("pause", NullValueHandling = NullValueHandling.Ignore)]
        public object Pause { get; set; }

        [JsonProperty("reset", NullValueHandling = NullValueHandling.Ignore)]
        public object Reset { get; set; }

        [JsonProperty("start", NullValueHandling = NullValueHandling.Ignore)]
        public object Start { get; internal set; }
    }
}
