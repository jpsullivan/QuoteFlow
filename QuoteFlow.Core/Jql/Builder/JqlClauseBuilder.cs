using System;
using System.Collections.Generic;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Query.Operand;
using QuoteFlow.Api.Jql.Util;
using QuoteFlow.Core.Jql.Util;

namespace QuoteFlow.Core.Jql.Builder
{
    /// <summary>
    /// Default implementation of <seealso cref="IJqlClauseBuilder"/>.
    /// </summary>
    public class JqlClauseBuilder : IJqlClauseBuilder
    {
        private readonly JqlQueryBuilder parent;
        private readonly IJqlDateSupport jqlDateSupport;
        private SimpleClauseBuilder builder;

        public JqlClauseBuilder(JqlQueryBuilder parent, IJqlDateSupport jqlDateSupport, SimpleClauseBuilder builder)
        {
            if (jqlDateSupport == null)
            {
                throw new ArgumentNullException("jqlDateSupport");
            }

            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }

            parent = parent;
            jqlDateSupport = jqlDateSupport;
            builder = builder;
        }

        public JqlQueryBuilder EndWhere()
        {
            throw new NotImplementedException();
        }

        public IQuery BuildQuery()
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder Clear()
        {
            builder = builder.
        }

        public IJqlClauseBuilder DefaultAnd()
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder DefaultOr()
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder DefaultNone()
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder And()
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder Or()
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder Not()
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder Sub()
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder Endsub()
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder Manufacturer(params string[] types)
        {
            throw new NotImplementedException();
        }

        public IConditionBuilder Manufacturer()
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder Description(string value)
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder DescriptionIsEmpty()
        {
            throw new NotImplementedException();
        }

        public IConditionBuilder description()
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder Summary(string value)
        {
            throw new NotImplementedException();
        }

        public IConditionBuilder Summary()
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder Comment(string value)
        {
            throw new NotImplementedException();
        }

        public IConditionBuilder Comment()
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder Catalog(params string[] catalogs)
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder Catalog(params int?[] catalogIds)
        {
            throw new NotImplementedException();
        }

        public IConditionBuilder Catalog()
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder CreatedAfter(DateTime startDate)
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder CreatedAfter(string startDate)
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder CreatedBetween(DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder CreatedBetween(string startDateString, string endDateString)
        {
            throw new NotImplementedException();
        }

        public IConditionBuilder Created()
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder UpdatedAfter(DateTime startDate)
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder UpdatedAfter(string startDate)
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder UpdatedBetween(DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder UpdatedBetween(string startDateString, string endDateString)
        {
            throw new NotImplementedException();
        }

        public IConditionBuilder Updated()
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder Asset(params string[] assetIds)
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder AssetInHistory()
        {
            throw new NotImplementedException();
        }

        public IConditionBuilder Asset()
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder AttachmentsExists(bool hasAttachment)
        {
            throw new NotImplementedException();
        }

        public IConditionBuilder Field(string jqlName)
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder AddClause(IClause clause)
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder AddDateCondition(string clauseName, Operator @operator, DateTime date)
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder AddDateCondition(string clauseName, params DateTime[] dates)
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder AddDateCondition(string clauseName, Operator @operator, params DateTime[] dates)
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder AddDateCondition(string clauseName, ICollection<DateTime> dates)
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder AddDateCondition(string clauseName, Operator @operator, ICollection<DateTime> dates)
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder AddDateRangeCondition(string clauseName, DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder AddFunctionCondition(string clauseName, string functionName)
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder AddFunctionCondition(string clauseName, string functionName, params string[] args)
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder AddFunctionCondition(string clauseName, string functionName, ICollection<string> args)
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder AddFunctionCondition(string clauseName, Operator @operator, string functionName)
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder AddFunctionCondition(string clauseName, Operator @operator, string functionName, params string[] args)
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder AddFunctionCondition(string clauseName, Operator @operator, string functionName, ICollection<string> args)
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder AddStringCondition(string clauseName, string clauseValue)
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder AddStringCondition(string clauseName, params string[] clauseValues)
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder AddStringCondition(string clauseName, ICollection<string> clauseValues)
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder AddStringCondition(string clauseName, Operator @operator, string clauseValue)
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder AddStringCondition(string clauseName, Operator @operator, params string[] clauseValues)
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder AddStringCondition(string clauseName, Operator @operator, ICollection<string> clauseValues)
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder AddStringRangeCondition(string clauseName, string start, string end)
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder AddNumberCondition(string clauseName, long? clauseValue)
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder AddNumberCondition(string clauseName, params long?[] clauseValues)
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder AddNumberCondition(string clauseName, ICollection<long?> clauseValues)
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder AddNumberCondition(string clauseName, Operator @operator, long? clauseValue)
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder AddNumberCondition(string clauseName, Operator @operator, params long?[] clauseValues)
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder AddNumberCondition(string clauseName, Operator @operator, ICollection<long?> clauseValues)
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder AddNumberRangeCondition(string clauseName, long? start, long? end)
        {
            throw new NotImplementedException();
        }

        public IConditionBuilder AddCondition(string clauseName)
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder AddCondition(string clauseName, IOperand operand)
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder AddCondition(string clauseName, params IOperand[] operands)
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder AddCondition<T>(string clauseName, ICollection<T> operands) where T : IOperand
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder AddCondition(string clauseName, Operator @operator, IOperand operand)
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder AddCondition(string clauseName, Operator @operator, params IOperand[] operands)
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder AddCondition<T>(string clauseName, Operator @operator, ICollection<T> operands) where T : IOperand
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder AddRangeCondition(string clauseName, IOperand start, IOperand end)
        {
            throw new NotImplementedException();
        }

        public IJqlClauseBuilder AddEmptyCondition(string clauseName)
        {
            throw new NotImplementedException();
        }

        public IClause BuildClause()
        {
            throw new NotImplementedException();
        }
    }
}