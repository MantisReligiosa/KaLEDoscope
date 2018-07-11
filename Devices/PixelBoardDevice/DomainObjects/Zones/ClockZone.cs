using System;

namespace PixelBoardDevice.DomainObjects.Zones
{
    public class ClockZone : Zone, IFontableZone
    {
        public override int ZoneType => 5;
        public override string Name { get; set; } = "Часы";
        public int? FontId { get; set; }
        public int ClockType { get; set; }
        public int ClockFormat { get; set; }
        public bool AllowPeriodicTimeSync { get; set; }
        public bool AllowScheduledSync { get; set; }
        public int PeriodicSyncInterval { get; set; }
        public TimeSpan ScheduledTimeSync { get; set; }
        public bool UseWholeAlphabet => false;
        public string Alphabet => "1234567890:.";
        public string Sample { get; set; }
        public int? Alignment { get; set; }
    }
}
