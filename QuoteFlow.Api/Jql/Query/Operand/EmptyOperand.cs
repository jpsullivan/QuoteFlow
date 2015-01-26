namespace QuoteFlow.Api.Jql.Query.Operand
{
    /// <summary>
    /// Used to represent a value that has not been set for a field.
    /// </summary>
    public class EmptyOperand : IOperand
    {
        public const string OPERAND_NAME = "EMPTY";
        public static readonly EmptyOperand EMPTY = new EmptyOperand();

        public virtual string Name { get { return OPERAND_NAME; } }

        public virtual string DisplayString { get { return OPERAND_NAME; } }

        public virtual T Accept<T>(IOperandVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}