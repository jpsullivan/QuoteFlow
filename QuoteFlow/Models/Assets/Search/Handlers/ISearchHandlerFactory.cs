using QuoteFlow.Models.Assets.Fields;

namespace QuoteFlow.Models.Assets.Search.Handlers
{
    /// <summary>
    /// Factory to create <seealso cref="SearchHandler"/> instances.
    /// </summary>
    public interface ISearchHandlerFactory
    {
        /// <summary>
        /// Create the <see cref="SearchHandler"/> using for the passed field.
        /// </summary>
        /// <param name="field">The field to create the handler for.</param>
        /// <returns>A new SearchHandler for the passed field. Should never return null.</returns>
        SearchHandler CreateHandler(ISearchableField field);
    }
}