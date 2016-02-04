using System;
using System.Collections.Generic;
using System.Linq;

namespace QuoteFlow.Api.Jql.Query.Clause
{
    /// <summary>
    /// Used to represent a logical OR in the query tree.
    /// </summary>
    public class OrClause : MultiClause
    {
        public const string Or = "OR";

        public OrClause(params IClause[] clauses) : this(clauses.ToList())
        {
        }

        public OrClause(ICollection<IClause> clauses) : base(clauses)
        {
            if (!clauses.Any())
            {
                throw new ArgumentException("You can not construct an OrClause without any child clauses.");
            }
        }

		public override string Name => Or;

        public override T Accept<T>(IClauseVisitor<T> visitor)
		{
			return visitor.Visit(this);
		}
    }
}