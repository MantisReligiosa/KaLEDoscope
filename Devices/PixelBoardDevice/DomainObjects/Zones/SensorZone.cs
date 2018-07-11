namespace PixelBoardDevice.DomainObjects.Zones
{
    public class SensorZone : Zone, IFontableZone
    {
        public override int ZoneType => 2;
        public override string Name { get; set; } = "Датчик";
        public int? FontId { get; set; }
        public bool UseWholeAlphabet => true;
        public string Alphabet => string.Empty;
        public int? Alignment { get; set; }
    }
}
