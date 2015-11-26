using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Operator;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Resolver;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Util;

namespace QuoteFlow.Api.Jql.Validator
{
    /// <summary>
	/// An abstract Validator for the User field clauses
	/// </summary>
	public abstract class AbstractUserValidator : IClauseValidator
    {
        private readonly SupportedOperatorsValidator _supportedOperatorsValidator;
        private readonly DataValuesExistValidator<User>  _dataValuesExistValidator;

        public AbstractUserValidator(INameResolver<User> userResolver, IJqlOperandResolver operandResolver)
        {
            _supportedOperatorsValidator = SupportedOperatorsValidator;
            _dataValuesExistValidator = GetDataValuesValidator(userResolver, operandResolver);
        }

        public virtual IMessageSet Validate(User searcher, ITerminalClause terminalClause)
        {
            var errors = _supportedOperatorsValidator.Validate(searcher, terminalClause);
            if (!errors.HasAnyErrors())
            {
                errors = _dataValuesExistValidator.Validate(searcher, terminalClause);
            }
            return errors;
        }

        private SupportedOperatorsValidator SupportedOperatorsValidator => new SupportedOperatorsValidator(OperatorClasses.EqualityOperatorsWithEmpty);

        private DataValuesExistValidator<User> GetDataValuesValidator(INameResolver<User> resolver, IJqlOperandResolver operandResolver)
        {
            return new DataValuesExistValidator<User>(operandResolver, resolver, MessageSetLevel.Warning);
        }
    }
}