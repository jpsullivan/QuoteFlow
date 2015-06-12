using System.Collections.Generic;
using QuoteFlow.Api.Jql.Parser;
using Xunit;

namespace QuoteFlow.Core.Tests.Jql.Parser
{
    public class JqlParseErrorMessageTests
    {
        [Fact]
        public void TestCtorVarArgs()
        {
            JqlParseErrorMessage errorMessage = new JqlParseErrorMessage("key", 0, 10, 1111);
            Assert.Equal(-1, errorMessage.LineNumber);
            Assert.Equal(10, errorMessage.ColumnNumber);
            Assert.Equal("key", errorMessage.Key);
            Assert.Equal(new List<object> { 1111 }, errorMessage.Arguments);

            errorMessage = new JqlParseErrorMessage("qwertyt", 1, -220, "hello", "world");
            Assert.Equal(1, errorMessage.LineNumber);
            Assert.Equal(-1, errorMessage.ColumnNumber);
            Assert.Equal("qwertyt", errorMessage.Key);
            Assert.Equal(new List<object> { "hello", "world" }, errorMessage.Arguments);

            try
            {
                new JqlParseErrorMessage("", 10, 1, 10011);
                Assert.True(false, "Expected exception.");
            }
            catch (System.ArgumentException expected)
            {
                //expected.
            }

            try
            {
                new JqlParseErrorMessage(null, 10, 1, 10011);
                Assert.True(false, "Expected exception.");
            }
            catch (System.ArgumentException expected)
            {
                //expected.
            }

            try
            {
                new JqlParseErrorMessage("qwerty", 10, 1, "qwerty", null);
                Assert.True(false, "Expected exception.");
            }
            catch (System.ArgumentNullException expected)
            {
                //expected.
            }
        }

        [Fact]
        public void TestCtorCollections()
        {
            JqlParseErrorMessage errorMessage = new JqlParseErrorMessage("key", 0, 10, new List<object> { 18829 });
            Assert.Equal(-1, errorMessage.LineNumber);
            Assert.Equal(10, errorMessage.ColumnNumber);
            Assert.Equal("key", errorMessage.Key);
            Assert.Equal(new List<object> {18829}, errorMessage.Arguments);

            errorMessage = new JqlParseErrorMessage("qwertyt", 1, -220, "hello", "world");
            Assert.Equal(1, errorMessage.LineNumber);
            Assert.Equal(-1, errorMessage.ColumnNumber);
            Assert.Equal("qwertyt", errorMessage.Key);
            Assert.Equal(new List<object> {"hello", "world" }, errorMessage.Arguments);

            try
            {
                new JqlParseErrorMessage("", 10, 1, new List<object>());
                Assert.True(false, "Expected IAE.");
            }
            catch (System.ArgumentException expected)
            {
                //expected.
            }

            try
            {
                new JqlParseErrorMessage(null, 10, 1, new List<object>());
                Assert.True(false, "Expected IAE.");
            }
            catch (System.ArgumentException expected)
            {
                //expected.
            }

            try
            {
                new JqlParseErrorMessage("qwerty", 10, 1, null, "a");
                Assert.True(false, "Expected IAE.");
            }
            catch (System.ArgumentNullException expected)
            {
                //expected.
            }

            try
            {
                new JqlParseErrorMessage("qwerty", 10, 1, (ICollection<object>) null);
                Assert.True(false, "Expected IAE.");
            }
            catch (System.ArgumentNullException expected)
            {
                //expected.
            }
        }
    }
}