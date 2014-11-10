using System;
using System.Collections.Generic;
using QuoteFlow.Models.Search.Jql.Clauses;
using QuoteFlow.Models.Search.Jql.Context;
using QuoteFlow.Models.Search.Jql.Operand;
using QuoteFlow.Models.Search.Jql.Query;
using QuoteFlow.Models.Search.Jql.Query.Clause;
using QuoteFlow.Models.Search.Jql.Query.Operand;

namespace QuoteFlow.Models.Assets.Search
{
    public class QueryCache : IQueryCache
    {
        public bool? GetDoesQueryFitFilterFormCache(User searcher, Query query)
        {
            throw new NotImplementedException();
        }

        public void SetDoesQueryFitFilterFormCache(User searcher, Query query, bool doesItFit)
        {
            throw new NotImplementedException();
        }

        public IQueryContext GetQueryContextCache(User searcher, Query query)
        {
            throw new NotImplementedException();
        }

        public void SetQueryContextCache(User searcher, Query query, IQueryContext queryContext)
        {
            throw new NotImplementedException();
        }

        public IQueryContext GetSimpleQueryContextCache(User searcher, Query query)
        {
            throw new NotImplementedException();
        }

        public void SetSimpleQueryContextCache(User searcher, Query query, IQueryContext queryContext)
        {
            throw new NotImplementedException();
        }

        public ICollection<IClauseHandler> GetClauseHandlers(User searcher, string jqlClauseName)
        {
            throw new NotImplementedException();
        }

        public void SetClauseHandlers(User searcher, string jqlClauseName, ICollection<IClauseHandler> clauseHandlers)
        {
            throw new NotImplementedException();
        }

        public IList<QueryLiteral> GetValues(QueryCreationContext context, IOperand operand, TerminalClause jqlClause)
        {
            throw new NotImplementedException();
        }

        public void SetValues(QueryCreationContext context, IOperand operand, ITerminalClause jqlClause, IList<QueryLiteral> values)
        {
            throw new NotImplementedException();
        }
    }
}