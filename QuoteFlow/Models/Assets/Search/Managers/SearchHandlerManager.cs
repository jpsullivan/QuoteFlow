using System;
using System.Collections.Generic;
using QuoteFlow.Models.Assets.Fields;
using QuoteFlow.Models.Assets.Search.Searchers;
using QuoteFlow.Models.Search.Jql.Clauses;
using QuoteFlow.Models.Search.Jql.Values;

namespace QuoteFlow.Models.Assets.Search.Managers
{
    public class SearchHandlerManager : ISearchHandlerManager
    {
        private readonly ISystemClauseHandlerFactory systemClauseHandlerFactory;
//        private readonly QueryCache queryCache;
//        private readonly CachedReference<Helper> helperResettableLazyReference;

        public SearchHandlerManager(ISystemClauseHandlerFactory systemClauseHandlerFactory)
        {
            if (systemClauseHandlerFactory == null)
            {
                throw new ArgumentNullException("systemClauseHandlerFactory");
            }

            this.systemClauseHandlerFactory = systemClauseHandlerFactory;
        }

        public ICollection<IAssetSearcher<ISearchableField>> GetSearchers(User searcher, SearchContext context)
        {
            throw new NotImplementedException();
        }

        public ICollection<IAssetSearcher<ISearchableField>> AllSearchers { get; private set; }
        public ICollection<SearcherGroup> SearcherGroups { get; private set; }
        public IAssetSearcher<ISearchableField> GetSearcher(string id)
        {
            throw new NotImplementedException();
        }

        public void Refresh()
        {
            throw new NotImplementedException();
        }

        public ICollection<ClauseHandler> GetClauseHandler(User user, string jqlClauseName)
        {
            throw new NotImplementedException();
        }

        public ICollection<ClauseHandler> GetClauseHandler(string jqlClauseName)
        {
            throw new NotImplementedException();
        }

        public ICollection<ClauseNames> GetJqlClauseNames(string fieldId)
        {
            throw new NotImplementedException();
        }

        public ICollection<string> GetFieldIds(User searcher, string jqlClauseName)
        {
            throw new NotImplementedException();
        }

        public ICollection<string> GetFieldIds(string jqlClauseName)
        {
            throw new NotImplementedException();
        }

        public ICollection<ClauseNames> GetVisibleJqlClauseNames(User searcher)
        {
            throw new NotImplementedException();
        }

        public ICollection<IClauseHandler> getVisibleClauseHandlers(User searcher)
        {
            throw new NotImplementedException();
        }

        public ICollection<IAssetSearcher<ISearchableField>> GetSearchersByClauseName(User user, string jqlClauseName)
        {
            throw new NotImplementedException();
        }
    }
}