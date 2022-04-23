using System;

namespace KaLEDoscope
{
    public class LogItem
    {
        public LogLevel LogLevel { get; set; }
        public string Message { get; set; }
        public readonly TimeSpan TimeStamp = DateTime.Now.TimeOfDay;
        public string TimeSpanStr => TimeStamp.ToString(@"hh\:mm\:ss\.ff");
    }

    public enum LogLevel
    {
        Info,
        Debug,
        Warn,
        Error
    }
}
