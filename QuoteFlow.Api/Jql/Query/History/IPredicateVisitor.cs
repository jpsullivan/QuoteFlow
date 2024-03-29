﻿namespace QuoteFlow.Api.Jql.Query.History
{
    /// <summary>
    /// A visitor that allows you to perform operations on a <see cref="IHistoryPredicate"/>.
    /// </summary>
    public interface IPredicateVisitor<T>
    {
        /// <summary>
        /// Visit called when accepting a <see cref="IHistoryPredicate"/>.
        /// </summary>
        /// <param name="predicate">The node being visited.</param>
        /// <returns>The return type specified by the visitor.</returns>
        T Visit(IHistoryPredicate predicate);
    }
}