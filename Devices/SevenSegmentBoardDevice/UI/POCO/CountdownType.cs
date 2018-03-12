using Abstractions;

namespace SevenSegmentBoardDevice.UI.POCO
{
    public class CountdownType : IIdentified, INamed
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
