using System.Collections.Generic;
using QuoteFlow.Api.Jql.Operand;

namespace QuoteFlow.Api.Jql.Query.Operand
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