namespace QuoteFlow.Models.Search.Jql.Query.History
{
    /// <summary>
    /// Represents a possibly composite expression that may evaluate to true or false for a given change history item.
    /// The intended use is querying the change groups of an issue to find those that contain a change item that matches
    /// the predicate.
    /// </summary>
    public interface IHistoryPredicate
    {
        string DisplayString { get; }

        /// <summary>
        /// Allows us to perform operations over the clauses based on the passed in visitor. This method calls the
        /// Visit method on the visitor with this reference.
        /// </summary>
        /// <param name="visitor"> the visitor to accept.
        /// </param>
        /// <returns>The result of the Visit operation who's type is specified by the incoming visitor.</returns>
        T Accept<T>(IPredicateVisitor<T> visitor);
    }
}