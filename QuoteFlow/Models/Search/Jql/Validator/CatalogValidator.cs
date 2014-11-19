using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QuoteFlow.Infrastructure.Util;
using QuoteFlow.Models.Search.Jql.Operand;
using QuoteFlow.Models.Search.Jql.Query.Clause;
using QuoteFlow.Models.Search.Jql.Resolver;
using QuoteFlow.Services.Interfaces;

namespace QuoteFlow.Models.Search.Jql.Validator
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
            throw new NotImplementedException();
        }

        SupportedOperatorsValidator GetSupportedOperatorsValidator()
        {
            return new SupportedOperatorsValidator(OperatorClasses.EqualityOperatorsWithEmpty);
        }

        ValuesExistValidator GetValuesValidator(CatalogResolver catalogResolver, JqlOperandResolver operandResolver, ICatalogService catalogService)
        {
            return new ValuesExistValidator(operandResolver, new CatalogIndexInfoResolver(catalogResolver), catalogService);
        }
    }
}