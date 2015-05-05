using System;
using Lucene.Net.Search;
using QuoteFlow.Api.Asset.Index;
using QuoteFlow.Api.Asset.Search;

namespace QuoteFlow.Core.Asset.Search
{
    public class SearchProviderFactory : ISearchProviderFactory
    {
        public IAssetIndexManager AssetIndexManager { get; protected set; }

        public SearchProviderFactory(IAssetIndexManager assetIndexManager)
        {
            AssetIndexManager = assetIndexManager;
        }

        public IndexSearcher GetSearcher(string searcherName)
        {
            if (SearchProviderTypes.AssetIndex.Equals(searcherName))
            {
                return AssetIndexManager.GetAssetSearcher();
            }
            if (SearchProviderTypes.COMMENT_INDEX.Equals(searcherName))
            {
                return AssetIndexManager.GetCommentSearcher();
            }

            throw new NotSupportedException("Only asset and comment indexes are catered for currently");
        }
    }
}