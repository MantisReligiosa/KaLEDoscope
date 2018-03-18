namespace PixelBoardDevice.DomainObjects
{
    public class Sensor : Zone, IFonted
    {
        public int FontId { get; set; }

        public override string Name => "Датчик";
    }
}
