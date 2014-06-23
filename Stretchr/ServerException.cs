using System;

namespace Stretchr
{
    public class ServerException : StretchrException
    {
        private const string ServerExceptionPrefix = "The server produced an error";

        public ServerException()
            : this(ServerExceptionPrefix)
        {
        }

        public ServerException(string serverMessage)
            : base(ServerExceptionPrefix + ": " + serverMessage)
        {
        }

        public ServerException(string serverMessage, Exception inner)
            : base(ServerExceptionPrefix + ": " + serverMessage, inner)
        {
        }
    }
}