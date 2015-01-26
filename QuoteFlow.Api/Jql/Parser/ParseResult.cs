using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Util;

namespace QuoteFlow.Api.Jql.Parser
{
    public class ParseResult
    {
        /// <summary>
        /// Returns the JQL <see cref="IQuery"/> parsed, or null if the query string was not valid.
        /// </summary>
        public IQuery Query { get; set; }
        public IMessageSet Errors { get; set; }

        public ParseResult(IQuery query, IMessageSet errors)
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