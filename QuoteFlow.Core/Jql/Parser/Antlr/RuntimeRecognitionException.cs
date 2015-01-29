using System;
using Antlr.Runtime;
using QuoteFlow.Api.Jql.Parser;

namespace QuoteFlow.Core.Jql.Parser.Antlr
{
    public class RuntimeRecognitionException : RecognitionException
    {
        public JqlParseErrorMessage ParseErrorMessage { get; set; }

        internal RuntimeRecognitionException(JqlParseErrorMessage errorMessage)
            : base()
        {
            ParseErrorMessage = errorMessage;
        }

        internal RuntimeRecognitionException(JqlParseErrorMessage errorMessage, Exception cause)
            : base(errorMessage.ToString(), cause)
        {
            ParseErrorMessage = errorMessage;
        }
    }
}