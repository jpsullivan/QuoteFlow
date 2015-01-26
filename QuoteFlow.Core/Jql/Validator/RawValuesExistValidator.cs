using System.Collections;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Resolver;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Core.Jql.Validator
{
    /// <summary>
    /// A clause validator that can be used for multiple constant (priority, status, resolution) clause types that uses the
    /// <seealso cref="IIndexInfoResolver{T}"/> to determine if the value exists. 
    /// </summary>
    internal class RawValuesExistValidator<T> : ValuesExistValidator
    {
        private readonly IIndexInfoResolver<T> _indexInfoResolver;

        internal RawValuesExistValidator(IJqlOperandResolver operandResolver, IIndexInfoResolver<T> indexInfoResolver)
            : base(operandResolver)
        {
            _indexInfoResolver = indexInfoResolver;
        }

        internal override bool StringValueExists(User searcher, string value)
        {
            IList indexValues = _indexInfoResolver.GetIndexedValues(value);
            return indexValues != null && indexValues.Count > 0;
        }

        internal override bool IntValueExists(User searcher, int? value)
        {
            IList indexValues = _indexInfoResolver.GetIndexedValues(value);
            return indexValues != null && indexValues.Count > 0;
        }
    }
}