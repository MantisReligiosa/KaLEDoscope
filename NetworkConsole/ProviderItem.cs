using System;

namespace NetworkConsole
{
    public class ProviderItem
    {
        public string Name { get; set; }
        public Func<IProvider> GetProvider { get; set; }
        public bool AllowBroadCast { get; set; }
    }
}
