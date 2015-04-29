using Lucene.Net.Search;
using QuoteFlow.Api.Infrastructure.Lucene;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Query.Clause;

namespace QuoteFlow.Core.Jql.Query
{
    /// <summary>
    /// Creates a Lucene Query from a JQL clause.
    /// </summary>
    public class LuceneQueryBuilder : ILuceneQueryBuilder
    {
        private readonly IQueryRegistry _queryRegistry;
		private readonly ILuceneQueryModifier _luceneQueryModifier;
		private readonly WasClauseQueryFactory _wasClauseQueryFactory;
		private readonly ChangedClauseQueryFactory _changedClauseQueryFactory;

        public LuceneQueryBuilder(IQueryRegistry queryRegistry, ILuceneQueryModifier luceneQueryModifier, 
            WasClauseQueryFactory wasClauseQueryFactory, ChangedClauseQueryFactory changedClauseQueryFactory)
		{
			_queryRegistry = queryRegistry;
			_luceneQueryModifier = luceneQueryModifier;
			_wasClauseQueryFactory = wasClauseQueryFactory;
			_changedClauseQueryFactory = changedClauseQueryFactory;
		}

        public global::Lucene.Net.Search.Query CreateLuceneQuery(IQueryCreationContext queryCreationContext, IClause clause)
        {
            QueryVisitor queryVisitor = CreateQueryVisitor(queryCreationContext);
            global::Lucene.Net.Search.Query luceneQuery;
            try
            {
                luceneQuery = queryVisitor.CreateQuery(clause);
            }
            catch (QueryVisitor.JqlTooComplex jqlTooComplex)
            {
                throw new ClauseTooComplexSearchException(jqlTooComplex.Clause);
            }

            // we need to process the returned query so that it will run in lucene correctly. For instance, we
            // will add positive queries where necessary so that negations work.
            try
            {
                return _luceneQueryModifier.GetModifiedQuery(luceneQuery);
            }
            catch (BooleanQuery.TooManyClauses tooManyClauses)
            {
                throw new ClauseTooComplexSearchException(clause);
            }
        }

        public virtual QueryVisitor CreateQueryVisitor(IQueryCreationContext context)
		{
			return new QueryVisitor(_queryRegistry, context, _wasClauseQueryFactory, _changedClauseQueryFactory);
		}
    }
}