namespace PixelBoardDevice.DomainObjects.Zones
{
    public class TextZone : Zone, IFontableZone
    {
        public override int ZoneType => 1;
        public override string Name { get; set; } = "Текст";
        public int? FontId { get; set; }
        public string Text { get; set; }
        public bool UseWholeAlphabet => true;
        public string Alphabet => string.Empty;
        public int? Alignment { get; set; }
        public int? AnimationId { get; set; }
    }
}
