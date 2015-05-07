namespace QuoteFlow.Api.Infrastructure.Services
{
    /// <summary>
    /// A service result that also has an value.
    /// </summary>
    public interface IServiceOutcome<T> : IServiceResult
    {
        /// <summary>
        /// Returns the value that was returned by the service, or null.
        /// </summary>
        /// <returns>The value returned by the service, or null</returns>
        T ReturnedValue { get; }

        /// <summary>
        /// Returns the value that was returned by the service, or null.
        /// </summary>
        /// <returns>The value returned by the service, or null</returns>
        T Get();
    }
}