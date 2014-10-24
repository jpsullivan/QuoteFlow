using Lucene.Net.Documents;
using QuoteFlow.Models.Assets.Fields;
using QuoteFlow.Models.Assets.Search.Searchers.Information;
using QuoteFlow.Models.Search.Jql.Query.Clause;

namespace QuoteFlow.Models.Assets.Search.Searchers
{
    /// <summary>
    /// The interface defines an object responsible for all search related activities in the Asset Navigator.
    /// The interface operates similar to the <seealso cref="Field"/> objects (e.g. <seealso cref="OrderableField"/>. 
    /// It is responsible for populating itself from <seealso cref="ActionParams"/> and <seealso cref="SearchRequest"/> 
    /// as well as all rendering related activities.
    /// 
    /// <seealso cref="CustomField"/>Searchers should still extend the sub-interface <seealso cref="CustomFieldSearcher"/>.
    /// </summary>
    public interface IAssetSearcher<T> where T : ISearchableField
    {
        /// <summary>
        /// Initialises the searcher with a given field.
        /// </summary>
        /// <param name="field">The field object. This <strong>may</strong> be null. (So you can have searchers on non-fields) </param>
        void Init(T field);

        /// <summary>
        /// Provides an object that contains information about the Searcher.
        /// </summary>
        /// <returns> the search information provider for this searcher. </returns>
        ISearcherInformation<T> SearchInformation { get; }

        /// <summary>
        /// Provides an object that will allow you to transform raw request parameters to field holder values and
        /// field holder values to <seealso cref="IClause"/> search representations.
        /// </summary>
        /// <returns> the search input handler for this searcher. </returns>
        SearchInputTransformer SearchInputTransformer { get; }

        /// <summary>
        /// Provides an object that will allow you to render the edit and view html for a searcher. This also provides
        /// methods that indicate if the view and edit methods should be invoked.
        /// </summary>
        /// <returns> the search renderer for this searcher. </returns>
        SearchRenderer SearchRenderer { get; }
    }
}