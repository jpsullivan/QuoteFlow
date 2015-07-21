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
        None,
        LPAREN,
        RPAREN,
        OR,
        AND,
        NOT
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
                        throw new ArgumentNullException(nameof(left));
                    }

                    if (right == null)
                    {
                        throw new ArgumentNullException(nameof(right));
                    }

                    return new MultiMutableClause<IMutableClause>(builderOp, left, right);
                case BuilderOperator.AND:
                    if (left == null)
                    {
                        throw new ArgumentNullException(nameof(left));
                    }

                    if (right == null)
                    {
                        throw new ArgumentNullException(nameof(right));
                    }

                    return new MultiMutableClause<IMutableClause>(builderOp, left, right);
                case BuilderOperator.NOT:
                    if (left == null)
                    {
                        throw new ArgumentNullException(nameof(left));
                    }

                    return new NotMutableClause(left);
            }

            throw new InvalidOperationException();
        }

        public static int CompareOperator(this BuilderOperator builderOp, BuilderOperator otherOp)
        {
            var sortOrder = new[]
            {
                BuilderOperator.None,
                BuilderOperator.LPAREN,
                BuilderOperator.RPAREN,
                BuilderOperator.OR,
                BuilderOperator.AND,
                BuilderOperator.NOT
            };

            return Array.IndexOf(sortOrder, builderOp) - Array.IndexOf(sortOrder, otherOp);
        }
    }
}