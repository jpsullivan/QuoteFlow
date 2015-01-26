using QuoteFlow.Api.Jql.Query.Operand;
using QuoteFlow.Api.Models;

namespace QuoteFlow.Api.Jql.Function
{
    /// <summary>
    /// An additional interface which can be implemented by <see cref="IJqlFunction"/>
    /// classes in order to indicate to the <see cref="JqlOperandResolver"/> that their arguments
    /// are able to be sanitised if necessary.
    /// 
    /// This was not added to the <see cref="IJqlFunction"/> interface as the default
    /// behaviour is not to care about sanitising, and we didn't want to bloat the plugin point.
    /// </summary>
    public interface IClauseSanitizingJqlFunction
    {
        /// <summary>
        /// Sanitize a function operand for the specified user, so that information is not leaked.
        /// </summary>
        /// <param name="searcher">The user performing the search </param>
        /// <param name="operand">The operand to sanitise; will only be sanitised if valid </param>
        /// <returns>The sanitised operand; never null.</returns>
        FunctionOperand SanitizeOperand(User searcher, FunctionOperand operand);
    }
}