using System;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Resolver;
using QuoteFlow.Api.Models;
using QuoteFlow.Api.Util;

namespace QuoteFlow.Api.Jql.Validator
{
    /// <summary>
    /// A clause validator that can be used for multiple constant 
    /// (priority, status, resolution) clause types that uses the 
    /// <see cref="INameResolver{T}"/> to determine if the value exists.
    /// </summary>
    public class DataValuesExistValidator<T> : ValuesExistValidator
    {
        private readonly INameResolver<T> _nameResolver;

        public DataValuesExistValidator(IJqlOperandResolver operandResolver, INameResolver<T> nameResolver) 
            : base(operandResolver)
        {
            if (nameResolver == null)
            {
                throw new ArgumentNullException(nameof(nameResolver));
            }

            _nameResolver = nameResolver;
        }

        public DataValuesExistValidator(IJqlOperandResolver operandResolver, INameResolver<T> nameResolver, MessageSetLevel level = MessageSetLevel.Error) 
            : base(operandResolver, level)
        {
            if (nameResolver == null)
            {
                throw new ArgumentNullException(nameof(nameResolver));
            }

            _nameResolver = nameResolver;
        }

        protected override bool StringValueExists(User searcher, string value)
        {
            return _nameResolver.NameExists(value);
        }

        protected override bool IntValueExist(User searcher, int? value)
        {
            return value.HasValue && _nameResolver.IdExists(value.Value);
        }
    }
}