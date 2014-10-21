using System;
using QuoteFlow.Models.Search.Jql.Parser;

namespace QuoteFlow.Infrastructure.Exceptions
{
    /// <summary>
    /// Thrown when an error occurs while parsing a JQL string.
    /// </summary>
    public class JqlParseException : Exception
    {
        public JqlParseErrorMessage ParseErrorMessage { get; set; }

        public JqlParseException(JqlParseErrorMessage errorMessage)
        {
            ParseErrorMessage = errorMessage;
        }

        public JqlParseException(JqlParseErrorMessage errorMessage, string message)
            : base(message)
        {
            ParseErrorMessage = errorMessage;
        }

        public JqlParseException(JqlParseErrorMessage errorMessage, Exception cause)
            : base(cause.Message)
        {
            ParseErrorMessage = errorMessage;
        }

        public virtual int LineNumber
        {
            get
            {
                return ParseErrorMessage == null ? -1 : ParseErrorMessage.LineNumber;
            }
        }

        public virtual int ColumnNumber
        {
            get
            {
                return ParseErrorMessage == null ? -1 : ParseErrorMessage.ColumnNumber;
            }
        }
    }
}