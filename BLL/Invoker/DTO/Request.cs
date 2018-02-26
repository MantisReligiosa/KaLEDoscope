using Newtonsoft.Json;

namespace CommandProcessing.DTO
{
    internal class Request
    {
        public Request()
        {
        }

        [JsonProperty("scan")]
        public object Scan { get; set; }
    }
}