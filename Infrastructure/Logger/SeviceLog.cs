using NLog;
using System;

namespace Logger
{
    public class SeviceLog : ServiceInterfaces.ILogger
    {
        public event EventHandler<string> DebugRaised;
        public event EventHandler<string> ErrorRaised;
        public event EventHandler<string> InfoRaised;
        public event EventHandler<string> WarnRaised;

        private static NLog.Logger _logger = LogManager.GetCurrentClassLogger();

        public void Debug(object sender, string message)
        {
            DebugRaised?.Invoke(sender, message);
            _logger.Debug(message);
        }

        public void Error(object sender, string message)
        {
            ErrorRaised?.Invoke(sender, message);
            _logger.Error(message);
        }

        public void Error(object sender, string message, Exception exception)
        {
            ErrorRaised?.Invoke(sender, message);
            _logger.Error(exception, message);
            Debug(sender, $"{message}: {exception}");
        }

        public void Info(object sender, string message)
        {
            InfoRaised?.Invoke(sender, message);
            _logger.Info(message);
        }

        public void Warn(object sender, string message)
        {
            WarnRaised?.Invoke(sender, message);
            _logger.Warn(message);
        }
    }
}
