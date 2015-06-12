using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuoteFlow.Api.Infrastructure.Extensions;
using QuoteFlow.Api.Jql.Parser;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Query.Order;
using QuoteFlow.Api.Jql.Util;

namespace QuoteFlow.Core.Jql.Util
{
    public sealed class JqlStringSupport : IJqlStringSupport
    {
        private static readonly IDictionary<char?, char?> StringDecodeMapping;
		private static readonly IDictionary<char?, string> StringEncodeMapping;
		private const char QuoteChar = '"';
		private const char SquoteChar = '\'';

		public static readonly HashSet<string> ReservedWords;

		static JqlStringSupport()
		{
			IDictionary<char?, char?> decoderMapping = new Dictionary<char?, char?>();
			decoderMapping['t'] = '\t';
			decoderMapping['n'] = '\n';
			decoderMapping['r'] = '\r';
			decoderMapping['\\'] = '\\';
			decoderMapping[QuoteChar] = QuoteChar;
			decoderMapping[SquoteChar] = SquoteChar;
			decoderMapping[' '] = ' ';

            StringDecodeMapping = decoderMapping;

			IDictionary<char?, string> encoderStringMapping = new Dictionary<char?, string>();
			encoderStringMapping['\t'] = "\\t";
			encoderStringMapping['\n'] = "\\n";
			encoderStringMapping['\r'] = "\\r";
			encoderStringMapping[QuoteChar] = "\\\"";
			encoderStringMapping[SquoteChar] = "\\'";
			encoderStringMapping['\\'] = "\\\\";

		    StringEncodeMapping = encoderStringMapping;

            //NOTE: Changing the contents of this method will change the strings that the JQL parser will parse, so think
            // about the change you are about to make.
            // Also, see TestReservedWords.java in the func_test Catalog if you are going to make changes.
		    var bldr = new List<string>();
            bldr.AddRange(new List<string> { "abort", "access", "Add", "after", "alias", "all", "alter", "and", "any", "as", "asc" });
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
            bldr.AddRange(new List<string> { "str", "subtract", "sum", "synonym", "table", "then", "to", "trans", "transaction", "trigger", "true" });
            bldr.AddRange(new List<string> { "uid", "union", "unique", "update", "user", "validate", "values", "view", "was", "when", "whenever", "where" });
            bldr.AddRange(new List<string> { "while", "with" });

            ReservedWords = new HashSet<string>(bldr);
		}

		private readonly IJqlQueryParser parser;

        public JqlStringSupport(IJqlQueryParser parser)
		{
			this.parser = parser;
		}

        public string EncodeStringValue(string value)
        {
            if (value.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException("value");
            }

            if (!parser.IsValidValue(value))
            {
                return EncodeAsQuotedString(value);
            }

            return value;
        }

        public string EncodeValue(string value)
        {
            if (value.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException("value");
            }

            if (!parser.IsValidValue(value))
            {
                return EncodeAsQuotedString(value);
            }

            return value;
        }

        public string EncodeFunctionArgument(string argument)
        {
            if (!parser.IsValidFunctionArgument(argument))
            {
                return EncodeAsQuotedString(argument, true);
            }

            return argument;
        }

        public string EncodeFunctionName(string functionName)
        {
            if (!parser.IsValidFunctionName(functionName))
            {
                return EncodeAsQuotedString(functionName, true);
            }

            return functionName;
        }

        public string EncodeFieldName(string fieldName)
        {
            if (!parser.IsValidFieldName(fieldName))
            {
                return EncodeAsQuotedString(fieldName, true);
            }

            return fieldName;
        }

        public string GenerateJqlString(IQuery query)
        {
            if (query == null) throw new ArgumentNullException("query");

            StringBuilder builder = new StringBuilder();
            
            if (query.WhereClause != null)
            {
                ToJqlStringVisitor jqlStringVisitor = new ToJqlStringVisitor(this);
                builder.Append(jqlStringVisitor.ToJqlString(query.WhereClause));
            }

            if (query.OrderByClause != null && query.OrderByClause.SearchSorts.Any())
            {
                if (builder.Length != 0)
                {
                    builder.Append(" ");
                }
                
                builder.Append("ORDER BY ");

                var last = query.OrderByClause.SearchSorts.Last();
                foreach (var searchSort in query.OrderByClause.SearchSorts)
                {
                    builder.Append(EncodeFieldName(searchSort.Field));

                    if (searchSort.Property != null)
                    {
                        foreach (var property in searchSort.Property)
                        {
                            builder.Append("[")
                                .Append(EncodeFieldName(property.KeysAsString()))
                                .Append("].")
                                .Append(EncodeFieldName(property.ObjectReferencesAsString()));
                        }
                    }

                    if (searchSort.Order != SortOrder.None)
                    {
                        builder.Append(" ").Append(searchSort.Order);
                    }

                    // tack on a comma if this isn't the last iteration
                    if (!searchSort.Equals(last))
                    {
                        builder.Append(", ");
                    }
                }
            }

            return builder.ToString();

        }

        public string GenerateJqlString(IClause clause)
        {
            var jqlStringVisitor = new ToJqlStringVisitor(this);
            return jqlStringVisitor.ToJqlString(clause);
        }

        public HashSet<string> GetJqlReservedWords()
        {
            return ReservedWords;
        }

        /// <summary>
        /// Remove escaping JQL escaping from the passed str.
        /// </summary>
        /// <param name="str">The string to decode.</param>
        /// <returns>The decoded string.</returns>
        /// <exception cref="IllegalArgumentException"> if the input str contains invalid escape sequences. </exception>
	    public static string Decode(string str)
	    {
		    if (string.IsNullOrWhiteSpace(str))
		    {
			    return str;
		    }

		    StringBuilder stringBuilder = null;
		    for (int position = 0; position < str.Length;)
		    {
                char currentCharacter = str[position];
			    position++;
			    if (currentCharacter == '\\')
			    {
                    if (position >= str.Length)
				    {
					    throw new ArgumentException("Unterminated escape sequence.");
				    }
				    if (stringBuilder == null)
				    {
                        stringBuilder = new StringBuilder(str.Length);
					    if (position > 1)
					    {
                            stringBuilder.Append(str.Substring(0, position - 1));
					    }
				    }

                    char escapeCharacter = str[position];
				    position++;

			        char? substituteCharacter;
			        if (!StringDecodeMapping.TryGetValue(escapeCharacter, out substituteCharacter)) continue;
			        if (substituteCharacter == null)
			        {
			            // Maybe some unicode escaping ?
			            if (escapeCharacter == 'u')
			            {
			                if (position + 4 > str.Length)
			                {
			                    throw new ArgumentException("Unterminated escape sequence '\\u" + str.Substring(position) + "'.");
			                }

			                string hexString = str.Substring(position, 4);
			                position += 4;
			                try
			                {
			                    int i = Convert.ToInt32(hexString, 16);
			                    if (i < 0)
			                    {
			                        throw new ArgumentException("Illegal unicode escape '\\u" + hexString + "'.");
			                    }
			                    stringBuilder.Append((char)i);
			                }
			                catch (Exception e)
			                {
			                    throw new ArgumentException("Illegal unicode escape '\\u" + hexString + "'.", e);
			                }
			            }
			            else
			            {
			                throw new ArgumentException("Illegal escape sequence '\\" + escapeCharacter + "'.");
			            }
			        }
			        else
			        {
			            stringBuilder.Append(substituteCharacter);
			        }
			    }
			    else if (stringBuilder != null)
			    {
				    stringBuilder.Append(currentCharacter);
			    }
		    }

		    return stringBuilder == null ? str : stringBuilder.ToString();
	    }

        /// <summary>
        /// Encode the passed str into a valid JQL encoded quoted str. A JQL str can represent newline (\n) and
        /// carriage return (\r) in two different ways: Its raw value or its escaped value. The passed boolean flag can be
        /// used to control which representation is used.
        /// </summary>
        /// <param name="str"> the str to encode. </param>
        /// <param name="escapeNewline"> should escape and newline characters be escaped. </param>
        /// <returns> the encoded str. </returns>
        public static string EncodeAsQuotedString(string str, bool escapeNewline = false)
        {
            if (str == null)
            {
                return null;
            }

            StringBuilder builder = null;
            for (int position = 0; position < str.Length; position++)
            {
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final char currentCharacter = str.charAt(position);
                char currentCharacter = str[position];
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final String appendString;
                string appendString;

                //Newlines don't need to be quoted in strings.
                if (escapeNewline || (currentCharacter != '\r' && currentCharacter != '\n'))
                {
                    appendString = EncodeCharacter(currentCharacter, SquoteChar, false);
                }
                else
                {
                    appendString = null;
                }
                if (appendString != null)
                {
                    if (builder == null)
                    {
                        builder = new StringBuilder(str.Length);
                        builder.Append(QuoteChar);
                        if (position > 0)
                        {
                            builder.Append(str.Substring(0, position));
                        }
                    }
                    builder.Append(appendString);
                }
                else if (builder != null)
                {
                    builder.Append(currentCharacter);
                }
            }
            return builder == null ? QuoteChar + str + QuoteChar : builder.Append(QuoteChar).ToString();
        }

        /// <summary>
        /// Encode the passed character so that it may be used in JQL. null will be returned if the string does not need
        /// to be encoded and the encoding has not been forced.
        /// </summary>
        /// <param name="character"> the character to encode. </param>
        /// <param name="ignoredCharacter"> the character not to encode. -1 can be passed to indicate that this character should be
        /// excluded from encoding. This setting overrides force for this character. </param>
        /// <param name="force"> when true, the passed character will be encoded even if it does not need to be. </param>
        /// <returns> the encoded character or null if the passed character did not need to be encoded. </returns>
        private static string EncodeCharacter(char character, int ignoredCharacter, bool force)
        {
            if (ignoredCharacter >= 0 && character == (char)ignoredCharacter)
            {
                return null;
            }

            string encodedCharacter;
            if (StringEncodeMapping.TryGetValue(character, out encodedCharacter))
            {
                if (encodedCharacter == null && (force || IsJqlControl(character)))
                {
                    return string.Format("\\u{0:x4}", (int) character);
                }
                return encodedCharacter;
            }

            return null;
        }

        /// <summary>
        /// Escape the passed character so that it may be used in JQL. The character is escaped even if it does not need to be.
        /// </summary>
        /// <param name="character"> the character to escape. </param>
        /// <returns> the escaped character. </returns>
        public static string EncodeCharacterForce(char character)
        {
            return EncodeCharacter(character, -1, true);
        }

        /// <summary>
        /// Encode the passed character so that it may be used in JQL. null will be returned if the character does not need
        /// to be encoded.
        /// </summary>
        /// <param name="character"> the character to encode. </param>
        /// <returns> the encoded character or null if it does not need to be encoded. </returns>
        public static string EncodeCharacter(char character)
        {
            return EncodeCharacter(character, -1, false);
        }

        /// <summary>
        /// Tell the caller if the passed string is a reserved JQL string. We do this in here rather than the grammar because
        /// ANTLR does not deal well (generates a huge and very slow lexer) when matching lots of different tokens. In fact,
        /// the ANTLR grammar calls this method internally to see if a JQL string is reserved.
        /// </summary>
        /// <param name="str">The word to test.</param>
        /// <returns> true if the passed string is a JQL reserved word. </returns>
        public static bool IsReservedString(string str)
        {
            //NOTE: Changing the implementation of this method will change the strings that the JQL parser will parse. We can
            //simply call toLowerCase here becasue all the reserved words are ENGLISH.
            return ReservedWords.Contains(str.ToLower());
        }

        /// <summary>
        /// Tells if caller if the passed character is considered a control character by JQL.
        /// <p/>
        /// NOTE: This method duplicates some logic from the grammar. If the grammar changes then this method will also need
        /// to change. We have replicated the logic for effeciency reasons.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns> true if the passed character is a JQL control character, false otherwise. </returns>
        public static bool IsJqlControl(char c)
        {
            /*
            From the JQL grammar:

            fragment CONTROLCHARS
                :	'\u0000'..'\u0009'  //Exclude '\n' (\u000a)
                |   '\u000b'..'\u000c'  //Exclude '\r' (\u000d)
                |   '\u000e'..'\u001f'
                |	'\u007f'..'\u009f'
                //The following are Unicode non-characters. We don't want to parse them. Importantly, we wish
                //to ignore U+FFFF since ANTLR evilly uses this internally to represent EOF which can cause very
                //strange behaviour. For example, the Lexer will incorrectly tokenise the POSNUMBER 1234 as a STRING
                //when U+FFFF is not excluded from STRING.
                //
                //http://en.wikipedia.org/wiki/Unicode
                | 	'\ufdd0'..'\ufdef'
                |	'\ufffe'..'\uffff'
                ;

             */
            return (c >= '\u0000' && c <= '\u0009') || 
                (c >= '\u000b' && c <= '\u000c') || 
                (c >= '\u000e' && c <= '\u001f') || 
                (c >= '\u007f' && c <= '\u009f') || 
                (c >= '\ufdd0' && c <= '\ufdef') || 
                (c >= '\ufffe' && c <= '\uffff');
        }
    }
}