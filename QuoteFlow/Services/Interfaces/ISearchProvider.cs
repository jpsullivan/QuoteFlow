using Lucene.Net.Search;
using QuoteFlow.Infrastructure.Exceptions.Search;
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
        /// <summary>
        /// Search the index, and only return issues that are in the pager's range.
        /// <em>Note: that this method returns read only <see cref="Asset"/> objects, and should not be
        /// used where you need the issue for update</em>.
        /// 
        /// Also note that if you are only after the number of search results use
        /// <see cref="searchCount(IQuery ,User)"/> as it provides better performance.
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
        /// <em>Note that this method returns read only <see cref="Asset"/> objects, and should not be
        /// used where you need the issue for update</em>.
        /// 
        /// Also note that if you are only after the number of search results use
        /// <see cref="searchCount(IQuery ,User)"/> as it provides better performance.
        /// </summary>
        /// <param name="query"> contains the information required to perform the search. </param>
        /// <param name="searcher"> the user performing the search, which will be used to create a permission filter that filters out
        /// any of the results the user is not able to see and will be used to provide context for the search. </param>
        /// <param name="pager"> Pager filter (use <see cref="IPagerFilter#getUnlimitedFilter()"/> to get all issues). </param>
        /// <param name="andQuery"> raw lucene Query to AND with the request.
        /// </param>
        /// <returns>A <see cref="SearchResults"/> containing the resulting issues.</returns>
        SearchResults Search(IQuery query, User searcher, IPagerFilter pager, Lucene.Net.Search.Query andQuery);

        /// <summary>
        /// Search the index, and only return issues that are in the pager's range while AND'ing the raw lucene query
        /// to the generated query from the provided searchQuery, not taking into account any security
        /// constraints.
        /// 
        /// Do not use this method, user <see cref="Search(QuoteFlow.Models.Search.Jql.Query.IQuery,QuoteFlow.Models.User,QuoteFlow.Infrastructure.Paging.IPagerFilter,Lucene.Net.Search.Query)"/>
        /// instead, this should only be used when performing administrative tasks where you need to know ALL the issues
        /// that will be affected.
        /// 
        /// <em>Note that this method returns read only <see cref="Asset"/> objects, and should not be
        /// used where you need the issue for update</em>.  Also note that if you are only after the number of search
        /// results use <see cref="searchCount(IQuery, User)"/> as it provides better performance.
        /// </summary>
        /// <param name="query"> contains the information required to perform the search. </param>
        /// <param name="searcher"> the user performing the search which will be used to provide context for the search. </param>
        /// <param name="pager"> Pager filter (use <see cref="IPagerFilter#getUnlimitedFilter()"/> to get all issues). </param>
        /// <param name="andQuery"> raw lucene Query to AND with the request.
        /// </param>
        /// <returns> A <see cref="SearchResults"/> containing the resulting issues.</returns>
        SearchResults searchOverrideSecurity(IQuery query, User searcher, IPagerFilter pager, Lucene.Net.Search.Query andQuery);

        /// <summary>
        /// Return the number of issues matching the provided search criteria.
        /// <b>Note:</b> This does not load all results into memory and provides better performance than
        /// <see cref="Search"/>
        /// </summary>
        /// <param name="query"> contains the information required to perform the search. </param>
        /// <param name="searcher"> the user performing the search which will be used to provide context for the search.
        /// </param>
        /// <returns> number of matching results.
        /// </returns>
        /// <exception cref="SearchException"> thrown if there is a severe problem encountered with lucene when searching (wraps an
        /// IOException). </exception>
        /// <exception cref="ClauseTooComplexSearchException"> if the query or part of the query produces
        /// lucene that is too complex to be processed.
        ///  @since v4.3 </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: long searchCount(IQuery query, com.atlassian.crowd.embedded.api.User searcher) throws SearchException;
        long searchCount(IQuery query, User searcher);

        /// <summary>
        /// Return the number of issues matching the provided search criteria, overridding any security constraints.
        /// 
        /// Do not use this method, use <see cref="searchCount(IQuery , User)"/>
        /// instead, this should only be used when performing administrative tasks where you need to know ALL the issues
        /// that will be affected.
        /// 
        /// <b>Note:</b> This does not load all results into memory and provides better performance than
        /// <see cref="Search"/>
        /// </summary>
        /// <param name="query"> contains the information required to perform the search. </param>
        /// <param name="searcher"> the user performing the search which will be used to provide context for the search.
        /// </param>
        /// <returns> number of matching results.
        /// </returns>
        /// <exception cref="SearchException"> thrown if there is a severe problem encountered with lucene when searching (wraps an
        /// IOException). </exception>
        /// <exception cref="ClauseTooComplexSearchException"> if the query or part of the query produces
        /// lucene that is too complex to be processed.
        ///  @since v4.3 </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: long searchCountOverrideSecurity(IQuery query, com.atlassian.crowd.embedded.api.User searcher) throws SearchException;
        long searchCountOverrideSecurity(IQuery query, User searcher);

        /// <summary>
        /// Run a search based on the provided search criteria and, for each match, call Collector.collect().
        /// Collectors are low level Lucene classes, but they allow issues to be placed into buckets very quickly. Many of
        /// JIRA's graphs and stats are generated in this manner. This method is useful if you need to execute a query in
        /// constant-memory (i.e. you do not want to load the results of your complete search into memory) and the query
        /// generated via JQL needs to be augmented with some custom Lucene query.
        /// </summary>
        /// <param name="query"> contains the information required to perform the search. </param>
        /// <param name="searcher"> the user performing the search which will be used to provide context for the search. </param>
        /// <param name="collector"> the Lucene object that will have collect called for each match. </param>
        /// <param name="andQuery"> additional Lucene query to be anded with the lucene query that will be generated from JQL </param>
        /// <exception cref="SearchException"> thrown if there is a severe problem encountered with lucene when searching (wraps an
        /// IOException). </exception>
        /// <exception cref="ClauseTooComplexSearchException"> if the query or part of the query
        /// produces lucene that is too complex to be processed. </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: void search(IQuery query, com.atlassian.crowd.embedded.api.User searcher, org.apache.lucene.search.Collector collector, org.apache.lucene.search.Query andQuery) throws SearchException;
        void Search(IQuery query, User searcher, Collector collector, Lucene.Net.Search.Query andQuery);

        /// <summary>
        /// Run a search based on the provided search criteria and, for each match, call Collector.collect().
        /// Collectors are low level Lucene classes, but they allow issues to be placed into buckets very quickly.
        /// Many of JIRA's graphs and stats are generated in this manner. This method is useful if you need to execute a
        /// query in constant-memory (i.e. you do not want to load the results of your complete search into memory).
        /// </summary>
        /// <param name="query"> contains the information required to perform the search. </param>
        /// <param name="searcher"> the user performing the search which will be used to provide context for the search. </param>
        /// <param name="collector"> the Lucene object that will have collect called for each match.
        /// </param>
        /// <exception cref="SearchException"> thrown if there is a severe problem encountered with lucene when searching (wraps an
        /// IOException). </exception>
        /// <exception cref="ClauseTooComplexSearchException"> if the query or part of the query produces
        /// lucene that is too complex to be processed.
        ///  @since v4.3 </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: void search(IQuery query, com.atlassian.crowd.embedded.api.User searcher, org.apache.lucene.search.Collector collector) throws SearchException;
        void Search(IQuery query, User searcher, Collector collector);

        /// <summary>
        /// Run a search based on the provided search criteria and, for each match, call Collector.collect() not taking
        /// into account any security constraints.
        /// 
        /// Do not use this method, use <see cref="Search(QuoteFlow.Models.Search.Jql.Query.IQuery,QuoteFlow.Models.User,Lucene.Net.Search.Collector)"/>
        /// instead, this should only be used when performing administrative tasks where you need to know ALL the issues
        /// that will be affected.
        /// 
        /// Collectors are low level Lucene classes, but they allow issues to be placed into buckets very quickly.
        /// Many of JIRA's graphs and stats are generated in this manner. This method is useful if you need to execute a
        /// query in constant-memory (i.e. you do not want to load the results of your complete search into memory).
        /// </summary>
        /// <param name="query"> contains the information required to perform the search. </param>
        /// <param name="searcher"> the user performing the search which will be used to provide context for the search. </param>
        /// <param name="collector"> the Lucene object that will have collect called for each match. </param>
        /// <exception cref="ClauseTooComplexSearchException"> if the query or part of the query produces lucene that is too complex to be processed.</exception>
        void searchOverrideSecurity(IQuery query, User searcher, Collector collector);

        /// <summary>
        /// Run a search based on the provided search criteria and, for each match call Collector.collect(). This method
        /// is for Collectors that need the search results to be sorted.
        /// 
        /// <b>Note:</b> this is much slower than using <see cref="Search(QuoteFlow.Models.Search.Jql.Query.IQuery,QuoteFlow.Models.User,Lucene.Net.Search.Collector)"/>.
        /// 
        /// You may limit the number of results being collected by the Collector using the IPagerFilter parameter.
        /// This method is useful if you need to execute a query in constant-memory (i.e. you do not want to load
        /// the results of your complete search into memory).
        /// </summary>
        /// <param name="query"> contains the information required to perform the search. </param>
        /// <param name="searcher"> the user performing the search which will be used to provide context for the search. </param>
        /// <param name="collector"> the Lucene object that will have collect called for each match. </param>
        /// <param name="pager"> Pager filter (use <see cref="IPagerFilter#getUnlimitedFilter()"/> to get all issues). </param>
        /// <exception cref="ClauseTooComplexSearchException">If the query or part of the query produces lucene that is too complex to be processed.</exception>
        void SearchAndSort(IQuery query, User searcher, Collector collector, IPagerFilter pager);

        /// <summary>
        /// Run a search based on the provided search criteria and, for each match call Collector.collect(). This method
        /// is for Collectors that need the search results to be sorted.
        /// 
        /// Do not use this method, user <see cref="SearchAndSort"/>
        /// instead, this should only be used when performing administrative tasks where you need to know ALL the issues
        /// that will be effected.
        /// 
        /// <b>Note:</b> this is much slower than using <see cref="Search(QuoteFlow.Models.Search.Jql.Query.IQuery,QuoteFlow.Models.User,Lucene.Net.Search.Collector)"/>.
        /// 
        /// You may limit the number of results being collected by the Collector using the IPagerFilter parameter.
        /// This method is useful if you need to execute a query in constant-memory (i.e. you do not want to load
        /// the results of your complete search into memory).
        /// </summary>
        /// <param name="query">Contains the information required to perform the search.</param>
        /// <param name="searcher">The user performing the search which will be used to provide context for the search. </param>
        /// <param name="collector">The Lucene object that will have collect called for each match. </param>
        /// <param name="pager">Pager filter (use <see cref="IPagerFilter.GetUnlimitedFilter()"/> to get all issues).</param>
        /// <exception cref="ClauseTooComplexSearchException">If the query or part of the query produces lucene that is too complex to be processed.</exception>
        void searchAndSortOverrideSecurity(IQuery query, User searcher, Collector collector, IPagerFilter pager);
    }
}