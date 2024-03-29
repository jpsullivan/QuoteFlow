﻿using QuoteFlow.Api.Infrastructure.Extensions;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.History;
using QuoteFlow.Api.Jql.Query.Operand;

namespace QuoteFlow.Core.Jql.Query.History
{
    /// <summary>
    /// Represents an assertion about a change history, namely that the expression of combining the prefix operator with
    /// the operand. The result of evaluating the expression must be true or false. For example the operator may be
    /// <see cref="Operator.BEFORE"/> and the operator (which should be a datetime).
    /// </summary>
    public class TerminalHistoryPredicate : IHistoryPredicate
    {
        public IOperand Operand { get; private set; }
        public Operator Operator { get; private set; }

        public TerminalHistoryPredicate(Operator @operator, IOperand operand)
        {
            Operator = @operator;
            Operand = operand;
        }

        public virtual string DisplayString
        {
            get
            {
                return Operator.GetDisplayAttributeFrom(typeof(Operator)) + " " + Operand.DisplayString;
            }
        }

        public T Accept<T>(IPredicateVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

}