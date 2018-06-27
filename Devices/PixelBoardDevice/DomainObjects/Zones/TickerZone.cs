using System;

namespace PixelBoardDevice.DomainObjects.Zones
{
    public class TickerZone : Zone, IFontableZone
    {
        public override int ZoneType => 6;
        public override string Name { get; set; } = "Таймер";
        public int? FontId { get; set; }
        public int TickerType { get; set; }
        public TimeSpan TickerCountDownStartValue { get; set; }
        public bool UseWholeAlphabet => false;
        public string Alphabet => "1234567890:.";
    }
}
