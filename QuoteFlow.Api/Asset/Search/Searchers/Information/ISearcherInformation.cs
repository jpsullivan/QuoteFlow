using System.Collections.Generic;
using QuoteFlow.Api.Asset.Fields;
using QuoteFlow.Api.Asset.Index.Indexers;

namespace QuoteFlow.Api.Asset.Search.Searchers.Information
{
    /// <summary>
    /// Identifies a searcher by name and provides a display name that is i18n'ed.
    /// 
    /// @since v4.0
    /// </summary>
    public interface ISearcherInformation<T> where T : ISearchableField
    {
        /// <summary>
        /// The unique id of the searcher. 
        /// </summary>
        string Id { get; }

        /// <summary>
        /// The i18n key that is used to lookup the searcher's name when it is displayed. 
        /// </summary>
        string NameKey { get; }

        /// <summary>
        /// The field that this searcher was initialised with. If the searcher has not yet been initialised,
        /// this will return null.
        /// </summary>
        T Field { get; }

        /// <summary>
        /// Returns a list of <see cref="IFieldIndexer"/> objects. The objects should be initialised and ready for action.
        /// </summary>
        IEnumerable<IFieldIndexer> RelatedIndexers { get; }

        /// <summary>
        /// The searcher group the searcher should be placed in. Really only useful for system fields as custom
        /// fields are forced into the <see cref="SearcherGroupType#CUSTOM"/> group.
        /// </summary>
        SearcherGroupType SearcherGroupType { get; }

    }
}