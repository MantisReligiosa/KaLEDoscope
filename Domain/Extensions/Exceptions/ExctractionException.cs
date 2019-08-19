using System;

namespace Extensions.Exceptions
{
    public class ExctractionException : Exception
    {
        public ExctractionException(string message)
            : base(message)
        {

        }
    }
}
