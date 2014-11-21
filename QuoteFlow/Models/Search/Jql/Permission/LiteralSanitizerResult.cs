using System.Collections.Generic;
using QuoteFlow.Models.Search.Jql.Operand;

namespace QuoteFlow.Models.Search.Jql.Permission
{
    /// <summary>
    /// Dictates the result of sanitising a list of <see cref="QueryLiteral"/>s. If no modifications
    /// were made on the input, then the value returned by <see cref="GetLiterals()"/> is not guaranteed to be meaningful.
    /// </summary>
    public class LiteralSanitizerResult
    {
        public virtual bool Modified { get; private set; }

        public virtual IList<QueryLiteral> Literals { get; private set; }

        public LiteralSanitizerResult(bool modified, IList<QueryLiteral> literals)
		{
			this.Modified = modified;
			this.Literals = literals;
		}
    }
}