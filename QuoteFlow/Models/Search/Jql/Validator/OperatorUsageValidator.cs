using QuoteFlow.Infrastructure.Extensions;
using QuoteFlow.Infrastructure.Util;
using QuoteFlow.Models.Search.Jql.Operand;
using QuoteFlow.Models.Search.Jql.Query.Clause;

namespace QuoteFlow.Models.Search.Jql.Validator
{
    public class OperatorUsageValidator : IOperatorUsageValidator
    {
        private readonly IJqlOperandResolver operandResolver;

        public OperatorUsageValidator(IJqlOperandResolver operandResolver)
        {
            this.operandResolver = operandResolver;
        }

        public bool Check(User searcher, ITerminalClause clause)
        {
            return Validate(searcher, clause, null);
        }

        public IMessageSet Validate(User searcher, ITerminalClause clause)
        {
            var messages = new MessageSet();
            Validate(searcher, clause, messages);
            return messages;
        }

        private bool Validate(User user, ITerminalClause clause, IMessageSet set)
        {
            var valid = true;
            var operand = clause.Operand;
            
            if (!operandResolver.IsValidOperand(operand)) return true;
            
            // Check some global rules
            var @operator = clause.Operator;
            bool isList = operandResolver.IsListOperand(operand);
            if (isList)
            {
                if (!OperatorClasses.ListOnlyOperators.Contains(@operator))
                {
                    valid = false;
                    AddError(user, set, "jira.jql.operator.usage.not.support.list", @operator.GetDisplayAttributeFrom(typeof(Query.Operator)),
                        operand.DisplayString, clause.Name);
                }
            }
            else
            {
                if (OperatorClasses.ListOnlyOperators.Contains(@operator))
                {
                    valid = false;
                    AddError(user, set, "jira.jql.operator.usage.not.support.non.list", @operator.GetDisplayAttributeFrom(typeof(Query.Operator)),
                        operand.DisplayString, clause.Name);
                }
            }

            if (operandResolver.IsEmptyOperand(operand))
            {
                if (OperatorClasses.EmptyOperators.Contains(@operator)) return valid;

                valid = false;
                AddError(user, set, "jira.jql.operator.usage.not.support.empty", @operator.GetDisplayAttributeFrom(typeof(Query.Operator)),
                    operand.DisplayString, clause.Name);
            }
            else
            {
                if (!OperatorClasses.EmptyOnlyOperators.Contains(@operator)) return valid;

                valid = false;
                AddError(user, set, "jira.jql.operator.usage.is.only.supports.empty", @operator.GetDisplayAttributeFrom(typeof(Query.Operator)),
                    clause.Name);
            }

            return valid;
        }


        private void AddError(User searcher, IMessageSet messageset, string key, params object[] values)
        {
            if (messageset == null) return;
            // todo: utilize the searcher to pull an i18n string from key
            messageset.AddErrorMessage(string.Format("{0}: {1}", key, values));
        }
    }
}