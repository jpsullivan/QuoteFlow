using System;
using System.Collections.Generic;
using Moq;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Query.Operand;
using QuoteFlow.Core.Jql.Builder;
using QuoteFlow.Core.Jql.Query.Operand;
using QuoteFlow.Core.Jql.Util;
using Xunit;

namespace QuoteFlow.Core.Tests.Jql.Builder
{
    public class JqlClauseBuilderTests
    {
        [Fact]
        public void TestClear()
        {
            var mockClauseBuilder = new Mock<ISimpleClauseBuilder>(MockBehavior.Strict);
            var mockClauseBuilder2 = new Mock<ISimpleClauseBuilder>(MockBehavior.Strict);

            mockClauseBuilder.Setup(x => x.Clear()).Returns(mockClauseBuilder2.Object);
            mockClauseBuilder2.Setup(x => x.Clear()).Returns(mockClauseBuilder.Object);

            var builder = CreateBuilder(mockClauseBuilder.Object);
            Assert.Equal(builder, builder.Clear());
            Assert.Equal(builder, builder.Clear());

            mockClauseBuilder.Verify();
            mockClauseBuilder2.Verify();
        }

        [Fact]
        public void TestEmptyCondition()
        {
            var builder = new JqlClauseBuilder();
            try
            {
                builder.AddEmptyCondition(null);
                Assert.True(false, "Expected an exception.");
            }
            catch (ArgumentNullException)
            {
            }

            string clauseName = "name";

            var clauseBuilder = new Mock<ISimpleClauseBuilder>();
            clauseBuilder.Setup(x => x.Clause(new TerminalClause(clauseName, Operator.IS, EmptyOperand.Empty)))
                .Returns(clauseBuilder.Object);

            builder = CreateBuilder(clauseBuilder.Object);
            Assert.Equal(builder, builder.AddEmptyCondition(clauseName));

            clauseBuilder.Verify();
        }

        [Fact]
        public void TestEndWhere()
        {
            var queryBuilder = JqlQueryBuilder.NewBuilder();
            var builder = new JqlClauseBuilder(null);
            Assert.Null(builder.EndWhere());

            builder = new JqlClauseBuilder(queryBuilder);
            Assert.Same(queryBuilder, builder.EndWhere());
        }

        [Fact]
        public void TestAddStringRangeConditionString()
        {
            var builder = new JqlClauseBuilder();
            try
            {
                builder.AddStringCondition(null, string.Empty, string.Empty);
                Assert.True(false, "Expected exception.");
            }
            catch (Exception)
            {
            }

            try
            {
                builder.AddStringRangeCondition("qwerty", null, null);
                Assert.True(false, "Expected exception.");
            }
            catch (Exception)
            {
            }

            const string start = "%&*##*$&$";
            const string end = "dhakjdhakjhsdsa";
            const string clauseName = "weqekwq";

            var clauseBuilder = new Mock<ISimpleClauseBuilder>();
            clauseBuilder.Setup(x => x.Clause(new TerminalClause(clauseName, Operator.GREATER_THAN_EQUALS, start))).Returns(clauseBuilder.Object);
            clauseBuilder.Setup(x => x.Clause(new TerminalClause(clauseName, Operator.LESS_THAN_EQUALS, end))).Returns(clauseBuilder.Object);
            clauseBuilder.Setup(
                x =>
                    x.Clause(new AndClause(new TerminalClause(clauseName, Operator.GREATER_THAN_EQUALS, start),
                        new TerminalClause(
                            clauseName, Operator.LESS_THAN_EQUALS, end)))).Returns(clauseBuilder.Object);

            builder = CreateBuilder(clauseBuilder.Object);
            Assert.Equal(builder, builder.AddStringRangeCondition(clauseName, start, null));
            Assert.Equal(builder, builder.AddStringRangeCondition(clauseName, null, end));
            Assert.Equal(builder, builder.AddStringRangeCondition(clauseName, start, end));

            clauseBuilder.Verify();
        }

        [Fact]
        public void TestAddNumberRangeConditionString()
        {
            var builder = new JqlClauseBuilder();
            try
            {
                builder.AddNumberRangeCondition(null, 8, 23);
                Assert.True(false, "Expected exception.");
            }
            catch (Exception)
            {
            }

            try
            {
                builder.AddNumberRangeCondition("qwerty", null, null);
                Assert.True(false, "Expected exception.");
            }
            catch (Exception)
            {
            }

            const int start = 538748;
            const int end = 567;
            const string clauseName = "blah";

            var clauseBuilder = new Mock<ISimpleClauseBuilder>();
            clauseBuilder.Setup(x => x.Clause(new TerminalClause(clauseName, Operator.GREATER_THAN_EQUALS, start))).Returns(clauseBuilder.Object);
            clauseBuilder.Setup(x => x.Clause(new TerminalClause(clauseName, Operator.LESS_THAN_EQUALS, end))).Returns(clauseBuilder.Object);
            clauseBuilder.Setup(
                x =>
                    x.Clause(new AndClause(new TerminalClause(clauseName, Operator.GREATER_THAN_EQUALS, start),
                        new TerminalClause(
                            clauseName, Operator.LESS_THAN_EQUALS, end)))).Returns(clauseBuilder.Object);

            builder = CreateBuilder(clauseBuilder.Object);
            Assert.Equal(builder, builder.AddNumberRangeCondition(clauseName, start, null));
            Assert.Equal(builder, builder.AddNumberRangeCondition(clauseName, null, end));
            Assert.Equal(builder, builder.AddNumberRangeCondition(clauseName, start, end));

            clauseBuilder.Verify();
        }

        [Fact]
        public void TestAddRangeConditionString()
        {
            var builder = new JqlClauseBuilder();
            try
            {
                builder.AddRangeCondition(null, Operands.ValueOf(10), Operands.ValueOf(57584));
                Assert.True(false, "Expected exception.");
            }
            catch (Exception)
            {
            }

            try
            {
                builder.AddRangeCondition("qwerty", null, null);
                Assert.True(false, "Expected exception.");
            }
            catch (Exception)
            {
            }

            var start = Operands.ValueOf("%&*##*$&$");
            var end = Operands.ValueOf("dhakjdhakjhsdsa");
            const string clauseName = "qwerty";

            var clauseBuilder = new Mock<ISimpleClauseBuilder>();
            clauseBuilder.Setup(x => x.Clause(new TerminalClause(clauseName, Operator.GREATER_THAN_EQUALS, start))).Returns(clauseBuilder.Object);
            clauseBuilder.Setup(x => x.Clause(new TerminalClause(clauseName, Operator.LESS_THAN_EQUALS, end))).Returns(clauseBuilder.Object);
            clauseBuilder.Setup(
                x =>
                    x.Clause(new AndClause(new TerminalClause(clauseName, Operator.GREATER_THAN_EQUALS, start),
                        new TerminalClause(
                            clauseName, Operator.LESS_THAN_EQUALS, end)))).Returns(clauseBuilder.Object);

            builder = CreateBuilder(clauseBuilder.Object);
            Assert.Equal(builder, builder.AddRangeCondition(clauseName, start, null));
            Assert.Equal(builder, builder.AddRangeCondition(clauseName, null, end));
            Assert.Equal(builder, builder.AddRangeCondition(clauseName, start, end));

            clauseBuilder.Verify();
        }

        [Fact]
        public void TestEmpty()
        {
            var builder = new JqlClauseBuilder();
            Assert.Null(builder.BuildClause());
        }

        public class TheAddFunctionConditionMethod
        {
            [Fact]
            public void NoArgs()
            {
                var builder = new JqlClauseBuilder();
                try
                {
                    builder.AddFunctionCondition(null, "blah");
                    Assert.True(false, "Expected exception.");
                }
                catch (Exception)
                {
                }

                try
                {
                    builder.AddFunctionCondition("cool", null);
                    Assert.True(false, "Expected exception.");
                }
                catch (Exception)
                {
                }

                const string clauseName = "name";

                var clauseBuilder = new Mock<ISimpleClauseBuilder>();
                clauseBuilder.Setup(x => x.Clause(new TerminalClause(clauseName, Operator.EQUALS, new FunctionOperand("func")))).Returns(clauseBuilder.Object);

                builder = CreateBuilder(clauseBuilder.Object);
                Assert.Equal(builder, builder.AddFunctionCondition(clauseName, "func"));

                clauseBuilder.Verify();
            }

            [Fact]
            public void VarArgs()
            {
                var builder = new JqlClauseBuilder();
                try
                {
                    builder.AddFunctionCondition(null, "blah", "what");
                    Assert.True(false, "Expected exception.");
                }
                catch (Exception)
                {
                }

                try
                {
                    builder.AddFunctionCondition("cool", null, "what");
                    Assert.True(false, "Expected exception.");
                }
                catch (Exception)
                {
                }

                try
                {
                    builder.AddFunctionCondition("cool", "blah", (string[])null);
                    Assert.True(false, "Expected exception.");
                }
                catch (Exception)
                {
                }

                try
                {
                    builder.AddFunctionCondition("cool", "blah", null, "contains null");
                    Assert.True(false, "Expected exception.");
                }
                catch (Exception)
                {
                }

                const string clauseName = "name";

                var clauseBuilder = new Mock<ISimpleClauseBuilder>();
                clauseBuilder.Setup(x => x.Clause(new TerminalClause(clauseName, Operator.EQUALS, new FunctionOperand("func", "arg1", "arg2")))).Returns(clauseBuilder.Object);

                builder = CreateBuilder(clauseBuilder.Object);
                Assert.Equal(builder, builder.AddFunctionCondition(clauseName, "func", "arg1", "arg2"));

                clauseBuilder.Verify();
            }

            [Fact]
            public void Collection()
            {
                var builder = new JqlClauseBuilder();
                try
                {
                    builder.AddFunctionCondition(null, "blah", "what");
                    Assert.True(false, "Expected exception.");
                }
                catch (Exception)
                {
                }

                try
                {
                    builder.AddFunctionCondition("cool", null, "what");
                    Assert.True(false, "Expected exception.");
                }
                catch (Exception)
                {
                }

                try
                {
                    builder.AddFunctionCondition("cool", "blah", (List<string>)null);
                    Assert.True(false, "Expected exception.");
                }
                catch (Exception)
                {
                }

                try
                {
                    builder.AddFunctionCondition("cool", "blah", new List<string> { null });
                    Assert.True(false, "Expected exception.");
                }
                catch (Exception)
                {
                }

                const string clauseName = "name";

                var args = new List<string> { "arg1", "arg2", "arg3" };
                var clauseBuilder = new Mock<ISimpleClauseBuilder>();
                clauseBuilder.Setup(x => x.Clause(new TerminalClause(clauseName, Operator.EQUALS, new FunctionOperand("func", args)))).Returns(clauseBuilder.Object);

                builder = CreateBuilder(clauseBuilder.Object);
                Assert.Equal(builder, builder.AddFunctionCondition(clauseName, "func", args));

                clauseBuilder.Verify();
            }

            [Fact]
            public void WithOperatorNoArgs()
            {
                var builder = new JqlClauseBuilder();
                try
                {
                    builder.AddFunctionCondition(null, Operator.EQUALS, "blah");
                    Assert.True(false, "Expected exception.");
                }
                catch (Exception)
                {
                }

                try
                {
                    builder.AddFunctionCondition("cool", Operator.EQUALS, null);
                    Assert.True(false, "Expected exception.");
                }
                catch (Exception)
                {
                }

                try
                {
                    builder.AddFunctionCondition("cool", null, "func");
                    Assert.True(false, "Expected exception.");
                }
                catch (Exception)
                {
                }

                const string clauseName = "name";

                var clauseBuilder = new Mock<ISimpleClauseBuilder>();
                clauseBuilder.Setup(x => x.Clause(new TerminalClause(clauseName, Operator.LESS_THAN, new FunctionOperand("func")))).Returns(clauseBuilder.Object);

                builder = CreateBuilder(clauseBuilder.Object);
                Assert.Equal(builder, builder.AddFunctionCondition(clauseName, Operator.LESS_THAN, "func"));

                clauseBuilder.Verify();
            }

            [Fact]
            public void WithOperatorVarArgs()
            {
                var builder = new JqlClauseBuilder();
                try
                {
                    builder.AddFunctionCondition(null, Operator.EQUALS, "blah", "what");
                    Assert.True(false, "Expected exception.");
                }
                catch (Exception)
                {
                }

                try
                {
                    builder.AddFunctionCondition("cool", Operator.EQUALS, null, "what");
                    Assert.True(false, "Expected exception.");
                }
                catch (Exception)
                {
                }

                try
                {
                    builder.AddFunctionCondition("cool", Operator.EQUALS, "blah", (string[])null);
                    Assert.True(false, "Expected exception.");
                }
                catch (Exception)
                {
                }

                try
                {
                    builder.AddFunctionCondition("cool", Operator.EQUALS, "blah", null, "contains null");
                    Assert.True(false, "Expected exception.");
                }
                catch (Exception)
                {
                }

                try
                {
                    builder.AddFunctionCondition("cool", null, "blah", "aaa");
                    Assert.True(false, "Expected exception.");
                }
                catch (Exception)
                {
                }

                const string clauseName = "name";

                var clauseBuilder = new Mock<ISimpleClauseBuilder>();
                clauseBuilder.Setup(x => x.Clause(new TerminalClause(clauseName, Operator.GREATER_THAN, new FunctionOperand("func", "arg1", "arg2")))).Returns(clauseBuilder.Object);

                builder = CreateBuilder(clauseBuilder.Object);
                Assert.Equal(builder, builder.AddFunctionCondition(clauseName, Operator.GREATER_THAN, "func", "arg1", "arg2"));

                clauseBuilder.Verify();
            }

            [Fact]
            public void WithOperatorCollection()
            {
                var builder = new JqlClauseBuilder();
                try
                {
                    builder.AddFunctionCondition(null, Operator.NOT_EQUALS, "blah", "what");
                    Assert.True(false, "Expected exception.");
                }
                catch (Exception)
                {
                }

                try
                {
                    builder.AddFunctionCondition("cool", Operator.NOT_EQUALS, null, "what");
                    Assert.True(false, "Expected exception.");
                }
                catch (Exception)
                {
                }

                try
                {
                    builder.AddFunctionCondition("cool", Operator.NOT_EQUALS, "blah", (List<string>)null);
                    Assert.True(false, "Expected exception.");
                }
                catch (Exception)
                {
                }

                try
                {
                    builder.AddFunctionCondition("cool", Operator.NOT_EQUALS, "blah", new List<string> { "sup" });
                    Assert.True(false, "Expected exception.");
                }
                catch (Exception)
                {
                }

                const string clauseName = "name";

                var args = new List<string> { "arg1", "arg2", "arg3" };
                var clauseBuilder = new Mock<ISimpleClauseBuilder>();
                clauseBuilder.Setup(x => x.Clause(new TerminalClause(clauseName, Operator.NOT_EQUALS, new FunctionOperand("func", args)))).Returns(clauseBuilder.Object);

                builder = CreateBuilder(clauseBuilder.Object);
                Assert.Equal(builder, builder.AddFunctionCondition(clauseName, Operator.NOT_EQUALS, "func", args));

                clauseBuilder.Verify();
            }
        }

        public class TheAddStringConditionMethod
        {
            [Fact]
            public void Single()
            {
                var builder = new JqlClauseBuilder();
                try
                {
                    builder.AddStringCondition(null, "blah");
                    Assert.True(false, "Expected exception.");
                }
                catch (Exception)
                {
                }

                try
                {
                    builder.AddStringCondition("cool", (string)null);
                    Assert.True(false, "Expected exception.");
                }
                catch (Exception)
                {
                }

                const string clauseName = "name";

                var clauseBuilder = new Mock<ISimpleClauseBuilder>();
                clauseBuilder.Setup(x => x.Clause(new TerminalClause(clauseName, Operator.EQUALS, "value"))).Returns(clauseBuilder.Object);

                builder = CreateBuilder(clauseBuilder.Object);
                Assert.Equal(builder, builder.AddStringCondition(clauseName, "value"));

                clauseBuilder.Verify();
            }

            [Fact]
            public void VarArgs()
            {
                var builder = new JqlClauseBuilder();
                try
                {
                    builder.AddStringCondition(null, "blah", "blah2");
                    Assert.True(false, "Expected exception.");
                }
                catch (Exception)
                {
                }

                try
                {
                    builder.AddStringCondition("cool", (string)null);
                    Assert.True(false, "Expected exception.");
                }
                catch (Exception)
                {
                }

                try
                {
                    builder.AddStringCondition("cool", "me", null);
                    Assert.True(false, "Expected exception.");
                }
                catch (Exception)
                {
                }

                const string clauseName = "name";

                var clauseBuilder = new Mock<ISimpleClauseBuilder>();
                clauseBuilder.Setup(x => x.Clause(new TerminalClause(clauseName, Operator.EQUALS, "value"))).Returns(clauseBuilder.Object);
                clauseBuilder.Setup(x => x.Clause(new TerminalClause(clauseName, Operator.IN, new MultiValueOperand("value", "value2")))).Returns(clauseBuilder.Object);

                builder = CreateBuilder(clauseBuilder.Object);
                Assert.Equal(builder, builder.AddStringCondition(clauseName, new[] { "value" }));
                Assert.Equal(builder, builder.AddStringCondition(clauseName, "value", "value2"));

                clauseBuilder.Verify();
            }

            [Fact]
            public void Collection()
            {
                var builder = new JqlClauseBuilder();
                try
                {
                    builder.AddStringCondition(null, new List<string>() { "blah", "blah2" });
                    Assert.True(false, "Expected exception.");
                }
                catch (Exception)
                {
                }

                try
                {
                    builder.AddStringCondition("cool", (List<string>)null);
                    Assert.True(false, "Expected exception.");
                }
                catch (Exception)
                {
                }

                try
                {
                    builder.AddStringCondition("cool", new List<string>() { "blah", null });
                    Assert.True(false, "Expected exception.");
                }
                catch (Exception)
                {
                }

                const string clauseName = "name";

                var clauseBuilder = new Mock<ISimpleClauseBuilder>();
                clauseBuilder.Setup(x => x.Clause(new TerminalClause(clauseName, Operator.EQUALS, "value"))).Returns(clauseBuilder.Object);
                clauseBuilder.Setup(x => x.Clause(new TerminalClause(clauseName, Operator.IN, new MultiValueOperand("value", "value2")))).Returns(clauseBuilder.Object);

                builder = CreateBuilder(clauseBuilder.Object);
                Assert.Equal(builder, builder.AddStringCondition(clauseName, new[] { "value" }));
                Assert.Equal(builder, builder.AddStringCondition(clauseName, "value", "value2"));

                clauseBuilder.Verify();
            }

            [Fact]
            public void SingleOperator()
            {
                var builder = new JqlClauseBuilder();
                try
                {
                    builder.AddStringCondition(null, Operator.IN, "blah");
                    Assert.True(false, "Expected exception.");
                }
                catch (Exception)
                {
                }

                try
                {
                    builder.AddStringCondition("cool", Operator.IS_NOT, (string)null);
                    Assert.True(false, "Expected exception.");
                }
                catch (Exception)
                {
                }

                try
                {
                    builder.AddStringCondition("cool", null, "ejklwr");
                    Assert.True(false, "Expected exception.");
                }
                catch (Exception)
                {
                }

                const string clauseName = "name";

                var clauseBuilder = new Mock<ISimpleClauseBuilder>();
                clauseBuilder.Setup(x => x.Clause(new TerminalClause(clauseName, Operator.LIKE, "value"))).Returns(clauseBuilder.Object);

                builder = CreateBuilder(clauseBuilder.Object);
                Assert.Equal(builder, builder.AddStringCondition(clauseName, Operator.LIKE, "value"));

                clauseBuilder.Verify();
            }

            [Fact]
            public void VarArgsOperator()
            {
                var builder = new JqlClauseBuilder();
                try
                {
                    builder.AddStringCondition(null, Operator.LIKE, "blah", "blah2");
                    Assert.True(false, "Expected exception.");
                }
                catch (Exception)
                {
                }

                try
                {
                    builder.AddStringCondition("cool", Operator.IS_NOT, (string)null);
                    Assert.True(false, "Expected exception.");
                }
                catch (Exception)
                {
                }

                try
                {
                    builder.AddStringCondition("cool", Operator.LIKE, "me", null);
                    Assert.True(false, "Expected exception.");
                }
                catch (Exception)
                {
                }

                try
                {
                    builder.AddStringCondition("cool", null, "me", "two");
                    Assert.True(false, "Expected exception.");
                }
                catch (Exception)
                {
                }

                const string clauseName = "name";

                var clauseBuilder = new Mock<ISimpleClauseBuilder>();
                clauseBuilder.Setup(x => x.Clause(new TerminalClause(clauseName, Operator.LIKE, new MultiValueOperand("value")))).Returns(clauseBuilder.Object);
                clauseBuilder.Setup(x => x.Clause(new TerminalClause(clauseName, Operator.NOT_IN, new MultiValueOperand("value", "value2")))).Returns(clauseBuilder.Object);

                builder = CreateBuilder(clauseBuilder.Object);
                Assert.Equal(builder, builder.AddStringCondition(clauseName, Operator.LIKE, new[] { "value" }));
                Assert.Equal(builder, builder.AddStringCondition(clauseName, Operator.NOT_IN, "value", "value2"));

                clauseBuilder.Verify();
            }

            [Fact]
            public void CollectionOperator()
            {
                var builder = new JqlClauseBuilder();
                try
                {
                    builder.AddStringCondition(null, Operator.GREATER_THAN, new List<string>() { "blah", "blah2" });
                    Assert.True(false, "Expected exception.");
                }
                catch (Exception)
                {
                }

                try
                {
                    builder.AddStringCondition("cool", Operator.LESS_THAN, (List<string>)null);
                    Assert.True(false, "Expected exception.");
                }
                catch (Exception)
                {
                }

                try
                {
                    builder.AddStringCondition("cool", Operator.LESS_THAN, new List<string>() { "blah", null });
                    Assert.True(false, "Expected exception.");
                }
                catch (Exception)
                {
                }

                const string clauseName = "name";

                var clauseBuilder = new Mock<ISimpleClauseBuilder>();
                clauseBuilder.Setup(x => x.Clause(new TerminalClause(clauseName, Operator.GREATER_THAN_EQUALS, new MultiValueOperand("value")))).Returns(clauseBuilder.Object);
                clauseBuilder.Setup(x => x.Clause(new TerminalClause(clauseName, Operator.LESS_THAN_EQUALS, new MultiValueOperand("value", "value2")))).Returns(clauseBuilder.Object);

                builder = CreateBuilder(clauseBuilder.Object);
                Assert.Equal(builder, builder.AddStringCondition(clauseName, Operator.GREATER_THAN_EQUALS, new List<string> { "value" }));
                Assert.Equal(builder, builder.AddStringCondition(clauseName, Operator.LESS_THAN_EQUALS, new List<string> { "value", "value2" }));

                clauseBuilder.Verify();
            }
        }

        public class TheAddNumberConditionMethod
        {
            [Fact]
            public void Single()
            {
                var builder = new JqlClauseBuilder();
                try
                {
                    builder.AddNumberCondition(null, 5);
                    Assert.True(false, "Expected exception.");
                }
                catch (Exception)
                {
                }

                try
                {
                    builder.AddNumberCondition("DHSJKKD", (int?) null);
                    Assert.True(false, "Expected exception.");
                }
                catch (Exception)
                {
                }

                const string clauseName = "name";

                var clauseBuilder = new Mock<ISimpleClauseBuilder>();
                clauseBuilder.Setup(x => x.Clause(new TerminalClause(clauseName, Operator.EQUALS, 5))).Returns(clauseBuilder.Object);

                builder = CreateBuilder(clauseBuilder.Object);
                Assert.Equal(builder, builder.AddNumberCondition(clauseName, 5));

                clauseBuilder.Verify();
            }

            [Fact]
            public void VarArgs()
            {
                var builder = new JqlClauseBuilder();
                try
                {
                    builder.AddNumberCondition(null, 5, 6);
                    Assert.True(false, "Expected exception.");
                }
                catch (Exception)
                {
                }

                try
                {
                    builder.AddNumberCondition("cool", (int?)null);
                    Assert.True(false, "Expected exception.");
                }
                catch (Exception)
                {
                }

                try
                {
                    builder.AddNumberCondition(null, 6, null, 56383);
                    Assert.True(false, "Expected exception.");
                }
                catch (Exception)
                {
                }

                const string clauseName = "name";

                var clauseBuilder = new Mock<ISimpleClauseBuilder>();
                clauseBuilder.Setup(x => x.Clause(new TerminalClause(clauseName, Operator.EQUALS, new SingleValueOperand(6)))).Returns(clauseBuilder.Object);
                clauseBuilder.Setup(x => x.Clause(new TerminalClause(clauseName, Operator.IN, new MultiValueOperand(6, 8)))).Returns(clauseBuilder.Object);

                builder = CreateBuilder(clauseBuilder.Object);
                Assert.Equal(builder, builder.AddNumberCondition(clauseName, new int?[] { 6 }));
                Assert.Equal(builder, builder.AddNumberCondition(clauseName, 6, 8));

                clauseBuilder.Verify();
            }

            [Fact]
            public void Collection()
            {
                var builder = new JqlClauseBuilder();
                try
                {
                    builder.AddNumberCondition(null, new List<int?> {51, 61});
                    Assert.True(false, "Expected exception.");
                }
                catch (Exception)
                {
                }

                try
                {
                    builder.AddNumberCondition("cool", (List<int?>)null);
                    Assert.True(false, "Expected exception.");
                }
                catch (Exception)
                {
                }

                try
                {
                    builder.AddNumberCondition(null, new List<int?> { 5, null });
                    Assert.True(false, "Expected exception.");
                }
                catch (Exception)
                {
                }

                const string clauseName = "name";

                var clauseBuilder = new Mock<ISimpleClauseBuilder>();
                clauseBuilder.Setup(x => x.Clause(new TerminalClause(clauseName, Operator.EQUALS, new SingleValueOperand(5)))).Returns(clauseBuilder.Object);
                clauseBuilder.Setup(x => x.Clause(new TerminalClause(clauseName, Operator.IN, new MultiValueOperand(6, 5747)))).Returns(clauseBuilder.Object);

                builder = CreateBuilder(clauseBuilder.Object);
                Assert.Equal(builder, builder.AddNumberCondition(clauseName, new List<int?> { 5 }));
                Assert.Equal(builder, builder.AddNumberCondition(clauseName, new List<int?> { 6, 5747 }));

                clauseBuilder.Verify();
            }

            [Fact]
            public void SingleOperator()
            {
                var builder = new JqlClauseBuilder();
                try
                {
                    builder.AddNumberCondition(null, Operator.IN, 6);
                    Assert.True(false, "Expected exception.");
                }
                catch (Exception)
                {
                }

                try
                {
                    builder.AddNumberCondition("cool", null, 5);
                    Assert.True(false, "Expected exception.");
                }
                catch (Exception)
                {
                }

                try
                {
                    builder.AddNumberCondition(null, Operator.LESS_THAN, (int?) null);
                    Assert.True(false, "Expected exception.");
                }
                catch (Exception)
                {
                }

                const string clauseName = "name";

                var clauseBuilder = new Mock<ISimpleClauseBuilder>();
                clauseBuilder.Setup(x => x.Clause(new TerminalClause(clauseName, Operator.LIKE, 6))).Returns(clauseBuilder.Object);

                builder = CreateBuilder(clauseBuilder.Object);
                Assert.Equal(builder, builder.AddNumberCondition(clauseName, Operator.LIKE, 6));

                clauseBuilder.Verify();
            }

            [Fact]
            public void MultipleOperator()
            {
                var builder = new JqlClauseBuilder();
                try
                {
                    builder.AddNumberCondition(null, Operator.IN, 5, 8);
                    Assert.True(false, "Expected exception.");
                }
                catch (Exception)
                {
                }

                try
                {
                    builder.AddNumberCondition("cool", Operator.IS_NOT, (int?) null);
                    Assert.True(false, "Expected exception.");
                }
                catch (Exception)
                {
                }

                try
                {
                    builder.AddNumberCondition(null, Operator.IS_NOT, 5, null, 47373);
                    Assert.True(false, "Expected exception.");
                }
                catch (Exception)
                {
                }

                const string clauseName = "name";

                var clauseBuilder = new Mock<ISimpleClauseBuilder>();
                clauseBuilder.Setup(x => x.Clause(new TerminalClause(clauseName, Operator.LIKE, new MultiValueOperand(7)))).Returns(clauseBuilder.Object);
                clauseBuilder.Setup(x => x.Clause(new TerminalClause(clauseName, Operator.NOT_IN, new MultiValueOperand(6, 12)))).Returns(clauseBuilder.Object);

                builder = CreateBuilder(clauseBuilder.Object);
                Assert.Equal(builder, builder.AddNumberCondition(clauseName, Operator.LIKE, new int?[] {7}));
                Assert.Equal(builder, builder.AddNumberCondition(clauseName, Operator.NOT_IN, 6, 12));

                clauseBuilder.Verify();
            }

            [Fact]
            public void CollectionOperator()
            {
                var builder = new JqlClauseBuilder();
                try
                {
                    builder.AddNumberCondition(null, Operator.GREATER_THAN, new List<int?> { 651, 34231 });
                    Assert.True(false, "Expected exception.");
                }
                catch (Exception)
                {
                }

                try
                {
                    builder.AddNumberCondition("cool", Operator.LESS_THAN, (List<int?>)null);
                    Assert.True(false, "Expected exception.");
                }
                catch (Exception)
                {
                }

                try
                {
                    builder.AddNumberCondition(null, Operator.LESS_THAN, new List<int?> { 61, null });
                    Assert.True(false, "Expected exception.");
                }
                catch (Exception)
                {
                }

                const string clauseName = "name";

                var clauseBuilder = new Mock<ISimpleClauseBuilder>();
                clauseBuilder.Setup(x => x.Clause(new TerminalClause(clauseName, Operator.GREATER_THAN_EQUALS, new MultiValueOperand(5)))).Returns(clauseBuilder.Object);
                clauseBuilder.Setup(x => x.Clause(new TerminalClause(clauseName, Operator.LESS_THAN_EQUALS, new MultiValueOperand(67, 654)))).Returns(clauseBuilder.Object);

                builder = CreateBuilder(clauseBuilder.Object);
                Assert.Equal(builder, builder.AddNumberCondition(clauseName, Operator.GREATER_THAN_EQUALS, new List<int?> {5}));
                Assert.Equal(builder, builder.AddNumberCondition(clauseName, Operator.LESS_THAN_EQUALS, new List<int?> {67, 654}));

                clauseBuilder.Verify();
            }
        }

        [Fact]
        public void TestConditionBuilder()
        {
            var clauseBuilder = new Mock<ISimpleClauseBuilder>();
            clauseBuilder.Setup(x => x.Clause(new TerminalClause("meh", Operator.EQUALS, 16))).Returns(clauseBuilder.Object);

            var builder = CreateBuilder(clauseBuilder.Object);
            Assert.Equal(builder, builder.AddNumberCondition("meh", Operator.EQUALS, 16));

            clauseBuilder.Verify();
        }

        public class TheAddConditionMethod
        {
            [Fact]
            public void Single()
            {
                var builder = new JqlClauseBuilder();
                try
                {
                    builder.AddCondition(null, new SingleValueOperand("5"));
                    Assert.True(false, "Expected exception.");
                }
                catch (Exception)
                {
                }

                const string clauseName = "name";

                var clauseBuilder = new Mock<ISimpleClauseBuilder>();
                clauseBuilder.Setup(x => x.Clause(new TerminalClause(clauseName, Operator.EQUALS, new SingleValueOperand(5)))).Returns(clauseBuilder.Object);
                clauseBuilder.Setup(x => x.Clause(new TerminalClause(clauseName, Operator.LESS_THAN_EQUALS, new MultiValueOperand(67, 654)))).Returns(clauseBuilder.Object);

                builder = CreateBuilder(clauseBuilder.Object);
                Assert.Equal(builder, builder.AddCondition(clauseName, new SingleValueOperand(5)));

                clauseBuilder.Verify();
            }

            [Fact]
            public void VarArgs()
            {
                
            }

            [Fact]
            public void Collection()
            {
                
            }

            [Fact]
            public void SingleOperator()
            {
                
            }

            [Fact]
            public void MultipleOperator()
            {
                
            }

            [Fact]
            public void CollectionOperator()
            {
                
            }
        }

        [Fact]
        public void TestBuildClause()
        {
            var expectedReturn = new TerminalClause("check", Operator.GREATER_THAN_EQUALS, 5);

            var clauseBuilder = new Mock<ISimpleClauseBuilder>();
            clauseBuilder.Setup(x => x.Build()).Returns(expectedReturn);

            var builder = CreateBuilder(clauseBuilder.Object);
            Assert.Equal(expectedReturn, builder.BuildClause());

            clauseBuilder.Verify();
        }

        [Fact]
        public void TestDefaultAnd()
        {
            var clauseBuilder = new Mock<ISimpleClauseBuilder>();
            var clauseBuilder2 = new Mock<ISimpleClauseBuilder>();

            clauseBuilder.Setup(x => x.DefaultAnd()).Returns(clauseBuilder2.Object);
            clauseBuilder2.Setup(x => x.DefaultAnd()).Returns(clauseBuilder.Object);

            var builder = CreateBuilder(clauseBuilder.Object);
            Assert.Equal(builder, builder.DefaultAnd());
            Assert.Equal(builder, builder.DefaultAnd());

            clauseBuilder.Verify();
            clauseBuilder2.Verify();
        }

        [Fact]
        public void TestAnd()
        {
            var clauseBuilder = new Mock<ISimpleClauseBuilder>();
            var clauseBuilder2 = new Mock<ISimpleClauseBuilder>();

            clauseBuilder.Setup(x => x.And()).Returns(clauseBuilder2.Object);
            clauseBuilder2.Setup(x => x.And()).Returns(clauseBuilder.Object);

            var builder = CreateBuilder(clauseBuilder.Object);
            Assert.Equal(builder, builder.And());
            Assert.Equal(builder, builder.And());

            clauseBuilder.Verify();
            clauseBuilder2.Verify();
        }

        [Fact]
        public void TestDefaultOr()
        {
            var clauseBuilder = new Mock<ISimpleClauseBuilder>();
            var clauseBuilder2 = new Mock<ISimpleClauseBuilder>();

            clauseBuilder.Setup(x => x.DefaultOr()).Returns(clauseBuilder2.Object);
            clauseBuilder2.Setup(x => x.DefaultOr()).Returns(clauseBuilder.Object);

            var builder = CreateBuilder(clauseBuilder.Object);
            Assert.Equal(builder, builder.DefaultOr());
            Assert.Equal(builder, builder.DefaultOr());

            clauseBuilder.Verify();
            clauseBuilder2.Verify();
        }

        [Fact]
        public void TestOr()
        {
            var clauseBuilder = new Mock<ISimpleClauseBuilder>();
            var clauseBuilder2 = new Mock<ISimpleClauseBuilder>();

            clauseBuilder.Setup(x => x.Or()).Returns(clauseBuilder2.Object);
            clauseBuilder2.Setup(x => x.Or()).Returns(clauseBuilder.Object);

            var builder = CreateBuilder(clauseBuilder.Object);
            Assert.Equal(builder, builder.Or());
            Assert.Equal(builder, builder.Or());

            clauseBuilder.Verify();
            clauseBuilder2.Verify();
        }

        [Fact]
        public void TestDefaultNone()
        {
            var clauseBuilder = new Mock<ISimpleClauseBuilder>();
            var clauseBuilder2 = new Mock<ISimpleClauseBuilder>();

            clauseBuilder.Setup(x => x.DefaultNone()).Returns(clauseBuilder2.Object);
            clauseBuilder2.Setup(x => x.DefaultNone()).Returns(clauseBuilder.Object);

            var builder = CreateBuilder(clauseBuilder.Object);
            Assert.Equal(builder, builder.DefaultNone());
            Assert.Equal(builder, builder.DefaultNone());

            clauseBuilder.Verify();
            clauseBuilder2.Verify();
        }

        [Fact]
        public void TestNot()
        {
            var clauseBuilder = new Mock<ISimpleClauseBuilder>();
            var clauseBuilder2 = new Mock<ISimpleClauseBuilder>();

            clauseBuilder.Setup(x => x.Not()).Returns(clauseBuilder2.Object);
            clauseBuilder2.Setup(x => x.Not()).Returns(clauseBuilder.Object);

            var builder = CreateBuilder(clauseBuilder.Object);
            Assert.Equal(builder, builder.Not());
            Assert.Equal(builder, builder.Not());

            clauseBuilder.Verify();
            clauseBuilder2.Verify();
        }

        [Fact]
        public void TestSub()
        {
            var clauseBuilder = new Mock<ISimpleClauseBuilder>();
            var clauseBuilder2 = new Mock<ISimpleClauseBuilder>();

            clauseBuilder.Setup(x => x.Sub()).Returns(clauseBuilder2.Object);
            clauseBuilder2.Setup(x => x.Sub()).Returns(clauseBuilder.Object);

            var builder = CreateBuilder(clauseBuilder.Object);
            Assert.Equal(builder, builder.Sub());
            Assert.Equal(builder, builder.Sub());

            clauseBuilder.Verify();
            clauseBuilder2.Verify();
        }

        [Fact]
        public void TestEndSub()
        {
            var clauseBuilder = new Mock<ISimpleClauseBuilder>();
            var clauseBuilder2 = new Mock<ISimpleClauseBuilder>();

            clauseBuilder.Setup(x => x.Endsub()).Returns(clauseBuilder2.Object);
            clauseBuilder2.Setup(x => x.Endsub()).Returns(clauseBuilder.Object);

            var builder = CreateBuilder(clauseBuilder.Object);
            Assert.Equal(builder, builder.Endsub());
            Assert.Equal(builder, builder.Endsub());

            clauseBuilder.Verify();
            clauseBuilder2.Verify();
        }

        [Fact]
        public void TestAddClause()
        {
            var clause1 = new TerminalClause("name", Operator.EQUALS, "value");
            var clause2 = new TerminalClause("bad", Operator.NOT_EQUALS, "egg");

            var clauseBuilder = new Mock<ISimpleClauseBuilder>();
            var clauseBuilder2 = new Mock<ISimpleClauseBuilder>();

            clauseBuilder.Setup(x => x.Clause(clause1)).Returns(clauseBuilder2.Object);
            clauseBuilder2.Setup(x => x.Clause(clause2)).Returns(clauseBuilder.Object);

            var builder = CreateBuilder(clauseBuilder.Object);
            Assert.Equal(builder, builder.AddClause(clause1));
            Assert.Equal(builder, builder.AddClause(clause2));

            clauseBuilder.Verify();
            clauseBuilder2.Verify();
        }

        private static JqlClauseBuilder CreateBuilder(ISimpleClauseBuilder clauseBuilder)
        {
            var dateSupport = new JqlDateSupport();
            return CreateBuilder(clauseBuilder, dateSupport);
        }

        private static JqlClauseBuilder CreateBuilder(ISimpleClauseBuilder clauseBuilder, JqlDateSupport dateSupport)
        {
            return new JqlClauseBuilder(null, clauseBuilder, dateSupport);
        }
    }
}