using System;
using System.Threading.Tasks;
using QuoteFlow.Infrastructure.Util;
using QuoteFlow.Models;
using QuoteFlow.Models.Assets.Search;
using QuoteFlow.Models.Search;
using QuoteFlow.Models.Search.Jql.Parser;
using QuoteFlow.Models.Search.Jql.Query;
using QuoteFlow.Services.Interfaces;

namespace QuoteFlow.Services
{
    public class LuceneSearchService : ISearchService
    {
        private Lucene.Net.Store.Directory _directory;

        private static readonly string[] FieldAliases = new[] { "Id", "Title", "Tag", "Tags", "Description", "Author", "Authors", "Owner", "Owners" };
        private static readonly string[] Fields = new[] { "Id", "Title", "Tags", "Description", "Authors", "Owners" };

        public bool ContainsAllVersions { get { return false; } }

        public LuceneSearchService(Lucene.Net.Store.Directory directory)
        {
            _directory = directory;
        }

        public Task<SearchResult> Search(SearchFilter searchFilter)
        {
            if (searchFilter == null)
            {
                throw new ArgumentNullException("searchFilter");
            }

            if (searchFilter.Skip < 0)
            {
                throw new ArgumentOutOfRangeException("searchFilter");
            }

            if (searchFilter.Take < 0)
            {
                throw new ArgumentOutOfRangeException("searchFilter");
            }

            return Task.FromResult(SearchCore(searchFilter));
        }

        private SearchResult SearchCore(SearchFilter searchFilter)
        {
            throw new NotImplementedException();
        }

        public SearchResults Search(User searcher, IQuery query)
        {
            throw new NotImplementedException();
        }

        public long SearchCount(User searcher, IQuery query)
        {
            throw new NotImplementedException();
        }

        public string GetQueryString(User searcher, string query)
        {
            throw new NotImplementedException();
        }

        public ParseResult ParseQuery(User searcher, string query)
        {
            throw new NotImplementedException();
        }

        public IMessageSet ValidateQuery(User searcher, IQuery query)
        {
            throw new NotImplementedException();
        }

        public IMessageSet ValidateQuery(User searcher, IQuery query, long searchRequestId)
        {
            throw new NotImplementedException();
        }
    }
}