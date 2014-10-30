using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QuoteFlow.Models.Assets.Fields;
using QuoteFlow.Models.Assets.Search.Searchers;
using QuoteFlow.Models.Search.Jql.Clauses;
using QuoteFlow.Models.Search.Jql.Values;

namespace QuoteFlow.Models.Assets.Search.Managers
{
    public class SearchHandlerManager : ISearchHandlerManager
    {
        private readonly FieldManager fieldManager;
        private readonly CustomFieldManager customFieldManager;
        private readonly SystemClauseHandlerFactory systemClauseHandlerFactory;
        private readonly QueryCache queryCache;


        private readonly CachedReference<Helper> helperResettableLazyReference;


        public ICollection<IAssetSearcher<ISearchableField>> getSearchers(User searcher, SearchContext context)
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