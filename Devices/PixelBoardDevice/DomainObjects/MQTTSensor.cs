using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelBoardDevice.DomainObjects
{
    public class MQTTSensor : Zone
    {
        public override string Name => "Тэг MQTT";
        public int FontId { get; set; }
        public string Tag { get; set; }
    }
}
