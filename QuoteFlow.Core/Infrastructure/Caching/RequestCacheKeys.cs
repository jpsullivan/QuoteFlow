namespace QuoteFlow.Core.Infrastructure.Caching
{
    public class RequestCacheKeys
    {
        public const string QueryContextCache = "quoteflow.query.context.cache";
        public const string SimpleQueryContextCache = "quoteflow.simple.query.context.cache";
        public const string JqlClauseHandlerCache = "quoteflow.jql.clause.handler.cache";
        public const string QueryLiteralsCache = "quoteflow.jql.query.literals.cache";
    }
}
