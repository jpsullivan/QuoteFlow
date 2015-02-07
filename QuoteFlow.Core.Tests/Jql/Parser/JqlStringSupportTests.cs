using System;
using System.Collections.Generic;
using Moq;
using QuoteFlow.Api.Jql.Parser;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Core.Jql.Builder;
using QuoteFlow.Core.Jql.Util;
using Xunit;
using Xunit.Extensions;

namespace QuoteFlow.Core.Tests.Jql.Parser
{
    public class JqlStringSupportTests
    {
        public class TheDecodeMethod
        {
            public static IEnumerable<object[]> ValidList
            {
                get
                {
                    yield return new object[] { "jill", JqlStringSupport.Decode("jill") };
                    yield return new object[] { "", JqlStringSupport.Decode("") };
                    yield return new object[] { null, JqlStringSupport.Decode(null) };
                    yield return new object[] { "   ", JqlStringSupport.Decode("   ") };
                    yield return new object[] { "this \u5678", JqlStringSupport.Decode("this \\u5678") };
                    yield return new object[] { "this \t\nthis is very bad\r", JqlStringSupport.Decode("this \\t\\nthis is very bad\\r") };
                    yield return new object[] { "\t", JqlStringSupport.Decode("\\t") };
                    yield return new object[] { "\\t\n", JqlStringSupport.Decode("\\\\t\\n") };
                    yield return new object[] { " this is a \u7463string with an escaped space.", JqlStringSupport.Decode("\\ this is a \\u7463string with an escaped space.") };
                    yield return new object[] { "don\"t", JqlStringSupport.Decode("don\\\"t") };
                    yield return new object[] { "don't", JqlStringSupport.Decode("don\\\'t") };
                }
            }
                
            [Theory]
            [PropertyData("ValidList")]
            public void ValidDecodes(string jql, string decodeResult)
            {
                Assert.Equal(jql, decodeResult);
            }

            [Theory]
            [InlineData("\\")]
            [InlineData("\\q")]
            [InlineData("bebwrnb\\qewner")]
            [InlineData("bebwrnb\\u377gqewner")]
            [InlineData("bebwrnb\\u377 ")]
            [InlineData("bebwrnb\\u 377 ")]
            [InlineData("bebwrnb\\u-377")]
            [InlineData("bebwrnb\\u377")]
            [InlineData("rjhewkjherkwjherkjwhe\\")]
            public void InvalidDecodes(string jql)
            {
                AssertInvalidDecode(jql);
            }

            private void AssertInvalidDecode(string input)
            {
                try
                {
                    JqlStringSupport.Decode(input);
                    Assert.True(false, string.Format("String '{0}' should have failed.", input));
                }
                catch (Exception ex)
                {
                    // ignored
                }
            }
        }

        public class TheEncodeAsQuotedStringMethod
        {
            public static IEnumerable<object[]> WrappingTestData
            {
                get
                {
                    yield return new object[] { "\"jack\"", JqlStringSupport.EncodeAsQuotedString("jack") };
                    yield return new object[] { "\"jack and jill\"", JqlStringSupport.EncodeAsQuotedString("jack and jill") };
                    yield return new object[] { "\"\"", JqlStringSupport.EncodeAsQuotedString("") };
                    yield return new object[] { "\"\uef12 is some kind of unicode character\"", JqlStringSupport.EncodeAsQuotedString("\uef12 is some kind of unicode character") };
                }
            }

            [Theory]
            [PropertyData("WrappingTestData")]
            public void WrappingTests(string jql, string quotedString)
            {
                Assert.Equal(jql, quotedString);
            }

            [Fact]
            public void EncodingNull_Wraps()
            {
                Assert.Null(JqlStringSupport.EncodeAsQuotedString(null));
            }

            public static IEnumerable<object[]> EncodableTestData
            {
                get
                {
                    yield return new object[] { "\"\\n\"", JqlStringSupport.EncodeAsQuotedString("\n", true) };
                    yield return new object[] { "\"\n\"", JqlStringSupport.EncodeAsQuotedString("\n", false) };
                    yield return new object[] { "\"Tab: \\t, CR: \\r, NL: \\n\"", JqlStringSupport.EncodeAsQuotedString("Tab: \t, CR: \r, NL: \n", true) };
                    yield return new object[] { "\"Tab: \\t, CR: \r, NL: \n\"", JqlStringSupport.EncodeAsQuotedString("Tab: \t, CR: \r, NL: \n", false) };
                    yield return new object[] { "\"Control Character: \\u000c \\u0005\"", JqlStringSupport.EncodeAsQuotedString("Control Character: \f \u0005") };
                    yield return new object[] { "\"Double quotes (\\\") need to be escaped.\"", JqlStringSupport.EncodeAsQuotedString("Double quotes (\") need to be escaped.") };
                    yield return new object[] { "\"This backslash \\\\ also needs escaping\"", JqlStringSupport.EncodeAsQuotedString("This backslash \\ also needs escaping") };
                }
            }

            [Theory]
            [PropertyData("EncodableTestData")]
            public void SuccessfullyEncodesAsQuotedString(string jql, string encodedString)
            {
                Assert.Equal(jql, encodedString);
            }
        }

        public class TheEncodeCharacterMethod
        {
            [Fact]
            public void EncodesTheCharacter()
            {
            }
        }

        public class TheEncodeCharacterForceMethod { }

        public class TheEncodeStringValueNumberMethod
        {
            [Fact]
            public void EncodesProperly()
            {
                var mockParser = new Mock<IJqlQueryParser>();
                var support = new JqlStringSupport(mockParser.Object);
                
                Assert.Equal("\"10002\"", support.EncodeStringValue("10002"));

                mockParser.Verify();
            }
        }

        public class TheEncodeStringValueMethod
        {
            [Fact]
            public void Valid()
            {
                var mockParser = new Mock<IJqlQueryParser>();
                mockParser.Setup(x => x.IsValidValue("testme2")).Returns(true);

                var support = new JqlStringSupport(mockParser.Object);
                Assert.Equal("testme2", support.EncodeStringValue("testme2"));

                mockParser.Verify();
            }

            [Fact]
            public void Invalid()
            {
                const string value = "tes\ntme";
                var mockParser = new Mock<IJqlQueryParser>();
                mockParser.Setup(x => x.IsValidValue(value)).Returns(false);

                var support = new JqlStringSupport(mockParser.Object);
                Assert.Equal("\"tes\ntme\"", support.EncodeStringValue(value));

                mockParser.Verify();
            }
        }

        public class TheEncodeValueNumberMethod
        {
            [Fact]
            public void EncodesProperly()
            {
                var mockParser = new Mock<IJqlQueryParser>();
                mockParser.Setup(x => x.IsValidValue("10002")).Returns(true);

                var support = new JqlStringSupport(mockParser.Object);
                Assert.Equal("10002", support.EncodeValue("10002"));

                mockParser.Verify();
            }
        }

        public class TheEncodeValueMethod
        {
            [Fact]
            public void Valid()
            {
                const string value = "tes\ntme";
                var mockParser = new Mock<IJqlQueryParser>();
                mockParser.Setup(x => x.IsValidValue(value)).Returns(false);

                var support = new JqlStringSupport(mockParser.Object);
                Assert.Equal("\"tes\ntme\"", support.EncodeValue(value));

                mockParser.Verify();
            }

            [Fact]
            public void Invalid()
            {
                var mockParser = new Mock<IJqlQueryParser>();
                mockParser.Setup(x => x.IsValidValue("testme2")).Returns(true);

                var support = new JqlStringSupport(mockParser.Object);
                Assert.Equal("testme2", support.EncodeValue("testme2"));

                mockParser.Verify();
            }
        }

        public class TheEncodeFunctionArgumentMethod
        {
            [Fact]
            public void Valid()
            {
                var mockParser = new Mock<IJqlQueryParser>();
                mockParser.Setup(x => x.IsValidFunctionArgument("testme2")).Returns(true);

                var support = new JqlStringSupport(mockParser.Object);
                Assert.Equal("testme2", support.EncodeFunctionArgument("testme2"));

                mockParser.Verify();
            }

            [Fact]
            public void Invalid()
            {
                const string argument = "tes\ntme";
                var mockParser = new Mock<IJqlQueryParser>();
                mockParser.Setup(x => x.IsValidFunctionArgument(argument)).Returns(false);

                var support = new JqlStringSupport(mockParser.Object);
                Assert.Equal("\"tes\\ntme\"", support.EncodeFunctionArgument(argument));

                mockParser.Verify();
            }
        }

        public class TheEncodeFunctionNameMethod
        {
            [Fact]
            public void Valid()
            {
                var mockParser = new Mock<IJqlQueryParser>();
                mockParser.Setup(x => x.IsValidFunctionName("testme2")).Returns(true);

                var support = new JqlStringSupport(mockParser.Object);
                Assert.Equal("testme2", support.EncodeFunctionName("testme2"));

                mockParser.Verify();
            }

            [Fact]
            public void Invalid()
            {
                var mockParser = new Mock<IJqlQueryParser>();
                mockParser.Setup(x => x.IsValidFunctionName("testme")).Returns(false);

                var support = new JqlStringSupport(mockParser.Object);
                Assert.Equal("\"testme\"", support.EncodeFunctionName("testme"));

                mockParser.Verify();
            }
        }

        public class TheEncodeFieldNameMethod
        {
            [Fact]
            public void Valid()
            {
                var mockParser = new Mock<IJqlQueryParser>();
                mockParser.Setup(x => x.IsValidFieldName("testme")).Returns(true);

                var support = new JqlStringSupport(mockParser.Object);
                Assert.Equal("testme", support.EncodeFieldName("testme"));

                mockParser.Verify();
            }

            [Fact]
            public void Invalid()
            {
                const string name = "test\nme";
                var mockParser = new Mock<IJqlQueryParser>();
                mockParser.Setup(x => x.IsValidFieldName(name)).Returns(false);

                var support = new JqlStringSupport(mockParser.Object);
                Assert.Equal("\"test\\nme\"", support.EncodeFieldName(name));

                mockParser.Verify();
            }
        }

        public class TheGenerateJqlStringMethod
        {
            [Fact]
            public void Empty()
            {
                var mockParser = new Mock<IJqlQueryParser>();
                mockParser.Setup(x => x.IsValidFieldName(It.IsAny<string>())).Returns(true);

                var support = new JqlStringSupport(mockParser.Object);
                var builder = JqlQueryBuilder.NewBuilder().Where().AddCondition("qwerty").Eq("").Or().Description("foo");

                IQuery query = new Query(builder.BuildClause(), "ignore = me");

                Assert.Equal("qwerty = \"\" OR description = \"foo\"", support.GenerateJqlString(query));

                mockParser.Verify();
            }
        }
    }
}
