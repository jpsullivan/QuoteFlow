using QuoteFlow.Models.Assets.Search;
using QuoteFlow.Models.Search;

namespace QuoteFlow.Models.Assets.Fields
{
    public interface ISearchableField : IField
    {
        /// <summary>
        /// Return <see cref="SearchHandler"/> for the field. This object tells QuoteFlow how to 
        /// search for values within the field.
        /// </summary>
        /// <returns>
        /// The SearchHandler associated with the field. Can return <code>null</code> when
        /// no searcher is associated with the field. This will mainly happen when a customfield is 
        /// configured to have no searcher. 
        /// </returns>
        SearchHandler CreateAssociatedSearchHandler();
    }
}