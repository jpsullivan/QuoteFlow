using System.Collections.Generic;
using Moq;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Query.Operand;
using QuoteFlow.Api.Models;
using QuoteFlow.Core.Tests.Jql.Validator;
using Xunit;

namespace QuoteFlow.Core.Tests.Jql.Query.Operand
{
    public class MultiValueOperandHandlerTests
    {
        private static readonly string Field = "field";
        private static readonly User TheUser = null;
        private static readonly IQueryCreationContext QueryCreationContext = new QueryCreationContext(TheUser);

        public class TheGetValuesMethod
        {
            [Fact]
            public void WithStrings()
            {
                var multiValueOperand = new MultiValueOperand("fred", "jane", "jimi", "123");
                var clause = new TerminalClause(Field, Operator.EQUALS, multiValueOperand);

                var mockMultiValueOperandHandler = new MultiValueOperandHandler(MockJqlOperandResolver.CreateSimpleSupport());
                var values = mockMultiValueOperandHandler.GetValues(QueryCreationContext, multiValueOperand, clause);

                var expectedList = new List<QueryLiteral>()
                {
                    SimpleLiteralFactory.CreateLiteral("fred"),
                    SimpleLiteralFactory.CreateLiteral("jane"),
                    SimpleLiteralFactory.CreateLiteral("jimi"),
                    SimpleLiteralFactory.CreateLiteral("123")
                };

                Assert.Equal(expectedList, values);
            }

            [Fact]
            public void WithInts()
            {
                var multiValueOperand = new MultiValueOperand(11, 1, 0, 9999);
                var clause = new TerminalClause(Field, Operator.EQUALS, multiValueOperand);

                var mockMultiValueOperandHandler = new MultiValueOperandHandler(MockJqlOperandResolver.CreateSimpleSupport());
                var values = mockMultiValueOperandHandler.GetValues(QueryCreationContext, multiValueOperand, clause);

                var expectedList = new List<QueryLiteral>()
                {
                    SimpleLiteralFactory.CreateLiteral(11),
                    SimpleLiteralFactory.CreateLiteral(1),
                    SimpleLiteralFactory.CreateLiteral(0),
                    SimpleLiteralFactory.CreateLiteral(9999)
                };

                Assert.Equal(expectedList, values);
            }

            [Fact]
            public void WithMixture()
            {
                var operandStringy = new SingleValueOperand("stringy");
                var operand2010 = new SingleValueOperand(2010);
                var operandMulti1 = new MultiValueOperand(new List<IOperand>()
                {
                    new SingleValueOperand("substring svo"),
                    new SingleValueOperand(333)
                });
                var operandMulti2 = new MultiValueOperand("sublist", "another");
                var operandValues = new List<IOperand>()
                {
                    operandStringy,
                    operand2010,
                    operandMulti1,
                    operandMulti2
                };

                var multiValueOperand = new MultiValueOperand(operandValues);
                var clause = new TerminalClause(Field, Operator.EQUALS, multiValueOperand);

                var mockMultiValueOperandHandler = new MultiValueOperandHandler(MockJqlOperandResolver.CreateSimpleSupport());
                var values = mockMultiValueOperandHandler.GetValues(QueryCreationContext, multiValueOperand, clause);

                var expectedList = new List<QueryLiteral>()
                {
                    SimpleLiteralFactory.CreateLiteral("stringy"),
                    SimpleLiteralFactory.CreateLiteral(2010),
                    SimpleLiteralFactory.CreateLiteral("substring svo"),
                    SimpleLiteralFactory.CreateLiteral(333),
                    SimpleLiteralFactory.CreateLiteral("sublist"),
                    SimpleLiteralFactory.CreateLiteral("another")
                };

                // sublists should be flattened out into one big list
                Assert.Equal(expectedList, values);
            }
        }

        public class TheIsEmptyMethod
        {
            [Fact]
            public void ReturnsFalse_If_NewOperandHandler_Provided()
            {
                // just mocking the operand resolver because I'm lazy
                var multiValueOperandHandler = new MultiValueOperandHandler(MockJqlOperandResolver.CreateSimpleSupport());
                Assert.False(multiValueOperandHandler.IsEmpty());
            }
        }

        public class TheIsListMethod
        {
            [Fact]
            public void ReturnsTrue_If_NewOperandHandler_Provided()
            {
                // just mocking the operand resolver because I'm lazy
                var multiValueOperandHandler = new MultiValueOperandHandler(MockJqlOperandResolver.CreateSimpleSupport());
                Assert.True(multiValueOperandHandler.IsList());
            }
        }

        public class TheIsFunctionMethod
        {
            [Fact]
            public void ReturnsFalse_If_NewOperandHandler_Provided()
            {
                // just mocking the operand resolver because I'm lazy
                var multiValueOperandHandler = new MultiValueOperandHandler(MockJqlOperandResolver.CreateSimpleSupport());
                Assert.False(multiValueOperandHandler.IsFunction());
            }
        }
    }
}