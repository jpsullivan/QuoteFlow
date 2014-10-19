// For testing out the ANTLR grammer hack code required to generate the
// JqlParser.cs file

using System;
using System.Collections;
using Antlr.Runtime;
using QuoteFlow.Infrastructure.Exceptions;
using QuoteFlow.Models.Search.Jql.Parser;

namespace QuoteFlow.Models.Search.Jql
{
    public class JqlParserTester
    {
        private int operandLevel = 0;
        private bool supportsHistoryPredicate = false;

	protected internal override object recoverFromMismatchedToken(IIntStream input, int ttype, BitArray follow)
	{
		throw new MismatchedTokenException(ttype, input);
	}

	public override object recoverFromMismatchedSet(IIntStream input, RecognitionException e, BitArray follow)
	{
		throw e;
	}

	public override void emitErrorMessage(string msg)
	{
		log.warn(msg);
	}

	/// <summary>
	/// Make sure that the passed token can be turned into a long. In ANTLR there
	/// does not appear to be an easy way to limit numbers to a valid Long range, so
	/// lets do so in Java.
	/// </summary>
	/// <param name="token"> the token to turn into a long. </param>
	/// <returns> the valid long. </returns>
	private long parseLong(Token token)
	{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String text = token.getText();
		string text = token.Text;
		try
		{
			return Convert.ToInt64(text);
		}
		catch (Exception e)
		{
			JqlParseErrorMessage message = JqlParseErrorMessages.illegalNumber(text, token.Line, token.CharPositionInLine);
			throw new RuntimeRecognitionException(message, e);
		}
	}

	private string checkFieldName(Token token)
	{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String text = token.getText();
		string text = token.Text;
		if (StringUtils.isBlank(text))
		{
			reportError(JqlParseErrorMessages.emptyFieldName(token.Line, token.CharPositionInLine), null);
		}
		return text;
	}

	private string checkFunctionName(Token token)
	{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String text = token.getText();
		string text = token.Text;
		if (StringUtils.isBlank(text))
		{
			reportError(JqlParseErrorMessages.emptyFunctionName(token.Line, token.CharPositionInLine), null);
		}
		return text;
	}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void reportError(com.atlassian.jira.jql.parser.JqlParseErrorMessage message, Throwable th) throws RuntimeRecognitionException
	private void reportError(JqlParseErrorMessage message, Exception th)
	{
		throw new RuntimeRecognitionException(message, th);
	}

	catch (RecognitionException e)
	{
		throw e;
	}

    }

}