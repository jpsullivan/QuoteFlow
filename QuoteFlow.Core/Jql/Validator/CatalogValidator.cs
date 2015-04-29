using QuoteFlow.Api.Jql.Operator;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Validator;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Services;
using QuoteFlow.Api.Util;
using QuoteFlow.Core.Jql.Operand;
using QuoteFlow.Core.Jql.Resolver;

namespace QuoteFlow.Core.Jql.Validator
{
    public class CatalogValidator : IClauseValidator
    {
        private readonly ValuesExistValidator _catalogValuesExistValidator;
        private readonly SupportedOperatorsValidator _supportedOperatorsValidator;

        public CatalogValidator(CatalogResolver catalogResolver, JqlOperandResolver operandResolver, ICatalogService catalogService)
        {
            _catalogValuesExistValidator = GetValuesValidator(catalogResolver, operandResolver, catalogService);
            _supportedOperatorsValidator = GetSupportedOperatorsValidator();
        }

        public IMessageSet Validate(User searcher, ITerminalClause terminalClause)
        {
            var messageSet = _supportedOperatorsValidator.Validate(searcher, terminalClause);
            if (!messageSet.HasAnyErrors())
            {
                messageSet.AddMessageSet(_catalogValuesExistValidator.Validate(searcher, terminalClause));
            }
            return messageSet;
        }

        SupportedOperatorsValidator GetSupportedOperatorsValidator()
        {
            return new SupportedOperatorsValidator(OperatorClasses.EqualityOperatorsWithEmpty);
        }

        ValuesExistValidator GetValuesValidator(CatalogResolver catalogResolver, JqlOperandResolver operandResolver, ICatalogService catalogService)
        {
            return new CatalogValuesExistValidator(operandResolver, new CatalogIndexInfoResolver(catalogResolver), catalogService);
        }
    }
}