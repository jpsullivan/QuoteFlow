using QuoteFlow.Api.Jql;
using QuoteFlow.Api.Jql.Context;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Validator;

namespace QuoteFlow.Core.Tests.Jql
{
    public class MockClauseHandler : IClauseHandler
    {
        public IClauseInformation Information { get; private set; }
        public IClauseQueryFactory Factory { get; private set; }
        public IClauseValidator Validator { get; private set; }
        public IClauseContextFactory ClauseContextFactory { get; private set; }

        public MockClauseHandler()
        {
        }

        public MockClauseHandler(IClauseQueryFactory factory, IClauseValidator validator, IClauseContextFactory clauseContextFactory)
        {
            Factory = factory;
            Validator = validator;
            ClauseContextFactory = clauseContextFactory;
        }

        public MockClauseHandler SetFactory(IClauseQueryFactory factory)
        {
            Factory = factory;
            return this;
        }

        public MockClauseHandler SetValidator(IClauseValidator validator)
        {
            Validator = validator;
            return this;
        }

        public MockClauseHandler SetContextFactory(IClauseContextFactory contextFactory)
        {
            ClauseContextFactory = contextFactory;
            return this;
        }

        public MockClauseHandler SetInformation(IClauseInformation information)
        {
            Information = information;
            return this;
        }
    }
}