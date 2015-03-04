using Lucene.Net.Search;
using QuoteFlow.Api.Infrastructure.Paging;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Search;

namespace QuoteFlow.Api.Services
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
        /// <summary>
        /// Search the index, and only return issues that are in the pager's range.
        /// <em>Note: that this method returns read only <see cref="QuoteFlow.Api.Asset"/> objects, and should not be
        /// used where you need the issue for update</em>.
        /// 
        /// Also note that if you are only after the number of search results use
        /// <see cref="SearchCount"/> as it provides better performance.
        /// </summary>
        /// <param name="query"> contains the information required to perform the search. </param>
        /// <param name="searcher"> the user performing the search, which will be used to create a permission filter that filters out
        /// any of the results the user is not able to see and will be used to provide context for the search. </param>
        /// <param name="pager"> Pager filter (use <see cref="IPagerFilter.GetUnlimitedFilter()"/> to get all issues).
        /// </param>
        /// <returns>A <see cref="SearchResults"/> containing the resulting issues.</returns>
        SearchResults Search(IQuery query, User searcher, IPagerFilter pager);

        /// <summary>
        /// Search the index, and only return issues that are in the pager's range while AND'ing the raw lucene query
        /// to the generated query from the provided searchQuery.
        /// 
        /// <em>Note that this method returns read only <see cref="QuoteFlow.Api.Asset"/> objects, and should not be
        /// used where you need the issue for update</em>.
        /// 
        /// Also note that if you are only after the number of search results use
        /// <see cref="SearchCount"/> as it provides better performance.
        /// </summary>
        /// <param name="query"> contains the information required to perform the search. </param>
        /// <param name="searcher"> the user performing the search, which will be used to create a permission filter that filters out
        /// any of the results the user is not able to see and will be used to provide context for the search. </param>
        /// <param name="pager"> Pager filter (use <see cref="IPagerFilter#getUnlimitedFilter()"/> to get all issues). </param>
        /// <param name="andQuery"> raw lucene Query to AND with the request.
        /// </param>
        /// <returns>A <see cref="SearchResults"/> containing the resulting issues.</returns>
        SearchResults Search(IQuery query, User searcher, IPagerFilter pager, global::Lucene.Net.Search.Query andQuery);

        /// <summary>
        /// Search the index, and only return issues that are in the pager's range while AND'ing the raw lucene query
        /// to the generated query from the provided searchQuery, not taking into account any security
        /// constraints.
        /// 
        /// Do not use this method, user <see cref="Search(IQuery,Api.Models.User,IPagerFilter,Lucene.Net.Search.Query)"/>
        /// instead, this should only be used when performing administrative tasks where you need to know ALL the issues
        /// that will be affected.
        /// 
        /// <em>Note that this method returns read only <see cref="QuoteFlow.Api.Asset"/> objects, and should not be
        /// used where you need the issue for update</em>.  Also note that if you are only after the number of search
        /// results use <see cref="SearchCount"/> as it provides better performance.
        /// </summary>
        /// <param name="query"> contains the information required to perform the search. </param>
        /// <param name="searcher"> the user performing the search which will be used to provide context for the search. </param>
        /// <param name="pager"> Pager filter (use <see cref="IPagerFilter#getUnlimitedFilter()"/> to get all issues). </param>
        /// <param name="andQuery"> raw lucene Query to AND with the request.
        /// </param>
        /// <returns> A <see cref="SearchResults"/> containing the resulting issues.</returns>
        SearchResults SearchOverrideSecurity(IQuery query, User searcher, IPagerFilter pager, global::Lucene.Net.Search.Query andQuery);

        /// <summary>
        /// Return the number of issues matching the provided search criteria.
        /// <b>Note:</b> This does not load all results into memory and provides better performance than
        /// <see cref="Search"/>
        /// </summary>
        /// <param name="query"> contains the information required to perform the search. </param>
        /// <param name="searcher"> the user performing the search which will be used to provide context for the search.
        /// </param>
        /// <returns>Number of matching results.</returns>
        long SearchCount(IQuery query, User searcher);

        /// <summary>
        /// Return the number of issues matching the provided search criteria, overridding any security constraints.
        /// 
        /// Do not use this method, use <see cref="SearchCount"/>
        /// instead, this should only be used when performing administrative tasks where you need to know ALL the issues
        /// that will be affected.
        /// 
        /// Note: This does not load all results into memory and provides better performance than <see cref="Search"/>.
        /// </summary>
        /// <param name="query">Contains the information required to perform the search.</param>
        /// <param name="searcher">The user performing the search which will be used to provide context for the search.</param>
        /// <returns>Number of matching results.</returns>
        long SearchCountOverrideSecurity(IQuery query, User searcher);

        /// <summary>
        /// Run a search based on the provided search criteria and, for each match, call Collector.collect().
        /// Collectors are low level Lucene classes, but they allow issues to be placed into buckets very quickly. Many of
        /// QuoteFlow's graphs and stats are generated in this manner. This method is useful if you need to execute a query in
        /// constant-memory (i.e. you do not want to load the results of your complete search into memory) and the query
        /// generated via JQL needs to be augmented with some custom Lucene query.
        /// </summary>
        /// <param name="query"> contains the information required to perform the search. </param>
        /// <param name="searcher"> the user performing the search which will be used to provide context for the search. </param>
        /// <param name="collector"> the Lucene object that will have collect called for each match. </param>
        /// <param name="andQuery"> additional Lucene query to be anded with the lucene query that will be generated from JQL </param>
        void Search(IQuery query, User searcher, Collector collector, global::Lucene.Net.Search.Query andQuery);

        /// <summary>
        /// Run a search based on the provided search criteria and, for each match, call Collector.collect().
        /// Collectors are low level Lucene classes, but they allow issues to be placed into buckets very quickly.
        /// Many of QuoteFlow's graphs and stats are generated in this manner. This method is useful if you need to execute a
        /// query in constant-memory (i.e. you do not want to load the results of your complete search into memory).
        /// </summary>
        /// <param name="query"> contains the information required to perform the search. </param>
        /// <param name="searcher"> the user performing the search which will be used to provide context for the search. </param>
        /// <param name="collector"> the Lucene object that will have collect called for each match.
        /// </param>
        void Search(IQuery query, User searcher, Collector collector);

        /// <summary>
        /// Run a search based on the provided search criteria and, for each match, call Collector.collect() not taking
        /// into account any security constraints.
        /// 
        /// Do not use this method, use <see cref="Search(IQuery,Api.Models.User,Collector)"/>
        /// instead, this should only be used when performing administrative tasks where you need to know ALL the issues
        /// that will be affected.
        /// 
        /// Collectors are low level Lucene classes, but they allow issues to be placed into buckets very quickly.
        /// Many of QuoteFlow's graphs and stats are generated in this manner. This method is useful if you need to execute a
        /// query in constant-memory (i.e. you do not want to load the results of your complete search into memory).
        /// </summary>
        /// <param name="query"> contains the information required to perform the search. </param>
        /// <param name="searcher"> the user performing the search which will be used to provide context for the search. </param>
        /// <param name="collector"> the Lucene object that will have collect called for each match. </param>
        void SearchOverrideSecurity(IQuery query, User searcher, Collector collector);

        /// <summary>
        /// Run a search based on the provided search criteria and, for each match call Collector.collect(). This method
        /// is for Collectors that need the search results to be sorted.
        /// 
        /// <b>Note:</b> this is much slower than using <see cref="Search(IQuery,Api.Models.User,Collector)"/>.
        /// 
        /// You may limit the number of results being collected by the Collector using the IPagerFilter parameter.
        /// This method is useful if you need to execute a query in constant-memory (i.e. you do not want to load
        /// the results of your complete search into memory).
        /// </summary>
        /// <param name="query"> contains the information required to perform the search. </param>
        /// <param name="searcher"> the user performing the search which will be used to provide context for the search. </param>
        /// <param name="collector"> the Lucene object that will have collect called for each match. </param>
        /// <param name="pager"> Pager filter (use <see cref="IPagerFilter#getUnlimitedFilter()"/> to get all issues). </param>
        void SearchAndSort(IQuery query, User searcher, Collector collector, IPagerFilter pager);

        /// <summary>
        /// Run a search based on the provided search criteria and, for each match call Collector.collect(). This method
        /// is for Collectors that need the search results to be sorted.
        /// 
        /// Do not use this method, user <see cref="SearchAndSort"/>
        /// instead, this should only be used when performing administrative tasks where you need to know ALL the issues
        /// that will be effected.
        /// 
        /// <b>Note:</b> this is much slower than using <see cref="Search(IQuery,Api.Models.User,Collector)"/>.
        /// 
        /// You may limit the number of results being collected by the Collector using the IPagerFilter parameter.
        /// This method is useful if you need to execute a query in constant-memory (i.e. you do not want to load
        /// the results of your complete search into memory).
        /// </summary>
        /// <param name="query">Contains the information required to perform the search.</param>
        /// <param name="searcher">The user performing the search which will be used to provide context for the search. </param>
        /// <param name="collector">The Lucene object that will have collect called for each match. </param>
        /// <param name="pager">Pager filter (use <see cref="IPagerFilter.GetUnlimitedFilter()"/> to get all issues).</param>
        void SearchAndSortOverrideSecurity(IQuery query, User searcher, Collector collector, IPagerFilter pager);
    }
}