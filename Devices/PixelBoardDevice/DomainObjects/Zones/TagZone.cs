namespace PixelBoardDevice.DomainObjects.Zones
{
    public class TagZone : Zone, IFontableZone
    {
        public override int ZoneType => 4;
        public override string Name { get; set; } = "Тэг внешнего сервера";
        public int? FontId { get; set; }
        public string ExternalSourceTag { get; set; }
        public bool UseWholeAlphabet => true;
        public string Alphabet => string.Empty;
    }
}
