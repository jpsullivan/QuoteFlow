using QuoteFlow.Api.Asset.Search.Constants;
using QuoteFlow.Api.Jql.Operand;
using QuoteFlow.Api.Jql.Validator;

namespace QuoteFlow.Core.Jql.Validator
{
    /// <summary>
    /// A validator for the summary field that is a simple wrapper around the text field validator.
    /// </summary>
    public class SummaryValidator : FreeTextFieldValidator
    {
        public SummaryValidator(IJqlOperandResolver operandResolver)
            : base(SystemSearchConstants.ForSummary().IndexField, operandResolver)
        {
        }
    }
}