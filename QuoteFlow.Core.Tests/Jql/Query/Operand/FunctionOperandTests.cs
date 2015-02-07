using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuoteFlow.Api.Jql.Query.Operand;
using Xunit;

namespace QuoteFlow.Core.Tests.Jql.Query.Operand
{
    public class FunctionOperandTests
    {
        public class TheCtor
        {
            [Fact]
            public void Throws_If_NameIsNull()
            {
                var ex = Assert.Throws<ArgumentNullException>(() => new FunctionOperand(null));
                Assert.Equal("name", ex.ParamName);
            }

            [Fact]
            public void Throws_If_ArgsContainsNulls()
            {
                var ex = Assert.Throws<ArgumentException>(() => new FunctionOperand("theName", new List<string> { "test", null }));
                Assert.Equal("Cannot be empty.\r\nParameter name: args", ex.Message);
            }
        }

        [Fact]
        public void OperandName_Returns_CorrectName()
        {
            var functionOperand = new FunctionOperand("weGotDaFunk", new List<string>());
            Assert.Equal("weGotDaFunk", functionOperand.Name);
        }

        [Fact]
        public void ArgsContains_All_Values()
        {
            var strings = new[] {"test", "value"};
            var functionOperand = new FunctionOperand("weGotDaFunk", strings);
            Assert.Equal(2, functionOperand.Args.Count);
            Assert.True(strings.Contains(functionOperand.Args.ElementAt(0)));
            Assert.True(strings.Contains(functionOperand.Args.ElementAt(1)));
        }

        [Fact]
        public void EqualsTests()
        {
            var function1 = new FunctionOperand("nAME");
            var function2 = new FunctionOperand("name");
            var function3 = new FunctionOperand("name", "asdfgh");
            var function4 = new FunctionOperand("NAMe", "asdfgh");

            Assert.True(function1.Equals(function2));
            Assert.True(function2.Equals(function1));
            Assert.False(function3.Equals(function1));
            Assert.False(function1.Equals(function3));
            Assert.False(function3.Equals(function2));

            Assert.True(function3.Equals(function4));
            Assert.True(function4.Equals(function3));

            Assert.False(function1.Equals(null));
        }
    }
}
