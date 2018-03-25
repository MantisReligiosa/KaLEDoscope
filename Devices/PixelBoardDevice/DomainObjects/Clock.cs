namespace PixelBoardDevice.DomainObjects
{
    public class Clock : Zone, IFonted
    {
        public override string Name => "Часы";
        public int FontId { get; set; }
        public int FormatId { get; set; }
    }
}