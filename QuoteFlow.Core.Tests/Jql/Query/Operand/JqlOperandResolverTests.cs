using System.Collections.Generic;
using Moq;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Query.Operand;
using QuoteFlow.Api.Jql.Query.Operand.Registry;
using QuoteFlow.Api.Models;
using QuoteFlow.Core.Asset.Search;
using QuoteFlow.Core.Jql.Operand;
using QuoteFlow.Core.Tests.Jql.Validator;
using Xunit;

namespace QuoteFlow.Core.Tests.Jql.Query.Operand
{
    public class JqlOperandResolverTests
    {
        private static readonly string Field = "blah";
        private static readonly User TheUser = null;

        private static Mock<IQueryCreationContext> QueryCreationContext;
        private static Mock<IQueryCache> QueryCache;
        private static Mock<IJqlFunctionHandlerRegistry> JqlFunctionHandlerRegistry; 

        public class TheCtor
        {
        }

        public class TheGetValuesMethod
        {
            [Fact]
            public void WithEmptyOperand()
            {
                var expectedValues = new List<QueryLiteral>();
                var clause = new TerminalClause(Field, Operator.EQUALS, EmptyOperand.Empty);
                var mockEmptyHandler = new Mock<IOperandHandler<EmptyOperand>>();
                mockEmptyHandler.Setup(x => x.GetValues(QueryCreationContext.Object, EmptyOperand.Empty, clause))
                    .Returns(expectedValues);

                var jqlOperandSupport = CreateResolver(null, mockEmptyHandler.Object, null, null);
                var list = jqlOperandSupport.GetValues(QueryCreationContext.Object, EmptyOperand.Empty, clause);
                Assert.Equal(expectedValues, list);
            }
        }

        private static JqlOperandResolver CreateResolver(IJqlFunctionHandlerRegistry functionRegistry,
            IOperandHandler<EmptyOperand> emptyHandler, IOperandHandler<SingleValueOperand> singleHandler,
            IOperandHandler<MultiValueOperand> multiHandler)
        {
            if (functionRegistry == null)
            {
                functionRegistry = new MockJqlFunctionHandlerRegistry();
            }
            if (emptyHandler == null)
            {
                emptyHandler = new MockOperandHandler<EmptyOperand>(false, true, false);
            }
            if (singleHandler == null)
            {
                singleHandler = new MockOperandHandler<SingleValueOperand>(false, false, false);
            }
            if (multiHandler == null)
            {
                multiHandler = new MockOperandHandler<MultiValueOperand>(true, false, false);
            }

            return new JqlOperandResolver(functionRegistry, emptyHandler, singleHandler, multiHandler, QueryCache.Object);
        }
    }
}