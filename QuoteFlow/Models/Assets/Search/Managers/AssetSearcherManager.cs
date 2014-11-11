using System.Collections.Generic;
using QuoteFlow.Models.Assets.Fields;
using QuoteFlow.Models.Assets.Search.Searchers;

namespace QuoteFlow.Models.Assets.Search.Managers
{
    public class AssetSearcherManager : IAssetSearcherManager
    {
        private readonly ISearchHandlerManager manager;

        public AssetSearcherManager(ISearchHandlerManager manager)
		{
			this.manager = manager;
		}

        public ICollection<IAssetSearcher<ISearchableField>> GetSearchers(User searcher, SearchContext context)
        {
            return manager.GetSearchers(searcher, context);
        }

        public ICollection<IAssetSearcher<ISearchableField>> AllSearchers { get; private set; }
        
        public ICollection<SearcherGroup> SearcherGroups { get; private set; }

        public IAssetSearcher<ISearchableField> GetSearcher(string id)
        {
            return manager.GetSearcher(id);
        }

        public void Refresh()
        {
            manager.Refresh();
        }
    }
}