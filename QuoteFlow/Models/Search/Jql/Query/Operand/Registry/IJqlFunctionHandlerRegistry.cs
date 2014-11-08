using System.Collections.Generic;

namespace QuoteFlow.Models.Search.Jql.Query.Operand.Registry
{
    /// <summary>
    /// Registry for <seealso cref="JqlFunction"/>s. Can resolve a
    /// <seealso cref="OperandHandler"/> of type <seealso cref="FunctionOperand"/>
    /// for a provided <seealso cref="FunctionOperand"/>. The handler returned wraps a
    /// registered <seealso cref="JqlFunction"/>.
    /// 
    /// @since v4.0
    /// </summary>
    public interface IJqlFunctionHandlerRegistry
    {
        /// <summary>
        /// Fetches the associated OperandHandler for the provided FunctionOperand. The returned handler is looked up by the
        /// name of the FunctionOperand (case insensitive).
        /// </summary>
        /// <param name="operand"> that defines the name for which we want to find the operand handler. </param>
        /// <returns> the operand handler associated with this operand, null if there is none. </returns>
        FunctionOperandHandler getOperandHandler(FunctionOperand operand);

        /// <summary>
        /// Fetches all function names ordered alphabetically.
        /// </summary>
        /// <returns> all function names ordered alphabetically, an empty collection if there are none. </returns>
        IEnumerable<string> AllFunctionNames { get; }

    }
}