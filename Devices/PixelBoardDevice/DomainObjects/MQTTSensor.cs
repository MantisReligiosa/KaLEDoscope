namespace PixelBoardDevice.DomainObjects
{
    public class MQTTSensor : Zone, IFonted
    {
        public override string Name => "Тэг MQTT";
        public int FontId { get; set; }
        public string Tag { get; set; }
    }
}
