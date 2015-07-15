using System;
using System.Collections.Generic;
using QuoteFlow.Api.Jql.Query.Operand;
using QuoteFlow.Core.Jql.Query.Operand;
using Xunit;

namespace QuoteFlow.Core.Tests.Jql.Query.Operand
{
    public class OperandsTests
    {
        public class TheValueOfMethod
        {
            [Fact]
            public void StringNull()
            {
                Assert.Throws<ArgumentNullException>(() => Operands.ValueOf((string) null));
            }

            [Fact]
            public void String()
            {
                const string value = "test";
                Assert.Equal(new SingleValueOperand(value), Operands.ValueOf(value));
            }

            [Fact]
            public void StringVarArgsBad()
            {
                Assert.Throws<ArgumentNullException>(() => Operands.ValueOf("djakdsa", null));
                Assert.Throws<ArgumentNullException>(() => Operands.ValueOf((string[]) null));
                Assert.Throws<ArgumentException>(() => Operands.ValueOf(new string[] {}));
            }

            [Fact]
            public void StringVarArgs()
            {
                const string value = "test";
                const string value2 = "test2";
                Assert.Equal(new MultiValueOperand(value, value2), Operands.ValueOf(value, value2));
            }

            [Fact]
            public void Number()
            {
                const int value = 1;
                Assert.Equal(new SingleValueOperand(value), Operands.ValueOf(value));
            }

            [Fact]
            public void NumberVarArgs()
            {
                const int value = 5733;
                const int value2 = 1231;
                Assert.Equal(new MultiValueOperand(value, value2), Operands.ValueOf(value, value2));
            }

            [Fact]
            public void OperandsVarArgsBad()
            {
                Assert.Throws<ArgumentNullException>(() => Operands.ValueOf(new SingleValueOperand(5), null));
                Assert.Throws<ArgumentNullException>(() => Operands.ValueOf((IOperand[]) null));
                Assert.Throws<ArgumentException>(() => Operands.ValueOf(new IOperand[] {}));
            }

            [Fact]
            public void OperandsVarArgs()
            {
                var value = new SingleValueOperand(5123);
                var value2 = new SingleValueOperand(1231);
                Assert.Equal(new MultiValueOperand(value, value2), Operands.ValueOf(value, value2));
            }
        }

        public class TheValueOfStringsMethod
        {
            [Fact]
            public void StringCollectionBad()
            {
                Assert.Throws<ArgumentNullException>(() => Operands.ValueOfStrings(new List<string> { "djakdsa", null }));
                Assert.Throws<ArgumentNullException>(() => Operands.ValueOfStrings(null));
                Assert.Throws<ArgumentException>(() => Operands.ValueOfStrings(new List<string>()));
            }

            [Fact]
            public void StringCollection()
            {
                const string value = "test";
                const string value2 = "test2";
                var values = new List<string>() {value, value2};
                Assert.Equal(new MultiValueOperand(value, value2), Operands.ValueOfStrings(values));
            }
        }

        public class TheValueOfNumbersMethod
        {
            [Fact]
            public void NumberCollectionBad()
            {
                Assert.Throws<ArgumentNullException>(() => Operands.ValueOfNumbers(null));
            }

            [Fact]
            public void NumberCollection()
            {
                const int value = 5733;
                const int value2 = 1231;
                var values = new List<int?>() { value, value2 };
                Assert.Equal(new MultiValueOperand(value, value2), Operands.ValueOfNumbers(values));
            }
        }

        public class TheValueOfOperandsMethod
        {
            [Fact]
            public void CollectionBad()
            {
                Assert.Throws<ArgumentNullException>(() => Operands.ValueOfOperands(new List<IOperand> {new SingleValueOperand(5), null}));
                Assert.Throws<ArgumentNullException>(() => Operands.ValueOfOperands(null));
                Assert.Throws<ArgumentException>(() => Operands.ValueOfOperands(new List<IOperand>()));
            }

            [Fact]
            public void Collection()
            {
                var value = new SingleValueOperand(5123);
                var value2 = new SingleValueOperand(1231);
                var values = new List<IOperand> {value, value2};
                Assert.Equal(new MultiValueOperand(value, value2), Operands.ValueOfOperands(values));
            }
        }

        public class TheGetEmptyMethod
        {
            [Fact]
            public void ReturnsEmptyOperand()
            {
                Assert.Equal(EmptyOperand.Empty, Operands.GetEmpty());
            }
        }
    }
}