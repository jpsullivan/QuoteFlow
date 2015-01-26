using System.Collections.Generic;
using QuoteFlow.Api.Jql;
using QuoteFlow.Api.Jql.Context;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Query.Operand;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Core.Asset.Search
{
    /// <summary>
    /// The query cache is a request level cache that stores the result of expensive query operations.
    /// 
    /// The cache is indexed with Query User pairs.
    /// </summary>
    public interface IQueryCache
    {
        /// <summary>
        /// Retrieve the result of the last doesQueryFitFiterForm operation in the current thread.
        /// for the <seealso cref="User"/> <see cref="IQuery"/> pair.
        /// </summary>
        /// <param name="searcher"> the user who is performing the search </param>
        /// <param name="query"> the query for which to find the result for; cannot be null. </param>
        /// <returns> the last result of the doesQueryFitFiterForm operation for the
        /// <seealso cref="User"/> <seealso cref="IQuery"/> pair in the current thread, or null if
        /// the operation has yet to be performed. </returns>
        bool? GetDoesQueryFitFilterFormCache(User searcher, IQuery query);

        /// <summary>
        /// Set the cached result of a doesQueryFitFiterForm operation on the
        /// <seealso cref="User"/> <seealso cref="IQuery"/> pair. The cache result
        /// is only held for the current thread.
        /// </summary>
        /// <param name="searcher"> the user who is performing the search </param>
        /// <param name="query"> the query for which to store the result under; cannot be null </param>
        /// <param name="doesItFit"> the result of a doesSearchRequestFitNavigator operation for the.
        /// <seealso cref="User"/> <seealso cref="IQuery"/> </param>
        void SetDoesQueryFitFilterFormCache(User searcher, IQuery query, bool doesItFit);

        /// <summary>
        /// Retrieve the result of the last getQueryContext operation in the current thread
        /// for the <seealso cref="User"/> <seealso cref="IQuery"/> pair.
        /// </summary>
        /// <param name="searcher"> the user who is performing the search </param>
        /// <param name="query"> the query for which to find the result for; cannot be null. </param>
        /// <returns> the last result of the getQueryContext operation for the
        /// <seealso cref="User"/> <see cref="IQuery"/> pair in the current thread, or null if
        /// the operation has yet to be performed. </returns>
        IQueryContext GetQueryContextCache(User searcher, IQuery query);

        /// <summary>
        /// Set the cached result of a getQueryContext operation on the
        /// <seealso cref="User"/> <seealso cref="IQuery"/> pair. The cache result
        /// is only held for the current thread.
        /// </summary>
        /// <param name="searcher"> the user who is performing the search </param>
        /// <param name="query"> the query for which to store the result under; cannot be null. </param>
        /// <param name="queryContext"> the queryContext result to store
        /// <seealso cref="User"/> <see cref="IQuery"/> </param>
        void SetQueryContextCache(User searcher, IQuery query, IQueryContext queryContext);

        /// <summary>
        /// Retrieve the result of the last getSimpleQueryContext operation in the current thread
        /// for the <seealso cref="User"/><see cref="IQuery"/> pair.
        /// </summary>
        /// <param name="searcher"> the user who is performing the search </param>
        /// <param name="query"> the query for which to find the result for; cannot be null. </param>
        /// <returns> the last result of the getSimpleQueryContext operation for the
        /// <seealso cref="User"/><see cref="Query"/> pair in the current thread, or null if
        /// the operation has yet to be performed. </returns>
        IQueryContext GetSimpleQueryContextCache(User searcher, IQuery query);

        /// <summary>
        /// Set the cached result of a getSimpleQueryContext operation on the
        /// <seealso cref="User"/> <seealso cref="IQuery"/> pair. The cache result
        /// is only held for the current thread.
        /// </summary>
        /// <param name="searcher"> the user who is performing the search </param>
        /// <param name="query"> the query for which to store the result under; cannot be null. </param>
        /// <param name="queryContext"> the querySimpleContext result to store
        /// <seealso cref="User"/> <seealso cref="IQuery"/> </param>
        void SetSimpleQueryContextCache(User searcher, IQuery query, IQueryContext queryContext);

        /// <summary>
        /// Retrieve the collection of <seealso cref="com.atlassian.jira.jql.ClauseHandler"/>s registered
        /// for the <seealso cref="User"/> jqlClauseName pair.
        /// </summary>
        /// <param name="searcher"> the user who is performing the search </param>
        /// <param name="jqlClauseName"> the jQLClauseName for which to find the result for; cannot be null. </param>
        /// <returns> the collection of <seealso cref="IClauseHandler"/>s registered
        /// for the <seealso cref="User"/> jqlClauseName pair. </returns>
        ICollection<IClauseHandler> GetClauseHandlers(User searcher, string jqlClauseName);

        /// <summary>
        /// Set the cached result of a getSimpleQueryContext operation on the
        /// <seealso cref="User"/> <seealso cref="com.atlassian.query.Query"/> pair. The cache result
        /// is only held for the current thread.
        /// </summary>
        /// <param name="searcher">The user who is performing the search </param>
        /// <param name="jqlClauseName">The jQLClauseName for which to store the result under; cannot be null. </param>
        /// <param name="clauseHandlers">The collection of <seealso cref="IClauseHandler"/>s
        /// <seealso cref="User"/> <seealso cref="IClauseHandler"/> </param>
        void SetClauseHandlers(User searcher, string jqlClauseName, ICollection<IClauseHandler> clauseHandlers);

        /// <summary>
        /// Retrieve the list of <seealso cref="QueryLiteral"/>s registered
        /// for the <seealso cref="IQueryCreationContext"/> <seealso cref="IOperand"/> jqlClause triplet.
        /// 
        /// </summary>
        /// <param name="context"> the query context of the search, which cannot be null. </param>
        /// <param name="operand"> the Operand which cannot be null </param>
        /// <param name="jqlClause"> the jQLClause for which to find the result for; cannot be null. </param>
        /// <returns> the list of <seealso cref="QueryLiteral"/>s registered
        /// for the <seealso cref="QueryCreationContext"/> jqlClause pair. </returns>
        IList<QueryLiteral> GetValues(IQueryCreationContext context, IOperand operand, ITerminalClause jqlClause);

        /// <summary>
        /// Set the cached result of a GetValues operation on the
        /// for the <seealso cref="IQueryCreationContext"/> <seealso cref="IOperand"/> jqlClause triplet. 
        /// The cache result is only held for the current thread.
        /// </summary>
        /// <param name="context">The query context the search is being performed in </param>
        /// <param name="operand">The Operand which cannot be null </param>
        /// <param name="jqlClause">The jQLClause for which to store the result under; cannot be null. </param>
        /// <param name="values">The collection of <seealso cref="QueryLiteral"/>s </param>
        void SetValues(IQueryCreationContext context, IOperand operand, ITerminalClause jqlClause, IEnumerable<QueryLiteral> values);
    }
}