namespace PixelBoardDevice.DomainObjects
{
    public class Ticker : Zone
    {
        public override string Name => "Бегущая трока";
        public string Text { get; set; }
        public int FontId { get; set; }
        public int AnimationId { get; set; }
    }
}
