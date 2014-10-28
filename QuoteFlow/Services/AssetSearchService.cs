using System;
using System.Collections.Generic;
using Lucene.Net.Search.Vectorhighlight;
using QuoteFlow.Infrastructure.Enumerables;
using QuoteFlow.Models;
using QuoteFlow.Models.Assets.Fields;
using QuoteFlow.Models.Assets.Search;
using QuoteFlow.Models.Assets.Search.Searchers;
using QuoteFlow.Models.Assets.Search.Searchers.Transformer;
using QuoteFlow.Models.Assets.Transport;
using QuoteFlow.Models.Search.Jql;
using QuoteFlow.Models.Search.Jql.Query;
using QuoteFlow.Models.Search.Jql.Query.Clause;
using QuoteFlow.Services.Interfaces;

namespace QuoteFlow.Services
{
    public class AssetSearchService : IAssetSearchService
    {
        public AssetSearchService() { }

        public SearchResults Search(ListWithDuplicates paramMap, long filterId)
        {
            throw new NotImplementedException();
        }

        public SearchResults SearchWithJql(string paramString, long filterId)
        {
            throw new NotImplementedException();
        }

        private Dictionary<string, SearchRendererHolder> GenerateQuery(ListWithDuplicates paramMap, User user, IEnumerable<IAssetSearcher<ISearchableField>> searchers)
        {
            return new Dictionary<string, SearchRendererHolder>();
        }

        private IDictionary<string, SearchRendererHolder> GenerateQuery(ISearchContext searchContext, User user, IQuery query, ICollection<T> searchers)
        {
            var clauses = new HashMap<string, SearchRendererHolder>();
            foreach (IAssetSearcher<ISearchableField> assetSearcher in searchers)
            {
                ISearchInputTransformer searchInputTransformer = assetSearcher.SearchInputTransformer;
                var fieldValuesHolder = new FieldValuesHolder();

                searchInputTransformer.PopulateFromQuery(user, fieldValuesHolder, query, searchContext);
                IClause clause = searchInputTransformer.GetSearchClause(user, fieldValuesHolder);
                if (null != clause)
                {
                    string id = assetSearcher.SearchInformation.Id;
                    clauses[id] = SearchRendererHolder.Valid(clause, fieldValuesHolder);
                }
            }
            return clauses;
        }
    }
}