using Abstractions;

namespace SevenSegmentBoardDevice.UI.POCO
{
    public class DisplayFormat : IIdentified, INamed
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }
    }
}
