using System;
using System.Collections.Generic;
using System.Linq;
using QuoteFlow.Api.Jql.Query.Operand;

namespace QuoteFlow.Core.Jql.Query.Operand
{
    /// <summary>
    /// Factory class for creating operands.
    /// </summary>
    public sealed class Operands
    {
        // Don't let people constuct me
        private Operands()
        {
        }

        /// <summary>
        /// Create an operand that represents the passed string.
        /// </summary>
        /// <param name="value"> the value to wrap as an operand. Cannot be null. </param>
        /// <returns> the operand that represents the passed value. </returns>
        public static IOperand ValueOf(string value)
        {
            return new SingleValueOperand(value);
        }

        /// <summary>
        /// Create an operands that represents a list of passed string values.
        /// </summary>
        /// <param name="values"> the list of values to represent. Cannot be null, empty or contain any null values. </param>
        /// <returns> the operand that represents the list of values. </returns>
        public static IOperand ValueOf(params string[] values)
        {
            return new MultiValueOperand(values);
        }

        /// <summary>
        /// Create an operands that represents a list of passed string values.
        /// </summary>
        /// <param name="values"> the list of values to represent. Cannot be null, empty or contain any null values. </param>
        /// <returns> the operand that represents the list of values. </returns>
        public static IOperand ValueOfStrings(ICollection<string> values)
        {
            if (values == null) throw new ArgumentNullException("values");
            return new MultiValueOperand(values.ToArray());
        }

        /// <summary>
        /// Create an operand that represents the passed number.
        /// </summary>
        /// <param name="value"> the value to wrap as an operand. Cannot be null. </param>
        /// <returns> the operand that represents the passed value. </returns>
        public static IOperand ValueOf(int? value)
        {
            return new SingleValueOperand(value);
        }

        /// <summary>
        /// Create an operands that represents a list of passed numbers.
        /// </summary>
        /// <param name="values">The list of values to represent. Cannot be null, empty or contain any null values. </param>
        /// <returns> the operand that represents the list of values. </returns>
        public static IOperand ValueOf(params int?[] values)
        {
            return new MultiValueOperand(values);
        }

        /// <summary>
        /// Create an operands that represents a list of passed numbers.
        /// </summary>
        /// <param name="values"> the list of values to represent. Cannot be null, empty or contain any null values. </param>
        /// <returns> the operand that represents the list of values. </returns>
        public static IOperand ValueOfNumbers(ICollection<int?> values)
        {
            if (values == null) throw new ArgumentNullException("values");
            return new MultiValueOperand(values.ToArray());
        }

        /// <summary>
        /// Create an operand that represents a list of the passed operands.
        /// </summary>
        /// <param name="operands"> the list of value to convert. Cannot be null, empty or contain any null values. </param>
        /// <returns> the operand that represents the list of operands. </returns>
        public static IOperand ValueOf(params IOperand[] operands)
        {
            return new MultiValueOperand(operands);
        }

        /// <summary>
        /// Create an operand that represents a list of the passed operands.
        /// </summary>
        /// <param name="operands"> the list of value to convert. Cannot be null, empty or contain any null values. </param>
        /// <returns> the operand that represents the list of operands. </returns>
        public static IOperand ValueOfOperands(IEnumerable<IOperand> operands)
        {
            return new MultiValueOperand(operands);
        }

        /// <summary>
        /// Return an operand that represents the JQL EMPTY value.
        /// </summary>
        /// <returns> the operand that represents the JQL EMPTY value. </returns>
        public IOperand IsEmpty()
        {
            return EmptyOperand.Empty;
        }
    }
}