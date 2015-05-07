using QuoteFlow.Api.Util;

namespace QuoteFlow.Api.Infrastructure.Services
{
    /// <summary>
    /// This interface defines a service method call result in QuoteFlow that can contain human readable errors. New service
    /// methods should prefer the generic <seealso cref="IServiceOutcome{T}"/>.
    /// </summary>
    public interface IServiceResult
    {
        /// <returns> true if there are no errors, false otherwise. </returns>
        bool IsValid();

        /// <returns> an <see cref="IErrorCollection"/> that contains any errors that may have happened as a result of the validations. </returns>
        IErrorCollection ErrorCollection { get; }
    }
}