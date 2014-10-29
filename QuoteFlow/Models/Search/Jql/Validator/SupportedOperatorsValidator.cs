using System.Collections.Generic;
using QuoteFlow.Infrastructure.Extensions;
using QuoteFlow.Infrastructure.Util;
using QuoteFlow.Models.Search.Jql.Query;
using QuoteFlow.Models.Search.Jql.Query.Clause;
using Wintellect.PowerCollections;

namespace QuoteFlow.Models.Search.Jql.Validator
{
    /// <summary>
    /// Does validation to determine if the operator in use is something other than the specified accepted
    /// operators and adds an error message if so.
    /// </summary>
    public class SupportedOperatorsValidator
    {
        private readonly ICollection<Operator> supportedOperators;

        public SupportedOperatorsValidator(params ICollection<Operator>[] supportedOperatorSets)
        {
            var tmpOperators = new Set<Operator>();
            foreach (var supportedOperatorSet in supportedOperatorSets)
            {
                tmpOperators.AddMany(supportedOperatorSet);
            }

            supportedOperators = new Set<Operator>(tmpOperators);
        }

        public virtual IMessageSet Validate(User searcher, ITerminalClause terminalClause)
        {
            var messageSet = new MessageSet();
            // First lets validate that we are not being used with the greater-than/less-than operators, we don't support it
            Operator @operator = terminalClause.Operator;
            
            if (supportedOperators.Contains(@operator)) return messageSet;
            
            messageSet.AddErrorMessage(string.Format("quoteflow.jql.clause.does.not.support.operator: {0}, {1}", @operator.GetDisplayAttributeFrom(typeof(Operator)), terminalClause.Name));
            return messageSet;
        }
    }
}