using Newtonsoft.Json;

namespace Stretchr.Tests
{
    /// <summary>
    ///     TestResource is a simple resource object used for testing.
    /// </summary>
    internal class TestResource : Resource
    {
        [JsonIgnore]
        public string Name
        {
            get { return Get("name"); }
            set { Set("name", value); }
        }
    }
}