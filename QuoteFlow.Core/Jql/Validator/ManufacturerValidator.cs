using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Operator;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Validator;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Util;
using QuoteFlow.Core.Jql.Resolver;

namespace QuoteFlow.Core.Jql.Validator
{
    /// <summary>
    /// A validator for the Manufacturer field clauses.
    /// </summary>
    public sealed class ManufacturerValidator : IClauseValidator
    {
        private readonly RawValuesExistValidator<Manufacturer> _rawValuesExistValidator;
		private readonly SupportedOperatorsValidator _supportedOperatorsValidator;

        public ManufacturerValidator(ManufacturerResolver issueTypeResolver, IJqlOperandResolver operandResolver)
		{
			_rawValuesExistValidator = GetRawValuesValidator(issueTypeResolver, operandResolver);
			_supportedOperatorsValidator = SupportedOperatorsValidator;
		}

		public IMessageSet Validate(User searcher, ITerminalClause terminalClause)
		{
			var errors = _supportedOperatorsValidator.Validate(searcher, terminalClause);
			if (!errors.HasAnyErrors())
			{
				errors = _rawValuesExistValidator.Validate(searcher, terminalClause);
			}
			return errors;
		}

        private static SupportedOperatorsValidator SupportedOperatorsValidator => new SupportedOperatorsValidator(OperatorClasses.EqualityOperatorsWithEmpty);

        private RawValuesExistValidator<Manufacturer> GetRawValuesValidator(ManufacturerResolver resolver, IJqlOperandResolver operandResolver)
		{
			return new RawValuesExistValidator<Manufacturer>(operandResolver, new AssetConstantInfoResolver<Manufacturer>(resolver));
		}
    }
}