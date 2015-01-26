using System;
using System.Collections.Generic;
using QuoteFlow.Api.Jql;
using QuoteFlow.Api.Jql.Context;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;
using QuoteFlow.Api.Jql.Query.Operand;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Core.Asset.Search
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