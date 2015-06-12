using System;
using System.Collections.Generic;
using System.Linq;
using Antlr.Runtime;
using QuoteFlow.Api.Infrastructure.Extensions;
using QuoteFlow.Api.Jql.Parser;
using QuoteFlow.Core.Jql.Util;
using QuoteFlow.Models.Jql.AntlrGen;

namespace QuoteFlow.Core.Jql.Parser
{
	/// <summary>
	/// Factory for <see cref="JqlParseErrorMessage"/> objects.
	/// 
	/// @since v4.0
	/// </summary>
	public class JqlParseErrorMessages
	{
		private JqlParseErrorMessages()
		{
			//This is an abstract factory class, no need to construct it.
		}

		public static JqlParseErrorMessage ReservedWord(string reservedWord, int antlrLine, int antlrColumn)
		{
		    if (reservedWord == null) throw new ArgumentNullException("reservedWord");
		    return CreateMessage("jql.parse.reserved.word", new Position(antlrLine, antlrColumn), NormalizeString(reservedWord));
		}

	    public static JqlParseErrorMessage IllegalEsacpe(string illegalEscape, int antlrLine, int antlrColumn)
		{
			var pos = new Position(antlrLine, antlrColumn);
			string normalizedString = NormalizeString(illegalEscape);
            if (string.IsNullOrEmpty(illegalEscape))
			{
				return CreateMessage("jql.parse.illegal.escape.blank", pos);
			}
		    return CreateMessage("jql.parse.illegal.escape", pos, normalizedString);
		}

		public static JqlParseErrorMessage UnfinishedString(string currentString, int antlrLine, int antlrColumn)
		{
			var pos = new Position(antlrLine, antlrColumn);
			string normalizedString = NormalizeString(currentString);

			if (currentString.IsNullOrEmpty())
			{
				return CreateMessage("jql.parse.unfinished.string.blank", pos);
			}
		    return CreateMessage("jql.parse.unfinished.string", pos, normalizedString);
		}

		public static JqlParseErrorMessage IllegalCharacter(char currentChar, int antlrLine, int antlrColumn)
		{
			var pos = new Position(antlrLine, antlrColumn);
			string escapeChar = JqlStringSupport.EncodeCharacterForce(currentChar);
			string printableChar = GetPrintableCharacter(currentChar);

			return CreateMessage("jql.parse.illegal.character", pos, printableChar, escapeChar);
		}

		public static JqlParseErrorMessage ReservedCharacter(char currentChar, int antlrLine, int antlrColumn)
		{
			var pos = new Position(antlrLine, antlrColumn);
			string escapeChar = JqlStringSupport.EncodeCharacterForce(currentChar);
			string printableChar = GetPrintableCharacter(currentChar);

			return CreateMessage("jql.parse.reserved.character", pos, printableChar, escapeChar);
		}

		public static JqlParseErrorMessage GenericParseError()
		{
			return new JqlParseErrorMessage("jql.parse.unknown.no.pos", -1, -1);
		}

		public static JqlParseErrorMessage GenericParseError(int antlrLine, int antlrColumn)
		{
			var pos = new Position(antlrLine, antlrColumn);
			return CreateMessage("jql.parse.unknown", pos);
		}

		public static JqlParseErrorMessage GenericParseError(IToken token)
		{
		    if (token == null) throw new ArgumentNullException("token");

		    var position = new Position(token);
			if (IsEofToken(token))
			{
				return new JqlParseErrorMessage("jql.parse.unknown.no.pos", position.Line, position.Column);
			}
		    return CreateMessage("jql.parse.unknown", position);
		}

		public static JqlParseErrorMessage IllegalNumber(string number, int antlrLine, int antlrColumn)
		{
		    if (number == null) throw new ArgumentNullException("number");

		    var pos = new Position(antlrLine, antlrColumn);
			return CreateMessage("jql.parse.illegal.number", pos, NormalizeString(number), long.MinValue, long.MaxValue);
		}

		public static JqlParseErrorMessage EmptyFieldName(int antlrLine, int antlrColumn)
		{
			return CreateMessage("jql.parse.empty.field", new Position(antlrLine, antlrColumn));
		}

		public static JqlParseErrorMessage EmptyFunctionName(int antlrLine, int antlrColumn)
		{
			return CreateMessage("jql.parse.empty.function", new Position(antlrLine, antlrColumn));
		}

		public static JqlParseErrorMessage BadFieldName(IToken token)
		{
		    if (token == null) throw new ArgumentNullException("token");

		    var pos = new Position(token);
			if (IsEofToken(token))
			{
				return new JqlParseErrorMessage("jql.parse.no.field.eof", pos.Line, pos.Column);
			}
		    if (token.Type == JqlLexer.LBRACKET)
		    {
		        return CreateMessage("jql.parse.no.cf.field", pos);
		    }
		    return CreateMessage("jql.parse.no.field", pos, NormalizeString(token.Text));
		}

		public static JqlParseErrorMessage BadSortOrder(IToken token)
		{
		    if (token == null) throw new ArgumentNullException("token");

		    var pos = new Position(token);
			if (IsEofToken(token))
			{
				return new JqlParseErrorMessage("jql.parse.no.order.eof", pos.Line, pos.Column);
			}
		    return CreateMessage("jql.parse.no.order", pos, NormalizeString(token.Text));
		}

		public static JqlParseErrorMessage BadOperator(IToken token)
		{
		    if (token == null) throw new ArgumentNullException("token");

		    var pos = new Position(token);
			if (IsEofToken(token))
			{
				return new JqlParseErrorMessage("jql.parse.no.operator.eof", pos.Line, pos.Column);
			}
		    return CreateMessage("jql.parse.no.operator", pos, NormalizeString(token.Text));
		}

		public static JqlParseErrorMessage BadPropertyArgument(IToken token)
		{
			var pos = new Position(token);
			if (IsEofToken(token))
			{
				return new JqlParseErrorMessage("jql.parse.bad.property.id.eof", pos.Line, pos.Column);
			}
		    return CreateMessage("jql.parse.bad.property.id", pos, NormalizeString(token.Text));
		}

		public static JqlParseErrorMessage BadCustomFieldId(IToken token)
		{
			var pos = new Position(token);
			if (IsEofToken(token))
			{
				return new JqlParseErrorMessage("jql.parse.bad.custom.field.id.eof", pos.Line, pos.Column);
			}
		    return CreateMessage("jql.parse.bad.custom.field.id", pos, NormalizeString(token.Text));
		}

		public static JqlParseErrorMessage BadFunctionArgument(IToken token)
		{
		    if (token == null) throw new ArgumentNullException("token");

		    var pos = new Position(token);
			if (IsEofToken(token))
			{
				return new JqlParseErrorMessage("jql.parse.bad.function.argument.eof", pos.Line, pos.Column);
			}
		    return CreateMessage("jql.parse.bad.function.argument", pos, NormalizeString(token.Text));
		}

		public static JqlParseErrorMessage NeedLogicalOperator(IToken token)
		{
		    if (token == null) throw new ArgumentNullException("token");

		    var pos = new Position(token);
			if (IsEofToken(token))
			{
				return new JqlParseErrorMessage("jql.parse.logical.operator.eof", pos.Line, pos.Column);
			}
		    return CreateMessage("jql.parse.logical.operator", pos, NormalizeString(token.Text));
		}

		public static JqlParseErrorMessage BadOperand(IToken token)
		{
		    if (token == null) throw new ArgumentNullException("token");

		    var pos = new Position(token);
			if (IsEofToken(token))
			{
				return new JqlParseErrorMessage("jql.parse.bad.operand.eof", pos.Line, pos.Column);
			}
		    return CreateMessage("jql.parse.bad.operand", pos, NormalizeString(token.Text));
		}

		public static JqlParseErrorMessage EmptyFunctionArgument(IToken token)
		{
		    if (token == null) throw new ArgumentNullException("token");

		    return CreateMessage("jql.parse.empty.function.argument", new Position(token));
		}

	    public static JqlParseErrorMessage ExpectedText(IToken token, string expected)
		{
	        if (token == null) throw new ArgumentNullException("token");
	        if (expected.IsNullOrEmpty()) throw new ArgumentNullException("expected");

	        var pos = new Position(token);
			if (IsEofToken(token))
			{
				return new JqlParseErrorMessage("jql.parse.expected.text.eof", pos.Line, pos.Column, expected);
			}
		    return CreateMessage("jql.parse.expected.text", pos, expected, NormalizeString(token.Text));
		}

		public static JqlParseErrorMessage ExpectedText(IToken token, string expected1, string expected2)
		{
		    if (token == null) throw new ArgumentNullException("token");
		    if (expected1.IsNullOrEmpty()) throw new ArgumentNullException("expected1");
            if (expected2.IsNullOrEmpty()) throw new ArgumentNullException("expected2");

		    var pos = new Position(token);
			if (IsEofToken(token))
			{
				return new JqlParseErrorMessage("jql.parse.expected.text.2.eof", pos.Line, pos.Column, expected1, expected2);
			}
		    return CreateMessage("jql.parse.expected.text.2", pos, expected1, expected2, NormalizeString(token.Text));
		}

		public static JqlParseErrorMessage UnsupportedPredicate(string predicate, string @operator)
		{
			var pos = new Position(0, 0);
			return CreateMessage("jql.parse.predicate.unsupported",pos, @operator, predicate);
		}

		public static JqlParseErrorMessage UnsupportedOperand(string @operator, string operand)
		{
			var pos = new Position(0, 0);
			return CreateMessage("jql.parse.operand.unsupported",pos, @operator, operand);
		}

		private static JqlParseErrorMessage CreateMessage(string key, Position pos, params object[] args)
		{
		    var arguments = new List<object>(args.Length + 2) {pos.LineString, pos.ColumnString};
		    arguments.AddRange(args.ToList());
			return new JqlParseErrorMessage(key, pos.Line, pos.Column, arguments);
		}

		private static string NormalizeString(string argument)
		{
		    return argument != null ? argument.Replace('\n', ' ').Replace('\r', ' ') : null;
		}

	    private static string GetPrintableCharacter(char c)
		{
			//char.UnicodeBlock unicodeBlock = char.UnicodeBlock.of(c);
			if (JqlStringSupport.IsJqlControl(c) || char.IsWhiteSpace(c)) // || unicodeBlock == null || unicodeBlock == char.UnicodeBlock.SPECIALS)
			{
			    return c == '\t' ? "TAB" : string.Format("U+{0:X4}", (int) c);
			}
		    return Convert.ToString(c);
		}

		private static bool IsEofToken(IToken token)
		{
			return token.Type == TokenTypes.EndOfFile;
		}

		private class Position
		{
		    public int Line { get; set; }
		    public int Column { get; set; }

			internal Position(IToken token) : this(token.Line, token.CharPositionInLine)
			{
			}

			internal Position(int line, int column)
			{
			    Line = NormalizeLine(line);
			    Column = NormalizeColumn(column);
			}

		    private static int NormalizeLine(int line)
			{
				return line <= 0 ? - 1 : line;
			}

		    private static int NormalizeColumn(int column)
			{
				return column < 0 ? - 1 : column + 1;
			}

			public virtual string LineString
			{
				get
				{
				    return Line < 0 ? "?" : Convert.ToString(Line);
				}
			}

			public virtual string ColumnString
			{
				get
				{
				    if (Line < 0 || Column < 0)
					{
						return "?";
					}
				    return Convert.ToString(Column);
				}
			}
		}
	}
}