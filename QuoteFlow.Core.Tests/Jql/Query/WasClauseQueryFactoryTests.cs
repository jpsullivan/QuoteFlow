using Lucene.Net.Index;
using Lucene.Net.Search;
using Moq;
using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Query.Operand;
using QuoteFlow.Api.Jql.Resolver;
using QuoteFlow.Core.Jql.Query;
using QuoteFlow.Core.Tests.Jql.Validator;
using Xunit;

namespace QuoteFlow.Core.Tests.Jql.Query
{
    public class WasClauseQueryFactoryTests
    {
        public Mock<ISearchProviderFactory> MockSearchProviderFactory { get; set; }
        public Mock<MockJqlOperandResolver> MockJqlOperandResolver { get; set; }
        public Mock<HistoryPredicateQueryFactory> MockHistoryPredicateQueryFactory { get; set; }
        public Mock<ChangeHistoryFieldIdResolver> MockChangeHistoryFieldIdResolver { get; set; }

        public Mock<IOperandHandler<FunctionOperand>> MockOperandHandler { get; set; }
        public Mock<IndexSearcher> MockIndexSearcher { get; set; }
        public Mock<IndexReader> MockIndexReader { get; set; }

//        [Fact]
//        public void TestSupportedOperator()
//        {
//            var singleValueOperand = new SingleValueOperand("testOperand");
//            var queryLiteral = new QueryLiteral(singleValueOperand, "testOperand");
//
//            MockSearchProviderFactory.Setup(x => x.GetSearcher("changes"));
//        }
    }

    public class TestableWasClauseQueryFactory : WasClauseQueryFactory
    {
        public Mock<ISearchProviderFactory> MockSearchProviderFactory { get; set; }
        public Mock<MockJqlOperandResolver> MockJqlOperandResolver { get; set; }
        public Mock<HistoryPredicateQueryFactory> MockHistoryPredicateQueryFactory { get; set; }
        public Mock<ChangeHistoryFieldIdResolver> MockChangeHistoryFieldIdResolver { get; set; }

        public Mock<IOperandHandler<FunctionOperand>> MockOperandHandler { get; set; }
        public Mock<IndexSearcher> MockIndexSearcher { get; set; }
        public Mock<IndexReader> MockIndexReader { get; set; }

        public TestableWasClauseQueryFactory(MockBehavior behavior = MockBehavior.Default)
        {
            SearchProviderFactory = (MockSearchProviderFactory = new Mock<ISearchProviderFactory>(behavior)).Object;
            OperandResolver = (MockJqlOperandResolver = new Mock<MockJqlOperandResolver>(behavior)).Object;
            WasPredicateQueryFactory = (MockHistoryPredicateQueryFactory = new Mock<HistoryPredicateQueryFactory>(behavior)).Object;
            ChangeHistoryFieldIdResolver = (MockChangeHistoryFieldIdResolver = new Mock<ChangeHistoryFieldIdResolver>(behavior)).Object;
        }
    }
}
