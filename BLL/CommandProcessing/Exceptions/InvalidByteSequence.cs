using System;

namespace CommandProcessing.Exceptions
{
    public class InvalidByteSequenceException : Exception
    {
        public InvalidByteSequenceException(string message) : base(message) { }
    }
}
