using System;
using System.Collections.Generic;
using Moq;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Operand;
using QuoteFlow.Core.Jql.Builder;
using Xunit;

namespace QuoteFlow.Core.Tests.Jql.Builder
{
    public class ConditionBuilderTests
    {
        public class TheCtor
        {
            [Fact]
            public void BadCtor()
            {
                var mockJqlClauseBuilder = new Mock<IJqlClauseBuilder>();

                try
                {
                    new ConditionBuilder("blah", null);
                    Assert.True(false, "Expecting an exception.");
                }
                catch (ArgumentNullException ex)
                {
                    // expected
                }

                try
                {
                    new ConditionBuilder(null, mockJqlClauseBuilder.Object);
                    Assert.True(false, "Expecting an exception.");
                }
                catch (ArgumentNullException ex)
                {
                    // expected
                }

                mockJqlClauseBuilder.Verify();
            }
        }

        public class TheEqMethod
        {
            [Fact]
            public void TestString()
            {
                const string name = "fieldName";
                const string value = "mine";

                var mockJqlClauseBuilder = new Mock<IJqlClauseBuilder>();

                mockJqlClauseBuilder.Setup(x => x.AddStringCondition(name, Operator.EQUALS, value))
                    .Returns(mockJqlClauseBuilder.Object);

                var builder = new ConditionBuilder(name, mockJqlClauseBuilder.Object);
                Assert.Same(mockJqlClauseBuilder.Object, builder.Eq(value));

                mockJqlClauseBuilder.Verify();
            }

            [Fact]
            public void TestOperand()
            {
                const string name = "fieldName";
                IOperand value = new SingleValueOperand("4372738");

                var mockJqlClauseBuilder = new Mock<IJqlClauseBuilder>();

                mockJqlClauseBuilder.Setup(x => x.AddCondition(name, Operator.EQUALS, value))
                    .Returns(mockJqlClauseBuilder.Object);

                var builder = new ConditionBuilder(name, mockJqlClauseBuilder.Object);
                Assert.Same(mockJqlClauseBuilder.Object, builder.Eq(value));

                mockJqlClauseBuilder.Verify();
            }

            [Fact]
            public void TestInt()
            {
                const string name = "fieldName";
                const int value = 1;

                var mockJqlClauseBuilder = new Mock<IJqlClauseBuilder>();

                mockJqlClauseBuilder.Setup(x => x.AddNumberCondition(name, Operator.EQUALS, value))
                    .Returns(mockJqlClauseBuilder.Object);

                var builder = new ConditionBuilder(name, mockJqlClauseBuilder.Object);
                Assert.Same(mockJqlClauseBuilder.Object, builder.Eq(value));

                mockJqlClauseBuilder.Verify();
            }

            [Fact]
            public void TestBuilder()
            {
                const string name = "fieldName";
                const int value = 1;

                var mockJqlClauseBuilder = new Mock<IJqlClauseBuilder>();

                mockJqlClauseBuilder.Setup(x => x.AddNumberCondition(name, Operator.EQUALS, value))
                    .Returns(mockJqlClauseBuilder.Object);

                var builder = new ConditionBuilder(name, mockJqlClauseBuilder.Object);
                var valueBuilder = builder.Eq();
                Assert.NotNull(valueBuilder);
                Assert.Same(mockJqlClauseBuilder.Object, valueBuilder.Number(value));

                mockJqlClauseBuilder.Verify();
            }
        }

        public class TheEqEmptyMethod
        {
            [Fact]
            public void TestEmpty()
            {
                const string name = "fieldName";
                var mockJqlClauseBuilder = new Mock<IJqlClauseBuilder>();

                mockJqlClauseBuilder.Setup(x => x.AddCondition(name, Operator.EQUALS, EmptyOperand.Empty))
                    .Returns(mockJqlClauseBuilder.Object);

                var builder = new ConditionBuilder(name, mockJqlClauseBuilder.Object);
                Assert.Same(mockJqlClauseBuilder.Object, builder.EqEmpty());

                mockJqlClauseBuilder.Verify();
            }
        }

        public class TheLikeMethod
        {
            [Fact]
            public void TestString()
            {
                const string name = "fieldName";
                const string value = "mine";

                var mockJqlClauseBuilder = new Mock<IJqlClauseBuilder>();

                mockJqlClauseBuilder.Setup(x => x.AddStringCondition(name, Operator.LIKE, value))
                    .Returns(mockJqlClauseBuilder.Object);

                var builder = new ConditionBuilder(name, mockJqlClauseBuilder.Object);
                Assert.Same(mockJqlClauseBuilder.Object, builder.Like(value));

                mockJqlClauseBuilder.Verify();
            }

            [Fact]
            public void TestOperand()
            {
                const string name = "fieldName";
                IOperand value = new SingleValueOperand("4372738");

                var mockJqlClauseBuilder = new Mock<IJqlClauseBuilder>();

                mockJqlClauseBuilder.Setup(x => x.AddCondition(name, Operator.LIKE, value))
                    .Returns(mockJqlClauseBuilder.Object);

                var builder = new ConditionBuilder(name, mockJqlClauseBuilder.Object);
                Assert.Same(mockJqlClauseBuilder.Object, builder.Like(value));

                mockJqlClauseBuilder.Verify();
            }

            [Fact]
            public void TestInt()
            {
                const string name = "fieldName";
                const int value = 1;

                var mockJqlClauseBuilder = new Mock<IJqlClauseBuilder>();

                mockJqlClauseBuilder.Setup(x => x.AddNumberCondition(name, Operator.LIKE, value))
                    .Returns(mockJqlClauseBuilder.Object);

                var builder = new ConditionBuilder(name, mockJqlClauseBuilder.Object);
                Assert.Same(mockJqlClauseBuilder.Object, builder.Like(value));

                mockJqlClauseBuilder.Verify();
            }

            [Fact]
            public void TestBuilder()
            {
                const string name = "fieldName";
                const int value = 1;

                var mockJqlClauseBuilder = new Mock<IJqlClauseBuilder>();

                mockJqlClauseBuilder.Setup(x => x.AddNumberCondition(name, Operator.LIKE, value))
                    .Returns(mockJqlClauseBuilder.Object);

                var builder = new ConditionBuilder(name, mockJqlClauseBuilder.Object);
                var valueBuilder = builder.Like();
                Assert.NotNull(valueBuilder);
                Assert.Same(mockJqlClauseBuilder.Object, valueBuilder.Number(value));

                mockJqlClauseBuilder.Verify();
            }
        }

        public class TheNotLikeMethod
        {
            [Fact]
            public void TestOperand()
            {
                const string name = "fieldName";
                IOperand value = new SingleValueOperand("4372738");

                var mockJqlClauseBuilder = new Mock<IJqlClauseBuilder>();

                mockJqlClauseBuilder.Setup(x => x.AddCondition(name, Operator.NOT_LIKE, value))
                    .Returns(mockJqlClauseBuilder.Object);

                var builder = new ConditionBuilder(name, mockJqlClauseBuilder.Object);
                Assert.Same(mockJqlClauseBuilder.Object, builder.NotLike(value));

                mockJqlClauseBuilder.Verify();
            }

            [Fact]
            public void TestInt()
            {
                const string name = "fieldName";
                const int value = 1;

                var mockJqlClauseBuilder = new Mock<IJqlClauseBuilder>();

                mockJqlClauseBuilder.Setup(x => x.AddNumberCondition(name, Operator.NOT_LIKE, value))
                    .Returns(mockJqlClauseBuilder.Object);

                var builder = new ConditionBuilder(name, mockJqlClauseBuilder.Object);
                Assert.Same(mockJqlClauseBuilder.Object, builder.NotLike(value));

                mockJqlClauseBuilder.Verify();
            }

            [Fact]
            public void TestBuilder()
            {
                const string name = "fieldName";
                const int value = 1;

                var mockJqlClauseBuilder = new Mock<IJqlClauseBuilder>();

                mockJqlClauseBuilder.Setup(x => x.AddNumberCondition(name, Operator.NOT_LIKE, value))
                    .Returns(mockJqlClauseBuilder.Object);

                var builder = new ConditionBuilder(name, mockJqlClauseBuilder.Object);
                var valueBuilder = builder.NotLike();
                Assert.NotNull(valueBuilder);
                Assert.Same(mockJqlClauseBuilder.Object, valueBuilder.Number(value));

                mockJqlClauseBuilder.Verify();
            }
        }

        public class TheEmptyProperty
        {
            [Fact]
            public void TestIsEmpty()
            {
                const string name = "fieldName";

                var mockJqlClauseBuilder = new Mock<IJqlClauseBuilder>();
                mockJqlClauseBuilder.Setup(x => x.AddCondition(name, Operator.IS, EmptyOperand.Empty))
                    .Returns(mockJqlClauseBuilder.Object);

                var builder = new ConditionBuilder(name, mockJqlClauseBuilder.Object);
                Assert.Same(mockJqlClauseBuilder.Object, builder.Empty);

                mockJqlClauseBuilder.Verify();
            }
        }

        public class TheIsMethod
        {
            [Fact]
            public void TestBuilder()
            {
                const string name = "fieldName";

                var mockJqlClauseBuilder = new Mock<IJqlClauseBuilder>();
                mockJqlClauseBuilder.Setup(x => x.AddCondition(name, Operator.IS, EmptyOperand.Empty))
                    .Returns(mockJqlClauseBuilder.Object);

                var builder = new ConditionBuilder(name, mockJqlClauseBuilder.Object);
                var valueBuilder = builder.Is();
                Assert.NotNull(valueBuilder);
                Assert.Same(mockJqlClauseBuilder.Object, valueBuilder.Empty());

                mockJqlClauseBuilder.Verify();
            }
        }

        public class TheIsNotMethod
        {
            [Fact]
            public void Empty()
            {
                const string name = "fieldName";

                var mockJqlClauseBuilder = new Mock<IJqlClauseBuilder>();
                mockJqlClauseBuilder.Setup(x => x.AddCondition(name, Operator.IS_NOT, EmptyOperand.Empty))
                    .Returns(mockJqlClauseBuilder.Object);

                var builder = new ConditionBuilder(name, mockJqlClauseBuilder.Object);
                Assert.Same(mockJqlClauseBuilder.Object, builder.NotEmpty);

                mockJqlClauseBuilder.Verify();
            }

            [Fact]
            public void Builder()
            {
                const string name = "fieldName";

                var mockJqlClauseBuilder = new Mock<IJqlClauseBuilder>();
                mockJqlClauseBuilder.Setup(x => x.AddCondition(name, Operator.IS_NOT, EmptyOperand.Empty))
                    .Returns(mockJqlClauseBuilder.Object);

                var builder = new ConditionBuilder(name, mockJqlClauseBuilder.Object);
                var valueBuilder = builder.IsNot();
                Assert.NotNull(valueBuilder);
                Assert.Same(mockJqlClauseBuilder.Object, valueBuilder.Empty());

                mockJqlClauseBuilder.Verify();
            }
        }

        public class TheLtMethod
        {
            [Fact]
            public void TestString()
            {
                const string name = "fieldName";
                const string value = "mine";

                var mockJqlClauseBuilder = new Mock<IJqlClauseBuilder>();

                mockJqlClauseBuilder.Setup(x => x.AddStringCondition(name, Operator.LESS_THAN, value))
                    .Returns(mockJqlClauseBuilder.Object);

                var builder = new ConditionBuilder(name, mockJqlClauseBuilder.Object);
                Assert.Same(mockJqlClauseBuilder.Object, builder.Lt(value));

                mockJqlClauseBuilder.Verify();
            }

            [Fact]
            public void TestOperand()
            {
                const string name = "fieldName";
                IOperand value = new SingleValueOperand("4372738");

                var mockJqlClauseBuilder = new Mock<IJqlClauseBuilder>();

                mockJqlClauseBuilder.Setup(x => x.AddCondition(name, Operator.LESS_THAN, value))
                    .Returns(mockJqlClauseBuilder.Object);

                var builder = new ConditionBuilder(name, mockJqlClauseBuilder.Object);
                Assert.Same(mockJqlClauseBuilder.Object, builder.Lt(value));

                mockJqlClauseBuilder.Verify();
            }

            [Fact]
            public void TestInt()
            {
                const string name = "fieldName";
                const int value = 1;

                var mockJqlClauseBuilder = new Mock<IJqlClauseBuilder>();

                mockJqlClauseBuilder.Setup(x => x.AddNumberCondition(name, Operator.LESS_THAN, value))
                    .Returns(mockJqlClauseBuilder.Object);

                var builder = new ConditionBuilder(name, mockJqlClauseBuilder.Object);
                Assert.Same(mockJqlClauseBuilder.Object, builder.Lt(value));

                mockJqlClauseBuilder.Verify();
            }

            [Fact]
            public void TestBuilder()
            {
                const string name = "fieldName";
                const int value = 1;

                var mockJqlClauseBuilder = new Mock<IJqlClauseBuilder>();

                mockJqlClauseBuilder.Setup(x => x.AddNumberCondition(name, Operator.LESS_THAN, value))
                    .Returns(mockJqlClauseBuilder.Object);

                var builder = new ConditionBuilder(name, mockJqlClauseBuilder.Object);
                var valueBuilder = builder.Lt();
                Assert.NotNull(valueBuilder);
                Assert.Same(mockJqlClauseBuilder.Object, valueBuilder.Number(value));

                mockJqlClauseBuilder.Verify();
            }
        }

        public class TheLtEqMethod
        {
            [Fact]
            public void TestOperand()
            {
                const string name = "fieldName";
                IOperand value = new SingleValueOperand("4372738");

                var mockJqlClauseBuilder = new Mock<IJqlClauseBuilder>();

                mockJqlClauseBuilder.Setup(x => x.AddCondition(name, Operator.LESS_THAN_EQUALS, value))
                    .Returns(mockJqlClauseBuilder.Object);

                var builder = new ConditionBuilder(name, mockJqlClauseBuilder.Object);
                Assert.Same(mockJqlClauseBuilder.Object, builder.LtEq(value));

                mockJqlClauseBuilder.Verify();
            }

            [Fact]
            public void TestInt()
            {
                const string name = "fieldName";
                const int value = 1;

                var mockJqlClauseBuilder = new Mock<IJqlClauseBuilder>();

                mockJqlClauseBuilder.Setup(x => x.AddNumberCondition(name, Operator.LESS_THAN_EQUALS, value))
                    .Returns(mockJqlClauseBuilder.Object);

                var builder = new ConditionBuilder(name, mockJqlClauseBuilder.Object);
                Assert.Same(mockJqlClauseBuilder.Object, builder.LtEq(value));

                mockJqlClauseBuilder.Verify();
            }

            [Fact]
            public void TestBuilder()
            {
                const string name = "fieldName";
                const int value = 1;

                var mockJqlClauseBuilder = new Mock<IJqlClauseBuilder>();

                mockJqlClauseBuilder.Setup(x => x.AddNumberCondition(name, Operator.LESS_THAN_EQUALS, value))
                    .Returns(mockJqlClauseBuilder.Object);

                var builder = new ConditionBuilder(name, mockJqlClauseBuilder.Object);
                var valueBuilder = builder.LtEq();
                Assert.NotNull(valueBuilder);
                Assert.Same(mockJqlClauseBuilder.Object, valueBuilder.Number(value));

                mockJqlClauseBuilder.Verify();
            }
        }

        public class TheGtMethod
        {
            [Fact]
            public void TestString()
            {
                const string name = "fieldName";
                const string value = "mine";

                var mockJqlClauseBuilder = new Mock<IJqlClauseBuilder>();

                mockJqlClauseBuilder.Setup(x => x.AddStringCondition(name, Operator.GREATER_THAN, value))
                    .Returns(mockJqlClauseBuilder.Object);

                var builder = new ConditionBuilder(name, mockJqlClauseBuilder.Object);
                Assert.Same(mockJqlClauseBuilder.Object, builder.Gt(value));

                mockJqlClauseBuilder.Verify();
            }

            [Fact]
            public void TestOperand()
            {
                const string name = "fieldName";
                IOperand value = new SingleValueOperand("4372738");

                var mockJqlClauseBuilder = new Mock<IJqlClauseBuilder>();

                mockJqlClauseBuilder.Setup(x => x.AddCondition(name, Operator.GREATER_THAN, value))
                    .Returns(mockJqlClauseBuilder.Object);

                var builder = new ConditionBuilder(name, mockJqlClauseBuilder.Object);
                Assert.Same(mockJqlClauseBuilder.Object, builder.Gt(value));

                mockJqlClauseBuilder.Verify();
            }

            [Fact]
            public void TestInt()
            {
                const string name = "fieldName";
                const int value = 1;

                var mockJqlClauseBuilder = new Mock<IJqlClauseBuilder>();

                mockJqlClauseBuilder.Setup(x => x.AddNumberCondition(name, Operator.GREATER_THAN, value))
                    .Returns(mockJqlClauseBuilder.Object);

                var builder = new ConditionBuilder(name, mockJqlClauseBuilder.Object);
                Assert.Same(mockJqlClauseBuilder.Object, builder.Gt(value));

                mockJqlClauseBuilder.Verify();
            }

            [Fact]
            public void TestBuilder()
            {
                const string name = "fieldName";
                const int value = 1;

                var mockJqlClauseBuilder = new Mock<IJqlClauseBuilder>();

                mockJqlClauseBuilder.Setup(x => x.AddNumberCondition(name, Operator.GREATER_THAN, value))
                    .Returns(mockJqlClauseBuilder.Object);

                var builder = new ConditionBuilder(name, mockJqlClauseBuilder.Object);
                var valueBuilder = builder.Gt();
                Assert.NotNull(valueBuilder);
                Assert.Same(mockJqlClauseBuilder.Object, valueBuilder.Number(value));

                mockJqlClauseBuilder.Verify();
            }
        }

        public class TheGtEqMethod
        {
            [Fact]
            public void TestOperand()
            {
                const string name = "fieldName";
                IOperand value = new SingleValueOperand("4372738");

                var mockJqlClauseBuilder = new Mock<IJqlClauseBuilder>();

                mockJqlClauseBuilder.Setup(x => x.AddCondition(name, Operator.GREATER_THAN_EQUALS, value))
                    .Returns(mockJqlClauseBuilder.Object);

                var builder = new ConditionBuilder(name, mockJqlClauseBuilder.Object);
                Assert.Same(mockJqlClauseBuilder.Object, builder.GtEq(value));

                mockJqlClauseBuilder.Verify();
            }

            [Fact]
            public void TestInt()
            {
                const string name = "fieldName";
                const int value = 1;

                var mockJqlClauseBuilder = new Mock<IJqlClauseBuilder>();

                mockJqlClauseBuilder.Setup(x => x.AddNumberCondition(name, Operator.GREATER_THAN_EQUALS, value))
                    .Returns(mockJqlClauseBuilder.Object);

                var builder = new ConditionBuilder(name, mockJqlClauseBuilder.Object);
                Assert.Same(mockJqlClauseBuilder.Object, builder.GtEq(value));

                mockJqlClauseBuilder.Verify();
            }

            [Fact]
            public void TestBuilder()
            {
                const string name = "fieldName";
                const int value = 1;

                var mockJqlClauseBuilder = new Mock<IJqlClauseBuilder>();

                mockJqlClauseBuilder.Setup(x => x.AddNumberCondition(name, Operator.GREATER_THAN_EQUALS, value))
                    .Returns(mockJqlClauseBuilder.Object);

                var builder = new ConditionBuilder(name, mockJqlClauseBuilder.Object);
                var valueBuilder = builder.GtEq();
                Assert.NotNull(valueBuilder);
                Assert.Same(mockJqlClauseBuilder.Object, valueBuilder.Number(value));

                mockJqlClauseBuilder.Verify();
            }
        }

        public class TheInMethod
        {
            [Fact]
            public void TestStrings()
            {
                const string name = "fieldName";
                const string value1 = "value1";
                const string value2 = "value2";

                var mockJqlClauseBuilder = new Mock<IJqlClauseBuilder>();
                mockJqlClauseBuilder.Setup(x => x.AddStringCondition(name, Operator.IN, value1, value2))
                    .Returns(mockJqlClauseBuilder.Object);

                var builder = new ConditionBuilder(name, mockJqlClauseBuilder.Object);
                Assert.Same(mockJqlClauseBuilder.Object, builder.In(value1, value2));

                mockJqlClauseBuilder.Verify();
            }

            [Fact]
            public void TestStringCollection()
            {
                const string name = "fieldName";
                const string value1 = "value1";
                const string value2 = "value2";
                var values = new List<string>() {value1, value2};

                var mockJqlClauseBuilder = new Mock<IJqlClauseBuilder>();
                mockJqlClauseBuilder.Setup(x => x.AddStringCondition(name, Operator.IN, values))
                    .Returns(mockJqlClauseBuilder.Object);

                var builder = new ConditionBuilder(name, mockJqlClauseBuilder.Object);
                Assert.Same(mockJqlClauseBuilder.Object, builder.InStrings(values));

                mockJqlClauseBuilder.Verify();
            }
        }
    }
}