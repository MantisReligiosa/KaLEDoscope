using System;
using System.Runtime.Serialization;

namespace CommandProcessing.Exceptions
{
    [Serializable]
    public class ExchangeException : Exception
    {
        public ExchangeException(string message)
            : base(message)
        { }

        protected ExchangeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
