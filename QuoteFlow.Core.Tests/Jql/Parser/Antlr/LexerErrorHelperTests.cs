using System;
using Antlr.Runtime;
using QuoteFlow.Api.Jql.Parser;
using QuoteFlow.Core.Jql.AntlrGen;
using QuoteFlow.Core.Jql.Parser;
using QuoteFlow.Core.Jql.Parser.Antlr;
using Xunit;

namespace QuoteFlow.Core.Tests.Jql.Parser.Antlr
{
    public class LexerErrorHelperTests
    {
        public class TheCtor
        {
            [Fact]
            public void ProperlyAssignsValues()
            {
                var stream = new ANTLRStringStream("theStream");
                var position = new AntlrPosition(JqlParser.AMPER, stream);
                var helper = new LexerErrorHelper(stream, position);

                Assert.Same(stream, helper.Stream);
                Assert.Same(position, helper.Position);
            }

            [Fact]
            public void Throws_If_StreamIsNull()
            {
                var ex = Assert.Throws<ArgumentNullException>(() => new LexerErrorHelper(null, null));
                Assert.Equal("stream", ex.ParamName);
            }
        }

        public class TheHandlerErrorMethod
        {
            [Fact]
            public void UnknownError()
            {
                ANTLRStringStream stream = new ANTLRStringStream("t\nhis is a very long test string");
                stream.Seek(5);

                var helper = new LexerErrorHelper(stream, new AntlrPosition(JqlLexer.AMPER_AMPER, stream));
                var expectedMessage = JqlParseErrorMessages.GenericParseError(2, 3);
                var expectedCause = new RecognitionException();

                AssertResult(helper, expectedMessage, expectedCause);
            }

            [Fact]
            public void NoPosition()
            {
                var stream = new ANTLRStringStream("t\nhis is a very long test string");
                stream.Seek(5);

                var helper = new LexerErrorHelper(stream, null);
                var expectedMessage = JqlParseErrorMessages.GenericParseError(2, 3);
                var expectedCause = new RecognitionException();

                AssertResult(helper, expectedMessage, expectedCause);
            }

            [Fact]
            public void HandleEscape()
            {
                // Check unfinished escape sequence.
                const string input = "this\\";
                ANTLRStringStream stream = new ANTLRStringStream(input);
                stream.Seek(input.Length - 1);
                var antlrPosition = new AntlrPosition(JqlLexer.ESCAPE, stream);
                var helper = new LexerErrorHelper(stream, antlrPosition);
                var expectedMessage = JqlParseErrorMessages.IllegalEscape(null, 1, 4);
                var expectedCause = new RecognitionException();

                AssertResult(helper, expectedMessage, expectedCause);

                // Check an illegal escape sequence.
                stream = new ANTLRStringStream("this\\n");
                // Set the escape start for the position.
                stream.Seek(4);
                antlrPosition = new AntlrPosition(JqlLexer.ESCAPE, stream);
                // Move to the first error character.
                stream.Consume();
                helper = new LexerErrorHelper(stream, antlrPosition);

                expectedMessage = JqlParseErrorMessages.IllegalEscape("\\n", 1, 4);
                expectedCause = new RecognitionException();

                AssertResult(helper, expectedMessage, expectedCause);
            }

            [Fact]
            public void StringErrorEof()
            {
                // check an unfinished string.
                string input = "this = \"";
                var stream = new ANTLRStringStream(input);

                // the string position starts before the end.
                stream.Seek(input.Length - 1);
                var antlrPosition = new AntlrPosition(JqlLexer.SQUOTE_STRING, stream);
                // move to the EOF.
                stream.Consume();

                var helper = new LexerErrorHelper(stream, antlrPosition);
                var expectedMessage = JqlParseErrorMessages.UnfinishedString("", 1, 7);
                var expectedCause = new RecognitionException();

                AssertResult(helper, expectedMessage, expectedCause);

                // check an unfinished string to EOF.
                input = "\"this";
                stream = new ANTLRStringStream(input);

                // string starts at the start.
                antlrPosition = new AntlrPosition(JqlLexer.SQUOTE_STRING, stream);
                stream.Seek(input.Length);

                helper = new LexerErrorHelper(stream, antlrPosition);
                expectedMessage = JqlParseErrorMessages.UnfinishedString("this", 1, 0);
                expectedCause = new RecognitionException();

                AssertResult(helper, expectedMessage, expectedCause);
            }

            [Fact]
            public void StringErrorNewLine()
            {
                // check an unfinished string on newline.
                string input = "this=\"\nblah";
                var stream = new ANTLRStringStream(input);

                // the string position starts before the end.
                stream.Seek(5);
                var antlrPosition = new AntlrPosition(JqlLexer.QUOTE_STRING, stream);
                // move to the EOF.
                stream.Consume();

                var helper = new LexerErrorHelper(stream, antlrPosition);
                var expectedMessage = JqlParseErrorMessages.UnfinishedString("", 1, 5);
                var expectedCause = new RecognitionException();

                AssertResult(helper, expectedMessage, expectedCause);

                // check an unfinished string on newline with trailing text.
                input = "\"this\nistheend";
                stream = new ANTLRStringStream(input);

                // string starts at the start.
                antlrPosition = new AntlrPosition(JqlLexer.SQUOTE_STRING, stream);
                stream.Seek(5);

                helper = new LexerErrorHelper(stream, antlrPosition);
                expectedMessage = JqlParseErrorMessages.UnfinishedString("this", 1, 0);
                expectedCause = new RecognitionException();

                AssertResult(helper, expectedMessage, expectedCause);
            }

            [Fact]
            public void StringIllegalCharacter()
            {
                //Check a string with illegal characters.
                String input = "wwwww\uffffblah";
                ANTLRStringStream stream = new ANTLRStringStream(input);

                stream.Seek(5);
                AntlrPosition antlrPosition = new AntlrPosition(JqlLexer.QUOTE_STRING, stream);

                LexerErrorHelper helper = new LexerErrorHelper(stream, antlrPosition);
                JqlParseErrorMessage expectedMessage = JqlParseErrorMessages.IllegalCharacter('\uffff', 1, 5);
                RecognitionException expectedCause = new RecognitionException();

                AssertResult(helper, expectedMessage, expectedCause);
            }

            [Fact]
            public void CharacterString()
            {
                //Check a that ends with a quote.
                string input = "comment=\"";
                ANTLRStringStream stream = new ANTLRStringStream(input);

                stream.Seek(8);
                AntlrPosition antlrPosition = new AntlrPosition(JqlLexer.ERRORCHAR, stream);

                LexerErrorHelper helper = new LexerErrorHelper(stream, antlrPosition);
                JqlParseErrorMessage expectedMessage = JqlParseErrorMessages.UnfinishedString(null, 1, 8);
                RecognitionException expectedCause = new RecognitionException();

                AssertResult(helper, expectedMessage, expectedCause);

                //Check what happens when we have quote followed by an illegal character.
                input = "comment=\"\ufdd4";
                stream = new ANTLRStringStream(input);

                stream.Seek(8);
                antlrPosition = new AntlrPosition(JqlLexer.ERRORCHAR, stream);

                helper = new LexerErrorHelper(stream, antlrPosition);
                expectedMessage = JqlParseErrorMessages.IllegalCharacter('\ufdd4', 1, 9);
                expectedCause = new RecognitionException();

                AssertResult(helper, expectedMessage, expectedCause);

                //Check what happens when we have quote followed by a newline.
                input = "comment=\"\r";
                stream = new ANTLRStringStream(input);

                stream.Seek(8);
                antlrPosition = new AntlrPosition(JqlLexer.ERRORCHAR, stream);

                helper = new LexerErrorHelper(stream, antlrPosition);
                expectedMessage = JqlParseErrorMessages.UnfinishedString(null, 1, 8);
                expectedCause = new RecognitionException();

                AssertResult(helper, expectedMessage, expectedCause);
            }

            [Fact]
            public void CharacterEscape()
            {
                //Check a query that ends with an empty escape.
                String input = "comment=\\";
                ANTLRStringStream stream = new ANTLRStringStream(input);

                stream.Seek(8);
                AntlrPosition antlrPosition = new AntlrPosition(JqlLexer.ERRORCHAR, stream);

                LexerErrorHelper helper = new LexerErrorHelper(stream, antlrPosition);
                JqlParseErrorMessage expectedMessage = JqlParseErrorMessages.IllegalEscape(null, 1, 8);
                RecognitionException expectedCause = new RecognitionException();

                AssertResult(helper, expectedMessage, expectedCause);

                //Check a query that ends with an illegal escape.
                input = "comment=\\c";
                stream = new ANTLRStringStream(input);

                stream.Seek(8);
                antlrPosition = new AntlrPosition(JqlLexer.ERRORCHAR, stream);

                helper = new LexerErrorHelper(stream, antlrPosition);
                expectedMessage = JqlParseErrorMessages.IllegalEscape(@"\c", 1, 8);
                expectedCause = new RecognitionException();

                AssertResult(helper, expectedMessage, expectedCause);
            }

            [Fact]
            public void CharacterIllegalCharacter()
            {
                // Check a query that ends with an empty escape.
                string input = "comment=bad\uffffchatacer";
                var stream = new ANTLRStringStream(input);

                stream.Seek(11);
                var antlrPosition = new AntlrPosition(JqlLexer.ERRORCHAR, stream);

                var helper = new LexerErrorHelper(stream, antlrPosition);
                var expectedMessage = JqlParseErrorMessages.IllegalCharacter('\uffff', 1, 11);
                var expectedCause = new RecognitionException();

                AssertResult(helper, expectedMessage, expectedCause);
            }

            [Fact]
            public void Reserved()
            {
                String input = @"chat#acer";
                ANTLRStringStream stream = new ANTLRStringStream(input);

                stream.Seek(4);
                AntlrPosition antlrPosition = new AntlrPosition(JqlLexer.ERROR_RESERVED, stream);

                LexerErrorHelper helper = new LexerErrorHelper(stream, antlrPosition);
                JqlParseErrorMessage expectedMessage = JqlParseErrorMessages.ReservedCharacter('#', 1, 4);

                AssertResult(helper, expectedMessage, null);
            }

            private void AssertResult(LexerErrorHelper helper, JqlParseErrorMessage expectedMessage, RecognitionException expectedCause)
            {
                try
                {
                    helper.HandleError(expectedCause);
                    Assert.True(false, "Expected exception");
                }
                catch (RuntimeRecognitionException ex)
                {
                    Assert.Equal(expectedMessage, ex.ParseErrorMessage);
                    Assert.Same(expectedCause, ex.InnerException);
                }
            }
        }
    }
}
