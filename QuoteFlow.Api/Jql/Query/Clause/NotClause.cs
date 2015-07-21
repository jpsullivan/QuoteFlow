using System;
using System.Collections.Generic;
using System.Text;

namespace QuoteFlow.Api.Jql.Query.Clause
{
    /// <summary>
    /// Used to represent a logical NOT in the query tree.
    /// </summary>
    public class NotClause : IClause
    {
        public const string NOT = "NOT";

        private readonly IClause _subClause;

        public NotClause()
        {
        }

        public NotClause(IClause subClause)
        {
            if (subClause == null) throw new ArgumentNullException(nameof(subClause));

            _subClause = subClause;
        }

        public virtual string Name
        {
            get { return NOT; }
        }

        public virtual IEnumerable<IClause> Clauses
        {
            get { return new List<IClause> {_subClause}; }
        }

        public virtual T Accept<T>(IClauseVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public virtual IClause SubClause
        {
            get
            {
                return _subClause;
            }
        }

        public override string ToString()
        {
            var currentPrecedence = ClausePrecedenceHelper.GetPrecedence(this);
            var subClausePrecedence = ClausePrecedenceHelper.GetPrecedence(_subClause);
            var sb = (new StringBuilder(NOT)).Append(" ");
            if (subClausePrecedence < currentPrecedence)
            {
                sb.Append("( ");
            }

            sb.Append(_subClause.ToString());

            if (subClausePrecedence < currentPrecedence)
            {
                sb.Append(" )");
            }

            return sb.ToString();
        }

        public override bool Equals(object o)
        {
            if (this == o)
            {
                return true;
            }
            if (o == null || GetType() != o.GetType())
            {
                return false;
            }

            var notClause = (NotClause) o;

            return _subClause.Equals(notClause._subClause);
        }

        public override int GetHashCode()
        {
            return _subClause.GetHashCode();
        }
    }
}