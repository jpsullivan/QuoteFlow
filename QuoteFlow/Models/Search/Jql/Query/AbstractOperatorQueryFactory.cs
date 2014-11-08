using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Index;
using Lucene.Net.Search;
using QuoteFlow.Models.Search.Jql.Operand;
using QuoteFlow.Models.Search.Jql.Resolver;

namespace QuoteFlow.Models.Search.Jql.Query
{
    /// <summary>
    /// Base class for operator query factories that can generate a query for a fieldName and a predicate.
    /// 
    /// @since v4.0
    /// </summary>
    public abstract class AbstractOperatorQueryFactory<T>
    {
        private readonly IIndexInfoResolver<T> indexInfoResolver;

        protected internal AbstractOperatorQueryFactory(IIndexInfoResolver<T> indexInfoResolver)
        {
            this.indexInfoResolver = indexInfoResolver;
        }

        protected internal virtual TermQuery GetTermQuery(string fieldName, string indexValue)
        {
            return new TermQuery(new Term(fieldName, indexValue));
        }

        protected internal virtual QueryFactoryResult CheckQueryForEmpty(BooleanQuery query)
        {
            // There is a special case where we were unable to resolve any of the id's provided to this method in
            // listOfIds and we therefore need to return a false query. If we were only unable to resolve some of the
            // id's this does not matter since they are all OR'ed together and TRUE || FALSE = TRUE
            if (!query.Clauses.Any())
            {
                return QueryFactoryResult.CreateFalseResult();
            }
            return new QueryFactoryResult(query);
        }

        /// <param name="rawValues"> the raw values to convert </param>
        /// <returns> a list of index values in String form; never null, but may contain null values if empty literals were passed in. </returns>
        internal virtual List<string> GetIndexValues(IList<QueryLiteral> rawValues)
        {
            if (rawValues == null || rawValues.Count == 0)
            {
                return new List<string>();
            }

            var indexValues = new List<string>();
            foreach (QueryLiteral rawValue in rawValues)
            {
                if (rawValue == null) continue;

                IList<string> vals;
                // Turn the raw values into index values
                if (rawValue.StringValue != null)
                {
                    vals = indexInfoResolver.GetIndexedValues(rawValue.StringValue);
                }
                else if (rawValue.IntValue != null)
                {
                    vals = indexInfoResolver.GetIndexedValues(rawValue.IntValue);
                }
                else
                {
                    // Note: we expect that the IndexInfoResolver result above does not contain nulls, so when we
                    // add null here to the indexValues, this is signifying that an Empty query literal was seen
                    indexValues.Add(null);
                    continue;
                }

                if (vals != null && vals.Count > 0)
                {
                    // Just aggregate all the values together into one big list.
                    indexValues.AddRange(vals);
                }
            }
            return indexValues;
        }
    }

}