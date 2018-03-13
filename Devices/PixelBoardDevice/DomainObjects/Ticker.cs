namespace PixelBoardDevice.DomainObjects
{
    public class Ticker : Zone
    {
        public string Text { get; set; }
        public int FontId { get; set; }
        public int AnimationId { get; set; }
    }
}
