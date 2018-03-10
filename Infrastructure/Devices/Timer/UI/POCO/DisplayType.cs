using Abstractions;

namespace SevenSegmentBoardDevice.UI.POCO
{
    public class DisplayType : IIdentified, INamed
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsFontEnabled { get; set; }
    }
}
