using PixelBoardDevice.DomainObjects;
using System;

namespace PixelBoardDevice.POCO
{
    public class ZoneType
    {
        public byte Id { get; set; }
        public string Name { get; set; }
        public bool AllowText { get; set; }
        public bool AllowAnimation { get; set; }
        public bool AllowBitmap { get; set; }
        public bool AllowMQTT { get; set; }
        public Func<Zone,bool> ZoneCondition { get; set; }
        public Func<Zone> Customize { get; set; }
        public bool AllowClock { get; set; }
        public bool AllowTextEditing { get; set; }
        public bool AllowTicker { get; set; }
    }
}
