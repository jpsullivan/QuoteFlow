using System.Collections.Generic;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Query.Clause;

namespace QuoteFlow.Core.Jql.Operand
{
    public class EmptyWasClauseOperandHandler
    {
        //private readonly ChangeHistoryFieldConfigurationManager changeHistoryFieldConfigurationManager;

        public EmptyWasClauseOperandHandler()
        {
            //this.changeHistoryFieldConfigurationManager = changeHistoryFieldConfigurationManager;
        }

        public virtual IList<QueryLiteral> GetEmptyValue(IWasClause clause)
        {
            var literals = new List<QueryLiteral>();
            if (clause != null)
            {
                literals.Add(new QueryLiteral(clause.Operand, GetStringValueForEmpty(clause)));
            }
            return literals;
        }

        private string GetStringValueForEmpty(IWasClause clause)
        {
            return null;
//            return (clause != null) ? changeHistoryFieldConfigurationManager.getEmptyValue(clause.Field.ToLower()) : null;
        }
    }
}