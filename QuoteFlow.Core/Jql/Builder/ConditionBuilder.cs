using System;
using System.Collections.Generic;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Operand;

namespace QuoteFlow.Core.Jql.Builder
{
    /// <summary>
    /// Default implementation of <see cref="IConditionBuilder"/>.
    /// </summary>
    public class ConditionBuilder : IConditionBuilder
    {
        private readonly IJqlClauseBuilder _builder;
        private readonly string _clauseName;

        public ConditionBuilder(string clauseName, IJqlClauseBuilder builder)
        {
            if (clauseName == null) throw new ArgumentNullException("clauseName");
            if (builder == null) throw new ArgumentNullException("builder");

            _builder = builder;
            _clauseName = clauseName;
        }

        public virtual IValueBuilder Eq()
        {
            return new DefaultValueBuilder(_builder, _clauseName, Operator.EQUALS);
        }

        public virtual IJqlClauseBuilder Eq(string value)
        {
            return _builder.AddStringCondition(_clauseName, Operator.EQUALS, value);
        }

        public virtual IJqlClauseBuilder Eq(int? value)
        {
            return _builder.AddNumberCondition(_clauseName, Operator.EQUALS, value);
        }

        public virtual IJqlClauseBuilder Eq(DateTime date)
        {
            return _builder.AddDateCondition(_clauseName, Operator.EQUALS, date);
        }

        public virtual IJqlClauseBuilder Eq(IOperand operand)
        {
            return _builder.AddCondition(_clauseName, Operator.EQUALS, operand);
        }

        public virtual IJqlClauseBuilder EqEmpty()
        {
            return _builder.AddCondition(_clauseName, Operator.EQUALS, EmptyOperand.Empty);
        }

        public virtual IJqlClauseBuilder EqFunc(string funcName)
        {
            return _builder.AddFunctionCondition(_clauseName, Operator.EQUALS, funcName);
        }

        public virtual IJqlClauseBuilder EqFunc(string funcName, params string[] args)
        {
            return _builder.AddFunctionCondition(_clauseName, Operator.EQUALS, funcName, args);
        }

        public virtual IJqlClauseBuilder EqFunc(string funcName, ICollection<string> args)
        {
            return _builder.AddFunctionCondition(_clauseName, Operator.EQUALS, funcName, args);
        }

        public virtual IValueBuilder NotEq()
        {
            return new DefaultValueBuilder(_builder, _clauseName, Operator.NOT_EQUALS);
        }

        public virtual IJqlClauseBuilder NotEq(string value)
        {
            return _builder.AddStringCondition(_clauseName, Operator.NOT_EQUALS, value);
        }

        public virtual IJqlClauseBuilder NotEq(int? value)
        {
            return _builder.AddNumberCondition(_clauseName, Operator.NOT_EQUALS, value);
        }

        public virtual IJqlClauseBuilder NotEq(IOperand operand)
        {
            return _builder.AddCondition(_clauseName, Operator.NOT_EQUALS, operand);
        }

        public virtual IJqlClauseBuilder NotEq(DateTime date)
        {
            return _builder.AddDateCondition(_clauseName, Operator.NOT_EQUALS, date);
        }

        public virtual IJqlClauseBuilder NotEqEmpty()
        {
            return _builder.AddCondition(_clauseName, Operator.NOT_EQUALS, EmptyOperand.Empty);
        }

        public virtual IJqlClauseBuilder NotEqFunc(string funcName)
        {
            return _builder.AddFunctionCondition(_clauseName, Operator.NOT_EQUALS, funcName);
        }

        public virtual IJqlClauseBuilder NotEqFunc(string funcName, params string[] args)
        {
            return _builder.AddFunctionCondition(_clauseName, Operator.NOT_EQUALS, funcName, args);
        }

        public virtual IJqlClauseBuilder NotEqFunc(string funcName, ICollection<string> args)
        {
            return _builder.AddFunctionCondition(_clauseName, Operator.NOT_EQUALS, funcName, args);
        }

        public virtual IValueBuilder Like()
        {
            return new DefaultValueBuilder(_builder, _clauseName, Operator.LIKE);
        }

        public virtual IJqlClauseBuilder Like(string value)
        {
            return _builder.AddStringCondition(_clauseName, Operator.LIKE, value);
        }

        public virtual IJqlClauseBuilder Like(int? value)
        {
            return _builder.AddNumberCondition(_clauseName, Operator.LIKE, value);
        }

        public virtual IJqlClauseBuilder Like(IOperand operand)
        {
            return _builder.AddCondition(_clauseName, Operator.LIKE, operand);
        }

        public virtual IJqlClauseBuilder Like(DateTime date)
        {
            return _builder.AddDateCondition(_clauseName, Operator.LIKE, date);
        }

        public virtual IJqlClauseBuilder LikeFunc(string funcName)
        {
            return _builder.AddFunctionCondition(_clauseName, Operator.LIKE, funcName);
        }

        public virtual IJqlClauseBuilder LikeFunc(string funcName, params string[] args)
        {
            return _builder.AddFunctionCondition(_clauseName, Operator.LIKE, funcName, args);
        }

        public virtual IJqlClauseBuilder LikeFunc(string funcName, ICollection<string> args)
        {
            return _builder.AddFunctionCondition(_clauseName, Operator.LIKE, funcName, args);
        }

        public virtual IValueBuilder NotLike()
        {
            return new DefaultValueBuilder(_builder, _clauseName, Operator.NOT_LIKE);
        }

        public virtual IJqlClauseBuilder NotLike(string value)
        {
            return _builder.AddStringCondition(_clauseName, Operator.NOT_LIKE, value);
        }

        public virtual IJqlClauseBuilder NotLike(int? value)
        {
            return _builder.AddNumberCondition(_clauseName, Operator.NOT_LIKE, value);
        }

        public virtual IJqlClauseBuilder NotLike(IOperand operand)
        {
            return _builder.AddCondition(_clauseName, Operator.NOT_LIKE, operand);
        }

        public virtual IJqlClauseBuilder NotLike(DateTime date)
        {
            return _builder.AddDateCondition(_clauseName, Operator.NOT_LIKE, date);
        }

        public virtual IJqlClauseBuilder NotLikeFunc(string funcName)
        {
            return _builder.AddFunctionCondition(_clauseName, Operator.NOT_LIKE, funcName);
        }

        public virtual IJqlClauseBuilder NotLikeFunc(string funcName, params string[] args)
        {
            return _builder.AddFunctionCondition(_clauseName, Operator.NOT_LIKE, funcName, args);
        }

        public virtual IJqlClauseBuilder NotLikeFunc(string funcName, ICollection<string> args)
        {
            return _builder.AddFunctionCondition(_clauseName, Operator.NOT_LIKE, funcName, args);
        }

        public virtual IValueBuilder Is()
        {
            return new DefaultValueBuilder(_builder, _clauseName, Operator.IS);
        }

        public virtual IJqlClauseBuilder Empty
        {
            get { return _builder.AddCondition(_clauseName, Operator.IS, EmptyOperand.Empty); }
        }

        public virtual IValueBuilder IsNot()
        {
            return new DefaultValueBuilder(_builder, _clauseName, Operator.IS_NOT);
        }

        public virtual IJqlClauseBuilder NotEmpty
        {
            get { return _builder.AddCondition(_clauseName, Operator.IS_NOT, EmptyOperand.Empty); }
        }

        public virtual IValueBuilder Lt()
        {
            return new DefaultValueBuilder(_builder, _clauseName, Operator.LESS_THAN);
        }

        public virtual IJqlClauseBuilder Lt(string value)
        {
            return _builder.AddStringCondition(_clauseName, Operator.LESS_THAN, value);
        }

        public virtual IJqlClauseBuilder Lt(int? value)
        {
            return _builder.AddNumberCondition(_clauseName, Operator.LESS_THAN, value);
        }

        public virtual IJqlClauseBuilder Lt(IOperand operand)
        {
            return _builder.AddCondition(_clauseName, Operator.LESS_THAN, operand);
        }

        public virtual IJqlClauseBuilder Lt(DateTime date)
        {
            return _builder.AddDateCondition(_clauseName, Operator.LESS_THAN, date);
        }

        public virtual IJqlClauseBuilder LtFunc(string funcName)
        {
            return _builder.AddFunctionCondition(_clauseName, Operator.LESS_THAN, funcName);
        }

        public virtual IJqlClauseBuilder LtFunc(string funcName, params string[] args)
        {
            return _builder.AddFunctionCondition(_clauseName, Operator.LESS_THAN, funcName, args);
        }

        public virtual IJqlClauseBuilder LtFunc(string funcName, ICollection<string> args)
        {
            return _builder.AddFunctionCondition(_clauseName, Operator.LESS_THAN, funcName, args);
        }

        public virtual IValueBuilder LtEq()
        {
            return new DefaultValueBuilder(_builder, _clauseName, Operator.LESS_THAN_EQUALS);
        }

        public virtual IJqlClauseBuilder LtEq(string value)
        {
            return _builder.AddStringCondition(_clauseName, Operator.LESS_THAN_EQUALS, value);
        }

        public virtual IJqlClauseBuilder LtEq(int? value)
        {
            return _builder.AddNumberCondition(_clauseName, Operator.LESS_THAN_EQUALS, value);
        }

        public virtual IJqlClauseBuilder LtEq(IOperand operand)
        {
            return _builder.AddCondition(_clauseName, Operator.LESS_THAN_EQUALS, operand);
        }

        public virtual IJqlClauseBuilder LtEq(DateTime date)
        {
            return _builder.AddDateCondition(_clauseName, Operator.LESS_THAN_EQUALS, date);
        }

        public virtual IJqlClauseBuilder LtEqFunc(string funcName)
        {
            return _builder.AddFunctionCondition(_clauseName, Operator.LESS_THAN_EQUALS, funcName);
        }

        public virtual IJqlClauseBuilder LtEqFunc(string funcName, params string[] args)
        {
            return _builder.AddFunctionCondition(_clauseName, Operator.LESS_THAN_EQUALS, funcName, args);
        }

        public virtual IJqlClauseBuilder LtEqFunc(string funcName, ICollection<string> args)
        {
            return _builder.AddFunctionCondition(_clauseName, Operator.LESS_THAN_EQUALS, funcName, args);
        }

        public virtual IValueBuilder Gt()
        {
            return new DefaultValueBuilder(_builder, _clauseName, Operator.GREATER_THAN);
        }

        public virtual IJqlClauseBuilder Gt(string value)
        {
            return _builder.AddStringCondition(_clauseName, Operator.GREATER_THAN, value);
        }

        public virtual IJqlClauseBuilder Gt(int? value)
        {
            return _builder.AddNumberCondition(_clauseName, Operator.GREATER_THAN, value);
        }

        public virtual IJqlClauseBuilder Gt(IOperand operand)
        {
            return _builder.AddCondition(_clauseName, Operator.GREATER_THAN, operand);
        }

        public virtual IJqlClauseBuilder Gt(DateTime date)
        {
            return _builder.AddDateCondition(_clauseName, Operator.GREATER_THAN, date);
        }

        public virtual IJqlClauseBuilder GtFunc(string funcName)
        {
            return _builder.AddFunctionCondition(_clauseName, Operator.GREATER_THAN, funcName);
        }

        public virtual IJqlClauseBuilder GtFunc(string funcName, params string[] args)
        {
            return _builder.AddFunctionCondition(_clauseName, Operator.GREATER_THAN, funcName, args);
        }

        public virtual IJqlClauseBuilder GtFunc(string funcName, ICollection<string> args)
        {
            return _builder.AddFunctionCondition(_clauseName, Operator.GREATER_THAN, funcName, args);
        }

        public virtual IValueBuilder GtEq()
        {
            return new DefaultValueBuilder(_builder, _clauseName, Operator.GREATER_THAN_EQUALS);
        }

        public virtual IJqlClauseBuilder GtEq(string value)
        {
            return _builder.AddStringCondition(_clauseName, Operator.GREATER_THAN_EQUALS, value);
        }

        public virtual IJqlClauseBuilder GtEq(int? value)
        {
            return _builder.AddNumberCondition(_clauseName, Operator.GREATER_THAN_EQUALS, value);
        }

        public virtual IJqlClauseBuilder GtEq(IOperand operand)
        {
            return _builder.AddCondition(_clauseName, Operator.GREATER_THAN_EQUALS, operand);
        }

        public virtual IJqlClauseBuilder GtEq(DateTime date)
        {
            return _builder.AddDateCondition(_clauseName, Operator.GREATER_THAN_EQUALS, date);
        }

        public virtual IJqlClauseBuilder GtEqFunc(string funcName)
        {
            return _builder.AddFunctionCondition(_clauseName, Operator.GREATER_THAN_EQUALS, funcName);
        }

        public virtual IJqlClauseBuilder GtEqFunc(string funcName, params string[] args)
        {
            return _builder.AddFunctionCondition(_clauseName, Operator.GREATER_THAN_EQUALS, funcName, args);
        }

        public virtual IJqlClauseBuilder GtEqFunc(string funcName, ICollection<string> args)
        {
            return _builder.AddFunctionCondition(_clauseName, Operator.GREATER_THAN_EQUALS, funcName, args);
        }

        public virtual IValueBuilder In()
        {
            return new DefaultValueBuilder(_builder, _clauseName, Operator.IN);
        }

        public virtual IJqlClauseBuilder In(params string[] values)
        {
            return _builder.AddStringCondition(_clauseName, Operator.IN, values);
        }

        public virtual IJqlClauseBuilder InStrings(ICollection<string> values)
        {
            return _builder.AddStringCondition(_clauseName, Operator.IN, values);
        }

        public virtual IJqlClauseBuilder In(params int?[] values)
        {
            return _builder.AddNumberCondition(_clauseName, Operator.IN, values);
        }

        public virtual IJqlClauseBuilder InNumbers(ICollection<int?> values)
        {
            return _builder.AddNumberCondition(_clauseName, Operator.IN, values);
        }

        public virtual IJqlClauseBuilder In(params IOperand[] operands)
        {
            return _builder.AddCondition(_clauseName, Operator.IN, operands);
        }

        public virtual IJqlClauseBuilder InOperands(ICollection<IOperand> operands)
        {
            return _builder.AddCondition(_clauseName, Operator.IN, operands);
        }

        public virtual IJqlClauseBuilder In(params DateTime[] dates)
        {
            return _builder.AddDateCondition(_clauseName, Operator.IN, dates);
        }

        public virtual IJqlClauseBuilder InDates(ICollection<DateTime> dates)
        {
            return _builder.AddDateCondition(_clauseName, Operator.IN, dates);
        }

        public virtual IJqlClauseBuilder InFunc(string funcName)
        {
            return _builder.AddFunctionCondition(_clauseName, Operator.IN, funcName);
        }

        public virtual IJqlClauseBuilder InFunc(string funcName, params string[] args)
        {
            return _builder.AddFunctionCondition(_clauseName, Operator.IN, funcName, args);
        }

        public virtual IJqlClauseBuilder InFunc(string funcName, ICollection<string> args)
        {
            return _builder.AddFunctionCondition(_clauseName, Operator.IN, funcName, args);
        }

        public virtual IValueBuilder NotIn()
        {
            return new DefaultValueBuilder(_builder, _clauseName, Operator.NOT_IN);
        }

        public virtual IJqlClauseBuilder NotIn(params string[] values)
        {
            return _builder.AddStringCondition(_clauseName, Operator.NOT_IN, values);
        }

        public virtual IJqlClauseBuilder NotInStrings(ICollection<string> values)
        {
            return _builder.AddStringCondition(_clauseName, Operator.NOT_IN, values);
        }

        public virtual IJqlClauseBuilder NotIn(params int?[] values)
        {
            return _builder.AddNumberCondition(_clauseName, Operator.NOT_IN, values);
        }

        public virtual IJqlClauseBuilder NotInNumbers(ICollection<int?> values)
        {
            return _builder.AddNumberCondition(_clauseName, Operator.NOT_IN, values);
        }

        public virtual IJqlClauseBuilder NotIn(params IOperand[] operands)
        {
            return _builder.AddCondition(_clauseName, Operator.NOT_IN, operands);
        }

        public virtual IJqlClauseBuilder NotIn(params DateTime[] dates)
        {
            return _builder.AddDateCondition(_clauseName, Operator.NOT_IN, dates);
        }

        public virtual IJqlClauseBuilder NotInDates(ICollection<DateTime> dates)
        {
            return _builder.AddDateCondition(_clauseName, Operator.NOT_IN, dates);
        }

        public virtual IJqlClauseBuilder NotInOperands(ICollection<IOperand> operands)
        {
            return _builder.AddCondition(_clauseName, Operator.NOT_IN, operands);
        }

        public virtual IJqlClauseBuilder NotInFunc(string funcName)
        {
            return _builder.AddFunctionCondition(_clauseName, Operator.NOT_IN, funcName);
        }

        public virtual IJqlClauseBuilder NotInFunc(string funcName, params string[] args)
        {
            return _builder.AddFunctionCondition(_clauseName, Operator.NOT_IN, funcName, args);
        }

        public virtual IJqlClauseBuilder NotInFunc(string funcName, ICollection<string> args)
        {
            return _builder.AddFunctionCondition(_clauseName, Operator.NOT_IN, funcName, args);
        }

        public virtual IJqlClauseBuilder Range(DateTime start, DateTime end)
        {
            return _builder.AddDateRangeCondition(_clauseName, start, end);
        }

        public virtual IJqlClauseBuilder Range(string start, string end)
        {
            return _builder.AddStringRangeCondition(_clauseName, start, end);
        }

        public virtual IJqlClauseBuilder Range(int? start, int? end)
        {
            return _builder.AddNumberRangeCondition(_clauseName, start, end);
        }

        public virtual IJqlClauseBuilder Range(IOperand start, IOperand end)
        {
            return _builder.AddRangeCondition(_clauseName, start, end);
        }
    }
}
