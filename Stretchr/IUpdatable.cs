namespace Stretchr
{
    /// <summary>
    ///     IUpdatable describes objects capable of being updated in reaction to
    ///     changes in the Response.
    /// </summary>
    public interface IUpdatable
    {
        /// <summary>
        ///     Set sets the key to the value for this resource.
        /// </summary>
        /// <param name="key">The key of the value to set.</param>
        /// <param name="value">The value to assign to the key.</param>
        void Set(string key, dynamic value);
    }
}