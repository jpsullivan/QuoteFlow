using System.Collections.Generic;
using QuoteFlow.Api.Jql.Query.Operand;

namespace QuoteFlow.Api.Asset.Search.Searchers.Transformer
{
    /// <summary>
    /// Provides a registry for a field, navigator flag values and operand associations.
    /// </summary>
    public interface IFieldFlagOperandRegistry
    {
        /// <summary>
        /// Retrieves the operand associated with a field and navigator flag value pair.
        /// For example the issuetype field has the -2 navigator flag value which maps to the
        /// <seealso cref="FunctionOperand"/> corresponding to the
        /// <seealso cref="AllStandardAssetTypesFunction"/>
        /// </summary>
        /// <param name="fieldName"> The name of the field </param>
        /// <param name="flagValue"> The navigator flag value </param>
        /// <returns> the operand associated with the field and navigator flag value; null if there is none </returns>
        IOperand GetOperandForFlag(string fieldName, string flagValue);

        /// <summary>
        /// Retrieves the navigator flag values associated with the field name and operand pair.
        /// </summary>
        /// <param name="fieldName"> The name of the field </param>
        /// <param name="operand"> the <seealso cref="IOperand"/> </param>
        /// <returns> The navigator flag value associated with the field name and operand pair; null if there is none </returns>
        ISet<string> GetFlagForOperand(string fieldName, IOperand operand);
    }
}