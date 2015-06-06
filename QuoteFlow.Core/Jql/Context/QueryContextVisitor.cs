using System;
using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Api.Asset.Search.Constants;
using QuoteFlow.Api.Asset.Search.Managers;
using QuoteFlow.Api.Jql;
using QuoteFlow.Api.Jql.Context;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Models;
using QuoteFlow.Core.Jql.Clause;

namespace QuoteFlow.Core.Jql.Context
{
    /// <summary>
    /// A visitor that is used to generate a <see cref="ContextResult"/>, which contains the full and simple
    /// <see cref="IQueryContext"/>s of the visited <see cref="IQuery"/>.
    /// To construct an instance of this class, please use the <see cref="QueryContextVisitor.QueryContextVisitorFactory"/>.
    /// </summary>
    public class QueryContextVisitor : IClauseVisitor<QueryContextVisitor.ContextResult>
    {
        private readonly User searcher;
        private readonly ContextSetUtil contextSetUtil;
        private readonly ISearchHandlerManager searchHandlerManager;
        private bool rootClause = true;

        public QueryContextVisitor(User searcher, ContextSetUtil contextSetUtil, ISearchHandlerManager searchHandlerManager)
        {
            this.searcher = searcher;
            this.contextSetUtil = contextSetUtil;
            this.searchHandlerManager = searchHandlerManager;
        }

        public virtual ContextResult CreateContext(IClause clause)
        {
            // This method handles the root clause case
            rootClause = false;
            IClause normalisedClause = clause.Accept(new DeMorgansVisitor());
            ContextResult result = normalisedClause.Accept(this);
            // reset the rootClause to stay immutable (but not thread safe)
            rootClause = true;

            return ReplaceEmptyContextsWithGlobal(result);
        }

        private static ContextResult ReplaceEmptyContextsWithGlobal(ContextResult result)
        {
            IClauseContext fullContext = !result.FullContext.Contexts.Any() ? ClauseContext.CreateGlobalClauseContext() : result.FullContext;
            IClauseContext simpleContext = !result.SimpleContext.Contexts.Any() ? ClauseContext.CreateGlobalClauseContext() : result.SimpleContext;
            return new ContextResult(fullContext, simpleContext);
        }

        public ContextResult Visit(AndClause andClause)
        {
            if (rootClause)
            {
                return CreateContext(andClause);
            }

            ISet<IClauseContext> fullChildClauseContexts = new HashSet<IClauseContext>();
            ISet<IClauseContext> simpleChildClauseContexts = new HashSet<IClauseContext>();
            var childClauses = andClause.Clauses;
            foreach (IClause childClause in childClauses)
            {
                ContextResult result = childClause.Accept(this);
                fullChildClauseContexts.Add(result.FullContext);
                simpleChildClauseContexts.Add(result.SimpleContext);
            }

            // Now lets perform an intersection of all the child clause contexts.
            return CreateIntersectionResult(fullChildClauseContexts, simpleChildClauseContexts);
        }

        public ContextResult Visit(NotClause notClause)
        {
            if (rootClause)
            {
                return CreateContext(notClause);
            }
            throw new Exception("We have removed all the NOT clauses from the query, this should never occur.");
        }

        public ContextResult Visit(OrClause orClause)
        {
            if (rootClause)
            {
                return CreateContext(orClause);
            }
            ISet<IClauseContext> fullChildClauseContexts = new HashSet<IClauseContext>();
            ISet<IClauseContext> simpleChildClauseContexts = new HashSet<IClauseContext>();
            var childClauses = orClause.Clauses;
            foreach (IClause childClause in childClauses)
            {
                ContextResult result = childClause.Accept(this);
                fullChildClauseContexts.Add(result.FullContext);
                simpleChildClauseContexts.Add(result.SimpleContext);
            }

            // Now lets perform a union of all the child clause contexts.
            return CreateUnionResult(fullChildClauseContexts, simpleChildClauseContexts);
        }

        public ContextResult Visit(ITerminalClause clause)
        {
            if (rootClause)
            {
                return CreateContext(clause);
            }

            string clauseName = clause.Name;
            var handlers = searchHandlerManager.GetClauseHandler(searcher, clauseName);
            ISet<IClauseContext> fullClauseContexts = new HashSet<IClauseContext>();
            ISet<IClauseContext> simpleClauseContexts = new HashSet<IClauseContext>();
            bool @explicit = IsExplict(clause);

            foreach (IClauseHandler clauseHandler in handlers)
            {
                // keep track of this context in the full contexts
                IClauseContext context = clauseHandler.ClauseContextFactory.GetClauseContext(searcher, clause);
                if (context.Contexts.Any())
                {
                    fullClauseContexts.Add(context);

                    // the simple context is only made up of project and issue type clauses - if this clause is not one of those
                    // then ignore (by adding the Global)
                    if (@explicit)
                    {
                        simpleClauseContexts.Add(context);
                    }
                    else
                    {
                        simpleClauseContexts.Add(ClauseContext.CreateGlobalClauseContext());
                    }
                }
            }

            return CreateUnionResult(fullClauseContexts, simpleClauseContexts);
        }

        ContextResult IClauseVisitor<ContextResult>.Visit(IWasClause clause)
        {
            //for now simply return the ALL-ALL context
            return new ContextResult(ClauseContext.CreateGlobalClauseContext(), ClauseContext.CreateGlobalClauseContext());
        }

        ContextResult IClauseVisitor<ContextResult>.Visit(IChangedClause clause)
        {
            //for now simply return the ALL-ALL context
            return new ContextResult(ClauseContext.CreateGlobalClauseContext(), ClauseContext.CreateGlobalClauseContext());
        }

        private static bool IsExplict(ITerminalClause clause)
        {
            return SystemSearchConstants.ForCatalog().JqlClauseNames.Contains(clause.Name) || SystemSearchConstants.ForManufacturer().JqlClauseNames.Contains(clause.Name);
        }

        private ContextResult CreateUnionResult<T1, T2>(ISet<T1> fullContexts, ISet<T2> simpleContexts)
            where T1 : IClauseContext
            where T2 : IClauseContext
        {
            IClauseContext fullContext = SafeUnion(fullContexts);
            IClauseContext simpleContext = fullContexts.Equals(simpleContexts) ? fullContext : SafeUnion(simpleContexts);
            return new ContextResult(fullContext, simpleContext);
        }

        private ContextResult CreateIntersectionResult<T1, T2>(ISet<T1> fullContexts, ISet<T2> simpleContexts)
            where T1 : IClauseContext
            where T2 : IClauseContext
        {
            IClauseContext fullContext = SafeIntersection(fullContexts);
            IClauseContext simpleContext = fullContexts.Equals(simpleContexts) ? fullContext : SafeIntersection(simpleContexts);
            return new ContextResult(fullContext, simpleContext);
        }

        private IClauseContext SafeUnion<T>(ISet<T> contexts) where T : IClauseContext
        {
            if (contexts == null || !contexts.Any())
            {
                return ClauseContext.CreateGlobalClauseContext();
            }

            var returnContext = contexts.Count() == 1 ? contexts.First() : contextSetUtil.Union(contexts);
            return (!returnContext.Contexts.Any()) ? ClauseContext.CreateGlobalClauseContext() : returnContext;
        }

        private IClauseContext SafeIntersection<T>(ISet<T> contexts) where T : IClauseContext
        {
            if (contexts == null || !contexts.Any())
            {
                return ClauseContext.CreateGlobalClauseContext();
            }

            var returnContext = contexts.Count() == 1 ? contexts.First() : contextSetUtil.Intersect(contexts);
            return (!returnContext.Contexts.Any()) ? ClauseContext.CreateGlobalClauseContext() : returnContext;
        }

        /// <summary>
        /// Constructs an instance of <see cref="QueryContextVisitor"/> for use.
        /// </summary>
        public class QueryContextVisitorFactory
        {
            private readonly ContextSetUtil _contextSetUtil;
            private readonly ISearchHandlerManager _searchHandlerManager;

            public QueryContextVisitorFactory()
            {
            }

            public QueryContextVisitorFactory(ContextSetUtil contextSetUtil, ISearchHandlerManager searchHandlerManager)
            {
                _contextSetUtil = contextSetUtil;
                _searchHandlerManager = searchHandlerManager;
            }

            /// <summary>
            /// Use this to calculate the context for an entire Query.
            /// </summary>
            /// <param name="searcher"> the user to calculate the contexts for </param>
            /// <returns> a visitor that will calculate the context for all clauses specified in the <see cref="com.atlassian.query.Query"/>. </returns>
            public virtual QueryContextVisitor CreateVisitor(User searcher)
            {
                return new QueryContextVisitor(searcher, _contextSetUtil, _searchHandlerManager);
            }
        }

        /// <summary>
        /// The result of visiting a <see cref="IQuery"/> with the <see cref="QueryContextVisitor"/>.
        /// Contains a reference to the full and simple <see cref="IClauseContext"/>s.
        /// 
        /// The <strong>full</strong> ClauseContext takes into account all clauses in the Query, and hence may contain a combination
        /// of explicit and implicit projects and issue types.
        /// 
        /// The <strong>simple</strong> ClauseContext only takes into account the project and issue type clauses in the Query,
        /// and hence will only contain explicit projects and issue types.
        /// 
        /// To understand why we need this distinction, consider the following scenario. A custom field <code>cf[100]</code>
        /// has only one field configuration for the project <code>HSP</code>. There is also another project called
        /// <code>MKY</code>, for which this custom field is not visible. Consider the query
        /// <code>cf[100] = "a" AND project IN (HSP, MKY)</code>.
        /// 
        /// The full ClauseContext is the intersection of the ClauseContexts of the custom field and project clauses. In this case,
        /// the custom field context is implicitly the HSP project with all issue types, since the HSP project is the only
        /// project it is configured for. The project clause's context is explicitly the HSP and MKY projects, since it names
        /// them both. Intersecting these yields the <strong>explicit</strong> context of project <strong>HSP</strong> with
        /// all issue types. If you think about what kind of results this query could return, this makes sense: the query
        /// could only return issues from project HSP, since only those issues will have values for that custom field.
        /// 
        /// The simple ClauseContext, on the other hand, is the intersection of the Global Context and the ClauseContexts of the
        /// project and issue type clauses, of which there is only one. (The Global Context gets substituted in place of any
        /// non-project or non-issuetype clauses.) Again, the project clause's context is explicitly the HSP and MKY projects,
        /// since it names them both. Intersecting these yields the <strong>explicit</strong> context of projects
        /// <strong>HSP and MKY</strong> and all issue types.
        /// 
        /// So, by knowing both of these contexts, we get more information about the query's clauses. The full context tells us more
        /// about what results to expect, but at the same time can hide information about what was originally specified in the
        /// query. The simple context gives us an easy way to figure out what catalog and manufacturers were explicitly specified
        /// in the query. This is useful for testing fitness in the filter form.
        /// </summary>
        public class ContextResult
        {
            public virtual IClauseContext FullContext { get; private set; }
            public virtual IClauseContext SimpleContext { get; private set; }

            public ContextResult(IClauseContext fullContext, IClauseContext simpleContext)
            {
                FullContext = fullContext;
                SimpleContext = simpleContext;
            }

            public override string ToString()
            {
                return string.Format("[Complex: {0}, Simple: {1}]", FullContext, SimpleContext);
            }
        }
    }
}