using System;
using System.Collections.Generic;
using Lucene.Net.Search.Vectorhighlight;
using QuoteFlow.Models;
using QuoteFlow.Models.Assets.Fields;
using QuoteFlow.Models.Assets.Search;
using QuoteFlow.Models.Assets.Search.Searchers;
using QuoteFlow.Models.Assets.Search.Searchers.Transformer;
using QuoteFlow.Models.Assets.Transport;
using QuoteFlow.Models.Search.Jql;
using QuoteFlow.Models.Search.Jql.Parser;
using QuoteFlow.Models.Search.Jql.Query;
using QuoteFlow.Models.Search.Jql.Query.Clause;
using QuoteFlow.Services.Interfaces;
using Wintellect.PowerCollections;

namespace QuoteFlow.Services
{
    public class AssetSearchService : IAssetSearchService
    {
        #region IoC

        protected ISearchService SearchService { get; set; }

        public AssetSearchService() { }

        public AssetSearchService(ISearchService searchService)
        {
            SearchService = searchService;
        }

        #endregion

        public QuerySearchResults Search(MultiDictionary<string, string[]> paramMap, long filterId)
        {
            throw new NotImplementedException();
        }

        public QuerySearchResults SearchWithJql(string paramString, long filterId)
        {
            throw new NotImplementedException();
        }

        private IDictionary<string, SearchRendererHolder> GenerateQuery<T>(ISearchContext searchContext, User user, IQuery query, IEnumerable<T> searchers)
        {
            var clauses = new HashMap<string, SearchRendererHolder>();
            foreach (var searcher in searchers)
            {
                var assetSearcher = (IAssetSearcher<ISearchableField>)searcher;
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
                if (jqlParams.ContainsKey(id))
                {
                    ParseResult parseResult = SearchService.ParseQuery(user, jqlParams[id][0]);
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

        private IActionParams GetActionParameters(MultiDictionary<string, string[]> paramMap)
        {
            var newParams = (MultiDictionary<string, string[]>) paramMap.FindAll(i => !i.Key.StartsWith("__jql_"));
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
    }
}