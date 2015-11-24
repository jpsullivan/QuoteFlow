﻿using QuoteFlow.Api.Asset.Search.Constants;
using QuoteFlow.Api.Jql;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Validator;
using Wintellect.PowerCollections;

namespace QuoteFlow.Api.Asset.CustomFields.Searchers
{
    /// <summary>
    /// Provides access to objects that can perform validation and query generation for clauses generated by this searcher.
    /// </summary>
    public interface ICustomFieldSearcherClauseHandler
    {
        /// <summary>
        /// Provides a validator for <see cref="TerminalClause"/>'s created by this searcher.
        /// </summary>
        /// <returns> a validator for <see cref="TerminalClause"/>'s created by this searcher. </returns>
        IClauseValidator ClauseValidator { get; }

        /// <summary>
        /// Provides a lucene query generator for <see cref="TerminalClause"/>'s created by this searcher.
        /// </summary>
        /// <returns> a lucene query generator for <see cref="TerminalClause"/>'s created by this searcher. </returns>
        IClauseQueryFactory ClauseQueryFactory { get; }

        /// <summary>
        /// Provides a set of the supported <see cref="Operator"/>'s that this custom field searcher
        /// can handle for its searching.
        /// 
        /// This will be used to populate the <see cref="IClauseInformation.SupportedOperators"/>.
        /// </summary>
        /// <returns> a set of supported operators. </returns>
        Set<Operator> SupportedOperators { get; }

        /// <summary>
        /// Provides the <see cref="IQuoteFlowDataType"/> that this clause handles and searches on. This allows us
        /// to infer some information about how the search will behave and how it will interact with other elements in
        /// the system.
        /// 
        /// This will be used to populate the <see cref="ClauseInformation.DataType"/>.
        /// </summary>
        /// <returns>The QuoteFlowDataType that this clause can handle.</returns>
        IQuoteFlowDataType DataType { get; }
    }
}