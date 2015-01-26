using System;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Validator;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Util;

namespace QuoteFlow.Core.Jql.Validator
{
    /// <summary>
    /// Validation for the "all text fields" clause. Since this clause does not support searching on EMPTY, we can just
    /// reuse the <seealso cref="CommentValidator"/>.
    /// 
    /// "All text" clause only supports the LIKE operator - NOT LIKE is too hard due to field visibility calculations, and we
    /// couldn't decide whether or not aggregate results should be ORed or ANDed together.
    /// 
    /// All free text fields ultimately validate in the same way, using <seealso cref="FreeTextFieldValidator"/>, so we only do one
    /// validation as opposed to going through each field and validating.
    /// </summary>
    public class AllTextValidator : IClauseValidator
    {
        private readonly CommentValidator @delegate;
        private readonly SupportedOperatorsValidator supportedOperatorsValidator;

        public AllTextValidator(CommentValidator @delegate)
        {
            if (@delegate == null)
            {
                throw new ArgumentNullException("@delegate");
            }
            this.@delegate = @delegate;
            supportedOperatorsValidator = SupportedOperatorsValidator;
        }

        public virtual IMessageSet Validate(User searcher, ITerminalClause terminalClause)
        {
            var messageSet = supportedOperatorsValidator.Validate(searcher, terminalClause);
            if (messageSet.HasAnyErrors())
            {
                return messageSet;
            }
            return @delegate.Validate(searcher, terminalClause);
        }

        internal virtual SupportedOperatorsValidator SupportedOperatorsValidator
        {
            get
            {
                return new SupportedOperatorsValidator(new[] { Operator.LIKE });
            }
        }
    }

}