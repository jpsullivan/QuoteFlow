using System.Collections.Generic;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Query.Operand;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Util;

namespace QuoteFlow.Api.Jql.Function
{
    /// <summary>
    /// Functions in JQL can be used to provide values for search criteria. For example, the <tt>membersOf("myGroup")</tt>
    /// JQL function returns a list of the usernames who are members of the group "myGroup". This function can then be used
    /// in any JQL clause that operates on a list of usernames. For example, the JQL clause
    /// <tt>assignee in membersOf("myGroup")</tt> returns all issues assigned to a member of the JIRA group "myGroup". This
    /// is very powerful, as it removes the need to enumerate over all the members of the group manually.
    /// 
    /// Implementations of JQL functions need to know how to validate a <seealso cref="FunctionOperand"/>
    /// (which contains their arguments), and also need to know how to produce <seealso cref="QueryLiteral"/>
    /// values from that operand. They must also specify whether or not the function produces a list of values or a single
    /// value.
    /// 
    /// The validate and GetValues method take the <seealso cref="ITerminalClause"/> that contained the
    /// <seealso cref="FunctionOperand"/> on its left-hand side. This can be used to create advanced functionality,
    /// such as adjusting the functions result or validation based on the clauses right-hand side value or operator.
    /// 
    /// For plugin developers wishing to write their own JQL functions - you may find it useful to extend from our
    /// provided <seealso cref="AbstractJqlFunction"/>. In addition to implementing this
    /// interface, you must also provide an XML descriptor for your function. For an example, see {@link
    /// com.atlassian.jira.plugin.jql.function.JqlFunctionModuleDescriptor}.
    /// 
    /// <seealso cref="QueryLiteral"/>s returned by the {@link #GetValues(com.atlassian.jira.jql.query.QueryCreationContext,
    /// com.atlassian.query.operand.FunctionOperand, com.atlassian.query.clause.TerminalClause)} method must have the operand
    /// source of the passed in <seealso cref="FunctionOperand"/>.
    /// 
    /// The function must be thread safe. Only one instance of the function is created to service all JQL queries. As a
    /// result the function may have multiple threads calling it at the same time.
    /// 
    /// The function will be executed each time a query using it is run. A query is only going to run as fast as its
    /// slowest part, thus the function must be very fast to ensure that queries run as quickly as possible. The function also
    /// needs to perform well under concurrent load.
    /// </summary>
    public interface IJqlFunction
    {
        /// <summary>
        /// Will validate the function operand's arguments and report back any errors.
        /// </summary>
        /// <param name="searcher"> the user performing the search </param>
        /// <param name="operand"> the operand to validate </param>
        /// <param name="terminalClause"> the terminal clause that contains the operand </param>
        /// <returns> a MessageSet which will contain any validation errors or warnings or will be empty if there is nothing to
        ///         report; never null. </returns>
        IMessageSet Validate(User searcher, FunctionOperand operand, ITerminalClause terminalClause);

        /// <summary>
        /// <p>Gets the unexpanded values provided by the user on input. This is the output values that will later be
        /// transformed into index values.
        /// 
        /// <p>For example, a function who returns all the released versions of a specified project should return {@link
        /// com.atlassian.jira.jql.operand.QueryLiteral}s representing the ids of those versions. For correctness, always opt
        /// to return the most specific identifier for the object; if you can return either the id (which is stored in the
        /// index) or a string name (that would require resolving to get the index value), choose the id.
        /// </summary>
        /// <param name="queryCreationContext"> the context of query creation </param>
        /// <param name="operand"> the operand to get values from </param>
        /// <param name="terminalClause"> the terminal clause that contains the operand </param>
        /// <returns> a List of objects that represent this Operands raw values. Cannot be null. </returns>
        IEnumerable<QueryLiteral> GetValues(IQueryCreationContext queryCreationContext, FunctionOperand operand, ITerminalClause terminalClause);

        /// <summary>
        /// This method should return true if the function is meant to be used with the IN or NOT IN operators, that is, if
        /// the function should be viewed as returning a list. The method should return false when it is to be used with the
        /// other relational operators (e.g. =, !=, <, >, ...) that only work with single values.
        /// <p/>
        /// As a general rule, if a function is going to return more than one value then it should return true here,
        /// otherwise it should return false. This does not necessarily need to be the case. For example, it is possible for
        /// function that returns false here to return more than one value when it is run.
        /// </summary>
        /// <returns> true if the function can should be considered a list (i.e. work with IN and NOT IN), or false otherwise.
        ///         In this case it is considered to return a single value (i.e. work with =, !=, <, >, ...). </returns>
        bool IsList();

        /// <summary>
        /// This method must return the number of arguments that the function expects to perform its operation correctly. If
        /// the function can accept a variable number of arguments this value should be the lower limit. It is perfectly
        /// legal for a function to take no arguments and return 0 for this method.
        /// </summary>
        /// <returns> the number of arguments that the function expects to perform its operation correctly. Must be >=0. </returns>
        int MinimumNumberOfExpectedArguments { get; }

        /// <summary>
        /// The name of the function. Multiple calls to this method must return the same result. This means that the function
        /// name cannot be internationalised with respect to the searcher.
        /// </summary>
        /// <returns> the name of the function. Cannot be null. </returns>
        string FunctionName { get; }

        /// <summary>
        /// Provides the <seealso cref="IQuoteFlowDataType"/> that this function handles and creates values for. This
        /// allows us to infer some information about how it will interact with other elements in the system.
        /// 
        /// For example, if this returns <seealso cref="QuoteFlowDataTypes.Date"/> then we know that we can provide
        /// values for any clauses that also specify a data type of DATE.
        /// </summary>
        /// <returns>The QuoteFlow data type that this function produces values for. Cannot be null.</returns>
        IQuoteFlowDataType DataType { get; }

    }
}