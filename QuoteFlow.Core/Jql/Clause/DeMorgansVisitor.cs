using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Core.Jql.Query.Clause;

namespace QuoteFlow.Core.Jql.Clause
{
    /// <summary>
    /// Expands the not clauses in a clause tree using DeMorgans law
    /// and flips the operators to remove the not alltogether when possible.
    /// </summary>
    public class DeMorgansVisitor : IClauseVisitor<IClause>
    {
        private int _notCount;

        public IClause Visit(AndClause andClause)
        {
            IList<IClause> nodes = VisitChildren(andClause);

            if (Negating)
            {
                return new OrClause(nodes);
            }
            return new AndClause(nodes);
        }

        public IClause Visit(NotClause notClause)
        {
            _notCount++;
            IClause logicalClause = notClause.SubClause.Accept(this);
            _notCount--;
            return logicalClause;
        }

        public IClause Visit(OrClause orClause)
        {
            IList<IClause> nodes = VisitChildren(orClause);

            if (Negating)
            {
                return new AndClause(nodes);
            }
            return new OrClause(nodes);
        }

        public IClause Visit(ITerminalClause clause)
        {
            if (Negating)
            {
                Operator notOperator = GetNotOperator(clause.Operator);
                if (notOperator != null)
                {
                    return new TerminalClause(clause.Name, notOperator, clause.Operand, clause.Property);
                }
                
                // NOTE: this should never happen as we have a NOT version of every operator we use.
                return new NotClause(clause);
            }

            return clause;
        }

        public IClause Visit(IWasClause clause)
        {
            if (Negating)
            {
                return new WasClause(clause.Name, GetNotOperator(clause.Operator), clause.Operand, clause.Predicate);
            }
            return clause;
        }

        public IClause Visit(IChangedClause clause)
        {
            if (Negating)
            {
                return new ChangedClause(clause.Field, GetNotOperator(clause.Operator), clause.Predicate);
            }
            return clause;
        }

        private bool Negating
        {
            get
            {
                // If the notCount is odd then this is true, otherwise it is false
                return (_notCount%2) == 1;
            }
        }

        private IList<IClause> VisitChildren(MultiClause multiClause)
        {
            var nodes = new List<IClause>(multiClause.Clauses.Count());
            foreach (IClause logicalClause in multiClause.Clauses)
            {
                nodes.Add(logicalClause.Accept(this));
            }
            return nodes;
        }

        private static Operator GetNotOperator(Operator @operator)
        {
            switch (@operator)
            {
                case Operator.IS:
                    return Operator.IS_NOT;
                case Operator.IS_NOT:
                    return Operator.IS;
                case Operator.IN:
                    return Operator.NOT_IN;
                case Operator.NOT_IN:
                    return Operator.IN;
                case Operator.LIKE:
                    return Operator.NOT_LIKE;
                case Operator.NOT_LIKE:
                    return Operator.LIKE;
                case Operator.EQUALS:
                    return Operator.NOT_EQUALS;
                case Operator.NOT_EQUALS:
                    return Operator.EQUALS;
                case Operator.GREATER_THAN:
                    return Operator.LESS_THAN_EQUALS;
                case Operator.GREATER_THAN_EQUALS:
                    return Operator.LESS_THAN;
                case Operator.LESS_THAN:
                    return Operator.GREATER_THAN_EQUALS;
                case Operator.LESS_THAN_EQUALS:
                    return Operator.GREATER_THAN;
                case Operator.WAS:
                    return Operator.WAS_NOT;
                case Operator.WAS_NOT:
                    return Operator.WAS;
                case Operator.CHANGED:
                    return Operator.NOT_CHANGED;
                case Operator.NOT_CHANGED:
                    return Operator.CHANGED;
            }

            // default return type
            return Operator.LIKE;
        }
    }
}