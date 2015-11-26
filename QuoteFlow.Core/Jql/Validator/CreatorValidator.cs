using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Resolver;
using QuoteFlow.Api.Jql.Validator;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Core.Jql.Validator
{
    /// <summary>
    /// A validator for the Creator field clauses.
    /// </summary>
    public class CreatorValidator : AbstractUserValidator, IClauseValidator
    {
        public CreatorValidator(INameResolver<User> userResolver, IJqlOperandResolver operandResolver) 
            : base(userResolver, operandResolver)
        {
        }
    }
}