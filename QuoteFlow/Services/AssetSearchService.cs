using System;
using System.Collections.Generic;
using QuoteFlow.Infrastructure.Enumerables;
using QuoteFlow.Models;
using QuoteFlow.Models.Search.Jql;
using QuoteFlow.Services.Interfaces;

namespace QuoteFlow.Services
{
    public class AssetSearchService : IAssetSearchService
    {
        public AssetSearchService() { }

        public SearchResults Search(ListWithDuplicates paramMap, long paramLong)
        {
            throw new NotImplementedException();
        }

        public SearchResults SearchWithJql(string paramString, long paramLong)
        {
            throw new NotImplementedException();
        }

        private Dictionary<string, SearchRendererHolder> GenerateQuery(ListWithDuplicates paramMap, User user, IEnumerable<AssetSearcher> )
        {
            
        }
    }
}