using QuoteFlow.Models.Search.Jql.Query;
using QuoteFlow.Models.Search.Jql.Query.Clause;
using QuoteFlow.Models.Search.Jql.Query.Operand;

namespace QuoteFlow.Models.Assets.Search
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