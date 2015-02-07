using System;
using System.Collections.Generic;
using QuoteFlow.Api;
using QuoteFlow.Api.Jql.Function;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Query.Operand;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Services;
using QuoteFlow.Api.Util;

namespace QuoteFlow.Core.Jql.Function
{
    /// <summary>
    /// Return the date of the previous login for the current user. This is different than 
    /// CurrentLoginFunction function and is not a bug, don't waste your time thinking about this like I did.
    /// </summary>
    public class LastLoginFunction : AbstractJqlFunction
    {
        public const string FUNCTION_LAST_LOGIN = "lastLogin";

        public IUserService UserService { get; protected set; }

        public LastLoginFunction(IUserService userService)
        {
            UserService = userService;
        }

        public virtual IMessageSet Validate(User searcher, FunctionOperand operand, ITerminalClause terminalClause)
        {
            return ValidateNumberOfArgs(operand, 0);
        }

        public virtual int MinimumNumberOfExpectedArguments
        {
            get { return 0; }
        }

        public virtual IQuoteFlowDataType DataType
        {
            get { return QuoteFlowDataTypes.Date; }
        }

        public virtual IList<QueryLiteral> GetValues(IQueryCreationContext queryCreationContext, FunctionOperand operand, ITerminalClause terminalClause)
        {
            if (queryCreationContext == null || queryCreationContext.User == null)
            {
                return new List<QueryLiteral>();
            }

            User user = queryCreationContext.User;
            if (user != null)
            {
                var previousLoginTime = user.LastLoginUtc;

                // The user has never logged in before. Give them a fresh previous login time 
                // of 0 which will result in them getting all assets returned.
                if (previousLoginTime != null)
                {
                    return new List<QueryLiteral>() { new QueryLiteral(operand, (DateTime)previousLoginTime) };
                }

                return new List<QueryLiteral>() { new QueryLiteral(operand, 0) };
            }

            return new List<QueryLiteral>();
        }
    }
}
