// For testing out the ANTLR grammer hack code required to generate the
// JqlParser.cs file

using System;
using Antlr.Runtime;
using QuoteFlow.Api.Jql.Parser;
using QuoteFlow.Core.Jql.Antlr;
using QuoteFlow.Core.Jql.Parser;

namespace QuoteFlow.Core.Jql
{
    public class JqlParserTester : global::Antlr.Runtime.Parser
    {
        private int operandLevel = 0;
        private bool supportsHistoryPredicate = false;

        #region Do not include these

        public JqlParserTester(ITokenStream input) : base(input)
        {
        }

        public JqlParserTester(ITokenStream input, RecognizerSharedState state) : base(input, state)
        {
        }

        #endregion

        protected override object RecoverFromMismatchedToken(IIntStream input, int ttype, BitSet follow)
        {
            throw new MismatchedTokenException(ttype, input);
        }

        public override object RecoverFromMismatchedSet(IIntStream input, RecognitionException e, BitSet follow)
        {
            throw e;
        }

        public override void EmitErrorMessage(string msg)
        {
            base.EmitErrorMessage(msg);
        }

        /// <summary>
        /// Make sure that the passed token can be turned into a long. In ANTLR there
        /// does not appear to be an easy way to limit numbers to a valid Long range, so
        /// lets do so in Java.
        /// </summary>
        /// <param name="token"> the token to turn into a long. </param>
        /// <returns> the valid long. </returns>
        private long ParseLong(IToken token)
        {
            string text = token.Text;
            try
            {
                return Convert.ToInt64(text);
            }
            catch (Exception e)
            {
                var message = JqlParseErrorMessages.IllegalNumber(text, token.Line, token.CharPositionInLine);
                throw new RuntimeRecognitionException(message, e);
            }
        }

        private string CheckFieldName(IToken token)
        {
            string text = token.Text;
            if (string.IsNullOrEmpty(text))
            {
                ReportError(JqlParseErrorMessages.EmptyFieldName(token.Line, token.CharPositionInLine), null);
            }
            return text;
        }

        private string CheckFunctionName(IToken token)
        {
            string text = token.Text;
            if (string.IsNullOrEmpty(text))
            {
                ReportError(JqlParseErrorMessages.EmptyFunctionName(token.Line, token.CharPositionInLine), null);
            }
            return text;
        }

        private void ReportError(JqlParseErrorMessage message, Exception th)
        {
            throw new RuntimeRecognitionException(message, th);
        }

// Uncomment when pasting into jql.g
//    @rulecatch
//    catch (RecognitionException ex)
//	{
//	    throw ex;
//	}
    }
}