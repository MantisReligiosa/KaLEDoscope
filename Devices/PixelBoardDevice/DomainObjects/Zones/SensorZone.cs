using System.Windows;

namespace PixelBoardDevice.DomainObjects.Zones
{
    public class SensorZone : Zone, IFontableZone
    {
        public override ZoneTypes ZoneType => ZoneTypes.Sensor;
        public override string Name { get; set; } = "Датчик";
        public byte? FontId { get; set; }
        public bool UseWholeAlphabet => true;
        public string Alphabet => string.Empty;
        public TextAlignment? Alignment { get; set; }
    }
}
