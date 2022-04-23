using System.Windows;

namespace PixelBoardDevice.DomainObjects.Zones
{
    public class TagZone : Zone, IFontableZone
    {
        public override ZoneTypes ZoneType => ZoneTypes.Tag;
        public override string Name { get; set; } = "Тэг внешнего сервера";
        public byte? FontId { get; set; }
        public string ExternalSourceTag { get; set; }
        public bool UseWholeAlphabet => true;
        public string Alphabet => string.Empty;
        public TextAlignment? Alignment { get; set; }
    }
}
