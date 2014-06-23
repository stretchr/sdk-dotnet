using System.Collections.Generic;

namespace Stretchr.Tests
{
    public class FakeTransport : ITransport
    {
        public FakeTransport()
        {
            Requests = new Queue<Request>();
            Responses = new Queue<dynamic>();
        }

        public Queue<Request> Requests { get; private set; }
        public Queue<dynamic> Responses { get; private set; }

        public T MakeRequest<T>(Request request) where T : Response
        {
            Requests.Enqueue(request);
            if (Responses.Count > 0)
            {
                return (T) Responses.Dequeue();
            }
            return null;
        }
    }
}