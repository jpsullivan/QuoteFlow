using System.Collections.Generic;
using QuoteFlow.Api.Jql.Operand;

namespace QuoteFlow.Api.Jql.Permission
{
    /// <summary>
    /// Defines how to sanitize a list of query literals.
    /// </summary>
    public interface ILiteralSanitizer
    {
        /// <summary>
        /// Note: in general, it is possible that a literal can expand out into multiple id values. The strategy for handling
        /// these should be that if ALL values are sanitised, then modification should occur, but if at least one is okay
        /// then we should keep the original literal.
        /// </summary>
        /// <param name="literals">The literals to sanitise; must not be null.</param>
        /// <returns>The result object, which states if there was any modification, and also contains the resultant literals. Never null.</returns>
        LiteralSanitizerResult SanitiseLiterals(IEnumerable<QueryLiteral> literals);
    }
}