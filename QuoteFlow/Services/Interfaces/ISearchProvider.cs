using QuoteFlow.Infrastructure.Paging;
using QuoteFlow.Models;
using QuoteFlow.Models.Search;
using QuoteFlow.Models.Search.Jql.Query;

namespace QuoteFlow.Services.Interfaces
{
    /// <summary>
    /// Allows users to run structured searches against QuoteFlow Lucene index as opposed
    /// to database (SQL) based queries.
    /// 
    /// All search methods takes a <see cref="IQuery"/> which defines the criteria of the search,
    /// including any sort of supplemental information.
    /// </summary>
    public interface ISearchProvider
    {
        SearchResults Search(IQuery query, User searcher, IPagerFilter pager);
    }
}