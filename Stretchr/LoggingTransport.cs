using System.IO;

namespace Stretchr
{
    /// <summary>
    ///     LoggingTransport is an ITransport decorator that logs out
    ///     activity to the specified Stream,
    ///     before optionally passing execution on to the actual ITransport specified.
    /// </summary>
    public class LoggingTransport : ITransport
    {
        public LoggingTransport(Stream output, ITransport actualTransport = null)
        {
            ActualTransport = actualTransport;
            Output = new StreamWriter(output);
        }

        private ITransport ActualTransport { get; set; }
        private StreamWriter Output { get; set; }

        public T MakeRequest<T>(Request request) where T : Response
        {
            Output.WriteLine("Stretchr: (Request) " + request);

            if (ActualTransport != null)
            {
                var response = ActualTransport.MakeRequest<T>(request);
                if (response != null)
                {
                    Output.WriteLine("Stretchr: (Response) " + response);
                }
                else
                {
                    Output.WriteLine("Stretchr: (Response) null");
                }
                return response;
            }

            Output.WriteLine("Stretchr: (No actual ITransport set in LoggingTransport - skipping execution)");

            return null;
        }
    }
}