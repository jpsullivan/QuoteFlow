using System.Linq;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;

namespace QuoteFlow.Api.Asset.Search.Util
{
    /// <summary>
    /// Look through the query and try to find "text" searching conditions. We use a simple heuristic
    /// that any <see cref="TerminalClause"/> with a "LIKE" and "NOT LIKE" means a text match.
    /// </summary>
    public class FreeTextVisitor : IClauseVisitor<bool>
    {
        public bool Visit(AndClause andClause)
        {
            return DoVisit(andClause);
        }

        public bool Visit(NotClause notClause)
        {
            return DoVisit(notClause);
        }

        public bool Visit(OrClause orClause)
        {
            return DoVisit(orClause);
        }

        public bool Visit(ITerminalClause clause)
        {
            var op = clause.Operator;
            return op == Operator.LIKE || op == Operator.NOT_LIKE;
        }

        bool IClauseVisitor<bool>.Visit(IWasClause clause)
        {
            return false;
        }

        bool IClauseVisitor<bool>.Visit(IChangedClause clause)
        {
            // changed does not support free text search
            return false;
        }

        public static bool ContainsFreeTextCondition(IClause clause)
        {
            if (clause == null)
            {
                return false;
            }

            var visitor = new FreeTextVisitor();
            return clause.Accept(visitor);
        }

        private bool DoVisit(IClause andClause)
        {
            return andClause.Clauses.Any(clause => clause.Accept(this));
        }
    }
}