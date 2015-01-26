using QuoteFlow.Api.Jql.Query.History;

namespace QuoteFlow.Api.Jql.Query.Clause
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