using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuoteFlow.Models.Search.Jql.Query.Clause
{
    public abstract class MultiClause : IClause
    {
        private readonly IEnumerable<IClause> _clauses;
        public virtual IEnumerable<IClause> Clauses { get { return _clauses; }} 

        public abstract T Accept<T>(IClauseVisitor<T> visitor);
		public abstract string Name {get;}

        protected MultiClause(IEnumerable<IClause> clauses)
        {
            _clauses = new List<IClause>(clauses);
        }

        public override string ToString()
		{
			var sb = new StringBuilder();
			ClausePrecedence currentPrecedence = ClausePrecedenceHelper.GetPrecedence(this);
            for (int i = 0; i < Clauses.Count(); i++)
            {
                IClause clause = Clauses.ElementAt(i);
                var childPrecedence = ClausePrecedenceHelper.GetPrecedence(clause);
                if (childPrecedence < currentPrecedence)
                {
                    sb.Append("( ");
                }
                sb.Append(clause.ToString());

                if (childPrecedence < currentPrecedence)
                {
                    sb.Append(" )");
                }

                if (i != (Clauses.Count() - 1))
                {
                    sb.Append(" ").Append(Name).Append(" ");
                }
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

			var that = (MultiClause) o;

			return _clauses != null ? _clauses.Equals(that._clauses) : that._clauses == null;
		}

		public override int GetHashCode()
		{
			return (_clauses != null ? _clauses.GetHashCode() : 0);
		}
    }
}