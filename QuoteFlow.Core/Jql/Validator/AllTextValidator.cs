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
    /// reuse the <see cref="CommentValidator"/>.
    /// 
    /// "All text" clause only supports the LIKE operator - NOT LIKE is too hard due to field visibility calculations, and we
    /// couldn't decide whether or not aggregate results should be ORed or ANDed together.
    /// 
    /// All free text fields ultimately validate in the same way, using <see cref="FreeTextFieldValidator"/>, so we only do one
    /// validation as opposed to going through each field and validating.
    /// </summary>
    public class AllTextValidator : IClauseValidator
    {
        private readonly CommentValidator _delegate;
        private SupportedOperatorsValidator _supportedOperatorsValidator;

        public virtual SupportedOperatorsValidator SupportedOperatorsValidator
        {
            get { return _supportedOperatorsValidator ?? new SupportedOperatorsValidator(new[] {Operator.LIKE}); }
            set { _supportedOperatorsValidator = value; }
        }

        public AllTextValidator(CommentValidator @delegate)
        {
            if (@delegate == null)
            {
                throw new ArgumentNullException("@delegate");
            }
            
            _delegate = @delegate;
            _supportedOperatorsValidator = SupportedOperatorsValidator;
        }

        public AllTextValidator(CommentValidator @delegate, SupportedOperatorsValidator supportedOperatorsValidator)
        {
            _delegate = @delegate;
            _supportedOperatorsValidator = supportedOperatorsValidator;
        }

        public IMessageSet Validate(User searcher, ITerminalClause terminalClause)
        {
            var messageSet = _supportedOperatorsValidator.Validate(searcher, terminalClause);
            return messageSet.HasAnyErrors() ? messageSet : _delegate.Validate(searcher, terminalClause);
        }
    }
}