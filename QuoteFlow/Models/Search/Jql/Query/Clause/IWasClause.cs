using QuoteFlow.Models.Search.Jql.Query.History;

namespace QuoteFlow.Models.Search.Jql.Query.Clause
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