using System;
using System.Collections.Generic;
using System.ComponentModel;
using Lucene.Net.Documents;
using Lucene.Net.Search;
using Ninject;
using QuoteFlow.Api.Asset.Fields;
using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Asset.Search.Managers;
using QuoteFlow.Api.Asset.Search.Util;
using QuoteFlow.Api.Infrastructure.Paging;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Order;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Search;
using QuoteFlow.Api.Services;
using QuoteFlow.Core.Asset.Fields;
using QuoteFlow.Core.Asset.Search;
using QuoteFlow.Core.Jql.Query;
using Container = QuoteFlow.Core.DependencyResolution.Container;
using Query = Lucene.Net.Search.Query;

namespace QuoteFlow.Core.Services
{
    public class LuceneSearchService : ISearchProvider
    {
        #region DI

        private readonly ISearchProviderFactory searchProviderFactory;
        private readonly IAssetService _asssetService;
        private readonly ISearchHandlerManager searchHandlerManager;
        private readonly ISearchSortUtil searchSortUtil;
        private readonly ILuceneQueryBuilder luceneQueryBuilder;

        public LuceneSearchService(IAssetService asssetService, ISearchProviderFactory searchProviderFactory, 
            ISearchHandlerManager searchHandlerManager, ISearchSortUtil searchSortUtil, ILuceneQueryBuilder luceneQueryBuilder)
        {
            _asssetService = asssetService;
            this.searchProviderFactory = searchProviderFactory;
            this.searchHandlerManager = searchHandlerManager;
            this.searchSortUtil = searchSortUtil;
            this.luceneQueryBuilder = luceneQueryBuilder;
        }

        #endregion

        public virtual SearchResults Search(IQuery query, User searcher, IPagerFilter pager)
        {
            return Search(query, searcher, pager, null);
        }

        public virtual SearchResults Search(IQuery query, User searcher, IPagerFilter pager, Query andQuery)
        {
            return Search(query, searcher, pager, andQuery, false);
        }

        public virtual SearchResults SearchOverrideSecurity(IQuery query, User searcher, IPagerFilter pager, Query andQuery)
        {
            return Search(query, searcher, pager, andQuery, true);
        }

        public virtual long SearchCount(IQuery query, User user)
        {
            IndexSearcher issueSearcher = searchProviderFactory.GetSearcher(SearchProviderTypes.AssetIndex);
            return GetHitCount(query, user, null, null, false, issueSearcher, null);
        }

        public virtual long SearchCountOverrideSecurity(IQuery query, User user)
        {
            IndexSearcher issueSearcher = searchProviderFactory.GetSearcher(SearchProviderTypes.AssetIndex);
            return GetHitCount(query, user, null, null, true, issueSearcher, null);
        }

        public virtual void Search(IQuery query, User user, Collector collector)
        {
            Search(query, user, collector, null, false);
        }

        public virtual void Search(IQuery query, User searcher, Collector collector, Query andQuery)
        {
            Search(query, searcher, collector, andQuery, false);
        }

        public virtual void SearchOverrideSecurity(IQuery query, User user, Collector collector)
        {
            Search(query, user, collector, null, true);
        }

        public virtual void SearchAndSort(IQuery query, User user, Collector collector, IPagerFilter pagerFilter)
        {
            SearchAndSort(query, user, collector, pagerFilter, false);
        }

        public virtual void SearchAndSortOverrideSecurity(IQuery query, User user, Collector collector, IPagerFilter pagerFilter)
        {
            SearchAndSort(query, user, collector, pagerFilter, true);
        }

        /// <summary>
        /// Returns 0 if there are no Lucene parameters (search request is null), otherwise 
        /// returns the hit count. The count is 0 if there are no matches.
        /// </summary>
        /// <param name="searchQuery">Search request</param>
        /// <param name="searchUser">User performing the search </param>
        /// <param name="sortField">Array of fields to sort by </param>
        /// <param name="andQuery">A query to join with the request </param>
        /// <param name="overrideSecurity">Ignore the user security permissions </param>
        /// <param name="issueSearcher">The IndexSearcher to be used when searching </param>
        /// <param name="pager">A pager which holds information about which page of search results is actually required.</param>
        /// <returns>The hit count</returns>
        /// <exception cref="ClauseTooComplexSearchException">If query creates a lucene query that is too complex to be processed.</exception>
        private long GetHitCount(IQuery searchQuery, User searchUser, SortField[] sortField, Query andQuery, bool overrideSecurity, IndexSearcher issueSearcher, IPagerFilter pager)
        {
            if (searchQuery == null)
            {
                return 0;
            }
            try
            {
                var permissionsFilter = GetPermissionsFilter(overrideSecurity, searchUser);
                var finalQuery = CreateLuceneQuery(searchQuery, andQuery, searchUser, overrideSecurity);

                // todo: uncomment
//                var hitCountCollector = new TotalHitCountCollector();
//                issueSearcher.Search(finalQuery, permissionsFilter, hitCountCollector);
//                return hitCountCollector.TotalHits;
                return 0;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Returns null if there are no Lucene parameters (search request is null), otherwise returns a collection of Lucene
        /// Document objects.
        /// 
        /// The collection has 0 results if there are no matches.
        /// </summary>
        /// <param name="searchQuery">Search request</param>
        /// <param name="searchUser">User performing the search</param>
        /// <param name="sortField">Array of fields to sort by</param>
        /// <param name="andQuery">A query to join with the request</param>
        /// <param name="overrideSecurity">Ignore the user security permissions</param>
        /// <param name="issueSearcher">The IndexSearcher to be used when searching</param>
        /// <param name="pager">A pager which holds information about which page of search results is actually required.</param>
        /// <returns>Hits</returns>
        /// <exception cref="SearchException"> if error occurs </exception>
        /// <exception cref="ClauseTooComplexSearchException">If query creates a lucene query that is too complex to be processed. </exception>
        private TopDocs GetHits(IQuery searchQuery, User searchUser, SortField[] sortField, Query andQuery, bool overrideSecurity, IndexSearcher issueSearcher, IPagerFilter pager)
        {
            if (searchQuery == null)
            {
                return null;
            }
            try
            {
                Filter permissionsFilter = GetPermissionsFilter(overrideSecurity, searchUser);
                Query finalQuery = CreateLuceneQuery(searchQuery, andQuery, searchUser, overrideSecurity);
                return RunSearch(issueSearcher, finalQuery, permissionsFilter, sortField, searchQuery.ToString(), pager);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private void Search(IQuery searchQuery, User user, Collector collector, Query andQuery, bool overrideSecurity)
        {
            IndexSearcher searcher = searchProviderFactory.GetSearcher(SearchProviderTypes.AssetIndex);
            Query finalQuery = andQuery;

            if (searchQuery.WhereClause != null)
            {
                QueryCreationContext context = new QueryCreationContext(user, overrideSecurity);
                Query query = luceneQueryBuilder.CreateLuceneQuery(context, searchQuery.WhereClause);
                if (query != null)
                {
                    if (finalQuery != null)
                    {
                        BooleanQuery join = new BooleanQuery();
                        join.Add(finalQuery, Occur.MUST);
                        join.Add(query, Occur.MUST);
                        finalQuery = join;
                    }
                    else
                    {
                        finalQuery = query;
                    }
                }
            }

            Filter permissionsFilter = GetPermissionsFilter(overrideSecurity, user);

            // NOTE: we do this because when you are searching for everything the query is EMPTY
            if (finalQuery == null)
            {
                finalQuery = new MatchAllDocsQuery();
            }
            try
            {
                searcher.Search(finalQuery, permissionsFilter, collector);
            }
            catch (Exception e)
            {
                throw new Exception("Exception occurred whilst searching for assets + " + e.Message, e);
            }
        }

        private Query CreateLuceneQuery(IQuery searchQuery, Query andQuery, User searchUser, bool overrideSecurity)
        {
            string jqlSearchQuery = searchQuery.ToString();

            Query finalQuery = andQuery;

            if (searchQuery.WhereClause != null)
            {
                QueryCreationContext context = new QueryCreationContext(searchUser, overrideSecurity);
                Query query = luceneQueryBuilder.CreateLuceneQuery(context, searchQuery.WhereClause);
                if (query != null)
                {
                    if (finalQuery != null)
                    {
                        var join = new BooleanQuery();
                        join.Add(finalQuery, Occur.MUST);
                        join.Add(query, Occur.MUST);
                        finalQuery = join;
                    }
                    else
                    {
                        finalQuery = query;
                    }
                }
            }

            // NOTE: we do this because when you are searching for everything the query is null
            if (finalQuery == null)
            {
                finalQuery = new MatchAllDocsQuery();
            }

            return finalQuery;
        }

        private SearchResults Search(IQuery query, User searcher, IPagerFilter pager, Query andQuery, bool overrideSecurity)
        {
            IndexSearcher issueSearcher = searchProviderFactory.GetSearcher(SearchProviderTypes.AssetIndex);
            TopDocs luceneMatches = GetHits(query, searcher, GetSearchSorts(searcher, query), andQuery, overrideSecurity, issueSearcher, pager);

            try
            {
                List<IAsset> matches;
                int totalIssueCount = luceneMatches == null ? 0 : luceneMatches.TotalHits;
                if ((luceneMatches != null) && (luceneMatches.TotalHits >= pager.Start))
                {
                    int end = Math.Min(pager.End, luceneMatches.TotalHits);
                    matches = new List<IAsset>();
                    for (int i = pager.Start; i < end; i++)
                    {
                        Document doc = issueSearcher.Doc(luceneMatches.ScoreDocs[i].Doc);
                        matches.Add(_asssetService.GetAsset(doc));
                    }
                }
                else
                {
                    //if there were no lucene-matches, or the length of the matches is less than the page start index
                    //return an empty list of issues.
                    matches = new List<IAsset>();
                }

                return new SearchResults(matches, totalIssueCount, pager);
            }
            catch (Exception e)
            {
                throw new Exception("Exception occurred whilst searching for assets: " + e.Message, e);
            }
        }

        private void SearchAndSort(IQuery query, User user, Collector collector, IPagerFilter pagerFilter, bool overrideSecurity)
        {
            try
            {
                IndexSearcher issueSearcher = searchProviderFactory.GetSearcher(SearchProviderTypes.AssetIndex);

                TopDocs hits = GetHits(query, user, GetSearchSorts(user, query), null, overrideSecurity, issueSearcher, pagerFilter);
                if (hits != null)
                {
                    if (collector is ITotalHitsAwareCollector)
                    {
                        ((ITotalHitsAwareCollector)collector).TotalHits = hits.TotalHits;
                    }

                    if (hits.TotalHits >= pagerFilter.Start)
                    {
                        int end = Math.Min(pagerFilter.End, hits.TotalHits);
                        collector.SetNextReader(issueSearcher.IndexReader, 0);
                        for (int i = pagerFilter.Start; i < end; i++)
                        {
                            collector.Collect(hits.ScoreDocs[i].Doc);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Exception occurred whilst searching for assets: " + e.Message, e);
            }
        }

//        private CachedWrappedFilterCache CachedWrappedFilterCache
//        {
//            get
//            {
//                CachedWrappedFilterCache cache = (CachedWrappedFilterCache)JiraAuthenticationContextImpl.RequestCache.get(RequestCacheKeys.CACHED_WRAPPED_FILTER_CACHE);
//
//                if (cache == null)
//                {
//                    if (log.DebugEnabled)
//                    {
//                        log.debug("Creating new CachedWrappedFilterCache");
//                    }
//                    cache = new CachedWrappedFilterCache();
//                    JiraAuthenticationContextImpl.RequestCache.put(RequestCacheKeys.CACHED_WRAPPED_FILTER_CACHE, cache);
//                }
//
//                return cache;
//            }
//        }

        private Filter GetPermissionsFilter(bool overRideSecurity, User searchUser)
        {
//            if (!overRideSecurity)
//            {
//                // JRA-14980: first attempt to retrieve the filter from cache
//                CachedWrappedFilterCache cache = CachedWrappedFilterCache;
//
//                Filter filter = cache.getFilter(searchUser);
//                if (filter != null)
//                {
//                    return filter;
//                }
//
//                // if not in cache, construct a query (also using a cache)
//                Query permissionQuery = permissionsFilterGenerator.GetQuery(searchUser);
//                filter = new CachingWrapperFilter(new QueryWrapperFilter(permissionQuery));
//
//                // JRA-14980: store the wrapped filter in the cache
//                // this is because the CachingWrapperFilter gives us an extra benefit of precalculating its BitSet, and so
//                // we should use this for the duration of the request.
//                cache.StoreFilter(filter, searchUser);
//
//                return filter;
//            }

            return null;
        }

        private TopDocs RunSearch(IndexSearcher searcher, Query query, Filter filter, SortField[] sortFields, string searchQueryString, IPagerFilter pager)
        {
            TopDocs hits;
            try
            {

                int maxHits;
                if (pager != null && pager.End > 0)
                {
                    maxHits = pager.End;
                }
                else
                {
                    maxHits = int.MaxValue;
                }
                if ((sortFields != null) && (sortFields.Length > 0)) // a zero length array sorts in very weird ways! JRA-5151
                {
                    hits = searcher.Search(query, filter, maxHits, new Sort(sortFields));
                }
                else
                {
                    hits = searcher.Search(query, filter, maxHits);
                }
                // NOTE: this is only here so we can flag any queries in production that are taking long and try to figure out
                // why they are doing that.
//                long timeQueryTook = opTimer.end().MillisecondsTaken;
//                if (timeQueryTook > 400)
//                {
//                    logSlowQuery(query, searchQueryString, timeQueryTook);
//                }
            }
            finally
            {
            }

            return hits;
        }

        public SortField[] GetSearchSorts(User searcher, IQuery query)
        {
            if (query == null)
            {
                return null;
            }

            var sorts = searchSortUtil.GetSearchSorts(query);

            var luceneSortFields = new List<SortField>();
            // When the sorts have been specifically set to null then we run the search with no sorts
            if (sorts != null)
            {
                var fieldManager = Container.Kernel.TryGet<FieldManager>();

                foreach (SearchSort searchSort in sorts)
                {
//                    if (searchSort.Property.Defined && EntityPropertyType.IsJqlClause(searchSort.Field))
//                    {
//                        EntityPropertyType entityPropertyType = EntityPropertyType.getEntityPropertyTypeForClause(searchSort.Field);
//                        Property property = searchSort.Property.get();
//                        luceneSortFields.Add(new SortField(entityPropertyType.IndexPrefix + "_" + property.AsPropertyString, SortField.STRING, getSortOrder(searchSort, null)));
//                    }
//                    else
//                    {
//                        // Lets figure out what field this searchSort is referring to. The {@link SearchSort#getField} method
//                        //actually a JQL name.
//                        var fieldIds = new List<string>(searchHandlerManager.GetFieldIds(searcher, searchSort.Field));
//                        // sort to get consistent ordering of fields for clauses with multiple fields
//                        fieldIds.Sort();
//
//                        foreach (string fieldId in fieldIds)
//                        {
//
//                            if (fieldManager.IsNavigableField(fieldId))
//                            {
//                                INavigableField field = fieldManager.GetNavigableField(fieldId);
//                                luceneSortFields.AddRange(field.GetSortFields(getSortOrder(searchSort, field)));
//                            }
//                            else
//                            {
////                                log.debug("Search sort contains invalid field: " + searchSort);
//                            }
//                        }
//                    }

                    // Lets figure out what field this searchSort is referring to. 
                    // The {@link SearchSort#getField} method actually a JQL name.
                    var fieldIds = new List<string>(searchHandlerManager.GetFieldIds(searcher, searchSort.Field));
                    // sort to get consistent ordering of fields for clauses with multiple fields
                    fieldIds.Sort();

                    foreach (string fieldId in fieldIds)
                    {

                        if (fieldManager.IsNavigableField(fieldId))
                        {
                            INavigableField field = fieldManager.GetNavigableField(fieldId);
                            luceneSortFields.AddRange(field.GetSortFields(getSortOrder(searchSort, field)));
                        }
                        else
                        {
                            //                                log.debug("Search sort contains invalid field: " + searchSort);
                        }
                    }
                }
            }

            return luceneSortFields.ToArray();
        }

        private bool getSortOrder(SearchSort searchSort, INavigableField field)
        {
            bool order;

            if (searchSort.Order == null)
            {
                // We need to handle the case where the sort order is null, we will delegate off to the fields
                // default SearchSort for order in this case.
                string defaultSortOrder = field == null ? "DESC" : field.DefaultSortOrder;
                if (defaultSortOrder == null)
                {
                    order = false;
                }
                else
                {
                    SortOrder enumResult;
                    Enum.TryParse(defaultSortOrder, out enumResult);
                    order = enumResult == SortOrder.DESC;
                }
            }
            else
            {
                order = searchSort.Reverse;
            }
            return order;
        }

//        protected internal virtual void logSlowQuery(IQuery query, string searchQueryString, long timeQueryTook)
//        {
//            if (log.DebugEnabled || slowLog.InfoEnabled)
//            {
//                // truncate lucene query at 800 characters
//                string msg = string.Format("JQL query '{0}' produced lucene query '{1,-1}' and took '{2:D}' ms to run.", searchQueryString, query.ToString(), timeQueryTook);
//                if (log.DebugEnabled)
//                {
//                    log.debug(msg);
//                }
//                if (slowLog.InfoEnabled)
//                {
//                    slowLog.info(msg);
//                }
//            }
//        }
    }
}