using System;
using System.Collections.Generic;
using Lucene.Net.Search;

namespace QuoteFlow.Api.Jql.Query
{
    public class RangeQueryFactory<T>
    {
        public static RangeQueryFactory<string> StringRangeQueryFactory()
        {
            // just return the input
            return new RangeQueryFactory<string>(s => s);
        }

        private readonly Func<T, string> _valueFactory;

        public RangeQueryFactory(Func<T, string> valueFactory)
        {
            if (valueFactory == null) throw new ArgumentNullException(nameof(valueFactory));

            _valueFactory = valueFactory;
        }

        public global::Lucene.Net.Search.Query Get(Operator @operator, string fieldName, T value)
        {
            switch (@operator)
            {
                case Operator.LESS_THAN:
                    return HandleLessThan(fieldName, value);
                case Operator.LESS_THAN_EQUALS:
                    return HandleLessThanEquals(fieldName, value);
                case Operator.GREATER_THAN:
                    return HandleGreaterThan(fieldName, value);
                case Operator.GREATER_THAN_EQUALS:
                    return HandleGreaterThanEquals(fieldName, value);
                default:
                    // should not have gotten here
                    throw new ArgumentException("Unhandled Operator: " + @operator);
            }
        }

        public global::Lucene.Net.Search.Query Get(Operator @operator, string fieldName, IList<T> values)
        {
            if (@operator == Operator.DURING)
            {
                if (values[0] == null)
                {
                    return HandleLessThanEquals(fieldName, values[1]);
                }
                if (values[1] == null)
                {
                    return HandleGreaterThanEquals(fieldName, values[0]);
                }
                return HandleDuring(fieldName, values[0], values[1]);
            }

            // should not have gotten here
            throw new ArgumentException("Unhandled Operator: " + @operator);
        }

        protected virtual global::Lucene.Net.Search.Query HandleLessThan(string fieldName, T value)
        {
            return new TermRangeQuery(fieldName, null, _valueFactory.Invoke(value), true, false);
        }

        private global::Lucene.Net.Search.Query HandleLessThanEquals(string fieldName, T value)
        {
            return new TermRangeQuery(fieldName, null, _valueFactory.Invoke(value), true, true);
        }

        protected virtual global::Lucene.Net.Search.Query HandleGreaterThan(string fieldName, T value)
        {
            return new TermRangeQuery(fieldName, _valueFactory.Invoke(value), null, false, true);
        }

        protected virtual global::Lucene.Net.Search.Query HandleGreaterThanEquals(string fieldName, T value)
        {
            return new TermRangeQuery(fieldName, _valueFactory.Invoke(value), null, true, true);
        }

        protected virtual global::Lucene.Net.Search.Query HandleDuring(string fieldName, T lowerValue, T upperValue)
        {
            return new TermRangeQuery(fieldName, _valueFactory.Invoke(lowerValue), _valueFactory.Invoke(upperValue), true, true);
        }
    }
}