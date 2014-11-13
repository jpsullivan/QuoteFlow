using System;
using System.Text.RegularExpressions;
using Lucene.Net.QueryParsers;
using QuoteFlow.Infrastructure.Util;
using QuoteFlow.Models.Assets.Search.Searchers.Util;

namespace QuoteFlow.Models.Assets.Search.Searchers.Transformer
{
    /// <summary>
    /// A single class to validate text queries (LIKE) - used in few places to handle validation consistently.
    /// </summary>
    public class TextQueryValidator
    {
        public static readonly Regex BAD_RANGEIN_PATTERN = new Regex("Cannot parse.*Was expecting.*(\"TO\"|<RANGEIN_QUOTED>|<RANGEIN_GOOP>|\"]\"|\"}\").*", RegexOptions.Singleline);
        public static readonly Regex BAD_RANGEEX_PATTERN = new Regex("Cannot parse.*Was expecting.*(\"TO\"|<RANGEEX_QUOTED>|<RANGEEX_GOOP>|\"]\"|\"}\").*", RegexOptions.Singleline);

        public virtual IMessageSet Validate(QueryParser queryParser, string query, string fieldName, string sourceFunction, bool shortMessage)
        {
            var messageSet = new MessageSet();
            try
            {
                queryParser.Parse(TextTermEscaper.Escape(query.ToCharArray()));
                // if it didn't throw an exception it must be valid
            }
            catch (ParseException e)
            {
                HandleException(fieldName, sourceFunction, query, messageSet, shortMessage, e);
            }
            catch (Exception re)
            {
                HandleException(fieldName, sourceFunction, query, messageSet, shortMessage, re);
            }
            return messageSet;
        }

        private void HandleException(string fieldName, string sourceFunction, string value, IMessageSet messageSet, bool useShortMessage, Exception ex)
        {
            string errorMessage = TranslateException(ex, fieldName, sourceFunction, useShortMessage, value);

            if (errorMessage != null)
            {
                messageSet.AddErrorMessage(errorMessage);
            }
            else
            {
                //log.debug(string.Format("Unable to parse the text '{0}' for field '{1}'.", value, fieldName), ex);
                if (sourceFunction != null)
                {
                    messageSet.AddErrorMessage(useShortMessage
                        ? string.Format("navigator.error.parse.function: {0}", sourceFunction)
                        : string.Format("jira.jql.text.clause.does.not.parse.function: {0}, {1}", fieldName,
                            sourceFunction));
                }
                else
                {
                    messageSet.AddErrorMessage(useShortMessage
                        ? string.Format("navigator.error.parse")
                        : string.Format("jira.jql.text.clause.does.not.parse: {0}, {1}", value, fieldName));
                }
            }
        }

        /// <summary>
        /// This method handles known lucene errors into user-friendly error message.
        /// </summary>
        private string TranslateException(Exception ex, string fieldName, string sourceFunction, bool shortMessage, string value)
        {
            if (!(ex is ParseException)) return null;

            var parseException = (ParseException) ex;
            string exMessage = parseException.Message;
                
            if (exMessage.EndsWith("'*' or '?' not allowed as first character in WildcardQuery"))
            {
                return GetErrorMessage(sourceFunction, shortMessage, fieldName, value, "jira.jql.text.clause.bad.start.in.wildcard");
            }
                
            if (BAD_RANGEIN_PATTERN.Match(exMessage).Success || BAD_RANGEEX_PATTERN.Match(exMessage).Success)
            {
                return GetErrorMessage(sourceFunction, shortMessage, fieldName, value, "jira.jql.text.clause.incorrect.range.query");
            }
            return null;
        }

        private static string GetErrorMessage(string sourceFunction, bool shortMessage, string fieldName, string value, string i18nMessagePrefix)
        {
            if (sourceFunction != null)
            {
                return shortMessage ? string.Format(i18nMessagePrefix + ".function.short", sourceFunction) : string.Format(i18nMessagePrefix + ".function: {0}, {1}", sourceFunction, fieldName);
            }
            
            return shortMessage ? string.Format(i18nMessagePrefix + ".short") : string.Format(i18nMessagePrefix, value, fieldName);
        }
    }
}