using System.Collections.Generic;
using QuoteFlow.Api.Jql.Function;

namespace QuoteFlow.Api.Jql.Query.Operand.Registry
{
    /// <summary>
    /// Registry for <see cref="IJqlFunction"/>s. Can resolve a <see cref="OperandHandler"/> 
    /// of type <see cref="FunctionOperand"/> for a provided <see cref="FunctionOperand"/>. 
    /// The handler returned wraps a registered <see cref="IJqlFunction"/>.
    /// </summary>
    public interface IJqlFunctionHandlerRegistry
    {
        /// <summary>
        /// Fetches the associated OperandHandler for the provided FunctionOperand. The returned handler is looked up by the
        /// name of the FunctionOperand (case insensitive).
        /// </summary>
        /// <param name="operand">That defines the name for which we want to find the operand handler.</param>
        /// <returns>
        /// The operand handler associated with this operand, null if there is none.
        /// </returns>
        FunctionOperandHandler GetOperandHandler(FunctionOperand operand);

        /// <summary>
        /// Fetches all function names ordered alphabetically.
        /// </summary>
        /// <returns>
        /// All function names ordered alphabetically, an empty collection if there are none.
        /// </returns>
        IEnumerable<string> AllFunctionNames { get; }
    }
}