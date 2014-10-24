using System.Collections.Generic;
using QuoteFlow.Models.Assets.Fields;
using QuoteFlow.Models.Assets.Search.Searchers;
using QuoteFlow.Models.Search.Jql.Clauses;

namespace QuoteFlow.Models.Assets.Search
{

    /// <summary>
    /// Holds the link between an <seealso cref="AssetSearcher"/> and the JQL clauses (as
    /// <seealso cref="ClauseRegistration"/>s) that it uses in the background to implement searching. 
    /// This relationship is kept within QuoteFlow so that is can perform the JQL to Asset navigator
    /// translation.
    /// </summary>
	public class SearcherRegistration
    {
        public IAssetSearcher<ISearchableField> AssetSearcher { get; set; } 
        public IEnumerable<ClauseRegistration> ClauseHandlers { get; set; }

        public SearcherRegistration(IAssetSearcher<ISearchableField> searcher, IClauseHandler clauseHandler)
            : this(searcher, new ClauseRegistration(clauseHandler))
        {   
        }

        public SearcherRegistration(IAssetSearcher<ISearchableField> searcher, IEnumerable<ClauseRegistration> clauseHandlers)
        {
            AssetSearcher = searcher;
            ClauseHandlers = clauseHandlers;
        }

        public SearcherRegistration(IAssetSearcher<ISearchableField> searcher, ClauseRegistration clauseRegistration)
            : this(searcher, new List<ClauseRegistration> {clauseRegistration})
        {   
        }
	}
}