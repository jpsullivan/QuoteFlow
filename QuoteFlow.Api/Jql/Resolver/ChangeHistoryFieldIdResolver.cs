using System.Collections.Generic;
using System.Collections.ObjectModel;
using QuoteFlow.Api.Jql.Operand;

namespace QuoteFlow.Api.Jql.Resolver
{
    /// <summary>
    /// As both WasClauseQueryFactor and ChangedClauseQueryFactory need to resolve ids this is a helper class
    /// to accomplish this.
    /// 
    /// @since v5.0
    /// </summary>
    public class ChangeHistoryFieldIdResolver
    {
        // -1 is the generic empty indicator
        private static readonly ICollection<string> EmptyId = new Collection<string>() {"-1"};

        public virtual ICollection<string> ResolveIdsForField(string field, QueryLiteral literal, bool emptyOperand)
        {
            var value = (literal.IntValue != null) ? literal.IntValue.ToString() : literal.StringValue;
            if (emptyOperand)
            {
                return (value != null) ? new List<string>() { value } : EmptyId;
            }

            // If passed an id, that is what we should return.
            if (literal.IntValue != null)
            {
                return new[] { literal.IntValue.ToString() };
            }
//            // Things that can be renamed (like issue constants, versions, and now users) need special treatment
//            // during the history searches, because we need to search for IDs that had that value in the past,
//            // not just the ones that currently have it.
//            if (isConstantField(field))
//            {
//                return resolveIdsForConstantField(field, literal.StringValue);
//            }
//            if (isAssigneeOrReporterField(field))
//            {
//                return resolveIdsForUserField(value);
//            }
//            if (isVersionField(field))
//            {
//                return resolveIdsForVersion(value);
//            }
            return new List<string>() { value };
        }
    }
}