using System;
using Antlr.Runtime;
using QuoteFlow.Api.Jql.Parser;
using QuoteFlow.Core.Jql.AntlrGen;
using QuoteFlow.Core.Jql.Parser;
using Xunit;

namespace QuoteFlow.Core.Tests.Jql.Parser
{
    public class JqlParseErrorMessagesTests
    {
        [Fact]
        public void TestReservedWord()
        {
            var expectedMessage = new JqlParseErrorMessage("jql.parse.reserved.word", 1, 21, "1", "21", "reserved");
            var actualMessage = JqlParseErrorMessages.ReservedWord("reserved", 1, 20);
            Assert.Equal(expectedMessage, actualMessage);

            expectedMessage = new JqlParseErrorMessage("jql.parse.reserved.word", -1, 11, "?", "?", "reserved");
            actualMessage = JqlParseErrorMessages.ReservedWord("reserved", -1, 10);
            Assert.Equal(expectedMessage, actualMessage);

            expectedMessage = new JqlParseErrorMessage("jql.parse.reserved.word", 10, -1, "10", "?", "reserved");
            actualMessage = JqlParseErrorMessages.ReservedWord("reserved", 10, -1);
            Assert.Equal(expectedMessage, actualMessage);

            expectedMessage = new JqlParseErrorMessage("jql.parse.reserved.word", 10, -1, "10", "?", "rese rv ed");
            actualMessage = JqlParseErrorMessages.ReservedWord("rese\nrv\red", 10, -1);
            Assert.Equal(expectedMessage, actualMessage);

            try
            {
                JqlParseErrorMessages.ReservedWord(null, 1, 20);
                Assert.True(false, "Error is expected.");
            }
            catch (ArgumentNullException e)
            {
                //expected
            }
        }

        [Fact]
        public void TestIllegalEscape()
        {
            var expectedMessage = new JqlParseErrorMessage("jql.parse.illegal.escape", 1, 21, "1", "21", "\\u484");
            var actualMessage = JqlParseErrorMessages.IllegalEscape("\\u484", 1, 20);
            Assert.Equal(expectedMessage, actualMessage);

            expectedMessage = new JqlParseErrorMessage("jql.parse.illegal.escape", 1, 21, "1", "21", "\\u4 84");
            actualMessage = JqlParseErrorMessages.IllegalEscape("\\u4\n84", 1, 20);
            Assert.Equal(expectedMessage, actualMessage);

            expectedMessage = new JqlParseErrorMessage("jql.parse.illegal.escape.blank", 1, -2902, "1", "?");
            actualMessage = JqlParseErrorMessages.IllegalEscape(null, 1, -1);
            Assert.Equal(expectedMessage, actualMessage);

            expectedMessage = new JqlParseErrorMessage("jql.parse.illegal.escape.blank", -1, 2003, "?", "?");
            actualMessage = JqlParseErrorMessages.IllegalEscape("   ", -1, 2002);
            Assert.Equal(expectedMessage, actualMessage);
        }

        [Fact]
        public void TestReservedCharacter()
        {
            var expectedMessage = new JqlParseErrorMessage("jql.parse.reserved.character", 1, 21, "1", "21", "a", "\\u0061");
            var actualMessage = JqlParseErrorMessages.ReservedCharacter('a', 1, 20);
            Assert.Equal(expectedMessage, actualMessage);

            expectedMessage = new JqlParseErrorMessage("jql.parse.reserved.character", 7, 8, "7", "8", "U+000A", "\\n");
            actualMessage = JqlParseErrorMessages.ReservedCharacter('\n', 7, 7);
            Assert.Equal(expectedMessage, actualMessage);

            expectedMessage = new JqlParseErrorMessage("jql.parse.reserved.character", -1, 8, "?", "?", "U+FFFF", "\\uffff");
            actualMessage = JqlParseErrorMessages.ReservedCharacter('\uffff', -1, 7);
            Assert.Equal(expectedMessage, actualMessage);

            expectedMessage = new JqlParseErrorMessage("jql.parse.reserved.character", 4, 8, "4", "8", "TAB", "\\t");
            actualMessage = JqlParseErrorMessages.ReservedCharacter('\t', 4, 7);
            Assert.Equal(expectedMessage, actualMessage);
        }

        [Fact]
        public void TestIllegalCharacter()
        {
            var expectedMessage = new JqlParseErrorMessage("jql.parse.illegal.character", 1, 21, "1", "21", "a", "\\u0061");
            var actualMessage = JqlParseErrorMessages.IllegalCharacter('a', 1, 20);
            Assert.Equal(expectedMessage, actualMessage);

            expectedMessage = new JqlParseErrorMessage("jql.parse.illegal.character", 7, 8, "7", "8", "U+000D", "\\r");
            actualMessage = JqlParseErrorMessages.IllegalCharacter('\r', 7, 7);
            Assert.Equal(expectedMessage, actualMessage);

            expectedMessage = new JqlParseErrorMessage("jql.parse.illegal.character", -1, 8, "?", "?", "U+FFFF", "\\uffff");
            actualMessage = JqlParseErrorMessages.IllegalCharacter('\uffff', -1, 7);
            Assert.Equal(expectedMessage, actualMessage);

            expectedMessage = new JqlParseErrorMessage("jql.parse.illegal.character", 4, 9, "4", "9", "TAB", "\\t");
            actualMessage = JqlParseErrorMessages.IllegalCharacter('\t', 4, 8);
            Assert.Equal(expectedMessage, actualMessage);
        }

        [Fact]
        public void TestGenericErrorPosition()
        {
            var expectedMessage = new JqlParseErrorMessage("jql.parse.unknown", 1, 21, "1", "21");
            var actualMessage = JqlParseErrorMessages.GenericParseError(1, 20);
            Assert.Equal(expectedMessage, actualMessage);

            expectedMessage = new JqlParseErrorMessage("jql.parse.unknown", -1, 21, "?", "?");
            actualMessage = JqlParseErrorMessages.GenericParseError(-2, 20);
            Assert.Equal(expectedMessage, actualMessage);

            expectedMessage = new JqlParseErrorMessage("jql.parse.unknown", 99, -1, "99", "?");
            actualMessage = JqlParseErrorMessages.GenericParseError(99, -27387198);
            Assert.Equal(expectedMessage, actualMessage);
        }

        [Fact]
        public void TestGenericError()
        {
            var expectedMessage = new JqlParseErrorMessage("jql.parse.unknown.no.pos", -1, -1);
            var actualMessage = JqlParseErrorMessages.GenericParseError();
            Assert.Equal(expectedMessage, actualMessage);
        }

        [Fact]
        public void TestGenericErrorToken()
        {
            var expectedMessage = new JqlParseErrorMessage("jql.parse.unknown", 1, 21, "1", "21");
            var actualMessage = JqlParseErrorMessages.GenericParseError(CreateToken(null, 1, 20));
            Assert.Equal(expectedMessage, actualMessage);

            expectedMessage = new JqlParseErrorMessage("jql.parse.unknown", -1, 21, "?", "?");
            actualMessage = JqlParseErrorMessages.GenericParseError(CreateToken(null, -2, 20));
            Assert.Equal(expectedMessage, actualMessage);

            expectedMessage = new JqlParseErrorMessage("jql.parse.unknown", 99, -1, "99", "?");
            actualMessage = JqlParseErrorMessages.GenericParseError(CreateToken(null, 99, -27387198));
            Assert.Equal(expectedMessage, actualMessage);

            expectedMessage = new JqlParseErrorMessage("jql.parse.unknown.no.pos", -1, -1);
            actualMessage = JqlParseErrorMessages.GenericParseError(CreateEofToken());
            Assert.Equal(expectedMessage, actualMessage);

            try
            {
                JqlParseErrorMessages.GenericParseError(null);
                Assert.True(false, "Expected an error to be thrown.");
            }
            catch (ArgumentNullException ignored)
            {
                //expected.
            }
        }

        [Fact]
        public void TestUnfinishedString()
        {
            JqlParseErrorMessage expectedMessage = new JqlParseErrorMessage("jql.parse.unfinished.string.blank", -1, -1, "?", "?");
            JqlParseErrorMessage actualMessage = JqlParseErrorMessages.UnfinishedString(null, -1, -1);
            Assert.Equal(expectedMessage, actualMessage);

            expectedMessage = new JqlParseErrorMessage("jql.parse.unfinished.string", -1, -1, "?", "?", "dylan");
            actualMessage = JqlParseErrorMessages.UnfinishedString("dylan", -20000, -1);
            Assert.Equal(expectedMessage, actualMessage);

            expectedMessage = new JqlParseErrorMessage("jql.parse.unfinished.string", -1, -1, "?", "?", "d ylan");
            actualMessage = JqlParseErrorMessages.UnfinishedString("d\nylan", -20000, -1);
            Assert.Equal(expectedMessage, actualMessage);
        }

        [Fact]
        public void TestIllegalNumber()
        {
            const string number = "373738";
            JqlParseErrorMessage expectedMessage = new JqlParseErrorMessage("jql.parse.illegal.number", 1, 5, "1", "5", number, long.MinValue, long.MaxValue);
            JqlParseErrorMessage actualMessage = JqlParseErrorMessages.IllegalNumber(number, 1, 4);
            Assert.Equal(expectedMessage, actualMessage);

            expectedMessage = new JqlParseErrorMessage("jql.parse.illegal.number", 1, 5, "1", "5", "7383 eeee", long.MinValue, long.MaxValue);
            actualMessage = JqlParseErrorMessages.IllegalNumber("7383\neeee", 1, 4);
            Assert.Equal(expectedMessage, actualMessage);

            try
            {
                JqlParseErrorMessages.IllegalNumber(null, 1, 4);
                Assert.True(false, "Expected an error to be thrown.");
            }
            catch (ArgumentNullException ignored)
            {
                //expected.
            }
        }

        [Fact]
        public void TestEmptyFieldName()
        {
            JqlParseErrorMessage expectedMessage = new JqlParseErrorMessage("jql.parse.empty.field", 1, 21, "1", "21");
            JqlParseErrorMessage actualMessage = JqlParseErrorMessages.EmptyFieldName(1, 20);
            Assert.Equal(expectedMessage, actualMessage);

            expectedMessage = new JqlParseErrorMessage("jql.parse.empty.field", -1, 21, "?", "?");
            actualMessage = JqlParseErrorMessages.EmptyFieldName(-2, 20);
            Assert.Equal(expectedMessage, actualMessage);

            expectedMessage = new JqlParseErrorMessage("jql.parse.empty.field", 99, -1, "99", "?");
            actualMessage = JqlParseErrorMessages.EmptyFieldName(99, -27387198);
            Assert.Equal(expectedMessage, actualMessage);
        }

        [Fact]
        public void TestEmptyFunctionName()
        {
            JqlParseErrorMessage expectedMessage = new JqlParseErrorMessage("jql.parse.empty.function", 1, 21, "1", "21");
            JqlParseErrorMessage actualMessage = JqlParseErrorMessages.EmptyFunctionName(1, 20);
            Assert.Equal(expectedMessage, actualMessage);

            expectedMessage = new JqlParseErrorMessage("jql.parse.empty.function", -1, 21, "?", "?");
            actualMessage = JqlParseErrorMessages.EmptyFunctionName(-2, 20);
            Assert.Equal(expectedMessage, actualMessage);

            expectedMessage = new JqlParseErrorMessage("jql.parse.empty.function", 99, -1, "99", "?");
            actualMessage = JqlParseErrorMessages.EmptyFunctionName(99, -27387198);
            Assert.Equal(expectedMessage, actualMessage);
        }

        [Fact]
        public void TestBadFieldName()
        {
            JqlParseErrorMessage expectedMessage = new JqlParseErrorMessage("jql.parse.no.field.eof", -1, -1);
            JqlParseErrorMessage actualMessage = JqlParseErrorMessages.BadFieldName(CreateEofToken());
            Assert.Equal(expectedMessage, actualMessage);

            expectedMessage = new JqlParseErrorMessage("jql.parse.no.field", 99, 3, "99", "3", "bad");
            actualMessage = JqlParseErrorMessages.BadFieldName(CreateToken("bad", 99, 2));
            Assert.Equal(expectedMessage, actualMessage);

            expectedMessage = new JqlParseErrorMessage("jql.parse.no.field", 99, -1, "99", "?", "ba dder");
            actualMessage = JqlParseErrorMessages.BadFieldName(CreateToken("ba\ndder", 99, -232482907));
            Assert.Equal(expectedMessage, actualMessage);

            expectedMessage = new JqlParseErrorMessage("jql.parse.no.cf.field", 99, -1, "99", "?");
            actualMessage = JqlParseErrorMessages.BadFieldName(CreateToken("badder", 99, -232482907, JqlLexer.LBRACKET));
            Assert.Equal(expectedMessage, actualMessage);

            try
            {
                JqlParseErrorMessages.BadFieldName(null);
                Assert.True(false, "Expected an error to be thrown.");
            }
            catch (ArgumentNullException ignored)
            {
                //expected.
            }
        }

        [Fact]
        public void TestBadSortOrder()
        {
            JqlParseErrorMessage expectedMessage = new JqlParseErrorMessage("jql.parse.no.order.eof", -1, -1);
            JqlParseErrorMessage actualMessage = JqlParseErrorMessages.BadSortOrder(CreateEofToken());
            Assert.Equal(expectedMessage, actualMessage);

            expectedMessage = new JqlParseErrorMessage("jql.parse.no.order", 99, 3, "99", "3", "bad");
            actualMessage = JqlParseErrorMessages.BadSortOrder(CreateToken("bad", 99, 2));
            Assert.Equal(expectedMessage, actualMessage);

            expectedMessage = new JqlParseErrorMessage("jql.parse.no.order", -38, 124, "?", "?", "ba dder");
            actualMessage = JqlParseErrorMessages.BadSortOrder(CreateToken("ba\ndder", -32, 123));
            Assert.Equal(expectedMessage, actualMessage);

            try
            {
                JqlParseErrorMessages.BadSortOrder(null);
                Assert.True(false, "Expected an error to be thrown.");
            }
            catch (ArgumentNullException ignored)
            {
                //expected.
            }
        }

        [Fact]
        public void TestBadOperator()
        {
            JqlParseErrorMessage expectedMessage = new JqlParseErrorMessage("jql.parse.no.operator.eof", -1, -1);
            JqlParseErrorMessage actualMessage = JqlParseErrorMessages.BadOperator(CreateEofToken());
            Assert.Equal(expectedMessage, actualMessage);

            expectedMessage = new JqlParseErrorMessage("jql.parse.no.operator", 99, 3, "99", "3", "bad");
            actualMessage = JqlParseErrorMessages.BadOperator(CreateToken("bad", 99, 2));
            Assert.Equal(expectedMessage, actualMessage);

            expectedMessage = new JqlParseErrorMessage("jql.parse.no.operator", 1, -3, "1", "?", "bad der");
            actualMessage = JqlParseErrorMessages.BadOperator(CreateToken("bad\nder", 1, -3));
            Assert.Equal(expectedMessage, actualMessage);

            try
            {
                JqlParseErrorMessages.BadOperator(null);
                Assert.True(false, "Expected an error to be thrown.");
            }
            catch (ArgumentNullException ignored)
            {
                //expected.
            }
        }

        [Fact]
        public void TestNeedLogicalOperator()
        {
            JqlParseErrorMessage expectedMessage = new JqlParseErrorMessage("jql.parse.logical.operator.eof", -1, -1);
            JqlParseErrorMessage actualMessage = JqlParseErrorMessages.NeedLogicalOperator(CreateEofToken());
            Assert.Equal(expectedMessage, actualMessage);

            expectedMessage = new JqlParseErrorMessage("jql.parse.logical.operator", 5, 3, "5", "3", "b a d");
            actualMessage = JqlParseErrorMessages.NeedLogicalOperator(CreateToken("b\ra\nd", 5, 2));
            Assert.Equal(expectedMessage, actualMessage);

            expectedMessage = new JqlParseErrorMessage("jql.parse.logical.operator", 1, -3, "1", "?", "badder");
            actualMessage = JqlParseErrorMessages.NeedLogicalOperator(CreateToken("badder", 1, -3));
            Assert.Equal(expectedMessage, actualMessage);

            try
            {
                JqlParseErrorMessages.NeedLogicalOperator(null);
                Assert.True(false, "Expected an error to be thrown.");
            }
            catch (ArgumentNullException ignored)
            {
                //expected.
            }
        }

        [Fact]
        public void TestBadOperand()
        {
            JqlParseErrorMessage expectedMessage = new JqlParseErrorMessage("jql.parse.bad.operand.eof", -1, -1);
            JqlParseErrorMessage actualMessage = JqlParseErrorMessages.BadOperand(CreateEofToken());
            Assert.Equal(expectedMessage, actualMessage);

            expectedMessage = new JqlParseErrorMessage("jql.parse.bad.operand", 5, -2829, "5", "?", "bad");
            actualMessage = JqlParseErrorMessages.BadOperand(CreateToken("bad", 5, -2829));
            Assert.Equal(expectedMessage, actualMessage);

            expectedMessage = new JqlParseErrorMessage("jql.parse.bad.operand", -22828, -3, "?", "?", "ba d der");
            actualMessage = JqlParseErrorMessages.BadOperand(CreateToken("ba\rd\nder", -22828, -3));
            Assert.Equal(expectedMessage, actualMessage);

            try
            {
                JqlParseErrorMessages.BadOperand(null);
                Assert.True(false, "Expected an error to be thrown.");
            }
            catch (ArgumentNullException ignored)
            {
                //expected.
            }
        }

        [Fact]
        public void TestBadFunctionArgument()
        {
            JqlParseErrorMessage expectedMessage = new JqlParseErrorMessage("jql.parse.bad.function.argument.eof", -1, -1);
            JqlParseErrorMessage actualMessage = JqlParseErrorMessages.BadFunctionArgument(CreateEofToken());
            Assert.Equal(expectedMessage, actualMessage);

            expectedMessage = new JqlParseErrorMessage("jql.parse.bad.function.argument", 1, 3, "1", "3", "b  ad");
            actualMessage = JqlParseErrorMessages.BadFunctionArgument(CreateToken("b\n\nad", 1, 2));
            Assert.Equal(expectedMessage, actualMessage);

            expectedMessage = new JqlParseErrorMessage("jql.parse.bad.function.argument", -1, -3, "?", "?", "badder");
            actualMessage = JqlParseErrorMessages.BadFunctionArgument(CreateToken("badder", -1, -3));
            Assert.Equal(expectedMessage, actualMessage);

            try
            {
                JqlParseErrorMessages.BadFunctionArgument(null);
                Assert.True(false, "Expected an error to be thrown.");
            }
            catch (ArgumentNullException ignored)
            {
                //expected.
            }
        }

        [Fact]
        public void TestEmptyFunctionArgument()
        {
            JqlParseErrorMessage expectedMessage = new JqlParseErrorMessage("jql.parse.empty.function.argument", -1, -1, "?", "?");
            JqlParseErrorMessage actualMessage = JqlParseErrorMessages.EmptyFunctionArgument(CreateEofToken());
            Assert.Equal(expectedMessage, actualMessage);

            expectedMessage = new JqlParseErrorMessage("jql.parse.empty.function.argument", 1, 3, "1", "3");
            actualMessage = JqlParseErrorMessages.EmptyFunctionArgument(CreateToken("bad", 1, 2));
            Assert.Equal(expectedMessage, actualMessage);

            expectedMessage = new JqlParseErrorMessage("jql.parse.empty.function.argument", -1, -3, "?", "?");
            actualMessage = JqlParseErrorMessages.EmptyFunctionArgument(CreateToken("badder", -1, -3));
            Assert.Equal(expectedMessage, actualMessage);

            try
            {
                JqlParseErrorMessages.EmptyFunctionArgument(null);
                Assert.True(false, "Expected an error to be thrown.");
            }
            catch (ArgumentNullException ignored)
            {
                //expected.
            }
        }

        [Fact]
        public void TestExpectedText()
        {
            JqlParseErrorMessage expectedMessage = new JqlParseErrorMessage("jql.parse.expected.text.eof", -1, -1, "end");
            JqlParseErrorMessage actualMessage = JqlParseErrorMessages.ExpectedText(CreateEofToken(), "end");
            Assert.Equal(expectedMessage, actualMessage);

            expectedMessage = new JqlParseErrorMessage("jql.parse.expected.text", 99, 3, "99", "3", "by", "bad");
            actualMessage = JqlParseErrorMessages.ExpectedText(CreateToken("bad", 99, 2), "by");
            Assert.Equal(expectedMessage, actualMessage);

            expectedMessage = new JqlParseErrorMessage("jql.parse.expected.text", -38, 124, "?", "?", "order", "ba dd er");
            actualMessage = JqlParseErrorMessages.ExpectedText(CreateToken("ba\ndd\ner", -32, 123), "order");
            Assert.Equal(expectedMessage, actualMessage);

            try
            {
                JqlParseErrorMessages.ExpectedText(null, "rjek");
                Assert.True(false, "Expected an error to be thrown.");
            }
            catch (ArgumentNullException ignored)
            {
                //expected.
            }

            try
            {
                JqlParseErrorMessages.ExpectedText(CreateEofToken(), null);
                Assert.True(false, "Expected an error to be thrown.");
            }
            catch (ArgumentNullException ignored)
            {
                //expected.
            }
        }

        [Fact]
        public void TestExpectedText2()
        {
            JqlParseErrorMessage expectedMessage = new JqlParseErrorMessage("jql.parse.expected.text.2.eof", -1, -1, "end", "here");
            JqlParseErrorMessage actualMessage = JqlParseErrorMessages.ExpectedText(CreateEofToken(), "end", "here");
            Assert.Equal(expectedMessage, actualMessage);

            expectedMessage = new JqlParseErrorMessage("jql.parse.expected.text.2", 99, 3, "99", "3", "by", "order", "bad");
            actualMessage = JqlParseErrorMessages.ExpectedText(CreateToken("bad", 99, 2), "by", "order");
            Assert.Equal(expectedMessage, actualMessage);

            expectedMessage = new JqlParseErrorMessage("jql.parse.expected.text.2", -38, 124, "?", "?", "order", "by", "ba  dder");
            actualMessage = JqlParseErrorMessages.ExpectedText(CreateToken("ba\r\ndder", -32, 123), "order", "by");
            Assert.Equal(expectedMessage, actualMessage);

            try
            {
                JqlParseErrorMessages.ExpectedText(null, "rjek", "rgeh");
                Assert.True(false, "Expected an error to be thrown.");
            }
            catch (ArgumentNullException ignored)
            {
                //expected.
            }

            try
            {
                JqlParseErrorMessages.ExpectedText(CreateEofToken(), null, "hjsahdkas");
                Assert.True(false, "Expected an error to be thrown.");
            }
            catch (ArgumentNullException ignored)
            {
                //expected.
            }

            try
            {
                JqlParseErrorMessages.ExpectedText(CreateEofToken(), "hrewhrejw", null);
                Assert.True(false, "Expected an error to be thrown.");
            }
            catch (ArgumentNullException ignored)
            {
                //expected.
            }
        }

        private static IToken CreateEofToken()
        {
            return new CommonToken(CharStreamConstants.EndOfFile);
        }

        private static IToken CreateToken(string text, int line, int position, int type = 10)
        {
            var token = new CommonToken(type)
            {
                Line = line,
                CharPositionInLine = position,
                Text = text
            };
            return token;
        }
    }
}