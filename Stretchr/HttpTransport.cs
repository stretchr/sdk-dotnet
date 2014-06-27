using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

namespace Stretchr
{
    /// <summary>
    ///     HttpTransport is an ITransport that makes real requests over HTTP.
    /// </summary>
    public class HttpTransport : ITransport
    {
        private readonly JsonSerializer _jsonSerializer;
        private readonly string _version;

        public HttpTransport()
        {
            _jsonSerializer = new JsonSerializer();
            _version = "dotnet-1.1.0.0";
        }

        /// <summary>
        ///     MakeRequest makes a real HTTP request.
        /// </summary>
        /// <typeparam name="T">
        ///     The Response type expected as a result of the
        ///     specified Request.
        /// </typeparam>
        /// <param name="request">The Request to make.</param>
        /// <returns>Returns a Response object of the specified type.</returns>
        public T MakeRequest<T>(Request request) where T : Response
        {
            // prepare the request
            var req = (HttpWebRequest) WebRequest.Create(request.Url);
            req.Method = request.HttpMethod.Method;
            req.ContentType = "application/json";
            req.Headers["X-Stretchr-SDK"] = _version;

            // if there's a body object - serialize it
            if (request.DataObject != null)
            {
                Stream requestStream = null;
                var requestWaiter = new ManualResetEvent(false);
                // setup the request body
                req.BeginGetRequestStream(cb =>
                {
                    var aReq = cb.AsyncState as HttpWebRequest;
                    if (aReq == null)
                    {
                        throw new ArgumentNullException("request");
                    }
                    requestStream = aReq.EndGetRequestStream(cb);
                    requestWaiter.Set();
                }, req);
                requestWaiter.WaitOne();

                var encoding = new UTF8Encoding();
                Byte[] byteArray = encoding.GetBytes(JsonConvert.SerializeObject(request.DataObject));
                requestStream.Write(byteArray, 0, byteArray.Length);
                _jsonSerializer.Serialize(new StreamWriter(requestStream), request.DataObject);
            }

            var responseWaiter = new ManualResetEvent(false);
            Stream reader = null;

            req.BeginGetResponse(cb =>
            {
                var aReq = cb.AsyncState as WebRequest;

                if (aReq == null)
                {
                    throw new ArgumentNullException("request");
                }

                // Get the response in a way that ignores exceptions
                // becuase the HTTP package will throw an exception for
                // non-200 responses.
                WebResponse aResp;
                try
                {
                    aResp = aReq.EndGetResponse(cb);
                }
                catch (WebException e)
                {
                    aResp = e.Response as HttpWebResponse;
                    if (aResp == null) throw;
                }

                // now we have the repsonse, get the response stream to read it.
                reader = aResp.GetResponseStream();

                // we're done
                responseWaiter.Set();
            }, req);
            responseWaiter.WaitOne();

            // deserialize the response
            var response = _jsonSerializer.Deserialize<T>(new JsonTextReader(new StreamReader(reader)));

            return response;
        }
    }
}