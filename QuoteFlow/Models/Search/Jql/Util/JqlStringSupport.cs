using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using QuoteFlow.Models.Search.Jql.Query.Clause;

namespace QuoteFlow.Models.Search.Jql.Util
{
    public sealed class JqlStringSupport : IJqlStringSupport
    {
        private static readonly IDictionary<char?, char?> STRING_DECODE_MAPPING;
		private static readonly IDictionary<char?, string> STRING_ENCODE_MAPPING;
		private const char QUOTE_CHAR = '"';
		private const char SQUOTE_CHAR = '\'';

		public static readonly HashSet<string> RESERVED_WORDS;

		static JqlStringSupport()
		{
			IDictionary<char?, char?> decoderMapping = new Dictionary<char?, char?>();
			decoderMapping['t'] = '\t';
			decoderMapping['n'] = '\n';
			decoderMapping['r'] = '\r';
			decoderMapping['\\'] = '\\';
			decoderMapping[QUOTE_CHAR] = QUOTE_CHAR;
			decoderMapping[SQUOTE_CHAR] = SQUOTE_CHAR;
			decoderMapping[' '] = ' ';

            STRING_DECODE_MAPPING = decoderMapping;

			IDictionary<char?, string> encoderStringMapping = new Dictionary<char?, string>();
			encoderStringMapping['\t'] = "\\t";
			encoderStringMapping['\n'] = "\\n";
			encoderStringMapping['\r'] = "\\r";
			encoderStringMapping[QUOTE_CHAR] = "\\\"";
			encoderStringMapping[SQUOTE_CHAR] = "\\'";
			encoderStringMapping['\\'] = "\\\\";

		    STRING_ENCODE_MAPPING = encoderStringMapping;

		    var bldr = new List<string>();
            bldr.AddRange(new List<string> { "abort", "access", "add", "after", "alias", "all", "alter", "and", "any", "as", "asc" });
            bldr.AddRange(new List<string> { "audit", "avg", "before", "begin", "between", "boolean", "break", "by", "byte", "catch", "cf", "changed" });
            bldr.AddRange(new List<string> { "char", "character", "check", "checkpoint", "collate", "collation", "column", "commit", "connect", "continue" });
            bldr.AddRange(new List<string> { "count", "create", "current", "date", "decimal", "declare", "decrement", "default", "defaults", "define", "delete" });
            bldr.AddRange(new List<string> { "delimiter", "desc", "difference", "distinct", "divide", "do", "double", "drop", "else", "empty", "encoding" });
            bldr.AddRange(new List<string> { "end", "equals", "escape", "exclusive", "exec", "execute", "exists", "explain", "false", "fetch", "file", "field" });
            bldr.AddRange(new List<string> { "first", "float", "for", "from", "function", "go", "goto", "grant", "greater", "group", "having" });
            bldr.AddRange(new List<string> { "identified", "if", "immediate", "in", "increment", "index", "initial", "inner", "inout", "input", "insert" });
            bldr.AddRange(new List<string> { "int", "integer", "intersect", "intersection", "into", "is", "isempty", "isnull", "join", "last", "left" });
            bldr.AddRange(new List<string> { "less", "like", "limit", "lock", "long", "max", "min", "minus", "mode", "modify" });
            bldr.AddRange(new List<string> { "modulo", "more", "multiply", "next", "noaudit", "not", "notin", "nowait", "null", "number", "object" });
            bldr.AddRange(new List<string> { "of", "on", "option", "or", "order", "outer", "output", "power", "previous", "prior", "privileges" });
            bldr.AddRange(new List<string> { "public", "raise", "raw", "remainder", "rename", "resource", "return", "returns", "revoke", "right", "row" });
            bldr.AddRange(new List<string> { "rowid", "rownum", "rows", "select", "session", "set", "share", "size", "sqrt", "start", "strict" });
            bldr.AddRange(new List<string> { "string", "subtract", "sum", "synonym", "table", "then", "to", "trans", "transaction", "trigger", "true" });
            bldr.AddRange(new List<string> { "uid", "union", "unique", "update", "user", "validate", "values", "view", "was", "when", "whenever", "where" });
            bldr.AddRange(new List<string> { "while", "with" });

		    var test = new StringBuilder();
		    var shit = new HashSet<string> {"abort", "access"};
            shit.

			//NOTE: Changing the contents of this method will change the strings that the JQL parser will parse, so think
			// about the change you are about to make.
			// Also, see TestReservedWords.java in the func_test project if you are going to make changes.
			CollectionBuilder<string> builder = CollectionBuilder.newBuilder();
			builder.addAll("abort", "access", "add", "after", "alias", "all", "alter", "and", "any", "as", "asc");
			builder.addAll("audit", "avg", "before", "begin", "between", "boolean", "break", "by", "byte", "catch", "cf", "changed");
			builder.addAll("char", "character", "check", "checkpoint", "collate", "collation", "column", "commit", "connect", "continue");
			builder.addAll("count", "create", "current", "date", "decimal", "declare", "decrement", "default", "defaults", "define", "delete");
			builder.addAll("delimiter", "desc", "difference", "distinct", "divide", "do", "double", "drop", "else", "empty", "encoding");
			builder.addAll("end", "equals", "escape", "exclusive", "exec", "execute", "exists", "explain", "false", "fetch", "file", "field");
			builder.addAll("first", "float", "for", "from", "function", "go", "goto", "grant", "greater", "group", "having");
			builder.addAll("identified", "if", "immediate", "in", "increment", "index", "initial", "inner", "inout", "input", "insert");
			builder.addAll("int", "integer", "intersect", "intersection", "into", "is", "isempty", "isnull", "join", "last", "left");
			builder.addAll("less", "like", "limit", "lock", "long", "max", "min", "minus", "mode", "modify");
			builder.addAll("modulo", "more", "multiply", "next", "noaudit", "not", "notin", "nowait", "null", "number", "object");
			builder.addAll("of", "on", "option", "or", "order", "outer", "output", "power", "previous", "prior", "privileges");
			builder.addAll("public", "raise", "raw", "remainder", "rename", "resource", "return", "returns", "revoke", "right", "row");
			builder.addAll("rowid", "rownum", "rows", "select", "session", "set", "share", "size", "sqrt", "start", "strict");
			builder.addAll("string", "subtract", "sum", "synonym", "table", "then", "to", "trans", "transaction", "trigger", "true");
			builder.addAll("uid", "union", "unique", "update", "user", "validate", "values", "view", "was", "when", "whenever", "where");
			builder.addAll("while", "with");
			RESERVED_WORDS = builder.asSet();
		}

		private readonly JqlQueryParser parser;

        public JqlStringSupport(JqlQueryParser parser)
		{
			this.parser = notNull("parser", parser);
		}


        public string EncodeStringValue(string value)
        {
            throw new NotImplementedException();
        }

        public string EncodeFunctionArgument(string argument)
        {
            throw new NotImplementedException();
        }

        public string EncodeFunctionName(string functionName)
        {
            throw new NotImplementedException();
        }

        public string EncodeFieldName(string fieldName)
        {
            throw new NotImplementedException();
        }

        public string GenerateJqlString(Query.Query query)
        {
            throw new NotImplementedException();
        }

        public string GenerateJqlString(IClause clause)
        {
            throw new NotImplementedException();
        }

        public HashSet<string> GetJqlReservedWords()
        {
            throw new NotImplementedException();
        }
    }
}