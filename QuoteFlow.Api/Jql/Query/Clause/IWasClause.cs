using QuoteFlow.Api.Jql.Query.History;

namespace QuoteFlow.Api.Jql.Query.Clause
{
    /// <summary>
    /// Used to represent WAS in the query tree.
    /// </summary>
    public interface IWasClause : ITerminalClause
    {
        string Field { get; set; }

        IHistoryPredicate Predicate { get; set; }
    }
}