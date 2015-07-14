using System;
using System.Text.RegularExpressions;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Core.Jql.Builder;
using Xunit;

namespace QuoteFlow.Core.Tests.Jql.Builder
{
    public class PrecedenceSimpleClauseBuilderTests
    {
        [Fact]
        public void TestStartState()
        {
            var builder = new PrecedenceSimpleClauseBuilder();
            Assert.Null(builder.Build());
            Assert.Equal(String.Empty, builder.ToString());
            Assert.Equal(String.Empty, builder.Copy().ToString());

            builder.And();

            Assert.Null(builder.Build());
            Assert.Equal(String.Empty, builder.ToString());
            Assert.Equal(String.Empty, builder.Copy().ToString());

            builder.Or();

            Assert.Null(builder.Build());
            Assert.Equal(String.Empty, builder.ToString());
            Assert.Equal(String.Empty, builder.Copy().ToString());

            try
            {
                builder.Endsub();
                Assert.True(false, "Expected exception.");
            }
            catch (Exception ex)
            {
                // expected
            }

            var expectedClause = new TerminalClause("test", Operator.EQUALS, "pass");
            ISimpleClauseBuilder copy = builder.Copy();
            Assert.Equal(expectedClause, copy.Clause(expectedClause).Build());

            copy = builder.Copy();
            Assert.Null(copy.Build());
        }

        [Fact]
        public void TestNotState()
        {
            var builder = new PrecedenceSimpleClauseBuilder();
            builder.Not();
            Assert.Equal("NOT", builder.ToString());

            try
            {
                builder.And();
                Assert.True(false, "Expected exception");
            }
            catch (Exception ex)
            {
                // expected
            }

            try
            {
                builder.Or();
                Assert.True(false, "Expected exception");
            }
            catch (Exception ex)
            {
                // expected
            }

            try
            {
                builder.Build();
                Assert.True(false, "Expected exception");
            }
            catch (Exception ex)
            {
                // expected
            }

            try
            {
                builder.Endsub();
                Assert.True(false, "Expected exception");
            }
            catch (Exception ex)
            {
                // expected
            }

            builder.Not();
            Assert.Equal("NOT NOT", builder.ToString());

            try
            {
                builder.And();
                Assert.True(false, "Expected exception");
            }
            catch (Exception ex)
            {
                // expected
            }

            try
            {
                builder.Or();
                Assert.True(false, "Expected exception");
            }
            catch (Exception ex)
            {
                // expected
            }

            try
            {
                builder.Build();
                Assert.True(false, "Expected exception");
            }
            catch (Exception ex)
            {
                // expected
            }

            try
            {
                builder.Endsub();
                Assert.True(false, "Expected exception");
            }
            catch (Exception ex)
            {
                // expected
            }

            Assert.Equal("NOT NOT NOT", builder.Copy().Not().ToString());

            var expectedClause = new TerminalClause("test", Operator.EQUALS, "pass");
            var expectedClause2 = new TerminalClause("test2", Operator.EQUALS, "pass2");
            var copy = builder.Copy();

            copy.Clause(expectedClause);
            Assert.Equal(new NotClause(new NotClause(expectedClause)), copy.Build());

            builder.Sub().Clause(expectedClause).Or().Clause(expectedClause2).Endsub();
            Assert.Equal(new NotClause(new NotClause(new OrClause(expectedClause, expectedClause2))), builder.Build());
        }

        [Fact]
        public void TestSingleClause()
        {
            var expectedClause = new TerminalClause("test", Operator.EQUALS, "pass");
            var builder = new PrecedenceSimpleClauseBuilder();

            Assert.Same(builder, builder.Clause(expectedClause));
            Assert.Equal(expectedClause, builder.Build());
            Assert.Equal("{test = \"pass\"}", builder.ToString());

            try
            {
                builder.Not();
                Assert.True(false, "Expected exception");
            }
            catch (Exception ex)
            {
                // expected
            }

            try
            {
                builder.Clause(new TerminalClause("test2", Operator.IS, "ERROR"));
                Assert.True(false, "Expected exception");
            }
            catch (Exception ex)
            {
                // expected
            }

            try
            {
                builder.Endsub();
                Assert.True(false, "Expected exception");
            }
            catch (Exception ex)
            {
                // expected
            }

            Assert.Same(builder, builder.And());
        }

        [Fact]
        public void TestOperatorState()
        {
            var termClause1 = new TerminalClause("test1", Operator.EQUALS, "pass1");
            var termClause2 = new TerminalClause("test2", Operator.EQUALS, "pass2");
            var termClause3 = new TerminalClause("test3", Operator.EQUALS, "pass3");

            PrecedenceSimpleClauseBuilder builder = new PrecedenceSimpleClauseBuilder();

            Assert.Same(builder, builder.Clause(termClause1));

            try
            {
                builder.Not();
                Assert.True(false, "Expected exception.");
            }
            catch (InvalidOperationException e)
            {
                //expected.
            }

            try
            {
                builder.Sub();
                Assert.True(false, "Expected exception.");
            }
            catch (InvalidOperationException e)
            {
                //expected.
            }

            try
            {
                builder.Clause(termClause2);
                Assert.True(false, "Expected exception.");
            }
            catch (InvalidOperationException e)
            {
                //expected.
            }

            try
            {
                builder.Endsub();
                Assert.True(false, "Expected exception.");
            }
            catch (InvalidOperationException e)
            {
                //expected.
            }

            Assert.Equal(termClause1, builder.Build());
            Assert.Equal(new AndClause(termClause1, termClause2), builder.Copy().And().Clause(termClause2).Build());
            Assert.Equal(new OrClause(termClause1, termClause2), builder.Copy().Or().Clause(termClause2).Build());

            //We now have a sub-clause in the expected state.
            builder.And().Sub().Clause(termClause2);

            try
            {
                builder.Not();
                Assert.True(false, "Expected exception.");
            }
            catch (InvalidOperationException e)
            {
                //expected.
            }

            try
            {
                builder.Sub();
                Assert.True(false, "Expected exception.");
            }
            catch (InvalidOperationException e)
            {
                //expected.
            }

            try
            {
                builder.Clause(termClause2);
                Assert.True(false, "Expected exception.");
            }
            catch (InvalidOperationException e)
            {
                //expected.
            }

            try
            {
                builder.Build();
                Assert.True(false, "Expected exception.");
            }
            catch (InvalidOperationException e)
            {
                //expected.
            }

            builder.Endsub();

            Assert.Equal(new AndClause(termClause1, termClause2), builder.Build());
            Assert.Equal(new AndClause(termClause1, termClause2, termClause3), builder.And().Clause(termClause3).Build());
        }

        [Fact]
        public void TestClauseState()
        {
            var termClause1 = new TerminalClause("test1", Operator.EQUALS, "pass1");
            var termClause2 = new TerminalClause("test2", Operator.NOT_EQUALS, "pass2");
            var termClause3 = new TerminalClause("test3", Operator.NOT_IN, "pass3");
            var termClause4 = new TerminalClause("test4", Operator.GREATER_THAN_EQUALS, "pass4");

            PrecedenceSimpleClauseBuilder builder = new PrecedenceSimpleClauseBuilder();
            builder.Clause(termClause1).And();

            try
            {
                builder.And();
                Assert.True(false, "Expected exception.");
            }
            catch (InvalidOperationException e)
            {
                //expected.
            }

            try
            {
                builder.Or();
                Assert.True(false, "Expected exception.");
            }
            catch (InvalidOperationException e)
            {
                //expected.
            }

            try
            {
                builder.Endsub();
                Assert.True(false, "Expected exception.");
            }
            catch (InvalidOperationException e)
            {
                //expected.
            }

            try
            {
                builder.Build();
                Assert.True(false, "Expected exception.");
            }
            catch (InvalidOperationException e)
            {
                //expected.
            }


            Assert.Equal(new AndClause(termClause1, termClause2), builder.Copy().Clause(termClause2).Build());
            Assert.Equal(new AndClause(termClause1, new NotClause(termClause2)), builder.Copy().Not().Clause(termClause2).Build());
            Assert.Equal(new AndClause(termClause1, new OrClause(termClause3, termClause4)), builder.Sub().Clause(termClause3).Or().Clause(termClause4).Endsub().Build());
        }

        [Fact]
        public void TestStartGroupState()
        {
            var termClause2 = new TerminalClause("test2", Operator.NOT_EQUALS, "pass2");
            var termClause3 = new TerminalClause("test3", Operator.NOT_IN, "pass3");
            var termClause4 = new TerminalClause("test4", Operator.GREATER_THAN_EQUALS, "pass4");

            ISimpleClauseBuilder builder = (new PrecedenceSimpleClauseBuilder()).Sub();

            try
            {
                builder.And();
                Assert.True(false, "Expected exception.");
            }
            catch (InvalidOperationException e)
            {
                //expected.
            }

            try
            {
                builder.Or();
                Assert.True(false, "Expected exception.");
            }
            catch (InvalidOperationException e)
            {
                //expected.
            }

            try
            {
                builder.Endsub();
                Assert.True(false, "Expected exception.");
            }
            catch (InvalidOperationException e)
            {
                //expected.
            }

            try
            {
                builder.Build();
                Assert.True(false, "Expected exception.");
            }
            catch (InvalidOperationException e)
            {
                //expected.
            }


            Assert.Equal(termClause2, builder.Copy().Clause(termClause2).Endsub().Build());
            Assert.Equal(new NotClause(termClause2), builder.Copy().Not().Clause(termClause2).Endsub().Build());
            Assert.Equal(new OrClause(termClause3, termClause4), builder.Sub().Clause(termClause3).Or().Clause(termClause4).Endsub().Endsub().Build());
        }

        [Fact]
        public void TestOperatorPrecedence()
        {
            var termClause1 = new TerminalClause("test1", Operator.EQUALS, "pass1");
            var termClause2 = new TerminalClause("test2", Operator.EQUALS, "pass2");
            var termClause3 = new TerminalClause("test3", Operator.EQUALS, "pass3");
            var termClause4 = new TerminalClause("test4", Operator.EQUALS, "pass4");

            {
                PrecedenceSimpleClauseBuilder builder = new PrecedenceSimpleClauseBuilder();
                builder.Clause(termClause1).Or().Clause(termClause2).And().Clause(termClause3);

                var expectedClause = new OrClause(termClause1, new AndClause(termClause2, termClause3));
                Assert.Equal(expectedClause, builder.Build());

                builder.Or().Clause(termClause4);
                expectedClause = new OrClause(termClause1, new AndClause(termClause2, termClause3), termClause4);
                Assert.Equal(expectedClause, builder.Build());
            }

            {
                PrecedenceSimpleClauseBuilder builder = new PrecedenceSimpleClauseBuilder();
                builder.Not().Clause(termClause1).And().Clause(termClause2).Or().Clause(termClause3);

                var expectedClause = new OrClause(new AndClause(new NotClause(termClause1), termClause2), termClause3);
                Assert.Equal(expectedClause, builder.Build());
            }
        }

        [Fact]
        public void TestSubExpression()
        {
            var termClause1 = new TerminalClause("test1", Operator.EQUALS, "pass1");
            var termClause2 = new TerminalClause("test2", Operator.EQUALS, "pass2");
            var termClause3 = new TerminalClause("test3", Operator.EQUALS, "pass3");

            PrecedenceSimpleClauseBuilder rootBuilder = new PrecedenceSimpleClauseBuilder();
            ISimpleClauseBuilder subBuilder = rootBuilder.Sub();
            Assert.Same(subBuilder, rootBuilder);

            try
            {
                rootBuilder.And();
                Assert.True(false, "Expected and exception");
            }
            catch (InvalidOperationException e)
            {
                //expected.
            }

            try
            {
                rootBuilder.Or();
                Assert.True(false, "Expected and exception");
            }
            catch (InvalidOperationException e)
            {
                //expected.
            }

            try
            {
                rootBuilder.Endsub();
                Assert.True(false, "Expected and exception");
            }
            catch (InvalidOperationException e)
            {
                //expected.
            }

            try
            {
                rootBuilder.Build();
                Assert.True(false, "Expected and exception");
            }
            catch (InvalidOperationException e)
            {
                //expected.
            }

            Assert.Same(subBuilder, subBuilder.Clause(termClause1).Or().Clause(termClause2));

            ISimpleClauseBuilder copy = rootBuilder.Copy();
            Assert.Same(rootBuilder, rootBuilder.Endsub().And().Clause(termClause3));
            Assert.NotSame(rootBuilder, copy.Endsub().And().Clause(termClause3));

            try
            {
                rootBuilder.Sub();
                Assert.True(false, "Expected and exception");
            }
            catch (InvalidOperationException e)
            {
                //expected.
            }

            OrClause subClause = new OrClause(termClause1, termClause2);
            var expectedClause = new AndClause(subClause, termClause3);
            Assert.Equal(expectedClause, rootBuilder.Build());
            Assert.Equal(expectedClause, subBuilder.Build());

            //try and end a sub-expression when it is incomplete.
            try
            {
                rootBuilder.And().Sub().Sub().Clause(termClause1).And().Endsub();
                Assert.True(false, "Expected and exception.");
            }
            catch (InvalidOperationException e)
            {
                //expected.
            }

            //Lets make a copy of the builder and try to build it while a sub-expression remains incomplete.
            subBuilder = rootBuilder.Copy().Clause(termClause3).Endsub();
            try
            {
                subBuilder.Build();
                Assert.True(false, "Expected and exception.");
            }
            catch (InvalidOperationException e)
            {
                //expected.
            }
        }

        [Fact]
        public void TestCopy()
        {
            var termClause1 = new TerminalClause("test1", Operator.EQUALS, "pass1");
            var termClause2 = new TerminalClause("test2", Operator.EQUALS, "pass2");
            var termClause3 = new TerminalClause("test3", Operator.EQUALS, "pass3");

            PrecedenceSimpleClauseBuilder builder = new PrecedenceSimpleClauseBuilder();
            builder.Not().Clause(termClause1);

            ISimpleClauseBuilder copy = builder.Copy();
            copy.And().Clause(termClause2);

            var notClause = new NotClause(termClause1);
            Assert.Equal(notClause, builder.Build());
            Assert.Equal(new AndClause(notClause, termClause2), copy.Build());

            Assert.Equal(new OrClause(notClause, termClause3), builder.Or().Clause(termClause3).Build());
            Assert.Equal(new AndClause(notClause, termClause2), copy.Build());
        }

        [Fact]
        public void TestComplexCondition()
        {
            var termClause1 = new TerminalClause("test1", Operator.EQUALS, "pass1");
            var termClause2 = new TerminalClause("test2", Operator.EQUALS, "pass2");
            var termClause3 = new TerminalClause("test3", Operator.EQUALS, "pass3");
            var termClause4 = new TerminalClause("test4", Operator.EQUALS, "pass4");

            ISimpleClauseBuilder builder = new PrecedenceSimpleClauseBuilder();
            builder.Clause(termClause1);
            Assert.Equal(termClause1, builder.Build());

            var expectedClause = new AndClause(termClause1, termClause2);
            Assert.Equal(expectedClause, builder.And().Copy().Sub().Clause(termClause2).Endsub().Build());

            expectedClause = new AndClause(termClause1, new NotClause(new NotClause(new OrClause(termClause2, termClause3, new NotClause(termClause4)))));
            Assert.Equal(expectedClause, builder.Not().Not().Sub().Clause(termClause2).Or().Clause(termClause3).Or().Not().Clause(termClause4).Endsub().Build());
        }

        [Fact]
        public void TestToString()
        {
            var termClause1 = new TerminalClause("test1", Operator.EQUALS, "pass1");
            var termClause2 = new TerminalClause("test2", Operator.EQUALS, "pass2");
            var termClause3 = new TerminalClause("test3", Operator.EQUALS, "pass3");

            ISimpleClauseBuilder builder = new PrecedenceSimpleClauseBuilder();
            AssertEqualsNoSpaces("", builder.ToString());
            AssertEqualsNoSpaces("NOT", builder.Not().ToString());
            AssertEqualsNoSpaces("NOT (", builder.Sub().ToString());
            AssertEqualsNoSpaces("NOT (NOT", builder.Not().ToString());
            AssertEqualsNoSpaces("NOT (NOT (", builder.Sub().ToString());
            AssertEqualsNoSpaces("NOT (NOT ({test1 = \"pass1\"}", builder.Clause(termClause1).ToString());
            AssertEqualsNoSpaces("NOT (NOT ({test1 = \"pass1\"} OR", builder.Or().ToString());
            AssertEqualsNoSpaces("NOT (NOT ({test1 = \"pass1\"} OR NOT", builder.Not().ToString());
            AssertEqualsNoSpaces("NOT (NOT ({test1 = \"pass1\"} OR NOT {test2 = \"pass2\"}", builder.Clause(termClause2).ToString());
            AssertEqualsNoSpaces("NOT (NOT ({test1 = \"pass1\"} OR NOT {test2 = \"pass2\"})", builder.Endsub().ToString());
            AssertEqualsNoSpaces("NOT (NOT ({test1 = \"pass1\"} OR NOT {test2 = \"pass2\"}) AND", builder.And().ToString());
            AssertEqualsNoSpaces("NOT (NOT ({test1 = \"pass1\"} OR NOT {test2 = \"pass2\"}) AND {test3 = \"pass3\"}", builder.Clause(termClause3).ToString());
            AssertEqualsNoSpaces("NOT (NOT ({test1 = \"pass1\"} OR NOT {test2 = \"pass2\"}) AND {test3 = \"pass3\"})", builder.Endsub().ToString());

            builder = new PrecedenceSimpleClauseBuilder();
            AssertEqualsNoSpaces("", builder.ToString());
            AssertEqualsNoSpaces("NOT", builder.Not().ToString());
            AssertEqualsNoSpaces("NOT (", builder.Sub().ToString());
            AssertEqualsNoSpaces("NOT ({test1 = \"pass1\"}", builder.Clause(termClause1).ToString());
            AssertEqualsNoSpaces("NOT {test1 = \"pass1\"}", builder.Endsub().ToString());
        }

        [Fact]
        public void TestAndDefault()
        {
            var termClause1 = new TerminalClause("test1", Operator.EQUALS, "pass1");
            var termClause2 = new TerminalClause("test2", Operator.EQUALS, "pass2");
            var termClause3 = new TerminalClause("test3", Operator.EQUALS, "pass3");
            var termClause4 = new TerminalClause("test4", Operator.GREATER_THAN_EQUALS, "pass4");

            ISimpleClauseBuilder builder = new PrecedenceSimpleClauseBuilder();
            builder.Clause(termClause1);
            try
            {
                builder.Clause(termClause2);
                Assert.True(false, "Expecting an error");
            }
            catch (InvalidOperationException e)
            {
                //expected.
            }

            IClause expectedClause = new AndClause(termClause1, termClause2);
            Assert.Equal(expectedClause, builder.DefaultAnd().Clause(termClause2).Build());

            expectedClause = new OrClause(new AndClause(termClause1, termClause2), termClause3);
            Assert.Equal(expectedClause, builder.Or().Clause(termClause3).Build());

            try
            {
                builder.DefaultNone().Clause(termClause4);
                Assert.True(false, "Expecting an error");
            }
            catch (InvalidOperationException e)
            {
                //expected.
            }

            expectedClause = new OrClause(new AndClause(termClause1, termClause2), new AndClause(termClause3, termClause4));
            Assert.Equal(expectedClause, builder.And().Clause(termClause4).Build());

            builder = (new PrecedenceSimpleClauseBuilder()).DefaultAnd();
            builder.Clause(termClause1).Not().Clause(termClause2).Sub().Clause(termClause3).Clause(termClause4).Endsub();
            expectedClause = new AndClause(termClause1, new NotClause(termClause2), new AndClause(termClause3, termClause4));
            Assert.Equal(expectedClause, builder.Build());
        }

        [Fact]
        public void TestOrDefault()
        {
            var termClause1 = new TerminalClause("test1", Operator.EQUALS, "pass1");
            var termClause2 = new TerminalClause("test2", Operator.EQUALS, "pass2");
            var termClause3 = new TerminalClause("test3", Operator.EQUALS, "pass3");
            var termClause4 = new TerminalClause("test4", Operator.GREATER_THAN_EQUALS, "pass4");

            ISimpleClauseBuilder builder = new PrecedenceSimpleClauseBuilder();
            builder.Clause(termClause1);
            try
            {
                builder.Clause(termClause2);
                Assert.True(false, "Expecting an error");
            }
            catch (InvalidOperationException e)
            {
                //expected.
            }

            IClause expectedClause = new OrClause(termClause1, termClause2);
            Assert.Equal(expectedClause, builder.DefaultOr().Clause(termClause2).Build());

            expectedClause = new OrClause(termClause1, new AndClause(termClause2, termClause3));
            Assert.Equal(expectedClause, builder.And().Clause(termClause3).Build());
            try
            {
                builder.DefaultNone().Clause(termClause4);
                Assert.True(false, "Expecting an error");
            }
            catch (InvalidOperationException e)
            {
                //expected.
            }

            expectedClause = new OrClause(termClause1, new AndClause(termClause2, termClause3), termClause4);
            Assert.Equal(expectedClause, builder.Or().Clause(termClause4).Build());

            builder = (new PrecedenceSimpleClauseBuilder()).DefaultOr();
            builder.Clause(termClause1).Not().Clause(termClause2).Sub().Clause(termClause3).Clause(termClause4).Endsub();
            expectedClause = new OrClause(termClause1, new NotClause(termClause2), new OrClause(termClause3, termClause4));
            Assert.Equal(expectedClause, builder.Build());
        }

        [Fact]
        public void TestClear()
        {
            var termClause1 = new TerminalClause("test1", Operator.EQUALS, "pass1");
            var termClause2 = new TerminalClause("test2", Operator.EQUALS, "pass2");
            var termClause3 = new TerminalClause("test3", Operator.EQUALS, "pass3");

            ISimpleClauseBuilder builder = new PrecedenceSimpleClauseBuilder();
            builder.Clause(termClause1).DefaultAnd().Clause(termClause2).Clause(termClause3);

            builder.Clear();

            // has the builder been reset?
            Assert.Null(builder.Build());

            builder.Sub().Clause(termClause1).And().Not().Sub();
            builder.Clear();

            // has the builder been cleared.
            Assert.Null(builder.Build());

            builder.Clause(termClause1);
            Assert.Equal(termClause1, builder.Build());

            builder.Clear();
            // has the builder been cleared.
            Assert.Null(builder.Build());
        }

        private static void AssertEqualsNoSpaces(string expected, string actual)
        {
            if (NormalizeSpace(expected).Equals(NormalizeSpace(actual))) return;

            var msg = string.Format("Assertion failed.{0} Expected '{1}'.{0} Actual '{2}'.{0}", Environment.NewLine, expected, actual);
            Assert.True(false, msg);
        }

        private static string NormalizeSpace(string str)
        {
            return Regex.Replace(str, @"\s+", "");
        }
    }
}