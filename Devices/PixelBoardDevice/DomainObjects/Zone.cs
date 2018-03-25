using System.Linq;

namespace PixelBoardDevice.DomainObjects
{
    public class Zone
    {
        public virtual string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public int Id { get; set; }
        public bool IsValid { get; set; }
        public int ZoneType { get; set; }
        public int? FontId { get; set; }
        public string Text { get; set; }
        public bool IsFonted
        {
            get
            {
                return (new int[] { (int)ZoneTypes.Text, (int)ZoneTypes.Sensor, (int)ZoneTypes.MQTT, (int)ZoneTypes.Clock }).Contains(ZoneType);
            }
        }

        public string MqttTag { get; set; }
    }

    public enum ZoneTypes
    {
        Text = 1,
        Sensor,
        Picture,
        MQTT,
        Clock
    }
}