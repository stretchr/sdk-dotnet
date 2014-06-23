using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Stretchr
{
    /// <summary>
    ///     Response is the abstract type for all server responses.  Specialist classes
    ///     add additinal helpers for certain types of response.
    /// </summary>
    public abstract class Response
    {
        /// <summary>
        ///     Errors gets an IEnumerable of errors that occurred when
        ///     making the request.
        /// </summary>
        [JsonProperty("~errors")]
        public IEnumerable<Error> Errors { get; set; }

        /// <summary>
        ///     Status contains the HttpStatusCode that occurred when
        ///     the request was made.
        /// </summary>
        [JsonProperty("~status")]
        public HttpStatusCode Status { get; set; }

        /// <summary>
        ///     IsSuccess gets whether the request was successful or not.
        /// </summary>
        [JsonIgnore]
        public bool IsSuccess
        {
            get { return ((int) Status >= 100) && ((int) Status < 400); }
        }

        /// <summary>
        ///     ErrorMessage gets the first error message from the Errors list.
        ///     Or returns null if no errors occurred.
        /// </summary>
        [JsonIgnore]
        public string ErrorMessage
        {
            get
            {
                if (Errors == null) return null;
                return !Errors.Any() ? null : Errors.First().Message;
            }
        }
    }
}