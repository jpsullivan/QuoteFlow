using System;
using System.Collections.Generic;
using QuoteFlow.Api.Jql.Query.Clause;
using Wintellect.PowerCollections;

namespace QuoteFlow.Api.Asset.Search.Searchers.Transformer
{
    /// <summary>
    /// A visitor records all the TerminalClauses that match a particular condition. The visitor records whteher or not all
    /// the matched clauses are part of a standard navigator query. A standard navigator query is either a single terminal
    /// clause or an and clause with terminal clauses as children. This visitor only checks that the matched nodes form part
    /// of a standard query.
    /// </summary>
    /// //TODO: The NamedTerminalClauseCollectingVisitor is almost this. Do we need to merge or do something similar?
    public class SimpleNavigatorCollectorVisitor : IClauseVisitor<bool>
    {
        private readonly IList<ITerminalClause> clauses = new List<ITerminalClause>();
        private readonly Set<string> clauseNames;

        protected internal bool valid = true;
        protected internal bool validPath = true;

        public SimpleNavigatorCollectorVisitor(string clauseName) : this(new Set<string> {clauseName})
        {
        }

        public SimpleNavigatorCollectorVisitor(IEnumerable<string> clauseNames)
        {
            var names = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
            if (clauseNames != null)
            {
                foreach (var clauseName in clauseNames)
                {
                    names.Add(clauseName);
                }
            }
            this.clauseNames = new Set<string>(names);
        }

        public SimpleNavigatorCollectorVisitor(ClauseNames clauseNames) : this(clauseNames.JqlFieldNames)
        {
        }

        public virtual IList<ITerminalClause> Clauses
        {
            get { return clauses; }
        }

        public virtual bool Valid
        {
            get { return valid; }
        }

        public virtual bool Visit(AndClause andClause)
        {
            foreach (var clause in andClause.Clauses)
            {
                clause.Accept(this);
            }
            return true;
        }

        public virtual bool Visit(NotClause notClause)
        {
            bool oldValidPath = validPath;
            validPath = false;
            notClause.SubClause.Accept(this);
            validPath = oldValidPath;

            return true;
        }

        public virtual bool Visit(OrClause orClause)
        {
            bool oldValidPath = validPath;
            validPath = false;

            foreach (var clause in orClause.Clauses)
            {
                clause.Accept(this);
            }

            validPath = oldValidPath;
            return true;
        }

        public bool Visit(ITerminalClause clause)
        {
            if (!Matches(clause))
            {
                return false;
            }

            clauses.Add(clause);
            if (!validPath)
            {
                valid = false;
            }

            return true;
        }

        public bool Visit(IWasClause clause)
        {
            return true;
        }

        public bool Visit(IChangedClause clause)
        {
            return true;
        }

        //TODO: This probably need to be made protected or something to make this class extensible, maybe even incoprating
        //the NamedTerminalClauseCollectingVisitor here.
        private bool Matches(ITerminalClause terminalClause)
        {
            return clauseNames.Contains(terminalClause.Name);
        }
    }
}