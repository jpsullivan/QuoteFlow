using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Query.Operand;

namespace QuoteFlow.Api.Asset.Search
{
    /// <summary>
    /// A a key used for caching on Context, operand and <see cref="ITerminalClause"/> triplets.
    /// </summary>
    internal class QueryCacheLiteralsKey
    {
        private IQueryCreationContext Context { get; set; }
        private IOperand Operand { get; set; }
        private ITerminalClause JqlClause { get; set; }

        public QueryCacheLiteralsKey(IQueryCreationContext context, IOperand operand, ITerminalClause jqlClause)
        {
            Context = context;
            Operand = operand;
            JqlClause = jqlClause;
        }
    }
}