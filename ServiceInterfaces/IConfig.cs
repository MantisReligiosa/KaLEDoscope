namespace ServiceInterfaces
{
    public interface IConfig
    {
        string AutosaveFilename { get; set; }
        int AutosavePeriod { get; set; }
        int RequestPort { get; set; }
        int ResponceTimeout { get; set; }
        int ScanPeriod { get; set; }
        int ScanPort { get; set; }

        void Save();
    }
}