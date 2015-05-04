namespace QuoteFlow.Api.Jql.Query.Operand
{
    /// <summary>
    /// A visitor that the caller to perform operations on Operands.
    /// </summary>
    /// <typeparam name="T">The return type from the visitor methods.</typeparam>
    public interface IOperandVisitor<T>
    {
        /// <summary>
        /// The method called when visiting an <see cref="EmptyOperand"/>.
        /// </summary>
        /// <param name="empty"> the operand being visited. </param>
        /// <returns> the value to return from the operand visit. </returns>
        T Visit(EmptyOperand empty);

        /// <summary>
        /// The method called when visiting a <see cref="FunctionOperand"/>.
        /// </summary>
        /// <param name="function"> the operand being visited. </param>
        /// <returns> the value to return from the operand visit. </returns>
        T Visit(FunctionOperand function);

        /// <summary>
        /// The method called when visiting an <see cref="MultiValueOperand"/>.
        /// </summary>
        /// <param name="multiValue"> the operand being visited. </param>
        /// <returns> the value to return from the operand visit. </returns>
        T Visit(MultiValueOperand multiValue);


        /// <summary>
        /// The method called when visiting an <see cref="SingleValueOperand"/>.
        /// </summary>
        /// <param name="singleValueOperand"> the operand being visited. </param>
        /// <returns> the value to return from the operand visit. </returns>
        T Visit(SingleValueOperand singleValueOperand);
    }
}