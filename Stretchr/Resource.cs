using System.Collections.Generic;
using Newtonsoft.Json;

namespace Stretchr
{
    /// <summary>
    ///     Resource represents a single resource in Stretchr. You can use this
    ///     class directly, or as a superclass for your own types.
    /// </summary>
    public class Resource : Dictionary<string, dynamic>, IUpdatable
    {
        /// <summary>
        ///     Id gets the identifier for this Resource.
        /// </summary>
        [JsonIgnore]
        public string Id
        {
            get { return Get("~id"); }
        }

        /// <summary>
        ///     Sets the value to the specified key.
        /// </summary>
        /// <param name="key">The key whose value should be set.</param>
        /// <param name="value">The value to set to the key.</param>
        public void Set(string key, dynamic value)
        {
            this[key] = value;
        }

        /// <summary>
        ///     Gets the value for the specified key or returns null if the
        ///     key is not set.
        /// </summary>
        /// <param name="key">The key for the value to get.</param>
        /// <returns>Returns the value for the specified key, or null.</returns>
        protected dynamic Get(string key)
        {
            dynamic value;
            return TryGetValue(key, out value) ? value : null;
        }
    }
}