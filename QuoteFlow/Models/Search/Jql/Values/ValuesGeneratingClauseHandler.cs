using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QuoteFlow.Models.Search.Jql.Clauses;
using QuoteFlow.Models.Search.Jql.Context;
using QuoteFlow.Models.Search.Jql.Query;
using QuoteFlow.Models.Search.Jql.Validator;

namespace QuoteFlow.Models.Search.Jql.Values
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