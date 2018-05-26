using Newtonsoft.Json;
using ServiceInterfaces;
using System.Text;

namespace JsonExchange
{
    public class JsonRequestBuilder : IRequestBuilder
    {
        private Request _request;
        public byte[] GetBytes()
        {
            return Encoding.UTF8.GetBytes(GetString());
        }

        public string GetString()
        {
            return JsonConvert.SerializeObject(_request);
        }

        public void SetRequest(Request request)
        {
            _request = request;
        }
    }
}
