using System;
using QuoteFlow.Api.Jql.Query.Clause;

namespace QuoteFlow.Core.Jql.Builder
{
    /// <summary>
    /// A <see cref="IMutableClause"/> that holds one JQL clause.
    /// </summary>
    internal class SingleMutableClause : IMutableClause
    {
        private readonly IClause clause;

        internal SingleMutableClause(IClause clause)
        {
            if (clause == null)
            {
                throw new ArgumentNullException("clause");
            }
            this.clause = clause;
        }

        public virtual IMutableClause Combine(BuilderOperator logicalOperator, IMutableClause otherClause)
        {
            if (logicalOperator == BuilderOperator.None)
            {
                throw new ArgumentException("logicalOperator must have another selection except `none`.");
            }

            return logicalOperator.CreateClauseForOperator(this, otherClause);
        }

        public virtual IClause AsClause()
        {
            return clause;
        }

        public virtual IMutableClause Copy()
        {
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

            SingleMutableClause that = (SingleMutableClause) obj;

            if (!clause.Equals(that.clause))
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return clause.GetHashCode();
        }

        public override string ToString()
        {
            return clause.ToString();
        }
    }
}
