using System.Collections.Generic;
using QuoteFlow.Models.Search.Jql.Operand;

namespace QuoteFlow.Models.Search.Jql.Query
{
    /// <summary>
    /// An abstraction for the creation of a <see cref="Lucene.Search.Query"/> from an expression and operator.
    /// Because our Lucene index cannot support all operator-field combinations, we need to implement this in different
    /// ways for different fields.
    /// </summary>
    public interface IOperatorSpecificQueryFactory
    {
        /// <summary>
        /// Generates the Query for a single operand id.
        /// </summary>
        /// <param name="fieldName">The index field name the query should be generated for.</param>
        /// <param name="oprator">Operator which is handled by this implementation.</param>
        /// <param name="rawValues">The raw values provided to the operand that need to be converted to index values. </param>
        /// <returns>The queryFactoryResult that contains the query and the metadata.</returns>
        /// <exception cref="OperatorDoesNotSupportOperand">
        /// If the method is passed an operator that it
        /// can not handle. In this case the <see cref="HandlesOperator(Operator)"/> call will have
        /// returned false.
        /// </exception>
        /// <exception cref="OperatorDoesNotSupportSingleOperand">
        /// If the implementation does not supportthe operator for single values. 
        /// </exception>
        QueryFactoryResult CreateQueryForSingleValue(string fieldName, Operator oprator, List<QueryLiteral> rawValues);

        /// <summary>
        /// Generates the Query for a list of operand ids.
        /// </summary>
        /// <param name="fieldName">        the index field name the query should be generated for. </param>
        /// <param name="operator">         operator which is handled by this implementation. </param>
        /// <param name="rawValues">        the raw values provided to the operand that need to be converted to index values. </param>
        /// <returns> the queryFactoryResult that contains the query and the metadata.
        /// </returns>
        /// <exception cref="com.atlassian.query.operator.OperatorDoesNotSupportOperand"> if the method is passed an operator that it
        /// can not handle. In this case the <seealso cref="#handlesOperator(com.atlassian.query.operator.Operator)"/> call will have
        /// returned false. </exception>
        /// <exception cref="com.atlassian.query.operator.OperatorDoesNotSupportMultiValueOperand"> if the implementation does not support
        /// the operator for multiple values. </exception>
        QueryFactoryResult CreateQueryForMultipleValues(string fieldName, Operator @operator, List<QueryLiteral> rawValues);

        /// <summary>
        /// Generates the query for an operand that has an <see cref="OperandHandler"/> that
        /// returns true for the isEmpty method. This should generate a Lucene query that will perform the correct search
        /// for issues where the field value is not set.
        /// </summary>
        /// <param name="fieldName">The index field name the query should be generated for. </param>
        /// <param name="oprator">Operator which is handled by this implementation. </param>
        /// <returns> the queryFactoryResult that contains the query and the metadata. </returns>
        QueryFactoryResult CreateQueryForEmptyOperand(string fieldName, Operator oprator);

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="oprator">The operator in question.</param>
        /// <returns>True if this implementation can handle the operator, false otherwise.</returns>
        bool HandlesOperator(Operator oprator);
    }
}