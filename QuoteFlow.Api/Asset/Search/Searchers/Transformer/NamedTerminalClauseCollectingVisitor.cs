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
        private readonly IList<string> _clauseNames;
        private readonly IList<ITerminalClause> _namedClauses;

        public NamedTerminalClauseCollectingVisitor(string clauseName)
            : this(new List<string> { clauseName })
        {
        }

        public NamedTerminalClauseCollectingVisitor(IEnumerable<string> clauseNames)
        {
            var names = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
            if (clauseNames != null)
            {
                foreach (var clauseName in clauseNames)
                {
                    names.Add(clauseName);
                }
            }
            
            _clauseNames = new List<string>(names);
            _namedClauses = new List<ITerminalClause>();
        }

        public virtual IEnumerable<ITerminalClause> NamedClauses => _namedClauses;

        public virtual bool ContainsNamedClause()
        {
            return _namedClauses.Count > 0;
        }

        public override object Visit(ITerminalClause clause)
        {
            if (_clauseNames.Contains(clause.Name))
            {
                _namedClauses.Add(clause);
            }
            return null;
        }
    }
}