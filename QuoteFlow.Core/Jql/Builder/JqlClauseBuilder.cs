using System;
using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Api.Asset.Search.Constants;
using QuoteFlow.Api.Infrastructure.Extensions;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Query.Operand;
using QuoteFlow.Api.Jql.Util;

namespace QuoteFlow.Core.Jql.Builder
{
    /// <summary>
    /// Default implementation of <seealso cref="IJqlClauseBuilder"/>.
    /// </summary>
    public class JqlClauseBuilder : IJqlClauseBuilder
    {
        private readonly JqlQueryBuilder _parent;
        private readonly IJqlDateSupport _jqlDateSupport;
        private ISimpleClauseBuilder _builder;

        public JqlClauseBuilder(JqlQueryBuilder parent, ISimpleClauseBuilder builder, IJqlDateSupport jqlDateSupport)
        {
            if (_jqlDateSupport == null)
            {
                throw new ArgumentNullException("jqlDateSupport");
            }

            if (_builder == null)
            {
                throw new ArgumentNullException("builder");
            }

            _parent = parent;
            _jqlDateSupport = jqlDateSupport;
            _builder = builder;
        }

        public JqlQueryBuilder EndWhere()
        {
            return _parent;
        }

        public IQuery BuildQuery()
        {
            return _parent != null ? _parent.BuildQuery() : new Api.Jql.Query.Query(BuildClause());
        }

        public IJqlClauseBuilder Clear()
        {
            _builder = _builder.Clear();
            return this;
        }

        public IJqlClauseBuilder DefaultAnd()
        {
            _builder = _builder.DefaultAnd();
            return this;
        }

        public IJqlClauseBuilder DefaultOr()
        {
            _builder = _builder.DefaultOr();
            return this;
        }

        public IJqlClauseBuilder DefaultNone()
        {
            _builder = _builder.DefaultNone();
            return this;
        }

        public IJqlClauseBuilder And()
        {
            _builder = _builder.And();
            return this;
        }

        public IJqlClauseBuilder Or()
        {
            _builder = _builder.Or();
            return this;
        }

        public IJqlClauseBuilder Not()
        {
            _builder = _builder.Not();
            return this;
        }

        public IJqlClauseBuilder Sub()
        {
            _builder = _builder.Sub();
            return this;
        }

        public IJqlClauseBuilder Endsub()
        {
            _builder = _builder.Endsub();
            return this;
        }

        public IJqlClauseBuilder Manufacturer(params string[] types)
        {
            return AddStringCondition(SystemSearchConstants.ForManufacturer().JqlClauseNames.PrimaryName, types);
        }

        public IConditionBuilder Manufacturer()
        {
            return new ConditionBuilder(SystemSearchConstants.ForManufacturer().JqlClauseNames.PrimaryName, this);
        }

        public IJqlClauseBuilder Description(string value)
        {
            return AddStringCondition(SystemSearchConstants.ForDescription().JqlClauseNames.PrimaryName, Operator.LIKE, value);
        }

        public IJqlClauseBuilder DescriptionIsEmpty()
        {
            return AddEmptyCondition(SystemSearchConstants.ForDescription().JqlClauseNames.PrimaryName);
        }

        public IConditionBuilder Description()
        {
            return new DefaultConditionBuilder(SystemSearchConstants.ForDescription().JqlClauseNames.PrimaryName, this);
        }

        public IJqlClauseBuilder Summary(string value)
        {
            return AddStringCondition(SystemSearchConstants.ForSummary().JqlClauseNames.PrimaryName, Operator.LIKE, value);
        }

        public IConditionBuilder Summary()
        {
            return new DefaultConditionBuilder(SystemSearchConstants.ForSummary().JqlClauseNames.PrimaryName, this);
        }

        public IJqlClauseBuilder Comment(string value)
        {
            return AddStringCondition(SystemSearchConstants.ForComments().JqlClauseNames.PrimaryName, Operator.LIKE, value);
        }

        public IConditionBuilder Comment()
        {
            return new DefaultConditionBuilder(SystemSearchConstants.ForComments().JqlClauseNames.PrimaryName, this);
        }

        public IJqlClauseBuilder Catalog(params string[] catalogs)
        {
            return AddStringCondition(SystemSearchConstants.ForCatalog().JqlClauseNames.PrimaryName, catalogs);
        }

        public IJqlClauseBuilder Catalog(params int?[] catalogIds)
        {
            return AddNumberCondition(SystemSearchConstants.ForCatalog().JqlClauseNames.PrimaryName, catalogIds);
        }

        public IConditionBuilder Catalog()
        {
            return new DefaultConditionBuilder(SystemSearchConstants.ForCatalog().JqlClauseNames.PrimaryName, this);
        }

        public IJqlClauseBuilder CreatedAfter(DateTime startDate)
        {
            return AddDateCondition(SystemSearchConstants.ForCreatedDate().JqlClauseNames.PrimaryName, Operator.GREATER_THAN_EQUALS, startDate);
        }

        public IJqlClauseBuilder CreatedAfter(string startDate)
        {
            return AddStringCondition(SystemSearchConstants.ForCreatedDate().JqlClauseNames.PrimaryName, Operator.GREATER_THAN_EQUALS, startDate);
        }

        public IJqlClauseBuilder CreatedBetween(DateTime startDate, DateTime endDate)
        {
            return AddDateRangeCondition(SystemSearchConstants.ForCreatedDate().JqlClauseNames.PrimaryName, startDate, endDate);
        }

        public IJqlClauseBuilder CreatedBetween(string startDateString, string endDateString)
        {
            return AddStringRangeCondition(SystemSearchConstants.ForCreatedDate().JqlClauseNames.PrimaryName, startDateString, endDateString);
        }

        public IConditionBuilder Created()
        {
            return new DefaultConditionBuilder(SystemSearchConstants.ForCreatedDate().JqlClauseNames.PrimaryName, this);
        }

        public IJqlClauseBuilder UpdatedAfter(DateTime startDate)
        {
            return AddDateCondition(SystemSearchConstants.ForUpdatedDate().JqlClauseNames.PrimaryName, Operator.GREATER_THAN_EQUALS, startDate);
        }

        public IJqlClauseBuilder UpdatedAfter(string startDate)
        {
            return AddStringCondition(SystemSearchConstants.ForUpdatedDate().JqlClauseNames.PrimaryName, Operator.GREATER_THAN_EQUALS, startDate);
        }

        public IJqlClauseBuilder UpdatedBetween(DateTime startDate, DateTime endDate)
        {
            return AddDateRangeCondition(SystemSearchConstants.ForUpdatedDate().JqlClauseNames.PrimaryName, startDate, endDate);
        }

        public IJqlClauseBuilder UpdatedBetween(string startDateString, string endDateString)
        {
            return AddStringRangeCondition(SystemSearchConstants.ForUpdatedDate().JqlClauseNames.PrimaryName, startDateString, endDateString);
        }

        public IConditionBuilder Updated()
        {
            return new DefaultConditionBuilder(SystemSearchConstants.ForUpdatedDate().JqlClauseNames.PrimaryName, this);
        }

        public IJqlClauseBuilder Asset(params string[] assetIds)
        {
            return AddStringCondition(SystemSearchConstants.ForAssetId().JqlClauseNames.PrimaryName, Operator.IN, assetIds);
        }

        public IConditionBuilder Asset()
        {
            return new DefaultConditionBuilder(SystemSearchConstants.ForAssetId().JqlClauseNames.PrimaryName, this);
        }

        public IJqlClauseBuilder AttachmentsExists(bool hasAttachment)
        {
            //return AddStringCondition(SystemSearchConstants.ForAttachments().JqlClauseNames.PrimaryName, hasAttachment ? Operator.IS_NOT : Operator.IS, EmptyOperand.OPERAND_NAME);
            throw new NotImplementedException();
        }

        public IConditionBuilder Field(string jqlName)
        {
            if (jqlName.IsNullOrEmpty())
            {
                throw new ArgumentNullException("jqlName");
            }

            return new DefaultConditionBuilder(jqlName, this);
        }

        public IJqlClauseBuilder AddClause(IClause clause)
        {
            if (clause == null) throw new ArgumentNullException("clause");

            _builder = _builder.Clause(clause);
            return this;
        }

        public IJqlClauseBuilder AddDateCondition(string clauseName, Operator @operator, DateTime date)
        {
            if (clauseName == null) throw new ArgumentNullException("clauseName");

            return AddTerminalClause(clauseName, @operator, new SingleValueOperand(_jqlDateSupport.GetDateString(date)));
        }

        public IJqlClauseBuilder AddDateCondition(string clauseName, params DateTime[] dates)
        {
            if ((dates != null) && (dates.Length == 1))
            {
                return AddDateCondition(clauseName, Operator.EQUALS, dates[0]);
            }
            
            return AddDateCondition(clauseName, Operator.IN, dates);
        }

        public IJqlClauseBuilder AddDateCondition(string clauseName, Operator @operator, params DateTime[] dates)
        {
            if (clauseName == null) throw new ArgumentNullException("clauseName");
            if (dates == null) throw new ArgumentNullException("dates");

            if (!dates.Any())
            {
                throw new ArgumentException("clauseValues must not be empty.");
            }

            if (dates.Any(clauseValue => clauseValue == null))
            {
                throw new ArgumentException("No nulls are allowed", "clauseValues");
            }

            string[] args = new string[dates.Length];
            int position = 0;
            foreach (DateTime date in dates)
            {
                args[position++] = _jqlDateSupport.GetDateString(date);
            }

            return AddTerminalClause(clauseName, @operator, new MultiValueOperand(args));
        }

        public IJqlClauseBuilder AddDateCondition(string clauseName, ICollection<DateTime> dates)
        {
            if ((dates != null) && (dates.Count == 1))
            {
                dates.GetEnumerator().MoveNext();
                return AddDateCondition(clauseName, Operator.EQUALS, dates.GetEnumerator().Current);
            }
            
            return AddDateCondition(clauseName, Operator.IN, dates);
        }

        public IJqlClauseBuilder AddDateCondition(string clauseName, Operator @operator, ICollection<DateTime> dates)
        {
            if (clauseName == null) throw new ArgumentNullException("clauseName");
            if (dates == null) throw new ArgumentNullException("dates");

            if (!dates.Any())
            {
                throw new ArgumentException("clauseValues must not be empty.");
            }

            if (dates.Any(clauseValue => clauseValue == null))
            {
                throw new ArgumentException("No nulls are allowed", "clauseValues");
            }

            string[] args = new string[dates.Count];
            int position = 0;
            foreach (DateTime date in dates)
            {
                args[position++] = _jqlDateSupport.GetDateString(date);
            }

            return AddTerminalClause(clauseName, @operator, new MultiValueOperand(args));
        }

        public IJqlClauseBuilder AddDateRangeCondition(string clauseName, DateTime startDate, DateTime endDate)
        {
            IOperand startOperand = startDate == null ? null : new SingleValueOperand(_jqlDateSupport.GetDateString(startDate));
            IOperand endOperand = endDate == null ? null : new SingleValueOperand(_jqlDateSupport.GetDateString(endDate));

            return AddRangeCondition(clauseName, startOperand, endOperand);
        }

        public IJqlClauseBuilder AddFunctionCondition(string clauseName, string functionName)
        {
            return AddFunctionCondition(clauseName, Operator.EQUALS, functionName);
        }

        public IJqlClauseBuilder AddFunctionCondition(string clauseName, string functionName, params string[] args)
        {
            return AddFunctionCondition(clauseName, Operator.EQUALS, functionName, args);
        }

        public IJqlClauseBuilder AddFunctionCondition(string clauseName, string functionName, ICollection<string> args)
        {
            return AddFunctionCondition(clauseName, Operator.EQUALS, functionName, args);
        }

        public IJqlClauseBuilder AddFunctionCondition(string clauseName, Operator @operator, string functionName)
        {
            if (clauseName == null) throw new ArgumentNullException("clauseName");
            if (functionName == null) throw new ArgumentNullException("functionName");

            return AddTerminalClause(clauseName, @operator, new FunctionOperand(functionName));
        }

        public IJqlClauseBuilder AddFunctionCondition(string clauseName, Operator @operator, string functionName, params string[] args)
        {
            if (clauseName == null) throw new ArgumentNullException("clauseName");
            if (functionName == null) throw new ArgumentNullException("functionName");
            if (args == null) throw new ArgumentNullException("args");

            if (args.Any(clauseValue => clauseValue == null))
            {
                throw new ArgumentException("No nulls are allowed", "clauseValues");
            }

            return AddTerminalClause(clauseName, @operator, new FunctionOperand(functionName, args));
        }

        public IJqlClauseBuilder AddFunctionCondition(string clauseName, Operator @operator, string functionName, ICollection<string> args)
        {
            if (clauseName == null) throw new ArgumentNullException("clauseName");
            if (functionName == null) throw new ArgumentNullException("functionName");
            if (args == null) throw new ArgumentNullException("args");

            if (args.Any(clauseValue => clauseValue == null))
            {
                throw new ArgumentException("No nulls are allowed", "clauseValues");
            }

            return AddTerminalClause(clauseName, @operator, new FunctionOperand(functionName, args));
        }

        public IJqlClauseBuilder AddStringCondition(string clauseName, string clauseValue)
        {
            return AddStringCondition(clauseName, Operator.EQUALS, clauseValue);
        }

        public IJqlClauseBuilder AddStringCondition(string clauseName, params string[] clauseValues)
        {
            if ((clauseValues != null) && (clauseValues.Length == 1))
            {
                return AddStringCondition(clauseName, Operator.EQUALS, clauseValues[0]);
            }
            
            return AddStringCondition(clauseName, Operator.IN, clauseValues);
        }

        public IJqlClauseBuilder AddStringCondition(string clauseName, ICollection<string> clauseValues)
        {
            if ((clauseValues != null) && (clauseValues.Count == 1))
            {
                clauseValues.GetEnumerator().MoveNext();
                return AddStringCondition(clauseName, Operator.EQUALS, clauseValues.GetEnumerator().Current);
            }
            
            return AddStringCondition(clauseName, Operator.IN, clauseValues);
        }

        public IJqlClauseBuilder AddStringCondition(string clauseName, Operator @operator, string clauseValue)
        {
            if (clauseName == null) throw new ArgumentNullException("clauseName");
            if (clauseValue == null) throw new ArgumentNullException("clauseValue");

            return AddTerminalClause(clauseName, @operator, new SingleValueOperand(clauseValue));
        }

        public IJqlClauseBuilder AddStringCondition(string clauseName, Operator @operator, params string[] clauseValues)
        {
            if (clauseName == null) throw new ArgumentNullException("clauseName");
            if (clauseValues == null) throw new ArgumentNullException("clauseValues");

            if (!clauseValues.Any())
            {
                throw new ArgumentException("clauseValues must not be empty.");
            }

            if (clauseValues.Any(clauseValue => clauseValue == null))
            {
                throw new ArgumentException("No nulls are allowed", "clauseValues");
            }

            return AddTerminalClause(clauseName, @operator, new MultiValueOperand(clauseValues));
        }

        public IJqlClauseBuilder AddStringCondition(string clauseName, Operator @operator, ICollection<string> clauseValues)
        {
            if (clauseName == null) throw new ArgumentNullException("clauseName");
            if (clauseValues == null) throw new ArgumentNullException("clauseValues");

            if (!clauseValues.Any())
            {
                throw new ArgumentException("clauseValues must not be empty.");
            }

            if (clauseValues.Any(clauseValue => clauseValue == null))
            {
                throw new ArgumentException("No nulls are allowed", "clauseValues");
            }

            return AddTerminalClause(clauseName, @operator, new MultiValueOperand(clauseValues.ToArray()));
        }

        public IJqlClauseBuilder AddStringRangeCondition(string clauseName, string start, string end)
        {
            IOperand startClause = start == null ? null : new SingleValueOperand(start);
            IOperand endClause = end == null ? null : new SingleValueOperand(end);

            return AddRangeCondition(clauseName, startClause, endClause);
        }

        public IJqlClauseBuilder AddNumberCondition(string clauseName, int? clauseValue)
        {
            return AddNumberCondition(clauseName, Operator.EQUALS, clauseValue);
        }

        public IJqlClauseBuilder AddNumberCondition(string clauseName, params int?[] clauseValues)
        {
            if ((clauseValues != null) && (clauseValues.Length == 1))
            {
                return AddNumberCondition(clauseName, Operator.EQUALS, clauseValues[0]);
            }
            
            return AddNumberCondition(clauseName, Operator.IN, clauseValues);
        }

        public IJqlClauseBuilder AddNumberCondition(string clauseName, ICollection<int?> clauseValues)
        {
            if ((clauseValues != null) && (clauseValues.Count == 1))
            {
                clauseValues.GetEnumerator().MoveNext();
                return AddNumberCondition(clauseName, Operator.EQUALS, clauseValues.GetEnumerator().Current);
            }
            
            return AddNumberCondition(clauseName, Operator.IN, clauseValues);
        }

        public IJqlClauseBuilder AddNumberCondition(string clauseName, Operator @operator, int? clauseValue)
        {
            if (clauseName == null) throw new ArgumentNullException("clauseName");
            if (clauseValue == null) throw new ArgumentNullException("clauseValue");

            return AddTerminalClause(clauseName, @operator, new SingleValueOperand(clauseValue));
        }

        public IJqlClauseBuilder AddNumberCondition(string clauseName, Operator @operator, params int?[] clauseValues)
        {
            if (clauseName == null) throw new ArgumentNullException("clauseName");
            if (clauseValues == null) throw new ArgumentNullException("clauseValues");

            if (!clauseValues.Any())
            {
                throw new ArgumentException("clauseValues must not be empty.");
            }

            if (clauseValues.Any(clauseValue => clauseValue == null))
            {
                throw new ArgumentException("No nulls are allowed", "clauseValues");
            }

            return AddTerminalClause(clauseName, @operator, new MultiValueOperand(clauseValues));
        }

        public IJqlClauseBuilder AddNumberCondition(string clauseName, Operator @operator, ICollection<int?> clauseValues)
        {
            if (clauseName == null) throw new ArgumentNullException("clauseName");
            if (clauseValues == null) throw new ArgumentNullException("clauseValues");

            if (!clauseValues.Any())
            {
                throw new ArgumentException("clauseValues must not be empty.");
            }

            if (clauseValues.Any(clauseValue => clauseValue == null))
            {
                throw new ArgumentException("No nulls are allowed", "clauseValues");
            }

            return AddTerminalClause(clauseName, @operator, new MultiValueOperand(clauseValues.ToArray()));
        }

        public IJqlClauseBuilder AddNumberRangeCondition(string clauseName, int? start, int? end)
        {
            IOperand startClause = start == null ? null : new SingleValueOperand(start);
            IOperand endClause = end == null ? null : new SingleValueOperand(end);

            return AddRangeCondition(clauseName, startClause, endClause);
        }

        public IConditionBuilder AddCondition(string clauseName)
        {
            if (clauseName == null)
            {
                throw new ArgumentNullException("clauseName");
            }

            return new DefaultConditionBuilder(clauseName, this);
        }

        public IJqlClauseBuilder AddCondition(string clauseName, IOperand operand)
        {
            return AddCondition(clauseName, Operator.EQUALS, operand);
        }

        public IJqlClauseBuilder AddCondition(string clauseName, params IOperand[] operands)
        {
            return AddCondition(clauseName, Operator.IN, operands);
        }

        public IJqlClauseBuilder AddCondition<T>(string clauseName, ICollection<T> operands) where T : IOperand
        {
            return AddCondition(clauseName, Operator.IN, operands);
        }

        public IJqlClauseBuilder AddCondition(string clauseName, Operator @operator, IOperand operand)
        {
            if (clauseName == null) throw new ArgumentNullException("clauseName");
            if (operand == null) throw new ArgumentNullException("operand");

            return AddTerminalClause(clauseName, @operator, operand);
        }

        public IJqlClauseBuilder AddCondition(string clauseName, Operator @operator, params IOperand[] operands)
        {
            if (clauseName == null) throw new ArgumentNullException("clauseName");
            if (operands == null) throw new ArgumentNullException("operands");

            if (operands.Any(op => op == null))
            {
                throw new ArgumentException("No nulls are allowed", "clauseValues");
            }

            return AddTerminalClause(clauseName, @operator, new MultiValueOperand(operands));
        }

        public IJqlClauseBuilder AddCondition<T>(string clauseName, Operator @operator, ICollection<T> operands) where T : IOperand
        {
            if (clauseName == null) throw new ArgumentNullException("clauseName");
            if (operands == null) throw new ArgumentNullException("operands");

            if (operands.Any(op => op == null))
            {
                throw new ArgumentException("No nulls are allowed", "clauseValues");
            }

            return AddTerminalClause(clauseName, @operator, new MultiValueOperand(operands));
        }

        public IJqlClauseBuilder AddRangeCondition(string clauseName, IOperand start, IOperand end)
        {
            if (clauseName == null) throw new ArgumentNullException("clauseName");
            if ((start == null) && (end == null))
            {
                throw new ArgumentException("Start and End are both null.");
            }

            IClause clause;
            if (start != null)
            {
                if (end != null)
                {
                    TerminalClause startClause = new TerminalClause(clauseName, Operator.GREATER_THAN_EQUALS, start);
                    TerminalClause endClause = new TerminalClause(clauseName, Operator.LESS_THAN_EQUALS, end);

                    clause = new AndClause(startClause, endClause);
                }
                else
                {
                    clause = new TerminalClause(clauseName, Operator.GREATER_THAN_EQUALS, start);
                }
            }
            else
            {
                clause = new TerminalClause(clauseName, Operator.LESS_THAN_EQUALS, end);
            }

            return AddClause(clause);
        }

        public IJqlClauseBuilder AddEmptyCondition(string clauseName)
        {
            if (clauseName == null) throw new ArgumentNullException("clauseName");
            return AddTerminalClause(clauseName, Operator.IS, EmptyOperand.Empty);
        }

        private JqlClauseBuilder AddTerminalClause(string clauseName, Operator @operator, IOperand clauseValue)
        {
            _builder = _builder.Clause(new TerminalClause(clauseName, @operator, clauseValue));
            return this;
        }

        public IClause BuildClause()
        {
            return _builder.Build();
        }
    }
}