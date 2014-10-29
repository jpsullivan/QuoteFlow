using System;
using System.Threading.Tasks;
using QuoteFlow.Infrastructure.Util;
using QuoteFlow.Models;
using QuoteFlow.Models.Assets.Search;
using QuoteFlow.Models.Search;
using QuoteFlow.Models.Search.Jql.Operand;
using QuoteFlow.Models.Search.Jql.Parser;
using QuoteFlow.Models.Search.Jql.Query;
using QuoteFlow.Models.Search.Jql.Util;
using QuoteFlow.Services.Interfaces;

namespace QuoteFlow.Services
{
    public class SearchService : ISearchService
    {
        public IJqlQueryParser JqlQueryParser { get; set; }
        public IJqlStringSupport JqlStringSupport { get; set; }
        public JqlOperandResolver JqlOperandResolver { get; set; }

        public SearchService(
            IJqlQueryParser jqlQueryParser, 
            IJqlStringSupport jqlStringSupport, 
            JqlOperandResolver jqlOperandResolver)
        {
            JqlQueryParser = jqlQueryParser;
            JqlStringSupport = jqlStringSupport;
            JqlOperandResolver = jqlOperandResolver;
        }

        public Task<SearchResult> Search(SearchFilter filter)
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