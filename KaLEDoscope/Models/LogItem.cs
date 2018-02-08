namespace KaLEDoscope
{
    public class LogItem
    {
        public LogLevel LogLevel { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return $"{LogLevel}: {Message}";
        }
    }

    public enum LogLevel
    {
        Info,
        Debug,
        Warn,
        Error
    }
}