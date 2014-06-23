using System;

namespace Stretchr
{
    /// <summary>
    ///     Client allows you to interact with Stretchr services.
    /// </summary>
    public class Client
    {
        public Client(string account, string project, string key, ITransport transport = null, string host = null,
            string apiVersion = "1.1", string protocol = "https")
        {
            Account = account;
            Project = project;
            Key = key;

            // setup host by default
            Host = String.IsNullOrEmpty(host) ? account + ".stretchr.com" : host;
            ApiVersion = apiVersion;
            Protocol = protocol;
            Transport = transport ?? new HttpTransport();
        }

        public string Account { get; private set; }
        public string Project { get; private set; }
        public string Key { get; private set; }
        public string Host { get; private set; }
        public string ApiVersion { get; private set; }
        public string Protocol { get; private set; }
        public ITransport Transport { get; private set; }

        public RequestBuilder At(string path)
        {
            return new RequestBuilder(this, path);
        }

        public string UrlBase()
        {
            return String.Concat(Protocol, "://", Host, "/api/v", ApiVersion, "/", Project, "/");
        }
    }
}