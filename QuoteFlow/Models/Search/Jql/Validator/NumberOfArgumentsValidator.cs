using System;
using System.Collections.Generic;
using QuoteFlow.Infrastructure.Util;
using QuoteFlow.Models.Search.Jql.Query.Operand;

namespace QuoteFlow.Models.Search.Jql.Validator
{
    /// <summary>
    /// Simple Function Operand Validator that validates the number of arguments in the
    /// <seealso cref="FunctionOperand"/>.
    /// </summary>
    public class NumberOfArgumentsValidator
    {
        private readonly int minExpected;
        private readonly int maxExpected;

        /// <param name="expected"> the number of arguments expected. Input operands must have this exact number of arguments. Cannot be negative. </param>
        public NumberOfArgumentsValidator(int expected)
            : this(expected, expected)
        {
        }

        /// <param name="minExpected"> the minimum number of arguments expected (inclusive). Cannot be negative. </param>
        /// <param name="maxExpected"> the maximum number of arguments expected (inclusive). Cannot be negative. </param>
        public NumberOfArgumentsValidator(int minExpected, int maxExpected)
        {
            if (minExpected < 0 || maxExpected < 0)
            {
                throw new ArgumentException("expected args must not be negative");
            }
            if (minExpected > maxExpected)
            {
                throw new ArgumentException("Minimum number of args must be <= maximum number of args");
            }
            this.minExpected = minExpected;
            this.maxExpected = maxExpected;
        }

        /// <param name="operand"> the function operand to validate </param>
        /// <returns> a message set with errors if the number of arguments was not as expected, otherwise an empty message set. Never null. </returns>
        public virtual IMessageSet Validate(FunctionOperand operand)
        {
            IList<string> args = operand.Args;
            string name = operand.Name;
            var messages = new MessageSet();
            if (minExpected == maxExpected && args.Count != minExpected)
            {
                messages.AddErrorMessage(string.Format("QuoteFlow.jql.function.arg.incorrect.exact: {0}, {1}, {2}", name, minExpected, args.Count));
            }
            else if (minExpected > args.Count || maxExpected < args.Count)
            {
                messages.AddErrorMessage(string.Format("QuoteFlow.jql.function.arg.incorrect.range: {0}, {1}, {2}", name, minExpected, args.Count));
            }
            return messages;
        }
    }
}