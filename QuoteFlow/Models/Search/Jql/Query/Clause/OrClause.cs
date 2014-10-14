using System;
using System.Collections.Generic;
using System.Linq;

namespace QuoteFlow.Models.Search.Jql.Query.Clause
{
    /// <summary>
    /// Used to represent a logical OR in the query tree.
    /// </summary>
    public class OrClause : MultiClause
    {
        private const string Or = "OR";

        public OrClause(IList<IClause> clauses) : base(clauses)
        {
            if (!clauses.Any())
            {
                throw new ArgumentException("You can not construct an OrClause without any child clauses.");
            }
        }

		public override string Name
		{
			get { return Or; }
		}

		public override T Accept<T>(IClauseVisitor<T> visitor)
		{
			return visitor.Visit(this);
		}
    }
}