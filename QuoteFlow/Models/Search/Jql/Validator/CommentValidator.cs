using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QuoteFlow.Infrastructure.Util;
using QuoteFlow.Models.Assets.Search.Constants;
using QuoteFlow.Models.Search.Jql.Operand;
using QuoteFlow.Models.Search.Jql.Query.Clause;

namespace QuoteFlow.Models.Search.Jql.Validator
{
    /// <summary>
    /// A clause validator for the comment system field.
    /// </summary>
    public class CommentValidator : FreeTextFieldValidator
    {
        private readonly IJqlOperandResolver jqlOperandResolver;
        
        public CommentValidator(JqlOperandResolver jqlOperandResolver)
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