using System;

namespace QuoteFlow.Core.Jql.Builder
{
    /// <summary>
	/// Represents the logical operators that the builder deals with.
	/// </summary>
    public enum BuilderOperator
    {
        // The order of these operators is important. The BuilderOperator.compareTo method must
        // order them in precedence order.
        LPAREN,
        RPAREN,
        OR,
        AND,
        NOT,
        None = 0,
    }

    public static class BuilderOperatorExtensions
    {
        public static IMutableClause CreateClauseForOperator(this BuilderOperator builderOp, IMutableClause left, IMutableClause right)
        {
            switch (builderOp)
            {
                case BuilderOperator.LPAREN:
                    throw new NotSupportedException();
                case BuilderOperator.RPAREN:
                    throw new NotSupportedException();
                case BuilderOperator.OR:
                    if (left == null)
                    {
                        throw new ArgumentNullException("left");
                    }

                    if (right == null)
                    {
                        throw new ArgumentNullException("right");
                    }

                    return new MultiMutableClause<IMutableClause>(builderOp, left, right);
                case BuilderOperator.AND:
                    if (left == null)
                    {
                        throw new ArgumentNullException("left");
                    }

                    if (right == null)
                    {
                        throw new ArgumentNullException("right");
                    }

                    return new MultiMutableClause<IMutableClause>(builderOp, left, right);
                case BuilderOperator.NOT:
                    if (left == null)
                    {
                        throw new ArgumentNullException("left");
                    }

                    return new NotMutableClause(left);
            }

            throw new InvalidOperationException();
        }
    }
}