using System.Collections.Generic;
using QuoteFlow.Api.Asset.Fields;
using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Asset.Search.Managers;
using QuoteFlow.Api.Asset.Search.Searchers;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Core.Asset.Search.Managers
{
    public class AssetSearcherManager : IAssetSearcherManager
    {
        private readonly ISearchHandlerManager manager;

        public AssetSearcherManager(ISearchHandlerManager manager)
		{
			this.manager = manager;
		}

        public ICollection<IAssetSearcher<ISearchableField>> GetSearchers(User searcher, ISearchContext context)
        {
            return manager.GetSearchers(searcher, context);
        }

        public ICollection<IAssetSearcher<ISearchableField>> GetAllSearchers()
        {
            return manager.GetAllSearchers();
        }
        
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