using QuoteFlow.Api.Jql.Query.Clause;

namespace QuoteFlow.Api.Asset.Search.Searchers.Util
{
    /// <summary>
    /// A base clause visitor that recursively visits each clause in a clause tree.
    /// </summary>
    public class RecursiveClauseVisitor : IClauseVisitor<object>
    {
        public virtual object Visit(AndClause andClause)
        {
            foreach (var clause in andClause.Clauses)
            {
                clause.Accept(this);
            }
            return null;
        }

        public virtual object Visit(NotClause notClause)
        {
            return notClause.SubClause.Accept(this);
        }

        public virtual object Visit(OrClause orClause)
        {
            foreach (var clause in orClause.Clauses)
            {
                clause.Accept(this);
            }
            return null;
        }

        public virtual object Visit(ITerminalClause clause)
        {
            return null;
        }

        object IClauseVisitor<object>.Visit(IWasClause clause)
        {
            return null;
        }

        // ChangedClause is a terminal clause that has no subclauses
        object IClauseVisitor<object>.Visit(IChangedClause clause)
        {
            return null;
        }
    }

}