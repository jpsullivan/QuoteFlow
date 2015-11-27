using System;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Resolver;

namespace QuoteFlow.Core.Jql.Query
{
    /// <summary>
    /// Used to generate equality Lucene queries. When this searches for EMPTY values,
    /// it will search the index for the provided fieldName with the value that 
    /// is provided to represent an Empty value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EqualityWithSpecifiedEmptyValueQueryFactory<T> : AbstractEqualityQueryFactory<T>
    {
        private readonly string _emptyValue;

        public EqualityWithSpecifiedEmptyValueQueryFactory(IIndexInfoResolver<T> indexInfoResolver, string emptyValue) 
            : base(indexInfoResolver)
        {
            _emptyValue = emptyValue;
        }

        public override QueryFactoryResult CreateQueryForEmptyOperand(string fieldName, Operator oprator)
        {
            switch (oprator)
            {
                case Operator.IS:
                case Operator.EQUALS:
                    return new QueryFactoryResult(GetIsEmptyQuery(fieldName));
                case Operator.IS_NOT:
                case Operator.NOT_EQUALS:
                    return new QueryFactoryResult(GetIsNotEmptyQuery(fieldName));
            }

            Console.WriteLine("Cannot create a query for an Empty operand using the operator {0}", oprator); 
            return QueryFactoryResult.CreateFalseResult();
        }

        public override global::Lucene.Net.Search.Query GetIsEmptyQuery(string fieldName)
        {
            return GetTermQuery(fieldName, _emptyValue);
        }

        public override global::Lucene.Net.Search.Query GetIsNotEmptyQuery(string fieldName)
        {
            // We are returning a query that will exclude empties by specifying a MUST_NOT
            // occurrance. We should add the visibility query so that we exclude documents
            // which don't have fieldName indexed.
            var result = new QueryFactoryResult(GetTermQuery(fieldName, _emptyValue), true);
            return QueryFactoryResult.WrapWithVisibilityQuery(fieldName, result).LuceneQuery;
        }
    }
}