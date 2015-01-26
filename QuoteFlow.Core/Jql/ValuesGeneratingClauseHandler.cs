using QuoteFlow.Api.Jql;
using QuoteFlow.Api.Jql.Context;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Validator;
using QuoteFlow.Api.Jql.Values;

namespace QuoteFlow.Core.Jql
{
    public class ValuesGeneratingClauseHandler : IClauseHandler, IValueGeneratingClauseHandler
    {
        public IClauseValuesGenerator ValuesGenerator { get; set; }

        public IClauseInformation Information { get; private set; }
        public IClauseQueryFactory Factory { get; private set; }
        public IClauseValidator Validator { get; private set; }
        public IClauseContextFactory ClauseContextFactory { get; private set; }

        public ValuesGeneratingClauseHandler(
            IClauseInformation information, 
            IClauseQueryFactory factory, 
            IClauseValidator validator, 
            IClauseContextFactory clauseContextFactory, 
            IClauseValuesGenerator valuesGenerator)
        {
            Information = information;
            Factory = factory;
            Validator = validator;
            ClauseContextFactory = clauseContextFactory;
            ValuesGenerator = valuesGenerator;
        }

        public IClauseValuesGenerator GetClauseValuesGenerator()
        {
            return ValuesGenerator;
        }
    }
}