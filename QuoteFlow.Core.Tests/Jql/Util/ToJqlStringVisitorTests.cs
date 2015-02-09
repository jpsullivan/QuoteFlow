using System;
using System.Collections.Generic;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Query.History;
using QuoteFlow.Api.Jql.Query.Operand;
using QuoteFlow.Api.Jql.Util;
using QuoteFlow.Core.Jql.Query.Clause;
using QuoteFlow.Core.Jql.Query.History;
using QuoteFlow.Core.Jql.Util;
using Xunit;

namespace QuoteFlow.Core.Tests.Jql.Util
{
    public class ToJqlStringVisitorTests
    {
        public class TheVisitMethod
        {
            [Fact]
            public void EmptyOperand()
            {
                Assert.Equal(Api.Jql.Query.Operand.EmptyOperand.OperandName, Api.Jql.Query.Operand.EmptyOperand.Empty.Accept(new ToJqlStringVisitor(new MockJqlStringSupport())));
            }

            [Fact]
            public void FunctionOperand_NoArgs()
            {
                var operand = new FunctionOperand("function");
                Assert.Equal("funcName:function()", operand.Accept(new ToJqlStringVisitor(new MockJqlStringSupport())));
            }

            [Fact]
            public void FunctionOperand_WithArgs()
            {
                var operand = new FunctionOperand("functionName", "arg2", "arg3");
                Assert.Equal("funcName:functionName(funcArg:arg2, funcArg:arg3)", operand.Accept(new ToJqlStringVisitor(new MockJqlStringSupport())));
            }

            [Fact]
            public void MultiValueOperand()
            {
                var multi = new MultiValueOperand("value1", "value2");
                Assert.Equal("(value:value1, value:value2)", multi.Accept(new ToJqlStringVisitor(new MockJqlStringSupport())));
            }

            [Fact]
            public void SingleValueOperand_String()
            {
                var single = new SingleValueOperand("value1");
                Assert.Equal("value:value1", single.Accept(new ToJqlStringVisitor(new MockJqlStringSupport())));
            }

            [Fact]
            public void SingleValueOperand_Number()
            {
                var single = new SingleValueOperand(1029);
                Assert.Equal("1029", single.Accept(new ToJqlStringVisitor(new MockJqlStringSupport())));
            }

            [Fact]
            public void TerminalClause()
            {
                var clause = new TerminalClause("field", Operator.EQUALS, "value");
                var actual = clause.Accept(new ToJqlStringVisitor(new MockJqlStringSupport()));
                Assert.Equal("fieldName:field = value:value", actual.Jql);
            }

            [Fact]
            public void NotWith_TerminalClause()
            {
                var terminalClause = new TerminalClause("field", Operator.EQUALS, "value");
                var notClause = new NotClause(terminalClause);

                var actual = notClause.Accept(new ToJqlStringVisitor(new MockJqlStringSupport()));

                Assert.Equal("NOT fieldName:field = value:value", actual.Jql);
                Assert.Equal(ClausePrecedence.NOT, actual.Precedence);
            }

            [Fact]
            public void NotAndClause()
            {
                var andClause = new AndClause(new TerminalClause("field", Operator.EQUALS, "value"),
                    new TerminalClause("field2", Operator.EQUALS, "value2"));
                var notClause = new NotClause(andClause);

                var actual = notClause.Accept(new ToJqlStringVisitor(new MockJqlStringSupport()));

                Assert.Equal("NOT (fieldName:field = value:value AND fieldName:field2 = value:value2)", actual.Jql);
                Assert.Equal(ClausePrecedence.NOT, actual.Precedence);
            }

            [Fact]
            public void AndClause()
            {
                var andClause = new AndClause(new TerminalClause("field", Operator.LIKE, "value"),
                    new TerminalClause("field2", Operator.LESS_THAN, "value2"));

                var actual = andClause.Accept(new ToJqlStringVisitor(new MockJqlStringSupport()));
                Assert.Equal("fieldName:field ~ value:value AND fieldName:field2 < value:value2", actual.Jql);
                Assert.Equal(ClausePrecedence.AND, actual.Precedence);
            }

            [Fact]
            public void AndClause_Complex()
            {
                var andClause = new AndClause(new TerminalClause("field", Operator.LIKE, "value"),
                    new OrClause(
                        new TerminalClause("field2", Operator.LESS_THAN, "value2"),
                        new TerminalClause("field3", Operator.GREATER_THAN, "value3")
                    ));

                var actual = andClause.Accept(new ToJqlStringVisitor(new MockJqlStringSupport()));
                Assert.Equal("fieldName:field ~ value:value AND (fieldName:field2 < value:value2 OR fieldName:field3 > value:value3)", actual.Jql);
                Assert.Equal(ClausePrecedence.AND, actual.Precedence);
            }

            [Fact]
            public void OrClause()
            {
                var orClause = new OrClause(new TerminalClause("field", Operator.NOT_IN, "value"),
                    new TerminalClause("field2", Operator.IS, Api.Jql.Query.Operand.EmptyOperand.Empty));

                var actual = orClause.Accept(new ToJqlStringVisitor(new MockJqlStringSupport()));
                Assert.Equal("fieldName:field not in value:value OR fieldName:field2 is EMPTY", actual.Jql);
                Assert.Equal(ClausePrecedence.OR, actual.Precedence);
            }

            [Fact]
            public void OrClause_Complex()
            {
                OrClause orClause = new OrClause(new TerminalClause("field", Operator.NOT_IN, "value"),
                new AndClause(new TerminalClause("field2", Operator.IS, Api.Jql.Query.Operand.EmptyOperand.Empty),
                        new TerminalClause("field3", Operator.IS_NOT, Api.Jql.Query.Operand.EmptyOperand.Empty)));

                var actual = orClause.Accept(new ToJqlStringVisitor(new MockJqlStringSupport()));
                Assert.Equal("fieldName:field not in value:value OR fieldName:field2 is EMPTY AND fieldName:field3 is not EMPTY", actual.Jql);
                Assert.Equal(ClausePrecedence.OR, actual.Precedence);
            }

            [Fact]
            public void WasClause()
            {
                var wasClause = new WasClause("field", Operator.WAS, new SingleValueOperand("value"), null);
                var actual = wasClause.Accept(new ToJqlStringVisitor(new MockJqlStringSupport()));
                Assert.Equal("fieldName:field was value:value", actual.Jql);
                Assert.Equal(ClausePrecedence.TERMINAL, actual.Precedence);
            }

            [Fact]
            public void WasNotClause()
            {
                var wasClause = new WasClause("field", Operator.WAS_NOT, new SingleValueOperand("value"), null);
                var actual = wasClause.Accept(new ToJqlStringVisitor(new MockJqlStringSupport()));
                Assert.Equal("fieldName:field was not value:value", actual.Jql);
                Assert.Equal(ClausePrecedence.TERMINAL, actual.Precedence);
            }

            [Fact]
            public void WasClause_With_MultiplePredicates()
            {
                var wasClause = new WasClause("field", Operator.WAS, new SingleValueOperand("value"), GetHistoryPredicate());
                var actual = wasClause.Accept(new ToJqlStringVisitor(new MockJqlStringSupport()));
                Assert.Equal("fieldName:field was value:value by value:user after value:today ", actual.Jql);
                Assert.Equal(ClausePrecedence.TERMINAL, actual.Precedence);
            }

            private IHistoryPredicate GetHistoryPredicate()
            {
                var predicates = new List<IHistoryPredicate>();
                predicates.Add(new TerminalHistoryPredicate(Operator.BY, new SingleValueOperand("user")));
                predicates.Add(new TerminalHistoryPredicate(Operator.AFTER, new SingleValueOperand("today")));

                return new AndHistoryPredicate(predicates);
            }
        }

        private class MockJqlStringSupport : IJqlStringSupport
        {
            public virtual string EncodeStringValue(string value)
            {
                return "value:" + value;
            }

            public virtual string EncodeValue(string value)
            {
                return "value:" + value;
            }

            public virtual string EncodeFunctionArgument(string argument)
            {
                return "funcArg:" + argument;
            }

            public virtual string EncodeFunctionName(string functionName)
            {
                return "funcName:" + functionName;
            }

            public virtual string EncodeFieldName(string fieldName)
            {
                return "fieldName:" + fieldName;
            }

            public virtual string GenerateJqlString(IQuery query)
            {
                throw new NotSupportedException();
            }

            public virtual string GenerateJqlString(IClause clause)
            {
                throw new NotSupportedException();
            }

            public virtual HashSet<string> GetJqlReservedWords()
            {
                throw new NotSupportedException();
            }
        }
    }
}
