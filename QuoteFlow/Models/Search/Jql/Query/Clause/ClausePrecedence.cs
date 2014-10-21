using System;
namespace QuoteFlow.Models.Search.Jql.Query.Clause
{
    /// <summary>
    /// Used to determine the logical precedence of the clauses that can be contained in a SearchQuery.
    /// </summary>
    public enum ClausePrecedence
    {
        OR = 700,
        AND = 1000,
        NOT = 2000,
        TERMINAL = Int32.MaxValue
    }

    public static class ClausePrecedenceHelper
    {
        public static ClausePrecedence GetPrecedence(IClause clause)
        {
            if (clause is AndClause)
            {
                return ClausePrecedence.AND;
            }
            if (clause is OrClause)
            {
                return ClausePrecedence.OR;
            }
            if (clause is NotClause)
            {
                return ClausePrecedence.NOT;
            }
            if (clause is ITerminalClause || clause is IChangedClause)
            {
                return ClausePrecedence.TERMINAL;
            }

            throw new ArgumentException("Attempt to get precedence for an unsupported clause.");
        }
    }
}