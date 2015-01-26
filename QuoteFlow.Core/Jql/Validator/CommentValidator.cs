using QuoteFlow.Api.Asset.Search.Constants;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Validator;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Util;

namespace QuoteFlow.Core.Jql.Validator
{
    /// <summary>
    /// A clause validator for the comment system field.
    /// </summary>
    public class CommentValidator : FreeTextFieldValidator
    {
        private readonly IJqlOperandResolver jqlOperandResolver;
        
        public CommentValidator(IJqlOperandResolver jqlOperandResolver)
            : base(SystemSearchConstants.ForComments().IndexField, jqlOperandResolver)
        {
            this.jqlOperandResolver = jqlOperandResolver;
        }

        public override IMessageSet Validate(User searcher, ITerminalClause terminalClause)
        {
            // Comments are funny, they can not support empty searching so lets check that first
            if (jqlOperandResolver.IsEmptyOperand(terminalClause.Operand))
            {
                var messageSet = new MessageSet();
                messageSet.AddErrorMessage(string.Format("jira.jql.clause.field.does.not.support.empty: {0}", terminalClause.Name));
                return messageSet;
            }
            return base.Validate(searcher, terminalClause);
        }
    }
}