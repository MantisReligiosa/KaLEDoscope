using System;
using System.Windows;

namespace PixelBoardDevice.DomainObjects.Zones
{
    public class ClockZone : Zone, IFontableZone
    {
        public override ZoneTypes ZoneType => ZoneTypes.Clock;
        public override string Name { get; set; } = "Часы";
        public byte? FontId { get; set; }
        public ClockTypes ClockType { get; set; }
        public byte ClockFormat { get; set; }
        public bool AllowPeriodicTimeSync { get; set; }
        public bool AllowScheduledSync { get; set; }
        public ushort PeriodicSyncInterval { get; set; }
        public TimeSpan ScheduledTimeSync { get; set; }
        public bool UseWholeAlphabet => false;
        public string Alphabet => "1234567890:.";
        public string Sample { get; set; }
        public TextAlignment? Alignment { get; set; }
    }

    public enum ClockTypes
    {
        Digital = 1,
        Analog = 2
    }
}
