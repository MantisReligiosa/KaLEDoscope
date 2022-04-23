using System.Windows;

namespace PixelBoardDevice.DomainObjects.Zones
{
    public class TextZone : Zone, IFontableZone
    {
        public override ZoneTypes ZoneType => ZoneTypes.Text;
        public override string Name { get; set; } = "Текст";
        public byte? FontId { get; set; }
        public string Text { get; set; }
        public string Alphabet => string.Empty;
        public TextAlignment? Alignment { get; set; }
        public byte? AnimationId { get; set; }
        public byte? AnimationSpeed { get; set; }
        public byte AnimationTimeout { get; set; }
    }
}
