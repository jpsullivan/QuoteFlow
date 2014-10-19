using System;
using System.Collections.Generic;
using Antlr.Runtime;
using QuoteFlow.Models.Search.Jql.Util;

namespace QuoteFlow.Models.Search.Jql.Parser
{
	/// <summary>
	/// Factory for <seealso cref="JqlParseErrorMessage"/> objects.
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
			return CreateMessage("jql.parse.reserved.word", new Position(antlrLine, antlrColumn), normalizeString(reservedWord));
		}

		public static JqlParseErrorMessage IllegalEsacpe(string illegalEscape, int antlrLine, int antlrColumn)
		{
			Position pos = new Position(antlrLine, antlrColumn);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String normalizedString = normalizeString(illegalEscape);
			string normalizedString = normalizeString(illegalEscape);
			if (StringUtils.isBlank(illegalEscape))
			{
				return CreateMessage("jql.parse.illegal.escape.blank", pos);
			}
			else
			{
				return CreateMessage("jql.parse.illegal.escape", pos, normalizedString);
			}
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static JqlParseErrorMessage unfinishedString(final String currentString, final int antlrLine, final int antlrColumn)
		public static JqlParseErrorMessage unfinishedString(string currentString, int antlrLine, int antlrColumn)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Position pos = new Position(antlrLine, antlrColumn);
			Position pos = new Position(antlrLine, antlrColumn);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String normalizedString = normalizeString(currentString);
			string normalizedString = normalizeString(currentString);

			if (StringUtils.isBlank(currentString))
			{
				return CreateMessage("jql.parse.unfinished.string.blank", pos);
			}
			else
			{
				return CreateMessage("jql.parse.unfinished.string", pos, normalizedString);
			}
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static JqlParseErrorMessage illegalCharacter(final char currentChar, final int antlrLine, final int antlrColumn)
		public static JqlParseErrorMessage illegalCharacter(char currentChar, int antlrLine, int antlrColumn)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Position pos = new Position(antlrLine, antlrColumn);
			Position pos = new Position(antlrLine, antlrColumn);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String escapeChar = com.atlassian.jira.jql.util.JqlStringSupportImpl.encodeCharacterForce(currentChar);
			string escapeChar = JqlStringSupportImpl.encodeCharacterForce(currentChar);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String printableChar = getPrintableCharacter(currentChar);
			string printableChar = getPrintableCharacter(currentChar);

			return CreateMessage("jql.parse.illegal.character", pos, printableChar, escapeChar);
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static JqlParseErrorMessage reservedCharacter(final char currentChar, final int antlrLine, final int antlrColumn)
		public static JqlParseErrorMessage reservedCharacter(char currentChar, int antlrLine, int antlrColumn)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Position pos = new Position(antlrLine, antlrColumn);
			Position pos = new Position(antlrLine, antlrColumn);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String escapeChar = com.atlassian.jira.jql.util.JqlStringSupportImpl.encodeCharacterForce(currentChar);
			string escapeChar = JqlStringSupportImpl.encodeCharacterForce(currentChar);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String printableChar = getPrintableCharacter(currentChar);
			string printableChar = getPrintableCharacter(currentChar);

			return CreateMessage("jql.parse.reserved.character", pos, printableChar, escapeChar);
		}

		public static JqlParseErrorMessage genericParseError()
		{
			return new JqlParseErrorMessage("jql.parse.unknown.no.pos", -1, -1);
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static JqlParseErrorMessage genericParseError(final int antlrLine, final int antlrColumn)
		public static JqlParseErrorMessage genericParseError(int antlrLine, int antlrColumn)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Position pos = new Position(antlrLine, antlrColumn);
			Position pos = new Position(antlrLine, antlrColumn);
			return CreateMessage("jql.parse.unknown", pos);
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static JqlParseErrorMessage genericParseError(final org.antlr.runtime.Token token)
		public static JqlParseErrorMessage genericParseError(Token token)
		{
			notNull("token", token);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Position position = new Position(token);
			Position position = new Position(token);
			if (isEofToken(token))
			{
				return new JqlParseErrorMessage("jql.parse.unknown.no.pos", position.Line, position.Column);
			}
			else
			{
				return CreateMessage("jql.parse.unknown", position);
			}
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static JqlParseErrorMessage illegalNumber(final String number, final int antlrLine, final int antlrColumn)
		public static JqlParseErrorMessage illegalNumber(string number, int antlrLine, int antlrColumn)
		{
			notBlank("number", number);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Position pos = new Position(antlrLine, antlrColumn);
			Position pos = new Position(antlrLine, antlrColumn);
			return CreateMessage("jql.parse.illegal.number", pos, normalizeString(number), long.MinValue, long.MaxValue);
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static JqlParseErrorMessage emptyFieldName(final int antlrLine, final int antlrColumn)
		public static JqlParseErrorMessage emptyFieldName(int antlrLine, int antlrColumn)
		{
			return CreateMessage("jql.parse.empty.field", new Position(antlrLine, antlrColumn));
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static JqlParseErrorMessage emptyFunctionName(final int antlrLine, final int antlrColumn)
		public static JqlParseErrorMessage emptyFunctionName(int antlrLine, int antlrColumn)
		{
			return CreateMessage("jql.parse.empty.function", new Position(antlrLine, antlrColumn));
		}

		public static JqlParseErrorMessage badFieldName(Token token)
		{
			notNull("token", token);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Position pos = new Position(token);
			Position pos = new Position(token);
			if (isEofToken(token))
			{
				return new JqlParseErrorMessage("jql.parse.no.field.eof", pos.Line, pos.Column);
			}
			else
			{
				if (token.Type == JqlLexer.LBRACKET)
				{
					return CreateMessage("jql.parse.no.cf.field", pos);
				}
				else
				{
					return CreateMessage("jql.parse.no.field", pos, normalizeString(token.Text));
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static JqlParseErrorMessage badSortOrder(final org.antlr.runtime.Token token)
		public static JqlParseErrorMessage badSortOrder(Token token)
		{
			notNull("token", token);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Position pos = new Position(token);
			Position pos = new Position(token);
			if (isEofToken(token))
			{
				return new JqlParseErrorMessage("jql.parse.no.order.eof", pos.Line, pos.Column);
			}
			else
			{
				return CreateMessage("jql.parse.no.order", pos, normalizeString(token.Text));
			}
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static JqlParseErrorMessage badOperator(final org.antlr.runtime.Token token)
		public static JqlParseErrorMessage badOperator(Token token)
		{
			notNull("token", token);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Position pos = new Position(token);
			Position pos = new Position(token);
			if (isEofToken(token))
			{
				return new JqlParseErrorMessage("jql.parse.no.operator.eof", pos.Line, pos.Column);
			}
			else
			{
				return CreateMessage("jql.parse.no.operator", pos, normalizeString(token.Text));
			}
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static JqlParseErrorMessage badPropertyArgument(final org.antlr.runtime.Token token)
		public static JqlParseErrorMessage badPropertyArgument(Token token)
		{
			notNull("token", token);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Position pos = new Position(token);
			Position pos = new Position(token);
			if (isEofToken(token))
			{
				return new JqlParseErrorMessage("jql.parse.bad.property.id.eof", pos.Line, pos.Column);
			}
			else
			{
				return CreateMessage("jql.parse.bad.property.id", pos, normalizeString(token.Text));
			}
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static JqlParseErrorMessage badCustomFieldId(final org.antlr.runtime.Token token)
		public static JqlParseErrorMessage badCustomFieldId(Token token)
		{
			notNull("token", token);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Position pos = new Position(token);
			Position pos = new Position(token);
			if (isEofToken(token))
			{
				return new JqlParseErrorMessage("jql.parse.bad.custom.field.id.eof", pos.Line, pos.Column);
			}
			else
			{
				return CreateMessage("jql.parse.bad.custom.field.id", pos, normalizeString(token.Text));
			}
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static JqlParseErrorMessage badFunctionArgument(final org.antlr.runtime.Token token)
		public static JqlParseErrorMessage badFunctionArgument(Token token)
		{
			notNull("token", token);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Position pos = new Position(token);
			Position pos = new Position(token);
			if (isEofToken(token))
			{
				return new JqlParseErrorMessage("jql.parse.bad.function.argument.eof", pos.Line, pos.Column);
			}
			else
			{
				return CreateMessage("jql.parse.bad.function.argument", pos, normalizeString(token.Text));
			}
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static JqlParseErrorMessage needLogicalOperator(final org.antlr.runtime.Token token)
		public static JqlParseErrorMessage needLogicalOperator(Token token)
		{
			notNull("token", token);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Position pos = new Position(token);
			Position pos = new Position(token);
			if (isEofToken(token))
			{
				return new JqlParseErrorMessage("jql.parse.logical.operator.eof", pos.Line, pos.Column);
			}
			else
			{
				return CreateMessage("jql.parse.logical.operator", pos, normalizeString(token.Text));
			}
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static JqlParseErrorMessage badOperand(final org.antlr.runtime.Token token)
		public static JqlParseErrorMessage badOperand(Token token)
		{
			notNull("token", token);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Position pos = new Position(token);
			Position pos = new Position(token);
			if (isEofToken(token))
			{
				return new JqlParseErrorMessage("jql.parse.bad.operand.eof", pos.Line, pos.Column);
			}
			else
			{
				return CreateMessage("jql.parse.bad.operand", pos, normalizeString(token.Text));
			}
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static JqlParseErrorMessage emptyFunctionArgument(final org.antlr.runtime.Token token)
		public static JqlParseErrorMessage emptyFunctionArgument(Token token)
		{
			notNull("token", token);

			return CreateMessage("jql.parse.empty.function.argument", new Position(token));
		}


//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static JqlParseErrorMessage expectedText(final org.antlr.runtime.Token token, String expected)
		public static JqlParseErrorMessage expectedText(Token token, string expected)
		{
			notNull("token", token);
			notBlank("expected", expected);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Position pos = new Position(token);
			Position pos = new Position(token);
			if (isEofToken(token))
			{
				return new JqlParseErrorMessage("jql.parse.expected.text.eof", pos.Line, pos.Column, expected);
			}
			else
			{
				return CreateMessage("jql.parse.expected.text", pos, expected, normalizeString(token.Text));
			}
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static JqlParseErrorMessage expectedText(final org.antlr.runtime.Token token, String expected1, String expected2)
		public static JqlParseErrorMessage expectedText(Token token, string expected1, string expected2)
		{
			notNull("token", token);
			notBlank("expected1", expected1);
			notBlank("expected2", expected2);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Position pos = new Position(token);
			Position pos = new Position(token);
			if (isEofToken(token))
			{
				return new JqlParseErrorMessage("jql.parse.expected.text.2.eof", pos.Line, pos.Column, expected1, expected2);
			}
			else
			{
				return CreateMessage("jql.parse.expected.text.2", pos, expected1, expected2, normalizeString(token.Text));
			}
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static JqlParseErrorMessage unsupportedPredicate(final String predicate, final String operator)
		public static JqlParseErrorMessage unsupportedPredicate(string predicate, string @operator)
		{
			notNull("predicate", predicate);
			notBlank("operator", @operator);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Position pos = new Position(0, 0);
			Position pos = new Position(0, 0);
			return CreateMessage("jql.parse.predicate.unsupported",pos, @operator, predicate);
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static JqlParseErrorMessage unsupportedOperand(final String operator, final String operand)
		public static JqlParseErrorMessage unsupportedOperand(string @operator, string operand)
		{
			notNull("operand", operand);
			notBlank("operator", @operator);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Position pos = new Position(0, 0);
			Position pos = new Position(0, 0);
			return CreateMessage("jql.parse.operand.unsupported",pos, @operator, operand);
		}

		private static JqlParseErrorMessage CreateMessage(string key, Position pos, params object[] args)
		{
			IList<object> arguments = new List<object>(args.Length + 2);
			arguments.Add(pos.LineString);
			arguments.Add(pos.ColumnString);
			arguments.AddRange(Arrays.asList(args));
			return new JqlParseErrorMessage(key, pos.Line, pos.Column, arguments);
		}

		private static string normalizeString(string argument)
		{
			if (argument != null)
			{
				return argument.Replace('\n', ' ').Replace('\r', ' ');
			}
			else
			{
				return argument;
			}
		}

		private static string getPrintableCharacter(char c)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Character.UnicodeBlock unicodeBlock = Character.UnicodeBlock.of(c);
			char.UnicodeBlock unicodeBlock = char.UnicodeBlock.of(c);
			if (JqlStringSupport.IsJqlControl(c) || char.IsWhiteSpace(c) || unicodeBlock == null || unicodeBlock == char.UnicodeBlock.SPECIALS)
			{
			    return c == '\t' ? string.Format("TAB") : string.Format("U+{0:X4}", (int) c);
			}
		    return Convert.ToString(c);
		}

		private static bool IsEofToken(IToken token)
		{
			return token.Type == Token.EOF;
		}

		private class Position
		{
            private int Line { get; set; }
            private int Column { get; set; }

			internal Position(IToken token) : this(IToken.Line, token.CharPositionInLine)
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