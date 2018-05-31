using System;

namespace CommandProcessing.Exceptions
{
    public class ExchangeException : Exception
    {
        public ExchangeException(string message)
            : base(message)
        { }
    }
}
