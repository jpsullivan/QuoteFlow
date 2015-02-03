namespace QuoteFlow.Api.Jql.Query.Operand
{
    /// <summary>
    /// Used to represent a value that has not been set for a field.
    /// </summary>
    public class EmptyOperand : IOperand
    {
        public const string OperandName = "EMPTY";
        public static readonly EmptyOperand Empty = new EmptyOperand();

        public virtual string Name { get { return OperandName; } }

        public virtual string DisplayString { get { return OperandName; } }

        public virtual T Accept<T>(IOperandVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }
            if (obj == null || this.GetType() != obj.GetType())
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return OperandName.GetHashCode();
        }
    }
}