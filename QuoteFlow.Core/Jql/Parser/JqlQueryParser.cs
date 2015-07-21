using System;
using Antlr.Runtime;
using QuoteFlow.Api.Jql.Parser;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Order;
using QuoteFlow.Api.Jql.Util;
using QuoteFlow.Core.Jql.AntlrGen;
using QuoteFlow.Core.Jql.Parser.Antlr;
using QuoteFlow.Models.Jql.AntlrGen;

namespace QuoteFlow.Core.Jql.Parser
{
    /// <summary>
    /// An implementation of <see cref="IJqlQueryParser"/> that implements the query parser using ANTLR.
    /// </summary>
    public sealed class JqlQueryParser : IJqlQueryParser
    {
        public IQuery ParseQuery(string jqlQuery)
        {
            if (jqlQuery == null)
            {
                throw new ArgumentNullException(nameof(jqlQuery));
            }

            JqlParser.query_return aReturn = ParseClause(jqlQuery);
            // Never let a parsed order by be null, this means no sorts and we want the default sorts.
            IOrderBy orderByClause = aReturn.order ?? new OrderBy();
            return new Api.Jql.Query.Query(aReturn.clause, orderByClause, jqlQuery);
        }

        public bool IsValidFieldName(string fieldName)
        {
            if (fieldName == null)
            {
                throw new ArgumentNullException(nameof(fieldName));
            }

            try
            {
                if (JqlCustomFieldId.IsJqlCustomFieldId(fieldName))
                {
                    return true;
                }

                var parser = CreateJqlParser(fieldName);
                var fieldCheck = parser.fieldCheck();
                return fieldName == CreateJqlParser(fieldName).fieldCheck().Name;
            }
            catch (RecognitionException e)
            {
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool IsValidFunctionArgument(string argument)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(nameof(argument));
            }

            try
            {
                return argument == CreateJqlParser(argument).argumentCheck();
            }
            catch (RecognitionException e)
            {
                return false;
            }
        }

        public bool IsValidFunctionName(string functionName)
        {
            if (functionName == null)
            {
                throw new ArgumentNullException(nameof(functionName));
            }

            try
            {
                return functionName == CreateJqlParser(functionName).funcNameCheck();
            }
            catch (RecognitionException e)
            {
                return false;
            }

        }

        public bool IsValidValue(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            try
            {
                return IsInt(value) || value == CreateJqlParser(value).stringValueCheck();
            }
            catch (RecognitionException e)
            {
                return false;
            }
        }

        private JqlParser.query_return ParseClause(string clauseString)
        {
            try
            {
                try
                {
                    return CreateJqlParser(clauseString).query();
                }
                catch (RecognitionException e)
                {
                    throw new JqlParseException(JqlParseErrorMessages.GenericParseError(e.Token), e);
                }
            }
            catch (RuntimeRecognitionException e)
            {
                //Our code throws this exception when we really want ANTLR to stop working. At the moment ANTLR produces a
                //lexer will not quit on errors, but will rather drop input. We use this RuntimeException as a workaround.
                throw new JqlParseException(e.ParseErrorMessage, e);
            }
        }

        private JqlParser CreateJqlParser(string clauseString)
        {
            var lexer = new JqlLexer(new ANTLRStringStream(clauseString));
            return new JqlParser(new CommonTokenStream(lexer));
        }

        private bool IsInt(string intString)
        {
            try
            {
                var value = Convert.ToInt32(intString);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

    }
}