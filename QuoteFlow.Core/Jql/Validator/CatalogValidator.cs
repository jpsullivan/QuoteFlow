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
        private ValuesExistValidator catalogValuesExistValidator;
        private SupportedOperatorsValidator supportedOperatorsValidator;

        public CatalogValidator(CatalogResolver catalogResolver, JqlOperandResolver operandResolver, ICatalogService catalogService)
        {
            this.catalogValuesExistValidator = GetValuesValidator(catalogResolver, operandResolver, catalogService);
            this.supportedOperatorsValidator = GetSupportedOperatorsValidator();
        }

        public IMessageSet Validate(User searcher, ITerminalClause terminalClause)
        {
            var messageSet = supportedOperatorsValidator.Validate(searcher, terminalClause);
            if (!messageSet.HasAnyErrors())
            {
                messageSet.AddMessageSet(catalogValuesExistValidator.Validate(searcher, terminalClause));
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