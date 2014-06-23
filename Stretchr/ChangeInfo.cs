using System.Collections.Generic;
using Newtonsoft.Json;

namespace Stretchr
{
    /// <summary>
    ///     ChangeInfo holds information about changes that occurred on the server.
    /// </summary>
    public class ChangeInfo
    {
        [JsonProperty("~created")]
        public int Created { get; set; }

        [JsonProperty("~updated")]
        public int Updated { get; set; }

        [JsonProperty("~deleted")]
        public int Deleted { get; set; }

        [JsonProperty("~deltas")]
        public List<Dictionary<string, dynamic>> Deltas { get; set; }
    }
}