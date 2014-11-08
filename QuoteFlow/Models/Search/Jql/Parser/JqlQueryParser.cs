using System;
using Antlr.Runtime;
using QuoteFlow.Infrastructure.Exceptions;
using QuoteFlow.Infrastructure.Exceptions.Antlr;
using QuoteFlow.Models.Search.Jql.AntlrGen;
using QuoteFlow.Models.Search.Jql.Query;
using QuoteFlow.Models.Search.Jql.Query.Order;
using QuoteFlow.Models.Search.Jql.Util;

namespace QuoteFlow.Models.Search.Jql.Parser
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
                throw new ArgumentNullException("jqlQuery");
            }

            JqlParser.query_return aReturn = ParseClause(jqlQuery);
            // Never let a parsed order by be null, this means no sorts and we want the default sorts.
            IOrderBy orderByClause = aReturn.order ?? new OrderBy();
            return new Query.Query(aReturn.clause, orderByClause, jqlQuery);
        }

        public bool IsValidFieldName(string fieldName)
        {
            if (fieldName == null)
            {
                throw new ArgumentNullException("fieldName");
            }

            try
            {
                if (JqlCustomFieldId.IsJqlCustomFieldId(fieldName))
                {
                    return true;
                }
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
                throw new ArgumentNullException("argument");
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
                throw new ArgumentNullException("functionName");
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
                throw new ArgumentNullException("value");
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