namespace PixelBoardDevice.DomainObjects
{
    public class BoardHardware
    {
        public BoardHardwareType Type { get; set; } = BoardHardwareType.Hub12;
    }

    public enum BoardHardwareType
    {
        Hub12 = 1,
        Hub08,
        RsPanel16x16,
        RsPanel8x16,
        RsPanel8x8,
        RsPanel8x4,
        RsPanel12x12,
        RsPanel9x28,
        Rs7Segment
    }
}
