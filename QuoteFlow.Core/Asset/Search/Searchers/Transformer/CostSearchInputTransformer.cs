using System;
using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Api.Asset.Search;
using QuoteFlow.Api.Asset.Search.Constants;
using QuoteFlow.Api.Asset.Search.Searchers.Transformer;
using QuoteFlow.Api.Asset.Search.Searchers.Util;
using QuoteFlow.Api.Asset.Transport;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Query.Operand;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Services;
using QuoteFlow.Api.Util;
using QuoteFlow.Core.Asset.Search.Searchers.Util;
using QuoteFlow.Core.Jql.Builder;
using QuoteFlow.Core.Jql.Resolver;

namespace QuoteFlow.Core.Asset.Search.Searchers.Transformer
{
    public class CostSearchInputTransformer : ISearchInputTransformer
    {
        private readonly SimpleFieldSearchConstants _constants;
        private readonly CostSearcherConfig _config;
        private readonly IJqlOperandResolver _operandResolver;

        public CostSearchInputTransformer(SimpleFieldSearchConstants constants, CostSearcherConfig config, IJqlOperandResolver operandResolver)
        {
            if (constants == null) throw new ArgumentNullException(nameof(constants));
            if (config == null) throw new ArgumentNullException(nameof(config));
            if (operandResolver == null) throw new ArgumentNullException(nameof(operandResolver));

            _constants = constants;
            _config = config;
            _operandResolver = operandResolver;
        }

        public virtual void PopulateFromParams(User user, IFieldValuesHolder fieldValuesHolder, IActionParams actionParams)
        {
            if (fieldValuesHolder == null) throw new ArgumentNullException(nameof(fieldValuesHolder));
            
            fieldValuesHolder.Add(GetMinField(), actionParams.GetFirstValueForKey(GetMinField()));
            fieldValuesHolder.Add(GetMaxField(), actionParams.GetFirstValueForKey(GetMaxField()));
        }

        public void ValidateParams(User searcher, ISearchContext searchContext, IFieldValuesHolder fieldValuesHolder, IErrorCollection errors)
        {
            if (fieldValuesHolder == null) throw new ArgumentNullException(nameof(fieldValuesHolder));

            var minLimit = ValidateRatioField(fieldValuesHolder, GetMinField(), errors, "navigator.filter.cost.min.error");
            var maxLimit = ValidateRatioField(fieldValuesHolder, GetMaxField(), errors, "navigator.filter.cost.max.error");

            // check that min <= max
            if (minLimit != null && maxLimit != null && (minLimit > maxLimit))
            {
                errors.AddError(GetMinField(), "navigator.filter.cost.limits.error");
            }
        }

        public virtual void PopulateFromQuery(User user, IFieldValuesHolder fieldValuesHolder, IQuery query, ISearchContext searchContext)
        {
            if (fieldValuesHolder == null) throw new ArgumentNullException(nameof(fieldValuesHolder));
            if (query == null) throw new ArgumentNullException(nameof(query));

            if (query.WhereClause == null)
            {
                return;
            }

            var helper = CreateCostSearcherInputHelper();
            var result = helper.ConvertClause(query.WhereClause, user);
            if (result == null)
            {
                return;
            }

            foreach (var field in result)
            {
                fieldValuesHolder.Add(field.Key, field.Value);
            }
        }

        public virtual bool DoRelevantClausesFitFilterForm(User user, IQuery query, ISearchContext searchContext)
        {
            return true;
        }

        public virtual IClause GetSearchClause(User user, IFieldValuesHolder fieldValuesHolder)
        {
            if (fieldValuesHolder == null) throw new ArgumentNullException(nameof(fieldValuesHolder));

            decimal? minValue = null;
            decimal? maxValue = null;

            TryGetBothValues(fieldValuesHolder, ref minValue, ref maxValue);

            var builder = JqlQueryBuilder.NewClauseBuilder();
            IClause result;
            if (minValue.HasValue && maxValue.HasValue)
            {
                result = builder.Cost().Range(minValue.Value, maxValue.Value).BuildClause();
            }
            else if (minValue.HasValue)
            {
                result = builder.Cost().GtEq(minValue.Value).BuildClause();
            }
            else if (maxValue.HasValue)
            {
                result = builder.Cost().LtEq(maxValue).BuildClause();
            }
            else
            {
                result = null;
            }

            return result;
        }

        private decimal? ValidateRatioField(IFieldValuesHolder fieldValuesHolder, string fieldId, IErrorCollection errors, string errorKey)
        {
            var input = string.Empty;
            object unresolvedValue;
            if (fieldValuesHolder.TryGetValue(fieldId, out unresolvedValue))
            {
                input = (string) unresolvedValue;
            }

            decimal? limitValue = null;
            if (!string.IsNullOrEmpty(input))
            {
                try
                {
                    limitValue = Convert.ToDecimal(input);
                }
                catch (FormatException)
                {
                    errors.AddError(fieldId, errorKey);
                }
            }
            return limitValue;
        }

        private string GetMinField()
        {
            return _config.Min;
        }

        private string GetMaxField()
        {
            return _config.Max;
        }

        private void TryGetBothValues(IFieldValuesHolder fieldValuesHolder, ref decimal? minValue, ref decimal? maxValue)
        {
            object minResult;
            if (fieldValuesHolder.TryGetValue(_config.Min, out minResult))
            {
                try
                {
                    if (minResult != null)
                    {
                        minValue = Convert.ToDecimal(minResult);
                    }
                }
                catch (FormatException)
                {
                    // don't care, ignore it.
                }

            }
            object maxResult;
            if (fieldValuesHolder.TryGetValue(_config.Max, out maxResult))
            {
                try
                {
                    if (maxResult != null)
                    {
                        maxValue = Convert.ToDecimal(maxResult);
                    }
                }
                catch (FormatException)
                {
                    // don't care, ignore it.
                }
            }
        }

        protected virtual CostSearcherInputHelper CreateCostSearcherInputHelper()
        {
            return new CostSearcherInputHelper(_constants, _operandResolver);
        }
    }
}