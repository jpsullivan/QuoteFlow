using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Query.Operand;

namespace QuoteFlow.Core.Tests.Jql.Query.Operand
{
    /// <summary>
    /// Provides static methods that create QueryLiterals with <see cref="SingleValueOperand"/>
    /// as the source operand with the literals value.
    /// </summary>
    public static class SimpleLiteralFactory
    {
        public static QueryLiteral CreateLiteral(string value)
        {
            return new QueryLiteral(new SingleValueOperand(value), value);
        }

        public static QueryLiteral CreateLiteral(int? value)
        {
            return new QueryLiteral(new SingleValueOperand(value), value);
        }

        public static QueryLiteral CreateLiteral()
        {
            return new QueryLiteral();
        }
    }
}
