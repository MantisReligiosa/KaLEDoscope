using Abstractions;

namespace KaLEDoscope.POCO.Timer
{
    public class SyncSource : IIdentified, INamed
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsCutomized { get; set; }
        public bool AllowTimezones { get; set; }
    }
}
