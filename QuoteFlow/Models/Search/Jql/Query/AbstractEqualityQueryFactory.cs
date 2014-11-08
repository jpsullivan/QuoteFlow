using System;
using System.Collections.Generic;
using Lucene.Net.Search;
using QuoteFlow.Models.Search.Jql.Operand;
using QuoteFlow.Models.Search.Jql.Resolver;

namespace QuoteFlow.Models.Search.Jql.Query
{
    public abstract class AbstractEqualityQueryFactory<T> : AbstractOperatorQueryFactory<T>,
        IOperatorSpecificQueryFactory
    {
        public AbstractEqualityQueryFactory(IIndexInfoResolver<T> indexInfoResolver) : base(indexInfoResolver)
        {
        }

        public QueryFactoryResult CreateQueryForSingleValue(string fieldName, Operator oprator,
            List<QueryLiteral> rawValues)
        {
            if (Operator.EQUALS.Equals(oprator))
            {
                return HandleEquals(fieldName, GetIndexValues(rawValues));
            }
            if (Operator.NOT_EQUALS.Equals(oprator))
            {
                return HandleNotEquals(fieldName, GetIndexValues(rawValues));
            }
//				if (log.DebugEnabled)
//				{
//					log.debug("Create query for single value was called with operator '" + oprator.DisplayString + "', this only handles '=' and '!='.");
//				}
            return QueryFactoryResult.CreateFalseResult();
        }

        public QueryFactoryResult CreateQueryForMultipleValues(string fieldName, Operator oprator,
            List<QueryLiteral> rawValues)
        {
            if (Operator.IN.Equals(oprator))
            {
                return HandleEquals(fieldName, GetIndexValues(rawValues));
            }
            if (Operator.NOT_IN.Equals(oprator))
            {
                return HandleNotEquals(fieldName, GetIndexValues(rawValues));
            }
//				if (log.DebugEnabled)
//				{
//					log.debug("Create query for multiple value was called with operator '" + oprator.DisplayString + "', this only handles 'in'.");
//				}
            return QueryFactoryResult.CreateFalseResult();
        }

        public QueryFactoryResult CreateQueryForEmptyOperand(string fieldName, Operator oprator)
        {
            throw new NotImplementedException();
        }

        public virtual bool HandlesOperator(Operator oprator)
        {
            return OperatorClasses.EqualityOperatorsWithEmpty.Contains(oprator);
        }

        internal virtual QueryFactoryResult HandleNotEquals(string fieldName, IEnumerable<string> indexValues)
        {
            var notQueries = new List<global::Lucene.Net.Search.Query>();

            if (indexValues != null)
            {
                foreach (string indexValue in indexValues)
                {
                    // don't bother keeping track of empty literals - empty query gets added later anyway
                    if (indexValue != null)
                    {
                        notQueries.Add(GetTermQuery(fieldName, indexValue));
                    }
                }
            }
            if (notQueries.Count == 0)
            {
                // if we didn't find non-empty literals, then return the isNotEmpty query
                return new QueryFactoryResult(GetIsNotEmptyQuery(fieldName));
            }
            var boolQuery = new BooleanQuery();
            // Because this is a NOT equality query we are generating we always need to explicity exclude the
            // EMPTY results from the query we are generating.
            boolQuery.Add(GetIsNotEmptyQuery(fieldName), Occur.MUST);

            // Add all the not queries that were specified by the user.
            foreach (global::Lucene.Net.Search.Query query in notQueries)
            {
                boolQuery.Add(query, Occur.MUST_NOT);
            }

            // We should add the visibility query so that we exclude documents which don't have fieldName indexed.
            boolQuery.Add(TermQueryFactory.VisibilityQuery(fieldName), Occur.MUST);

            return new QueryFactoryResult(boolQuery, false);
        }

        internal QueryFactoryResult HandleEquals(string fieldName, IList<string> indexValues)
        {
            if (indexValues == null)
            {
                return QueryFactoryResult.CreateFalseResult();
            }
            if (indexValues.Count == 1)
            {
                string id = indexValues[0];
                return (id == null)
                    ? new QueryFactoryResult(GetIsEmptyQuery(fieldName))
                    : new QueryFactoryResult(GetTermQuery(fieldName, id));
            }
            var orQuery = new BooleanQuery();
            foreach (string id in indexValues)
            {
                orQuery.Add(id != null ? GetTermQuery(fieldName, id) : GetIsEmptyQuery(fieldName), Occur.SHOULD);
            }

            return new QueryFactoryResult(orQuery);
        }

        /// <summary>
        /// Get the query for the concrete impl class that means "give me all results for this field that are empty".
        /// This query will also include an additional visibility query if necessary. For example, a negative query such as
        /// <code>-nonemptyfieldids=duedate</code> will be combined with <code>visiblefields:duedate</code>. Hence, further
        /// wrapping is not required.
        /// </summary>
        /// <param name="fieldName"> the field to search on empty. </param>
        /// <returns> a lucene Query, possibly combined with a visibility query </returns>
        internal abstract global::Lucene.Net.Search.Query GetIsEmptyQuery(string fieldName);

        /// <summary>
        /// Get the query for the concrete impl class that means "give me all results for this field that are not empty".
        /// This query will also include an additional visibility query if necessary. For example, a negative query such as
        /// <code>-priority=-1</code> will be combined with <code>visiblefields:priority</code>. Hence, further
        /// wrapping is not required.
        /// </summary>
        /// <param name="fieldName"> the field to search on empty. </param>
        /// <returns> a lucene Query, possibly combined with a visibility query </returns>
        internal abstract global::Lucene.Net.Search.Query GetIsNotEmptyQuery(string fieldName);
    }
}