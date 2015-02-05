using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuoteFlow.Api.Jql.Query.Clause;

namespace QuoteFlow.Core.Jql.Builder
{
    /// <summary>
	/// A <seealso cref="IMutableClause"/> that represents a collection of MutableClauses joined by
	/// either an <seealso cref="BuilderOperator.AND"/> or an {@link
	/// BuilderOperator#OR}.
	/// </summary>
    public class MultiMutableClause<T> : IMutableClause where T : IMutableClause
    {
        private List<IMutableClause> Clauses { get; set; }
        private BuilderOperator LogicalOperator { get; set; }

        public MultiMutableClause(BuilderOperator logicalOperator, params IMutableClause[] clauses) : this(logicalOperator, clauses.ToList())
		{
		}

        private MultiMutableClause(BuilderOperator logicalOperator, List<IMutableClause> clauses)
        {
            if (logicalOperator != BuilderOperator.AND && logicalOperator != BuilderOperator.OR)
			{
				throw new ArgumentException("logicalOperator must be 'AND' or 'OR'.");
			}

            if (clauses == null)
            {
                throw new ArgumentNullException("");
            }

            if (!clauses.Any())
            {
                throw new ArgumentException("clauses is empty.");
            }

			LogicalOperator = logicalOperator;
			Clauses.AddRange(clauses);
        }

        public IMutableClause Combine(BuilderOperator logicalOperator, IMutableClause otherClause)
        {
			if (LogicalOperator == logicalOperator)
			{
			    if (otherClause == null)
			    {
			        throw new ArgumentNullException("otherClause");
			    }

				Clauses.Add(otherClause);
				return this;
			}
            
            return logicalOperator.CreateClauseForOperator(this, otherClause);
        }

        public IClause AsClause()
        {
            var newClauses = Clauses.Select(mc => mc.AsClause()).Where(c => c != null).ToList();

            if (!newClauses.Any())
			{
				return null;
			}
            
            if (newClauses.Count == 1)
            {
                return newClauses.First();
            }
            
            if (LogicalOperator == BuilderOperator.AND)
            {
                return new AndClause(newClauses);
            }
            
            if (LogicalOperator == BuilderOperator.OR)
            {
                return new OrClause(newClauses);
            }
            
            throw new InvalidOperationException();
        }

        public IMutableClause Copy()
        {
            var copiedClauses = Clauses.Select(mutableClause => mutableClause.Copy()).ToList();
            return new MultiMutableClause<T>(LogicalOperator, copiedClauses);
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

            var that = (MultiMutableClause<T>) obj;

            if (!Clauses.SequenceEqual(that.Clauses))
            {
                return false;
            }
            if (LogicalOperator != that.LogicalOperator)
            {
                return false;
            }

            return true;
        }

		public override int GetHashCode()
		{
			int result = Clauses.GetHashCode();
			result = 31 * result + LogicalOperator.GetHashCode();
			return result;
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder("(");
			bool first = true;
			foreach (IMutableClause clause in Clauses)
			{
				if (!first)
				{
					builder.Append(' ').Append(LogicalOperator).Append(' ');
				}
				else
				{
					first = false;
				}

				builder.Append(clause);
			}
			return builder.Append(")").ToString();
		}
    }
}
