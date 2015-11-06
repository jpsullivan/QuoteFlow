using System;
using System.Collections.Generic;
using QuoteFlow.Api.Asset.CustomFields.Searchers.Transformer;
using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Asset.Search.Searchers.Transformer;
using QuoteFlow.Api.Asset.Search.Searchers.Util;
using QuoteFlow.Api.Asset.Transport;
using QuoteFlow.Api.Infrastructure.Elmah;
using QuoteFlow.Api.Infrastructure.Extensions;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Util;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Core.Asset.Search.Searchers.Transformer
{
    /// <summary>
    /// A SearchInputTransformer for Dates.
    /// 
    /// @since v4.0
    /// </summary>
    public class DateSearchInputTransformer : ISearchInputTransformer
    {
        private readonly DateSearcherConfig dateSearcherConfig;
        private readonly IJqlOperandResolver operandResolver;
        private readonly IJqlDateSupport jqlDateSupport;
        private readonly bool _allowTimeComponent;
        private readonly ICustomFieldInputHelper customFieldInputHelper;

        public DateSearchInputTransformer(bool allowTimeComponent, DateSearcherConfig config, IJqlOperandResolver operandResolver, IJqlDateSupport jqlDateSupport, ICustomFieldInputHelper customFieldInputHelper)
        {
            this._allowTimeComponent = allowTimeComponent;
            dateSearcherConfig = config;
            this.operandResolver = operandResolver;
            this.jqlDateSupport = jqlDateSupport;
            this.customFieldInputHelper = customFieldInputHelper;
        }

        public virtual void PopulateFromParams(User user, IFieldValuesHolder fieldValuesHolder, IActionParams actionParams)
        {
            if (fieldValuesHolder == null)
            {
                throw new ArgumentNullException(nameof(fieldValuesHolder));
            }

            if (actionParams == null)
            {
                throw new ArgumentNullException(nameof(actionParams));
            }

            fieldValuesHolder[dateSearcherConfig.BeforeField] = actionParams.GetFirstValueForKey(dateSearcherConfig.BeforeField);
            fieldValuesHolder[dateSearcherConfig.AfterField] = actionParams.GetFirstValueForKey(dateSearcherConfig.AfterField);
            fieldValuesHolder[dateSearcherConfig.PreviousField] = actionParams.GetFirstValueForKey(dateSearcherConfig.PreviousField);
            fieldValuesHolder[dateSearcherConfig.NextField] = actionParams.GetFirstValueForKey(dateSearcherConfig.NextField);
        }

        public virtual void ValidateParams(User user, ISearchContext searchContext, IFieldValuesHolder fieldValuesHolder)
        {
            if (fieldValuesHolder == null)
            {
                throw new ArgumentNullException(nameof(fieldValuesHolder));
            }
            
            ValidateAbsoluteDates(fieldValuesHolder);
            ValidateRelativeDates(fieldValuesHolder);
        }

        private void ValidateAbsoluteDates(IFieldValuesHolder fieldValuesHolder)
        {
            // for each field, try to convert its value into a Timestamp
            string[] dateParamNames = dateSearcherConfig.AbsoluteFields;
            DateTime[] dateParamValues = new DateTime[2];

            int i = 0;
            foreach (String dateParamName in dateParamNames)
            {
                var dateString = (String) fieldValuesHolder[dateParamName];
                if (dateString.HasValue())
                {
                    //DateTimeFormatter formatter = dateTimeFormatterFactory.formatter().forLoggedInUser().withStyle(DATE_PICKER);
                    try
                    {
                        dateParamValues[i] = Convert.ToDateTime(dateString);
                    }
                    catch (ArgumentException e)
                    {
                        //errors.addError(dateParamName, i18nHelper.getText("fields.validation.data.format", formatter.FormatHint));
                    }
                }
                i++;
            }

            // validate date format After and Before are not stupid
            DateTime afterDate = dateParamValues[0];
            DateTime beforeDate = dateParamValues[1];
            if (afterDate != null && beforeDate != null)
            {
                if (beforeDate.CompareTo(afterDate) < 0)
                {
                    //errors.addError(dateSearcherConfig.AfterField, i18nHelper.getText("fields.validation.date.absolute.before.after"));
                }
            }
        }

        private void ValidateRelativeDates(IFieldValuesHolder fieldValuesHolder)
        {
            // for each field, try to convert its value into a Duration
            string[] periodParamNames = dateSearcherConfig.RelativeFields;
            //string[] periodParamLabels = { i18nHelper.getText("navigator.filter.constants.duedate.from"), i18nHelper.getText("navigator.filter.constants.duedate.to") };
            for (int i = 0; i < periodParamNames.Length; i++)
            {
                string periodParam = (string)fieldValuesHolder[periodParamNames[i]];
                if (periodParam.HasValue())
                {
                    try
                    {
                        //DateUtils.getDurationWithNegative(periodParam);
                    }
                    catch (Exception e)
                    {
//                        string validationKey = (fieldValuesHolder.Count() > 1) ? "fields.validation.date.period.format" : "fields.validation.date.period.format.single.field";
//                        errors.addError(periodParamNames[i], i18nHelper.getText(validationKey, periodParamLabels[i]));
                        QuietLog.LogHandledException(e);
                    }
                }
            }

            // Validate that 'from' is not after 'to'
            string previousDateString = (string)fieldValuesHolder[dateSearcherConfig.PreviousField];
            string nextDateString = (string)fieldValuesHolder[dateSearcherConfig.NextField];
            if (previousDateString.HasValue() && nextDateString.HasValue())
            {
                try
                {
//                    long prevDateLong = DateUtils.getDurationWithNegative(previousDateString);
//                    long nextDateLong = DateUtils.getDurationWithNegative(nextDateString);
//                    if (prevDateLong > nextDateLong)
//                    {
//                        //errors.addError(dateSearcherConfig.PreviousField, i18nHelper.getText("fields.validation.date.period.from.to"));
//                    }
                }
                catch (Exception e)
                {
                    // Errors logged previously
                }
            }
        }

        public virtual void PopulateFromQuery(User user, IFieldValuesHolder fieldValuesHolder, IQuery query, ISearchContext searchContext)
        {
            if (fieldValuesHolder == null)
            {
                throw new ArgumentNullException(nameof(fieldValuesHolder));
            }

            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            if (query.WhereClause != null)
            {
                IDateSearcherInputHelper helper = CreateDateSearcherInputHelper();
                var clauseResult = helper.ConvertClause(query.WhereClause, user, _allowTimeComponent);
                IDictionary<string, string> result = clauseResult.Fields;
                if (result != null)
                {
                    foreach (var r in result)
                    {
                        fieldValuesHolder[r.Key] = r.Value;
                    }
                }
            }
        }

        public virtual bool DoRelevantClausesFitFilterForm(User user, IQuery query, ISearchContext searchContext)
        {
            if (query != null && query.WhereClause != null)
            {
                IClause whereClause = query.WhereClause;
                // check that it conforms to simple navigator structure, and that the right number of clauses appear
                // with the correct operators
                var inputHelper = CreateDateSearcherInputHelper();
                return inputHelper.ConvertClause(whereClause, user, _allowTimeComponent).FitsFilterForm;
            }
            return true;
        }

        internal virtual IDateSearcherInputHelper CreateDateSearcherInputHelper()
        {
            return new DateSearcherInputHelper(dateSearcherConfig, operandResolver, jqlDateSupport);
        }

        public virtual IClause GetSearchClause(User user, IFieldValuesHolder fieldValuesHolder)
        {
            if (fieldValuesHolder == null)
            {
                throw new ArgumentNullException(nameof(fieldValuesHolder));
            }

            string clauseName = GetClauseName(user);
            IClause relativeClause = CreatePeriodClause((string)fieldValuesHolder[dateSearcherConfig.PreviousField], (string)fieldValuesHolder[dateSearcherConfig.NextField], clauseName);
            IClause absoluteClause = CreateDateClause((string)fieldValuesHolder[dateSearcherConfig.AfterField], (string)fieldValuesHolder[dateSearcherConfig.BeforeField], clauseName);
            return CreateCompoundClause(relativeClause, absoluteClause);
        }

        private IClause CreatePeriodClause(string lower, string upper, string clauseName)
        {
            return CreateCompoundClause(ParsePeriodClause(lower, Operator.GREATER_THAN_EQUALS, clauseName), ParsePeriodClause(upper, Operator.LESS_THAN_EQUALS, clauseName));
        }

        private IClause ParsePeriodClause(string period, Operator @operator, string clauseName)
        {
            if (period.IsNullOrEmpty())
            {
                return null;
            }
            return new TerminalClause(clauseName, @operator, period);
        }

        private IClause CreateDateClause(string lower, string upper, string clauseName)
        {
            var fromClause = CreateDateClause(lower, Operator.GREATER_THAN_EQUALS, clauseName);
            var toClause = CreateDateClause(upper, Operator.LESS_THAN_EQUALS, clauseName);
            return CreateCompoundClause(fromClause, toClause);
        }

        private IClause CreateDateClause(string date, Operator @operator, string clauseName)
        {
            if (date.HasValue())
            {
                try
                {
                    DateTime parsedDate = Convert.ToDateTime(date);
                    if (parsedDate != null)
                    {
                        string jqlDate = jqlDateSupport.GetDateString(parsedDate);
                        if (jqlDate != null)
                        {
                            return new TerminalClause(clauseName, @operator, jqlDate);
                        }
                    }
                }
                catch (ArgumentException e)
                {
                    //log.info(string.Format("Unable to parse date '{0}'.", date));
                }
                // If the parsing of the user date failed just put in the original input.
                return new TerminalClause(clauseName, @operator, date);
            }
            return null;
        }

        private static IClause CreateCompoundClause(IClause left, IClause right)
        {
            if (left == null)
            {
                return right;
            }
            if (right != null)
            {
                return new AndClause(left, right);
            }
            return left;
        }

        private string GetClauseName(User searcher)
        {
            string primaryName = dateSearcherConfig.ClauseNames.PrimaryName;
            string fieldName = dateSearcherConfig.FieldName;

            if (primaryName.Equals(fieldName, StringComparison.CurrentCultureIgnoreCase))
            {
                return fieldName;
            }
            return customFieldInputHelper.GetUniqueClauseName(searcher, primaryName, fieldName);
        }
    }
}