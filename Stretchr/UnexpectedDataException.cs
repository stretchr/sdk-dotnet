using System;

namespace Stretchr
{
    public class UnexpectedDataException : StretchrException
    {
        public UnexpectedDataException() : this("Unexpected data")
        {
        }

        public UnexpectedDataException(string message) : base(message)
        {
        }

        public UnexpectedDataException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}