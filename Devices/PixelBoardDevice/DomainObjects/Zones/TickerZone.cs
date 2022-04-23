using System;
using System.Windows;

namespace PixelBoardDevice.DomainObjects.Zones
{
    public class TickerZone : Zone, IFontableZone
    {
        public override ZoneTypes ZoneType => ZoneTypes.Ticker;
        public override string Name { get; set; } = "Таймер";
        public byte? FontId { get; set; }
        public byte TickerType { get; set; }
        public TimeSpan TickerCountDownStartValue { get; set; }
        public bool UseWholeAlphabet => false;
        public string Alphabet => "1234567890:.";
        public TextAlignment? Alignment { get; set; }
    }
}
