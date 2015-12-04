using System;
using QuoteFlow.Api.Jql.Operand;

namespace QuoteFlow.Api.Jql.Query.Operand
{
    public sealed class SingleValueOperand : IOperand
    {
        public const string OperandName = "SingleValueOperand";

        public SingleValueOperand(string stringValue)
        {
            if (stringValue == null) throw new ArgumentNullException(nameof(stringValue));

            StringValue = stringValue;
            IntValue = null;
            DecimalValue = null;
        }

        public SingleValueOperand(int? intValue)
        {
            IntValue = intValue;
            DecimalValue = null;
            StringValue = null;
        }

        public SingleValueOperand(decimal? decimalValue)
        {
            DecimalValue = decimalValue;
            IntValue = null;
            StringValue = null;
        }

        /// <summary>
        /// Note: cannot accept an empty <see cref="QueryLiteral"/>.
        /// Use <see cref="EmptyOperand"/> instead.
        /// </summary>
        /// <param name="literal">The query literal to convert to an operand; must not be null or empty.</param>
        public SingleValueOperand(QueryLiteral literal)
        {
            if (literal.IntValue != null)
            {
                IntValue = literal.IntValue;
                StringValue = null;
            }
            else if (literal.StringValue != null)
            {
                StringValue = literal.StringValue;
                IntValue = null;
            }
            else
            {
                throw new ArgumentException("QueryLiteral '" + literal + "' must contain at least one non-null value");
            }
        }

        public string Name => OperandName;

        public string DisplayString
        {
            get
            {
                if (IntValue == null)
                {
                    return "\"" + StringValue + "\"";
                }
                return IntValue.ToString();
            }
        }

        public T Accept<T>(IOperandVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public int? IntValue { get; }

        public decimal? DecimalValue { get; }

        public string StringValue { get; }

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var that = (SingleValueOperand) obj;

            if (!IntValue?.Equals(that.IntValue) ?? that.IntValue != null)
            {
                return false;
            }
            if (!StringValue?.Equals(that.StringValue) ?? that.StringValue != null)
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            int result = IntValue?.GetHashCode() ?? 0;
            result = 31 * result + (StringValue?.GetHashCode() ?? 0);
            return result;
        }

        public override string ToString()
        {
            return "Single Value Operand [" + DisplayString + "]";
        }
    }
}