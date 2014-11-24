using System.Collections.Generic;
using QuoteFlow.Models.Search.Jql.Operand;

namespace QuoteFlow.Models.Search.Jql.Query.Operand
{
    /// <summary>
    /// Return the values from the Operand.
    /// </summary>
    public interface IPredicateOperandHandler
    {
        IEnumerable<QueryLiteral> Values { get; }

        bool Empty { get; }

        bool List { get; }

        bool Function { get; }
    }
}