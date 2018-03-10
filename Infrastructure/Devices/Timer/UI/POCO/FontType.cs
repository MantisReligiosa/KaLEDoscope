using Abstractions;

namespace KaLEDoscope.POCO.Timer
{
    public class FontType : IIdentified, INamed
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
