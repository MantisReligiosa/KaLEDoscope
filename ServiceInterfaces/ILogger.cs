using System;

namespace ServiceInterfaces
{
    public interface ILogger
    {
        void Debug(object sender, string message);
        void Info(object sender, string message);
        void Warn(object sender, string message);
        void Error(object sender, string message);
        void Error(object sender, string message, Exception exception);

        event EventHandler<string> DebugRaised;
        event EventHandler<string> InfoRaised;
        event EventHandler<string> WarnRaised;
        event EventHandler<string> ErrorRaised;
    }
}
