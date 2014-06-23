using System.Net.Http;

namespace Stretchr
{
    /// <summary>
    ///     Request holds the details of a single request.
    /// </summary>
    public class Request
    {
        public Request(string url, HttpMethod method, object dataObject = null)
        {
            Url = url;
            HttpMethod = method;
            DataObject = dataObject;
        }

        public string Url { get; private set; }
        public HttpMethod HttpMethod { get; private set; }
        public object DataObject { get; private set; }
    }
}