using System.Collections.Generic;

namespace QuoteFlow.Models.Search.Jql.Query.Clause
{
    /// <summary>
    /// Represents a node in the tree that gets generated for a <see cref="Query"/>.
    /// The tree of these will be used to generate an overall search.
    /// </summary>
    public interface IClause
    {
        /// <summary>
        /// The name of the individual clause, this should be unique amongst the implementations otherwise
        /// the clauses will be treated as the "same" type of clause.
        /// </summary>
        /// <returns>The name of the individual clause.</returns>
        string Name { get; }

        /// <summary>
        /// Child clauses if the clause has any, empty list if it has none.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IClause> Clauses { get; }

        /// <summary>
		/// Allows us to perform operations over the clauses based on the passed in visitor. This method calls the
		/// visit method on the visitor with this reference.
		/// </summary>
		/// <param name="visitor"> the visitor to accept.
		/// </param>
		/// <returns> the result of the visit operation who's type is specified by the incoming visitor. </returns>
		T Accept<T>(IClauseVisitor<T> visitor);

        /// <summary>
        /// Return a string representation of the clause. This string representation should not 
        /// be used to represent the clause to the user as it may not be valid. For example, this 
        /// method makes no attempt to escape invalid names and strings.
        /// </summary>
        /// <returns></returns>
        string ToString();
    }
}