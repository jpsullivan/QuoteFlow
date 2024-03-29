﻿using System;
using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Api.Infrastructure.Extensions;

namespace QuoteFlow.Api.Jql.Parser
{
	/// <summary>
	/// Represents a parse error message from the JqlParser. Internally contains a i18n key, its arguments and the position
	/// of the error.
	/// </summary>
	public sealed class JqlParseErrorMessage
	{
	    public string Key { get; set; }
	    public IList<object> Arguments { get; set; } 
        public int LineNumber { get; set; }
        public int ColumnNumber { get; set; }

	    public JqlParseErrorMessage(string key, int lineNumber, int columnNumber)
	    {
            Key = key;
            Arguments = new List<object>();
            LineNumber = lineNumber <= 0 ? -1 : lineNumber;
            ColumnNumber = columnNumber <= 0 ? -1 : columnNumber;
	    }

	    public JqlParseErrorMessage(string key, int lineNumber, int columnNumber, string argument)
	    {
            Key = key;
	        Arguments = new List<object> {argument};
            LineNumber = lineNumber <= 0 ? -1 : lineNumber;
            ColumnNumber = columnNumber <= 0 ? -1 : columnNumber;
	    }

		public JqlParseErrorMessage(string key, int lineNumber, int columnNumber, IList<object> arguments)
		{
		    if (arguments == null) throw new ArgumentNullException(nameof(arguments));
		    if (!arguments.Any()) throw new ArgumentException("Arguments cannot be empty", nameof(arguments));
		    if (arguments.Any(argument => argument == null))
		    {
		        throw new ArgumentNullException("Arguments cannot contain any nulls", "arguments");
		    }

		    if (key.IsNullOrEmpty())
		    {
		        throw new ArgumentException("Key cannot be empty");
		    }

			Key = key;
		    Arguments = new List<object>(arguments);
			LineNumber = lineNumber <= 0 ? - 1 : lineNumber;
			ColumnNumber = columnNumber <= 0 ? - 1 : columnNumber;
		}

	    public JqlParseErrorMessage(string key, int lineNumber, int columnNumber, params object[] arguments)
            : this(key, lineNumber, columnNumber, arguments.ToList())
	    {
	    }

		public override bool Equals(object o)
		{
			if (this == o)
			{
				return true;
			}
			if (o == null || GetType() != o.GetType())
			{
				return false;
			}

			var that = (JqlParseErrorMessage) o;

			if (ColumnNumber != that.ColumnNumber)
			{
				return false;
			}
			if (LineNumber != that.LineNumber)
			{
				return false;
			}
			if (!Arguments.SequenceEqual(that.Arguments))
			{
				return false;
			}
			if (!Key.Equals(that.Key))
			{
				return false;
			}

			return true;
		}

		public override int GetHashCode()
		{
			int result = Key.GetHashCode();
			result = 31 * result + Arguments.GetHashCode();
			result = 31 * result + LineNumber;
			result = 31 * result + ColumnNumber;
			return result;
		}

		public override string ToString()
		{
		    return string.Format("Error<{0}:{1}> - {2}{3}", LineNumber, ColumnNumber, Key, Arguments);
		}
	}
}