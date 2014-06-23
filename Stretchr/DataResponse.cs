using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Stretchr
{
    /// <summary>
    ///     DataResponse represents a single response from the server.
    /// </summary>
    public class DataResponse<T> : Response
    {
        [JsonProperty("~data")]
        public object Data { get; set; }

        public IList<T> Items()
        {
            IList<T> items = new List<T>();

            var dataObject = Data as JObject;
            if (dataObject == null) throw new NullReferenceException("Data expected to be JObject.");
            if (dataObject["~items"] != null)
            {
                // collection
                JToken dataList = dataObject["~items"];
                foreach (JToken item in dataList.AsEnumerable())
                {
                    items.Add(item.ToObject<T>());
                }
            }
            else
            {
                // single item
                items.Add((Data as JObject).ToObject<T>());
            }

            return items;
        }
    }
}