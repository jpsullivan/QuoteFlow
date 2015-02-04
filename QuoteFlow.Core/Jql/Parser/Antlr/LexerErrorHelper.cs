using System;
using Antlr.Runtime;
using QuoteFlow.Api.Jql.Parser;
using QuoteFlow.Models.Jql.AntlrGen;

namespace QuoteFlow.Core.Jql.Parser.Antlr
{
    /// <summary>
    /// Helps with Jql Lexer error handling.
    /// </summary>
    public class LexerErrorHelper
    {
        public ICharStream Stream { get; set; }
        public AntlrPosition Position { get; set; }

        public LexerErrorHelper(ICharStream stream, AntlrPosition position)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            Stream = stream;
            Position = position;
        }

        public virtual void HandleError(RecognitionException re)
        {
            JqlParseErrorMessage message;
            if (Position == null)
            {
                message = JqlParseErrorMessages.GenericParseError(Stream.Line, Stream.CharPositionInLine);
            }
            else
            {
                switch (Position.TokenType)
                {
                    case JqlLexer.ESCAPE:
                        message = HandleEscape();
                        break;
                    case JqlLexer.SQUOTE_STRING:
                    case JqlLexer.QUOTE_STRING:
                        message = HandleStringError();
                        break;
                    case JqlLexer.ERRORCHAR:
                        message = HandleErrorCharacter();
                        break;
                    case JqlLexer.ERROR_RESERVED:
                        message = HandleReservedCharacter();
                        break;
                    default:
                        message = JqlParseErrorMessages.GenericParseError(Position.LinePosition, Position.CharPosition);
                        break;
                }
            }
            throw new RuntimeRecognitionException(message, re);
        }

        /// <summary>
        /// This is called when ANTLR finds a character that the grammar does not recognise. The grammar
        /// lexer uses a DFA to decide if a character is in error or not. This can mean that legal characters
        /// come as an error because they do not form a valid token. For example, the JQL 'comment ~ "' will
        /// produce an ERRORCHAR token for the quote because the character after is not a valid string character.
        /// Because of this we have to do some extra checks here.
        /// </summary>
        /// <returns></returns>
        private JqlParseErrorMessage HandleErrorCharacter()
        {
            JqlParseErrorMessage message;
            var currentChar = (char) Stream.LT(1);
            if (IsQuote(currentChar))
            {
                //This can happen when the quote is at the end of the string (e.g. comment ~ ")

                //is the next token the EOF.
                bool nextEof = Stream.LT(2) == CharStreamConstants.EndOfFile;
                if (nextEof)
                {
                    message = JqlParseErrorMessages.UnfinishedString(null, Position.LinePosition, Position.CharPosition);
                }
                else
                {
                    // we need to consume to get an accurate error line and position.
                    int marker = Stream.Mark();
                    Stream.Consume();
                    var nextChar = (char) Stream.LT(1);

                    if (IsNewLine(nextChar))
                    {
                        message = JqlParseErrorMessages.UnfinishedString(null, Position.LinePosition, Position.CharPosition);
                    }
                    else
                    {
                        message = JqlParseErrorMessages.IllegalCharacter(nextChar, Stream.Line, Stream.CharPositionInLine);
                    }
                    Stream.Rewind(marker);
                }
            }
            else if (IsEscape(currentChar))
            {
                // This can happen (e.g. comment ~ "\)

                //is the next token EOF.
                bool nextEof = Stream.LT(2) == CharStreamConstants.EndOfFile;
                string text = nextEof ? null : Stream.Substring(Position.Index, Index + 1 - (Position.Index));
                message = JqlParseErrorMessages.IllegalEsacpe(text, Position.LinePosition, Position.CharPosition);
            }
            else
            {
                // Assume that it is an illegal character.
                message = JqlParseErrorMessages.IllegalCharacter(currentChar, Position.LinePosition, Position.CharPosition);
            }

            return message;
        }

        /*
		 * Called when we see an reserved character that is not escaped. 
		 */
		private JqlParseErrorMessage HandleReservedCharacter()
		{
			var currentChar = (char) Stream.LT(1);
			return JqlParseErrorMessages.ReservedCharacter(currentChar, Position.LinePosition, Position.CharPosition);
		}

        /// <summary>
        /// Called when we get an error when trying to tokenise a quoted or single quoted string.
        /// Three main errors can occur here:
        /// 1. There is an unfinished string before the eof.
        /// 2. There is a newline in a string.
        /// 3. There is some kind of illegal character.
        /// </summary>
        /// <returns></returns>
		private JqlParseErrorMessage HandleStringError()
		{
			JqlParseErrorMessage message;
			int currentInt = Stream.LT(1);
            if (currentInt == CharStreamConstants.EndOfFile)
			{
				// End the query without closing a string.
				string text = Stream.Substring(Position.Index + 1, Index - 1 - (Position.Index + 1));
				message = JqlParseErrorMessages.UnfinishedString(text, Position.LinePosition, Position.CharPosition);
			}
			else
			{
			    var currentChar = (char) currentInt;
				if (IsNewLine(currentChar))
				{
					//End the line without closing a string.
					string text = Stream.Substring(Position.Index + 1, Index - 1 - (Position.Index + 1));
					message = JqlParseErrorMessages.UnfinishedString(text, Position.LinePosition, Position.CharPosition);
				}
				else
				{
					//Some form of illegal character in the string.
					message = JqlParseErrorMessages.IllegalCharacter(currentChar, Stream.Line, Stream.CharPositionInLine);
				}
			}
			return message;
		}

        /// <summary>
        /// Called when we see an escape that it incorrect (e.g. bad = "ththt\c"). This method is not called
        /// when you try to start a string with an illegal character (e.g. bad = \c) because ANTLR detects
        /// that it is not a valid start to a string and considers it an error character (i.e. ERRORCHAR).
        /// </summary>
        /// <returns></returns>
		private JqlParseErrorMessage HandleEscape()
		{
            int index = Stream.LT(1) == CharStreamConstants.EndOfFile ? Index - 1 : Index;
            string text = index <= Position.Index ? null : Stream.Substring(Position.Index, index - (Position.Index));
			//We have some sort of escaping error.
			return JqlParseErrorMessages.IllegalEsacpe(text, Position.LinePosition, Position.CharPosition);
		}

		private static bool IsNewLine(char ch)
		{
			return ch == '\r' || ch == '\n';
		}

		private static bool IsQuote(char ch)
		{
			return ch == '\'' || ch == '"';
		}

		private static bool IsEscape(char ch)
		{
			return ch == '\\';
		}

		private int Index
		{
			get { return Stream.Index; }
		}
	}
}