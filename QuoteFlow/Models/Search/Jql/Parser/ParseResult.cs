using QuoteFlow.Infrastructure.Util;
using QuoteFlow.Models.Search.Jql.Query;

namespace QuoteFlow.Models.Search.Jql.Parser
{
    public class ParseResult
    {
        /// <summary>
        /// Returns the JQL <see cref="IQuery"/> parsed, or null if the query string was not valid.
        /// </summary>
        public IQuery Query { get; set; }
        public IMessageSet Errors { get; set; }

        public ParseResult(IQuery query, MessageSet errors)
        {
            Query = query;
            Errors = errors;
        }

        public bool IsValid()
        {
            return !Errors.HasAnyErrors();
        }

    }
}