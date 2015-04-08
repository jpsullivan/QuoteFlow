using System;
using System.Collections.Generic;
using Moq;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Operand;
using QuoteFlow.Core.Jql.Builder;
using QuoteFlow.Core.Jql.Query.Operand;
using Xunit;

namespace QuoteFlow.Core.Tests.Jql.Builder
{
    public class ValueBuilderTests
    {
        public class TheCtor
        {
            [Fact]
            public void BadConstructor()
            {
                var mockBuilder = new Mock<IJqlClauseBuilder>();

                try
                {
                    new ValueBuilder(null, "name", Operator.EQUALS);
                    Assert.True(false, "Exception expected.");
                }
                catch (ArgumentNullException ex)
                {
                    // expected
                }

                try
                {
                    new ValueBuilder(mockBuilder.Object, null, Operator.EQUALS);
                    Assert.True(false, "Exception expected.");
                }
                catch (ArgumentNullException ex)
                {
                    // expected
                }

                mockBuilder.Verify();
            }
        }

        public class TheStringMethod
        {
            [Fact]
            public void TestSingle()
            {
                const string name = "name";
                const string value = "value";

                var mockBuilder = new Mock<IJqlClauseBuilder>();
                mockBuilder.Setup(x => x.AddStringCondition(name, Operator.EQUALS, value)).Returns(mockBuilder.Object);

                var valueBuilder = new ValueBuilder(mockBuilder.Object, name, Operator.EQUALS);
                Assert.Same(mockBuilder.Object, valueBuilder.String(value));

                mockBuilder.Verify();
            }
        }

        public class TheStringsMethod
        {
            [Fact]
            public void TestVarArgs()
            {
                const string name = "name";
                const string value = "value";
                const string value2 = "value2";

                var mockBuilder = new Mock<IJqlClauseBuilder>();
                mockBuilder.Setup(x => x.AddStringCondition(name, Operator.EQUALS, value, value2))
                    .Returns(mockBuilder.Object);

                var valueBuilder = new ValueBuilder(mockBuilder.Object, name, Operator.EQUALS);
                Assert.Same(mockBuilder.Object, valueBuilder.Strings(value, value2));

                mockBuilder.Verify();
            }

            [Fact]
            public void TestCollection()
            {
                const string name = "name";
                const string value = "value";
                const string value2 = "value2";
                var values = new List<string>() {value, value2};
                
                var mockBuilder = new Mock<IJqlClauseBuilder>();
                mockBuilder.Setup(x => x.AddStringCondition(name, Operator.EQUALS, values)).Returns(mockBuilder.Object);

                var valueBuilder = new ValueBuilder(mockBuilder.Object, name, Operator.EQUALS);
                Assert.Same(mockBuilder.Object, valueBuilder.Strings(values));

                mockBuilder.Verify();
            }
        }

        public class TheNumberMethod
        {
            [Fact]
            public void TestSingle()
            {
                const string name = "name";
                const int value = -7;

                var mockBuilder = new Mock<IJqlClauseBuilder>();
                mockBuilder.Setup(x => x.AddNumberCondition(name, Operator.EQUALS, value)).Returns(mockBuilder.Object);

                var valueBuilder = new ValueBuilder(mockBuilder.Object, name, Operator.EQUALS);
                Assert.Same(mockBuilder.Object, valueBuilder.Number(value));

                mockBuilder.Verify();
            }
        }

        public class TheNumbersMethod
        {
            [Fact]
            public void TestVarArgs()
            {
                const string name = "name";
                const int value = 5;
                const int value2 = 6;

                var mockBuilder = new Mock<IJqlClauseBuilder>();
                mockBuilder.Setup(x => x.AddNumberCondition(name, Operator.EQUALS, value, value2))
                    .Returns(mockBuilder.Object);

                var valueBuilder = new ValueBuilder(mockBuilder.Object, name, Operator.EQUALS);
                Assert.Same(mockBuilder.Object, valueBuilder.Numbers(value, value2));

                mockBuilder.Verify();
            }

            [Fact]
            public void TestCollection()
            {
                const string name = "name";
                const int value = 5;
                const int value2 = 6;
                var values = new List<int?>() { value, value2 };

                var mockBuilder = new Mock<IJqlClauseBuilder>();
                mockBuilder.Setup(x => x.AddNumberCondition(name, Operator.EQUALS, values)).Returns(mockBuilder.Object);

                var valueBuilder = new ValueBuilder(mockBuilder.Object, name, Operator.EQUALS);
                Assert.Same(mockBuilder.Object, valueBuilder.Numbers(values));

                mockBuilder.Verify();
            }
        }

        public class TheOperandMethod
        {
            [Fact]
            public void TestSingle()
            {
                const string name = "name";
                IOperand value = Operands.ValueOf("3");

                var mockBuilder = new Mock<IJqlClauseBuilder>();
                mockBuilder.Setup(x => x.AddCondition(name, Operator.EQUALS, value)).Returns(mockBuilder.Object);

                var valueBuilder = new ValueBuilder(mockBuilder.Object, name, Operator.EQUALS);
                Assert.Same(mockBuilder.Object, valueBuilder.Operand(value));

                mockBuilder.Verify();
            }
        }

        public class TheOperandsMethod
        {
            [Fact]
            public void TestVarArgs()
            {
                const string name = "name";
                IOperand value = Operands.ValueOf("5");
                IOperand value2 = Operands.ValueOf(6);

                var mockBuilder = new Mock<IJqlClauseBuilder>();
                mockBuilder.Setup(x => x.AddCondition(name, Operator.EQUALS, value, value2))
                    .Returns(mockBuilder.Object);

                var valueBuilder = new ValueBuilder(mockBuilder.Object, name, Operator.EQUALS);
                Assert.Same(mockBuilder.Object, valueBuilder.Operands(value, value2));

                mockBuilder.Verify();
            }

            [Fact]
            public void TestCollection()
            {
                const string name = "name";
                IOperand value = Operands.ValueOf(6);
                IOperand value2 = Operands.ValueOf("7");
                var values = new List<IOperand>() { value, value2 };

                var mockBuilder = new Mock<IJqlClauseBuilder>();
                mockBuilder.Setup(x => x.AddCondition(name, Operator.EQUALS, values)).Returns(mockBuilder.Object);

                var valueBuilder = new ValueBuilder(mockBuilder.Object, name, Operator.EQUALS);
                Assert.Same(mockBuilder.Object, valueBuilder.Operands(values));

                mockBuilder.Verify();
            }
        }

        [Fact]
        public void TestEmpty()
        {
            const string name = "name";

            var mockBuilder = new Mock<IJqlClauseBuilder>();
            mockBuilder.Setup(x => x.AddCondition(name, Operator.EQUALS, EmptyOperand.Empty)).Returns(mockBuilder.Object);

            var valueBuilder = new ValueBuilder(mockBuilder.Object, name, Operator.EQUALS);
            Assert.Same(mockBuilder.Object, valueBuilder.Empty());

            mockBuilder.Verify();
        }

        public class TheFunctionMethod
        {
            [Fact]
            public void TestSingle()
            {
                const string name = "name";
                const string funcName = "funcName";

                var mockBuilder = new Mock<IJqlClauseBuilder>();
                mockBuilder.Setup(x => x.AddFunctionCondition(name, Operator.EQUALS, funcName)).Returns(mockBuilder.Object);

                var valueBuilder = new ValueBuilder(mockBuilder.Object, name, Operator.EQUALS);
                Assert.Same(mockBuilder.Object, valueBuilder.Function(funcName));

                mockBuilder.Verify();
            }

            [Fact]
            public void TestVarArgs()
            {
                const string name = "name";
                const string funcName = "funcName";

                var args = new[] {"7", "8", "9"};

                var mockBuilder = new Mock<IJqlClauseBuilder>();
                mockBuilder.Setup(x => x.AddFunctionCondition(name, Operator.EQUALS, funcName, args)).Returns(mockBuilder.Object);

                var valueBuilder = new ValueBuilder(mockBuilder.Object, name, Operator.EQUALS);
                Assert.Same(mockBuilder.Object, valueBuilder.Function(funcName, args));

                mockBuilder.Verify();
            }

            [Fact]
            public void TestCollection()
            {
                const string name = "name";
                const string funcName = "funcName";

                var args = new List<string>() {"what", "is", "a", "builder"};

                var mockBuilder = new Mock<IJqlClauseBuilder>();
                mockBuilder.Setup(x => x.AddFunctionCondition(name, Operator.EQUALS, funcName, args)).Returns(mockBuilder.Object);

                var valueBuilder = new ValueBuilder(mockBuilder.Object, name, Operator.EQUALS);
                Assert.Same(mockBuilder.Object, valueBuilder.Function(funcName, args));

                mockBuilder.Verify();
            }
        }
    }
}