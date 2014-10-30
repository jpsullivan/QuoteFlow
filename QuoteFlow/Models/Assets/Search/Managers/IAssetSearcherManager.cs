using System.Collections.Generic;
using QuoteFlow.Models.Assets.Fields;
using QuoteFlow.Models.Assets.Search.Searchers;

namespace QuoteFlow.Models.Assets.Search.Managers
{
    /// <summary>
    /// Manager to obtain a list of <seealso cref="IAssetSearcher"/> objects as well 
    /// as <seealso cref="SearcherGroup"/> collections.
    /// </summary>
    public interface IAssetSearcherManager
    {
         /// <summary>
		/// Get searchers that are applicable for a given context. This is found through the
		/// <seealso cref="AssetSearcher.GetSearchRenderer()#isShown(SearchContext)"/> method.</summary>
		/// <param name="searcher">The user that is performing this action.</param>
		/// <param name="context">The context for the list of searchers. Must not be null.</param>
		/// <returns>Collection of <seealso cref="IAssetSearcher"/></returns>
		ICollection<IAssetSearcher<ISearchableField>> GetSearchers(User searcher, SearchContext context);

		/// <summary>
		/// Return all the active searchers in QuoteFlow. It will not return the searchers unless 
		/// they are associated with a field.
		/// </summary>
		/// <returns>All the searchers in QuoteFlow.</returns>
		ICollection<IAssetSearcher<ISearchableField>> AllSearchers { get; }

		/// <summary>
		/// Get all searcher groups. Note that the <see cref="SearcherGroup"/> will
		/// still appear even if no <seealso cref="IAssetSearcher"/> are shown for the group.
		/// </summary>
		/// <returns>Collection of <see cref="SearcherGroup"/>.</returns>
		ICollection<SearcherGroup> SearcherGroups {get;}

		/// <summary>
		/// Get a searcher by the searchers name.
		/// </summary>
		/// <param name="id">The string identifier returned by the <see cref="AssetSearcher.GetSearchInformation"/>.</param>
		/// <returns>The searcher matching the ID, null if none is found.</returns>
        IAssetSearcher<ISearchableField> GetSearcher(string id);

		/// <summary>
		/// Refreshes the <see cref="IAssetSearcher"/> cache.
		/// </summary>
		void Refresh();
    }
}