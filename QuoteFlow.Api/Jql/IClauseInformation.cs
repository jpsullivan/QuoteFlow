using System.Collections.Generic;
using QuoteFlow.Api.Asset.Search;

namespace QuoteFlow.Api.Jql
{
    /// <summary>
    /// Used to tie together field names, JQL clause names, and document constant names.
    /// </summary>
    public interface IClauseInformation
    {
        /// <returns> the allowed JQL clause names. </returns>
        ClauseNames JqlClauseNames { get; }

        /// <summary>
        /// The string that represents the field id in the lucene index; may be null if the clause 
        /// does not search directly on the index e.g. "saved filter" or "all text" clause.
        /// </summary>
        string IndexField { get; }

        /// <summary>
        /// The system or custom field id that this clause is associated with; may be null if the 
        /// clause does not have a corresponding field e.g. "parent issue" or "saved filter" clause.
        /// </summary>
        string FieldId { get; }

        /// <summary>
        /// Provides a set of the supported <see cref="Operator"/>'s that this custom field searcher
        /// can handle for its searching.
        /// </summary>
        /// <returns>A set of supported operators.</returns>
        HashSet<Query.Operator> SupportedOperators { get; }

        /// <summary>
        /// Provides the <see cref="QuoteFlowDataType"/> that this clause handles and searches on. This allows us
        /// to infer some information about how the search will behave and how it will interact with other elements in
        /// the system.
        /// 
        /// For example, if this returns <see cref="QuoteFlowDataTypes.Date"/> then we know that we could provide
        /// users with a date picker for an input field, and we know that this clause should only be used by functions
        /// that also specify dates.
        /// </summary>
        /// <returns>The QuoteFlowDataType that this clause can handle.</returns>
        IQuoteFlowDataType DataType { get; }
    }
}