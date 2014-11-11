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
        public bool? GetDoesQueryFitFilterFormCache(User searcher, IQuery query)
        {
            throw new NotImplementedException();
        }

        public void SetDoesQueryFitFilterFormCache(User searcher, IQuery query, bool doesItFit)
        {
            throw new NotImplementedException();
        }

        public IQueryContext GetQueryContextCache(User searcher, IQuery query)
        {
            throw new NotImplementedException();
        }

        public void SetQueryContextCache(User searcher, IQuery query, IQueryContext queryContext)
        {
            throw new NotImplementedException();
        }

        public IQueryContext GetSimpleQueryContextCache(User searcher, IQuery query)
        {
            throw new NotImplementedException();
        }

        public void SetSimpleQueryContextCache(User searcher, IQuery query, IQueryContext queryContext)
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

        public IList<QueryLiteral> GetValues(IQueryCreationContext context, IOperand operand, ITerminalClause jqlClause)
        {
            throw new NotImplementedException();
        }

        public void SetValues(IQueryCreationContext context, IOperand operand, ITerminalClause jqlClause, IEnumerable<QueryLiteral> values)
        {
            throw new NotImplementedException();
        }
    }
}