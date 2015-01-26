using System;
using System.Collections.Generic;
using QuoteFlow.Api.Asset.Search.Searchers.Util;
using QuoteFlow.Api.Jql.Query.Clause;
using Wintellect.PowerCollections;

namespace QuoteFlow.Api.Asset.Search.Searchers.Transformer
{
    /// <summary>
    /// A <see cref="RecursiveClauseVisitor"/> which collects <see cref="ITerminalClause"/>s that 
    /// have the specified clause names.
    /// Note: this visitor does not perform any structure checking. It simply collects all the 
    /// clauses with the specified names.
    /// </summary>
    public class NamedTerminalClauseCollectingVisitor : RecursiveClauseVisitor, IClauseVisitor<object>
    {
        private readonly Set<string> clauseNames;
        private readonly IList<ITerminalClause> namedClauses;

        public NamedTerminalClauseCollectingVisitor(string clauseName)
            : this(new List<string> { clauseName })
        {
        }

        public NamedTerminalClauseCollectingVisitor(IEnumerable<string> clauseNames)
        {
            var names = new SortedSet<string>(null, StringComparer.OrdinalIgnoreCase);
            if (clauseNames != null)
            {
                foreach (var clauseName in clauseNames)
                {
                    names.Add(clauseName);
                }
            }
            this.clauseNames = new Set<string>(names);
            this.namedClauses = new List<ITerminalClause>();
        }

        public virtual IEnumerable<ITerminalClause> NamedClauses
        {
            get { return namedClauses; }
        }

        public virtual bool ContainsNamedClause()
        {
            return namedClauses.Count > 0;
        }

        public override object Visit(ITerminalClause clause)
        {
            if (clauseNames.Contains(clause.Name))
            {
                this.namedClauses.Add(clause);
            }
            return null;
        }
    }
}