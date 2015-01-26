using System.Collections.Generic;
using QuoteFlow.Api.Infrastructure.Extensions;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Util;
using Wintellect.PowerCollections;

namespace QuoteFlow.Api.Jql.Validator
{
    /// <summary>
    /// Does validation to determine if the operator in use is something other than the specified accepted
    /// operators and adds an error message if so.
    /// </summary>
    public class SupportedOperatorsValidator
    {
        private readonly ICollection<Query.Operator> supportedOperators;

        public SupportedOperatorsValidator(params ICollection<Query.Operator>[] supportedOperatorSets)
        {
            var tmpOperators = new Set<Query.Operator>();
            foreach (var supportedOperatorSet in supportedOperatorSets)
            {
                tmpOperators.AddMany(supportedOperatorSet);
            }

            supportedOperators = new Set<Query.Operator>(tmpOperators);
        }

        public virtual IMessageSet Validate(User searcher, ITerminalClause terminalClause)
        {
            var messageSet = new MessageSet();
            // First lets validate that we are not being used with the greater-than/less-than operators, we don't support it
            Query.Operator @operator = terminalClause.Operator;
            
            if (supportedOperators.Contains(@operator)) return messageSet;
            
            messageSet.AddErrorMessage(string.Format("quoteflow.jql.clause.does.not.support.operator: {0}, {1}", @operator.GetDisplayAttributeFrom(typeof(Query.Operator)), terminalClause.Name));
            return messageSet;
        }
    }
}