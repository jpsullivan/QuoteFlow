using QuoteFlow.Models.Search.Jql.Query.History;

namespace QuoteFlow.Models.Search.Jql.Query.Clause
{
    /// <summary>
    /// Used to represent changed clause in the query tree
    /// </summary>
    public interface IChangedClause : IClause
    {
        string Field { get; set; }

        IHistoryPredicate Predicate { get; set; }

        Operator Operator { get; set; }
    }
}