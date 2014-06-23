using System;

namespace Stretchr
{
    public abstract class StretchrException : Exception
    {
        protected StretchrException()
        {
        }

        protected StretchrException(string message)
            : base(message)
        {
        }

        protected StretchrException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}