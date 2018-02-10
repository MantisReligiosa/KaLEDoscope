namespace DomainData
{
    public class Brightness
    {
        public Mode Mode { get; set; }
        public int ManualValue { get; set; }
    }

    public enum Mode
    {
        Auto,
        Manual,
        Scheduled
    }
}