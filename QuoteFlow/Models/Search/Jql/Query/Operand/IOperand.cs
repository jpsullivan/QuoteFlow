namespace QuoteFlow.Models.Search.Jql.Query.Operand
{
    /// <summary>
    /// Represents the right hand side value of a clause.
    /// </summary>
    public interface IOperand
    {
        /// <summary>
        /// The name that represents this Operand, null if the operand is unnamed.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Produces the unexpanded representation of the Operand. In the case of a function 
        /// operand this would be the function as represented in the Query (i.e. group(quoteflow-users)).
        /// </summary>
        string DisplayString { get; }

        /// <summary>
        /// Allows us to perform operations over the operand based on the passed in visitor. This calls the
        /// visit method on the visitor with this reference.
        /// </summary>
        T Accept<T>(IOperandVisitor<T> visitor);
    }
}