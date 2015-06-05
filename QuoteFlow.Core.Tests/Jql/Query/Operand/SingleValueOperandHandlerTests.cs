using System.Linq;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Query.Operand;
using QuoteFlow.Api.Models;
using Xunit;

namespace QuoteFlow.Core.Tests.Jql.Query.Operand
{
    public class SingleValueOperandHandlerTests
    {
        private static readonly User TheUser = null;
        private static readonly IQueryCreationContext QueryCreationContext = new QueryCreationContext(TheUser);

        public class TheIsEmptyMethod
        {
            [Fact]
            public void ReturnsFalse_If_NewOperandHandler_IsProvided()
            {
                Assert.False(new SingleValueOperandHandler().IsEmpty());
            }
        }

        public class TheIsListMethod
        {
            [Fact]
            public void ReturnsFalse_If_NewOperandHandler_IsProvided()
            {
                Assert.False(new SingleValueOperandHandler().IsList());
            }
        }

        public class TheIsFunctionMethod
        {
            [Fact]
            public void ReturnsFalse_If_NewOperandHandler_IsProvided()
            {
                Assert.False(new SingleValueOperandHandler().IsFunction());
            }
        }

        public class TheValidateMethod
        {
            [Fact]
            public void ValidatesCorrectly()
            {
                var operandHandler = new SingleValueOperandHandler();
                Assert.NotNull(operandHandler.Validate(null, null, null));
                Assert.False(operandHandler.Validate(null, null, null).HasAnyMessages());
            }
        }

        public class TheGetValuesMethod
        {
            [Fact]
            public void GetsValues()
            {
                var operandHandler = new SingleValueOperandHandler();

                var operand = new SingleValueOperand("test");
                var terminalClause = new TerminalClause("field", Operator.EQUALS, operand);
                var list = operandHandler.GetValues(QueryCreationContext, operand, terminalClause);
                Assert.Equal(1, list.Count());
                Assert.Equal("test", list.First().StringValue);
                Assert.Null(list.First().IntValue);

                operand = new SingleValueOperand(10);
                terminalClause = new TerminalClause("field", Operator.EQUALS, operand);
                list = operandHandler.GetValues(QueryCreationContext, operand, terminalClause);
                Assert.Equal(1, list.Count());
                Assert.Equal(10, list.First().IntValue);
                Assert.Null(list.First().StringValue);
            }
        }
    }
}