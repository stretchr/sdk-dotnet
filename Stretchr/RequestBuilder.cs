using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Stretchr
{
    /// <summary>
    ///     RequestBuilder describes a request and provides methods through which
    ///     the request can be made.
    /// </summary>
    public class RequestBuilder
    {
        private IDictionary<string, IList<string>> _parameters;

        public RequestBuilder(Client client, string path)
        {
            Path = path;
            Client = client;
        }

        /// <summary>
        ///     Path is the relative path for the request.
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        ///     Client is the Client that helps this RequestBuilder build URLs
        ///     and perform actions.
        /// </summary>
        public Client Client { get; private set; }

        /// <summary>
        ///     Parameters gets the query parameters for this request.
        /// </summary>
        public IDictionary<string, IList<string>> Parameters
        {
            get
            {
                if (_parameters == null)
                {
                    _parameters = new Dictionary<string, IList<string>>();
                    var keyVal = new List<string>();
                    keyVal.Add(Client.Key);
                    _parameters.Add("key", keyVal);
                }
                return _parameters;
            }
        }

        /// <summary>
        ///     With adds a parameter to the request.
        /// </summary>
        /// <param name="key">Key is the name of the parameter to add.</param>
        /// <param name="value">Value is the value of the parameter.</param>
        /// <returns>Returns this for chaining.</returns>
        public RequestBuilder With(string key, string value)
        {
            if (!Parameters.ContainsKey(key))
            {
                Parameters[key] = new List<string>();
            }
            Parameters[key].Add(value);
            return this;
        }

        /// <summary>
        ///     Where adds a filter to the request.
        /// </summary>
        /// <param name="field">The field to filter on.</param>
        /// <param name="check">The filter check.</param>
        /// <returns>Returns this for chaining.</returns>
        public RequestBuilder Where(string field, string check)
        {
            // make sure the field starts with :
            field = field.StartsWith(":") ? field : ":" + field;

            // add the field
            return With(field, check);
        }

        /// <summary>
        ///     PathAndParams is the relative URL with encoded parameters as a string.
        /// </summary>
        /// <returns></returns>
        public string PathAndParams()
        {
            return String.Concat(Path, ParamString());
        }

        /// <summary>
        ///     ParamString is the encoded parameters as a URL string.
        /// </summary>
        /// <returns></returns>
        private string ParamString()
        {
            if (Parameters.Count <= 0) return String.Empty;
            List<string> pairs = (from key in Parameters.Keys
                from value in Parameters[key]
                select String.Format("{0}={1}", Uri.EscapeDataString(key), Uri.EscapeDataString(value))).ToList();
            return String.Concat("?", String.Join("&", pairs));
        }

        /// <summary>
        ///     Url gets the full URL for the request, including the UrlBase from the client.
        /// </summary>
        /// <returns>Returns a string containing the absolute URL of the request.</returns>
        public string Url()
        {
            return String.Concat(Client.UrlBase(), PathAndParams());
        }

        /// <summary>
        ///     ToRequest creates a Request object from this RequestBuilder.
        /// </summary>
        /// <param name="method">The HTTP method for the request.</param>
        /// <param name="dataObject">An optional data object for the body of the request.</param>
        /// <returns>Returns a Request object.</returns>
        private Request ToRequest(HttpMethod method, object dataObject = null)
        {
            return new Request(Url(), method, dataObject);
        }

        private bool ApplyChangesToObject(Dictionary<string, dynamic> delta, dynamic obj)
        {
            var updatable = obj as IUpdatable;
            if (updatable == null) return false;

            foreach (string key in delta.Keys)
            {
                updatable.Set(key, delta[key]);
            }
            return true;
        }

        private bool ApplyChanges(ChangeInfo changes, dynamic obj)
        {
            var list = obj as IList;

            if (list == null)
            {
                if (changes.Deltas.Count != 1)
                    throw new UnexpectedDataException(
                        "When applying changes to single object, only a single delta object is expected but got " +
                        changes.Deltas.Count);

                Dictionary<string, dynamic> delta = changes.Deltas[0];
                ApplyChangesToObject(delta, obj);
            }
            else
            {
                int deltaIndex = 0;

                foreach (object item in list)
                {
                    ApplyChangesToObject(changes.Deltas[deltaIndex], item);
                    deltaIndex++;
                }
            }
            return true;
        }

        #region Actions

        /// <summary>
        ///     Read performs a read action.
        /// </summary>
        /// <returns>Returns a default DataResponse.</returns>
        public
            DataResponse<Dictionary<string, dynamic>> Read()
        {
            return Client.Transport.MakeRequest<DataResponse<Dictionary<string, dynamic>>>(ToRequest(HttpMethod.Get));
        }

        /// <summary>
        ///     Read performs a read action expecting a return of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of object expected.</typeparam>
        /// <returns>Returns a DataResponse of the specified type.</returns>
        public DataResponse<T> Read<T>()
        {
            return Client.Transport.MakeRequest<DataResponse<T>>(ToRequest(HttpMethod.Get));
        }

        /// <summary>
        ///     MustReadOne performs a read action and returns the object or throws
        ///     an exception if the read fails for any reason.
        /// </summary>
        /// <returns>Returns a default Dictionary containing the object.</returns>
        public Dictionary<string, dynamic> MustReadOne()
        {
            return MustReadOne<Dictionary<string, dynamic>>();
        }

        /// <summary>
        ///     MustReadOne performs a read action and returns the object cast to the
        ///     specific type.  An exception is thrown is the read fails for any reason.
        /// </summary>
        /// <typeparam name="T">The type of the object to read.</typeparam>
        /// <returns>Returns the read object.</returns>
        public T MustReadOne<T>()
        {
            DataResponse<T> response = Read<T>();
            if (!response.IsSuccess)
            {
                throw new ServerException(response.ErrorMessage);
            }
            if (response.Items().Count != 1)
            {
                throw new UnexpectedDataException(String.Format("MustReadOne expected 1 item but got {0}.",
                    response.Items().Count));
            }
            return response.Items().First();
        }

        /// <summary>
        ///     MustReadMany performs a read action and returns an IList of the appropriate
        ///     type containing the results.  An exception is thrown if the read fails for any
        ///     reason.
        /// </summary>
        /// <typeparam name="T">The type of the objects to read.</typeparam>
        /// <returns>Returns the read objects.</returns>
        public IList<T> MustReadMany<T>()
        {
            DataResponse<T> response = Read<T>();
            if (!response.IsSuccess)
            {
                throw new ServerException(response.ErrorMessage);
            }
            return response.Items();
        }

        /// <summary>
        ///     MustReadMany performs a read action and returns an IList contianing the results.
        ///     An exception is thrown if the read fails for any reason.
        /// </summary>
        /// <returns>Returns the read objects.</returns>
        public IList<Dictionary<string, dynamic>> MustReadMany()
        {
            return MustReadMany<Dictionary<string, dynamic>>();
        }

        /// <summary>
        ///     Delete performs a delete action.
        /// </summary>
        /// <returns></returns>
        public ChangesResponse Delete()
        {
            return Client.Transport.MakeRequest<ChangesResponse>(ToRequest(HttpMethod.Delete));
        }

        /// <summary>
        ///     Create performs a request that creates the specified object.
        /// </summary>
        /// <param name="obj">The object to create.</param>
        /// <returns>Returns a ChangesResponse object.</returns>
        public ChangesResponse Create(object obj)
        {
            var response = Client.Transport.MakeRequest<ChangesResponse>(ToRequest(HttpMethod.Post, obj));
            if (response.IsSuccess)
            {
                ApplyChanges(response.Changes, obj);
            }
            return response;
        }

        /// <summary>
        ///     Replace performs a request that replaces the specified object.
        /// </summary>
        /// <param name="obj">The object to replace.</param>
        /// <returns>Returns a ChangesResponse object.</returns>
        public ChangesResponse Replace(object obj)
        {
            var response = Client.Transport.MakeRequest<ChangesResponse>(ToRequest(HttpMethod.Put, obj));
            if (response.IsSuccess)
            {
                ApplyChanges(response.Changes, obj);
            }
            return response;
        }

        /// <summary>
        ///     Update performs a request that updates the specified object.
        /// </summary>
        /// <param name="obj">The object to update.</param>
        /// <returns>Returns a ChangesResponse object.</returns>
        public ChangesResponse Update(object obj)
        {
            var response = Client.Transport.MakeRequest<ChangesResponse>(ToRequest(new HttpMethod("PATCH"), obj));
            if (response.IsSuccess)
            {
                ApplyChanges(response.Changes, obj);
            }
            return response;
        }

        #endregion
    }
}