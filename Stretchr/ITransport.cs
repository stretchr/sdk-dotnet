namespace Stretchr
{
    /// <summary>
    ///     ITransport represents the interface for objects capable of
    ///     communicating with remote services.
    /// </summary>
    public interface ITransport
    {
        /// <summary>
        ///     MakeRequest makes the specified
        ///     <param name="request">request</param>
        ///     over the
        ///     transport and returns the Response specified type
        ///     <typeparam name="T">T</typeparam>
        ///     .
        /// </summary>
        /// <typeparam name="T">The type of Response to return.</typeparam>
        /// <param name="request">The Request to make.</param>
        /// <returns></returns>
        T MakeRequest<T>(Request request) where T : Response;
    }
}