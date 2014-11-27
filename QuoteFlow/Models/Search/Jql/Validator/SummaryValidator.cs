using QuoteFlow.Models.Assets.Search.Constants;
using QuoteFlow.Models.Search.Jql.Operand;

namespace QuoteFlow.Models.Search.Jql.Validator
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