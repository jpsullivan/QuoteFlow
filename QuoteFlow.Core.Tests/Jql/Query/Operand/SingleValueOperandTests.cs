using System;
using QuoteFlow.Api.Jql.Query.Operand;
using Xunit;

namespace QuoteFlow.Core.Tests.Jql.Query.Operand
{
    public class SingleValueOperandTests
    {
        public class TheCtor
        {
            [Fact]
            public void Throws_If_StringValue_IsNull()
            {
                var ex = Assert.Throws<ArgumentNullException>(() => new SingleValueOperand((string) null));
                Assert.Equal("stringValue", ex.ParamName);
            }
        }

        [Fact]
        public void OperandName_Returns_CorrectName()
        {
            Assert.Equal("SingleValueOperand", new SingleValueOperand(SimpleLiteralFactory.CreateLiteral(123)).Name);
        }

        public class TheIntValueProperty
        {
            [Fact]
            public void GetsTheCorrectValue()
            {
                var svo = new SingleValueOperand(123);
                Assert.Equal(123, svo.IntValue);
                Assert.Null(svo.StringValue);
            }

            [Fact]
            public void GetsTheCorrectValue_FromQueryLiteral()
            {
                var svo = new SingleValueOperand(SimpleLiteralFactory.CreateLiteral(123));
                Assert.Equal(123, svo.IntValue);
                Assert.Null(svo.StringValue);
            }
        }

        public class TheStringValueProperty
        {
            [Fact]
            public void GetsTheCorrectValue()
            {
                var svo = new SingleValueOperand("123");
                Assert.Equal("123", svo.StringValue);
                Assert.Null(svo.IntValue);
            }

            [Fact]
            public void GetsTheCorrectValue_FromQueryLiteral()
            {
                var svo = new SingleValueOperand(SimpleLiteralFactory.CreateLiteral("123"));
                Assert.Equal("\"123\"", svo.StringValue);
                Assert.Null(svo.IntValue);
            }
        }

        public class TheDisplayStringProperty
        {
            [Fact]
            public void FromIntValue()
            {
                var svo = new SingleValueOperand(SimpleLiteralFactory.CreateLiteral(123));
                Assert.Equal("123", svo.DisplayString);
            }

            [Fact]
            public void FromStringValue()
            {
                var svo = new SingleValueOperand(SimpleLiteralFactory.CreateLiteral("123"));
                Assert.Equal("\"123\"", svo.DisplayString);
            }
        }
    }
}
