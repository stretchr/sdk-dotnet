using Newtonsoft.Json;

namespace Stretchr
{
    /// <summary>
    ///     Error represents a single error returned from the server.
    /// </summary>
    public class Error
    {
        [JsonProperty("~message")]
        public string Message { get; set; }
    }
}