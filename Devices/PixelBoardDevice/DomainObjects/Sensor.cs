namespace PixelBoardDevice.DomainObjects
{
    public class Sensor : Zone
    {
        public int FontId { get; set; }

        public override string Name => "Датчик";
    }
}
