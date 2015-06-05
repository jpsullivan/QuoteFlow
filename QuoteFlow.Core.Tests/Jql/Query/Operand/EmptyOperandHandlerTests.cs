using System.Linq;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Query.Operand;
using QuoteFlow.Api.Models;
using Xunit;

namespace QuoteFlow.Core.Tests.Jql.Query.Operand
{
    public class EmptyOperandHandlerTests
    {
        private static readonly User TheUser = null;
        private static readonly IQueryCreationContext QueryCreationContext = new QueryCreationContext(TheUser);

        public class TheIsEmptyMethod
        {
            [Fact]
            public void ReturnsTrue_If_NewOperandHandler_IsProvided()
            {
                Assert.True(new EmptyOperandHandler().IsEmpty());
            }
        }

        public class TheIsListMethod
        {
            [Fact]
            public void ReturnsFalse_If_NewOperandHandler_IsProvided()
            {
                Assert.False(new EmptyOperandHandler().IsList());
            }
        }

        public class TheIsFunctionMethod
        {
            [Fact]
            public void ReturnsFalse_If_NewOperandHandler_IsProvided()
            {
                Assert.False(new EmptyOperandHandler().IsFunction());
            }
        }

        public class TheValidateMethod
        {
            [Fact]
            public void ValidatesCorrectly()
            {
                var terminalClause = new TerminalClause("bah", Operator.EQUALS, new EmptyOperand());
                var messageSet = new EmptyOperandHandler().Validate(TheUser, new EmptyOperand(), terminalClause);
                Assert.NotNull(messageSet);
                Assert.False(messageSet.HasAnyMessages());
            }
        }

        public class TheGetValuesMethod
        {
            [Fact]
            public void GetsValues()
            {
                var terminalClause = new TerminalClause("bah", Operator.EQUALS, new EmptyOperand());
                var values = new EmptyOperandHandler().GetValues(QueryCreationContext, new EmptyOperand(), terminalClause);
                Assert.NotNull(values);
                Assert.Equal(1, values.Count());
                Assert.Equal(new QueryLiteral(), values.First());
            }
        } 
    }
}