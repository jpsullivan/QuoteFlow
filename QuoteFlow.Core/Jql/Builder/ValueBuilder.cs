using System;
using System.Collections.Generic;
using QuoteFlow.Api.Infrastructure.Extensions;
using QuoteFlow.Api.Jql.Function;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Operand;
using QuoteFlow.Core.Jql.Function;

namespace QuoteFlow.Core.Jql.Builder
{
    /// <summary>
    /// Default implementation of <seealso cref="ValueBuilder"/>.
    /// 
    /// @since v4.0
    /// </summary>
    public class ValueBuilder : IValueBuilder
    {
        private readonly IJqlClauseBuilder _builder;
        private readonly string _clauseName;
        private readonly Operator _operator;

        public ValueBuilder(IJqlClauseBuilder builder, string clauseName, Operator @operator)
        {
            if (builder == null) throw new ArgumentNullException("builder");
            if (clauseName == null) throw new ArgumentNullException("clauseName");

            _builder = builder;
            _clauseName = clauseName;
            _operator = @operator;
        }

        public virtual IJqlClauseBuilder String(string value)
        {
            return _builder.AddStringCondition(_clauseName, _operator, value);
        }

        public virtual IJqlClauseBuilder Strings(params string[] values)
        {
            return _builder.AddStringCondition(_clauseName, _operator, values);
        }

        public virtual IJqlClauseBuilder Strings(ICollection<string> values)
        {
            return _builder.AddStringCondition(_clauseName, _operator, values);
        }

        public virtual IJqlClauseBuilder Number(int? value)
        {
            return _builder.AddNumberCondition(_clauseName, _operator, value);
        }

        public virtual IJqlClauseBuilder Numbers(params int?[] value)
        {
            return _builder.AddNumberCondition(_clauseName, _operator, value);
        }

        public virtual IJqlClauseBuilder Numbers(ICollection<int?> value)
        {
            return _builder.AddNumberCondition(_clauseName, _operator, value);
        }

        public virtual IJqlClauseBuilder Operand(IOperand operand)
        {
            return _builder.AddCondition(_clauseName, _operator, operand);
        }

        public virtual IJqlClauseBuilder Operands(params IOperand[] operands)
        {
            return _builder.AddCondition(_clauseName, _operator, operands);
        }

        public virtual IJqlClauseBuilder Operands<T>(ICollection<T> operands) where T : IOperand
        {
            return _builder.AddCondition(_clauseName, _operator, operands);
        }

        public virtual IJqlClauseBuilder Empty()
        {
            return _builder.AddCondition(_clauseName, _operator, EmptyOperand.Empty);
        }

        public virtual IJqlClauseBuilder Function(string funcName)
        {
            return _builder.AddFunctionCondition(_clauseName, _operator, funcName);
        }

        public virtual IJqlClauseBuilder Function(string funcName, params string[] args)
        {
            return _builder.AddFunctionCondition(_clauseName, _operator, funcName, args);
        }

        public virtual IJqlClauseBuilder Function(string funcName, ICollection<string> args)
        {
            return _builder.AddFunctionCondition(_clauseName, _operator, funcName, args);
        }

        public virtual IJqlClauseBuilder FunctionMembersOf(string groupName)
        {
            if (groupName.IsNullOrEmpty()) throw new ArgumentNullException("groupName");

            return Function(MembersOfFunction.FUNCTION_MEMBERSOF, groupName);
        }

        public virtual IJqlClauseBuilder FunctionCurrentUser()
        {
            return Function(CurrentUserFunction.FUNCTION_CURRENT_USER);
        }

        public IJqlClauseBuilder FunctionNow()
        {
            //return Function(NowFunction.FUNCTION_NOW);
            throw new NotImplementedException();
        }

        public virtual IJqlClauseBuilder FunctionCascadingOption(string parent)
        {
            //return Function(CascadeOptionFunction.FUNCTION_CASCADE_OPTION, parent);
            throw new NotImplementedException();
        }

        public virtual IJqlClauseBuilder FunctionCascadingOption(string parent, string child)
        {
            //return Function(CascadeOptionFunction.FUNCTION_CASCADE_OPTION, parent, child);
            throw new NotImplementedException();
        }

        public virtual IJqlClauseBuilder FunctionCascadingOptionParentOnly(string parent)
        {
            //return Function(CascadeOptionFunction.FUNCTION_CASCADE_OPTION, parent, CascadeOptionFunction.QUOTED_EMPTY_VALUE);
            throw new NotImplementedException();
        }

        public virtual IJqlClauseBuilder FunctionLastLogin()
        {
            return Function(LastLoginFunction.FUNCTION_LAST_LOGIN);
        }

        public virtual IJqlClauseBuilder FunctionCurrentLogin()
        {
            //return Function(CurrentLoginFunction.FUNCTION_CURRENT_LOGIN);
            throw new NotImplementedException();
        }

        public virtual IJqlClauseBuilder Date(DateTime date)
        {
            return _builder.AddDateCondition(_clauseName, _operator, date);
        }

        public virtual IJqlClauseBuilder Dates(params DateTime[] dates)
        {
            return _builder.AddDateCondition(_clauseName, _operator, dates);
        }

        public virtual IJqlClauseBuilder Dates(ICollection<DateTime> dates)
        {
            return _builder.AddDateCondition(_clauseName, _operator, dates);
        }
    }
}
