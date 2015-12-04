using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Operator;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Validator;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Util;

namespace QuoteFlow.Core.Jql.Validator
{
    public sealed class CostValidator : IClauseValidator
    {
        private readonly SupportedOperatorsValidator _supportedOperatorsValidator;
        private readonly DecimalValueValidator _decimalValueValidator; 

        public CostValidator(IJqlOperandResolver operandResolver)
        {
            _supportedOperatorsValidator = SupportedOperatorsValidator;
            _decimalValueValidator = GetIntegerValueValidator(operandResolver);

        }

        public IMessageSet Validate(User searcher, ITerminalClause terminalClause)
        {
            var errors = _supportedOperatorsValidator.Validate(searcher, terminalClause);
            if (!errors.HasAnyErrors())
            {
                errors = _decimalValueValidator.Validate(searcher, terminalClause);
            }
            return errors;
        }

        private static SupportedOperatorsValidator SupportedOperatorsValidator
            =>
                new SupportedOperatorsValidator(OperatorClasses.EqualityOperatorsWithEmpty,
                    OperatorClasses.RelationalOnlyOperators);

        private static DecimalValueValidator GetIntegerValueValidator(IJqlOperandResolver operandResolver)
        {
            return new DecimalValueValidator(operandResolver);
        }
    }
}