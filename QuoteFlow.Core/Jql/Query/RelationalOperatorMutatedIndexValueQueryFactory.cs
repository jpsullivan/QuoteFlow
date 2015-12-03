using System;
using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Api.Infrastructure.Extensions;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Operator;
using QuoteFlow.Api.Jql.Query;
using QuoteFlow.Api.Jql.Resolver;

namespace QuoteFlow.Core.Jql.Query
{
    /// <summary>
    /// Creates relational queries for clauses with operands whose index values 
    /// representation is based on mutated raw values as opposed to domain values.
    /// </summary>
    public class RelationalOperatorMutatedIndexValueQueryFactory<T> : IOperatorSpecificQueryFactory
    {
        private readonly IIndexInfoResolver<T> _indexInfoResolver;
        private readonly RangeQueryFactory<string> _rangeQueryFactory = RangeQueryFactory<string>.StringRangeQueryFactory();

        public RelationalOperatorMutatedIndexValueQueryFactory(IIndexInfoResolver<T> indexInfoResolver)
        {
            _indexInfoResolver = indexInfoResolver;
        }

        public QueryFactoryResult CreateQueryForSingleValue(string fieldName, Operator oprator, List<QueryLiteral> rawValues)
        {
            if (!HandlesOperator(oprator))
            {
                Console.WriteLine("Integer operands do not support operator '{0}'", oprator);
                return QueryFactoryResult.CreateFalseResult();
            }

            var mutatedValues = GetIndexValues(rawValues);

            // most operators only expect one value
            // if we somehow got null as the value, don't error out but just return a false query
            if (!mutatedValues.Any() || mutatedValues.First() == null)
            {
                return QueryFactoryResult.CreateFalseResult();
            }

            return new QueryFactoryResult(_rangeQueryFactory.Get(oprator, fieldName, mutatedValues.First()));
        }

        public QueryFactoryResult CreateQueryForMultipleValues(string fieldName, Operator @operator, List<QueryLiteral> rawValues)
        {
            //Console.WriteLine("Mutli value operands are not supported by this query factory.");
            return QueryFactoryResult.CreateFalseResult();
        }

        public QueryFactoryResult CreateQueryForEmptyOperand(string fieldName, Operator oprator)
        {
            //Console.WriteLine("Empty operands are not supported by this query factory.");
            return QueryFactoryResult.CreateFalseResult();
        }

        public bool HandlesOperator(Operator oprator)
        {
            return OperatorClasses.RelationalOnlyOperators.Contains(oprator);
        }

        private List<string> GetIndexValues(List<QueryLiteral> rawValues)
        {
            if ((rawValues == null) || !rawValues.Any())
            {
                return new List<string>();
            }

            var indexValues = new List<string>();
            foreach (var rawValue in rawValues.Where(rawValue => rawValue != null))
            {
                List<string> vals;

                // turn the raw values into index values
                if (rawValue.StringValue != null)
                {
                    vals = _indexInfoResolver.GetIndexedValues(rawValue.StringValue);
                }
                else if (rawValue.IntValue != null)
                {
                    vals = _indexInfoResolver.GetIndexedValues(rawValue.IntValue);
                }
                else
                {
                    // Note: we expect that the IndexInfoResolver result above does not 
                    // contain nulls, so when we add null to the indexValues, this is
                    // signifying that an Empty query literal was seen.
                    indexValues.Add(null);
                    continue;
                }

                if (vals.AnySafe())
                {
                    // just aggregate all the values together into one big list
                    indexValues.AddRange(vals);
                }
            }

            return indexValues;
        } 
    }
}