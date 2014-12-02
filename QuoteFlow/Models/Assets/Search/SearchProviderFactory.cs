using System;
using Lucene.Net.Search;
using QuoteFlow.Models.Assets.Index;

namespace QuoteFlow.Models.Assets.Search
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
            if (SearchProviderTypes.ISSUE_INDEX.Equals(searcherName))
            {
                return AssetIndexManager.AssetSearcher;
            }
            if (SearchProviderTypes.COMMENT_INDEX.Equals(searcherName))
            {
                return AssetIndexManager.CommentSearcher;
            }
            if (SearchProviderTypes.CHANGE_HISTORY_INDEX.Equals(searcherName))
            {
                return AssetIndexManager.ChangeHistorySearcher;
            }
            throw new NotSupportedException("Only issue, comment and change history indexes are catered for currently");

        }
    }
}