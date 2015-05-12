using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Support;
using QuoteFlow.Api.Asset.Fields;
using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Asset.Search.Managers;
using QuoteFlow.Api.Asset.Search.Searchers;
using QuoteFlow.Api.Asset.Search.Searchers.Transformer;
using QuoteFlow.Api.Asset.Transport;
using QuoteFlow.Api.Infrastructure.Extensions;
using QuoteFlow.Api.Infrastructure.Services;
using QuoteFlow.Api.Jql;
using QuoteFlow.Api.Jql.Parser;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Util;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Services;
using QuoteFlow.Core.Infrastructure.Services;
using QuoteFlow.Core.Util;
using Wintellect.PowerCollections;

namespace QuoteFlow.Core.Services
{
    public class AssetSearchService : IAssetSearchService
    {
        private readonly string JqlInvalidErrorMessage = "jqlInvalid";
        private readonly string JqlTooComplexErrorMessage = "jqlTooComplex";

        #region DI

        public IJqlStringSupport JqlStringSupport { get; protected set; }
        public IAssetSearcherManager AssetSearcherManager { get; protected set; }
        public ISearchHandlerManager SearchHandlerManager { get; protected set; }
        public ISearchContextHelper SearchContextHelper { get; protected set; }
        public ISearchService SearchService { get; protected set; }

        public AssetSearchService()
        {
        }

        public AssetSearchService(IJqlStringSupport jqlStringSupport, IAssetSearcherManager assetSearcherManager, ISearchHandlerManager searchHandlerManager, ISearchContextHelper searchContextHelper, ISearchService searchService)
        {
            JqlStringSupport = jqlStringSupport;
            AssetSearcherManager = assetSearcherManager;
            SearchHandlerManager = searchHandlerManager;
            SearchContextHelper = searchContextHelper;
            SearchService = searchService;
        }

        #endregion

        public IServiceOutcome<QuerySearchResults> Search(User user, MultiDictionary<string, string[]> paramMap, long filterId)
        {
            var searchers = AssetSearcherManager.GetAllSearchers();
            
            var clauses = GenerateQuery(paramMap, user, searchers);
            var query = BuildQuery(clauses);

            var searchContext = SearchService.GetSearchContext(user, query);
            var searchResults = GetSearchResults(true, user, searchers, clauses, query, searchContext);
            
            //throw new NotImplementedException();
            return searchResults;
        }

        public IServiceOutcome<QuerySearchResults> SearchWithJql(User user, string jqlContext, long filterId)
        {
            var parseResult = SearchService.ParseQuery(user, jqlContext);
            if (!parseResult.IsValid())
            {
                var errors = new SimpleErrorCollection();
                errors.AddErrorMessage(JqlInvalidErrorMessage);

                foreach (var error in parseResult.Errors.ErrorMessages)
                {
                    errors.AddError("jql", error);
                }

                if (!errors.HasAnyErrors())
                {
                    errors.AddError("jql", "Error in the JQL query: Unable to parse the query.");
                }

                return new ServiceOutcome<QuerySearchResults>(errors);
            }

            // Is the query too complex to be expressed with searchers?
            var query = parseResult.Query;
//            if (!SearchService.DoesQueryFitFilterForm(user, query))
//            {
//                return ServiceOutcome<string>.Error(JqlTooComplexErrorMessage);
//            }

            var searchers = AssetSearcherManager.GetAllSearchers();
            var searchContext = SearchService.GetSearchContext(user, parseResult.Query);
            var clauses = GenerateQuery(searchContext, user, query, searchers);

            return GetSearchResults(true, user, searchers, clauses, query, searchContext);
        }

        public IServiceOutcome<string> GetEditHtml(string searcherId, string jqlContext)
        {
            var searchContextWithFieldValues =
                SearchContextHelper.GetSearchContextWithFieldValuesFromJqlString(jqlContext);

            return GetEditHtml(searcherId, searchContextWithFieldValues, CreateDisplayParams());
        }

        public Searchers GetSearchers(string jqlContext)
        {
            throw new NotImplementedException();
        }

        private IServiceOutcome<QuerySearchResults> GetSearchResults(bool includePrimes, User user, ICollection<IAssetSearcher<ISearchableField>> searchers, IDictionary<string, SearchRendererHolder> clauses, IQuery query, ISearchContext searchContext)
        {
            var outcome = GetValueResults(includePrimes, user, searchers, clauses, query, searchContext);
            if (!outcome.IsValid())
            {
                return new ServiceOutcome<QuerySearchResults>(outcome.ErrorCollection);
            }

            var renderableSearchers = GetSearchers(searchContext, user);
            return ServiceOutcome<QuerySearchResults>.Ok(new QuerySearchResults(renderableSearchers, outcome.ReturnedValue));
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
            var jqlParams = new HashMap<string, string[]>();
            foreach (KeyValuePair<string, ICollection<string[]>> entry in paramMap)
            {
                if (entry.Key.StartsWith("__jql_"))
                {
                    jqlParams[entry.Key.Substring("__jql_".Length)].SetValue(entry.Value, 0);
                }
            }

            return jqlParams;
        }

        private IServiceOutcome<SearchRendererValueResults> GetValueResults(bool includePrimes, User user, ICollection<IAssetSearcher<ISearchableField>> searchers, IDictionary<string, SearchRendererHolder> clauses, IQuery query, ISearchContext searchContext)
        {
            var results = new SearchRendererValueResults();
            foreach (var assetSearcher in searchers)
            {
                string id = assetSearcher.SearchInformation.Id;

                IFieldValuesHolder fieldParams = new FieldValuesHolder();

                SearchRendererHolder clause;
                var hasValue = clauses.TryGetValue(id, out clause);
                if (hasValue)
                {
                    var searchInputTransformer = assetSearcher.SearchInputTransformer;

                    if (includePrimes)
                    {
                        if (clause.IsValid)
                        {
                            fieldParams = clause.FieldParams;
                        }
                        else
                        {
                            searchInputTransformer.PopulateFromQuery(user, fieldParams, new Query(clause.Clause), searchContext);
                        }

                        searchInputTransformer.ValidateParams(user, searchContext, fieldParams);

                        string jql = JqlStringSupport.GenerateJqlString(clause.Clause);
                        results.Add(id, new SearchRendererValue(assetSearcher.SearchInformation.NameKey, jql, null, null, true, true));
                    }
                    else if (!clause.IsValid)
                    {
                        results.Add(id, new SearchRendererValue(assetSearcher.SearchInformation.NameKey, JqlStringSupport.GenerateJqlString(clause.Clause), null, null, false, true));
                    }
                }
            }

            return ServiceOutcome<SearchRendererValueResults>.Ok(results);
        }

        private Searchers GetSearchers(ISearchContext searchContext, User user)
		{
			var searcherGroups = new Dictionary<string, FilteredSearcherGroup>();
			foreach (var searcher in SearchHandlerManager.GetAllSearchers())
			{
				//IssueSearcherPanelMap.Panel panel = IssueSearcherPanelMap.getPanel(searcher.GetType());
			    FilteredSearcherGroup group;
			    if (!searcherGroups.TryGetValue(searcher.ToString(), out group))
			    {
			        group = new FilteredSearcherGroup(searcher.SearchInformation.NameKey);
			        group.Title = searcher.SearchInformation.Id;
                    searcherGroups.Add(searcher.ToString(), group);
			    }
				
                var searcherInfo = searcher.SearchInformation;
				ISearchableField field = searcherInfo.Field;
				string fieldKey = "text";
				if (field != null)
				{
//					if ((field is ICustomField))
//					{
//						fieldKey = ((CustomField) field).CustomFieldType.Key;
//					}
//					else
//					{
//						fieldKey = field.NameKey;
//					}

                    fieldKey = field.NameKey;
				}

				string searcherId = searcherInfo.Id;
				group.AddSearcher(new Searcher(searcherId, searcherInfo.NameKey, fieldKey, true, 0));
			}

			Searchers searchers = new Searchers();
			searchers.AddGroups(searcherGroups.Values);
			return searchers;
		}

        private IServiceOutcome<string> GetEditHtml(string searcherId, SearchContextWithFieldValues searchContextWithFieldValues, IDictionary<string, string> displayParams)
        {
		    if (null == searcherId)
		    {
			    return ServiceOutcome<string>.Error("No search renderer found");
		    }
            
            var issueSearcher = AssetSearcherManager.GetSearcher(searcherId);
		    if (issueSearcher == null)
		    {
                return ServiceOutcome<string>.Error("No search renderer found");
		    }

		    //RecentSearchersService.addRecentSearcher(user, issueSearcher);

		    string editHtml = issueSearcher.SearchRenderer.getEditHtml(user, searchContextWithFieldValues.searchContext, searchContextWithFieldValues.fieldValuesHolder, displayParams, action);
            if (editHtml.IsNullOrEmpty())
		    {
			    editHtml = emptySearchResultHtmlProvider.getHtml(searcherId);
		    }

		    return ServiceOutcome<string>.Ok(editHtml);
        }

        private IDictionary<string, string> CreateDisplayParams()
        {
            var displayParams = new Dictionary<string, string>();
            displayParams["theme"] = "aui";
            displayParams["checkboxmultiselect"] = "on";
            return displayParams;
        }
    }
}