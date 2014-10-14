using System;
using System.Collections.Generic;
using System.Linq;

namespace QuoteFlow.Models.Search.Jql.Query.Clause
{
    /// <summary>
	/// Used to represent a logical AND in the query tree.
	/// </summary>
	public class AndClause : MultiClause
	{
		public const string And = "AND";

        public AndClause(IList<IClause> clauses) : base(clauses)
        {
            if (!clauses.Any())
            {
                throw new ArgumentException("You can not construct an AndClause without any child clauses.");
            }
        }

		public override string Name { get { return And; } }

		public override T Accept<T>(IClauseVisitor<T> visitor)
		{
			return visitor.Visit(this);
		}
	}
}