using System;
using System.ComponentModel.DataAnnotations;

namespace QuoteFlow.Models.Search.Jql.Query.Clause
{
    /// <summary>
    /// Used to determine the logical precedence of the clauses that can be contained in a SearchQuery.
    /// </summary>
    public enum ClausePrecedence
    {
        [Display(Order = 700)]
        OR,
        [Display(Order = 1000)]
        AND,
        [Display(Order = 2000)]
        NOT,
        [Display(Order = Int32.MaxValue)]
        TERMINAL
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