using System.Collections.Generic;
using QuoteFlow.Api.Asset.Index.Indexers;

namespace QuoteFlow.Core.Asset.Index.Managers
{
    public interface IFieldIndexerManager
    {
        IEnumerable<IFieldIndexer> AllAssetIndexers { get; }

        void Refresh();
    }
}