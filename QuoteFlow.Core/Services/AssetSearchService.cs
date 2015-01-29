using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Support;
using QuoteFlow.Api.Asset.Fields;
using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Asset.Search.Managers;
using QuoteFlow.Api.Asset.Search.Searchers;
using QuoteFlow.Api.Asset.Search.Searchers.Transformer;
using QuoteFlow.Api.Asset.Transport;
using QuoteFlow.Api.Jql;
using QuoteFlow.Api.Jql.Parser;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Util;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Search;
using QuoteFlow.Api.Services;
using QuoteFlow.Core.Asset.Search;
using Wintellect.PowerCollections;

namespace QuoteFlow.Core.Services
{
    public class AssetSearchService : IAssetSearchService
    {
        #region IoC

        public IJqlStringSupport JqlStringSupport { get; protected set; }
        public ISearchService SearchService { get; protected set; }
        public IAssetSearcherManager AssetSearcherManager { get; protected set; }

        public AssetSearchService() { }

        public AssetSearchService(IJqlStringSupport jqlStringSupport, ISearchService searchService, IAssetSearcherManager assetSearcherManager)
        {
            JqlStringSupport = jqlStringSupport;
            SearchService = searchService;
            AssetSearcherManager = assetSearcherManager;
        }

        #endregion

        public QuerySearchResults Search(User user, MultiDictionary<string, string[]> paramMap, long filterId)
        {
            var searchers = AssetSearcherManager.GetAllSearchers();
            
            var clauses = GenerateQuery(paramMap, user, searchers);
            var query = BuildQuery(clauses);

            var searchContext = SearchService.GetSearchContext(user, query);
            var searchResults = GetSearchResults(true, user, searchers, clauses, query, searchContext);
            
            throw new NotImplementedException();
        }

        public QuerySearchResults SearchWithJql(User user, string paramString, long filterId)
        {
            throw new NotImplementedException();
        }

        private QuerySearchResults GetSearchResults(bool includePrimes, User user, ICollection<IAssetSearcher<ISearchableField>> searchers, IDictionary<string, SearchRendererHolder> clauses, IQuery query, ISearchContext searchContext)
        {
            SearchRendererValueResults results = GetValueResults(includePrimes, user, searchers, clauses, query, searchContext);
            return new QuerySearchResults(null, results);
        }

        private IDictionary<string, SearchRendererHolder> GenerateQuery<T>(ISearchContext searchContext, User user, IQuery query, IEnumerable<T> searchers)
        {
            var clauses = new HashMap<string, SearchRendererHolder>();
            foreach (var searcher in searchers)
            {
                var assetSearcher = (IAssetSearcher<ISearchableField>) searcher;
                ISearchInputTransformer searchInputTransformer = assetSearcher.SearchInputTransformer;
                var fieldValuesHolder = new FieldValuesHolder();

                searchInputTransformer.PopulateFromQuery(user, fieldValuesHolder, query, searchContext);
                IClause clause = searchInputTransformer.GetSearchClause(user, fieldValuesHolder);
                
                if (null == clause) continue;

                string id = assetSearcher.SearchInformation.Id;
                clauses[id] = SearchRendererHolder.Valid(clause, fieldValuesHolder);
            }

            return clauses;
        }

        private Dictionary<string, SearchRendererHolder> GenerateQuery(MultiDictionary<string, string[]> paramMap, User user, IEnumerable<IAssetSearcher<ISearchableField>> searchers)
        {
            var clauses = new Dictionary<string, SearchRendererHolder>();

            var actionParams = GetActionParameters(paramMap);
            IDictionary<string, String[]> jqlParams = GetJqlParameters(paramMap);
            foreach (var assetSearcher in searchers)
            {
                string id = assetSearcher.SearchInformation.Id;
                ISearchInputTransformer searchInputTransformer = assetSearcher.SearchInputTransformer;
                string[] value;
                if (jqlParams.TryGetValue(id, out value))
                {
                    ParseResult parseResult = SearchService.ParseQuery(user, value[0]);
                    if (parseResult.IsValid())
                    {
                        IClause clause = parseResult.Query.WhereClause;
                        clauses[id] = SearchRendererHolder.Invalid(clause);
                    }
                    else
                    {
                        Console.WriteLine(parseResult.Errors.ErrorMessages);
//                        ErrorCollection errors = new SimpleErrorCollection();
//                        errors.addErrorMessages(parseResult.Errors.ErrorMessages);
//                        return ServiceOutcomeImpl.from(errors, null);
                    }
                }
                else
                {
                    IFieldValuesHolder fieldValuesHolder = new FieldValuesHolder();
                    searchInputTransformer.PopulateFromParams(user, fieldValuesHolder, actionParams);
                    IClause clause = searchInputTransformer.GetSearchClause(user, fieldValuesHolder);
                    if (null != clause)
                    {
                        clauses[id] = SearchRendererHolder.Valid(clause, fieldValuesHolder);
                    }
                }
            }

            return clauses;
        }

        private IQuery BuildQuery(IDictionary<string, SearchRendererHolder> clauses)
        {
            var actualClauses = clauses.Values.Select(c => c.Clause).ToList();
            IClause jqlClause = clauses.Any() ? new AndClause(actualClauses) : null;
            return new Query(jqlClause);
        }

        private IActionParams GetActionParameters(IEnumerable<KeyValuePair<string, ICollection<string[]>>> paramMap)
        {
            var newParams = new MultiDictionary<string, string[]>(true);
            foreach (var p in paramMap.Where(p => !p.Key.StartsWith("__jql_")))
            {
                newParams.Add(p);
            }
            return new ActionParams(newParams);
        }

        private IDictionary<string, String[]> GetJqlParameters(IEnumerable<KeyValuePair<string, ICollection<string[]>>> paramMap)
        {
            var jqlParams = new HashMap<string, String[]>();
            foreach (KeyValuePair<string, ICollection<string[]>> entry in paramMap)
            {
                if (entry.Key.StartsWith("__jql_"))
                {
                    jqlParams[entry.Key.Substring("__jql_".Length)].SetValue(entry.Value, 0);
                }
            }

            return jqlParams;
        }

        private SearchRendererValueResults GetValueResults(bool includePrimes, User user, ICollection<IAssetSearcher<ISearchableField>> searchers, IDictionary<string, SearchRendererHolder> clauses, IQuery query, ISearchContext searchContext)
        {
            var results = new SearchRendererValueResults();
            foreach (var assetSearcher in searchers)
            {
                string id = assetSearcher.SearchInformation.Id;
                SearchRendererHolder clause;
                var hasValue = clauses.TryGetValue(id, out clause);
                if (hasValue)
                {
                    if (includePrimes)
                    {
                    }

                    string jql = clause != null ? JqlStringSupport.GenerateJqlString(clause.Clause) : null;
                    results.Add(id, new SearchRendererValue(assetSearcher.SearchInformation.NameKey, jql, null, null, true, true));
                }
            }

            return results;
        }

//        private Searchers GetSearchers(ISearchContext searchContext, User user)
//        {
//            var searcherGroups = new Dictionary<string, FilteredSearcherGroup>();
//            foreach (var assetSearcher in AssetSearcherManager.GetAllSearchers())
//            {
//                
//            }
//
//		        foreach (IssueSearcher<?> searcher in this.searchHandlerManager.AllSearchers)
//		        {
//			        IssueSearcherPanelMap.Panel panel = IssueSearcherPanelMap.getPanel(searcher.GetType());
//			        FilteredSearcherGroup group = (FilteredSearcherGroup) searcherGroups[panel.name()];
//			        if (group == null)
//			        {
//				        group = new FilteredSearcherGroup(panel.name());
//				        if (panel.TitleKey != null)
//				        {
//					        group.Title = i18nHelper.getText(panel.TitleKey);
//				        }
//				        searcherGroups[panel.name()] = group;
//			        }
//        //JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//        //ORIGINAL LINE: SearcherInformation<?> searcherInfo = searcher.getSearchInformation();
//			        SearcherInformation<?> searcherInfo = searcher.SearchInformation;
//			        SearchableField field = searcherInfo.Field;
//			        string fieldKey = "text";
//			        if (field != null)
//			        {
//				        if ((field is CustomField))
//				        {
//					        fieldKey = ((CustomField) field).CustomFieldType.Key;
//				        }
//				        else
//				        {
//					        fieldKey = field.NameKey;
//				        }
//			        }
//			        string searcherId = searcherInfo.Id;
//			        group.addSearcher(new Searcher(searcherId, i18nHelper.getText(searcherInfo.NameKey), fieldKey, Convert.ToBoolean(searcher.SearchRenderer.isShown(user, searchContext)), (long?) recentSearchers[searcherId]));
//		        }
//		        Searchers searchers = new Searchers();
//		        searchers.addGroups(searcherGroups.Values);
//		        return searchers;
//        }
    }
}