namespace KaLEDoscope.ViewModel
{
    public class ConfigViewModel
    {
        public int AutosavePeriod { get; set; }
        public string AutosaveFilename { get; set; }
        public int ScanPort { get; set; }
        public int ScanPeriod { get; set; }
        public int RequestPort { get; set; }
        public int ResponceTimeout { get; set; }
    }
}
