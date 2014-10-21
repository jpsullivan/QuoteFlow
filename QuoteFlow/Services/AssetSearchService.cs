using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QuoteFlow.Models.Search.Jql;
using QuoteFlow.Services.Interfaces;

namespace QuoteFlow.Services
{
    public class AssetSearchService : IAssetSearchService
    {
        public AssetSearchService() { }

        public SearchResults Search(Dictionary<string, string[]> paramMap, long paramLong)
        {
            throw new NotImplementedException();
        }

        public SearchResults SearchWithJql(string paramString, long paramLong)
        {
            throw new NotImplementedException();
        }
    }
}