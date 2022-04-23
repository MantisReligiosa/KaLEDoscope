using System;
using System.Runtime.Serialization;

namespace CommandProcessing.Exceptions
{
    [Serializable]
    public class InvalidByteSequenceException : Exception
    {
        public InvalidByteSequenceException(string message) : base(message) { }

        protected InvalidByteSequenceException(SerializationInfo info, StreamingContext context)
        : base(info, context)
        {
        }
    }
}
