﻿using QuoteFlow.Models.Search.Jql.Clauses;
using QuoteFlow.Models.Search.Jql.Query;
using QuoteFlow.Models.Search.Jql.Query.Clause;
using QuoteFlow.Models.Search.Jql.Validator;
using Wintellect.PowerCollections;

namespace QuoteFlow.Models.Assets.CustomFields.Searchers
{
    /// <summary>
    /// Provides access to objects that can perform validation and query generation for clauses generated by this searcher.
    /// </summary>
    public interface ICustomFieldSearcherClauseHandler
    {
        /// <summary>
        /// Provides a validator for <seealso cref="TerminalClause"/>'s created by this searcher.
        /// </summary>
        /// <returns> a validator for <seealso cref="TerminalClause"/>'s created by this searcher. </returns>
        IClauseValidator ClauseValidator { get; }

        /// <summary>
        /// Provides a lucene query generator for <seealso cref="TerminalClause"/>'s created by this searcher.
        /// </summary>
        /// <returns> a lucene query generator for <seealso cref="TerminalClause"/>'s created by this searcher. </returns>
        IClauseQueryFactory ClauseQueryFactory { get; }

        /// <summary>
        /// Provides a set of the supported <seealso cref="Operator"/>'s that this custom field searcher
        /// can handle for its searching.
        /// 
        /// This will be used to populate the <seealso cref="IClauseInformation.GetSupportedOperators()"/>.
        /// </summary>
        /// <returns> a set of supported operators. </returns>
        Set<Operator> SupportedOperators { get; }

//        /// <summary>
//        /// Provides the <seealso cref="com.atlassian.jira.JiraDataType"/> that this clause handles and searches on. This allows us
//        /// to infer some information about how the search will behave and how it will interact with other elements in
//        /// the system.
//        /// 
//        /// This will be used to populate the <seealso cref="com.atlassian.jira.jql.ClauseInformation#getDataType()"/>.
//        /// </summary>
//        /// <returns> the JiraDataType that this clause can handle. </returns>
//        JiraDataType DataType { get; }

    }
}