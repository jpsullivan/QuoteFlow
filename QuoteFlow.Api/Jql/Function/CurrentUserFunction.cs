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
    /// Creates a value that is the current search user.
    /// 
    /// @since v4.0
    /// </summary>
    public class CurrentUserFunction : AbstractJqlFunction
    {
        public const string FUNCTION_CURRENT_USER = "currentUser";
        private const int EXPECTED_ARGS = 0;

        public virtual IMessageSet Validate(User searcher, FunctionOperand operand, ITerminalClause terminalClause)
        {
            return ValidateNumberOfArgs(operand, EXPECTED_ARGS);
        }

        public virtual IEnumerable<QueryLiteral> GetValues(IQueryCreationContext queryCreationContext, FunctionOperand operand, TerminalClause terminalClause)
        {
            if (queryCreationContext == null || queryCreationContext.User == null)
            {
                return new List<QueryLiteral>();
            }
            return new List<QueryLiteral> {new QueryLiteral(operand, queryCreationContext.User.Username)};
        }

        public virtual int MinimumNumberOfExpectedArguments
        {
            get { return 0; }
        }

        public virtual IQuoteFlowDataType DataType
        {
            get { return QuoteFlowDataTypes.User; }
        }
    }

}