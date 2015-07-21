using System;
using QuoteFlow.Api.Jql.Query.Clause;

namespace QuoteFlow.Core.Jql.Builder
{
    /// <summary>
    /// A <see cref="IMutableClause"/> that represents the negation of another MutableClause.
    /// </summary>
    public class NotMutableClause : IMutableClause
    {
        private IMutableClause Clause { get; set; }

        public NotMutableClause(IMutableClause clause)
        {
            if (clause == null)
            {
                throw new ArgumentNullException(nameof(clause));
            }

            Clause = clause;
        }

        public IMutableClause Combine(BuilderOperator logicalOperator, IMutableClause otherClause)
        {
            return logicalOperator.CreateClauseForOperator(this, otherClause);
        }

        public IClause AsClause()
        {
            IClause subclause = Clause.AsClause();
            return subclause == null ? null : new NotClause(subclause);
        }

        public IMutableClause Copy()
        {
            IMutableClause copyClause = Clause.Copy();
            if (copyClause != Clause)
            {
                return new NotMutableClause(copyClause);
            }
            return this;
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }
            if (obj == null || this.GetType() != obj.GetType())
            {
                return false;
            }

            NotMutableClause that = (NotMutableClause) obj;

            if (!Clause.Equals(that.Clause))
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return Clause.GetHashCode();
        }

        public override string ToString()
        {
            return $"{BuilderOperator.NOT}({Clause})";
        }
    }
}
