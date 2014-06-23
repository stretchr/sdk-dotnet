namespace Stretchr
{
    public static class ParameterHelperExtensions
    {
        /// <summary>
        ///     Order sets the order in which results should be returned.
        /// </summary>
        /// <param name="rb">The RequestBuilder to set the value on.</param>
        /// <param name="orderBy">The value to set.</param>
        /// <returns>Returns this for chaining.</returns>
        public static RequestBuilder Order(this RequestBuilder rb, string orderBy)
        {
            return rb.With("order", orderBy);
        }

        /// <summary>
        ///     WithTotal specifies that the total value should be returned in collection requests.
        /// </summary>
        /// <param name="rb">The RequestBuilder to set the value on.</param>
        /// <returns>Returns this for chaining.</returns>
        public static RequestBuilder WithTotal(this RequestBuilder rb)
        {
            return rb.With("total", 1.ToString());
        }

        /// <summary>
        ///     WithVersions specifies that the server should maintain previous
        ///     versions of the data when replacing or updating.
        /// </summary>
        /// <param name="rb">The RequestBuilder to set the value on.</param>
        /// <returns>Returns this for chaining.</returns>
        public static RequestBuilder WithVersions(this RequestBuilder rb)
        {
            return rb.With("versioning", 1.ToString());
        }

        /// <summary>
        ///     WithAggregation sets the aggregation query to make.
        /// </summary>
        /// <param name="rb">The RequestBuilder to set the value on.</param>
        /// <param name="aggregationQuery">The value to set.</param>
        /// <returns>Returns this for chaining.</returns>
        public static RequestBuilder WithAggregation(this RequestBuilder rb, string aggregationQuery)
        {
            return rb.With("agg", aggregationQuery);
        }

        /// <summary>
        ///     Skip tells the server to skip the specified amount of records before it starts
        ///     returning others.
        ///     For paging, it is recommended that you use Page(pageSize, currentPage) instead.
        /// </summary>
        /// <param name="rb">The RequestBuilder to set the value on.</param>
        /// <param name="skip">The value to set.</param>
        /// <returns>Returns this for chaining.</returns>
        public static RequestBuilder Skip(this RequestBuilder rb, int skip)
        {
            return rb.With("skip", skip.ToString());
        }

        /// <summary>
        ///     Limit tells the server to return only the specified number of resources.
        ///     For paging, it is recommended that you use Page(pageSize, currentPage) instead.
        /// </summary>
        /// <param name="rb">The RequestBuilder to set the value on.</param>
        /// <param name="limit">The value to set.</param>
        /// <returns>Returns this for chaining.</returns>
        public static RequestBuilder Limit(this RequestBuilder rb, int limit)
        {
            return rb.With("limit", limit.ToString());
        }

        /// <summary>
        ///     Page sets the page of resources to return.
        /// </summary>
        /// <param name="rb"></param>
        /// <param name="pageSize">The number of resources per page.</param>
        /// <param name="currentPage">The page number to get (starting at 1)</param>
        /// <returns>Returns this for chaining.</returns>
        public static RequestBuilder Page(this RequestBuilder rb, int pageSize, int currentPage)
        {
            return rb.Limit(pageSize).Skip((currentPage - 1)*pageSize);
        }
    }
}