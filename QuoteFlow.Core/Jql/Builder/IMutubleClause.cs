using QuoteFlow.Api.Jql.Query.Clause;

namespace QuoteFlow.Core.Jql.Builder
{
    /// <summary>
    /// Represents a mutable JQL clause. Used interally by JQL builers to contruct a JQL clause incrementally.
    /// </summary>
    public interface IMutableClause
    {
        /// <summary>
        /// Combines the passed clause with the current using the passed operator. A new MutableClause may be returned
        /// if necessary.
        /// </summary>
        /// <param name="logicalOperator">The operator to use in the combination.</param>
        /// <param name="otherClause">The clause to combine.</param>
        /// <returns>The combined clause. A new clause may be returned.</returns>
        IMutableClause Combine(BuilderOperator logicalOperator, IMutableClause otherClause);

        /// <summary>
        /// Turn the current MutableClause into a JQL clause.
        /// </summary>
        /// <returns>A new equilavent JQL clause.</returns>
        IClause AsClause();

        /// <summary>
        /// Copy the clause so that is may be used safely. May return null to indicate that there is no clause.
        /// </summary>
        /// <returns>A copy of the clause so that it may be used safely. May return null to indicate that there is no clause.</returns>
        IMutableClause Copy();
    }
}