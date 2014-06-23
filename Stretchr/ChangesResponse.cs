using Newtonsoft.Json;

namespace Stretchr
{
    /// <summary>
    ///     ChangesResponse represents a response that contains ChangeInfo.
    /// </summary>
    public class ChangesResponse : Response
    {
        /// <summary>
        ///     Changes holds the changes that occurred when a request was
        ///     made.
        /// </summary>
        [JsonProperty("~changes")]
        public ChangeInfo Changes { get; set; }
    }
}