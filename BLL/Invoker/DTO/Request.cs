using Newtonsoft.Json;

namespace CommandProcessing.DTO
{
    internal class Request
    {
        public Request()
        {
        }

        [JsonProperty("scan", NullValueHandling = NullValueHandling.Ignore)]
        public object Scan { get; set; }

        [JsonProperty("configuration",NullValueHandling = NullValueHandling.Ignore)]
        public object Device { get; set; }

        [JsonProperty("getConfig", NullValueHandling = NullValueHandling.Ignore)]
        public object GetConfig { get; set; }
    }
}